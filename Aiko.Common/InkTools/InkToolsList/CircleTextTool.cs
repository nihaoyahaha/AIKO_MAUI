using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace Aiko.Common.InkTools
{
    /// <summary>
    /// 圆形文本工具
    /// </summary>
    public class CircleTextTool : TextTool
    {
        public override string Type => "CircleText";

        public CircleTextTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Black;
            Size = 16;
        }

        //public override void OnDown(SKPoint point, SKCanvasView canvasView)
        //{
        //    TextEditRequest(point, SKColors.White, Size, Font, Color);
        //}

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            currentStroke = new InkStroke
            {
                Type = this.Type,
                Points = new List<SKPoint> { point },
                Color = this.Color,
                Size = this.Size,
                Text = this.Text,
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

            var bounds = stroke.TextBounds;
            float centerX = bounds.MidX;
            float centerY = bounds.MidY;

            float radius = (float)Math.Sqrt(Math.Pow(bounds.Width / 2, 2) + Math.Pow(bounds.Height / 2, 2));
            radius += 8;

            using var bgPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = stroke.Color,
                IsAntialias = true
            };
            canvas.DrawCircle(centerX, centerY, radius, bgPaint);

            using var font = new SKFont
            {
                Typeface = Typefaces.TryGetValue(stroke.Font, out SKTypeface? value) ? value : SKTypeface.Default,
                Size = stroke.Size,
                Edging = SKFontEdging.Antialias
            };
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White,
                IsAntialias = true
            };

            DrawMultiLineText(canvas, stroke, font, paint);
        }
    }
}