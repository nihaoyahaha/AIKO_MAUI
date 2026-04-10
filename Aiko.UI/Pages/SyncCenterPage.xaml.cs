using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class SyncCenterPage : ContentPage
{
    public SyncCenterPage(
        SyncCenterPageVM vm,
        DownloadPage downloadPage,
        UploadPage uploadPage,
        LogPage logPage,
        DeletePage deletePage)
    {
        InitializeComponent();
        BindingContext = vm;

        downloadHost.Content = downloadPage;
        uploadHost.Content = uploadPage;
        logHost.Content = logPage;
        deleteHost.Content = deletePage;
    }
}
