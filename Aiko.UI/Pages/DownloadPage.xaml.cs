using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class DownloadPage : ContentView
{
    public DownloadPage(DownloadPageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
