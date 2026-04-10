using Aiko.UI.ViewModels.PageVMs;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Globalization;

namespace Aiko.UI;

public partial class MapViewPage : ContentPage
{
    // 当前底图实际显示尺寸（根据容器与原图比例计算）
    private double showW = 0;
    private double showH = 0;

    // 底图原始像素尺寸
    private double originalWidth = 0;
    private double originalHeight = 0;

    // 动态图层是否已初始化
    private bool _isShapesInitialized = false;
    // 需要动态更新位置/尺寸的图元集合（来自 VM）
    private readonly List<VisualElement> _dynamicShapes = new();

    // 标尺标签对象池（减少频繁创建 Label 带来的抖动）
    private readonly List<Label> _guideXPool = new();
    private readonly List<Label> _guideYPool = new();
    private readonly List<Label> _guideY2Pool = new();

    // 本轮渲染中各对象池已使用数量
    private int _guideXUsed = 0;
    private int _guideYUsed = 0;
    private int _guideY2Used = 0;

    // 视口变化节流
    private long _lastGuideTick = 0;
    private const int GuideThrottleMs = 16;

    private bool _isInitialImageReady = false;
    private static SKTypeface? _jpTypeface;

    private const int ImageSizeRetryCount = 10;
    private const int ImageSizeRetryDelayMs = 50;

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _jpTypeface = GetJapaneseTypeface();
    }

        public MapViewPage(MapViewPageVM mapViewPageVM)
    {
        InitializeComponent();

        // 首帧先隐藏，待尺寸计算完成后再显示，避免闪烁
        ContainerImg.Opacity = 0;
        pinchToZoom.Opacity = 0;
#if WINDOWS
        OverlayCanvas.IsVisible = false;
#endif

        BindingContext = mapViewPageVM;
        _dynamicShapes = mapViewPageVM.dynamicShapes;

        // 监听主容器尺寸变化，确保旋转/窗口变化后依旧按比例显示
        MainGridView.SizeChanged += (_, _) => UpdateImageDimensions();
        pinchToZoom.ViewportChanged += (_, _) => OnViewportChanged();

        RegisterMessages();
    }

    /// <summary>
    /// 注册页面所需消息
    /// </summary>
    private void RegisterMessages()
    {
        // 修改底图后：清空旧图元并重新触发底图加载流程
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshMapToken", (_, _) =>
        {
            _isShapesInitialized = false;
            ClearDynamicElements();
            OnContainerImgLoaded(null, EventArgs.Empty);
        });

        // 修改配筋点和工区后：清空并按当前尺寸重绘图元
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshMapContentToken", (_, _) =>
        {
            _isShapesInitialized = false;
            ClearDynamicElements();
            UpdateShapes(showW, showH);
        });

        // 刷新标尺
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshGuideToken", (_, _) =>
        {
            RenderGuides();
        });
    }

    /// <summary>
    /// 清理动态添加到 ZoomLayer 的元素（保留 XAML 中定义的底图控件）
    /// </summary>
    private void ClearDynamicElements()
    {
        var itemsToRemove = ZoomLayer.Children
            .Where(c => c != ContainerImg)
            .ToList();

        foreach (var item in itemsToRemove)
        {
            ZoomLayer.Children.Remove(item);
        }
    }

    /// <summary>
    /// 图片加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnContainerImgLoaded(object sender, EventArgs e)
    {
        // 确保图片源已加载
        if (ContainerImg.Source == null) return;

        for (int i = 0; i < ImageSizeRetryCount; i++)
        {
            // 获取图片的尺寸
            var size = await GetOriginalImageSize(ContainerImg);
            if (size.Width > 0 && size.Height > 0)
            {
                originalWidth = size.Width;
                originalHeight = size.Height;

                // 图片自适应宽度
                UpdateImageDimensions();

                return;
            }

            await Task.Delay(ImageSizeRetryDelayMs);
        }
    }

    private void UpdateImageDimensions()
    {
        if (originalWidth <= 0 || originalHeight <= 0)
            return;

        double cw = MainGridView.Width;
        double ch = MainGridView.Height;

        if (cw <= 0 || ch <= 0)
            return;

        double fitScale = Math.Min(1d, Math.Min(cw / originalWidth, ch / originalHeight));

        showW = originalWidth * fitScale;
        showH = originalHeight * fitScale;

        ZoomLayer.WidthRequest = showW;
        ZoomLayer.HeightRequest = showH;
        ZoomLayer.HorizontalOptions = LayoutOptions.Start;
        ZoomLayer.VerticalOptions = LayoutOptions.Start;

        ContainerImg.WidthRequest = showW;
        ContainerImg.HeightRequest = showH;

        pinchToZoom.SetBaseSize(showW, showH);
        UpdateShapes(showW, showH);

        if (!_isInitialImageReady)
        {
            _isInitialImageReady = true;
            ContainerImg.Opacity = 1;
            pinchToZoom.Opacity = 1;
        }

    }

    /// <summary>
    /// 获取图片的原始尺寸
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>    
    private static async Task<Size> GetOriginalImageSize(Image image)
    {
#if ANDROID
        var handler = image.Handler?.PlatformView as Android.Widget.ImageView;
        if (handler?.Drawable != null)
        {
            return new Size(handler.Drawable.IntrinsicWidth, handler.Drawable.IntrinsicHeight);
        }
#elif IOS
        var handler = image.Handler?.PlatformView as UIKit.UIImageView;
        if (handler?.Image != null)
        {
            return new Size(handler.Image.Size.Width, handler.Image.Size.Height);
        }
#elif WINDOWS
        var handler = image.Handler?.PlatformView as Microsoft.UI.Xaml.Controls.Image;
        if (handler?.Source is Microsoft.UI.Xaml.Media.Imaging.BitmapSource bs)
        {
            return new Size(bs.PixelWidth, bs.PixelHeight);
        }
#endif
        return Size.Zero;
    }

        // 1. 初始化形状（仅在图片首次加载成功后执行一次）
        private void InitializeShapesOnce()
    {
        if (_isShapesInitialized) return;

        foreach (var element in _dynamicShapes)
        {
#if WINDOWS
            // Windows：IMG/RECT/POLY/LBL 全部放在 ZoomLayer
            ZoomLayer.Children.Add(element);
#else
            // iOS/Mac：仅图片元素放入 ZoomLayer，其它由 SKCanvasView 绘制
            if (element.AutomationId?.StartsWith("IMG|") == true)
            {
                ZoomLayer.Children.Add(element);
            }
#endif
        }

        _isShapesInitialized = true;
    }



    // 2. 更新尺寸与重绘（在 UpdateImageDimensions 中调用）
        private void UpdateShapes(double layerWidth, double layerHeight)
    {
        if (!_isShapesInitialized) InitializeShapesOnce();
        if (originalWidth <= 0 || originalHeight <= 0) return;

        double sx = layerWidth / originalWidth;
        double sy = layerHeight / originalHeight;
        double baseScale = Math.Min(sx, sy);

        ZoomLayer.BatchBegin();

        foreach (var element in _dynamicShapes)
        {
            if (string.IsNullOrWhiteSpace(element.AutomationId))
                continue;

            var data = element.AutomationId.Split('|');
            if (data.Length < 3)
                continue;

            string type = data[0];

#if WINDOWS
            if (type == "IMG")
            {
                var vals = ParseNumbers(data[2]);
                if (vals.Length < 4)
                    continue;

                element.AnchorX = 0;
                element.AnchorY = 0;

                AbsoluteLayout.SetLayoutBounds(element, new Rect(
                    vals[0] * sx,
                    vals[1] * sy,
                    vals[2] * sx,
                    vals[3] * sy));
                AbsoluteLayout.SetLayoutFlags(element, AbsoluteLayoutFlags.None);
            }
            else if (type == "RECT" && element is Rectangle rect)
            {
                var vals = ParseNumbers(data[2]);
                if (vals.Length < 4)
                    continue;

                rect.StrokeThickness = Math.Max(1d, 3d * baseScale);

                AbsoluteLayout.SetLayoutBounds(rect, new Rect(
                    vals[0] * sx,
                    vals[1] * sy,
                    vals[2] * sx,
                    vals[3] * sy));
                AbsoluteLayout.SetLayoutFlags(rect, AbsoluteLayoutFlags.None);
            }
            else if (type == "POLY" && element is Polygon poly)
            {
                if (data.Length < 4)
                    continue;

                var points = new PointCollection();
                for (int i = 2; i < data.Length; i++)
                {
                    var xy = ParseNumbers(data[i]);
                    if (xy.Length < 2)
                        continue;

                    points.Add(new Point(xy[0] * sx, xy[1] * sy));
                }

                poly.Points = points;
                poly.StrokeThickness = Math.Max(1d, 3d * baseScale);

                AbsoluteLayout.SetLayoutBounds(poly, new Rect(0, 0, layerWidth, layerHeight));
                AbsoluteLayout.SetLayoutFlags(poly, AbsoluteLayoutFlags.None);
            }
            else if (type == "LBL" && element is Label lbl)
            {
                var vals = ParseNumbers(data[2]);
                if (vals.Length < 2)
                    continue;

                double baseFontSize = lbl.FontSize;
                if (data.Length > 3 && double.TryParse(data[3], CultureInfo.InvariantCulture, out var parsedFont))
                {
                    baseFontSize = parsedFont;
                }

                lbl.FontSize = Math.Max(1d, baseFontSize * baseScale);

                AbsoluteLayout.SetLayoutBounds(lbl, new Rect(
                    vals[0] * sx,
                    vals[1] * sy,
                    AbsoluteLayout.AutoSize,
                    AbsoluteLayout.AutoSize));
                AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
            }
#else
            // iOS/Mac：仅更新 IMG（RECT/POLY/LBL 在 OverlayCanvas 绘制）
            if (type == "IMG")
            {
                var vals = ParseNumbers(data[2]);
                if (vals.Length < 4)
                    continue;

                element.AnchorX = 0;
                element.AnchorY = 0;

                AbsoluteLayout.SetLayoutBounds(element, new Rect(
                    vals[0] * sx,
                    vals[1] * sy,
                    vals[2] * sx,
                    vals[3] * sy));

                AbsoluteLayout.SetLayoutFlags(element, AbsoluteLayoutFlags.None);
            }
#endif
        }

        ZoomLayer.BatchCommit();

#if !WINDOWS
        OverlayCanvas?.InvalidateSurface();
#endif
    }



        private static double[] ParseNumbers(string text)
    {
        // 使用 TryParse 避免脏数据导致绘制流程异常中断
        return text
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s =>
            {
                return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                    ? value
                    : double.NaN;
            })
            .Where(v => !double.IsNaN(v))
            .ToArray();
    }

    private enum GuideLayerKind
    {
        X,
        Y,
        Y2
    }

    private void HideAllGuides()
    {
        HideUnusedGuides(_guideXPool, 0);
        HideUnusedGuides(_guideYPool, 0);
        HideUnusedGuides(_guideY2Pool, 0);
    }

    private static void HideUnusedGuides(List<Label> pool, int usedCount)
    {
        for (int i = usedCount; i < pool.Count; i++)
        {
            pool[i].IsVisible = false;
        }
    }

    private void BeginGuideRender()
    {
        _guideXUsed = 0;
        _guideYUsed = 0;
        _guideY2Used = 0;

        GuideXLayer.BatchBegin();
        GuideYLayer.BatchBegin();
        GuideY2Layer.BatchBegin();
    }

    private void EndGuideRender()
    {
        HideUnusedGuides(_guideXPool, _guideXUsed);
        HideUnusedGuides(_guideYPool, _guideYUsed);
        HideUnusedGuides(_guideY2Pool, _guideY2Used);

        GuideY2Layer.BatchCommit();
        GuideYLayer.BatchCommit();
        GuideXLayer.BatchCommit();
    }

    private Label AcquireGuideLabel(GuideLayerKind kind, GuideDrawItem item, bool isX)
    {
        List<Label> pool;
        AbsoluteLayout layer;
        int index;

        switch (kind)
        {
            case GuideLayerKind.X:
                pool = _guideXPool;
                layer = GuideXLayer;
                index = _guideXUsed++;
                break;
            case GuideLayerKind.Y:
                pool = _guideYPool;
                layer = GuideYLayer;
                index = _guideYUsed++;
                break;
            default:
                pool = _guideY2Pool;
                layer = GuideY2Layer;
                index = _guideY2Used++;
                break;
        }

        Label lbl;
        if (index < pool.Count)
        {
            lbl = pool[index];
        }
        else
        {
            lbl = CreateGuideLabel(item, isX);
            pool.Add(lbl);
            layer.Children.Add(lbl);
        }

        UpdateGuideLabel(lbl, item, isX);
        lbl.IsVisible = true;
        return lbl;
    }

    private static void UpdateGuideLabel(Label lbl, GuideDrawItem item, bool isX)
    {
        lbl.Text = item.Text;
        lbl.WidthRequest = item.Width;
        lbl.HeightRequest = item.Height;
        lbl.FontSize = item.FontSize;
        lbl.TextColor = item.Color;
        lbl.FontFamily = NormalizeFontFamily(item.FontFamily);
        lbl.HorizontalTextAlignment = isX ? TextAlignment.Center : TextAlignment.End;
        lbl.VerticalTextAlignment = TextAlignment.Center;
        lbl.LineBreakMode = LineBreakMode.NoWrap;
        lbl.Padding = 0;
        lbl.Margin = 0;
    }

    private static string? NormalizeFontFamily(string? fontFamily)
    {
        if (string.IsNullOrWhiteSpace(fontFamily))
            return null;

        return fontFamily.Trim();
    }

    private void LayoutGuideLabel(Label lbl, double x, double y, double width, double height)
    {
        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(x, y, width, height));
        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
    }

    private void RenderGuides()
    {
        if (BindingContext is not MapViewPageVM vm || vm.CurrentMap == null || showW <= 0 || showH <= 0)
        {
            HideAllGuides();
            return;
        }

        double mapWidth = Convert.ToDouble(vm.CurrentMap.HM04009);
        double mapHeight = Convert.ToDouble(vm.CurrentMap.HM04010);

        if (mapWidth <= 0 || mapHeight <= 0)
        {
            HideAllGuides();
            return;
        }

        // map显示范围 -> ZoomLayer内容坐标
        double sx = showW / mapWidth;
        double sy = showH / mapHeight;

        double scale = pinchToZoom.CurrentScale;
        double tx = pinchToZoom.OffsetX;
        double ty = pinchToZoom.OffsetY;

        // 当前视口左上角在内容坐标系中的偏移
        double startX = scale == 0 ? 0 : Math.Max(0, -tx / scale);
        double startY = scale == 0 ? 0 : Math.Max(0, -ty / scale);

        BeginGuideRender();
        try
        {
            RenderXGuides(vm, sx, sy, scale, tx, ty, startX, startY);
            RenderYGuides(vm, sx, sy, scale, tx, ty, startX, startY);
        }
        finally
        {
            EndGuideRender();
        }
    }

    private void RenderXGuides(MapViewPageVM vm, double sx, double sy, double scale, double tx, double ty, double startX, double startY)
    {
        var map = vm.CurrentMap;
        if (map == null) return;

        double cviewWidth = MainGridView.Width;
        double cviewHeight = MainGridView.Height;

        int icut = 1;

        foreach (var item in vm.GuideXItems)
        {
            double localX = item.LogicalValue;
            double angle = item.Angle;

            if (angle == 0 && (localX < 0 || localX > Convert.ToDouble(map.HM04009)))
                continue;

            double baseScreenX = tx + localX * sx * scale;

            // =========================
            // angle = 0
            // =========================
            if (angle == 0)
            {
                if (baseScreenX < 0 || baseScreenX > cviewWidth)
                    continue;

                if (Convert.ToInt32(baseScreenX) <= Convert.ToInt32(item.Width / 2))
                {
                    if (icut == 1)
                    {
                        var lbl = AcquireGuideLabel(GuideLayerKind.X, item, true);
                        LayoutGuideLabel(lbl, 0, 0, item.Width, item.Height);
                        icut++;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    var lbl = AcquireGuideLabel(GuideLayerKind.X, item, true);
                    LayoutGuideLabel(lbl, baseScreenX - item.Width / 2, 0, item.Width, item.Height);
                }

                continue;
            }

            // =========================
            // angle < 0
            // =========================
            if (angle < 0)
            {
                double offX = Math.Ceiling(startY * scale * Math.Tan(Math.Abs(angle) * Math.PI / 180));
                double ex_x = baseScreenX + offX;

                if (ex_x < 0)
                {
                    double y = Math.Ceiling(
                        Math.Abs(ex_x) /
                        Math.Tan(Math.Abs(angle) * Math.PI / 180));

                    double drawY = y - item.Height / 2;

                    if (drawY >= 0 && drawY <= cviewHeight)
                    {
                        var lbl = AcquireGuideLabel(GuideLayerKind.Y, item, true);
                        LayoutGuideLabel(lbl, 0, drawY, item.Width, item.Height);
                    }
                }
                else
                {
                    if (ex_x <= cviewWidth)
                    {
                        var lbl = AcquireGuideLabel(GuideLayerKind.X, item, true);
                        LayoutGuideLabel(lbl, ex_x - item.Width / 2, 0, item.Width, item.Height);
                    }
                }
            }
            // =========================
            // angle > 0
            // =========================
            else
            {
                double offX = Math.Ceiling(startY * scale * Math.Tan(Math.Abs(angle) * Math.PI / 180));
                double ex_x = baseScreenX - offX;

                if (ex_x > cviewWidth)
                {
                    double y = Math.Ceiling(
                        Math.Abs(ex_x - cviewWidth) /
                        Math.Tan(Math.Abs(angle) * Math.PI / 180));

                    double drawY = y - item.Height / 2;

                    if (drawY >= 0 && drawY <= cviewHeight)
                    {
                        var lbl = AcquireGuideLabel(GuideLayerKind.Y2, item, true);
                        LayoutGuideLabel(lbl, 0, drawY, item.Width, item.Height);
                    }
                }
                else
                {
                    if (ex_x >= 0)
                    {
                        var lbl = AcquireGuideLabel(GuideLayerKind.X, item, true);
                        LayoutGuideLabel(lbl, ex_x - item.Width / 2, 0, item.Width, item.Height);
                    }
                }
            }
        }
    }

    private void RenderYGuides(MapViewPageVM vm, double sx, double sy, double scale, double tx, double ty, double startX, double startY)
    {
        var map = vm.CurrentMap;
        if (map == null) return;

        double cviewWidth = MainGridView.Width;
        double cviewHeight = MainGridView.Height;

        int icut = 1;

        foreach (var item in vm.GuideYItems)
        {
            double localY = item.LogicalValue;
            double angle = item.Angle;

            if (angle == 0 && (localY < 0 || localY > Convert.ToDouble(map.HM04010)))
                continue;

            double baseScreenY = ty + localY * sy * scale;

            // =========================
            // angle = 0
            // =========================
            if (angle == 0)
            {
                double ex0 = ty + localY * sy * scale;

                if (ex0 < 0 || ex0 > MainGridView.Height)
                    continue;

                if (Convert.ToInt32(ex0) <= Convert.ToInt32(item.Height / 2))
                {
                    if (icut == 1)
                    {
                        var lbl = AcquireGuideLabel(GuideLayerKind.Y, item, false);
                        LayoutGuideLabel(lbl, 0, 0, item.Width, item.Height);
                        icut++;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    var lbl = AcquireGuideLabel(GuideLayerKind.Y, item, false);
                    LayoutGuideLabel(lbl, 0, ex0 - item.Height / 2, item.Width, item.Height);
                }

                continue;
            }

            // =========================
            // angle < 0
            // =========================
            if (angle < 0)
            {
                double g_left = Math.Max(0, -tx);

                double offY = Math.Ceiling(Math.Abs(g_left) * Math.Tan(Math.Abs(angle) * Math.PI / 180));
                double ex_y = Math.Round(baseScreenY, 0) - offY;

                if (ex_y > cviewHeight)
                {
                    double ex_x = Math.Ceiling(
                        Math.Abs(ex_y) /
                        Math.Tan(Math.Abs(angle) * Math.PI / 180));

                    if (ex_x <= cviewWidth)
                    {
                        double drawX = ex_x - item.Width / 2;

                        var lbl = AcquireGuideLabel(GuideLayerKind.X, item, false);
                        LayoutGuideLabel(lbl, drawX, 0, item.Width, item.Height);
                    }
                    else
                    {
                        double ey_y_right = Math.Ceiling(
                            Math.Abs(ex_x - cviewWidth) *
                            Math.Tan(Math.Abs(angle) * Math.PI / 180));

                        double drawY = ey_y_right - item.Height / 2;

                        if (drawY >= 0 && drawY <= cviewHeight)
                        {
                            var lbl = AcquireGuideLabel(GuideLayerKind.Y2, item, false);
                            LayoutGuideLabel(lbl, 0, drawY, item.Width, item.Height);
                        }
                    }
                }
                else
                {
                    if (ex_y >= 0)
                    {
                        double drawY = ex_y - item.Height / 2;

                        var lbl = AcquireGuideLabel(GuideLayerKind.Y, item, false);
                        LayoutGuideLabel(lbl, 0, drawY, item.Width, item.Height);
                    }
                }
            }
            // =========================
            // angle > 0
            // =========================
            else
            {
                double g_left = Math.Max(0, -tx);

                double offY = Math.Ceiling(Math.Abs(g_left) * Math.Tan(Math.Abs(angle) * Math.PI / 180));
                double ex_y = Math.Round(baseScreenY, 0) + offY;

                if (ex_y < 0)
                {
                    double ex_x = Math.Ceiling(
                        Math.Abs(ex_y) /
                        Math.Tan(Math.Abs(angle) * Math.PI / 180));

                    double ex_y_right = Math.Ceiling(
                        (cviewWidth - ex_x) *
                        Math.Tan(Math.Abs(angle) * Math.PI / 180));

                    // 先落到顶部
                    if (ex_y_right > cviewHeight)
                    {
                        double drawX = ex_x - item.Width / 2;

                        var lbl = AcquireGuideLabel(GuideLayerKind.X, item, false);
                        LayoutGuideLabel(lbl, drawX, 0, item.Width, item.Height);
                    }
                    // 再落到右侧
                    else
                    {
                        double drawY = ex_y_right - item.Height / 2;

                        if (drawY >= 0 && drawY <= cviewHeight)
                        {
                            var lbl = AcquireGuideLabel(GuideLayerKind.Y2, item, false);
                            LayoutGuideLabel(lbl, 0, drawY, item.Width, item.Height);
                        }
                    }
                }
                else
                {
                    if (ex_y <= cviewHeight)
                    {
                        double drawY = ex_y - item.Height / 2;

                        var lbl = AcquireGuideLabel(GuideLayerKind.Y, item, false);
                        LayoutGuideLabel(lbl, 0, drawY, item.Width, item.Height);
                    }
                }
            }
        }
    }

        private Label CreateGuideLabel(GuideDrawItem item, bool isX)
    {
        return new Label
        {
            Text = item.Text,
            WidthRequest = item.Width,
            HeightRequest = item.Height,
            FontSize = item.FontSize,
            TextColor = item.Color,
            FontFamily = NormalizeFontFamily(item.FontFamily),
            HorizontalTextAlignment = isX ? TextAlignment.Center : TextAlignment.End,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.NoWrap,
            Padding = 0,
            Margin = 0
        };
    }

        private void OnOverlayCanvasPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
#if WINDOWS
        return;
#endif

        var canvas = e.Surface.Canvas;

        canvas.Clear(SKColors.Transparent);

        if (originalWidth <= 0 || originalHeight <= 0 || showW <= 0 || showH <= 0)
            return;

        float density = OverlayCanvas.Width > 0
            ? (float)(e.Info.Width / OverlayCanvas.Width)
            : (float)DeviceDisplay.Current.MainDisplayInfo.Density;

        if (density <= 0)
            density = 1f;

        double sx = showW / originalWidth;
        double sy = showH / originalHeight;
        double baseScale = Math.Min(sx, sy);

        double zoomScale = pinchToZoom.CurrentScale;
        double tx = pinchToZoom.OffsetX;
        double ty = pinchToZoom.OffsetY;

        foreach (var element in _dynamicShapes)
        {
            if (!element.IsVisible || string.IsNullOrWhiteSpace(element.AutomationId))
                continue;

            var data = element.AutomationId.Split('|');
            if (data.Length < 3)
                continue;

            string type = data[0];

            if (type == "RECT" && element is Rectangle rect)
            {
                var vals = ParseNumbers(data[2]);
                if (vals.Length < 4)
                    continue;

                float x = SnapPx(ToPx(tx + vals[0] * sx * zoomScale, density));
                float y = SnapPx(ToPx(ty + vals[1] * sy * zoomScale, density));
                float w = Math.Max(1f, ToPx(vals[2] * sx * zoomScale, density));
                float h = Math.Max(1f, ToPx(vals[3] * sy * zoomScale, density));

                var strokeColor = SKColors.Red;
                if (rect.Stroke is SolidColorBrush sb)
                {
                    strokeColor = ToSkColor(sb.Color);
                }

                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = strokeColor,
                    StrokeWidth = Math.Max(1f, ToPx(3d * baseScale * zoomScale, density))
                };

                canvas.DrawRect(new SKRect(x, y, x + w, y + h), paint);
            }
            else if (type == "POLY" && element is Polygon poly)
            {
                if (data.Length < 4)
                    continue;

                var strokeColor = SKColors.Red;
                if (poly.Stroke is SolidColorBrush sb)
                {
                    strokeColor = ToSkColor(sb.Color);
                }

                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = strokeColor,
                    StrokeWidth = Math.Max(1f, ToPx(3d * baseScale * zoomScale, density))
                };

                using var path = new SKPath();

                bool started = false;
                for (int i = 2; i < data.Length; i++)
                {
                    var xy = ParseNumbers(data[i]);
                    if (xy.Length < 2)
                        continue;

                    float px = SnapPx(ToPx(tx + xy[0] * sx * zoomScale, density));
                    float py = SnapPx(ToPx(ty + xy[1] * sy * zoomScale, density));

                    if (!started)
                    {
                        path.MoveTo(px, py);
                        started = true;
                    }
                    else
                    {
                        path.LineTo(px, py);
                    }
                }

                if (started)
                {
                    path.Close();
                    canvas.DrawPath(path, paint);
                }
            }
            else if (type == "LBL" && element is Label lbl)
            {
                var vals = ParseNumbers(data[2]);
                if (vals.Length < 2)
                    continue;

                double baseFontSize = lbl.FontSize;
                if (data.Length > 3 && double.TryParse(data[3], CultureInfo.InvariantCulture, out var parsedFont))
                {
                    baseFontSize = parsedFont;
                }

                float textSizePx = Math.Max(1f, ToPx(baseFontSize * baseScale * zoomScale, density));
                if (string.IsNullOrEmpty(lbl.Text))
                    continue;

                //using var typeface = GetTypefaceForJapaneseText(lbl.Text);

                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Color = ToSkColor(lbl.TextColor),
                    IsStroke = false
                };

                using var font = new SKFont
                {
                    Typeface = _jpTypeface,
                    Size = textSizePx,
                    Subpixel = true,

                    // 你的场景是透明叠加层 + 拖动中的文字
                    // 先用 Antialias 更稳一些
                    Edging = SKFontEdging.Antialias
                };

                float x = ToPx(tx + vals[0] * sx * zoomScale, density);
                float yTop = ToPx(ty + vals[1] * sy * zoomScale, density);

                var metrics = font.Metrics;
                float baseline = yTop - metrics.Ascent;

                canvas.DrawText(lbl.Text, x, baseline, font, paint);
            }
        }
    }

    private SKTypeface GetJapaneseTypeface()
    {
        var fm = SKFontManager.Default;

#if WINDOWS
    string[] preferredFamilies =
    {
        "Yu Gothic UI",
        "Yu Gothic",
        "Meiryo UI",
        "Meiryo",
        "MS Gothic"
    };
#elif IOS || MACCATALYST
        string[] preferredFamilies =
        {
        "Hiragino Sans",
        "Hiragino Kaku Gothic ProN",
        "Hiragino Kaku Gothic Pro"
    };
#else
    string[] preferredFamilies =
    {
        "Noto Sans CJK JP",
        "Noto Sans JP"
    };
#endif

        foreach (var family in preferredFamilies)
        {
            try
            {
                var tf = SKTypeface.FromFamilyName(family);
                if (tf != null && tf != SKTypeface.Default)
                    return tf;
            }
            catch
            {
            }
        }

        return SKTypeface.Default;
    }

    private static float ToPx(double dp, float density)
    {
        return (float)(dp * density);
    }

    private static float SnapPx(float px)
    {
        return (float)Math.Round(px);
    }

    private static SKColor ToSkColor(Color? color)
    {
        if (color == null)
            return SKColors.White;

        return new SKColor(
            (byte)(Math.Clamp(color.Red, 0, 1) * 255),
            (byte)(Math.Clamp(color.Green, 0, 1) * 255),
            (byte)(Math.Clamp(color.Blue, 0, 1) * 255),
            (byte)(Math.Clamp(color.Alpha, 0, 1) * 255));
    }

        private void OnViewportChanged()
    {
        long now = Environment.TickCount64;
        if (now - _lastGuideTick < GuideThrottleMs)
            return;

        _lastGuideTick = now;
        RenderGuides();
#if !WINDOWS
        OverlayCanvas?.InvalidateSurface();
#endif
    }
}