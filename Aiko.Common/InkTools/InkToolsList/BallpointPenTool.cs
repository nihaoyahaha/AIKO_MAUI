using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
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
            _manager.Status = InkStatusType.Drawing;

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
            _manager.Status = InkStatusType.Idle;

            if (currentStroke == null) return;

            // 抬笔点和最后一个移动点距离足够大时，补上最终点，避免路径尾部缺口
            if (currentStroke.Points.Count == 0 || Distance(currentStroke.Points[^1], point) > 0.5f)
            {
                currentStroke.Points.Add(point);
            }

            currentStroke.Points = InkService.SmoothLine(currentStroke.Points);

            // 抬笔后尝试识别闭合图形；识别成功则把手绘轨迹替换为规则化图形
            if (!TryConvertToRecognizedShape(currentStroke))
            {
                currentStroke.Path?.Dispose();
                currentStroke.Path = BuildPath(currentStroke.Points);
            }

            _manager.AddStroke(currentStroke);
            currentStroke = null;

            canvasView.InvalidateSurface();
        }

        /// <summary>
        /// 尝试识别并替换当前笔迹为规则化图形
        /// </summary>
        private static bool TryConvertToRecognizedShape(InkStroke stroke)
        {
            var result = InkShapeRecognizer.Recognize(stroke.Points);
            if (result == null) return false;

            stroke.Points = result.Points.ToList();
            stroke.Path?.Dispose();
            stroke.Path = BuildPath(stroke.Points);
            return true;
        }

        /// <summary>
        /// 根据点集合构建 Skia 折线路径
        /// </summary>
        private static SKPath BuildPath(IReadOnlyList<SKPoint> points)
        {
            // 识别器返回的是规则化点集，这里统一按折线路径重建 SKPath
            var path = new SKPath();
            if (points.Count == 0) return path;

            path.MoveTo(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(points[i]);
            }

            return path;
        }

        /// <summary>
        /// 计算两个点之间的直线距离
        /// </summary>
        private static float Distance(SKPoint first, SKPoint second)
        {
            float dx = second.X - first.X;
            float dy = second.Y - first.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
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

            stroke.Path = BuildPath(stroke.Points);

            return stroke;
        }
    }
}
