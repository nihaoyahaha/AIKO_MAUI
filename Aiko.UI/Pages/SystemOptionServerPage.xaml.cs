using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class SystemOptionServerPage : ContentPage
{
	public SystemOptionServerPage(SystemOptionServerPageVM systemoptionserverVM)
	{
		InitializeComponent();
        BindingContext = systemoptionserverVM;
    }
}
