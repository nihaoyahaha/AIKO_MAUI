using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class UploadPage : ContentPage
{
	public UploadPage(UploadPageVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
