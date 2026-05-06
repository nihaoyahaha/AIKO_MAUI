using CommunityToolkit.Mvvm.ComponentModel;

namespace Aiko.UI.ViewModels.UserControlVms;

public partial class  EditView_IncludingImageVM: ObservableValidator, IQueryAttributable
{
	/// <summary>
	/// 工事名
	/// </summary>
	[ObservableProperty]
	public partial string ConstructionName { get; set; }

	/// <summary>
	/// 工区名
	/// </summary>
	[ObservableProperty]
	public partial string WorkAreaName { get; set; }

	/// <summary>
	/// 部位名
	/// </summary>
	[ObservableProperty]
	public partial string RegionalName { get; set; }

	/// <summary>
	/// 断面名
	/// </summary>
	[ObservableProperty]
	public partial string SectionName { get; set; }

	/// <summary>
	/// 位置
	/// </summary>
	[ObservableProperty]
	public partial string Position { get; set; }

	/// <summary>
	/// 工程名
	/// </summary>
	[ObservableProperty]
	public partial string ProjectName { get; set; }

	/// <summary>
	/// 確認項目
	/// </summary>
	[ObservableProperty]
	public partial string ConfirmProject { get; set; }

	/// <summary>
	/// 撮影日
	/// </summary>
	[ObservableProperty]
	public partial string ShootingDate { get; set; }

	/// <summary>
	/// 備考
	/// </summary>
	[ObservableProperty]
	public partial string Remark { get; set; }

	/// <summary>
	/// 確認者
	/// </summary>
	[ObservableProperty]
	public partial string Confirmer { get; set; }

	/// <summary>
	/// 施工者
	/// </summary>
	[ObservableProperty]
	public partial string Constructor { get; set; }

	/// <summary>
	/// 断面図
	/// </summary>
	[ObservableProperty]
	public partial ImageSource SectionalDrawing { get; set; }

	/// <summary>
	/// 備考と確認者の切り替え展示
	/// </summary>
	[ObservableProperty]
	public partial string Describe { get; set; }

	/// <summary>
	/// 備考と確認者の切り替え展示明細
	/// </summary>
	[ObservableProperty]
	public partial string DescribeDetail { get; set; }

	/// <summary>
	/// 非表示を表示
	/// </summary>
	[ObservableProperty]
	public partial bool IsVisible { get; set; }

	/// <summary>
	/// 断面図の表示
	/// </summary>
	[ObservableProperty]
	public partial bool IsSectionalDrawingVisible { get; set; }

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{

	}
}
