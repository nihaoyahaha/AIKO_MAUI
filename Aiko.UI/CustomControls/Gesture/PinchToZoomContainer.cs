#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#endif

namespace Aiko.UI;

public class PinchToZoomContainer : ContentView
{
    private const double MinScale = 0.5;
    private const double MaxScale = 8.0;

    private double currentScale = 1.0;
    private double startScale = 1.0;

    private double xOffset = 0.0;
    private double yOffset = 0.0;

    private double panX = 0.0;
    private double panY = 0.0;

    private bool isPinching = false;

    public double LayerWidth { get; private set; }
    public double LayerHeight { get; private set; }

    // 鼠标拖动
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

    public bool IsAtInitialScale => currentScale < 1.001;//Math.Abs(currentScale - 1.0) < 0.001;

	#endregion

	public PinchToZoomContainer()
    {
        var pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += OnPinchUpdated;
        GestureRecognizers.Add(pinchGesture);

        var panGesture = new PanGestureRecognizer();
        panGesture.TouchPoints = 1;
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerPressed += OnPointerPressed;
        pointerGesture.PointerMoved += OnPointerMoved;
        pointerGesture.PointerReleased += OnPointerReleased;
        pointerGesture.PointerExited += OnPointerReleased;
        GestureRecognizers.Add(pointerGesture);

        SizeChanged += (_, _) => RefreshBounds();

        HandlerChanged += OnViewHandlerChanged;
        HandlerChanging += OnViewHandlerChanging;
    }

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

    private void RefreshBounds()
    {
        if (Content == null || Width <= 0 || Height <= 0 || LayerWidth <= 0 || LayerHeight <= 0)
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

    private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (Content == null || Width <= 0 || Height <= 0 || LayerWidth <= 0 || LayerHeight <= 0)
            return;

        switch (e.Status)
        {
            case GestureStatus.Started:
                isPinching = true;
                startScale = currentScale;

                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
                panX = xOffset;
                panY = yOffset;
                break;

            case GestureStatus.Running:
                currentScale += (e.Scale - 1.0) * startScale;
                currentScale = Math.Clamp(currentScale, MinScale, MaxScale);

                double targetX = CalculatePinchX(e.ScaleOrigin.X * Width);
                double targetY = CalculatePinchY(e.ScaleOrigin.Y * Height);

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
                break;

            case GestureStatus.Running:
                Content.TranslationX = ClampX(panX + e.TotalX, currentScale);
                Content.TranslationY = ClampY(panY + e.TotalY, currentScale);
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                panX = Content.TranslationX;
                panY = Content.TranslationY;
                xOffset = panX;
                yOffset = panY;
                break;
        }

        RaiseViewportChanged();
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e)
    {
        if (Content == null || isPinching)
            return;

        var pos = e.GetPosition(this);
        if (pos is null)
            return;

        isMouseDragging = true;
        mouseStartPoint = pos.Value;
        mouseStartX = Content.TranslationX;
        mouseStartY = Content.TranslationY;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (Content == null || !isMouseDragging || isPinching)
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

    private double CalculatePinchX(double focusX)
    {
        double scaledWidth = LayerWidth * currentScale;

        if (scaledWidth <= Width)
            return GetCenteredX(currentScale);

        if (startScale <= 0)
            return xOffset;

        return focusX - ((focusX - xOffset) / startScale) * currentScale;
    }

    private double CalculatePinchY(double focusY)
    {
        double scaledHeight = LayerHeight * currentScale;

        if (scaledHeight <= Height)
            return GetCenteredY(currentScale);

        if (startScale <= 0)
            return yOffset;

        return focusY - ((focusY - yOffset) / startScale) * currentScale;
    }

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

    private double ClampX(double x, double scale)
    {
        double scaledWidth = LayerWidth * scale;

        if (scaledWidth <= Width)
            return GetCenteredX(scale);

        double minX = Width - scaledWidth;
        double maxX = 0;
        return Math.Clamp(x, minX, maxX);
    }

    private double ClampY(double y, double scale)
    {
        double scaledHeight = LayerHeight * scale;

        if (scaledHeight <= Height)
            return GetCenteredY(scale);

        double minY = Height - scaledHeight;
        double maxY = 0;
        return Math.Clamp(y, minY, maxY);
    }

    private double GetCenteredX(double scale)
    {
        return (Width - LayerWidth * scale) / 2.0;
    }

    private double GetCenteredY(double scale)
    {
        return (Height - LayerHeight * scale) / 2.0;
    }

    private void OnViewHandlerChanged(object? sender, EventArgs e)
    {
#if WINDOWS
        DetachWindowsWheel();
        _platformView = Handler?.PlatformView as FrameworkElement;
        if (_platformView != null)
            _platformView.PointerWheelChanged += OnWindowsPointerWheelChanged;
#endif
    }

    private void OnViewHandlerChanging(object? sender, HandlerChangingEventArgs e)
    {
#if WINDOWS
        DetachWindowsWheel(e.OldHandler);
#endif
    }

#if WINDOWS
    private void DetachWindowsWheel(Microsoft.Maui.IElementHandler? oldHandler = null)
    {
        var view = oldHandler?.PlatformView as FrameworkElement ?? _platformView;
        if (view != null)
            view.PointerWheelChanged -= OnWindowsPointerWheelChanged;

        if (oldHandler == null)
            _platformView = null;
    }

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

    private void RaiseViewportChanged()
    {
        ViewportChanged?.Invoke(this, EventArgs.Empty);
    }
}