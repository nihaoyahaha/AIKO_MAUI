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
	private string _text;

	/// <summary>
	/// チェック状態
	/// </summary>
	[ObservableProperty]
	private bool _isSelected;
}
