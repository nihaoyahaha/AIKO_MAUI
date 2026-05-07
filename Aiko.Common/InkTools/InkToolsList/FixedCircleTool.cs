using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
{
    /// <summary>
    /// 固定圆
    /// </summary>
    public class FixedCircleTool : InkTool
    {
        public override string Type => "FixedCircle";

        public FixedCircleTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Green;
            Size = 30;
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

            canvas.DrawCircle(stroke.Points[0], stroke.Size, paint);
        }
    }
}