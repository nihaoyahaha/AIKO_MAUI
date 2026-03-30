using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class RemotingFtpServerPage : ContentPage
{
	public RemotingFtpServerPage(RemotingFtpServerPageVM remotingftpserverVM)
	{
		InitializeComponent();
		BindingContext = remotingftpserverVM;

    }
}
