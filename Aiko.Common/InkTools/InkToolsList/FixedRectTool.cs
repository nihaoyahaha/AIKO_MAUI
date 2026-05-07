using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
{
    /// <summary>
    /// 固定矩形
    /// </summary>
    public class FixedRectTool : InkTool
    {
        public override string Type => "FixedRect";

        public float Width { get; set; } = 20;
        public float Height { get; set; } = 10;

        public FixedRectTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Green;
        }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            currentStroke = new InkStroke
            {
                Type = this.Type,
                Path = new SKPath(),
                Points = new List<SKPoint> { point },
                Color = this.Color,
                Width = this.Width,
                Height = this.Height
            };

            _manager.AddStroke(currentStroke);
            currentStroke = null;

            canvasView.InvalidateSurface();
        }

        public override void OnMove(SKPoint point, SKCanvasView canvasView) { }

        public override void OnUp(SKPoint point, SKCanvasView canvasView) { }

        public override void Draw(SKCanvas canvas, InkStroke stroke)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = stroke.Color,
                IsAntialias = true
            };

            var rect = SKRect.Create(stroke.Points[0].X, stroke.Points[0].Y, stroke.Width, stroke.Height);

            canvas.DrawRect(rect, paint);
        }
    }
}