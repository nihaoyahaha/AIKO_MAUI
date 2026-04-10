using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class AppInfoPage : ContentPage
{
	public AppInfoPage(AppInfoPageVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}