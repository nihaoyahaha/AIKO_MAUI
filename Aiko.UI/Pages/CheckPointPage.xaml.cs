using Aiko.UI.ViewModels.PageVMs;
using CommunityToolkit.Mvvm.Messaging;

namespace Aiko.UI;

public partial class CheckPointPage : ContentPage
{
    double showW = 0;
    double showH = 0;

    // 原始尺寸
    private double originalWidth = 0;
	private double originalHeight = 0;

	public CheckPointPage(CheckPointPageVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
		
		// 监听 MainGridView 的尺寸变化，确保旋转或调整大小时图片依然填满宽度
		MainGridView.SizeChanged += (s, e) => UpdateImageDimensions();
		WeakReferenceMessenger.Default.Register<string, string>(this, "RefreshCheckPointPageImageToken", async (page, message) =>
		{
			OnContainerImgLoaded(null,null);
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

}

public class MyDataTemplateSelector : DataTemplateSelector
{
	public DataTemplate? DataTemplateDark { get; set; }
	public DataTemplate? DataTemplateLight { get; set; }

	protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
	{
		var app = Application.Current;
		if (app?.RequestedTheme == AppTheme.Dark)
		{
			return DataTemplateDark!;
		}

		return DataTemplateLight!;
	}
}
