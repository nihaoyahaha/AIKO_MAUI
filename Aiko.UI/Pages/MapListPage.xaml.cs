using Aiko.UI.ViewModels.PageVMs;
using System.ComponentModel;

namespace Aiko.UI;

public partial class MapListPage : ContentPage
{
    double showW = 0;
    double showH = 0;

    // 原始尺寸
    private double originalWidth = 0;
    private double originalHeight = 0;

    private MapListPageVM? _vm;

    public MapListPage(MapListPageVM mapListPageVM)
	{
		InitializeComponent();
        BindingContext = mapListPageVM;

        // 监听 MainGridView 的尺寸变化，确保旋转或调整大小时图片依然填满宽度
        MainGridView.SizeChanged += (s, e) => UpdateImageDimensions();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (_vm != null)
            _vm.PropertyChanged -= Vm_PropertyChanged;

        _vm = BindingContext as MapListPageVM;

        if (_vm != null)
            _vm.PropertyChanged += Vm_PropertyChanged;
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapListPageVM.CurrentImage))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await RefreshCurrentImageAsync();
            });
        }
    }

    private async Task RefreshCurrentImageAsync()
    {
        originalWidth = 0;
        originalHeight = 0;
        showW = 0;
        showH = 0;

        // 重置旧图片状态
        pinchToZoom.Reset();

        if (ContainerImg.Source == null)
            return;

        for (int i = 0; i < 10; i++)
        {
            var size = await GetOriginalImageSize(ContainerImg);
            if (size.Width > 0 && size.Height > 0)
            {
                originalWidth = size.Width;
                originalHeight = size.Height;

                UpdateImageDimensions();
                return;
            }

            await Task.Delay(50);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetTitleView(this, null);
    }

    /// <summary>
    /// 图片加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnContainerImgLoaded(object sender, EventArgs e)
    {
        if (originalWidth > 0 && originalHeight > 0)
            return;

        await RefreshCurrentImageAsync();
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

    private void OnSwipeLeft(object sender, SwipedEventArgs e)
    {
        if (!pinchToZoom.IsAtInitialScale)
            return;

        if (BindingContext is MapListPageVM vm)
            vm.NextImageCommand.Execute(null);
    }

    private void OnSwipeRight(object sender, SwipedEventArgs e)
    {
        if (!pinchToZoom.IsAtInitialScale)
            return;

        if (BindingContext is MapListPageVM vm)
            vm.PreviousImageCommand.Execute(null);
    }

}