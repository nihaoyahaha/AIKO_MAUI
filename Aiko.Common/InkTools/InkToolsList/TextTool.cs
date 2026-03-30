using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools.InkToolsList
{
    /// <summary>
    /// 文本工具
    /// </summary>
    public class TextTool : InkTool
    {
        public override string Type => "Text";
        public string Font { get; set; } = "Microsoft YaHei";

        public TextTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Black;
            Size = 16;

            if (manager != null)
            {
                TextEditRequested += manager.OnTextEditRequested;
            }
        }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            TextEditRequest(point, Color, Size, Font);
        }
        public override void OnMove(SKPoint point, SKCanvasView canvasView) { }
        public override void OnUp(SKPoint point, SKCanvasView canvasView) { }

        public override void Draw(SKCanvas canvas, InkStroke stroke)
        {
            if (string.IsNullOrEmpty(stroke.Text)) return;

            UpdateTextBounds(stroke);

            using var typeface = SKTypeface.FromFamilyName(stroke.Font);
            using var font = new SKFont(typeface, stroke.Size);
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = stroke.Color,
                IsAntialias = true
            };

            DrawMultiLineText(canvas, stroke, font, paint);
        }

        protected virtual void UpdateTextBounds(InkStroke stroke)
        {
            if (string.IsNullOrEmpty(stroke.Text) || stroke.Points.Count == 0) return;

            using var typeface = SKTypeface.FromFamilyName(stroke.Font);
            using var font = new SKFont(typeface, stroke.Size);
            using var paint = new SKPaint();

            var position = stroke.Points[0];
            string[] lines = stroke.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            float lineHeight = stroke.Size * 1.2f;
            float maxWidth = 0;

            foreach (var line in lines)
            {
                float width = font.MeasureText(line, paint);
                if (width > maxWidth) maxWidth = width;
            }

            stroke.TextBounds = new SKRect(
                position.X,
                position.Y,
                position.X + maxWidth,
                position.Y + (lines.Length * lineHeight) + (stroke.Size * 0.2f)
            );
        }

        protected void DrawMultiLineText(SKCanvas canvas, InkStroke stroke, SKFont font, SKPaint paint)
        {
            var position = stroke.Points[0];
            string[] lines = stroke.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            float lineHeight = stroke.Size * 1.2f;

            for (int i = 0; i < lines.Length; i++)
            {
                float y = position.Y + stroke.Size + (i * lineHeight);
                canvas.DrawText(lines[i], position.X, y, font, paint);
            }
        }

        public event Action<SKPoint, SKColor, float, string>? TextEditRequested;

        protected void TextEditRequest(SKPoint point, SKColor color, float size, string font)
        {
            TextEditRequested?.Invoke(point, color, size, font);
        }
    }
}