using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class LoginOutPage : ContentPage
{
	public LoginOutPage(LoginOutPageVM vm)
	{
		InitializeComponent();
#if IOS
    btnClose.IsVisible = false;
#endif
		BindingContext = vm;
    }

}