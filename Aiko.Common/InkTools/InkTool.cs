using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
{
    public abstract class InkTool : IInkTool
    {
        public abstract string Type { get; }

        public SKColor Color { get; set; } = SKColors.Black;
        public float Size { get; set; } = 1;

        protected InkStroke? currentStroke;

        protected readonly InkToolManager? _manager;

        public InkTool(InkToolManager? manager) => _manager = manager;

        public abstract void OnDown(SKPoint point, SKCanvasView canvasView);
        public abstract void OnMove(SKPoint point, SKCanvasView canvasView);
        public abstract void OnUp(SKPoint point, SKCanvasView canvasView);

        public abstract void Draw(SKCanvas canvas, InkStroke stroke);

        public virtual InkStroke? GetCurrentTempStroke() => currentStroke;

        public virtual InkStroke Rebuild(InkStrokeDTO json)
        {
            return new InkStroke
            {
                Id = Guid.TryParse(json.Id, out var guid) ? guid : Guid.NewGuid(),
                Type = json.Type,
                Points = json.Points?.Select(point => new SKPoint(point.X, point.Y)).ToList() ?? new List<SKPoint>(),
                Color = SKColor.Parse(json.Color),
                Size = json.Size,
                Width = json.Width,
                Height = json.Height,
                Text = json.Text,
                Font = json.Font
            };
        }
    }
}