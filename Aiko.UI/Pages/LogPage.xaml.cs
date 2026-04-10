using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class LogPage : ContentView
{
    public LogPage(LogPageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
