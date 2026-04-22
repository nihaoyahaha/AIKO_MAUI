using Aiko.Common;
using Aiko.UI.ViewModels.PageVMs;
using CommunityToolkit.Mvvm.Messaging;

namespace Aiko.UI;

public partial class AppShell : Shell
{
	readonly AikoAppContext _appContext;
	readonly string _title = "配筋検査システム";
	public AppShell(AikoAppContext aikoApp)
	{
		_appContext = aikoApp;
		InitializeComponent();

        var appName = _appContext.AppName;
        var version = _appContext.AppVersion;

        this.Title = $"Ver {version} {appName}";

        WeakReferenceMessenger.Default.Register<string, string>(this, "EnterOrLeaveLogoutPageToken", (page, message) =>
		{
			if (message == "Enter")
			{
				flyout_logout.IsVisible = true;
				CurrentItem = flyout_logout;
				flyout_login.IsVisible = false;
			}
			else
			{
				flyout_login.IsVisible = true;
				CurrentItem = flyout_login;
				flyout_logout.IsVisible = false;
			}
		});

		WeakReferenceMessenger.Default.Register<string, string>(this, "LoginOrLogoutToken", (_, message) =>
		{
			var isLogin = message == "login";
			lb_title.Text = isLogin ? _appContext.WorkNameExcludeCode : _title;
		});
	}
}

