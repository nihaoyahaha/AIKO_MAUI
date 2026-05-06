using CommunityToolkit.Mvvm.ComponentModel;

namespace Aiko.Common;

/// <summary>
/// カメラ画面の緑色背景板実体類
/// </summary>
public partial class GreenBackgroundModel : ObservableValidator
{
	/// <summary>
	/// 工事名
	/// </summary>
	[ObservableProperty]
	public partial string ConstructionName { get; set; } = string.Empty;

	/// <summary>
	/// 工区名
	/// </summary>
	[ObservableProperty]
	public partial string WorkAreaName { get; set; } = string.Empty;

	/// <summary>
	/// 部位名
	/// </summary>
	[ObservableProperty]
	public partial string RegionalName { get; set; } = string.Empty;

	/// <summary>
	/// 断面名
	/// </summary>
	[ObservableProperty]
	public partial string SectionName { get; set; } = string.Empty;

	/// <summary>
	/// 位置
	/// </summary>
	[ObservableProperty]
	public partial string Position { get; set; } = string.Empty; 

	/// <summary>
	/// 工程名
	/// </summary>
	[ObservableProperty]
	public partial string ProjectName { get; set; }= string.Empty;

	/// <summary>
	/// 確認項目
	/// </summary>
	[ObservableProperty]
	public partial string ConfirmProject { get; set; }=string.Empty;

	/// <summary>
	/// 撮影日
	/// </summary>
	[ObservableProperty]
	public partial string ShootingDate { get; set; } = string.Empty;

	/// <summary>
	/// 備考
	/// </summary>
	[ObservableProperty]
	public partial string Remark { get; set; } = string.Empty;

	/// <summary>
	/// 確認者
	/// </summary>
	[ObservableProperty]
	public partial string Confirmer { get; set; } = string.Empty;

	/// <summary>
	/// 施工者
	/// </summary>
	[ObservableProperty]
	public partial string Constructor { get; set; } = string.Empty;

	/// <summary>
	/// 断面図
	/// </summary>
	[ObservableProperty]
	public partial ImageSource SectionalDrawing { get; set; }

	/// <summary>
	/// 備考と確認者の切り替え展示
	/// </summary>
	[ObservableProperty]
	public partial string Describe { get; set; } = string.Empty;

	/// <summary>
	/// 備考と確認者の切り替え展示明細
	/// </summary>
	[ObservableProperty]
	public partial string DescribeDetail { get; set; } = string.Empty;

	/// <summary>
	/// 非表示を表示
	/// </summary>
	[ObservableProperty]
	public partial bool IsVisible { get; set; }

	/// <summary>
	/// 断面図の表示
	/// </summary>
	[ObservableProperty]
	public partial bool IsSectionalDrawingVisible { get; set;}

	/// <summary>
	/// 緑色背景板内のフォントサイズ
	/// </summary>
	[ObservableProperty]
	public partial double FontSize { get; set; }

}
