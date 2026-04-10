using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class DeletePage : ContentView
{
    public DeletePage(DeletePageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
