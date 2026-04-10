using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;
public partial class SwitchThemePage : ContentPage
{
	public SwitchThemePage(SwitchThemePageVM vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}
}
