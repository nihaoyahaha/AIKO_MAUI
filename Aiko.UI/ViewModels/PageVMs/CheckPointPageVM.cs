using Aiko.Common;
using Aiko.Common.Models;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Aiko.UI.ViewModels.UserControlVms;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
#if WINDOWS
using Windows.Devices.Enumeration;
using Windows.Security.Authorization.AppCapabilityAccess;
using static Microsoft.Maui.ApplicationModel.Permissions;

#endif

namespace Aiko.UI.ViewModels.PageVMs;

public partial class CheckPointPageVM : Observablebase<CheckPointPageVM, ICheckPointService>
{
	private readonly IPopupService _popupService;
	public CheckPointPageVM(ICheckPointService service, ILogger<CheckPointPageVM> logger, IPopupService popupService) : base(logger, service)
	{
		_popupService = popupService;
		WeakReferenceMessenger.Default.Register<string, string>(this, "TakePhotosToken", (page, message) => TakePhotos(message));
		WeakReferenceMessenger.Default.Register<string, string>(this, "DeletePhotosToken", (page, message) => DeletePhotos(message));
	}

	#region 観察可能属性
	/// <summary>
	/// 配筋確認名
	/// </summary>
	[ObservableProperty]
	private string _danmTitleName = "";

	/// <summary>
	/// 部位名
	/// </summary>
	[ObservableProperty]
	private string _buimName = "";

	/// <summary>
	/// 階名
	/// </summary>
	[ObservableProperty]
	private string _floorName = "";

	/// <summary>
	/// 断面名
	/// </summary>
	[ObservableProperty]
	private string _danmName = "";

	/// <summary>
	/// 工区名
	/// </summary>
	[ObservableProperty]
	private string _kokuName = "";

	/// <summary>
	/// 位置
	/// </summary>
	[ObservableProperty]
	private string _locationName = "";

	/// <summary>
	/// 確認项目リストのデータソース
	/// </summary>
	[ObservableProperty]
	private ObservableCollection<InspectionItem> _inspectionItems;

	/// <summary>
	/// 確認项目リスト選択行
	/// </summary>
	[ObservableProperty]
	private InspectionItem? _inspectionItemSelectedItem;

	/// <summary>
	/// 工程
	/// </summary>
	[ObservableProperty]
	private ObservableCollection<ListItem> _projects = new();

	/// <summary>
	/// 工程の選択された行インデックス
	/// </summary>
	[ObservableProperty]
	private int _projectSelectedIndex = -1;

	/// <summary>
	/// 第1行断面ボタンコンテナ
	/// </summary>
	[ObservableProperty]
	private ObservableCollection<SelectionButtonModel> _row1Buttons = new();

	/// <summary>
	/// 第2行断面ボタンコンテナ
	/// </summary>
	[ObservableProperty]
	private ObservableCollection<SelectionButtonModel> _row2Buttons = new();

	/// <summary>
	/// 第3行断面ボタンコンテナ
	/// </summary>
	[ObservableProperty]
	private ObservableCollection<SelectionButtonModel> _row3Buttons = new();

	/// <summary>
	/// 判定基準
	/// </summary>
	[ObservableProperty]
	private string _criterion = "";

	/// <summary>
	/// 確認日
	/// </summary>
	[ObservableProperty]
	private string _confirmationDate = "";

	/// <summary>
	/// 確認日の有効状態
	/// </summary>
	[ObservableProperty]
	private bool _confirmationDateIsEnabled = true;

	/// <summary>
	/// 確認日の背景色
	/// </summary>
	[ObservableProperty]
	private Color _confirmationDateBackgroundColor = null;

	/// <summary>
	/// 確認者
	/// </summary>
	[ObservableProperty]
	private string _confirmer = "";

	/// <summary>
	/// 確認者の有効状態
	/// </summary>
	[ObservableProperty]
	private bool _confirmerIsEnabled = true;

	/// <summary>
	/// 確認者の背景色
	/// </summary>
	[ObservableProperty]
	private Color _confirmerBackgroundColor = null;

	/// <summary>
	/// 値
	/// </summary>
	[ObservableProperty]
	private string _ti = "";

	/// <summary>
	/// 値の有効状態
	/// </summary>
	[ObservableProperty]
	private bool _tiIsEnabled = true;

	/// <summary>
	/// 値の背景色
	/// </summary>
	[ObservableProperty]
	private Color _tiBackgroundColor = null;

	/// <summary>
	/// 指摘日
	/// </summary>
	[ObservableProperty]
	private string _indicationDate = "";

	/// <summary>
	/// 指摘日の有効状態
	/// </summary>
	[ObservableProperty]
	private bool _indicationDateIsEnabled = true;

	/// <summary>
	/// 指摘日の背景色
	/// </summary>
	[ObservableProperty]
	private Color _indicationDateBackgroundColor = null;

	/// <summary>
	/// 指摘者
	/// </summary>
	[ObservableProperty]
	private string _ndicationer = "";

	/// <summary>
	/// 指摘者の有効状態
	/// </summary>
	[ObservableProperty]
	private bool _ndicationerIsEnabled = true;

	/// <summary>
	/// 指摘者の背景色
	/// </summary>
	[ObservableProperty]
	private Color _ndicationerBackgroundColor = null;

	/// <summary>
	/// 是正方法
	/// </summary>
	[ObservableProperty]
	private string _correctionMethod = "";

	/// <summary>
	/// 是正方法の有効状態
	/// </summary>
	[ObservableProperty]
	private bool _correctionMethodIsEnabled = true;

	/// <summary>
	/// 是正方法の背景色
	/// </summary>
	[ObservableProperty]
	private Color _correctionMethodBackgroundColor = null;

	/// <summary>
	/// 指摘事項
	/// </summary>
	[ObservableProperty]
	private string _indication = "";

	/// <summary>
	/// 指摘事項の有効状態
	/// </summary>
	[ObservableProperty]
	private bool _indicationIsEnabled = true;

	/// <summary>
	/// 指摘事項の背景色
	/// </summary>
	[ObservableProperty]
	private Color _indicationBackgroundColor = null;

	/// <summary>
	/// 確認方法の選択インデックス
	/// </summary>
	[ObservableProperty]
	private int _confirmationMethodSelectedIndex = 0;

	/// <summary>
	/// 確認方法の有効状態
	/// </summary>
	[ObservableProperty]
	private bool _confirmationMethodIsEnabled = true;

	/// <summary>
	/// 確認方法の背景色
	/// </summary>
	[ObservableProperty]
	private Color _confirmationMethodBackgroundColor = null;

	/// <summary>
	/// 指摘事項ボタンの有効状態
	/// </summary>
	[ObservableProperty]
	private bool _openButtonIsEnabled = true;

	/// <summary>
	/// 指摘事項ボタンの背景色
	/// </summary>
	[ObservableProperty]
	private Color _openButtonBackgroundColor = null;

	/// <summary>
	/// 未確認画像の枠の厚さ
	/// </summary>
	[ObservableProperty]
	private int _checkrsl1StrokeThickness = 0;

	/// <summary>
	/// 不合格画像の枠の厚さ
	/// </summary>
	[ObservableProperty]
	private int _checkrsl2StrokeThickness = 0;

	/// <summary>
	/// 対象外画像の枠の厚さ
	/// </summary>
	[ObservableProperty]
	private int _checkrsl3StrokeThickness = 0;

	/// <summary>
	/// 合格画像の枠の厚さ
	/// </summary>
	[ObservableProperty]
	private int _checkrsl4StrokeThickness = 0;

	/// <summary>
	/// 是正済画像の枠の厚さ
	/// </summary>
	[ObservableProperty]
	private int _checkrsl5StrokeThickness = 0;

	/// <summary>
	/// 断面図
	/// </summary>
	[ObservableProperty]
	private ImageSource _containerImageSource;
	#endregion

	#region プライベートフィールド
	/// <summary>
	/// 検査結果画像の枠線の厚さ
	/// </summary>
	private const int _strokeThickness = 2;

	/// <summary>
	/// Shellナビゲーションパラメータ(工程リスト)
	/// </summary>
	private string _applyQuery_ProjectCode = "";

	/// <summary>
	/// 現在選択されている検査結果画像
	/// </summary>
	private (string selectedCheckrslImageName, int tag) _currentCheckResultImageSelection = ("", -1);

	/// <summary>
	/// 灰色
	/// </summary>
	private Color _grey = Color.FromRgba(139, 139, 139, 250);

	/// <summary>
	/// 空色
	/// </summary>
	private Color _skyBlue = Color.FromRgba(153, 201, 239, 250);

	/// <summary>
	/// ピンク色
	/// </summary>
	private Color _pink = Color.FromRgba(254, 178, 178, 250);

	/// <summary>
	/// 撮影された写真コレクション
	/// </summary>
	private List<string> _photoPathList = new();

	/// <summary>
	/// 前のページの名前
	/// </summary>
	private string _fromPage = "";

	/// <summary>
	/// コントロール非有効状態の背景色
	/// </summary>
	private Color _enableFalseBackgroundColor = Color.FromArgb("#CCCCCC");

	/// <summary>
	/// コントロール有効状態の背景色
	/// </summary>
	private Color _enableTrueBackgroundColor;
	#endregion

	/// <summary>
	/// 前の画面で渡されたパラメータの処理
	/// </summary>
	/// <param name="query"></param>
	public override void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.Keys.Contains("ProjectCode"))
		{
			_applyQuery_ProjectCode = query["ProjectCode"].ToString();
		}
		if (query.Keys.Contains("FromPage"))
		{
			_fromPage = query["FromPage"].ToString();
		}
	}

	#region 処理方法

	/// <summary>
	/// 撮影した写真を追加する
	/// </summary>
	/// <param name="message"></param>
	void TakePhotos(string message)
	{
		_photoPathList.Add(message);
	}

	/// <summary>
	/// 撮影した写真を削除する
	/// </summary>
	/// <param name="message"></param>
	void DeletePhotos(string message)
	{
		_photoPathList.Remove(message);
	}

	async Task InitializeRowLayoutAsync()
	{
		Row1Buttons.Clear();
		Row2Buttons.Clear();
		Row3Buttons.Clear();
		List<HM10DANM> list = await Service.GetRowLayOutItemsAsync();
		var row1Data = list.Take(4).ToList();
		var row2Data = list.Skip(4).Take(4).ToList();
		var row3Data = list.Skip(8).Take(4).ToList();
		RowButtonAddChild(Row1Buttons, row1Data, 0);
		RowButtonAddChild(Row2Buttons, row2Data, 4);
		RowButtonAddChild(Row3Buttons, row3Data, 8);
		if (list.Count > 0)
		{
			Row1Buttons[0].BackgroundColor = _pink;
			ContainerImageSource = await Service.GetImageSourceAsync(0);
			WeakReferenceMessenger.Default.Send("", "RefreshCheckPointPageImageToken");
		}
	}

	/// <summary>
	/// 断面のボタンの初期化
	/// </summary>
	void RowButtonAddChild(ObservableCollection<SelectionButtonModel> rowButtons, List<HM10DANM> data, int num)
	{
		for (int i = 0; i < data.Count; i++)
		{
			SelectionButtonModel item = new SelectionButtonModel();
			item.Id = i + num;
			item.Text = (i == 0 && num == 0) ? "断面" : data[i].HM10004.Trim();
			item.ImageCode = data[i].HM10006.Trim();
			item.BackgroundColor = string.IsNullOrWhiteSpace(data[i].HM10006.Trim())
				? _grey
				: _skyBlue;
			rowButtons.Add(item);
		}
	}

	/// <summary>
	/// 断面図ボタンの背景色の設定
	/// </summary>
	/// <param name="rowButtons"></param>
	void SetRowButtonBackgroundColor(int id)
	{
		foreach (var item in Row1Buttons)
		{
			item.BackgroundColor = string.IsNullOrWhiteSpace(item.ImageCode) ? _grey : _skyBlue;
		}
		foreach (var item in Row2Buttons)
		{
			item.BackgroundColor = string.IsNullOrWhiteSpace(item.ImageCode) ? _grey : _skyBlue;
		}
		foreach (var item in Row3Buttons)
		{
			item.BackgroundColor = string.IsNullOrWhiteSpace(item.ImageCode) ? _grey : _skyBlue;
		}
		if (id <= 3)
		{
			Row1Buttons.FirstOrDefault(x => x.Id == id).BackgroundColor = _pink;
		}
		else if (id >= 4 && id <= 7)
		{
			Row2Buttons.FirstOrDefault(x => x.Id == id).BackgroundColor = _pink;
		}
		else if (id >= 8 && id <= 11)
		{
			Row3Buttons.FirstOrDefault(x => x.Id == id).BackgroundColor = _pink;
		}
	}

	/// <summary>
	/// 画面初期化時に、工程の選択された行インデックスの設定
	/// </summary>
	void SetProjectSelectedIndexFromApplyQuery()
	{
		ProjectSelectedIndex = -1;
		if (string.IsNullOrWhiteSpace(_applyQuery_ProjectCode))
		{
			ProjectSelectedIndex = 0;
			return;
		}
		var item = Projects.FirstOrDefault(x => x.Value == _applyQuery_ProjectCode);
		ProjectSelectedIndex = item == null ? 0 : Projects.IndexOf(item);
	}

	/// <summary>
	/// バインドデータの初期化
	/// </summary>
	/// <returns></returns>
	async Task InitializeControlBindingDataAsync()
	{
		DanmTitleName = $"配筋確認：{Convert.ToInt32(Service.GetDanmTitle())}";
		//部位
		BuimName = await Service.GetBuimAsync();
		//階
		FloorName = await Service.GetFloorAsync();
		//断面
		DanmName = await Service.GetDanmAsync();
		//工区
		KokuName = await Service.GetKoKuAsync();
		//位置
		LocationName = Service.GetLocation();
		//工程
		Projects = await Service.GetProjectsAsync();
		SetProjectSelectedIndexFromApplyQuery();

		_enableTrueBackgroundColor = Preferences.Default.Get("Theme", "Light") == "Light" ? Colors.Transparent : Color.FromArgb("#212121");

		await InitializeRowLayoutAsync();
	}

	/// <summary>
	/// 確認項目属性値の設定
	/// </summary>
	void SetConfirmationProjectPropertiesValue(InspectionItem selectedRow)
	{
		if (selectedRow == null) return;
		//判定基準
		Criterion = selectedRow.HM13012.TrimEnd();
		//確認日
		ConfirmationDate = selectedRow.HR02006;
		//確認者
		Confirmer = selectedRow.HR02007.TrimEnd();
		//値
		Ti = selectedRow.HR02004.TrimEnd();
		//指摘日
		IndicationDate = selectedRow.HR02008;
		//指摘者
		Ndicationer = selectedRow.HR02009.TrimEnd();
		//是正方法
		CorrectionMethod = selectedRow.HR02011.TrimEnd();
		//指摘事項
		Indication = selectedRow.HR02010.TrimEnd();
		//確認方法
		ConfirmationMethodSelectedIndex = selectedRow.HR02019 != 0 ? selectedRow.HR02019 - 1 : 0;
	}

	/// <summary>
	/// 確認項目属性有効状態の設定
	/// </summary>
	void SetConfirmationProjectPropertiesEnable(int type, bool flag, InspectionItem selectedRow)
	{
		switch (type)
		{
			case 1:
				//確認日
				ConfirmationDateIsEnabled = false;
				ConfirmationDateBackgroundColor = _enableFalseBackgroundColor;

				//確認者
				ConfirmerIsEnabled = false;
				ConfirmerBackgroundColor = _enableFalseBackgroundColor;

				//指摘日
				IndicationDateIsEnabled = false;
				IndicationDateBackgroundColor = _enableFalseBackgroundColor;

				//指摘者
				NdicationerIsEnabled = false;
				NdicationerBackgroundColor = _enableFalseBackgroundColor;

				//処理方法
				CorrectionMethodIsEnabled = false;
				CorrectionMethodBackgroundColor = _enableFalseBackgroundColor;

				//メモ
				ConfirmationMethodIsEnabled = false;
				ConfirmationMethodBackgroundColor = _enableFalseBackgroundColor;

				//指摘事項
				IndicationIsEnabled = false;
				IndicationBackgroundColor = _enableFalseBackgroundColor;
				OpenButtonIsEnabled = false;
				OpenButtonBackgroundColor = _enableFalseBackgroundColor;

				//値
				TiIsEnabled = true;
				TiBackgroundColor = _enableTrueBackgroundColor;

				if (flag)
				{
					//確認日
					ConfirmationDate = "";
					//確認者
					Confirmer = "";
					selectedRow.HR02006 = "";
					selectedRow.HR02007 = "";

					//指摘日
					IndicationDate = "";
					//指摘者
					Ndicationer = "";
					selectedRow.HR02008 = "";
					selectedRow.HR02009 = "";

					//処理方法
					CorrectionMethod = "";
					selectedRow.HR02011 = "";
					//指摘事項
					Indication = "";
					selectedRow.HR02010 = "";
					//値
					Ti = "";
					selectedRow.HR02004 = "";
				}
				break;
			case 2:
				//確認日
				ConfirmationDateIsEnabled = false;
				ConfirmationDateBackgroundColor = _enableFalseBackgroundColor;

				//確認者
				ConfirmerIsEnabled = false;
				ConfirmerBackgroundColor = _enableFalseBackgroundColor;

				//指摘日
				IndicationDateIsEnabled = true;
				IndicationDateBackgroundColor = _enableTrueBackgroundColor;

				//指摘者
				NdicationerIsEnabled = true;
				NdicationerBackgroundColor = _enableTrueBackgroundColor;

				//処理方法
				CorrectionMethodIsEnabled = false;
				CorrectionMethodBackgroundColor = _enableFalseBackgroundColor;

				//処理方法の入力内容をクリアする。
				CorrectionMethod = "";

				//メモ
				ConfirmationMethodIsEnabled = true;
				ConfirmationMethodBackgroundColor = _enableTrueBackgroundColor;

				//指摘事項
				IndicationIsEnabled = true;
				IndicationBackgroundColor = _enableTrueBackgroundColor;

				OpenButtonIsEnabled = true;
				OpenButtonBackgroundColor = Color.FromArgb("#512BD4");

				//値
				TiIsEnabled = false;
				TiBackgroundColor = _enableFalseBackgroundColor;
				if (flag)
				{
					// 確認日
					ConfirmationDate = "";
					//確認者
					Confirmer = "";
					selectedRow.HR02006 = "";
					selectedRow.HR02007 = "";

					//指摘日
					IndicationDate = DateTime.Now.ToString("yyyy/MM/dd");
					//指摘者
					Ndicationer = Service.AppContext.Name;
					selectedRow.HR02008 = IndicationDate;
					selectedRow.HR02009 = Ndicationer;
				}
				break;
			case 3:
				//確認日
				ConfirmationDateIsEnabled = true;
				ConfirmationDateBackgroundColor = _enableTrueBackgroundColor;

				//確認者
				ConfirmerIsEnabled = true;
				ConfirmerBackgroundColor = _enableTrueBackgroundColor;

				//指摘日
				IndicationDateIsEnabled = false;
				IndicationDateBackgroundColor = _enableFalseBackgroundColor;

				//指摘者
				NdicationerIsEnabled = false;
				NdicationerBackgroundColor = _enableFalseBackgroundColor;

				//処理方法
				CorrectionMethodIsEnabled = false;
				CorrectionMethodBackgroundColor = _enableFalseBackgroundColor;

				//メモ
				ConfirmationMethodIsEnabled = false;
				ConfirmationMethodBackgroundColor = _enableFalseBackgroundColor;

				//指摘事項
				IndicationIsEnabled = false;
				IndicationBackgroundColor = _enableFalseBackgroundColor;

				OpenButtonIsEnabled = false;
				OpenButtonBackgroundColor = _enableFalseBackgroundColor;

				//値
				TiIsEnabled = false;
				TiBackgroundColor = _enableFalseBackgroundColor;
				if (flag)
				{
					// 確認日
					ConfirmationDate = DateTime.Now.ToString("yyyy/MM/dd");
					//確認者
					Confirmer = Service.AppContext.Name;
					selectedRow.HR02006 = ConfirmationDate;
					selectedRow.HR02007 = Confirmer;
					//指摘事項
					Indication = "";
					selectedRow.HR02010 = "";
				}
				break;
			case 4:
				//確認日
				ConfirmationDateIsEnabled = true;
				ConfirmationDateBackgroundColor = _enableTrueBackgroundColor;

				//確認者
				ConfirmerIsEnabled = true;
				ConfirmerBackgroundColor = _enableTrueBackgroundColor;

				//指摘日
				IndicationDateIsEnabled = false;
				IndicationDateBackgroundColor = _enableFalseBackgroundColor;

				//指摘者
				NdicationerIsEnabled = false;
				NdicationerBackgroundColor = _enableFalseBackgroundColor;

				//処理方法
				CorrectionMethodIsEnabled = false;
				CorrectionMethodBackgroundColor = _enableFalseBackgroundColor;

				//メモ
				ConfirmationMethodIsEnabled = false;
				ConfirmationMethodBackgroundColor = _enableFalseBackgroundColor;

				//指摘事項
				IndicationIsEnabled = false;
				IndicationBackgroundColor = _enableFalseBackgroundColor;

				OpenButtonIsEnabled = false;
				OpenButtonBackgroundColor = _enableFalseBackgroundColor;

				//値
				TiIsEnabled = true;
				TiBackgroundColor = _enableTrueBackgroundColor;
				if (flag)
				{
					// 確認日
					ConfirmationDate = DateTime.Now.ToString("yyyy/MM/dd");
					//確認者
					Confirmer = Service.AppContext.Name;
					selectedRow.HR02006 = ConfirmationDate;
					selectedRow.HR02007 = Confirmer;
				}
				break;
			case 5:
				//確認日
				ConfirmationDateIsEnabled = true;
				ConfirmationDateBackgroundColor = _enableTrueBackgroundColor;

				//確認者
				ConfirmerIsEnabled = true;
				ConfirmerBackgroundColor = _enableTrueBackgroundColor;

				//指摘日
				IndicationDateIsEnabled = false;
				IndicationDateBackgroundColor = _enableFalseBackgroundColor;

				//指摘者
				NdicationerIsEnabled = false;
				NdicationerBackgroundColor = _enableFalseBackgroundColor;

				//処理方法
				CorrectionMethodIsEnabled = true;
				CorrectionMethodBackgroundColor = _enableTrueBackgroundColor;

				//メモ
				ConfirmationMethodIsEnabled = false;
				ConfirmationMethodBackgroundColor = _enableFalseBackgroundColor;

				//指摘事項
				IndicationIsEnabled = false;
				IndicationBackgroundColor = _enableFalseBackgroundColor;

				OpenButtonIsEnabled = false;
				OpenButtonBackgroundColor = _enableFalseBackgroundColor;

				//値
				TiIsEnabled = true;
				TiBackgroundColor = _enableTrueBackgroundColor;
				if (flag)
				{
					// 確認日
					ConfirmationDate = DateTime.Now.ToString("yyyy/MM/dd");
					//確認者
					Confirmer = Service.AppContext.Name;
					selectedRow.HR02006 = ConfirmationDate;
					selectedRow.HR02007 = Confirmer;
				}
				break;
		}
	}

	/// <summary>
	/// 現在選択されている検査結果画像の枠線の厚さを設定する
	/// </summary>
	/// <param name="imageName">画像名</param>
	void SetCheckResultImageStrokeThickness(string imageName)
	{
		switch (imageName)
		{
			case "checkrsl1.png":
				Checkrsl2StrokeThickness = 0;
				Checkrsl3StrokeThickness = 0;
				Checkrsl4StrokeThickness = 0;
				Checkrsl5StrokeThickness = 0;
				Checkrsl1StrokeThickness = Checkrsl1StrokeThickness == 0 ? _strokeThickness : 0;
				_currentCheckResultImageSelection.selectedCheckrslImageName = Checkrsl1StrokeThickness == 0 ? "" : imageName;
				_currentCheckResultImageSelection.tag = Checkrsl1StrokeThickness == 0 ? -1 : 0;
				break;
			case "checkrsl2.png":
				Checkrsl1StrokeThickness = 0;
				Checkrsl3StrokeThickness = 0;
				Checkrsl4StrokeThickness = 0;
				Checkrsl5StrokeThickness = 0;
				Checkrsl2StrokeThickness = Checkrsl2StrokeThickness == 0 ? _strokeThickness : 0;
				_currentCheckResultImageSelection.selectedCheckrslImageName = Checkrsl2StrokeThickness == 0 ? "" : imageName;
				_currentCheckResultImageSelection.tag = Checkrsl2StrokeThickness == 0 ? -1 : 1;
				break;
			case "checkrsl3.png":
				Checkrsl1StrokeThickness = 0;
				Checkrsl2StrokeThickness = 0;
				Checkrsl4StrokeThickness = 0;
				Checkrsl5StrokeThickness = 0;
				Checkrsl3StrokeThickness = Checkrsl3StrokeThickness == 0 ? _strokeThickness : 0;
				_currentCheckResultImageSelection.selectedCheckrslImageName = Checkrsl3StrokeThickness == 0 ? "" : imageName;
				_currentCheckResultImageSelection.tag = Checkrsl3StrokeThickness == 0 ? -1 : 2;
				break;
			case "checkrsl4.png":
				Checkrsl1StrokeThickness = 0;
				Checkrsl2StrokeThickness = 0;
				Checkrsl3StrokeThickness = 0;
				Checkrsl5StrokeThickness = 0;
				Checkrsl4StrokeThickness = Checkrsl4StrokeThickness == 0 ? _strokeThickness : 0;
				_currentCheckResultImageSelection.selectedCheckrslImageName = Checkrsl4StrokeThickness == 0 ? "" : imageName;
				_currentCheckResultImageSelection.tag = Checkrsl4StrokeThickness == 0 ? -1 : 3;
				break;
			case "checkrsl5.png":
				Checkrsl1StrokeThickness = 0;
				Checkrsl2StrokeThickness = 0;
				Checkrsl3StrokeThickness = 0;
				Checkrsl4StrokeThickness = 0;
				Checkrsl5StrokeThickness = Checkrsl5StrokeThickness == 0 ? _strokeThickness : 0;
				_currentCheckResultImageSelection.selectedCheckrslImageName = Checkrsl5StrokeThickness == 0 ? "" : imageName;
				_currentCheckResultImageSelection.tag = Checkrsl5StrokeThickness == 0 ? -1 : 4;
				break;
		}
	}

	/// <summary>
	/// 確認項目属性値と有効状態の設定
	/// </summary>
	/// <param name="checkResultImage"></param>
	/// <param name="tag"></param>
	void SetConfirmationProjectPropertiesValueAndEnabledStatus(string checkResultImage, int tag)
	{
		int hr02005 = InspectionItemSelectedItem.HR02005;
		if (tag == 1 || tag == 4 || hr02005 != 1)
		{
			InspectionItemSelectedItem.HR02005 = tag;
			InspectionItemSelectedItem.CheckResultImage = checkResultImage;
			SetConfirmationProjectPropertiesValue(InspectionItemSelectedItem);
			SetConfirmationProjectPropertiesEnable(tag + 1, true, InspectionItemSelectedItem);
		}
	}

	/// <summary>
	/// 画像を破棄
	/// </summary>
	async Task<bool> DiscardImageAsync()
	{
		if (_photoPathList.Count == 0) return true;
		string ErrMsg = ErrorMessage.ERRORPOP("CM01031");
		var result = await DialogHelper.MessageDialogButton2(ErrMsg);
		if (result == NCDialogResult.No) return false;
		await Service.DiscardImageAsync(InspectionItemSelectedItem.HM13004, _photoPathList);
		return true;
	}

	/// <summary>
	/// 確認项目明細画面に入る前のデータチェック
	/// </summary>
	/// <returns></returns>
	bool ValidateBeforeNavigateToDetails()
	{
		if (InspectionItemSelectedItem == null) return false;
		if (InspectionItemSelectedItem.LocalPicNum == 0) return false;
		return true;
	}

	/// <summary>
	/// 確認项目明細画面のナビゲーションパラメータを作成する
	/// </summary>
	/// <returns></returns>
	Dictionary<string, object> CreateNavigationParameterForCheckPointDetail()
	{
		var obj = new
		{
			DanmTitle = DanmTitleName,
			Buim = BuimName,
			Grpl = FloorName,
			Danm = DanmName,
			Koku = KokuName,
			Location = LocationName,
			HR01001 = Service.AppContext.WorkCD,
			HR01003 = Service.GetDanmTitle(),
			Proc = Projects[ProjectSelectedIndex].Value,
			Proj = InspectionItemSelectedItem.HM13004
		};
		var options = new JsonSerializerOptions
		{
			WriteIndented = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};
		string jsonString = JsonSerializer.Serialize(obj, options);
		return new Dictionary<string, object>
		{
			{ "json", jsonString }
		};
	}

	/// <summary>
	/// iosとMacCatalystシステムのカメラ権限の検証
	/// </summary>
	async Task<bool> CheckCameraPermissionForIOSAndMacCatalystAsync()
	{
		var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

		if (status != PermissionStatus.Granted)
		{
			status = await Permissions.RequestAsync<Permissions.Camera>();
		}

		return status == PermissionStatus.Granted;
	}

	/// <summary>
	/// ウィンドウズシステムカメラ権限の検証
	/// </summary>
	async Task<bool> CheckCameraPermissionForWindowsAsync()
	{
		var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
		var micStatus = await Permissions.CheckStatusAsync<Permissions.Microphone>();
		if (cameraStatus != PermissionStatus.Granted)
		{
			cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
		}
		if (cameraStatus != PermissionStatus.Granted)
		{
			return false;
		}
		
		if (micStatus != PermissionStatus.Granted)
		{
			micStatus = await Permissions.RequestAsync<Permissions.Microphone>();
		}
		if (micStatus != PermissionStatus.Granted)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// カメラが使用可能かどうかを確認する
	/// </summary>
	/// <returns></returns>
	async Task<bool> CheckCameraSupportAsync()
	{
		string noCameraMsg = "現在のデバイスには使用可能なカメラがありません。";
		string cameraDeniedMsg = "カメラ権限がオフになっていることを検出しました。配筋検査システムがカメラにアクセスできるようにしてください。";
		//デバイスにカメラが存在するかどうかを判断する
		bool hasCamera;
#if WINDOWS
		var videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
		hasCamera = videoDevices != null && videoDevices.Count > 0;
#else
        hasCamera = MediaPicker.Default.IsCaptureSupported;
#endif
		if (!hasCamera)
		{
			DialogHelper.MessageDialogOk(noCameraMsg);
			return false;
		}

		//カメラの使用権限の判断
		bool isAllowed;
#if WINDOWS
		isAllowed = await CheckCameraPermissionForWindowsAsync();
#else
        isAllowed = await CheckCameraPermissionForIOSAndMacCatalystAsync();
#endif
		if (!isAllowed)
		{
			bool askSettings = await Shell.Current.DisplayAlertAsync(
				"カメラ権限が必要",
				cameraDeniedMsg,
				"設定",
				"キャンセル");
			if (askSettings)
			{
				AppInfo.Current.ShowSettingsUI();
			}
			return false;
		}
		return true;
	}

#endregion

	#region コマンドハンドラ 
	/// <summary>
	/// 画面ロード後処理
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task PageLoadedAsync()
	{
		if(_fromPage == "MapViewPage") await InitializeControlBindingDataAsync();
	}

	/// <summary>
	/// 写真撮影画面にジャンプ
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task GotoCameraPage()
	{
		if (!await CheckCameraSupportAsync()) return;
		if (InspectionItemSelectedItem == null) return;
		GreenBackgroundModel model = new GreenBackgroundModel()
		{
			//工区名
			WorkAreaName = KokuName,
			//部位名
			RegionalName = BuimName,
			//断面名
			SectionName = DanmName,
			//位置
			Position = LocationName,
			//工程名
			ProjectName = InspectionItemSelectedItem.CompositionName.Trim(),
			//確認項目名
			ConfirmProject = InspectionItemSelectedItem.ArtistName.Trim(),
			//説明のタイトル
			Describe = "確認者",
			//備考と確認者の切り替え展示
			DescribeDetail = Service.AppContext.Name,
			//確認者
			Confirmer = Service.AppContext.Name,
		};
		await Service.SetGreenBackgroundModelAsync(model);
		Service.SetHM13004ToCameraService(InspectionItemSelectedItem.HM13004);
		Service.SetInspectionItemToCameraService(InspectionItemSelectedItem);
		await Service.GotoCameraPageAsync();
	}

	/// <summary>
	/// 断面ボタンクリック処理
	/// </summary>
	/// <param name="parameter"></param>
	[RelayCommand]
	private async Task SectionButtonClick(object parameter)
	{
		ContainerImageSource = await Service.GetImageSourceAsync((int)parameter);
		SetRowButtonBackgroundColor((int)parameter);
		WeakReferenceMessenger.Default.Send("", "RefreshCheckPointPageImageToken");
	}

	/// <summary>
	/// 工程の選択切替
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	public async Task ProjectSelectedIndexChanged()
	{
		if (ProjectSelectedIndex < 0) return;
		InspectionItems = await Service.GetInspectionItemsByProjectCodeAsync(Projects[ProjectSelectedIndex].Value);
		if (InspectionItems.Count > 0)
		{
			InspectionItemSelectedItem = InspectionItems[0];
		}
	}

	/// <summary>
	/// 検査結果一覧選択行変更
	/// </summary>
	[RelayCommand]
	public void ProjectCodeSelectionChanged()
	{
		SetConfirmationProjectPropertiesValue(InspectionItemSelectedItem);
		SetConfirmationProjectPropertiesEnable(InspectionItemSelectedItem.HR02005 + 1, false, InspectionItemSelectedItem);
	}

	/// <summary>
	/// 検査結果の設定
	/// </summary>
	/// <param name="parameter"></param>
	[RelayCommand]
	private void ChangeCheckResultImage(string parameter)
	{
		var parts = parameter.Split(',');
		int tag = int.Parse(parts[1]);
		SetCheckResultImageStrokeThickness(parts[0]);
		if (InspectionItemSelectedItem != null)
		{
			SetConfirmationProjectPropertiesValueAndEnabledStatus(parts[0], tag);
		}
	}

	/// <summary>
	/// 確認项目リスト選択行の検査結果画像
	/// </summary>
	[RelayCommand]
	private void InspectionItemSelectedItemCheckResultImageTapped()
	{
		if (InspectionItemSelectedItem != null &&
			!string.IsNullOrWhiteSpace(_currentCheckResultImageSelection.selectedCheckrslImageName))
		{
			SetConfirmationProjectPropertiesValueAndEnabledStatus(
				_currentCheckResultImageSelection.selectedCheckrslImageName,
				_currentCheckResultImageSelection.tag);
		}
	}

	/// <summary>
	/// 確認項目属性値と有効状態の一括設定
	/// </summary>
	[RelayCommand]
	public void BatchSetConfirmationProjectPropertiesValueAndEnabledStatus()
	{
		if (_currentCheckResultImageSelection.tag == -1) return;
		foreach (var item in InspectionItems.Where(x => x.HR02005 == 0))
		{
			item.HR02005 = _currentCheckResultImageSelection.tag;
			item.CheckResultImage = _currentCheckResultImageSelection.selectedCheckrslImageName;

			if (item.Equals(InspectionItemSelectedItem))
			{
				SetConfirmationProjectPropertiesValue(item);
			}
			SetConfirmationProjectPropertiesEnable(item.HR02005 + 1, true, item);
		}
	}

	/// <summary>
	/// 指摘事項弾枠を開く
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task PopupPunchList()
	{
		var queryAttributes = new Dictionary<string, object>
		{
			["punchList"] = await Service.GetHM11003sAsync()
		};
		IPopupResult<string> popResult = await _popupService.ShowPopupAsync<PunchListPopupVM, string>(
			Shell.Current,
			options: new PopupOptions
			{
				PageOverlayColor = Colors.Transparent,
				Shape = new RoundRectangle
				{
					CornerRadius = new CornerRadius(20),
					Stroke = Colors.Transparent,
				}
			},
			shellParameters: queryAttributes
			);
		if (!string.IsNullOrWhiteSpace(popResult.Result))
		{
			Indication = string.IsNullOrWhiteSpace(Indication)
				? $"{popResult.Result}"
				: $"{Indication},{popResult.Result}";
		}
	}

	/// <summary>
	/// 値のテキスト変更処理
	/// </summary>
	[RelayCommand]
	private void TiTextChanged()
	{
		if (InspectionItemSelectedItem == null) return;
		InspectionItemSelectedItem.HR02004 = Ti.TrimEnd();
	}

	/// <summary>
	/// 是正方法のテキスト変更処理
	/// </summary>
	[RelayCommand]
	private void CorrectionMethodTextChanged()
	{
		if (InspectionItemSelectedItem == null) return;
		InspectionItemSelectedItem.HR02011 = CorrectionMethod.TrimEnd();
	}

	/// <summary>
	/// 指摘事項のテキスト変更処理
	/// </summary>
	[RelayCommand]
	private void IndicationTextChanged()
	{
		if (InspectionItemSelectedItem == null) return;
		InspectionItemSelectedItem.HR02010 = Indication.TrimEnd();
	}

	/// <summary>
	/// 確認方法の値選択変更処理
	/// </summary>
	[RelayCommand]
	private void CheckSelectedIndexChanged()
	{
		if (InspectionItemSelectedItem == null) return;
		InspectionItemSelectedItem.HR02019 = ConfirmationMethodSelectedIndex + 1;
	}

	/// <summary>
	/// 後方へ移動
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	public async Task BackAsync()
	{
		if (!await DiscardImageAsync()) return;
		Service.InitializeInspectionItem();
		await Shell.Current.GoToAsync("..?FromPage=CheckPointPage");
	}

	/// <summary>
	/// 保存クリック処理
	/// </summary>
	[RelayCommand]
	private async Task SaveData()
	{
		if (!await Service.SaveAsync())
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM01004");
			DialogHelper.MessageDialogOk(ErrMsg);
		}
		else
		{
			int index = InspectionItems.IndexOf(InspectionItemSelectedItem);
			Service.InitializeInspectionItem();
			InspectionItems = await Service.GetInspectionItemsByProjectCodeAsync(Projects[ProjectSelectedIndex].Value);
			if (InspectionItems.Count > 0)
			{
				InspectionItemSelectedItem = InspectionItems[index];
			}
			_photoPathList.Clear();
		}
	}

	/// <summary>
	/// 完了処理
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task Complete()
	{
		if (!await Service.SaveAsync())
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM01004");
			DialogHelper.MessageDialogOk(ErrMsg);
		}
		_photoPathList.Clear();
		Service.InitializeInspectionItem();
		await Shell.Current.GoToAsync($"..?FromPage=CheckPointPage");
	}

	/// <summary>
	/// 確認项目明細画面にジャンプ
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task NavigateToDetails()
	{
		try
		{
			if (!ValidateBeforeNavigateToDetails()) return;
			await Shell.Current.GoToAsync($"CheckPointDetailPage", CreateNavigationParameterForCheckPointDetail());
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
	}
	#endregion

}

