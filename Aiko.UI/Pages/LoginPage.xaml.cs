using Aiko.UI.ViewModels.PageVMs;
namespace Aiko.UI;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageVM loginVM)
    {
        InitializeComponent();
#if IOS
    btn_Exit.IsVisible = false;
#endif
		BindingContext = loginVM;
    }


}
