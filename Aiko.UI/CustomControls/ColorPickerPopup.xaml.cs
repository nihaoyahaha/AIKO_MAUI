using Aiko.UI.ViewModels.UserControlVms;

namespace Aiko.UI.CustomControls;

public partial class ColorPickerPopup : ContentView
{
	public ColorPickerPopup(ColorPickerPopupVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
