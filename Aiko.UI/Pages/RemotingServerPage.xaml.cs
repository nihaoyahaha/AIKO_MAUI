using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class RemotingServerPage : ContentView
{
    public RemotingServerPage(RemotingServerPageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
