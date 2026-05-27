using Aiko.UI.ViewModels.UserControlVms;

namespace Aiko.UI.CustomControls;

public partial class PunchListPopup : ContentView
{
	public PunchListPopup(PunchListPopupVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
