using CommunityToolkit.Mvvm.ComponentModel;

namespace Aiko.Common;

/// <summary>
/// 指摘事項
/// </summary>
public partial class PunchListModel:ObservableObject
{
    /// <summary>
    /// 指摘事項メモ本文
    /// </summary>
    [ObservableProperty]
    public partial string Text { get; set; } = string.Empty;
 
    /// <summary>
    /// チェック状態
    /// </summary>
    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}
