using Aiko.UI.ViewModels.PageVMs;
using MetroLog.Maui;

namespace Aiko.UI;

public partial class LogPage : ContentPage
{
	public LogPage(LogPageVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}