using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Aiko.Services.Services;

public class DownloadService : BaseService<DownloadService>, IDownloadService
{
	readonly IAppInitializationService _appInitializationService;
	readonly DataSyncService _dataSyncService;
	IProgress <DownloadProgressArgs> _progressHandler;
	public DownloadService(
				IAppInitializationService appInitializationService,
				ServiceContext<DownloadService> context,
				DataSyncService dataSyncService)
		: base(context)
	{
		_appInitializationService = appInitializationService;
		_dataSyncService = dataSyncService;
	}

	/// <summary>
	/// aikoApp実行中のアプリケーションコンテキスト
	/// </summary>
	public AikoAppContext AppContext => AikoAppContext;

	/// <summary>
	/// App通信サービスの初期化
	/// </summary>
	/// <returns></returns>
	public async Task InitializeCommunicationServiceAsync() 
	{
		await _appInitializationService.InitializeAppCommunicationServiceFromSqliteAsync();
	}

	/// <summary>
	/// 現場の初期化
	/// </summary>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public async Task<ObservableCollection<ListItem>> InitializeConstructionListAsync()
	{
		List<HM03PROJ> hm03List = await HkksDb.GetHM03PROJListAsync();
		List<HM15OSUB> hm15List = await HkksDb.GetHM15ListAsync(AppContext.OperatorCD, AppContext.CompanyID);
		ObservableCollection<ListItem> list = new ObservableCollection<ListItem>();
		ListItem listItem = new ListItem(" ", " ");
		list.Add(listItem);
		for (int iHm15 = 0; iHm15 < hm15List.Count; iHm15++)
		{
			for (int iHm03 = 0; iHm03 < hm03List.Count; iHm03++)
			{
				if (hm15List[iHm15].HM15002 == hm03List[iHm03].HM03001)
				{
					// アイテムの追加
					listItem = new ListItem(hm03List[iHm03].HM03001 + "-" + hm03List[iHm03].HM03002, hm03List[iHm03].HM03001);
					list.Add(listItem);
				}
			}
		}
		return list;
	}

	/// <summary>
	/// ファイルをダウンロードする
	/// </summary>
	/// <param name="workCode">現場コード</param>
	/// <param name="isIncludeDrawingFile">図面ファイルを含む</param>
	/// <param name="isIncludePhotoFile">写真ファイルを含む</param>
	/// <returns></returns>
	public async Task DownLoadAsync(string workCode, bool isIncludeDrawingFile, bool isIncludePhotoFile, IProgress<DownloadProgressArgs> progress)
	{
		_progressHandler = progress;
		_dataSyncService.InitDataAndFileStatus();
		string systemTime = _dataSyncService.GetSystemDateTime();
		string saveDays = $"-{Preferences.Default.Get("SaveDays", "14")}";
		int passedDay = Convert.ToInt32(Convert.ToDateTime(systemTime).AddDays(int.Parse(saveDays)).ToString("yyyyMMdd"));
		int currentStep = 0;
		int totalSteps = 27;

		//HM04MAPM
		await ReportProgressAsync("マップ", currentStep++, totalSteps);
		var hm04List = await _dataSyncService.GetHM04MAPMAsync(workCode, isIncludeDrawingFile);

		//ペイントファイル画像のダウンロード
		await ReportProgressAsync("ペイントファイル画像のダウンロード", currentStep++, totalSteps);
		await _dataSyncService.DownloadPaintFilesAsync(workCode, hm04List, isIncludeDrawingFile, currentStep, totalSteps, ReportProgressAsync);

		//HM05KAIM
		await ReportProgressAsync("階", currentStep++, totalSteps);
		await _dataSyncService.GetHM05Async(workCode);

		//HM06BUIM
		await ReportProgressAsync("部位", currentStep++, totalSteps);
		await _dataSyncService.GetHM06Async(workCode);

		//HM07KOKU
		await ReportProgressAsync("工区", currentStep++, totalSteps);
		await _dataSyncService.GetHM07Async(workCode);

		//HM08GRPM
		await ReportProgressAsync("グループ", currentStep++, totalSteps);
		await _dataSyncService.GetHM08Async(workCode);

		//HM09PROC
		await ReportProgressAsync("工程", currentStep++, totalSteps);
		await _dataSyncService.GetHM09Async(workCode);

		await ReportProgressAsync("ファイル", currentStep++, totalSteps);
		var hm12Tuple = await _dataSyncService.GetHM12FILEAsync(workCode, isIncludeDrawingFile);
		var hm12List = hm12Tuple.hm12List;
		var hm12FileList = hm12Tuple.hm12ImageList;

		//HM12FILE
		await ReportProgressAsync("ファイルを更新", currentStep++, totalSteps);
		var hm12Result = await _dataSyncService.SetHM12UPDATEAsync(hm12List);

		//図面ファイル画像のダウンロード
		await ReportProgressAsync("図面ファイル画像のダウンロード", currentStep++, totalSteps);
		await _dataSyncService.DownloadFilesAsync(workCode, hm12FileList, isIncludeDrawingFile, currentStep, totalSteps, ReportProgressAsync);
		if (hm12Result)
		{
			List<string> tableNameList = new List<string>();
			tableNameList.Add("HM12FILE");
			await DownLoadTimeUtils.SaveTimesAsync(workCode, tableNameList, systemTime);
		}

		//HM10DANM
		await ReportProgressAsync("断面", currentStep++, totalSteps);
		var hm10Result = await _dataSyncService.GetHM10DANMListAsync(workCode);

		//断面画像のダウンロード
		await ReportProgressAsync("断面画像のダウンロード", currentStep++, totalSteps);
		await _dataSyncService.DownloadDamnFilesAsync(workCode, hm10Result.hm10List, currentStep, totalSteps, ReportProgressAsync);
		if (hm10Result.result)
		{
			List<string> tableNameList = new List<string>();
			tableNameList.Add("HM10DANM");
			await DownLoadTimeUtils.SaveTimesAsync(workCode, tableNameList, systemTime);
		}

		//HM13KNKM
		await ReportProgressAsync("確認項目", currentStep++, totalSteps);
		await _dataSyncService.GetHM13Async(workCode);

		//HM14GUID
		await ReportProgressAsync("マップガイド", currentStep++, totalSteps);
		await _dataSyncService.GetHM14Async(workCode);

		//hm16shdir
		await ReportProgressAsync("撮影方向", currentStep++, totalSteps);
		await _dataSyncService.GetHM16Async(workCode);

		//HM19GUIDCOLOR
		await ReportProgressAsync("マップガイド色マスター", currentStep++, totalSteps);
		await _dataSyncService.GetHM19Async(workCode);

		//HM20GUIDHEAD
		await ReportProgressAsync("マップガイドヘッダマスター", currentStep++, totalSteps);
		await _dataSyncService.GetHM20Async(workCode);

		//HM23BUNRUI
		await ReportProgressAsync("分類マスター", currentStep++, totalSteps);
		await _dataSyncService.GetHM23Async(workCode);

		//HR06SYASDEL
		await ReportProgressAsync("写真削除", currentStep++, totalSteps);
		var hr06List = await _dataSyncService.GetHR06Async(workCode);

		//HR01ITEM
		await ReportProgressAsync("アイテム", currentStep++, totalSteps);
		var hr01Result = await _dataSyncService.GetHR01Async(workCode, currentStep, totalSteps, ReportProgressAsync);
		if (hr01Result)
		{
			List<string> tableNameList = new List<string>();
			tableNameList.Add("HR01ITEM");
			await DownLoadTimeUtils.SaveTimesAsync(workCode, tableNameList, systemTime);
		}

		//HR02KSKK
		await ReportProgressAsync("検査結果", currentStep++, totalSteps);
		var hr02Result = await _dataSyncService.GetHR02Async(workCode, currentStep, totalSteps, ReportProgressAsync);
		if (hr02Result)
		{
			List<string> tableNameList = new List<string>();
			tableNameList.Add("HR02KSKK");
			await DownLoadTimeUtils.SaveTimesAsync(workCode, tableNameList, systemTime);
		}

		await ReportProgressAsync("写真", currentStep++, totalSteps);
		var hr03Tuple = await _dataSyncService.GetHR03SYASAsync(workCode, isIncludePhotoFile, passedDay,currentStep,totalSteps,ReportProgressAsync);

		// 写真ファイルをアップロード
		await ReportProgressAsync("写真を更新", currentStep++, totalSteps);
		await _dataSyncService.UploadPhotoFilesAsync(workCode, hr03Tuple.uploadHR03ImageList, currentStep, totalSteps, ReportProgressAsync);
		var hr03Result = await _dataSyncService.SetHR03UPDATEAsync(hr03Tuple.serverHR03List, hr03Tuple.localHR03List, workCode);
		List<HR03SYAS> hr03List = new List<HR03SYAS>();
		foreach (HR03SYAS item in hr03Tuple.downloadHR03ImageList)
		{
			if (item.HR03009 >= passedDay)
			{
				hr03List.Add(item);
			}
		}

		//写真画像のダウンロード
		await ReportProgressAsync("写真画像のダウンロード", currentStep++, totalSteps);
		await _dataSyncService.DownloadPhotoFilesAsync(workCode, hr03List, isIncludePhotoFile, currentStep, totalSteps, ReportProgressAsync);
		if (hr03Result)
		{
			List<string> tableNameList = new List<string>();
			tableNameList.Add("HR03SYAS");
			await DownLoadTimeUtils.SaveTimesAsync(workCode, tableNameList, systemTime);
		}

		//HR04KSHIS
		await ReportProgressAsync("検査履歴", currentStep++, totalSteps);
		await _dataSyncService.GetHR04Async(workCode);

		//HR05KOKUMINFO
		await ReportProgressAsync("工区多辺形情報", currentStep++, totalSteps);
		await _dataSyncService.GetHR05Async(workCode);

		//無効な画像の削除
		await ReportProgressAsync("無効な画像の削除", currentStep++, totalSteps);
		await _dataSyncService.DeletePhotoFilesAsync(workCode, hr06List, currentStep, totalSteps, ReportProgressAsync);

		await _dataSyncService.UpdateHM17Async(0, workCode);
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
