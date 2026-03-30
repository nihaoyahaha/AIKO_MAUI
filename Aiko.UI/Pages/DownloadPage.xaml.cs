using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class DownloadPage : ContentPage
{
    public DownloadPage(DownloadPageVM downloadPageVM)
    {
        InitializeComponent();
        BindingContext = downloadPageVM;
    }
}
