using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class UploadPage : ContentView
{
    public UploadPage(UploadPageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
