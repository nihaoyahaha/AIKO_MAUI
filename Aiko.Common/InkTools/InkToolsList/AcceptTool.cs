using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
{
    /// <summary>
    /// 确认符号
    /// </summary>
    public class AcceptTool : TextTool
    {
        public override string Type => "Accept";

        public string AcceptSymbol { get; } = "✓";

        public AcceptTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Green;
            Size = 36;
        }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            currentStroke = new InkStroke
            {
                Type = this.Type,
                Points = new List<SKPoint> { point },
                Color = this.Color,
                Size = this.Size,
                Text = this.AcceptSymbol,
                Font = this.Font
            };

            _manager.AddStroke(currentStroke);
            currentStroke = null;

            canvasView.InvalidateSurface();
        }

        public override void Draw(SKCanvas canvas, InkStroke stroke)
        {
            if (string.IsNullOrEmpty(stroke.Text)) return;

            UpdateTextBounds(stroke);

            using var font = new SKFont
            {
                Typeface = Typefaces.TryGetValue(stroke.Font, out SKTypeface? value) ? value : SKTypeface.Default,
                Size = stroke.Size,
                Edging = SKFontEdging.Antialias
            };
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = stroke.Color,
                IsAntialias = true
            };

            float x = stroke.TextBounds.Left;
            float y = stroke.Points[0].Y + (stroke.Size * 0.35f);

            canvas.DrawText(stroke.Text, x, y, font, paint);
        }

        protected override void UpdateTextBounds(InkStroke stroke)
        {
            if (string.IsNullOrEmpty(stroke.Text) || stroke.Points.Count == 0) return;

            using var font = new SKFont
            {
                Typeface = Typefaces.TryGetValue(stroke.Font, out SKTypeface? value) ? value : SKTypeface.Default,
                Size = stroke.Size,
                Edging = SKFontEdging.Antialias
            };
            using var paint = new SKPaint();

            var position = stroke.Points[0];
            float width = font.MeasureText(stroke.Text, paint);

            float x = position.X - (width / 2);
            float halfHeight = stroke.Size / 2;

            stroke.TextBounds = new SKRect(
                x,
                position.Y - halfHeight,
                x + width,
                position.Y + halfHeight
            );
        }
    }
}