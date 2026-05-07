using SkiaSharp;

namespace Aiko.Common.InkTools
{
    /// <summary>
    /// 铅笔
    /// </summary>
    public class PencilTool : BallpointPenTool
    {
        public override string Type => "Pencil";

        // 设置模糊效果，提前创建好 Filter，避免 Draw 时重复创建
        protected SKMaskFilter filter { get; set; } = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 3.0f);

        public PencilTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Gray;
            Size = 6;
        }

        public override void Draw(SKCanvas canvas, InkStroke stroke)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = stroke.Color.WithAlpha(200), // 铅笔的透明度
                StrokeWidth = stroke.Size,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                MaskFilter = filter
            };

            canvas.DrawPath(stroke.Path, paint);
        }
    }
}