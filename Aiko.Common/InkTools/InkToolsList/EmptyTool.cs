using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools.InkToolsList
{
    public class EmptyPenTool : InkTool
    {
        public override string Type => "Empty";

        public EmptyPenTool(InkToolManager manager) : base(manager) { }

        public override void OnDown(SKPoint point, SKCanvasView canvasView) { }
        public override void OnMove(SKPoint point, SKCanvasView canvasView) { }
        public override void OnUp(SKPoint point, SKCanvasView canvasView) { }

        public override void Draw(SKCanvas canvas, InkStroke stroke) { }
    }
}