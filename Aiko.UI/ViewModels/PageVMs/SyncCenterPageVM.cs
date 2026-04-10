using Aiko.Common;
using Aiko.UI.CustomControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class SyncCenterPageVM : ObservableObject
{
    readonly AikoAppContext _appContext;
    readonly SegmentedTabItem _downloadTabItem;
    readonly SegmentedTabItem _uploadTabItem;
    readonly SegmentedTabItem _logTabItem;
    readonly SegmentedTabItem _deleteTabItem;

    [ObservableProperty]
    private bool isUploadTabVisible = false;

    [ObservableProperty]
    private bool isDeleteTabVisible = false;

    [ObservableProperty]
    private SyncCenterTab selectedTab = SyncCenterTab.Download;

    public IReadOnlyList<SegmentedTabItem> Tabs { get; }

    public bool IsDownloadSelected => SelectedTab == SyncCenterTab.Download;
    public bool IsUploadSelected => SelectedTab == SyncCenterTab.Upload;
    public bool IsLogSelected => SelectedTab == SyncCenterTab.Log;
    public bool IsDeleteSelected => SelectedTab == SyncCenterTab.Delete;

    public SyncCenterPageVM(AikoAppContext appContext)
    {
        _appContext = appContext;
        isUploadTabVisible = _appContext.IsLogin;
        isDeleteTabVisible = _appContext.IsLogin;

        _downloadTabItem = new SegmentedTabItem
        {
            Text = "ダウンロード",
            Command = SelectDownloadCommand,
            IsSelected = true
        };
        _uploadTabItem = new SegmentedTabItem
        {
            Text = "同　期",
            Command = SelectUploadCommand,
            IsVisible = isUploadTabVisible
        };
        _logTabItem = new SegmentedTabItem
        {
            Text = "ロ　グ",
            Command = SelectLogCommand
        };
        _deleteTabItem = new SegmentedTabItem
        {
            Text = "削　除",
            Command = SelectDeleteCommand,
            IsVisible = isDeleteTabVisible
        };

        Tabs = new[] { _downloadTabItem, _uploadTabItem, _logTabItem, _deleteTabItem };
        UpdateTabStates();

        WeakReferenceMessenger.Default.Register<string, string>(this, "LoginOrLogoutToken", (_, message) =>
        {
            var isLogin = message == "login";
            IsUploadTabVisible = isLogin;
            IsDeleteTabVisible = isLogin;
            _uploadTabItem.IsVisible = isLogin;
            _deleteTabItem.IsVisible = isLogin;

            if (!isLogin && (SelectedTab == SyncCenterTab.Upload || SelectedTab == SyncCenterTab.Delete))
            {
                SelectedTab = SyncCenterTab.Download;
            }
        });
    }

    [RelayCommand]
    private void SelectDownload() => SelectedTab = SyncCenterTab.Download;

    [RelayCommand]
    private void SelectUpload()
    {
        if (IsUploadTabVisible)
        {
            SelectedTab = SyncCenterTab.Upload;
        }
    }

    [RelayCommand]
    private void SelectLog() => SelectedTab = SyncCenterTab.Log;

    [RelayCommand]
    private void SelectDelete()
    {
        if (IsDeleteTabVisible)
        {
            SelectedTab = SyncCenterTab.Delete;
        }
    }

    partial void OnSelectedTabChanged(SyncCenterTab value)
    {
        OnPropertyChanged(nameof(IsDownloadSelected));
        OnPropertyChanged(nameof(IsUploadSelected));
        OnPropertyChanged(nameof(IsLogSelected));
        OnPropertyChanged(nameof(IsDeleteSelected));
        UpdateTabStates();
    }

    void UpdateTabStates()
    {
        _downloadTabItem.IsSelected = SelectedTab == SyncCenterTab.Download;
        _uploadTabItem.IsSelected = SelectedTab == SyncCenterTab.Upload;
        _logTabItem.IsSelected = SelectedTab == SyncCenterTab.Log;
        _deleteTabItem.IsSelected = SelectedTab == SyncCenterTab.Delete;
    }
}

public enum SyncCenterTab
{
    Download,
    Upload,
    Log,
    Delete
}
