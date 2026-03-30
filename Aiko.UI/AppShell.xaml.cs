using CommunityToolkit.Mvvm.Messaging;

namespace Aiko.UI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		WeakReferenceMessenger.Default.Register<string, string>(this, "LoginOrLogoutToken", (page, message) =>
		{
			if (message == "login")
			{
				shellContent_Delete.IsVisible = true;
				shellContent_Upload.IsVisible = true;
			}
			else
			{
				shellContent_Delete.IsVisible = false;
				shellContent_Upload.IsVisible = false;
			}
		});
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
	}
}

