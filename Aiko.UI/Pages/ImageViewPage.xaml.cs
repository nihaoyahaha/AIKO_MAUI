using Aiko.UI.ViewModels.PageVMs;
using System.Globalization;
namespace Aiko.UI;

public partial class ImageViewPage : ContentPage
{
	public ImageViewPage(ImageViewPageVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
		vm.ZoomContainer = pinchToZoom;
	}
}

public class PictureFormatToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null || !(value is int))
		{
			return false;
		}

		var format = (int)value == 1 ? "Svg" : "Jpeg";

		var targetFormat = parameter as string;

		if (string.IsNullOrEmpty(targetFormat))
		{
			return false;
		}
		return format == targetFormat ? true : false;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}