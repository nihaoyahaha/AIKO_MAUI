using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
{
    /// <summary>
    /// 移动工具
    /// </summary>
    public class MoveTool : InkTool
    {
        public override string Type => "Move";

        private InkStroke? _selectedStroke;
        private SKPoint _lastPointer;
        private SKPoint _cumulativeOffset; // 本次操作的总偏移量

        public MoveTool(InkToolManager manager) : base(manager) { }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            _manager.Status = InkStatusType.Moving;

            _selectedStroke = HitTest(point);
            if (_selectedStroke == null) return;

            _lastPointer = point;
            _cumulativeOffset = new SKPoint(0, 0); // 重置累计偏移

            canvasView.InvalidateSurface();
        }
        public override void OnMove(SKPoint point, SKCanvasView canvasView)
        {
            if (_selectedStroke == null) return;

            var delta = point - _lastPointer;
            _cumulativeOffset += delta; // 累加偏移量

            TranslateStroke(_selectedStroke, delta);
            _lastPointer = point;

            canvasView.InvalidateSurface();
        }
        public override void OnUp(SKPoint point, SKCanvasView canvasView)
        {
            _manager.Status = InkStatusType.Idle;

            if (_selectedStroke == null) return;

            _manager.CommitMove(_selectedStroke, _cumulativeOffset);
            currentStroke = null;
            _selectedStroke = null;

            canvasView.InvalidateSurface();
        }

        public override void Draw(SKCanvas canvas, InkStroke stroke) { }

        public InkStroke? GetSelectedStroke() => _selectedStroke;

        public void TranslateStroke(InkStroke stroke, SKPoint offset)
        {
            for (int i = 0; i < stroke.Points.Count; i++)
            {
                stroke.Points[i] = new SKPoint(stroke.Points[i].X + offset.X, stroke.Points[i].Y + offset.Y);
            }

            if (stroke.Path != null)
            {
                var matrix = SKMatrix.CreateTranslation(offset.X, offset.Y);
                stroke.Path.Transform(matrix);
            }

            if (stroke.Type == "Text")
            {
                var rect = stroke.TextBounds;
                rect.Offset(offset.X, offset.Y);
                stroke.TextBounds = rect;
            }
        }

        // --- 命中测试 ---

        protected InkStroke? HitTest(SKPoint point)
        {
            // 从后往前找，优先选中最上层的
            for (int i = _manager.CompletedStrokes.Count - 1; i >= 0; i--)
            {
                var stroke = _manager.CompletedStrokes[i];
                if (IsPointOnStroke(stroke, point)) return stroke;
            }
            return null;
        }

        private bool IsPointOnStroke(InkStroke stroke, SKPoint point)
        {
            // 固定矩形命中
            if (stroke.Type == "FixedRect")
            {
                if (stroke.Points != null && stroke.Points.Count > 0)
                {
                    var rect = SKRect.Create(stroke.Points[0].X, stroke.Points[0].Y, stroke.Width, stroke.Height);
                    rect.Inflate(5, 5);

                    return rect.Contains(point);
                }
                return false;
            }

            // 固定圆命中
            if (stroke.Type == "FixedCircle")
            {
                if (stroke.Points != null && stroke.Points.Count > 0)
                {
                    float distance = SKPoint.Distance(stroke.Points[0], point);

                    return distance <= (stroke.Size + 5);
                }
                return false;
            }

            // 文本命中
            if (InkToolConfig.IsFont(stroke.Type))
            {
                var rect = stroke.TextBounds;
                var hitArea = new SKRect(
                    rect.Left - 10,
                    rect.Top - 10,
                    rect.Right + 10,
                    rect.Bottom + 10);

                return hitArea.Contains(point);
            }

            // 线条命中
            float threshold = stroke.Size + 5;
            for (int i = 0; i < stroke.Points.Count - 1; i++)
            {
                if (DistancePointToSegment(point, stroke.Points[i], stroke.Points[i + 1]) <= threshold)
                    return true;
            }
            return false;
        }

        private float DistancePointToSegment(SKPoint p, SKPoint a, SKPoint b)
        {
            var px = b.X - a.X;
            var py = b.Y - a.Y;
            var norm = px * px + py * py;
            if (norm == 0) return SKPoint.Distance(p, a);
            var u = ((p.X - a.X) * px + (p.Y - a.Y) * py) / norm;
            u = Math.Clamp(u, 0, 1);
            return SKPoint.Distance(p, new SKPoint(a.X + u * px, a.Y + u * py));
        }
    }
}