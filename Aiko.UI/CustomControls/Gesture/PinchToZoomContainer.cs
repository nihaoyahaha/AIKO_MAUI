#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#endif

namespace Aiko.UI;

public class PinchToZoomContainer : ContentView
{
    // 允许的最小/最大缩放比例。
    private const double MinScale = 0.5;
    private const double MaxScale = 8.0;

    // 当前缩放值，以及本次 pinch 开始时的缩放基准值。
    private double currentScale = 1.0;
    private double startScale = 1.0;

    // 当前内容平移偏移量。
    private double xOffset = 0.0;
    private double yOffset = 0.0;

    // 拖动开始时记录的平移基准值。
    private double panX = 0.0;
    private double panY = 0.0;

    // 双指缩放过程中，用来阻止 pan 与 pinch 同时生效。
    private bool isPinching = false;
    // 记录 pinch 开始时的双指中心点，缩放过程中保持不变，避免双指平移被误当成内容拖动。
    private Point pinchStartOrigin;

    // 被缩放内容的逻辑尺寸，外部通常在图片加载完成后设置。
    public double LayerWidth { get; private set; }
    public double LayerHeight { get; private set; }

    // Windows 鼠标拖动相关状态。
    private bool isMouseDragging = false;
    private Point mouseStartPoint;
    private double mouseStartX = 0.0;
    private double mouseStartY = 0.0;

#if WINDOWS
    private FrameworkElement? _platformView;
#endif

    #region guide
    public double CurrentScale => currentScale;
    public double OffsetX => Content?.TranslationX ?? 0;
    public double OffsetY => Content?.TranslationY ?? 0;

    public event EventHandler? ViewportChanged;

    // 用于外部判断是否仍处于初始视图，常见于图片左右切换时禁用手势翻页。
    public bool IsAtInitialScale => currentScale < 1.001;//Math.Abs(currentScale - 1.0) < 0.001;

	#endregion

	public PinchToZoomContainer()
    {
        // 双指缩放。
        var pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += OnPinchUpdated;
        GestureRecognizers.Add(pinchGesture);

        // 单指拖动。
        var panGesture = new PanGestureRecognizer();
        panGesture.TouchPoints = 1;
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);

#if WINDOWS
        // Windows 下补充鼠标拖动支持。
        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerPressed += OnPointerPressed;
        pointerGesture.PointerMoved += OnPointerMoved;
        pointerGesture.PointerReleased += OnPointerReleased;
        pointerGesture.PointerExited += OnPointerReleased;
        GestureRecognizers.Add(pointerGesture);
#endif

        // 容器尺寸变化后，重新约束内容位置，避免内容越界。
        SizeChanged += (_, _) => RefreshBounds();

        HandlerChanged += OnViewHandlerChanged;
        HandlerChanging += OnViewHandlerChanging;
    }

    // 设置内容的基础尺寸，并把内容锚点固定到左上角，后续缩放和平移都基于这个坐标系计算。
    public void SetBaseSize(double width, double height)
    {
        if (width <= 0 || height <= 0 || Content == null)
            return;

        LayerWidth = width;
        LayerHeight = height;

        Content.AnchorX = 0;
        Content.AnchorY = 0;
        Content.WidthRequest = width;
        Content.HeightRequest = height;
        Content.HorizontalOptions = LayoutOptions.Start;
        Content.VerticalOptions = LayoutOptions.Start;

        Reset();
    }

    // 恢复到 1 倍缩放，并把内容居中显示。
    public void Reset()
    {
        if (Content == null || Width <= 0 || Height <= 0 || LayerWidth <= 0 || LayerHeight <= 0)
            return;

        currentScale = 1.0;
        xOffset = GetCenteredX(currentScale);
        yOffset = GetCenteredY(currentScale);
        panX = xOffset;
        panY = yOffset;

        Content.Scale = currentScale;
        Content.TranslationX = xOffset;
        Content.TranslationY = yOffset;

        RaiseViewportChanged();
    }

    // 在容器大小变化时，把当前位置重新夹到合法范围内。
    // 如果当前正在缩放或鼠标拖动，则暂时不打断当前手势。
    private void RefreshBounds()
    {
        if (Content == null || Width <= 0 || Height <= 0 || LayerWidth <= 0 || LayerHeight <= 0)
            return;

        if (isPinching || isMouseDragging)
            return;

        double tx = ClampX(Content.TranslationX, currentScale);
        double ty = ClampY(Content.TranslationY, currentScale);

        Content.TranslationX = tx;
        Content.TranslationY = ty;

        xOffset = tx;
        yOffset = ty;
        panX = tx;
        panY = ty;

        RaiseViewportChanged();
    }

    // 处理双指缩放。
    // 缩放时会尽量保持手指中心点对应的内容位置不跳动。
    private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (Content == null || Width <= 0 || Height <= 0 || LayerWidth <= 0 || LayerHeight <= 0)
            return;

        switch (e.Status)
        {
            case GestureStatus.Started:
                isPinching = true;
                isMouseDragging = false;
                startScale = currentScale;
                pinchStartOrigin = e.ScaleOrigin;

                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
                panX = xOffset;
                panY = yOffset;
                break;

            case GestureStatus.Running:
                currentScale += (e.Scale - 1.0) * startScale;
                currentScale = Math.Clamp(currentScale, MinScale, MaxScale);

                double targetX = CalculatePinchX(pinchStartOrigin.X * Width);
                double targetY = CalculatePinchY(pinchStartOrigin.Y * Height);

                Content.Scale = currentScale;
                Content.TranslationX = ClampX(targetX, currentScale);
                Content.TranslationY = ClampY(targetY, currentScale);

                RaiseViewportChanged();
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
                panX = xOffset;
                panY = yOffset;
                isPinching = false;

                RaiseViewportChanged();
                break;
        }
    }

    // 处理单指拖动。
    // 拖动过程中始终通过 ClampX/ClampY 把内容限制在可视范围内。
    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (Content == null || Width <= 0 || Height <= 0 || LayerWidth <= 0 || LayerHeight <= 0)
            return;

        if (isPinching || isMouseDragging)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                panX = Content.TranslationX;
                panY = Content.TranslationY;
                RaiseViewportChanged();   // 可选，不要也行
                break;

            case GestureStatus.Running:
                Content.TranslationX = ClampX(panX + e.TotalX, currentScale);
                Content.TranslationY = ClampY(panY + e.TotalY, currentScale);
                RaiseViewportChanged();       // 走节流
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                panX = Content.TranslationX;
                panY = Content.TranslationY;
                xOffset = panX;
                yOffset = panY;
                RaiseViewportChanged(); // 最终位置强制通知
                break;
        }
    }

    // Windows 鼠标左键按下时，记录拖动起点和当前偏移。
    private void OnPointerPressed(object? sender, PointerEventArgs e)
    {
        if (Content == null || isPinching)
            return;

        if (!IsWindowsMouseLeftDrag(e))
            return;

        var pos = e.GetPosition(this);
        if (pos is null)
            return;

        isMouseDragging = true;
        mouseStartPoint = pos.Value;
        mouseStartX = Content.TranslationX;
        mouseStartY = Content.TranslationY;
    }

    // Windows 鼠标移动时按住左键，则按鼠标位移同步平移内容。
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (Content == null || !isMouseDragging || isPinching)
            return;

        if (!IsWindowsMouseLeftDrag(e))
            return;

        var pos = e.GetPosition(this);
        if (pos is null)
            return;

        double dx = pos.Value.X - mouseStartPoint.X;
        double dy = pos.Value.Y - mouseStartPoint.Y;

        Content.TranslationX = ClampX(mouseStartX + dx, currentScale);
        Content.TranslationY = ClampY(mouseStartY + dy, currentScale);

        RaiseViewportChanged();
    }

    // Windows 鼠标释放时，提交本次拖动后的最终偏移。
    private void OnPointerReleased(object? sender, PointerEventArgs e)
    {
        if (Content == null)
            return;

        if (!isMouseDragging)
            return;

        isMouseDragging = false;

        xOffset = Content.TranslationX;
        yOffset = Content.TranslationY;
        panX = xOffset;
        panY = yOffset;

        RaiseViewportChanged();
    }

    // 根据 pinch 焦点计算新的 X 偏移，让缩放围绕手指位置进行。
    private double CalculatePinchX(double focusX)
    {
        double scaledWidth = LayerWidth * currentScale;

        if (scaledWidth <= Width)
            return GetCenteredX(currentScale);

        if (startScale <= 0)
            return xOffset;

        return focusX - ((focusX - xOffset) / startScale) * currentScale;
    }

    // 根据 pinch 焦点计算新的 Y 偏移，让缩放围绕手指位置进行。
    private double CalculatePinchY(double focusY)
    {
        double scaledHeight = LayerHeight * currentScale;

        if (scaledHeight <= Height)
            return GetCenteredY(currentScale);

        if (startScale <= 0)
            return yOffset;

        return focusY - ((focusY - yOffset) / startScale) * currentScale;
    }

    // Windows 鼠标滚轮缩放入口。
    // 以鼠标当前位置为焦点执行缩放。
    private void ZoomAt(double focusX, double focusY, double factor)
    {
        if (Content == null || Width <= 0 || Height <= 0 || LayerWidth <= 0 || LayerHeight <= 0)
            return;

        double oldScale = currentScale;
        double newScale = Math.Clamp(oldScale * factor, MinScale, MaxScale);

        if (Math.Abs(newScale - oldScale) < 0.0001)
            return;

        xOffset = Content.TranslationX;
        yOffset = Content.TranslationY;

        double targetX;
        double targetY;

        double scaledWidth = LayerWidth * newScale;
        double scaledHeight = LayerHeight * newScale;

        if (scaledWidth <= Width)
            targetX = GetCenteredX(newScale);
        else
            targetX = focusX - ((focusX - xOffset) / oldScale) * newScale;

        if (scaledHeight <= Height)
            targetY = GetCenteredY(newScale);
        else
            targetY = focusY - ((focusY - yOffset) / oldScale) * newScale;

        currentScale = newScale;
        Content.Scale = currentScale;
        Content.TranslationX = ClampX(targetX, currentScale);
        Content.TranslationY = ClampY(targetY, currentScale);

        xOffset = Content.TranslationX;
        yOffset = Content.TranslationY;
        panX = xOffset;
        panY = yOffset;

        RaiseViewportChanged();
    }

    // 约束 X 方向偏移。
    // 如果内容比容器窄，则直接居中；否则限制在左右边界之间。
    private double ClampX(double x, double scale)
    {
        double scaledWidth = LayerWidth * scale;

        if (scaledWidth <= Width)
            return GetCenteredX(scale);

        double minX = Width - scaledWidth;
        double maxX = 0;
        return Math.Clamp(x, minX, maxX);
    }

    // 约束 Y 方向偏移。
    // 如果内容比容器矮，则直接居中；否则限制在上下边界之间。
    private double ClampY(double y, double scale)
    {
        double scaledHeight = LayerHeight * scale;

        if (scaledHeight <= Height)
            return GetCenteredY(scale);

        double minY = Height - scaledHeight;
        double maxY = 0;
        return Math.Clamp(y, minY, maxY);
    }

    // 计算当前缩放下的水平居中偏移。
    private double GetCenteredX(double scale)
    {
        return (Width - LayerWidth * scale) / 2.0;
    }

    // 计算当前缩放下的垂直居中偏移。
    private double GetCenteredY(double scale)
    {
        return (Height - LayerHeight * scale) / 2.0;
    }

    // 在 MAUI Handler 创建后，挂接 Windows 原生滚轮事件。
    private void OnViewHandlerChanged(object? sender, EventArgs e)
    {
#if WINDOWS
        DetachWindowsWheel();
        _platformView = Handler?.PlatformView as FrameworkElement;
        if (_platformView != null)
            _platformView.PointerWheelChanged += OnWindowsPointerWheelChanged;
#endif
    }

    // Handler 切换时先解绑旧的原生事件，避免重复订阅。
    private void OnViewHandlerChanging(object? sender, HandlerChangingEventArgs e)
    {
#if WINDOWS
        DetachWindowsWheel(e.OldHandler);
#endif
    }

#if WINDOWS
    // 统一处理滚轮事件解绑逻辑。
    private void DetachWindowsWheel(Microsoft.Maui.IElementHandler? oldHandler = null)
    {
        var view = oldHandler?.PlatformView as FrameworkElement ?? _platformView;
        if (view != null)
            view.PointerWheelChanged -= OnWindowsPointerWheelChanged;

        if (oldHandler == null)
            _platformView = null;
    }

    // 处理 Windows 鼠标滚轮缩放。
    private void OnWindowsPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (sender is not FrameworkElement nativeView)
            return;

        var pt = e.GetCurrentPoint(nativeView);
        double factor = pt.Properties.MouseWheelDelta > 0 ? 1.1 : 1.0 / 1.1;

        ZoomAt(pt.Position.X, pt.Position.Y, factor);
        e.Handled = true;
    }
#endif

    // 通知外部当前视口发生了变化，外部常用它同步覆盖层绘制。
    private void RaiseViewportChanged()
    {
        ViewportChanged?.Invoke(this, EventArgs.Empty);
    }

    // 仅允许真正的 Windows 鼠标左键拖动进入鼠标平移逻辑。
    // 触屏、触控板或其他 Pointer 类型都不走这里。
    private bool IsWindowsMouseLeftDrag(PointerEventArgs e)
    {
#if WINDOWS
    var pe = e.PlatformArgs?.PointerRoutedEventArgs;
    var sender = e.PlatformArgs?.Sender;
    if (pe == null || sender == null)
        return false;

    if (pe.Pointer.PointerDeviceType != Microsoft.UI.Input.PointerDeviceType.Mouse)
        return false;

    var point = pe.GetCurrentPoint(sender);
    return point.Properties.IsLeftButtonPressed;
#else
        return false;
#endif
    }
}
