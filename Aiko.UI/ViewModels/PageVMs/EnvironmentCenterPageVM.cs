using Aiko.UI.CustomControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class EnvironmentCenterPageVM : ObservableObject
{
    readonly SegmentedTabItem _remotingServerTabItem;
    readonly SegmentedTabItem _systemOptionTabItem;
    readonly SegmentedTabItem _ftpTabItem;

    [ObservableProperty]
    private EnvironmentCenterTab selectedTab = EnvironmentCenterTab.RemotingServer;

    public IReadOnlyList<SegmentedTabItem> Tabs { get; }

    public bool IsRemotingServerSelected => SelectedTab == EnvironmentCenterTab.RemotingServer;
    public bool IsSystemOptionSelected => SelectedTab == EnvironmentCenterTab.SystemOption;
    public bool IsFtpSelected => SelectedTab == EnvironmentCenterTab.Ftp;

    public EnvironmentCenterPageVM()
    {
        _remotingServerTabItem = new SegmentedTabItem
        {
            Text = "遠隔サーバー",
            Command = SelectRemotingServerCommand,
            IsSelected = true
        };
        _systemOptionTabItem = new SegmentedTabItem
        {
            Text = "オプション",
            Command = SelectSystemOptionCommand
        };
        _ftpTabItem = new SegmentedTabItem
        {
            Text = "FTP接続",
            Command = SelectFtpCommand
        };

        Tabs = new[] { _remotingServerTabItem, _systemOptionTabItem, _ftpTabItem };
        UpdateTabStates();
    }

    [RelayCommand]
    private void SelectRemotingServer() => SelectedTab = EnvironmentCenterTab.RemotingServer;

    [RelayCommand]
    private void SelectSystemOption() => SelectedTab = EnvironmentCenterTab.SystemOption;

    [RelayCommand]
    private void SelectFtp() => SelectedTab = EnvironmentCenterTab.Ftp;

    partial void OnSelectedTabChanged(EnvironmentCenterTab value)
    {
        OnPropertyChanged(nameof(IsRemotingServerSelected));
        OnPropertyChanged(nameof(IsSystemOptionSelected));
        OnPropertyChanged(nameof(IsFtpSelected));
        UpdateTabStates();
    }

    void UpdateTabStates()
    {
        _remotingServerTabItem.IsSelected = SelectedTab == EnvironmentCenterTab.RemotingServer;
        _systemOptionTabItem.IsSelected = SelectedTab == EnvironmentCenterTab.SystemOption;
        _ftpTabItem.IsSelected = SelectedTab == EnvironmentCenterTab.Ftp;
    }
}

public enum EnvironmentCenterTab
{
    RemotingServer,
    SystemOption,
    Ftp
}
