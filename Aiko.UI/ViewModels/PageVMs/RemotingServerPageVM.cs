using Aiko.Common;
using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class RemotingServerPageVM : Observablebase<RemotingServerPageVM, IRemotingService>
{
    public RemotingServerPageVM(ILogger<RemotingServerPageVM> logger, IRemotingService service) : base(logger, service)
    {

    }

    [ObservableProperty]
    private string server;

    [ObservableProperty]
    private string serverPort;

    [ObservableProperty]
    private string serverTimeOut;

    [RelayCommand]
    private void PageLoad()
    {
        Server = Preferences.Default.Get("Server", "");
        ServerPort = Preferences.Default.Get("ServerPort", "");
        ServerTimeOut = Preferences.Default.Get("ServerTimeOut", "");
    }

    [RelayCommand]
    private async void Save()
    {

        int ret = Service.CheckWcfAsync(Server, ServerPort, ServerTimeOut);

        if (ret == 1)
        {
            Preferences.Default.Set("Server", Server);
            Preferences.Default.Set("ServerPort", ServerPort);
            Preferences.Default.Set("ServerTimeOut", ServerTimeOut);

            DialogHelper.MessageDialogOk("保存に成功しました。");
        }
        else 
        {
            await DialogHelper.MessageDialogButton1(ErrorMessage.ERRORPOP("CM00006"));
        }
    }
}
