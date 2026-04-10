using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Aiko.UI.CustomControls;

public partial class SegmentedTabItem : ObservableObject
{
    [ObservableProperty]
    private string text = string.Empty;

    [ObservableProperty]
    private ICommand? command;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isVisible = true;
}
