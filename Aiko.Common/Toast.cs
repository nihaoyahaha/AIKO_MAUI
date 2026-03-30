using CommunityToolkit.Mvvm.ComponentModel;

namespace Aiko.Common;

public partial class Toast : ObservableObject
{
    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private double _opacity;

    public async Task ShowToast(string message, int duration = 2000)
    {
        Message = message;
        IsVisible = true;
        Opacity = 1;

        await Task.Delay(duration);

        int steps = 10;
        for (int i = 0; i < steps; i++)
        {
            await Task.Delay(20);
            Opacity = Math.Max(0, Opacity - 1.0 / steps);
        }

        IsVisible = false;
    }
}