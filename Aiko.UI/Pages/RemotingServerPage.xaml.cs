using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class RemotingServerPage : ContentPage
{
	public RemotingServerPage(RemotingServerPageVM remotingserverVM)
	{
		InitializeComponent();
		BindingContext = remotingserverVM;
    }
}
