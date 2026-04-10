using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class EnvironmentCenterPage : ContentPage
{
    public EnvironmentCenterPage(
        EnvironmentCenterPageVM vm,
        RemotingServerPage remotingServerPage,
        SystemOptionServerPage systemOptionServerPage,
        RemotingFtpServerPage remotingFtpServerPage)
    {
        InitializeComponent();
        BindingContext = vm;

        remotingServerHost.Content = remotingServerPage;
        systemOptionHost.Content = systemOptionServerPage;
        ftpHost.Content = remotingFtpServerPage;
    }
}
