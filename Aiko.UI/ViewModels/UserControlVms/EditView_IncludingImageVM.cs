using CommunityToolkit.Mvvm.ComponentModel;

namespace Aiko.UI.ViewModels.UserControlVms;

public partial class  EditView_IncludingImageVM: ObservableValidator, IQueryAttributable
{
	/// <summary>
	/// 工事名
	/// </summary>
	[ObservableProperty]
	private string constructionName;

	/// <summary>
	/// 工区名
	/// </summary>
	[ObservableProperty]
	private string workAreaName;

	/// <summary>
	/// 部位名
	/// </summary>
	[ObservableProperty]
	private string regionalName;

	/// <summary>
	/// 断面名
	/// </summary>
	[ObservableProperty]
	private string sectionName;

	/// <summary>
	/// 位置
	/// </summary>
	[ObservableProperty]
	private string position;

	/// <summary>
	/// 工程名
	/// </summary>
	[ObservableProperty]
	private string projectName;

	/// <summary>
	/// 確認項目
	/// </summary>
	[ObservableProperty]
	private string confirmProject;

	/// <summary>
	/// 撮影日
	/// </summary>
	[ObservableProperty]
	private string shootingDate;

	/// <summary>
	/// 備考
	/// </summary>
	[ObservableProperty]
	private string remark;

	/// <summary>
	/// 確認者
	/// </summary>
	[ObservableProperty]
	private string confirmer;

	/// <summary>
	/// 施工者
	/// </summary>
	[ObservableProperty]
	private string constructor;

	/// <summary>
	/// 断面図
	/// </summary>
	[ObservableProperty]
	private ImageSource sectionalDrawing;

	/// <summary>
	/// 備考と確認者の切り替え展示
	/// </summary>
	[ObservableProperty]
	private string describe;

	/// <summary>
	/// 備考と確認者の切り替え展示明細
	/// </summary>
	[ObservableProperty]
	private string describeDetail;

	/// <summary>
	/// 非表示を表示
	/// </summary>
	[ObservableProperty]
	private bool isVisible;

	/// <summary>
	/// 断面図の表示
	/// </summary>
	[ObservableProperty]
	private bool isSectionalDrawingVisible;

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{

	}
}
