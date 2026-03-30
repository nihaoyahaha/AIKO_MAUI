using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools.InkToolsList
{
    /// <summary>
    /// 直线
    /// </summary>
    public class LineTool : InkTool
    {
        public override string Type => "Line";

        private SKPoint _startPoint;

        public LineTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Blue;
            Size = 3;
        }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            _startPoint = point;

            currentStroke = new InkStroke
            {
                Type = this.Type,
                Path = new SKPath(),
                Points = new List<SKPoint> { point, point }, // 初始占位，两个点重合
                Color = this.Color,
                Size = this.Size
            };

            UpdatePath(point);

            canvasView.InvalidateSurface();
        }
        public override void OnMove(SKPoint point, SKCanvasView canvasView)
        {
            if (currentStroke == null) return;

            // 更新终点坐标
            currentStroke.Points[1] = point;

            // 重新构建 Path（直线只需要起点和终点）
            UpdatePath(point);

            canvasView.InvalidateSurface();
        }
        public override void OnUp(SKPoint point, SKCanvasView canvasView)
        {
            if (currentStroke == null) return;

            _manager.AddStroke(currentStroke);
            currentStroke = null;

            canvasView.InvalidateSurface();
        }

        private void UpdatePath(SKPoint endPoint)
        {
            if (currentStroke == null) return;

            currentStroke.Path.Reset(); // 清除之前的临时线段
            currentStroke.Path.MoveTo(_startPoint);
            currentStroke.Path.LineTo(endPoint);
        }

        public override void Draw(SKCanvas canvas, InkStroke stroke)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = stroke.Color,
                StrokeWidth = stroke.Size,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

            canvas.DrawPath(stroke.Path, paint);
        }

        public override InkStroke Rebuild(InkStrokeDTO json)
        {
            var stroke = base.Rebuild(json);

            stroke.Path = new SKPath();
            if (stroke.Points != null && stroke.Points.Count >= 2)
            {
                stroke.Path.MoveTo(stroke.Points[0]);
                stroke.Path.LineTo(stroke.Points[1]);
            }

            return stroke;
        }
    }
}