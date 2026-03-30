using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools.InkToolsList
{
    /// <summary>
    /// 荧光笔
    /// </summary>
    public class HighlighterTool : InkTool
    {
        public override string Type => "Highlighter";

        public HighlighterTool(InkToolManager manager) : base(manager)
        {
            Color = SKColors.Yellow;
            Size = 24;
        }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            currentStroke = new InkStroke
            {
                Type = this.Type,
                Color = this.Color,
                Size = this.Size,
                Points = new List<SKPoint> { point }
            };

            canvasView.InvalidateSurface();
        }
        public override void OnMove(SKPoint point, SKCanvasView canvasView)
        {
            if (currentStroke == null) return;

            currentStroke.Points.Add(point);

            canvasView.InvalidateSurface();
        }
        public override void OnUp(SKPoint point, SKCanvasView canvasView)
        {
            if (currentStroke == null) return;

            if (currentStroke.Points.Count >= 2)
            {
                BuildPolygonPath(currentStroke);
                _manager.AddStroke(currentStroke);
            }
            currentStroke = null;

            canvasView.InvalidateSurface();
        }

        public override void Draw(SKCanvas canvas, InkStroke stroke)
        {
            if (stroke.Points == null || stroke.Points.Count < 2) return;

            using var paint = CreateHighlighterPaint(stroke.Color);
            float halfWidth = stroke.Size / 2;

            for (int i = 0; i < stroke.Points.Count - 1; i++)
            {
                DrawSegmentAsRect(canvas, stroke.Points[i], stroke.Points[i + 1], halfWidth, paint);
            }
        }

        private void DrawSegmentAsRect(SKCanvas canvas, SKPoint p1, SKPoint p2, float halfWidth, SKPaint paint)
        {
            using var rectPath = new SKPath();
            rectPath.MoveTo(p1.X, p1.Y - halfWidth);
            rectPath.LineTo(p2.X, p2.Y - halfWidth);
            rectPath.LineTo(p2.X, p2.Y + halfWidth);
            rectPath.LineTo(p1.X, p1.Y + halfWidth);
            rectPath.Close();

            canvas.DrawPath(rectPath, paint);
        }

        private SKPaint CreateHighlighterPaint(SKColor color)
        {
            return new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color.WithAlpha(128),
                BlendMode = SKBlendMode.Darken,
                IsAntialias = true
            };
        }

        private void BuildPolygonPath(InkStroke stroke)
        {
            float halfWidth = stroke.Size / 2;
            var finalPath = new SKPath();

            for (int i = 0; i < stroke.Points.Count - 1; i++)
            {
                var p1 = stroke.Points[i];
                var p2 = stroke.Points[i + 1];

                finalPath.MoveTo(p1.X, p1.Y - halfWidth);
                finalPath.LineTo(p2.X, p2.Y - halfWidth);
                finalPath.LineTo(p2.X, p2.Y + halfWidth);
                finalPath.LineTo(p1.X, p1.Y + halfWidth);
                finalPath.Close();
            }

            stroke.Path = finalPath;
        }

        public override InkStroke Rebuild(InkStrokeDTO json)
        {
            var stroke = base.Rebuild(json);

            BuildPolygonPath(stroke);

            return stroke;
        }
    }
}