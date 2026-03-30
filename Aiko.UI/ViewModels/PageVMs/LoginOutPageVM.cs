using Aiko.Common;
using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.UI.ViewModels.PageVMs
{
    public partial class LoginOutPageVM : Observablebase<LoginOutPageVM, ILoginOutService>
	{
		public LoginOutPageVM(ILogger<LoginOutPageVM> logger, ILoginOutService service) : base(logger, service)
		{
		}

		[RelayCommand]
        private async Task Logout()
        {
            Service.AppContext.IsLogin = false;
			WeakReferenceMessenger.Default.Send("logout", "LoginOrLogoutToken");
			WeakReferenceMessenger.Default.Send("Leave", "EnterOrLeaveLogoutPageToken");
			await Shell.Current.GoToAsync("//Login");
        }

        [RelayCommand]
        private async Task Main()
        {
			WeakReferenceMessenger.Default.Send("Leave", "EnterOrLeaveLogoutPageToken");
            await Shell.Current.GoToAsync("//Login/MapView?FromPage=LoginOutPage");
        }

        [RelayCommand]
        private async Task Exit()
        {
            // 直接退出应用
            Application.Current.Quit();
        }
    }
}
