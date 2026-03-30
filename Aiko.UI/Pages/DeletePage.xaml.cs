using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class DeletePage : ContentPage
{
	public DeletePage(DeletePageVM vm)
	{
		InitializeComponent();
		BindingContext=vm;
	}
}