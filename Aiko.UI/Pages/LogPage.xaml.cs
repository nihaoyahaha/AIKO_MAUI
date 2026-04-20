using Aiko.UI.ViewModels.PageVMs;
using System.Globalization;

namespace Aiko.UI;

public partial class LogPage : ContentView
{
    public LogPage(LogPageVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

public class SelectedToStrokeThicknessConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		bool isSelected = (bool)value;
		return isSelected ? 2 : 0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
