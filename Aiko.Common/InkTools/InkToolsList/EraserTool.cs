using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
{
    /// <summary>
    /// 橡皮擦
    /// </summary>
    public class EraserTool : MoveTool
    {
        public override string Type => "Eraser";

        public EraserTool(InkToolManager manager) : base(manager) { }

        public override void OnDown(SKPoint point, SKCanvasView canvasView)
        {
            TryErase(point, canvasView);
        }
        public override void OnMove(SKPoint point, SKCanvasView canvasView)
        {
            TryErase(point, canvasView);
        }
        public override void OnUp(SKPoint point, SKCanvasView canvasView) { }

        private void TryErase(SKPoint point, SKCanvasView canvasView)
        {
            var target = HitTest(point);
            if (target == null) return;

            _manager.RemoveStroke(target);

            canvasView.InvalidateSurface();
        }

        public override void Draw(SKCanvas canvas, InkStroke stroke) { }
    }
}