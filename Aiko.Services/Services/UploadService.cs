using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Services.Services;

public class UploadService : BaseService<UploadService>, IUploadService
{
	IProgress<DownloadProgressArgs> _progressHandler;
	readonly DataSyncService _dataSyncService;

	public UploadService(ServiceContext<UploadService> context, 
		DataSyncService dataSyncService) : base(context)
	{
		_dataSyncService = dataSyncService;
	}

	public AikoAppContext AppContext => AikoAppContext;

	/// <summary>
	/// 現在のシステム時間
	/// </summary>
	/// <returns></returns>
	public string GetSystemDateTime()
	{
		return _dataSyncService.GetSystemDateTime();
	}

	/// <summary>
	/// 検査結果、確認履歴を同期する
	/// </summary>
	public async Task UploadDataAsync(IProgress<DownloadProgressArgs> progress)
	{
		_progressHandler = progress;
		_dataSyncService.InitDataAndFileStatus();
		await _dataSyncService.SetHR02Async(AikoAppContext.WorkCD, ReportProgressAsync);
	}

	public async Task UploadFileAsync(IProgress<DownloadProgressArgs> progress, bool checkDrawing)
	{
		_progressHandler = progress;
		_dataSyncService.InitDataAndFileStatus();
		int currentStep = 0;
		int totalSteps = 3;
		List<HM12FILE> hm12ImageList = new();
		List<string> tableNameList = new List<string>();
		string systemTime = _dataSyncService.GetSystemDateTime();
		string saveDays = $"-{Preferences.Default.Get("SaveDays", "14")}";
		int passedDay = Convert.ToInt32(Convert.ToDateTime(systemTime).AddDays(int.Parse(saveDays)).ToString("yyyyMMdd"));
		if (checkDrawing)
		{
			var hm12Tuple = await _dataSyncService.GetHM12FILEAsync(AikoAppContext.WorkCD,true);
			hm12ImageList = hm12Tuple.hm12ImageList;
			await _dataSyncService.SetHM12UPDATEAsync(hm12Tuple.hm12List);
			totalSteps += hm12Tuple.hm12ImageList.Count;
		}
		var hr06List = await _dataSyncService.GetHR06Async(AikoAppContext.WorkCD);
		var hr03Tuple = await _dataSyncService.GetHR03SYASAsync(AikoAppContext.WorkCD,false, passedDay,currentStep,totalSteps,ReportProgressAsync);
		totalSteps += hr03Tuple.uploadHR03ImageList.Count;
		
		await ReportProgressAsync("", currentStep++, totalSteps);
		
		bool uploadResult = await _dataSyncService.UploadPhotoFilesAsync(AikoAppContext.WorkCD, hr03Tuple.uploadHR03ImageList, currentStep, totalSteps, ReportProgressAsync);
		if (uploadResult)
		{
			throw new InvalidOperationException("ファイルのアップロードに失敗しました.");
		}
		await _dataSyncService.SetHR03UPDATEAsync(hr03Tuple.serverHR03List, hr03Tuple.localHR03List,AikoAppContext.WorkCD);

		await ReportProgressAsync("", currentStep++, totalSteps);

		if (hm12ImageList.Count > 0)
		{
			await _dataSyncService.DownloadFilesAsync(AikoAppContext.WorkCD, hm12ImageList, checkDrawing, currentStep, totalSteps, ReportProgressAsync);
		}
		await _dataSyncService.DeletePhotoFilesAsync(AikoAppContext.WorkCD, hr06List, currentStep, totalSteps, ReportProgressAsync);

		await ReportProgressAsync("", currentStep++, totalSteps);

		if (_dataSyncService.HM12DataStatus && checkDrawing)
		{
			tableNameList.Add("HM12FILE");
		}
		if (_dataSyncService.HR03DataStatus)
		{
			tableNameList.Add("HR03SYAS");
		}
		await DownLoadTimeUtils.SaveTimesAsync(AikoAppContext.WorkCD,tableNameList, systemTime);

		await _dataSyncService.DeleteLocalExpiredImagesAsync(passedDay);
	}

	/// <summary>
	/// 非同期でダウンロード/同期の進捗を報告し、UI表示を更新する
	/// </summary>
	/// <param name="progressHandler">進捗通知インターフェース</param>
	/// <param name="message">進捗メッセージ</param>
	/// <param name="currentStep">現在の完了ステップ数</param>
	/// <param name="TotalSteps">総ステップ数</param>
	/// <returns></returns>
	async Task ReportProgressAsync(string message, int currentStep, int TotalSteps)
	{
		try
		{
			if (_progressHandler == null) return;
			var progress = TotalSteps > 0 ? (double)currentStep / TotalSteps : 0;
			_progressHandler?.Report(new DownloadProgressArgs
			{
				Message = message,
				Progress = progress,
				PercentText = $"{(int)(progress * 100)}%"
			});
			await Task.Delay(50);
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
	}
}
