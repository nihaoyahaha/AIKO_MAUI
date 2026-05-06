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
