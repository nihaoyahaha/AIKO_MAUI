using Aiko.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class RemotingFtpServerPageVM: ObservableValidator
{
    [ObservableProperty]
    public partial string FtpServerProxyType { get; set; }

    [ObservableProperty]
    public partial string FtpServerProxy { get; set; }

    [ObservableProperty]
    public partial string FtpServerProxyPort { get; set; }

    [RelayCommand]
    private void PageLoad()
    {
        FtpServerProxyType = Preferences.Default.Get("FtpServerProxyType", "");
        FtpServerProxy = Preferences.Default.Get("FtpServerProxy", "");
        FtpServerProxyPort = Preferences.Default.Get("FtpServerProxyPort", "");
    }

    [RelayCommand]
    private async void Save()
    {
        Preferences.Default.Set("FtpServerProxyType", FtpServerProxyType);
        Preferences.Default.Set("FtpServerProxy", FtpServerProxy);
        Preferences.Default.Set("FtpServerProxyPort", FtpServerProxyPort);

        DialogHelper.MessageDialogOk("保存に成功しました。");
    }
}
