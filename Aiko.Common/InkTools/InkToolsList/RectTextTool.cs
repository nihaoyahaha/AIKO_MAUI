using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools.InkToolsList
{
    /// <summary>
    /// 矩形文本工具
    /// </summary>
    public class RectTextTool : TextTool
    {
        public override string Type => "RectText";

        public RectTextTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Black;
            Size = 16;
        }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            TextEditRequest(point, SKColors.White, Size, Font, Color);
        }

        public override void Draw(SKCanvas canvas, InkStroke stroke)
        {
            if (string.IsNullOrEmpty(stroke.Text)) return;

            UpdateTextBounds(stroke);

            using var bgPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = stroke.Color,
                IsAntialias = true
            };
            var rect = stroke.TextBounds;
            rect.Inflate(5, 5);

            canvas.DrawRect(rect, bgPaint);

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