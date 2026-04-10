using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class RemotingFtpServerPage : ContentView
{
    public RemotingFtpServerPage(RemotingFtpServerPageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
