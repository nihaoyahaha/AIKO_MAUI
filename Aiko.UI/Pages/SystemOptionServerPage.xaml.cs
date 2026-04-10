using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class SystemOptionServerPage : ContentView
{
    public SystemOptionServerPage(SystemOptionServerPageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
