using Aiko.SqliteDb;
using Aiko.UI.ViewModels.PageVMs;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Globalization;

namespace Aiko.UI;

public partial class MapViewPage : ContentPage
{
    double showW = 0;
    double showH = 0;

    // 原始尺寸
    private double originalWidth = 0;
    private double originalHeight = 0;

    private bool _isShapesInitialized = false;
    // 存储需要动态更新位置的控件引用
    private readonly List<VisualElement> _dynamicShapes = new();

    private readonly List<View> _guideViews = new();

    public MapViewPage(MapViewPageVM mapViewPageVM)
    {
        InitializeComponent();
        BindingContext = mapViewPageVM;
        _dynamicShapes = mapViewPageVM.dynamicShapes;
        // 监听 MainGridView 的尺寸变化，确保旋转或调整大小时图片依然填满宽度
        MainGridView.SizeChanged += (s, e) => UpdateImageDimensions();

        pinchToZoom.ViewportChanged += (_, _) => RenderGuides();

        // 修改底图的消息
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshMapToken", async (page, message) =>
        {
            _isShapesInitialized = false;
            // 1. 清理旧形状，但【保留】ContainerImg
            // 我们只移除不在 XAML 中定义的动态元素
            var itemsToRemove = ZoomLayer.Children
                .Where(c => c != ContainerImg)
                .ToList();

            foreach (var item in itemsToRemove)
            {
                ZoomLayer.Children.Remove(item);
            }

            OnContainerImgLoaded(null, null);

        });

        // 修改配筋点和工区的消息
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshMapContentToken", async (page, message) =>
        {
            _isShapesInitialized = false;
            // 1. 清理旧形状，但【保留】ContainerImg
            // 我们只移除不在 XAML 中定义的动态元素
            var itemsToRemove = ZoomLayer.Children
                .Where(c => c != ContainerImg)
                .ToList();

            foreach (var item in itemsToRemove)
            {
                ZoomLayer.Children.Remove(item);
            }

            UpdateShapes(showW, showH);

        });

        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshGuideToken", (page, message) =>
        {
            RenderGuides();
        });
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

        for (int i = 0; i < 10; i++)
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

            await Task.Delay(50);
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

        //Assembly assembly = GetType().GetTypeInfo().Assembly;

        foreach (var element in _dynamicShapes) ZoomLayer.Children.Add(element);

        _isShapesInitialized = true;
    }


    // 2. 更新尺寸与重绘（在 UpdateImageDimensions 中调用）
    private void UpdateShapes(double layerWidth, double layerHeight)
    {
        if (!_isShapesInitialized) InitializeShapesOnce();
        if (originalWidth <= 0 || originalHeight <= 0) return;

        double sx = layerWidth / originalWidth;
        double sy = layerHeight / originalHeight;
        double s = Math.Min(sx, sy);

        ZoomLayer.BatchBegin();

        foreach (var element in _dynamicShapes)
        {
            element.AnchorX = 0;
            element.AnchorY = 0;

            var data = element.AutomationId.Split('|');
            string type = data[0];

            if (type == "RECT")
            {
                var vals = ParseNumbers(data[2]);
                var r = (Rectangle)element;

                AbsoluteLayout.SetLayoutBounds(r, new Rect(
                    vals[0] * sx,
                    vals[1] * sy,
                    vals[2] * sx,
                    vals[3] * sy));

                AbsoluteLayout.SetLayoutFlags(r, AbsoluteLayoutFlags.None);
                r.StrokeThickness = 3 * s;
            }
            else if (type == "POLY")
            {
                var p = (Polygon)element;
                var newPoints = new PointCollection();

                for (int i = 2; i < data.Length; i++)
                {
                    var xy = ParseNumbers(data[i]);
                    newPoints.Add(new Point(xy[0] * sx, xy[1] * sy));
                }

                p.Points = newPoints;
                p.StrokeThickness = 3 * s;
            }
            else if (type == "IMG")
            {
                var vals = ParseNumbers(data[2]);

                AbsoluteLayout.SetLayoutBounds(element, new Rect(
                    vals[0] * sx,
                    vals[1] * sy,
                    vals[2] * sx,
                    vals[3] * sy));

                AbsoluteLayout.SetLayoutFlags(element, AbsoluteLayoutFlags.None);
            }
            else if (type == "LBL")
            {
                var vals = ParseNumbers(data[2]);
                var lbl = (Label)element;

                AbsoluteLayout.SetLayoutBounds(lbl, new Rect(
                    vals[0] * sx,
                    vals[1] * sy,
                    AbsoluteLayout.AutoSize,
                    AbsoluteLayout.AutoSize));

                AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);

                double fontSize = double.Parse(data[3], CultureInfo.InvariantCulture);
                lbl.FontSize = fontSize * s;
            }
        }

        ZoomLayer.BatchCommit();
    }

    private static double[] ParseNumbers(string text)
    {
        return text
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => double.Parse(s, CultureInfo.InvariantCulture))
            .ToArray();
    }

    private void ClearGuideViews()
    {
        foreach (var v in _guideViews)
        {
            GuideXLayer.Children.Remove(v);
            GuideYLayer.Children.Remove(v);
            GuideY2Layer.Children.Remove(v);
        }

        _guideViews.Clear();
    }

    private void RenderGuides()
    {
        if (BindingContext is not MapViewPageVM vm) return;
        if (vm.CurrentMap == null) return;
        if (showW <= 0 || showH <= 0) return;

        ClearGuideViews();

        double mapWidth = Convert.ToDouble(vm.CurrentMap.HM04009);
        double mapHeight = Convert.ToDouble(vm.CurrentMap.HM04010);

        if (mapWidth <= 0 || mapHeight <= 0) return;

        // map显示范围 -> ZoomLayer内容坐标
        double sx = showW / mapWidth;
        double sy = showH / mapHeight;

        double scale = pinchToZoom.CurrentScale;
        double tx = pinchToZoom.OffsetX;
        double ty = pinchToZoom.OffsetY;

        // 当前视口左上角在内容坐标系中的偏移
        double startX = scale == 0 ? 0 : Math.Max(0, -tx / scale);
        double startY = scale == 0 ? 0 : Math.Max(0, -ty / scale);

        RenderXGuides(vm, sx, sy, scale, tx, ty, startX, startY);
        RenderYGuides(vm, sx, sy, scale, tx, ty, startX, startY);
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

            var lbl = CreateGuideLabel(item, true);

            double baseScreenX = pinchToZoom.OffsetX+ localX * sy * scale;

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
                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, 0, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideXLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
                        icut++;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    AbsoluteLayout.SetLayoutBounds(lbl, new Rect(baseScreenX - item.Width / 2, 0, item.Width, item.Height));
                    AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                    GuideXLayer.Children.Add(lbl);
                    _guideViews.Add(lbl);
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
                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, drawY, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideYLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
                    }
                }
                else
                {
                    if (ex_x <= cviewWidth)
                    {
                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(ex_x - item.Width / 2, 0, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideXLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
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
                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, drawY, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideY2Layer.Children.Add(lbl);
                        _guideViews.Add(lbl);
                    }
                }
                else
                {
                    if (ex_x >= 0)
                    {
                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(ex_x - item.Width / 2, 0, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideXLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
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

            var lbl = CreateGuideLabel(item, false);

            double baseScreenY = pinchToZoom.OffsetY + localY * sy * scale;

            // =========================
            // angle = 0
            // =========================
            if (angle == 0)
            {
                double ex0 = pinchToZoom.OffsetY + localY * sy * scale;

                if (ex0 < 0 || ex0 > MainGridView.Height)
                    continue;

                if (Convert.ToInt32(ex0) <= Convert.ToInt32(item.Height / 2))
                {
                    if (icut == 1)
                    {
                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, 0, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideYLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
                        icut++;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, ex0 - item.Height / 2, item.Width, item.Height));
                    AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                    GuideYLayer.Children.Add(lbl);
                    _guideViews.Add(lbl);
                }

                continue;
            }

            // =========================
            // angle < 0
            // =========================
            if (angle < 0)
            {
                double g_left = Math.Max(0, -pinchToZoom.OffsetX);
                double g_top = Math.Max(0, -pinchToZoom.OffsetY);

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

                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(drawX, 0, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideXLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
                    }
                    else
                    {
                        double ey_y_right = Math.Ceiling(
                            Math.Abs(ex_x - cviewWidth) *
                            Math.Tan(Math.Abs(angle) * Math.PI / 180));

                        double drawY = ey_y_right - item.Height / 2;

                        if (drawY >= 0 && drawY <= cviewHeight)
                        {
                            AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, drawY, item.Width, item.Height));
                            AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                            GuideY2Layer.Children.Add(lbl);
                            _guideViews.Add(lbl);
                        }
                    }
                }
                else
                {
                    if (ex_y >= 0)
                    {
                        double drawY = ex_y - item.Height / 2;

                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, drawY, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideYLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
                    }
                }
            }
            // =========================
            // angle > 0
            // =========================
            else
            {
                double g_left = Math.Max(0, -pinchToZoom.OffsetX);

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

                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(drawX, 0, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideXLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
                    }
                    // 再落到右侧
                    else
                    {
                        double drawY = ex_y_right - item.Height / 2;

                        if (drawY >= 0 && drawY <= cviewHeight)
                        {
                            AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, drawY, item.Width, item.Height));
                            AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                            GuideY2Layer.Children.Add(lbl);
                            _guideViews.Add(lbl);
                        }
                    }
                }
                else
                {
                    if (ex_y <= cviewHeight)
                    {
                        double drawY = ex_y - item.Height / 2;

                        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(0, drawY, item.Width, item.Height));
                        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
                        GuideYLayer.Children.Add(lbl);
                        _guideViews.Add(lbl);
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
            FontFamily = string.IsNullOrWhiteSpace(item.FontFamily) ? null : item.FontFamily,
            HorizontalTextAlignment = isX ? TextAlignment.Center : TextAlignment.End,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.NoWrap,
            Padding = 0,
            Margin = 0
        };
    }
}