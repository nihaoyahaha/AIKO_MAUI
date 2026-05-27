using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

public enum InkStatusType { Idle, Drawing, Moving }

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

        public InkStatusType Status = InkStatusType.Idle;

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

            LoadToolPreferences();

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

        public T? GetTool<T>() where T : class, IInkTool
        {
            foreach (var tool in _tools.Values)
            {
                if (tool is T typedTool)
                {
                    return typedTool;
                }
            }

            return null;
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
        public void HandleCompletedDrawing(SKCanvas canvas, string[]? allowedTypes, string[]? bannedTypes)
        {
            foreach (var stroke in _completedStrokes)
            {
                if (stroke != null)
                {
                    HandleDrawing(canvas, stroke, allowedTypes, bannedTypes);
                }
            }
        }
        public void HandleBehindSelectedDrawing(SKCanvas canvas, string[]? allowedTypes, string[]? bannedTypes)
        {
            if (CurrentTool is not MoveTool moveTool) return;

            var selectedStroke = moveTool.GetSelectedStroke();

            if (selectedStroke == null) return;

            foreach (var stroke in _completedStrokes)
            {
                if (stroke == selectedStroke)
                {
                    break;
                }

                if (stroke != null)
                {
                    HandleDrawing(canvas, stroke, allowedTypes, bannedTypes);
                }
            }
        }
        public void HandleFrontSelectedDrawing(SKCanvas canvas, string[]? allowedTypes, string[]? bannedTypes)
        {
            if (CurrentTool is not MoveTool moveTool) return;

            var selectedStroke = moveTool.GetSelectedStroke();

            if (selectedStroke == null) return;

            bool front = false;
            foreach (var stroke in _completedStrokes)
            {
                if (!front)
                {
                    if (stroke == selectedStroke)
                    {
                        front = true;
                    }

                    continue;
                }

                if (stroke != null)
                {
                    HandleDrawing(canvas, stroke, allowedTypes, bannedTypes);
                }
            }
        }
        public void HandleCurrentDrawing(SKCanvas canvas, string[]? allowedTypes, string[]? bannedTypes)
        {
            var stroke = CurrentTool.GetCurrentTempStroke();

            if (stroke != null)
            {
                HandleDrawing(canvas, stroke, allowedTypes, bannedTypes);
            }

        }

        public void HandleDrawing(SKCanvas canvas, InkStroke stroke, string[]? allowedTypes, string[]? bannedTypes)
        {
            if (stroke == null) return;

            if ((allowedTypes == null || allowedTypes.Contains(stroke.Type)) && (bannedTypes == null || !bannedTypes.Contains(stroke.Type)))
            {
                _tools[stroke.Type].Draw(canvas, stroke);
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

        // --- 工具偏好相关 ---

        public void LoadToolPreferences()
        {
            var ballpointPenTool = _tools["BallpointPen"] as BallpointPenTool;
            var pencilTool = _tools["Pencil"] as PencilTool;
            var highlighterTool = _tools["Highlighter"] as HighlighterTool;
            var lineTool = _tools["Line"] as LineTool;
            var fixedRectTool = _tools["FixedRect"] as FixedRectTool;
            var fixedCircleTool = _tools["FixedCircle"] as FixedCircleTool;
            var textTool = _tools["Text"] as TextTool;
            var circleTextTool = _tools["CircleText"] as CircleTextTool;
            var rectTextTool = _tools["RectText"] as RectTextTool;
            var acceptTool = _tools["Accept"] as AcceptTool;

            string ballpointPenToolColor = Preferences.Get("BallpointPenTool_Color", "#000000");
            float ballpointPenToolSize = Preferences.Get("BallpointPenTool_Size", 1f);
            string pencilToolColor = Preferences.Get("PencilTool_Color", "#000000");
            float pencilToolSize = Preferences.Get("PencilTool_Size", 1f);
            string highlighterToolColor = Preferences.Get("HighlighterTool_Color", "#000000");
            float highlighterToolSize = Preferences.Get("HighlighterTool_Size", 12f);
            string lineToolColor = Preferences.Get("LineTool_Color", "#000000");
            float lineToolSize = Preferences.Get("LineTool_Size", 1f);
            string fixedRectToolColor = Preferences.Get("FixedRectTool_Color", "#000000");
            float fixedRectToolWidth = Preferences.Get("FixedRectTool_Width", 1f);
            float fixedRectToolHeight = Preferences.Get("FixedRectTool_Height", 1f);
            string fixedCircleToolColor = Preferences.Get("FixedCircleTool_Color", "#000000");
            float fixedCircleToolSize = Preferences.Get("FixedCircleTool_Size", 1f);
            string textToolColor = Preferences.Get("TextTool_Color", "#000000");
            string textToolFont = Preferences.Get("TextTool_Font", "ヒラギノ丸ゴ ProN");
            float textToolSize = Preferences.Get("TextTool_Size", 24f);
            string circleTextToolColor = Preferences.Get("CircleTextTool_Color", "#000000");
            string circleTextToolFont = Preferences.Get("CircleTextTool_Font", "ヒラギノ丸ゴ ProN");
            float circleTextToolSize = Preferences.Get("CircleTextTool_Size", 24f);
            string rectTextToolColor = Preferences.Get("RectTextTool_Color", "#000000");
            string rectTextToolFont = Preferences.Get("RectTextTool_Font", "ヒラギノ丸ゴ ProN");
            float rectTextToolSize = Preferences.Get("RectTextTool_Size", 24f);
            string acceptToolColor = Preferences.Get("AcceptTool_Color", "#000000");
            string acceptToolFont = Preferences.Get("AcceptTool_Font", "ヒラギノ丸ゴ ProN");
            float acceptToolSize = Preferences.Get("AcceptTool_Size", 24f);

            if (ballpointPenTool != null)
            {
                if (SKColor.TryParse(ballpointPenToolColor, out var color))
                {
                    ballpointPenTool.Color = color;
                }
                ballpointPenTool.Size = ballpointPenToolSize;
            }
            if (pencilTool != null)
            {
                if (SKColor.TryParse(pencilToolColor, out var color))
                {
                    pencilTool.Color = color;
                }
                pencilTool.Size = pencilToolSize;
            }
            if (highlighterTool != null)
            {
                if (SKColor.TryParse(highlighterToolColor, out var color))
                {
                    highlighterTool.Color = color;
                }
                highlighterTool.Size = highlighterToolSize;
            }
            if (lineTool != null)
            {
                if (SKColor.TryParse(lineToolColor, out var color))
                {
                    lineTool.Color = color;
                }
                lineTool.Size = lineToolSize;
            }
            if (fixedRectTool != null)
            {
                if (SKColor.TryParse(fixedRectToolColor, out var color))
                {
                    fixedRectTool.Color = color;
                }
                fixedRectTool.Width = fixedRectToolWidth;
                fixedRectTool.Height = fixedRectToolHeight;
            }
            if (fixedCircleTool != null)
            {
                if (SKColor.TryParse(fixedCircleToolColor, out var color))
                {
                    fixedCircleTool.Color = color;
                }
                fixedCircleTool.Size = fixedCircleToolSize;
            }
            if (textTool != null)
            {
                if (SKColor.TryParse(textToolColor, out var color))
                {
                    textTool.Color = color;
                }
                textTool.Font = textToolFont;
                textTool.Size = textToolSize;
            }
            if (circleTextTool != null)
            {
                if (SKColor.TryParse(circleTextToolColor, out var color))
                {
                    circleTextTool.Color = color;
                }
                circleTextTool.Font = circleTextToolFont;
                circleTextTool.Size = circleTextToolSize;
            }
            if (rectTextTool != null)
            {
                if (SKColor.TryParse(rectTextToolColor, out var color))
                {
                    rectTextTool.Color = color;
                }
                rectTextTool.Font = rectTextToolFont;
                rectTextTool.Size = rectTextToolSize;
            }
            if (acceptTool != null)
            {
                if (SKColor.TryParse(acceptToolColor, out var color))
                {
                    acceptTool.Color = color;
                }
                acceptTool.Font = acceptToolFont;
                acceptTool.Size = acceptToolSize;
            }
        }
        public void SaveToolPreferences()
        {
            var ballpointPenTool = _tools["BallpointPen"] as BallpointPenTool;
            var pencilTool = _tools["Pencil"] as PencilTool;
            var highlighterTool = _tools["Highlighter"] as HighlighterTool;
            var lineTool = _tools["Line"] as LineTool;
            var fixedRectTool = _tools["FixedRect"] as FixedRectTool;
            var fixedCircleTool = _tools["FixedCircle"] as FixedCircleTool;
            var textTool = _tools["Text"] as TextTool;
            var circleTextTool = _tools["CircleText"] as CircleTextTool;
            var rectTextTool = _tools["RectText"] as RectTextTool;
            var acceptTool = _tools["Accept"] as AcceptTool;

            if (ballpointPenTool != null)
            {
                Preferences.Set("BallpointPenTool_Color", ballpointPenTool.Color.ToString());
                Preferences.Set("BallpointPenTool_Size", ballpointPenTool.Size);
            }
            if (pencilTool != null)
            {
                Preferences.Set("PencilTool_Color", pencilTool.Color.ToString());
                Preferences.Set("PencilTool_Size", pencilTool.Size);
            }
            if (highlighterTool != null)
            {
                Preferences.Set("HighlighterTool_Color", highlighterTool.Color.ToString());
                Preferences.Set("HighlighterTool_Size", highlighterTool.Size);
            }
            if (lineTool != null)
            {
                Preferences.Set("LineTool_Color", lineTool.Color.ToString());
                Preferences.Set("LineTool_Size", lineTool.Size);
            }
            if (fixedRectTool != null)
            {
                Preferences.Set("FixedRectTool_Color", fixedRectTool.Color.ToString());
                Preferences.Set("FixedRectTool_Width", fixedRectTool.Width);
                Preferences.Set("FixedRectTool_Height", fixedRectTool.Height);
            }
            if (fixedCircleTool != null)
            {
                Preferences.Set("FixedCircleTool_Color", fixedCircleTool.Color.ToString());
                Preferences.Set("FixedCircleTool_Size", fixedCircleTool.Size);
            }
            if (textTool != null)
            {
                Preferences.Set("TextTool_Color", textTool.Color.ToString());
                Preferences.Set("TextTool_Font", textTool.Font);
                Preferences.Set("TextTool_Size", textTool.Size);
            }
            if (circleTextTool != null)
            {
                Preferences.Set("CircleTextTool_Color", circleTextTool.Color.ToString());
                Preferences.Set("CircleTextTool_Font", circleTextTool.Font);
                Preferences.Set("CircleTextTool_Size", circleTextTool.Size);
            }
            if (rectTextTool != null)
            {
                Preferences.Set("RectTextTool_Color", rectTextTool.Color.ToString());
                Preferences.Set("RectTextTool_Font", rectTextTool.Font);
                Preferences.Set("RectTextTool_Size", rectTextTool.Size);
            }
            if (acceptTool != null)
            {
                Preferences.Set("AcceptTool_Color", acceptTool.Color.ToString());
                Preferences.Set("AcceptTool_Font", acceptTool.Font);
                Preferences.Set("AcceptTool_Size", acceptTool.Size);
            }
        }
    }
}