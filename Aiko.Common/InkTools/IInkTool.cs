using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Aiko.Common.InkTools
{
    public interface IInkTool
    {
        string Type { get; }

        SKColor Color { get; set; }
        float Size { get; set; }

        void OnDown(SKPoint point, SKCanvasView canvasView);
        void OnMove(SKPoint point, SKCanvasView canvasView);
        void OnUp(SKPoint point, SKCanvasView canvasView);

        void Draw(SKCanvas canvas, InkStroke stroke);

        // 获取当前正在绘制的临时笔迹（用于 OnPaintSurface 渲染）
        InkStroke? GetCurrentTempStroke();

        // 用于将 JSON 解析出来的 InkStrokeDTO 重建为 InkStroke
        InkStroke Rebuild(InkStrokeDTO json);
    }
}