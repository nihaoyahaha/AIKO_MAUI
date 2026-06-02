using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.Services.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FFImageLoading;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class DownloadPageVM : Observablebase<DownloadPageVM, IDownloadService>
{
	readonly DataSyncService _dataSyncService;
	public DownloadPageVM(ILogger<DownloadPageVM> logger,
		IDownloadService service,
		DataSyncService dataSyncService) : base(logger, service)
	{
		_dataSyncService = dataSyncService;
	}

	[ObservableProperty]
	public partial bool IsEnableFlag { get; set; } = true;

	[ObservableProperty]
	public partial bool DownLoadButtonIsEnabled { get; set; } = true;

	/// <summary>
	/// 进度值 (0.0 - 1.0)
	/// </summary>
	[ObservableProperty]
	public partial double Progress { get; set; }

	[ObservableProperty]
	public partial string ProgressMessage { get; set; }

	[ObservableProperty]
	public partial string PercentText { get; set; } = "0%";

	[ObservableProperty]
	public partial bool IsShowProgressGridFlag { get; set; } = false;

	[ObservableProperty]
	public partial ObservableCollection<ListItem> Constructions { get; set; } = new();

	[ObservableProperty]
	public partial ListItem SelectedConstruction { get; set; }

	[ObservableProperty]
	public partial bool DownLoadFrameIsVisible { get; set; } = false;

	/// <summary>
	/// 図面ファイルを含む
	/// </summary>
	[ObservableProperty]
	public partial bool IsIncludeDrawingFile { get; set; } = false;

	/// <summary>
	/// 写真ファイルを含む
	/// </summary>
	[ObservableProperty]
	public partial bool IsIncludePhotoFile { get; set; } = false;

	/// <summary>
	/// 指定した時間内の写真をダウンロードするためのヒント
	/// </summary>
	[ObservableProperty]
	public partial string SaveDaysMessageInfo { get; set; } = "";

	// 总步骤数
	private const int TotalSteps = 8;

	[RelayCommand]
	private async Task PageLoaded()
	{
		string saveDays = Preferences.Default.Get("SaveDays", "14");
		SaveDaysMessageInfo = $"　　※撮影から{saveDays}日以内の写真データをダウンロードします。";
		DownLoadFrameIsVisible = Service.AppContext.IsLogin;
		await InitializeConstructionListAsync();
	}

	[RelayCommand]
	public async Task btnMDownload()
	{
		bool downLoadExp = false;
		bool ioExp = false;

		// UWPにおけるディスク容量不足の判断
		if (!await _dataSyncService.CheckDriveSpaceAsync())
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM00105");
			DialogHelper.MessageDialogOk(ErrMsg);
			return;
		}

		// サーバーへの接続に失敗しました、ローカルネットワーク接続を確認してください。 
		if (!_dataSyncService.CheckNetworkConnection())
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM99003");
			DialogHelper.MessageDialogOk(ErrMsg);
			return;
		}

		try
		{
			// ダウンロード時の制御ページの操作不可
			IsEnableFlag = false;
			DownLoadButtonIsEnabled = false;
			IsShowProgressGridFlag = true;
			_dataSyncService.InitDataAndFileStatus();
			// 初期化
			await UpdateProgressAsync(0, "ダウンロード準備中...");

			// HC01
			await UpdateProgressAsync(1, "HC01データをダウンロード中...");
			await _dataSyncService.GetHC01FlagAsync();

			// HM01
			await UpdateProgressAsync(2, "HM01データをダウンロード中...");
			await _dataSyncService.GetHM01FlagAsync();

			// HM02
			await UpdateProgressAsync(3, "HM02データをダウンロード中...");
			await _dataSyncService.GetHM02FlagAsync();

			// HM03
			await UpdateProgressAsync(4, "HM03データをダウンロード中...");
			await _dataSyncService.GetHM03FlagAsync();

			// HM11
			await UpdateProgressAsync(5, "HM11データをダウンロード中...");
			await _dataSyncService.GetHM11FlagAsync();

			// HM15
			await UpdateProgressAsync(6, "HM15データをダウンロード中...");
			await _dataSyncService.GetHM15FlagAsync();

			// HM17
			await UpdateProgressAsync(7, "HM17データをダウンロード中...");
			await _dataSyncService.GetHM17FlagAsync();

			// HM22
			await UpdateProgressAsync(8, "HM22データをダウンロード中...");
			await _dataSyncService.GetHM22FlagAsync();

			//App通信サービスの初期化
			await Service.InitializeCommunicationServiceAsync();
		}
		catch (Exception ex)
		{
			downLoadExp = true;
			// ディスク容量不足判定
			if (ex.Message.Contains("Full") ||
				ex.Message.Contains("ディスクに十分な空き領域がありません"))
			{
				ioExp = true;
			}

			Logger.LogError(ex.ToString(), "DownloadPageVM");
		}
		finally
		{
			InitializateProgress();

			// ボタン有効化
			IsEnableFlag = true;
			DownLoadButtonIsEnabled = true;

			// 結果メッセージ
			string errMsg;
			if (ioExp)
			{
				errMsg = ErrorMessage.ERRORPOP("CM00105");
			}
			else if (downLoadExp)
			{
				errMsg = ErrorMessage.ERRORPOP("CM00001");
			}
			else
			{
				errMsg = ErrorMessage.ERRORPOP("CM01030");
			}

			DialogHelper.MessageDialogOk(errMsg);
		}
	}

	/// <summary>
	/// 現場選択変更処理
	/// </summary>
	[RelayCommand]
	private void ConstructionSelectedIndexChanged()
	{
		DownLoadButtonIsEnabled = string.IsNullOrWhiteSpace(SelectedConstruction.Value) ? false : true;
	}

	/// <summary>
	/// ダウンロード処理
	/// </summary>
	[RelayCommand]
	private async Task DownLoad()
	{   // ディスク容量不足
		bool ioExp = false;
		// 実行結果
		bool blResult = true;
		if (!await ValidateBeforeDownloadAsync()) return;
		try
		{
			// プログレスプロセッサの作
			// Progress<T>のコンストラクタは、現在のUIスレッドコンテキストを自動的にキャプチャします
			IProgress<DownloadProgressArgs> progressHandler = new Progress<DownloadProgressArgs>(args =>
			{
				ProgressMessage = args.Message;
				Progress = args.Progress;
				PercentText = args.PercentText;
			});
			IsShowProgressGridFlag = true;
			await _dataSyncService.UpdateHM17Async(1, SelectedConstruction.Value);
			await Service.DownLoadAsync(SelectedConstruction.Value, IsIncludeDrawingFile, IsIncludePhotoFile, progressHandler);
		}
		catch (Exception exp)
		{
			// 実行結果 ダウンロードに失敗しました
			blResult = false;
			// ディスク容量不足
			if (exp.Message.Equals("Full") || exp.Message.Contains("ディスクに十分な空き領域がありません"))
			{
				ioExp = true;
			}
			Logger.LogError(exp.ToString());
		}
		finally
		{
			InitializateProgress();

			await _dataSyncService.UpdateHM17Async(0, SelectedConstruction.Value);
			// 結果メッセージ
			string errMsg;
			if (ioExp)
			{
				errMsg = ErrorMessage.ERRORPOP("CM00105");
			}
			//テーブルデータの同期に失敗しました
			else if (!_dataSyncService.DataStatus)
			{
				errMsg = ErrorMessage.ERRORPOP("CM01034");
			}
			else if (!_dataSyncService.FileStatus)
			{
				string msg = ErrorMessage.ERRORPOP("CM01038");
				errMsg = string.Format(msg, _dataSyncService.FileSyncErrorMessage);
			}
			// ダウンロードに失敗しました
			else if (!blResult)
			{
				errMsg = ErrorMessage.ERRORPOP("CM00001");
			}
			// ダウンロードに成功しました
			else
			{
				errMsg = ErrorMessage.ERRORPOP("CM01030");
			}
			DialogHelper.MessageDialogOk(errMsg);
		}
	}

	/// <summary>
	/// 更新进度
	/// </summary>
	async Task UpdateProgressAsync(int currentStep, string message)
	{
		// MAUI ProgressBar 范围是 0.0 - 1.0
		Progress = (double)currentStep / TotalSteps;
		ProgressMessage = message;
		PercentText = $"{(int)(Progress * 100)}%";

		// 给 UI 一点时间刷新
		await Task.Delay(50);
	}

	/// <summary>
	/// 現場選択の初期化
	/// </summary>
	/// <returns></returns>
	async Task InitializeConstructionListAsync()
	{
		Constructions = await Service.InitializeConstructionListAsync();
		SelectedConstruction = Constructions.FirstOrDefault(x => x.Value == Service.AppContext.WorkCD);
	}

	/// <summary>
	/// 初期化プログレスバー
	/// </summary>
	void InitializateProgress()
	{
		IsShowProgressGridFlag = false;
		Progress = 0;
		ProgressMessage = "";
		PercentText = "0%";
	}

	/// <summary>
	/// フィールドデータをダウンロードする前の検証
	/// </summary>
	async Task<bool> ValidateBeforeDownloadAsync()
	{
		//UWPにおけるディスク容量不足の判断
		if (!await _dataSyncService.CheckDriveSpaceAsync())
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM00105");
			DialogHelper.MessageDialogOk(ErrMsg);
			return false;
		}
		//サーバーへの接続に失敗しました、ローカルネットワーク接続を確認してください。 
		if (!_dataSyncService.CheckNetworkConnection())
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM99003");
			DialogHelper.MessageDialogOk(ErrMsg);
			return false;
		}
		//端末制限
		if (!await _dataSyncService.CheckUUID())
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM01136");
			DialogHelper.MessageDialogOk(ErrMsg);
			return false;
		}
		//IP制限
		if (!_dataSyncService.CheckIPAddress(SelectedConstruction.Value))
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM01127");
			DialogHelper.MessageDialogOk(ErrMsg);
			return false;
		}
		//工事が終わったと判断します
		if (_dataSyncService.CheckProjectFinish(SelectedConstruction.Value))
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM01032");
			DialogHelper.MessageDialogOk(ErrMsg);
			return false;
		}
		return true;
	}
}
