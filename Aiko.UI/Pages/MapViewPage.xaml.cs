using Aiko.Common;
using Aiko.UI.ViewModels.PageVMs;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.ComponentModel;

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
    // 首屏优先图元（IMG）是否已初始化
    private bool _isPriorityShapesInitialized = false;
    // 次级图元（RECT/POLY/LBL）是否已初始化
    private bool _isDeferredShapesInitialized = false;
    // 需要动态更新位置/尺寸的图元集合（来自 VM）
    private readonly List<MapShape> _dynamicShapes = new();
    // 由图元数据创建出的可交互控件缓存，避免重复创建图片/Windows 控件。
    private readonly Dictionary<MapShape, VisualElement> _shapeElements = new();
    // 常用地图图标缓存，避免相同文件名反复创建 ImageSource。
    private static readonly Dictionary<string, ImageSource> _mapIconSourceCache = new(StringComparer.OrdinalIgnoreCase);
    // 延迟补绘次级图元时使用的取消令牌，避免快速切图时旧任务回写。
    private CancellationTokenSource? _deferredShapeRenderCts;
    // 首帧图元延迟显示取消令牌。
    private CancellationTokenSource? _initialShapeDisplayCts;

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

    // 是否已经完成当前底图的首次尺寸计算与首帧显示。
    private bool _isInitialImageReady = false;
    // 地图切换时，先让底图单独显示一小段时间，再显示图元，避免视觉上出现“工区先于底图”。
    private bool _deferDynamicShapeDisplay = false;
    // Skia 绘制日文字体缓存，避免重复从资源加载。
    private static SKTypeface? _jpTypeface;

    // 获取底图原始尺寸时的重试次数。
    private const int ImageSizeRetryCount = 10;
    // 每次重试之间的等待毫秒数。
    private const int ImageSizeRetryDelayMs = 50;
    // 底图先显示后，图元再延迟出现的毫秒数。
    private const int InitialShapeDisplayDelayMs = 80;

    /// <summary>
    /// 需要跟随主题刷新的 Picker 集合
    /// </summary>
    /// <returns></returns>
    private IEnumerable<Picker> GetThemeAwarePickers()
    {
        yield return pck_Area;
        yield return pck_Position;
        yield return pck_Proc;
    }

    /// <summary>
    /// 判断是否为需要触发主题刷新的属性
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    private static bool IsThemeAwarePickerProperty(string? propertyName)
    {
        return propertyName == nameof(VisualElement.IsEnabled)
            || propertyName == nameof(Picker.ItemsSource)
            || propertyName == nameof(Picker.SelectedIndex);
    }

    /// <summary>
    /// 为主题相关 Picker 注册监听
    /// </summary>
    private void RegisterThemeAwarePickers()
    {
        foreach (var picker in GetThemeAwarePickers())
        {
            picker.PropertyChanged += OnThemeAwarePickerPropertyChanged;
            picker.HandlerChanged += OnThemeAwarePickerHandlerChanged;
        }
    }

    /// <summary>
    /// 统一刷新主题相关 Picker 的样式
    /// </summary>
    private void RefreshThemeAwarePickers()
    {
        foreach (var picker in GetThemeAwarePickers())
        {
            RefreshPickerTheme(picker);
        }
    }

    /// <summary>
    /// 在主线程刷新单个 Picker 的主题样式
    /// </summary>
    /// <param name="picker"></param>
    private void RefreshPickerThemeOnMainThread(Picker picker)
    {
        MainThread.BeginInvokeOnMainThread(() => RefreshPickerTheme(picker));
    }

    /// <summary>
    /// 在主线程刷新全部主题相关 Picker 的样式
    /// </summary>
    private void RefreshThemeAwarePickersOnMainThread()
    {
        MainThread.BeginInvokeOnMainThread(RefreshThemeAwarePickers);
    }

    /// <summary>
    /// 原生 Handler 变化时重新应用当前主题样式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnThemeAwarePickerHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is Picker picker)
        {
            RefreshPickerThemeOnMainThread(picker);
        }
    }

    /// <summary>
    /// Picker 状态或数据源变化时，重新应用当前主题样式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnThemeAwarePickerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is Picker picker && IsThemeAwarePickerProperty(e.PropertyName))
        {
            RefreshPickerThemeOnMainThread(picker);
        }
    }

    /// <summary>
    /// 按当前主题刷新单个 Picker 的前景色、标题色和背景色
    /// </summary>
    /// <param name="picker"></param>
    private void RefreshPickerTheme(Picker picker)
    {
        if (TryGetPickerColor(picker.IsEnabled ? "PickerTextColor" : "PickerDisabledTextColor", out var textColor))
        {
            picker.TextColor = textColor;
        }

        if (TryGetPickerColor(picker.IsEnabled ? "PickerTitleColor" : "PickerDisabledTitleColor", out var titleColor))
        {
            picker.TitleColor = titleColor;
        }

        if (TryGetPickerColor("PickerBackgroundColor", out var backgroundColor))
        {
            picker.BackgroundColor = backgroundColor;
        }

        picker.InvalidateMeasure();

        VisualStateManager.GoToState(picker, "Normal");
        if (!picker.IsEnabled)
        {
            VisualStateManager.GoToState(picker, "Disabled");
        }

        picker.Handler?.UpdateValue(nameof(Picker.TextColor));
        picker.Handler?.UpdateValue(nameof(Picker.TitleColor));
        picker.Handler?.UpdateValue(nameof(VisualElement.IsEnabled));
        picker.Handler?.UpdateValue(nameof(VisualElement.BackgroundColor));
    }

    /// <summary>
    /// 从应用资源中读取 Picker 颜色资源
    /// </summary>
    /// <param name="resourceKey"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private static bool TryGetPickerColor(string resourceKey, out Color color)
    {
        color = Colors.Transparent;

        if (Application.Current?.Resources.TryGetValue(resourceKey, out var value) != true)
        {
            return false;
        }

        if (value is Color mauiColor)
        {
            color = mauiColor;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mapViewPageVM"></param>
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
        // 注册需要跟随主题变化刷新的 Picker
        RegisterThemeAwarePickers();
        // 首次进入页面时先应用一次当前主题样式
        RefreshThemeAwarePickers();

        RegisterMessages();

#if !WINDOWS
        _ = LoadTypefaceAsync();
#endif

    }

    /// <summary>
    /// 异步加载日文字体，并在加载完成后刷新 Skia 叠加层。
    /// </summary>
    private async Task LoadTypefaceAsync()
    {
        if (_jpTypeface != null)
            return;

        _jpTypeface = await GetJapaneseTypefaceAsync("ヒラギノ角ゴシック");

        MainThread.BeginInvokeOnMainThread(() =>
        {
            OverlayCanvas?.InvalidateSurface();
        });
    }

    /// <summary>
    /// 注册页面所需消息
    /// </summary>
    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<string, string>(this, "CaptureMapViewportStateToken", (_, _) =>
        {
            if (BindingContext is MapViewPageVM vm)
            {
                vm.SavePendingViewportState(pinchToZoom.CaptureViewportState());
            }
        });

        // 修改底图后：清空旧图元并重新触发底图加载流程
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshMapToken", (_, _) =>
        {
            BeginMapVisualRefresh();
            _isShapesInitialized = false;
            _isPriorityShapesInitialized = false;
            _isDeferredShapesInitialized = false;
            ClearDynamicElements();
            //originalWidth = 0;
            //originalHeight = 0;
            //showW = 0;
            //showH = 0;

            // 等到底图绑定值写入到 Image 控件后，再主动启动一次尺寸探测，
            // 这里仅让新底图完成一次自适应，不再顺手重绘旧图元。
            //MainThread.BeginInvokeOnMainThread(async () =>
            //{
                //await Task.Yield();
                RefreshContainerImgLayout(renderShapes: false);
            //});
        });

        // 修改配筋点和工区后：清空并按当前尺寸重绘图元
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshMapContentToken", (_, _) =>
        {
            _isShapesInitialized = false;
            _isPriorityShapesInitialized = false;
            _isDeferredShapesInitialized = false;
            ClearDynamicElements();
            // 新图元数据已经准备完成，此时再放开显示并重绘。
            _deferDynamicShapeDisplay = false;
            UpdateShapes(showW, showH);
            TryRestorePendingViewportState();
        });

        // 仅更新现有图元的显示状态与文字，不重建整页数据
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshMapDisplayToken", (_, _) =>
        {
            // 轻量刷新：只根据现有图元数据更新布局与绘制，不清空也不重建。
            UpdateShapes(showW, showH);
        });

        // 刷新标尺
        WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshGuideToken", (_, _) =>
        {
            RenderGuides();
        });

        // 主题切换后，页面即使被缓存也主动刷新这三个 Picker 的样式
        WeakReferenceMessenger.Default.Register<string, string>(this, "ThemeChangedToken", (_, _) =>
        {
            RefreshThemeAwarePickersOnMainThread();
        });
    }

    /// <summary>
    /// 开始一次地图切换的视觉刷新。
    /// 先整体隐藏缩放容器，等待新底图尺寸与图元都准备好后再一起显示，
    /// 避免出现“工区先显示、底图后显示”的顺序问题。
    /// </summary>
    private void BeginMapVisualRefresh()
    {
        _initialShapeDisplayCts?.Cancel();
        _initialShapeDisplayCts?.Dispose();
        _initialShapeDisplayCts = null;

        _isInitialImageReady = false;
        _deferDynamicShapeDisplay = true;

        // 这里不再把整个底图层隐藏，避免尺寸探测失败时页面只剩 Guide。
        // 图元是否显示仍由 _deferDynamicShapeDisplay 控制。
        ContainerImg.Opacity = 1;
        pinchToZoom.Opacity = 1;
    }

    /// <summary>
    /// 清理动态添加到 ZoomLayer 的元素（保留 XAML 中定义的底图控件）
    /// </summary>
    private void ClearDynamicElements()
    {
        _initialShapeDisplayCts?.Cancel();
        _initialShapeDisplayCts?.Dispose();
        _initialShapeDisplayCts = null;

        _deferredShapeRenderCts?.Cancel();
        _deferredShapeRenderCts?.Dispose();
        _deferredShapeRenderCts = null;

        var itemsToRemove = ZoomLayer.Children
            .Where(c => c != ContainerImg)
            .ToList();

        foreach (var item in itemsToRemove)
        {
            ZoomLayer.Children.Remove(item);
        }

        _shapeElements.Clear();
    }

    /// <summary>
    /// 按图元数据获取或创建对应的真实控件实例。
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    private VisualElement GetOrCreateElement(MapShape shape)
    {
        if (_shapeElements.TryGetValue(shape, out var existing))
            return existing;

        // 真实控件只在首次需要时创建，后续复用，降低频繁刷新成本。
        VisualElement element = shape.Type switch
        {
            MapShapeType.Image => CreateImageElement(shape),
            MapShapeType.Rectangle => CreateRectangleElement(shape),
            MapShapeType.Polygon => CreatePolygonElement(shape),
            MapShapeType.Label => CreateLabelElement(shape),
            _ => throw new NotSupportedException($"Unsupported shape type: {shape.Type}")
        };

        _shapeElements[shape] = element;
        return element;
    }

    /// <summary>
    /// 获取可复用的地图图标 ImageSource。
    /// 常用确认点和工区图标文件名固定且重复率高，适合直接缓存。
    /// </summary>
    /// <param name="imageSource"></param>
    /// <returns></returns>
    private static ImageSource GetOrCreateMapIconSource(string imageSource)
    {
        if (string.IsNullOrWhiteSpace(imageSource))
            return string.Empty;

        if (_mapIconSourceCache.TryGetValue(imageSource, out var cachedSource))
            return cachedSource;

        ImageSource createdSource = ImageSource.FromFile(imageSource);
        _mapIconSourceCache[imageSource] = createdSource;
        return createdSource;
    }

    /// <summary>
    /// 根据图元数据创建图片控件。
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    private Image CreateImageElement(MapShape shape)
    {
        var image = new Image
        {
            Source = GetOrCreateMapIconSource(shape.ImageSource),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Aspect = Aspect.AspectFill,
            ZIndex = shape.ZIndex,
            IsVisible = shape.IsVisible
        };

        if (shape.CommandParameter != null && BindingContext is MapViewPageVM vm)
        {
            image.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 2,
                Command = vm.GoToCheckPointPageCommand,
                CommandParameter = shape.CommandParameter
            });
        }

        return image;
    }

    /// <summary>
    /// 根据图元数据创建矩形控件。
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    private static Rectangle CreateRectangleElement(MapShape shape)
    {
        return new Rectangle
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            InputTransparent = true,
            IsVisible = shape.IsVisible,
            Stroke = new SolidColorBrush(shape.StrokeColor),
            StrokeThickness = shape.StrokeThickness
        };
    }

    /// <summary>
    /// 根据图元数据创建多边形控件。
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    private static Polygon CreatePolygonElement(MapShape shape)
    {
        return new Polygon
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            InputTransparent = true,
            IsVisible = shape.IsVisible,
            Stroke = new SolidColorBrush(shape.StrokeColor),
            StrokeThickness = shape.StrokeThickness
        };
    }

    /// <summary>
    /// 根据图元数据创建文本标签控件。
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    private static Label CreateLabelElement(MapShape shape)
    {
        return new Label
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            LineBreakMode = LineBreakMode.NoWrap,
            Text = shape.Text,
            TextColor = shape.TextColor,
            FontSize = shape.FontSize,
            IsVisible = shape.IsVisible
        };
    }

    /// <summary>
    /// 按当前缩放比例把图元点集转换为页面坐标点集。
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="sx"></param>
    /// <param name="sy"></param>
    /// <returns></returns>
    private static PointCollection BuildPointCollection(MapShape shape, double sx, double sy)
    {
        var points = new PointCollection();
        foreach (var point in shape.Points)
        {
            points.Add(new Point(point.X * sx, point.Y * sy));
        }

        return points;
    }

    /// <summary>
    /// 底图控件加载后，读取原始像素尺寸并驱动页面进入实际布局流程。
    /// 这里会重试几次，兼容 WinUI 下图片异步解码较慢的情况。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnContainerImgLoaded(object sender, EventArgs e)
    {
        RefreshContainerImgLayout(renderShapes: true);
    }

    /// <summary>
    /// 读取当前底图原始尺寸，并按需要决定是否同时重绘图元。
    /// 切图阶段只做底图自适应；图元重绘交给 RefreshMapContentToken。
    /// </summary>
    /// <param name="renderShapes"></param>
    private async void RefreshContainerImgLayout(bool renderShapes)
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
                UpdateImageDimensions(renderShapes);

                return;
            }

            await Task.Delay(ImageSizeRetryDelayMs);
        }
    }

    /// <summary>
    /// 根据底图原始尺寸和当前容器大小，计算页面实际显示尺寸并更新图层布局。
    /// </summary>
    /// <param name="renderShapes">是否在完成底图自适应后立即重绘图元</param>
    private void UpdateImageDimensions(bool renderShapes = true)
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
        TryRestorePendingViewportState();
        if (renderShapes)
        {
            UpdateShapes(showW, showH);
        }

        if (!_isInitialImageReady)
        {
            _isInitialImageReady = true;
            ContainerImg.Opacity = 1;
            pinchToZoom.Opacity = 1;

            if (renderShapes)
            {
                ScheduleInitialShapeDisplay();
            }
        }

    }

    /// <summary>
    /// 如果 VM 中有待恢复的视口状态，就在当前底图尺寸稳定后恢复。
    /// 恢复成功后立即清空，避免后续普通刷新重复套用旧状态。
    /// </summary>
    private void TryRestorePendingViewportState()
    {
        if (BindingContext is not MapViewPageVM vm)
            return;

        if (!vm.TryGetPendingViewportState(out var viewportState))
            return;

        if (pinchToZoom.TryRestoreViewportState(viewportState))
        {
            vm.ClearPendingViewportState();
        }
    }

    /// <summary>
    /// 底图首帧显示后，稍微延迟再显示图元，避免图元比底图更早被用户感知。
    /// </summary>
    private void ScheduleInitialShapeDisplay()
    {
        if (!_deferDynamicShapeDisplay)
            return;

        _initialShapeDisplayCts?.Cancel();
        _initialShapeDisplayCts?.Dispose();
        _initialShapeDisplayCts = new CancellationTokenSource();
        var token = _initialShapeDisplayCts.Token;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await Task.Delay(InitialShapeDisplayDelayMs, token);

                if (token.IsCancellationRequested)
                    return;

                _deferDynamicShapeDisplay = false;
                UpdateShapes(showW, showH);
            }
            catch (OperationCanceledException)
            {
                // 页面切换过快时直接忽略。
            }
        });
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

    /// <summary>
    /// 初始化首屏优先图元。
    /// 当前仅包含图片类图元，优先保证确认点和工区图标尽快显示。
    /// </summary>
    private void InitializePriorityShapes()
    {
        if (_isPriorityShapesInitialized) return;

        foreach (var shape in _dynamicShapes)
        {
            if (shape.Type == MapShapeType.Image)
            {
                ZoomLayer.Children.Add(GetOrCreateElement(shape));
            }
        }

        _isPriorityShapesInitialized = true;
    }

    /// <summary>
    /// 初始化次级图元（RECT/POLY/LBL）。
    /// 这部分不阻塞首屏，等底图和确认点先显示后再补上。
    /// </summary>
    private void InitializeDeferredShapes()
    {
        if (_isDeferredShapesInitialized) return;

#if WINDOWS
        foreach (var shape in _dynamicShapes)
        {
            if (shape.Type != MapShapeType.Image)
            {
                ZoomLayer.Children.Add(GetOrCreateElement(shape));
            }
        }
#endif

        _isDeferredShapesInitialized = true;
        _isShapesInitialized = _isPriorityShapesInitialized && _isDeferredShapesInitialized;
    }

    /// <summary>
    /// 延迟补绘次级图元，优先让首屏显示底图和确认点。
    /// </summary>
    private void ScheduleDeferredShapeInitialization()
    {
#if WINDOWS
        if (_isDeferredShapesInitialized)
            return;

        _deferredShapeRenderCts?.Cancel();
        _deferredShapeRenderCts?.Dispose();

        _deferredShapeRenderCts = new CancellationTokenSource();
        var token = _deferredShapeRenderCts.Token;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await Task.Yield();

                if (token.IsCancellationRequested)
                    return;

                InitializeDeferredShapes();
                UpdateShapes(showW, showH);
            }
            catch (OperationCanceledException)
            {
                // 页面已切换时忽略即可。
            }
        });
#else
        _isDeferredShapesInitialized = true;
        _isShapesInitialized = _isPriorityShapesInitialized && _isDeferredShapesInitialized;
#endif
    }
    /// <summary>
    /// 按当前底图尺寸与缩放比例更新所有图元的位置、大小和显示状态。
    /// </summary>
    /// <param name="layerWidth"></param>
    /// <param name="layerHeight"></param>
    private void UpdateShapes(double layerWidth, double layerHeight)
    {
        if (!_isPriorityShapesInitialized)
        {
            InitializePriorityShapes();
        }

        if (originalWidth <= 0 || originalHeight <= 0) return;

        double sx = layerWidth / originalWidth;
        double sy = layerHeight / originalHeight;
        double baseScale = Math.Min(sx, sy);

        ZoomLayer.BatchBegin();

        foreach (var shape in _dynamicShapes)
        {
#if WINDOWS
            if (!_isDeferredShapesInitialized && shape.Type != MapShapeType.Image)
            {
                continue;
            }

            var element = GetOrCreateElement(shape);
            element.IsVisible = !_deferDynamicShapeDisplay && shape.IsVisible;

            if (shape.Type == MapShapeType.Image)
            {
                element.AnchorX = 0;
                element.AnchorY = 0;

                AbsoluteLayout.SetLayoutBounds(element, new Rect(
                    shape.Bounds.X * sx,
                    shape.Bounds.Y * sy,
                    shape.Bounds.Width * sx,
                    shape.Bounds.Height * sy));
                AbsoluteLayout.SetLayoutFlags(element, AbsoluteLayoutFlags.None);
            }
            else if (shape.Type == MapShapeType.Rectangle && element is Rectangle rect)
            {
                rect.StrokeThickness = Math.Max(1d, shape.StrokeThickness * baseScale);
                rect.Stroke = new SolidColorBrush(shape.StrokeColor);

                AbsoluteLayout.SetLayoutBounds(rect, new Rect(
                    shape.Bounds.X * sx,
                    shape.Bounds.Y * sy,
                    shape.Bounds.Width * sx,
                    shape.Bounds.Height * sy));
                AbsoluteLayout.SetLayoutFlags(rect, AbsoluteLayoutFlags.None);
            }
            else if (shape.Type == MapShapeType.Polygon && element is Polygon poly)
            {
                poly.Points = BuildPointCollection(shape, sx, sy);
                poly.StrokeThickness = Math.Max(1d, shape.StrokeThickness * baseScale);
                poly.Stroke = new SolidColorBrush(shape.StrokeColor);

                AbsoluteLayout.SetLayoutBounds(poly, new Rect(0, 0, layerWidth, layerHeight));
                AbsoluteLayout.SetLayoutFlags(poly, AbsoluteLayoutFlags.None);
            }
            else if (shape.Type == MapShapeType.Label && element is Label lbl)
            {
                lbl.Text = shape.Text;
                lbl.FontSize = Math.Max(1d, shape.FontSize * baseScale);
                lbl.TextColor = shape.TextColor;

                AbsoluteLayout.SetLayoutBounds(lbl, new Rect(
                    shape.Bounds.X * sx,
                    shape.Bounds.Y * sy,
                    AbsoluteLayout.AutoSize,
                    AbsoluteLayout.AutoSize));
                AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
            }
#else
            // iOS/Mac：仅更新 IMG（RECT/POLY/LBL 在 OverlayCanvas 绘制）
            if (shape.Type == MapShapeType.Image)
            {
                var element = GetOrCreateElement(shape);
                element.IsVisible = !_deferDynamicShapeDisplay && shape.IsVisible;

                element.AnchorX = 0;
                element.AnchorY = 0;

                AbsoluteLayout.SetLayoutBounds(element, new Rect(
                    shape.Bounds.X * sx,
                    shape.Bounds.Y * sy,
                    shape.Bounds.Width * sx,
                    shape.Bounds.Height * sy));

                AbsoluteLayout.SetLayoutFlags(element, AbsoluteLayoutFlags.None);
            }
#endif
        }

        ZoomLayer.BatchCommit();

        ScheduleDeferredShapeInitialization();

#if !WINDOWS
        OverlayCanvas?.InvalidateSurface();
#endif
    }
    /// <summary>
    /// 标尺标签所在的图层类型。
    /// </summary>
    private enum GuideLayerKind
    {
        X,
        Y,
        Y2
    }

    /// <summary>
    /// 隐藏全部标尺标签。
    /// </summary>
    private void HideAllGuides()
    {
        HideUnusedGuides(_guideXPool, 0);
        HideUnusedGuides(_guideYPool, 0);
        HideUnusedGuides(_guideY2Pool, 0);
    }

    /// <summary>
    /// 隐藏对象池中当前未使用的标尺标签。
    /// </summary>
    /// <param name="pool"></param>
    /// <param name="usedCount"></param>
    private static void HideUnusedGuides(List<Label> pool, int usedCount)
    {
        for (int i = usedCount; i < pool.Count; i++)
        {
            pool[i].IsVisible = false;
        }
    }

    /// <summary>
    /// 开始一次标尺渲染并重置对象池使用计数。
    /// </summary>
    private void BeginGuideRender()
    {
        _guideXUsed = 0;
        _guideYUsed = 0;
        _guideY2Used = 0;

        GuideXLayer.BatchBegin();
        GuideYLayer.BatchBegin();
        GuideY2Layer.BatchBegin();
    }

    /// <summary>
    /// 结束一次标尺渲染并提交批量布局更新。
    /// </summary>
    private void EndGuideRender()
    {
        HideUnusedGuides(_guideXPool, _guideXUsed);
        HideUnusedGuides(_guideYPool, _guideYUsed);
        HideUnusedGuides(_guideY2Pool, _guideY2Used);

        GuideY2Layer.BatchCommit();
        GuideYLayer.BatchCommit();
        GuideXLayer.BatchCommit();
    }

    /// <summary>
    /// 从对象池中获取可复用的标尺标签，不存在时创建新实例。
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="item"></param>
    /// <param name="isX"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 把标尺绘制数据同步到指定标签控件。
    /// </summary>
    /// <param name="lbl"></param>
    /// <param name="item"></param>
    /// <param name="isX"></param>
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

    /// <summary>
    /// 标准化字体名称，去除空白后返回。
    /// </summary>
    /// <param name="fontFamily"></param>
    /// <returns></returns>
    private static string? NormalizeFontFamily(string? fontFamily)
    {
        if (string.IsNullOrWhiteSpace(fontFamily))
            return null;

        return fontFamily.Trim();
    }

    /// <summary>
    /// 设置标尺标签在绝对布局中的位置和尺寸。
    /// </summary>
    /// <param name="lbl"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <summary>
    /// 设置标尺标签在 AbsoluteLayout 中的位置与尺寸。
    /// </summary>
    /// <param name="lbl"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private void LayoutGuideLabel(Label lbl, double x, double y, double width, double height)
    {
        AbsoluteLayout.SetLayoutBounds(lbl, new Rect(x, y, width, height));
        AbsoluteLayout.SetLayoutFlags(lbl, AbsoluteLayoutFlags.None);
    }

    /// <summary>
    /// 按当前视口与地图比例重新绘制上下左右标尺。
    /// </summary>
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

    /// <summary>
    /// 绘制 X 方向相关的标尺标签。
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="sx"></param>
    /// <param name="sy"></param>
    /// <param name="scale"></param>
    /// <param name="tx"></param>
    /// <param name="ty"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
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

    /// <summary>
    /// 绘制 Y 方向相关的标尺标签。
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="sx"></param>
    /// <param name="sy"></param>
    /// <param name="scale"></param>
    /// <param name="tx"></param>
    /// <param name="ty"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
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

    /// <summary>
    /// 创建单个标尺标签控件。
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isX"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 使用 Skia 在叠加层绘制矩形、多边形和文本等图元。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

        foreach (var shape in _dynamicShapes)
        {
            if (!shape.IsVisible)
                continue;

            if (shape.Type == MapShapeType.Rectangle)
            {
                float x = SnapPx(ToPx(tx + shape.Bounds.X * sx * zoomScale, density));
                float y = SnapPx(ToPx(ty + shape.Bounds.Y * sy * zoomScale, density));
                float w = Math.Max(1f, ToPx(shape.Bounds.Width * sx * zoomScale, density));
                float h = Math.Max(1f, ToPx(shape.Bounds.Height * sy * zoomScale, density));

                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = ToSkColor(shape.StrokeColor),
                    StrokeWidth = Math.Max(1f, ToPx(shape.StrokeThickness * baseScale * zoomScale, density))
                };

                canvas.DrawRect(new SKRect(x, y, x + w, y + h), paint);
            }
            else if (shape.Type == MapShapeType.Polygon)
            {
                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = ToSkColor(shape.StrokeColor),
                    StrokeWidth = Math.Max(1f, ToPx(shape.StrokeThickness * baseScale * zoomScale, density))
                };

                using var path = new SKPath();

                bool started = false;
                foreach (var point in shape.Points)
                {
                    float px = SnapPx(ToPx(tx + point.X * sx * zoomScale, density));
                    float py = SnapPx(ToPx(ty + point.Y * sy * zoomScale, density));

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
            else if (shape.Type == MapShapeType.Label)
            {
                float textSizePx = Math.Max(1f, ToPx(shape.FontSize * baseScale * zoomScale, density));
                if (string.IsNullOrEmpty(shape.Text))
                    continue;

                using var paint = new SKPaint
                {
                    IsAntialias = true,
                    Color = ToSkColor(shape.TextColor),
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

                float x = ToPx(tx + shape.Bounds.X * sx * zoomScale, density);
                float yTop = ToPx(ty + shape.Bounds.Y * sy * zoomScale, density);

                var metrics = font.Metrics;
                float baseline = yTop - metrics.Ascent;

                canvas.DrawText(shape.Text, x, baseline, font, paint);
            }
        }
    }

    /// <summary>
    /// 获取适合显示日文文本的字体。
    /// </summary>
    /// <returns></returns>
    private async Task<SKTypeface> GetJapaneseTypefaceAsync(string font)
    {
        if (string.IsNullOrWhiteSpace(font))
            return SKTypeface.Default;

        Stream? stream = null;

        try
        {
#if WINDOWS
        stream = typeof(App).Assembly.GetManifestResourceStream(font);
#else
            stream = await FileSystem.OpenAppPackageFileAsync(font);
#endif

            if (stream == null)
                return SKTypeface.Default;

            using var data = SKData.Create(stream);
            return SKTypeface.FromData(data) ?? SKTypeface.Default;
        }
        catch (Exception ex)
        {
            return SKTypeface.Default;
        }
        finally
        {
            stream?.Dispose();
        }
    }

    /// <summary>
    /// 把设备无关单位转换为物理像素。
    /// </summary>
    /// <param name="dp"></param>
    /// <param name="density"></param>
    /// <returns></returns>
    private static float ToPx(double dp, float density)
    {
        return (float)(dp * density);
    }

    /// <summary>
    /// 将像素值对齐到整数像素，减少边缘抖动。
    /// </summary>
    /// <param name="px"></param>
    /// <returns></returns>
    private static float SnapPx(float px)
    {
        return (float)Math.Round(px);
    }

    /// <summary>
    /// 把 MAUI 颜色转换为 Skia 颜色。
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 在缩放容器视口变化时刷新标尺与叠加绘制层。
    /// </summary>
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
