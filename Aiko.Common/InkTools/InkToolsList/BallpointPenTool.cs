using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools.InkToolsList
{
    /// <summary>
    /// 圆珠笔
    /// </summary>
    public class BallpointPenTool : InkTool
    {
        public override string Type => "BallpointPen";

        public BallpointPenTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Red;
            Size = 3;
        }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            currentStroke = new InkStroke
            {
                Type = this.Type,
                Path = new SKPath(),
                Points = new List<SKPoint> { point },
                Color = this.Color,
                Size = this.Size
            };
            currentStroke.Path.MoveTo(point);

            canvasView.InvalidateSurface();
        }
        public override void OnMove(SKPoint point, SKCanvasView canvasView)
        {
            if (currentStroke == null) return;

            currentStroke.Points.Add(point);
            currentStroke.Path.LineTo(point);

            canvasView.InvalidateSurface();
        }
        public override void OnUp(SKPoint point, SKCanvasView canvasView)
        {
            if (currentStroke == null) return;

            _manager.AddStroke(currentStroke);
            currentStroke = null;

            canvasView.InvalidateSurface();
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
            if (stroke.Points.Count > 0)
            {
                stroke.Path.MoveTo(stroke.Points[0]);
                for (int i = 1; i < stroke.Points.Count; i++)
                    stroke.Path.LineTo(stroke.Points[i]);
            }

            return stroke;
        }
    }
}