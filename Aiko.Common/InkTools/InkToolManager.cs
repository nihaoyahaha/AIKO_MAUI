using Aiko.Common.InkTools.InkToolsList;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
{
    public class InkToolManager
    {
        // 存储所有可用工具的字典
        private Dictionary<string, IInkTool> _tools;

        public IInkTool CurrentTool { get; private set; }

        public bool EditingMode => CurrentTool.Type != "Empty";

        // 全局笔迹数据
        private List<InkStroke> _completedStrokes = new List<InkStroke>();
        private List<InkStroke> _removedStrokes = new List<InkStroke>();
        private Stack<InkAction> _historyStack = new Stack<InkAction>();

        public IReadOnlyList<InkStroke> CompletedStrokes => _completedStrokes;

        private bool _cleared = false;

        public bool StrokesChanged => _historyStack.Count > 0 || _cleared;

        public InkToolManager()
        {
            _tools = new Dictionary<string, IInkTool>
            {
                { "Empty", new EmptyPenTool(this) },
                { "BallpointPen", new BallpointPenTool(this) },
                { "Pencil", new PencilTool(this) },
                { "Highlighter", new HighlighterTool(this) },
                { "Line", new LineTool(this) },
                { "FixedRect", new FixedRectTool(this) },
                { "FixedCircle", new FixedCircleTool(this) },
                { "Text", new TextTool(this) },
                { "CircleText", new CircleTextTool(this) },
                { "RectText", new RectTextTool(this) },
                { "Accept", new AcceptTool(this) },
                { "Eraser", new EraserTool(this) },
                { "Move", new MoveTool(this) }
            };

            SwitchTool();
        }

        // 切换工具
        public void SwitchTool()
        {
            CurrentTool = _tools["Empty"];
        }
        public void SwitchTool(string type)
        {
            if (_tools.ContainsKey(type))
            {
                CurrentTool = _tools[type];
            }
        }

        public void LoadTypefaces(Dictionary<string, SKTypeface> typefaces)
        {
            foreach (var tool in _tools.Values)
            {
                if (tool is TextTool textTool)
                {
                    textTool.Typefaces = typefaces;
                }
            }
        }

        // 分发触摸事件
        public void HandleTouch(SKTouchEventArgs e, SKCanvasView canvasView, string[]? allowedTypes, string[]? bannedTypes)
        {
            if (CurrentTool == null) return;

            if (!((allowedTypes == null || allowedTypes.Contains(CurrentTool.Type)) && (bannedTypes == null || !bannedTypes.Contains(CurrentTool.Type)))) return;

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    CurrentTool.OnDown(e.Location, canvasView);
                    break;
                case SKTouchAction.Moved:
                    if (e.InContact)
                        CurrentTool.OnMove(e.Location, canvasView);
                    break;
                case SKTouchAction.Released:
                case SKTouchAction.Cancelled:
                    CurrentTool.OnUp(e.Location, canvasView);
                    break;
            }
            e.Handled = true;
        }

        // 分发绘制事件
        public void HandleDrawing(SKCanvas canvas, string[]? allowedTypes, string[]? bannedTypes)
        {
            // 绘制所有已完成的笔迹
            foreach (var stroke in _completedStrokes)
            {
                if (_tools.ContainsKey(stroke.Type) && (allowedTypes == null || allowedTypes.Contains(stroke.Type)) && (bannedTypes == null || !bannedTypes.Contains(stroke.Type)))
                {
                    IInkTool renderer = _tools[stroke.Type];
                    renderer.Draw(canvas, stroke);
                }
            }

            // 绘制当前正在拖动的笔迹（临时）
            if (CurrentTool != null)
            {
                var tempStroke = CurrentTool.GetCurrentTempStroke();
                if (tempStroke != null && (allowedTypes == null || allowedTypes.Contains(CurrentTool.Type)) && (bannedTypes == null || !bannedTypes.Contains(CurrentTool.Type)))
                {
                    CurrentTool.Draw(canvas, tempStroke);
                }
            }
        }

        /// <summary>
        /// 添加笔迹
        /// </summary>
        public void AddStroke(InkStroke stroke)
        {
            if (stroke == null) return;

            _completedStrokes.Add(stroke);

            _historyStack.Push(new InkAction
            {
                Type = InkActionType.Add,
                TargetIds = new List<Guid> { stroke.Id }
            });
        }

        /// <summary>
        /// 移除笔迹
        /// </summary>
        public void RemoveStroke(InkStroke stroke)
        {
            if (stroke == null || !_completedStrokes.Contains(stroke)) return;

            _completedStrokes.Remove(stroke);
            _removedStrokes.Add(stroke);

            _historyStack.Push(new InkAction
            {
                Type = InkActionType.Erase,
                TargetIds = new List<Guid> { stroke.Id }
            });
        }

        /// <summary>
        /// 提交移动动作
        /// </summary>
        public void CommitMove(InkStroke stroke, SKPoint offset)
        {
            if (stroke == null || (offset.X == 0 && offset.Y == 0)) return;

            _historyStack.Push(new InkAction
            {
                Type = InkActionType.Move,
                TargetIds = new List<Guid> { stroke.Id },
                Offset = offset
            });
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            if (_historyStack.Count == 0) return;

            var lastAction = _historyStack.Pop();

            switch (lastAction.Type)
            {
                case InkActionType.Add:
                    _completedStrokes.Where(stroke => lastAction.TargetIds.Contains(stroke.Id)).ToList().ForEach(stroke => stroke.Dispose());
                    _completedStrokes.RemoveAll(stroke => lastAction.TargetIds.Contains(stroke.Id));
                    break;

                case InkActionType.Erase:
                    var recovered = _removedStrokes.Where(stroke => lastAction.TargetIds.Contains(stroke.Id)).ToList();
                    foreach (var stroke in recovered)
                    {
                        _completedStrokes.Add(stroke);
                        _removedStrokes.Remove(stroke);
                    }
                    break;

                case InkActionType.Move:
                    var reverseOffset = new SKPoint(-lastAction.Offset.X, -lastAction.Offset.Y);
                    var targets = _completedStrokes.Where(stroke => lastAction.TargetIds.Contains(stroke.Id));
                    foreach (var stroke in targets)
                    {
                        (_tools["Move"] as MoveTool).TranslateStroke(stroke, reverseOffset);
                    }
                    break;
            }
        }

        /// <summary>
        /// 清空笔迹
        /// </summary>
        public void ClearStrokes()
        {
            if (_completedStrokes.Count == 0) return;

            foreach (var stroke in _completedStrokes) stroke?.Dispose();
            _completedStrokes.Clear();
            foreach (var stroke in _removedStrokes) stroke?.Dispose();
            _removedStrokes.Clear();
            _historyStack.Clear();

            _cleared = true;
        }

        public void ResetStrokesChange()
        {
            foreach (var stroke in _removedStrokes) stroke?.Dispose();
            _removedStrokes.Clear();
            _historyStack.Clear();

            _cleared = false;
        }

        // --- 文本编辑事件 ---

        public event Action<SKPoint, SKColor, float, string, SKColor?>? TextEditRequested;
        public void OnTextEditRequested(SKPoint position, SKColor color, float size, string font, SKColor? bgColor)
        {
            TextEditRequested?.Invoke(position, color, size, font, bgColor);
        }
        public void AddTextStroke(string type, string text, SKPoint position, SKColor color, float size, string font)
        {
            var textStroke = new InkStroke
            {
                Type = type,
                Points = new List<SKPoint> { position },
                Color = color,
                Size = size,
                Text = text,
                Font = font,
            };

            _completedStrokes.Add(textStroke);

            _historyStack.Push(new InkAction
            {
                Type = InkActionType.Add,
                TargetIds = new List<Guid> { textStroke.Id }
            });
        }

        // --- 导入导出相关 ---

        public void LoadStrokes(List<InkStroke> strokes)
        {
            foreach (var stroke in _completedStrokes) stroke?.Dispose();
            _completedStrokes.Clear();
            _completedStrokes = strokes.Select(stroke => stroke.Clone()).ToList();

            ResetStrokesChange();
        }
    }
}