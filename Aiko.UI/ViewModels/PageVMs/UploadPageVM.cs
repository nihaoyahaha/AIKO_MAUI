using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.Services.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class UploadPageVM : Observablebase<UploadPageVM, IUploadService>
{
	readonly DataSyncService _dataSyncService;
	readonly DownLoadTimeUtils _downLoadTimeUtils;
	public UploadPageVM(ILogger<UploadPageVM> logger,
		IUploadService service,
		DataSyncService dataSyncService,
		DownLoadTimeUtils downLoadTimeUtils) : base(logger, service)
	{
		_dataSyncService = dataSyncService;
		_downLoadTimeUtils = downLoadTimeUtils;
	}

	/// <summary>
	/// 进度值 (0.0 - 1.0)
	/// </summary>
	[ObservableProperty]
	private double progress;

	[ObservableProperty]
	private string progressMessage;

	[ObservableProperty]
	private string percentText = "0%";

	[ObservableProperty]
	private bool isShowProgressGridFlag = false;

	/// <summary>
	/// 指定した時間内の写真をダウンロードするためのヒント
	/// </summary>
	[ObservableProperty]
	private string _saveDaysMessageInfo = "";

	/// <summary>
	/// 図面ファイルを含む
	/// </summary>
	[ObservableProperty]
	private bool _isCheckDrawing = false;

	[RelayCommand]
	private async Task PageLoaded()
	{
		string saveDays = Preferences.Default.Get("SaveDays", "14");
		SaveDaysMessageInfo = $"　　※撮影から{saveDays}日以内の写真データを保持します。";
	}

	/// <summary>
	/// 検査結果、確認履歴を同期
	/// </summary>
	[RelayCommand]
	private async Task DataSynchronization()
	{
		// ディスク容量不足
		bool ioExp = false;
		// 実行結果
		bool blResult = true;
		string downLoadTime = "";
		if (!await ValidateBeforeDownloadAsync()) return;
		try
		{
			IProgress<DownloadProgressArgs> progressHandler = new Progress<DownloadProgressArgs>(args =>
			{
				ProgressMessage = args.Message;
				Progress = args.Progress;
				PercentText = args.PercentText;
			});
			IsShowProgressGridFlag = true;
			downLoadTime = Service.GetSystemDateTime();
			await Service.UploadDataAsync(progressHandler);
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
			//ダウンロードもしくは同期処理完了後に利用バージョン(hm17version)のログイン中(hm17017)に0を登録する
			await _dataSyncService.UpdateHM17Async(0, Service.AppContext.WorkCD);
			// 結果メッセージ
			string errMsg;
			if (ioExp)
			{
				errMsg = ErrorMessage.ERRORPOP("CM00105");
			}
			// ダウンロードに失敗しました
			else if (!blResult)
			{
				errMsg = ErrorMessage.ERRORPOP("CM00001");
			}
			else
			{
				List<string> tableNameList = new List<string>();
				tableNameList.Add("HR02KSKK");
				await _downLoadTimeUtils.SaveTimesAsync(Service.AppContext.WorkCD, tableNameList, downLoadTime);

				errMsg = ErrorMessage.ERRORPOP("CM01030");
			}
			DialogHelper.MessageDialogOk(errMsg);
		}
	}

	/// <summary>
	/// 写真同期処理
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task FileSynchronization() 
	{
		// ディスク容量不足
		bool ioExp = false;
		// 実行結果
		bool blResult = true;
		if (!await ValidateBeforeDownloadAsync()) return;
		try
		{
			IProgress<DownloadProgressArgs> progressHandler = new Progress<DownloadProgressArgs>(args =>
			{
				ProgressMessage = args.Message;
				Progress = args.Progress;
				PercentText = args.PercentText;
			});
			IsShowProgressGridFlag = true;
			await Service.UploadFileAsync(progressHandler,IsCheckDrawing);
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
			// 結果メッセージ
			string errMsg;
			if (ioExp)
			{
				errMsg = ErrorMessage.ERRORPOP("CM00105");
			}
			//テーブルデータの同期に失敗しました
			else if (!_dataSyncService.HM12DataStatus || !_dataSyncService.HR03DataStatus)
			{
				errMsg = ErrorMessage.ERRORPOP("CM01034");
			}
			else if (!_dataSyncService.HM12FileStatus)
			{
				string msg = ErrorMessage.ERRORPOP("CM01038");
				errMsg = string.Format(msg, "図面ファイル");
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
		////端末制限
		//if (!_dataSyncService.CheckUUID())
		//{
		//	string ErrMsg = ErrorMessage.ERRORPOP("CM01136");
		//	DialogHelper.MessageDialogOk(ErrMsg);
		//	return false;
		//}
		////IP制限
		//if (!_dataSyncService.CheckIPAddress(Service.AppContext.WorkCD))
		//{
		//	string ErrMsg = ErrorMessage.ERRORPOP("CM01127");
		//	DialogHelper.MessageDialogOk(ErrMsg);
		//	return false;
		//}
		////工事が終わったと判断します
		//if (_dataSyncService.CheckProjectFinish(Service.AppContext.WorkCD))
		//{
		//	string ErrMsg = ErrorMessage.ERRORPOP("CM01032");
		//	DialogHelper.MessageDialogOk(ErrMsg);
		//	return false;
		//}
		return true;
	}
}
