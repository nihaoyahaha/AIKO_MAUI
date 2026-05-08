using Aiko.Common;
using Aiko.SqliteDb;
using DI.DiNetWinServiceObject;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;
#if WINDOWS
using Windows.Security.Cryptography;
using Windows.System.Profile;
using System.Management;
#endif

#if IOS || MACCATALYST
using UIKit;
#endif

namespace Aiko.Services.Services;

/// <summary>
/// ローカルsqliteデータベースとリモート・データベースのデータとファイルの同期に使用されるクラス
/// </summary>
public class DataSyncService
{
	#region 依存注入パラメータ初期化
	readonly HkksDatabase _hkksDb;
	readonly AikoAppContext _appContext;
	readonly ILogger<DataSyncService> _logger;
	readonly DownLoadTimeUtils _downLoadTimeUtils;
	readonly AikoWcf _aikoWcf;
	readonly FluentFtpClient _fluentFtpClient;
	readonly WebDavClient _webDavClient;

	public DataSyncService(
	HkksDatabase hkksDatabase,
	AikoAppContext appContext,
	ILogger<DataSyncService> logger,
	DownLoadTimeUtils downLoadTimeUtils,
	AikoWcf aikoWcf,
	FluentFtpClient ftpClient,
	WebDavClient webDavClient)
	{
		_hkksDb = hkksDatabase;
		_appContext = appContext;
		_logger = logger;
		_downLoadTimeUtils = downLoadTimeUtils;
		_aikoWcf = aikoWcf;
		_fluentFtpClient = ftpClient;
		_webDavClient = webDavClient;
	}
	#endregion

	#region プライベートフィールド

	int _timeout = 120000;
	decimal _pageSize = 10000;
	IUWPServiceAPI _serviceApi02;
	string[] _mimeTypes = { ".jpg", ".svg", ".json" };
	#endregion

	#region 属性

	/// <summary>
	/// 断面データ同期状態
	/// </summary>
	private bool _hm10DataStatus = true;
	public bool HM10DataStatus
	{
		get => _hm10DataStatus;
	}

	/// <summary>
	/// ファイルデータ同期状態
	/// </summary>
	private bool _hm12DataStatus = true;
	public bool HM12DataStatus
	{
		get => _hm12DataStatus;
	}

	/// <summary>
	/// アイテムデータ同期状態
	/// </summary>
	private bool _hr01DataStatus = true;
	public bool HR01DataStatus
	{
		get => _hr01DataStatus;
	}

	/// <summary>
	/// 検査結果データ同期状態
	/// </summary>
	private bool _hr02DataStatus = true;
	public bool HR02DataStatus
	{
		get => _hr02DataStatus;
	}

	/// <summary>
	/// 同期・写真テーブルデータ同期状態
	/// </summary>
	private bool _hr03DataStatus = true;
	public bool HR03DataStatus
	{
		get => _hr03DataStatus;
	}

	/// <summary>
	/// 断面ファイル同期状態
	/// </summary>
	private bool _hm10FileStatus = true;
	public bool HM10FileStatus
	{
		get => _hm10FileStatus;
		set => _hm10FileStatus = value;
	}

	/// <summary>
	/// ファイル同期状態
	/// </summary>
	private bool _hm12FileStatus = true;
	public bool HM12FileStatus
	{
		get => _hm12FileStatus;
		set => _hm12FileStatus = value;
	}

	/// <summary>
	/// 同期・写真テーブルファイル同期状態
	/// </summary>
	private bool _hr03FileStatus = true;
	public bool HR03FileStatus
	{
		get => _hr03FileStatus;
		set => _hr03FileStatus = value;
	}

	/// <summary>
	/// ペイントファイル画像同期状態
	/// </summary>
	private bool _hm04FileStatus = true;
	public bool HM04FileStatus
	{
		get => _hm04FileStatus;
		set => _hm04FileStatus = value;
	}

	/// <summary>
	/// 全体データの同期状態
	/// </summary>
	public bool DataStatus
	{
		get
		{
			if (!_hm10DataStatus
				|| !_hm12DataStatus
				|| !_hr01DataStatus
				|| !_hr02DataStatus
				|| !_hr03DataStatus)
				return false;
			else
				return true;
		}
	}

	/// <summary>
	/// 全体ファイルの同期状態
	/// </summary>
	public bool FileStatus
	{
		get
		{
			if (!_hm10FileStatus
				|| !_hm12FileStatus
				|| !_hr03FileStatus
				|| !_hm04FileStatus)
				return false;
			else
				return true;
		}
	}

	/// <summary>
	/// ファイルの同期に失敗したメッセージプロンプト
	/// </summary>
	private string _fileSyncErrorMessage = "";
	public string FileSyncErrorMessage
	{
		get
		{
			_fileSyncErrorMessage = "";
			if (FileStatus)
			{
				if (!_hm04FileStatus)
					_fileSyncErrorMessage += "マップペイントファイル、";
				if (!_hm12FileStatus)
					_fileSyncErrorMessage += "図面ファイル、";
				if (!_hr03FileStatus)
					_fileSyncErrorMessage += "写真ファイル、";
				if (_hm10FileStatus)
					_fileSyncErrorMessage += "断面ファイル";
			}
			return _fileSyncErrorMessage;
		}
	}

	#endregion

	#region 公開処理方法

	public void InitDataAndFileStatus()
	{
		// 断面データ同期状態
		_hm10DataStatus = true;

		// ファイルデータ同期状態
		_hm12DataStatus = true;

		// アイテムデータ同期状態
		_hr01DataStatus = true;

		// 検査結果データ同期状態
		_hr02DataStatus = true;

		// 同期・写真テーブルデータ同期状態
		_hr03DataStatus = true;

		// 断面ファイル同期状態
		_hm10FileStatus = true;

		// ファイル同期状態
		_hm12FileStatus = true;

		// 同期・写真テーブルファイル同期状態
		_hr03FileStatus = true;

		// ペイントファイル画像同期状態
		_hm04FileStatus = true;
	}

	/// <summary>
	/// サーバーへの接続に失敗しました、ローカルネットワーク接続を確認してください。
	/// </summary>
	/// <returns>链接成功：true , 链接失败：false</returns>
	public bool CheckNetworkConnection()
	{
		try
		{
			// UWP 检测网络状态 检测网络是否可用
			// MAUI 检测网络状态 - 使用 Microsoft.Maui.Networking.Connectivity
			NetworkAccess accessType = Connectivity.Current.NetworkAccess;
			bool isInternetAvailable = (accessType == NetworkAccess.Internet);

			if (isInternetAvailable)
			{
				string server = Preferences.Default.Get("Server", "");
				string serverPort = Preferences.Default.Get("ServerPort", "");
				string serverTimeOut = Preferences.Default.Get("ServerTimeOut", "");

				int ret = CheckWcfAsync(server, serverPort, serverTimeOut);
				if (ret == 1)
				{
					return true;
				}
			}
		}
		catch
		{
			return false;
		}

		return false;
	}

	public string GetSystemDateTime()
	{
		return _serviceApi02.GetSystemDateTime();
	}

	/// <summary>
	/// コントロールファイル
	/// </summary>
	/// <returns></returns>
	public async Task GetHC01FlagAsync()
	{
		List<HC01CONT> dataList = new List<HC01CONT>();
		IList<HC01CONTDC> dataListDC = new List<HC01CONTDC>();
		//サーバーDBの取得
		dataListDC = _serviceApi02.GetHC01CONTList();
		foreach (var item in dataListDC)
		{
			HC01CONT hc01 = new HC01CONT();
			hc01 = Mapper<HC01CONT, HC01CONTDC>(item);
			dataList.Add(hc01);
		}

		if (dataList.Count > 0)
		{
			//SQLiteデータの追加
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				//SQLiteデータの削除
				tran.DeleteAll<HC01CONT>();
				foreach (var item in dataList)
				{
					tran.InsertOrReplace(item);
				}
			});
		}
	}

	/// <summary>
	/// 会社マスター
	/// </summary>
	/// <returns></returns>
	public async Task GetHM01FlagAsync()
	{
		List<HM01KAIS> dataList = new List<HM01KAIS>();
		IList<HM01KAISDC> dataListDC = new List<HM01KAISDC>();
		//サーバーDBの取得
		dataListDC = _serviceApi02.GetHM01KAIScodeList();
		foreach (var item in dataListDC)
		{
			HM01KAIS hm01 = new HM01KAIS();
			hm01 = Mapper<HM01KAIS, HM01KAISDC>(item);
			dataList.Add(hm01);
		}

		if (dataList.Count > 0)
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				//SQLiteデータの削除
				tran.DeleteAll<HM01KAIS>();
				foreach (var item in dataList)
				{
					//SQLiteデータの追加
					tran.InsertOrReplace(item);
				}
			});
		}
	}

	/// <summary>
	/// オペレータマスター
	/// </summary>
	/// <returns></returns>
	public async Task GetHM02FlagAsync()
	{
		List<HM02OPER> dataList = new List<HM02OPER>();
		IList<HM02OPERDC> dataListDC = new List<HM02OPERDC>();
		//サーバーDBの取得
		dataListDC = _serviceApi02.GetHM02OPERcodeList();
		foreach (var item in dataListDC)
		{
			HM02OPER hm02 = new HM02OPER();
			hm02 = Mapper<HM02OPER, HM02OPERDC>(item);
			dataList.Add(hm02);
		}

		if (dataList.Count > 0)
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				//SQLiteデータの削除
				tran.DeleteAll<HM02OPER>();
				foreach (var item in dataList)
				{
					//SQLiteデータの追加
					tran.InsertOrReplace(item);
				}
			});
		}
	}

	/// <summary>
	/// 工事マスター
	/// </summary>
	/// <returns></returns>
	public async Task GetHM03FlagAsync()
	{
		List<HM03PROJ> dataList = new List<HM03PROJ>();
		IList<HM03PROJDC> dataListDC = new List<HM03PROJDC>();
		//サーバーDBの取得
		dataListDC = _serviceApi02.GetHM03PROJcodeList(null);

		foreach (var item in dataListDC)
		{
			HM03PROJ hm03 = new HM03PROJ();
			hm03 = Mapper<HM03PROJ, HM03PROJDC>(item);
			dataList.Add(hm03);
		}

		if (dataList.Count > 0)
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				//SQLiteデータの削除
				tran.DeleteAll<HM03PROJ>();
				foreach (var item in dataList)
				{
					//SQLiteデータの追加
					tran.InsertOrReplace(item);
				}
			});
		}
	}

	/// <summary>
	/// メモマスター
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM11FlagAsync()
	{
		List<HM11MEMO> dataList = new List<HM11MEMO>();
		IList<HM11MEMODC> dataListDC = new List<HM11MEMODC>();
		dataListDC = _serviceApi02.GetHM11MEMOListByCode();
		foreach (var item in dataListDC)
		{
			HM11MEMO hm11 = new HM11MEMO();
			hm11 = Mapper<HM11MEMO, HM11MEMODC>(item);
			dataList.Add(hm11);
		}
		if (dataList.Count > 0)
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				//SQLiteデータの削除
				tran.DeleteAll<HM11MEMO>();
				foreach (var item in dataList)
				{
					//SQLiteデータの追加
					tran.InsertOrReplace(item);
				}
			});
		}
	}

	/// <summary>
	/// オペレータサブテーブル
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM15FlagAsync()
	{
		List<HM15OSUB> dataList = new List<HM15OSUB>();
		IList<HM15OSUBDC> dataListDC = new List<HM15OSUBDC>();

		//サーバーDBの取得（工事コード）
		dataListDC = _serviceApi02.GetHM15OSUBListByCode(string.Empty);
		foreach (var itemDC in dataListDC)
		{
			HM15OSUB hm15 = new HM15OSUB();
			hm15 = Mapper<HM15OSUB, HM15OSUBDC>(itemDC);
			dataList.Add(hm15);
		}
		if (dataList.Count > 0)
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				//SQLiteデータの削除
				tran.DeleteAll<HM15OSUB>();
				foreach (var item in dataList)
				{
					//SQLiteデータの追加
					tran.InsertOrReplace(item);
				}
			});
		}
	}

	/// <summary>
	/// 利用バージョン
	/// </summary>
	public async Task GetHM17FlagAsync()
	{
		List<HM17VERSION> dataList = new List<HM17VERSION>();
		IList<HM17VERSIONDC> dataListDC = new List<HM17VERSIONDC>();

		//サーバーDBの取得（工事コード）
		dataListDC = _serviceApi02.GetHM17VERSIONList();

		foreach (var itemDC in dataListDC)
		{
			HM17VERSION hm17 = new HM17VERSION();
			hm17 = Mapper<HM17VERSION, HM17VERSIONDC>(itemDC);
			dataList.Add(hm17);
		}
		if (dataList.Count > 0)
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				tran.DeleteAll<HM17VERSION>();
				foreach (var item in dataList)
				{
					//SQLiteデータの追加
					tran.InsertOrReplace(item);
				}
			});
		}
	}

	/// <summary>
	/// ゼネコンマスター
	/// </summary>
	/// <returns></returns>
	public async Task GetHM22FlagAsync()
	{
		List<HM22GENECON> dataList = new List<HM22GENECON>();
		IList<HM22GENECONDC> dataListDC = new List<HM22GENECONDC>();

		//サーバーDBの取得（工事コード）
		dataListDC = _serviceApi02.GetHM22GENECONList();

		foreach (var itemDC in dataListDC)
		{
			HM22GENECON hm22 = new HM22GENECON();
			hm22 = Mapper<HM22GENECON, HM22GENECONDC>(itemDC);
			dataList.Add(hm22);
		}

		if (dataList.Count > 0)
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				tran.DeleteAll<HM22GENECON>();
				foreach (var item in dataList)
				{
					//SQLiteデータの追加
					tran.InsertOrReplace(item);
				}
			});
		}
	}

	/// <summary>
	/// IPが範囲にあるかどうかを判断する
	/// </summary>
	/// <param name="workCode">現場コード</param>
	public bool CheckIPAddress(string workCode)
	{
		try
		{
			string companyCode = _appContext.CompanyID;
			string userCode = _appContext.OperatorCD;
			string ipAddress = _serviceApi02.GetIpAddress();
			return _serviceApi02.CheckIP(companyCode, userCode, workCode, ipAddress);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 工事が終わったと判断します
	/// </summary>
	/// <param name="workCode">現場コード</param>
	/// <returns></returns>
	public bool CheckProjectFinish(string workCode)
	{
		try
		{
			IList<HM03PROJDC> hm03ListDC = new List<HM03PROJDC>();
			List<string> workCodeList = new List<string>();
			workCodeList.Add(workCode);
			hm03ListDC = _serviceApi02.GetHM03PROJcodeList(workCodeList);

			foreach (HM03PROJDC item in hm03ListDC)
			{
				if (item.HM03005 == 1)
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 利用バージョン登録、更新
	/// </summary>
	/// <param name="localSettings"></param>
	/// <param name="hm17017">0：ログアウト 1：ログイン</param>
	public async Task UpdateHM17Async(int hm17017, string workCode)
	{
		try
		{
			string companyCode = _appContext.CompanyID;
			string operatorId = _appContext.OperatorID;
			string operatorCode = _appContext.OperatorCD;
			string operatorName = _appContext.Name;
			string ipAddress = _serviceApi02.GetIpAddress();

			HM17VERSION hm17 = new HM17VERSION();
			hm17.HM17001 = DeviceInfo.Current.Name;
			hm17.HM17002 = operatorCode;
			hm17.HM17003 = operatorName;
			hm17.HM17004 = "";
			hm17.HM17005 = _appContext.AppVersion;
			hm17.HM17007 = operatorName;
			hm17.HM17009 = operatorName;
			hm17.HM17011 = operatorName;
			hm17.HM17012 = companyCode;
			hm17.HM17013 = operatorId;
			hm17.HM17014 = ipAddress;
			hm17.HM17015 = await GetDeviceId();
			hm17.HM17017 = hm17017;
			hm17.HM17018 = workCode;

			await _hkksDb.UpdateHM17VERSIONAsync(hm17);

			HM17VERSIONLISTDC hm17DCList = new HM17VERSIONLISTDC();
			ArrayList list = new ArrayList();
			//ClassをClassDCに転化する。
			HM17VERSIONDC hm17DC = new HM17VERSIONDC();
			hm17DC.HM17001 = hm17.HM17001;
			hm17DC.HM17002 = hm17.HM17002;
			hm17DC.HM17003 = hm17.HM17003;
			hm17DC.HM17004 = hm17.HM17004;
			hm17DC.HM17005 = hm17.HM17005;
			hm17DC.HM17007 = hm17.HM17007;
			hm17DC.HM17009 = hm17.HM17009;
			hm17DC.HM17011 = hm17.HM17011;
			hm17DC.HM17012 = companyCode;
			hm17DC.HM17013 = operatorId;
			hm17DC.HM17014 = ipAddress;
			hm17DC.HM17015 = await GetDeviceId();
            hm17DC.HM17017 = hm17017;
			hm17DC.HM17018 = workCode;

			list.Add(hm17DC);
			hm17DCList.HM17VERSION = list;
			_serviceApi02.UpdateMasterTable(hm17DCList);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// UWPにおけるディスク容量不足の判断
	/// </summary>
	/// <returns></returns>
	public Task<bool> CheckDriveSpaceAsync(int requiredMB = 20)
	{
		try
		{
			long requiredSpaceInBytes = requiredMB * 1024L * 1024L;

#if WINDOWS
			// Windows
			string appDataPath = FileSystem.AppDataDirectory;
			DriveInfo drive = new DriveInfo(Path.GetPathRoot(appDataPath));
			return Task.FromResult(drive.AvailableFreeSpace >= requiredSpaceInBytes);

#elif ANDROID
                    // Android
                    var statFs = new Android.OS.StatFs(FileSystem.AppDataDirectory);
                    long availableBytes = statFs.AvailableBlocksLong * statFs.BlockSizeLong;
                    return Task.FromResult(availableBytes >= requiredSpaceInBytes);
    
#elif IOS || MACCATALYST
			// iOS / Mac
			var attributes = Foundation.NSFileManager.DefaultManager.GetFileSystemAttributes(FileSystem.AppDataDirectory);
			if (attributes != null)
			{
				return Task.FromResult((long)attributes.FreeSize >= requiredSpaceInBytes);
			}
			return Task.FromResult(false);

#else
                    // 其他平台，使用通用方法
                    string appDataPath = FileSystem.AppDataDirectory;
                    DriveInfo drive = new DriveInfo(Path.GetPathRoot(appDataPath));
                    return Task.FromResult(drive.AvailableFreeSpace >= requiredSpaceInBytes);
#endif
		}
		catch (Exception)
		{
			return Task.FromResult(false);
		}
	}

	/// <summary>
	/// 検証端末制限
	/// </summary>
	public async Task<bool> CheckUUID()
	{
		try
		{
			HM26DEVRESDC hm26DC = new HM26DEVRESDC();
			hm26DC.HM26001 = _appContext.CompanyID;
			hm26DC.HM26002 = _appContext.OperatorCD;
			hm26DC.HM26003 = await GetDeviceId();
			hm26DC.HM26004 = DeviceInfo.Current.Name;
			hm26DC.HM26007 = 2;
			hm26DC.HM26008 = 0;
			hm26DC.HM26010 = _appContext.Name;
			hm26DC.HM26012 = _appContext.Name;

			return _serviceApi02.CheckUUID(hm26DC);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 写真テーブル
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <param name="isIncludePhotoFile">写真ファイルを含む</param>
	/// <param name="passedDay">過去の日</param>
	/// <returns></returns>
	public async Task<(List<HR03SYAS> serverHR03List,
		List<HR03SYAS> localHR03List,
		List<HR03SYAS> uploadHR03ImageList,
		List<HR03SYAS> downloadHR03ImageList)> GetHR03SYASAsync(string workCode, bool isIncludePhotoFile, int passedDay, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		List<HR03SYASDC> liHR03DCSer = new List<HR03SYASDC>();
		List<HR03SYAS> liHR03Ser = new List<HR03SYAS>();
		List<HR03SYAS> liHR03Local = new List<HR03SYAS>();
		List<HR03SYAS> dataListSer = new List<HR03SYAS>();
		List<HR03SYAS> dataListLocal = new List<HR03SYAS>();
		List<HR03SYAS> HR03ImageList = new List<HR03SYAS>();

		List<HR03SYAS> serverHR03List = new();
		List<HR03SYAS> localHR03List = new();
		List<HR03SYAS> uploadHR03ImageList = new();
		List<HR03SYAS> downloadHR03ImageList = new();

		// HR06表によるHR03表更新
		var result = await _hkksDb.Db.ExecuteAsync("DELETE FROM hr03syas WHERE EXISTS(SELECT 1 FROM hr06syasdel WHERE TRIM(hr03syas.hr03001)= TRIM(hr06001) AND TRIM(hr03syas.hr03002)= TRIM(hr06002) AND HR03001=?)", workCode);

		Dictionary<string, string> dictionaryDownLoadTime = new Dictionary<string, string>();
		string downLoadTime = _downLoadTimeUtils.GetDownLoadTime(workCode, "HR03SYAS");
		dictionaryDownLoadTime.Add(workCode, downLoadTime);

		int count = _serviceApi02.GetHR03SYASListCount(workCode, downLoadTime);

		IList<HR03SYASDC> hr03DCListSer = new List<HR03SYASDC>();
		if (count > 10000)
		{
			int index = Convert.ToInt32(Math.Ceiling((decimal)count / 10000));
			IList<HR03SYASDC> hr03List = new List<HR03SYASDC>();
			for (int i = 0; i < index; i++)
			{
				string message = $"写真:{i + 1}/{index}";
				await asyncMethod(message, currentStep, totalSteps);
				hr03List = _serviceApi02.GetHR03SYASListByPage(workCode, downLoadTime, 10000, (int)(i * 10000));
				if (hr03DCListSer.Count == 0)
				{
					hr03DCListSer = hr03List;
				}
				else
				{
					hr03DCListSer = hr03DCListSer.Union(hr03List).ToList();
				}
			}
		}
		else
		{
			//サーバーDBの取得
			hr03DCListSer = _serviceApi02.GetHR03SYASListByCode(workCode, downLoadTime);
		}

		liHR03DCSer.AddRange(hr03DCListSer);

		IList<HR03SYAS> hr03ListLocal = await _hkksDb.GetHR03ListAsync(workCode, downLoadTime, passedDay);
		liHR03Local.AddRange(hr03ListLocal);

		// 転送
		foreach (var itemDC in liHR03DCSer)
		{
			HR03SYAS hr03 = new HR03SYAS();
			hr03 = Mapper<HR03SYAS, HR03SYASDC>(itemDC);
			liHR03Ser.Add(hr03);
		}

		for (int iRowCount = 0; iRowCount < liHR03Local.Count; iRowCount++)
		{
			int isExists = 0;
			if (isIncludePhotoFile)
			{
				if (liHR03Local[iRowCount].HR03002.Trim() != "")
				{
					string fileName = liHR03Local[iRowCount].HR03017 == 0 ? $"{liHR03Local[iRowCount].HR03002.Trim()}.jpg" : $"{liHR03Local[iRowCount].HR03002.Trim()}.svg";
					string filePath = Path.Combine(_appContext.AppDataFoler, workCode, "photo", fileName);

					if (!File.Exists(filePath))
					{
						isExists = 1;
					}
					else
					{
						FileInfo f = new FileInfo(filePath);
						if (f.Length == 0)
						{
							isExists = 1;
						}
					}
				}
			}

			HR03SYAS hr03Change = liHR03Ser.Find(m => m.HR03001 == liHR03Local[iRowCount].HR03001 && m.HR03002.Trim() == liHR03Local[iRowCount].HR03002.Trim());

			if (hr03Change != null)
			{
				liHR03Ser.Remove(hr03Change);

				if (DateTime.Compare(Convert.ToDateTime(liHR03Local[iRowCount].HR03013), Convert.ToDateTime(hr03Change.HR03013)) > 0)
				{
					liHR03Local[iRowCount].HR03002 = liHR03Local[iRowCount].HR03002.PadRight(46, ' ');
					liHR03Local[iRowCount].HR03016 = _appContext.Name;
					dataListSer.Add(liHR03Local[iRowCount]);
				}

				else if (DateTime.Compare(Convert.ToDateTime(liHR03Local[iRowCount].HR03013), Convert.ToDateTime(hr03Change.HR03013)) < 0)
				{
					hr03Change.HR03002 = liHR03Local[iRowCount].HR03002.PadRight(46, ' ');
					dataListLocal.Add(hr03Change);
				}
			}
			else
			{
				if (dictionaryDownLoadTime[liHR03Local[iRowCount].HR03001] != "")
				{
					if (DateTime.Compare(Convert.ToDateTime(liHR03Local[iRowCount].HR03013), Convert.ToDateTime(dictionaryDownLoadTime[liHR03Local[iRowCount].HR03001])) > 0)
					{
						liHR03Local[iRowCount].HR03002 = liHR03Local[iRowCount].HR03002.PadRight(46, ' ');
						liHR03Local[iRowCount].HR03016 = _appContext.Name;

						dataListSer.Add(liHR03Local[iRowCount]);
					}
				}
				else
				{
					liHR03Local[iRowCount].HR03002 = liHR03Local[iRowCount].HR03002.PadRight(46, ' ');
					liHR03Local[iRowCount].HR03016 = _appContext.Name;

					dataListSer.Add(liHR03Local[iRowCount]);
				}
			}

			if (isExists == 1)
			{
				HR03ImageList.Add(liHR03Local[iRowCount]);
			}
		}
		dataListLocal.AddRange(liHR03Ser);

		serverHR03List = dataListSer;
		localHR03List = dataListLocal;

		uploadHR03ImageList = new List<HR03SYAS>();
		downloadHR03ImageList = new List<HR03SYAS>();

		for (int i = 0; i < dataListSer.Count; i++)
		{
			if (dictionaryDownLoadTime[dataListSer[i].HR03001] != "")
			{
				if (DateTime.Compare(Convert.ToDateTime(dataListSer[i].HR03020), Convert.ToDateTime(dictionaryDownLoadTime[dataListSer[i].HR03001])) > 0)
				{
					uploadHR03ImageList.Add(dataListSer[i]);
				}
			}
			else
			{
				uploadHR03ImageList.Add(dataListSer[i]);
			}
		}

		if (isIncludePhotoFile)
		{
			for (int i = 0; i < dataListLocal.Count; i++)
			{
				if (dictionaryDownLoadTime[dataListLocal[i].HR03001] != "")
				{
					if (DateTime.Compare(Convert.ToDateTime(dataListLocal[i].HR03020), Convert.ToDateTime(dictionaryDownLoadTime[dataListLocal[i].HR03001])) > 0)
					{
						downloadHR03ImageList.Add(dataListLocal[i]);
					}
				}
				else
				{
					downloadHR03ImageList.Add(dataListLocal[i]);
				}
			}
			downloadHR03ImageList = HR03ImageList.Concat(downloadHR03ImageList).ToList();
		}
		else
		{
			downloadHR03ImageList = new List<HR03SYAS>();
		}
		return (serverHR03List, localHR03List, uploadHR03ImageList, downloadHR03ImageList);
	}

	/// <summary>
	/// マップマスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <param name="isIncludeDrawingFile">図面ファイルを含む</param>
	/// <returns></returns>
	public async Task<List<HM04MAPM>> GetHM04MAPMAsync(string workCode, bool isIncludeDrawingFile)
	{
		List<HM04MAPM> hm04List = new List<HM04MAPM>();
		IList<HM04MAPMDC> hm04DCList = new List<HM04MAPMDC>();
		try
		{
			hm04DCList = _serviceApi02.GetHM04MAPMListByCode(workCode);
			foreach (var item in hm04DCList)
			{
				HM04MAPM hm04 = new HM04MAPM();
				hm04 = Mapper<HM04MAPM, HM04MAPMDC>(item);
				hm04List.Add(hm04);
			}
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				tran.Execute($"delete from HM04MAPM where HM04001 in ('{workCode}')");
				foreach (var item in hm04List)
				{
					tran.InsertOrReplace(item);
				}
			});

			if (isIncludeDrawingFile)
			{
				return await _hkksDb.Db.QueryAsync<HM04MAPM>($"select * from HM04MAPM where HM04001 in ('{workCode}') and trim(hm04042)!=''");
			}
			else
			{
				return new List<HM04MAPM>();
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return new List<HM04MAPM>();
		}
	}

	/// <summary>
	/// ファイルマスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <param name="isIncludeDrawingFile">図面ファイルを含む</param>
	/// <returns></returns>
	public async Task<(List<HM12FILE> hm12List, List<HM12FILE> hm12ImageList)> GetHM12FILEAsync(string workCode, bool isIncludeDrawingFile)
	{
		List<HM12FILE> hm12List = new List<HM12FILE>();
		List<HM12FILE> hm12ImageList = new List<HM12FILE>();
		Dictionary<string, string> dicHM12 = new Dictionary<string, string>();
		try
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				string sql = $" SELECT HM12002,HM12003 FROM HM12FILE WHERE hm12001='{workCode}'";
				List<HM12FILE> local_hm12ListAll = tran.Query<HM12FILE>(sql);
				IList<HM12FILEDC> server_hm12002List = _serviceApi02.GetHM12FILEKeyListByCode(workCode);

				List<string> local_hm12002List = new List<string>();
				List<string> dc_hm12002List = new List<string>();
				List<string> diff_hm12002List = new List<string>();
				foreach (HM12FILEDC hm12 in server_hm12002List)
				{
					dicHM12.Add(hm12.HM12002, hm12.HM12003);
				}
				if (local_hm12ListAll.Count > 0)
				{
					local_hm12002List = local_hm12ListAll.Select(p => p.HM12002).ToList();
					dc_hm12002List = server_hm12002List.Select(p => p.HM12002).ToList();
					diff_hm12002List = local_hm12002List.Except(dc_hm12002List).ToList();
					// sqlite無効データの削除
					for (int i = 0; i < diff_hm12002List.Count; i++)
					{
						tran.Execute("delete from HM12FILE where HM12001=? and HM12002=?", workCode, diff_hm12002List[i]);
						local_hm12002List.Remove(diff_hm12002List[i]);
					}
				}

				// 前回ダウンロードしたデータの最大更新時間
				string downLoadTime = _downLoadTimeUtils.GetDownLoadTime(workCode, "HM12FILE");
				IList<HM12FILEDC> dataListDC = new List<HM12FILEDC>();

				//サーバーDBの取得（工事コード）
				dataListDC = _serviceApi02.GetHM12FILEListByCode(workCode, downLoadTime);
				dc_hm12002List = dataListDC.Select(p => p.HM12002).ToList();
				// サービスから削除されたデータを計算します。
				diff_hm12002List = local_hm12002List.Intersect(dc_hm12002List).ToList();
				// sqlite無効データの削除
				for (int i = 0; i < diff_hm12002List.Count; i++)
				{
					tran.Execute("delete from HM12FILE where HM12001=? and HM12002=?", workCode, diff_hm12002List[i]);
					local_hm12002List.Remove(diff_hm12002List[i]);
				}

				if (isIncludeDrawingFile)
				{
					for (int i = 0; i < local_hm12002List.Count; i++)
					{
						string hm12002 = local_hm12002List[i].Trim();
						int isExists = 0;
						if (!string.IsNullOrWhiteSpace(local_hm12002List[i]))
						{
							string fileName = $"{hm12002}{Path.GetExtension(dicHM12[hm12002].Trim())}";
							string filePath = Path.Combine(_appContext.AppDataFoler, workCode, fileName);

							if (!File.Exists(filePath))
							{
								isExists = 1;
							}
							else
							{
								FileInfo f = new FileInfo(filePath);
								if (f.Length == 0)
								{
									isExists = 1;
								}
							}
						}
						if (isExists == 1)
						{
							HM12FILE hm12 = new HM12FILE();
							hm12.HM12001 = workCode;
							hm12.HM12002 = hm12002;
							hm12.HM12003 = dicHM12[hm12002].Trim();
							hm12ImageList.Add(hm12);
						}
					}
				}

				foreach (var itemDC in dataListDC)
				{
					HM12FILE hm12 = new HM12FILE();
					hm12 = Mapper<HM12FILE, HM12FILEDC>(itemDC);
					hm12List.Add(hm12);
				}
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
		finally
		{
			if (isIncludeDrawingFile)
			{
				hm12ImageList = hm12List.Concat(hm12ImageList).ToList();
			}
			else
			{
				hm12ImageList = new List<HM12FILE>();
			}
		}
		return (hm12List, hm12ImageList);
	}

	/// <summary>
	/// 断面マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task<(List<HM10DANM> hm10List, bool result)> GetHM10DANMListAsync(string workCode)
	{
		List<HM10DANM> hm10List = new List<HM10DANM>();
		List<HM10DANM> hm10ListOut = new List<HM10DANM>();
		List<HM10DANM> hm10025ListOut = new List<HM10DANM>();
		bool result = false;
		string strHM10003 = string.Empty;
		try
		{
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				string sql = $" SELECT HM10003,HM10025 FROM hm10danm WHERE hm10001='{workCode}' ";
				List<HM10DANM> local_hm10List = tran.Query<HM10DANM>(sql);
				IList<HM10DANMDC> server_hm10List = _serviceApi02.GetHM10DANMKeyListByCode(workCode);
				List<string> local_hm10003List = new List<string>();
				List<string> diff_hm10003List = new List<string>();
				if (local_hm10List.Count > 0)
				{
					local_hm10003List = local_hm10List.Select(p => p.HM10003).ToList();
					List<string> DC_HM10003List = server_hm10List.Select(p => p.HM10003).ToList();
					diff_hm10003List = local_hm10003List.Except(DC_HM10003List).ToList();
					for (int i = 0; i < diff_hm10003List.Count; i++)
					{
						tran.Execute("delete from HM10DANM where HM10001=? and HM10003=?", workCode, diff_hm10003List[i]);
					}

					List<string> HM10025List = local_hm10List
					.Where(p => !diff_hm10003List.Contains(p.HM10003) && p.HM10025.ToString().Trim() != "" && p.HM10025.ToString().Trim() != "0")
					.Select(p => p.HM10025).ToList();

					for (int i = 0; i < HM10025List.Count; i++)
					{
						int isExists = 0;
						if (HM10025List[i].Trim() != "")
						{
							string fileName = $"{HM10025List[i].Trim()}.jpg";
							string filePath = Path.Combine(_appContext.AppDataFoler, workCode, "danm", fileName);

							if (!File.Exists(filePath))
							{
								isExists = 1;
							}
							else
							{
								FileInfo f = new FileInfo(filePath);
								if (f.Length == 0)
								{
									isExists = 1;
								}
							}
						}
						if (isExists == 1)
						{
							HM10DANM hm10 = new HM10DANM();
							hm10.HM10001 = workCode;
							hm10.HM10025 = HM10025List[i].Trim();
							hm10025ListOut.Add(hm10);
						}
					}
				}
				// 前回ダウンロードしたデータの最大更新時間
				string downLoadTime = _downLoadTimeUtils.GetDownLoadTime(workCode, "HM10DANM");

				List<string> Update_HM10003List = new List<string>();
				if (downLoadTime != "")
				{
					Update_HM10003List = server_hm10List.Where(p => p.HM10015 > Convert.ToDateTime(downLoadTime)).Select(p => p.HM10003).ToList();
				}
				else
				{
					Update_HM10003List = server_hm10List.Select(p => p.HM10003).ToList();
				}
				diff_hm10003List = local_hm10003List.Intersect(Update_HM10003List).ToList();
				for (int i = 0; i < diff_hm10003List.Count; i++)
				{
					tran.Execute("delete from HM10DANM where HM10001=? and HM10003=?", workCode, diff_hm10003List[i]);
				}
				for (int i = 0; i < Update_HM10003List.Count; i++)
				{

					if ((!(i == 0) && (i % 10) == 0) || i == (Update_HM10003List.Count - 1))
					{
						strHM10003 = $"{strHM10003},'{Update_HM10003List[i]}'";
						if (strHM10003.StartsWith(","))
						{
							strHM10003 = strHM10003.Substring(1);
						}

						IList<HM10DANMDC> dataListTempDC = _serviceApi02.GetHM10DANMByCode(workCode, strHM10003);

						foreach (var itemDC in dataListTempDC)
						{
							HM10DANM hm10item = new HM10DANM();
							hm10item = Mapper<HM10DANM, HM10DANMDC>(itemDC);
							hm10List.Add(hm10item);
						}

						strHM10003 = string.Empty;
					}
					else
					{
						strHM10003 = $"{strHM10003},'{Update_HM10003List[i]}'";
					}
				}
				if (hm10List.Count > 0)
				{
					int count = 0;
					foreach (var item in hm10List)
					{
						count += tran.InsertOrReplace(item);
					}
					result = count == hm10List.Count ? true : false;
					_hm10DataStatus = result;
				}
			});
		}
		catch (Exception ex)
		{
			_hm10DataStatus = false;
			_logger.LogError(ex.ToString());
		}
		finally
		{
			if (hm10List.Count > 0)
			{
				hm10ListOut = hm10List.Where<HM10DANM>(p => !p.HM10025.Trim().Equals("") && !p.HM10025.Trim().Equals("0")).ToList();
			}
			else
			{
				hm10ListOut = new List<HM10DANM>();
			}

			for (int i = hm10025ListOut.Count - 1; i >= 0; i--)
			{
				if (hm10ListOut.Where<HM10DANM>(p => p.HM10001.Equals(hm10025ListOut[i].HM10001) && p.HM10025.Equals(hm10025ListOut[i].HM10025)).Count() > 0)
				{
					hm10025ListOut.RemoveAt(i);
				}
			}
			hm10ListOut = hm10ListOut.Union(hm10025ListOut).ToList();
		}
		return (hm10ListOut, result);

	}

	/// <summary>
	/// 階マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM05Async(string workCode)
	{
		List<HM05KAIM> dataList = new List<HM05KAIM>();
		IList<HM05KAIMDC> dataListDC = _serviceApi02.GetHM05KAIMListByCode(workCode);
		try
		{
			foreach (var itemDC in dataListDC)
			{
				HM05KAIM hm05 = new HM05KAIM();
				hm05 = Mapper<HM05KAIM, HM05KAIMDC>(itemDC);
				dataList.Add(hm05);
			}
			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM05KAIM where HM05001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 部位マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM06Async(string workCode)
	{
		List<HM06BUIM> dataList = new List<HM06BUIM>();
		try
		{
			IList<HM06BUIMDC> dataListDC = _serviceApi02.GetHM06BUIMListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM06BUIM hm06 = new HM06BUIM();
				hm06 = Mapper<HM06BUIM, HM06BUIMDC>(itemDC);
				dataList.Add(hm06);
			}
			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM06BUIM where HM06001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 工区マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM07Async(string workCode)
	{
		List<HM07KOKU> dataList = new List<HM07KOKU>();
		try
		{
			IList<HM07KOKUDC> dataListDC = _serviceApi02.GetHM07KOKUListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM07KOKU hm07 = new HM07KOKU();
				hm07 = Mapper<HM07KOKU, HM07KOKUDC>(itemDC);
				dataList.Add(hm07);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM07KOKU where HM07001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// グループマスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM08Async(string workCode)
	{
		List<HM08GRPM> dataList = new List<HM08GRPM>();
		try
		{
			IList<HM08GRPMDC> dataListDC = _serviceApi02.GetHM08GRPMListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM08GRPM hm08 = new HM08GRPM();
				hm08 = Mapper<HM08GRPM, HM08GRPMDC>(itemDC);
				dataList.Add(hm08);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM08GRPM where HM08001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 工程マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM09Async(string workCode)
	{
		List<HM09PROC> dataList = new List<HM09PROC>();
		try
		{
			IList<HM09PROCDC> dataListDC = _serviceApi02.GetHM09PROCListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM09PROC hm09 = new HM09PROC();
				hm09 = Mapper<HM09PROC, HM09PROCDC>(itemDC);
				dataList.Add(hm09);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM09PROC where HM09001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	public async Task<bool> SetHM12UPDATEAsync(List<HM12FILE> dataList)
	{
		try
		{
			if (dataList.Count == 0) return false;

			int resultCount = 0;
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				foreach (var item in dataList)
				{
					resultCount += tran.InsertOrReplace(item);
				}
			});
			bool result = resultCount != dataList.Count ? false : true;
			_hm12DataStatus = result;
			return result;
		}
		catch (Exception ex)
		{
			_hm12DataStatus = false;
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 確認項目マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM13Async(string workCode)
	{
		List<HM13KNKM> dataList = new List<HM13KNKM>();
		try
		{
			IList<HM13KNKMDC> dataListDC = _serviceApi02.GetHM13KNKMListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM13KNKM hm13 = new HM13KNKM();
				hm13 = Mapper<HM13KNKM, HM13KNKMDC>(itemDC);
				dataList.Add(hm13);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM13KNKM where HM13001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// マップガイドマスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM14Async(string workCode)
	{
		List<HM14GUID> dataList = new List<HM14GUID>();
		try
		{
			IList<HM14GUIDDC> dataListDC = _serviceApi02.GetHM14GUListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM14GUID hm14 = new HM14GUID();
				hm14 = Mapper<HM14GUID, HM14GUIDDC>(itemDC);
				hm14.HM14013 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				hm14.HM14014 = _appContext.Name;
				dataList.Add(hm14);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM14GUID where HM14001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 撮影方向マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM16Async(string workCode)
	{
		List<HM16SHDIR> dataList = new List<HM16SHDIR>();
		try
		{
			IList<HM16SHDIRDC> dataListDC = _serviceApi02.GetHM16SHDIRListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM16SHDIR hm16 = new HM16SHDIR();
				hm16 = Mapper<HM16SHDIR, HM16SHDIRDC>(itemDC);
				dataList.Add(hm16);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM16SHDIR where HM16001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// マップガイド色マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM19Async(string workCode)
	{
		List<HM19GUIDCOLOR> dataList = new List<HM19GUIDCOLOR>();
		try
		{
			IList<HM19GUIDCOLORDC> dataListDC = _serviceApi02.GetHM19GUIDCOLORListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM19GUIDCOLOR hm19 = new HM19GUIDCOLOR();
				hm19 = Mapper<HM19GUIDCOLOR, HM19GUIDCOLORDC>(itemDC);
				dataList.Add(hm19);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM19GUIDCOLOR where HM19001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// マップガイドヘッダマスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM20Async(string workCode)
	{
		List<HM20GUIDHEAD> dataList = new List<HM20GUIDHEAD>();
		try
		{
			IList<HM20GUIDHEADDC> dataListDC = _serviceApi02.GetHM20GUIDHEADListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM20GUIDHEAD hm20 = new HM20GUIDHEAD();
				hm20 = Mapper<HM20GUIDHEAD, HM20GUIDHEADDC>(itemDC);
				dataList.Add(hm20);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM20GUIDHEAD where HM20001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 分類マスター
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHM23Async(string workCode)
	{
		List<HM23BUNRUI> dataList = new List<HM23BUNRUI>();
		try
		{
			IList<HM23BUNRUIDC> dataListDC = _serviceApi02.GetHM23BUNRUIListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HM23BUNRUI hm23 = new HM23BUNRUI();
				hm23 = Mapper<HM23BUNRUI, HM23BUNRUIDC>(itemDC);
				dataList.Add(hm23);
			}

			if (dataList.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					tran.Execute("delete from HM23BUNRUI where HM23001=?", workCode);
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				});
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// アイテムテーブル
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task<bool> GetHR01Async(string workCode, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		bool result = false;
		List<HR01ITEM> dataList = new List<HR01ITEM>();
		try
		{
			string sql = $" SELECT HR01003 FROM HR01ITEM WHERE hr01001='{workCode}'";
			List<HR01ITEM> list_local_HR01ITEM = await _hkksDb.Db.QueryAsync<HR01ITEM>(sql);

			List<string> local__HR01003List = new List<string>();
			List<string> DC_HR01003List = new List<string>();
			List<string> DIFF_HR01003List = new List<string>();

			if (list_local_HR01ITEM.Count > 0)
			{
				IList<HR01ITEMDC> dataListKeyDC = new List<HR01ITEMDC>();
				dataListKeyDC = _serviceApi02.GetHR01ITEMKeyListByCode(workCode);
				local__HR01003List = list_local_HR01ITEM.Select(p => p.HR01003).ToList();
				DC_HR01003List = dataListKeyDC.Select(p => p.HR01003).ToList();
				// サービスから削除されたデータを計算します。
				DIFF_HR01003List = local__HR01003List.Except(DC_HR01003List).ToList();

				for (int i = 0; i < DIFF_HR01003List.Count; i++)
				{
					await _hkksDb.Db.ExecuteAsync("delete from HR01ITEM where HR01001=? and HR01003=?", workCode, DIFF_HR01003List[i]);
					await _hkksDb.Db.ExecuteAsync("delete from HR02KSKK where HR02001=? and HR02002=?", workCode, DIFF_HR01003List[i]);
					await _hkksDb.Db.ExecuteAsync("delete from HR03SYAS where HR03001=? and HR03003=?", workCode, DIFF_HR01003List[i]);
					await _hkksDb.Db.ExecuteAsync("delete from HR05KOKUMINFO where HR05001=? and HR05002=?", workCode, DIFF_HR01003List[i]);
				}
			}
			string downLoadTime = _downLoadTimeUtils.GetDownLoadTime(workCode, "HR01ITEM");
			int count = _serviceApi02.GetHR01ITEMListCount(workCode, downLoadTime);
			IList<HR01ITEMDC> dataListDC = new List<HR01ITEMDC>();

			if (count > _pageSize)
			{
				int index = Convert.ToInt32(Math.Ceiling(count / _pageSize));
				IList<HR01ITEMDC> hr01List = new List<HR01ITEMDC>();
				for (int i = 0; i < index; i++)
				{
					string message = $"アイテム:{i + 1}/{index}";
					await asyncMethod(message, currentStep, totalSteps);
					hr01List = _serviceApi02.GetHR01ITEMListByPage(workCode, downLoadTime, (int)_pageSize, (int)(i * _pageSize));
					if (dataListDC.Count == 0)
					{
						dataListDC = hr01List;
					}
					else
					{
						dataListDC = dataListDC.Union(hr01List).ToList();
					}
				}
			}
			else
			{
				//サーバーDBの取得（工事コード）
				dataListDC = _serviceApi02.GetHR01ITEMListByCode(workCode, downLoadTime);
			}
			//重複データを削除
			DC_HR01003List = dataListDC.Select(p => p.HR01003).ToList();
			DIFF_HR01003List = local__HR01003List.Intersect(DC_HR01003List).ToList();
			for (int i = 0; i < DIFF_HR01003List.Count; i++)
			{
				await _hkksDb.Db.ExecuteAsync("delete from HR01ITEM where HR01001=? and HR01003=?", workCode, DIFF_HR01003List[i]);
			}
			foreach (var itemDC in dataListDC)
			{
				HR01ITEM HR01 = new HR01ITEM();
				HR01 = Mapper<HR01ITEM, HR01ITEMDC>(itemDC);
				dataList.Add(HR01);
			}
			await _hkksDb.Db.RunInTransactionAsync(async tran =>
			{
				if (dataList.Count > 0)
				{
					int resultCount = 0;
					foreach (var item in dataList)
					{
						resultCount += tran.InsertOrReplace(item);
					}
					result = resultCount == dataList.Count ? true : false;
					_hr01DataStatus = result;
				}
			});
		}
		catch (Exception ex)
		{
			_hr01DataStatus = false;
			_logger.LogError(ex.ToString());
		}
		return result;
	}

	/// <summary>
	/// 検査結果テーブル
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task<bool> GetHR02Async(string workCode, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		bool hr02Result = false;
		Dictionary<string, HR02KSKK> dataListHR02KSKK = new Dictionary<string, HR02KSKK>();
		IList<HR02KSKKDC> dataListHR02KSKKDC = new List<HR02KSKKDC>();
		List<string> hm13004List = new List<string>();
		try
		{
			string downLoadTime = _downLoadTimeUtils.GetDownLoadTime(workCode, "HR02KSKK");
			int count = _serviceApi02.GetHR02KSKKListCount(workCode, downLoadTime);
			if (count > _pageSize)
			{
				int index = Convert.ToInt32(Math.Ceiling(count / _pageSize));
				IList<HR02KSKKDC> hr02List = new List<HR02KSKKDC>();
				for (int i = 0; i < index; i++)
				{
					string message = $"検査結果:{(i + 1)}/{index}";
					await asyncMethod(message, currentStep, totalSteps);
					hr02List = _serviceApi02.GetHR02KSKKListByPage(workCode, downLoadTime, (int)_pageSize, (int)(i * _pageSize));
					if (dataListHR02KSKKDC.Count == 0)
					{
						if (hr02List != null)
						{
							dataListHR02KSKKDC = hr02List;
						}
					}
					else
					{
						dataListHR02KSKKDC = dataListHR02KSKKDC.Union(hr02List).ToList();
					}
				}
			}
			else
			{
				//サーバーDBの取得
				dataListHR02KSKKDC = _serviceApi02.GetHR02KSKKListByCode2(workCode, downLoadTime);
			}
			foreach (var itemDC in dataListHR02KSKKDC)
			{
				HR02KSKK HR02 = new HR02KSKK();
				HR02.HR02001 = itemDC.HR02001;
				HR02.HR02002 = itemDC.HR02002;
				HR02.HR02003 = itemDC.HR02003;
				HR02.HR02004 = itemDC.HR02004;
				HR02.HR02005 = itemDC.HR02005;
				HR02.HR02006 = itemDC.HR02006;
				HR02.HR02007 = itemDC.HR02007;
				HR02.HR02008 = itemDC.HR02008;
				HR02.HR02009 = itemDC.HR02009;
				HR02.HR02010 = itemDC.HR02010;
				HR02.HR02011 = itemDC.HR02011;
				HR02.HR02012 = itemDC.HR02012;
				HR02.HR02013 = itemDC.HR02013.ToString("yyyy-MM-dd HH:mm:ss");
				HR02.HR02014 = itemDC.HR02014;
				HR02.HR02015 = itemDC.HR02015.ToString("yyyy-MM-dd HH:mm:ss");
				HR02.HR02016 = itemDC.HR02016;
				HR02.HR02017 = itemDC.HR02017.ToString("yyyy-MM-dd HH:mm:ss");
				HR02.HR02018 = itemDC.HR02018;
				HR02.HR02019 = itemDC.HR02019;
				string key = HR02.HR02001 + ";" + HR02.HR02002 + ";" + HR02.HR02003;
				dataListHR02KSKK.Add(key, HR02);
			}

			List<HR02KSKK> list_UWP_HR02KSKKAll = new List<HR02KSKK>();
			string sql = "";
			if (downLoadTime.Trim() != "")
			{
				sql = $" SELECT * FROM HR02KSKK WHERE hr02001='{workCode}' and hr02015>'{downLoadTime}' ";
			}
			else
			{
				sql = $" SELECT * FROM HR02KSKK WHERE hr02001='{workCode}' ";
			}
			List<HR02KSKK> list_UWP_HR02KSKK = await _hkksDb.Db.QueryAsync<HR02KSKK>(sql);
			list_UWP_HR02KSKKAll = list_UWP_HR02KSKKAll.Concat(list_UWP_HR02KSKK).ToList();

			// 確認項目マスター
			sql = $" SELECT HM13001,HM13004 FROM HM13KNKM WHERE HM13001='{workCode}' ";
			List<HM13KNKM> hm13knkmList = await _hkksDb.Db.QueryAsync<HM13KNKM>(sql);
			foreach (HM13KNKM item in hm13knkmList)
			{
				hm13004List.Add(item.HM13001.Trim() + "," + item.HM13004.Trim());
			}
			if (dataListHR02KSKK.Count > 0)
			{
				for (int i = list_UWP_HR02KSKKAll.Count - 1; i >= 0; i--)
				{
					string key = list_UWP_HR02KSKKAll[i].HR02001 + ";" + list_UWP_HR02KSKKAll[i].HR02002 + ";" + list_UWP_HR02KSKKAll[i].HR02003;
					if (dataListHR02KSKK.Keys.Contains(key))
					{
						HR02KSKK result = dataListHR02KSKK[key];
						if (DateTime.Compare(Convert.ToDateTime(result.HR02015), Convert.ToDateTime(list_UWP_HR02KSKKAll[i].HR02015)) < 0)
						{
							dataListHR02KSKK.Remove(key);
						}
						else if (DateTime.Compare(Convert.ToDateTime(result.HR02015), Convert.ToDateTime(list_UWP_HR02KSKKAll[i].HR02015)) == 0)
						{
							dataListHR02KSKK.Remove(key);
							list_UWP_HR02KSKKAll.Remove(list_UWP_HR02KSKKAll[i]);
						}
						else
						{
							list_UWP_HR02KSKKAll.Remove(list_UWP_HR02KSKKAll[i]);
						}
					}
					if (dataListHR02KSKK.Count() == 0)
					{
						break;
					}
				}
			}
			if (dataListHR02KSKK.Count > 0)
			{
				int resultCount = 0;
				var items = dataListHR02KSKK.Values.ToList();
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					foreach (var item in items)
					{
						resultCount += tran.InsertOrReplace(item);
					}
				});
				hr02Result = resultCount == dataListHR02KSKK.Count ? true : false;
				_hr02DataStatus = hr02Result;
			}
			if (list_UWP_HR02KSKKAll.Count > 0)
			{
				for (int i = 0; i < list_UWP_HR02KSKKAll.Count; i++)
				{
					// 排除已经删除了的 確認項目
					string hm13key = list_UWP_HR02KSKKAll[i].HR02001.Trim() + "," + list_UWP_HR02KSKKAll[i].HR02003.Trim();
					if (!hm13004List.Contains(hm13key))
					{
						list_UWP_HR02KSKKAll.Remove(list_UWP_HR02KSKKAll[i]);
						await _hkksDb.Db.ExecuteAsync("delete from HR02KSKK where HR02001=? and HR02002=? and HR02003=?", workCode, list_UWP_HR02KSKKAll[i].HR02002.Trim(), list_UWP_HR02KSKKAll[i].HR02003.Trim());
					}
				}
				//サーバーDBに更新する
				SetHR02Upload(list_UWP_HR02KSKKAll);
			}
			return hr02Result;
		}
		catch (Exception ex)
		{
			_hr02DataStatus = false;
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 検査履歴テーブル
	/// </summary>
	/// <param name="workCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHR04Async(string workCode)
	{
		try
		{
			var result = await _hkksDb.Db.ExecuteAsync("delete from HR04KSHIS where HR04001=? ", workCode);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 工区多辺形情報
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task GetHR05Async(string workCode)
	{
		List<HR05KOKUMINFO> dataList = new List<HR05KOKUMINFO>();
		try
		{
			IList<HR05KOKUMINFODC> dataListDC = _serviceApi02.GetHR05KOKUMINFOListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HR05KOKUMINFO hr05 = new HR05KOKUMINFO();
				hr05 = Mapper<HR05KOKUMINFO, HR05KOKUMINFODC>(itemDC);
				dataList.Add(hr05);
			}

			List<HR05KOKUMINFO> list_UWP_HR05KOKUMINFOAll = new List<HR05KOKUMINFO>();
			string sql = $" SELECT * FROM HR05KOKUMINFO WHERE hr05001='{workCode}' ";
			List<HR05KOKUMINFO> list_UWP_HR05KOKUMINFO = await _hkksDb.Db.QueryAsync<HR05KOKUMINFO>(sql);
			list_UWP_HR05KOKUMINFOAll = list_UWP_HR05KOKUMINFOAll.Concat(list_UWP_HR05KOKUMINFO).ToList();

			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				for (int i = 0; i < list_UWP_HR05KOKUMINFOAll.Count; i++)
				{
					HR05KOKUMINFO? result = dataList.FirstOrDefault(
						p => p.HR05001.Equals(list_UWP_HR05KOKUMINFOAll[i].HR05001) &&
						p.HR05002.Equals(list_UWP_HR05KOKUMINFOAll[i].HR05002) &&
						p.HR05008.Equals(list_UWP_HR05KOKUMINFOAll[i].HR05008));

					if (result != null)
					{
						dataList.Remove(result);
					}
					else
					{
						tran.Execute("delete from HR05KOKUMINFO where HR05001=? and  HR05002=? ", list_UWP_HR05KOKUMINFOAll[i].HR05001, list_UWP_HR05KOKUMINFOAll[i].HR05002);
					}
				}
				list_UWP_HR05KOKUMINFOAll.Clear();
				if (dataList.Count > 0)
				{
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				}
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 写真削除テーブル
	/// </summary>
	/// <param name="liCode"></param>
	/// <param name="outHR06List"></param>
	public async Task<List<HR06SYASDEL>> GetHR06Async(string workCode)
	{
		List<HR06SYASDEL> local_dataList = new List<HR06SYASDEL>();
		List<HR06SYASDEL> dataList = new List<HR06SYASDEL>();
		try
		{
			IList<HR06SYASDELDC> dataListDC = _serviceApi02.GetHR06SYASDELListByCode(workCode);
			foreach (var itemDC in dataListDC)
			{
				HR06SYASDEL hr06 = new HR06SYASDEL();
				hr06 = Mapper<HR06SYASDEL, HR06SYASDELDC>(itemDC);
				dataList.Add(hr06);
			}

			await _hkksDb.Db.CreateTableAsync<HR06SYASDEL>();
			List<HR06SYASDEL> list_local_HR06SYASDELAll = new List<HR06SYASDEL>();
			string sql = $" SELECT * FROM HR06SYASDEL WHERE hr06001='{workCode}'";
			List<HR06SYASDEL> list_UWP_HR06SYASDEL = await _hkksDb.Db.QueryAsync<HR06SYASDEL>(sql);
			list_local_HR06SYASDELAll = list_local_HR06SYASDELAll.Concat(list_UWP_HR06SYASDEL).ToList();

			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				for (int i = 0; i < list_local_HR06SYASDELAll.Count; i++)
				{
					HR06SYASDEL? result = dataList.FirstOrDefault(
						p => p.HR06001.Equals(list_local_HR06SYASDELAll[i].HR06001)
						&& p.HR06002.Equals(list_local_HR06SYASDELAll[i].HR06002)
						);

					if (result != null)
					{
						dataList.Remove(result);
					}
					else
					{
						local_dataList.Add(list_local_HR06SYASDELAll[i]);
					}
				}

				list_local_HR06SYASDELAll.Clear();

				if (dataList.Count > 0)
				{
					foreach (var item in dataList)
					{
						tran.InsertOrReplace(item);
					}
				}
			});
			if (local_dataList.Count > 0) SetHR06Upload(local_dataList);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
		return local_dataList;
	}

	public async Task SetHR02Async(string workCode, Func<string, int, int, Task> asyncMethod)
	{
		await asyncMethod("", 1, 6);
		Dictionary<string, HR02KSKK> dataListHR02KSKK = new Dictionary<string, HR02KSKK>();
		IList<HR02KSKKDC> dataListHR02KSKKDC = new List<HR02KSKKDC>();
		//确认项目集合
		List<string> hm13004List = new List<string>();
		string downLoadTime = _downLoadTimeUtils.GetDownLoadTime(workCode, "HR02KSKK");
		//サーバーDBの取得
		dataListHR02KSKKDC = _serviceApi02.GetHR02KSKKListByCode2(workCode, downLoadTime);

		foreach (var itemDC in dataListHR02KSKKDC)
		{
			HR02KSKK HR02 = new HR02KSKK();
			HR02.HR02001 = itemDC.HR02001;
			HR02.HR02002 = itemDC.HR02002;
			HR02.HR02003 = itemDC.HR02003;
			HR02.HR02004 = itemDC.HR02004;
			HR02.HR02005 = itemDC.HR02005;
			HR02.HR02006 = itemDC.HR02006;
			HR02.HR02007 = itemDC.HR02007;
			HR02.HR02008 = itemDC.HR02008;
			HR02.HR02009 = itemDC.HR02009;
			HR02.HR02010 = itemDC.HR02010;
			HR02.HR02011 = itemDC.HR02011;
			HR02.HR02012 = itemDC.HR02012;
			HR02.HR02013 = itemDC.HR02013.ToString("yyyy-MM-dd HH:mm:ss");
			HR02.HR02014 = itemDC.HR02014;
			HR02.HR02015 = itemDC.HR02015.ToString("yyyy-MM-dd HH:mm:ss");
			HR02.HR02016 = itemDC.HR02016;
			HR02.HR02017 = itemDC.HR02017.ToString("yyyy-MM-dd HH:mm:ss");
			HR02.HR02018 = itemDC.HR02018;
			HR02.HR02019 = itemDC.HR02019;
			string key = HR02.HR02001 + ";" + HR02.HR02002 + ";" + HR02.HR02003;
			dataListHR02KSKK.Add(key, HR02);
		}
		await asyncMethod("", 2, 6);
		try
		{
			List<HR02KSKK> list_UWP_HR02KSKKAll = new List<HR02KSKK>();
			string sql = $" SELECT HR01003 FROM HR01ITEM WHERE hr01001='{workCode}'";
			List<HR01ITEM> list_UWP_HR01ITEM = await _hkksDb.Db.QueryAsync<HR01ITEM>(sql);

			if (list_UWP_HR01ITEM.Count > 0)
			{
				IList<HR01ITEMDC> dataListKeyDC = new List<HR01ITEMDC>();
				dataListKeyDC = _serviceApi02.GetHR01ITEMKeyListByCode(workCode);

				List<string> UWP__HR01003List = list_UWP_HR01ITEM.Select(p => p.HR01003).ToList();

				List<string> DC_HR01003List = dataListKeyDC.Select(p => p.HR01003).ToList();
				// サービスから削除されたデータを計算します。
				List<string> DIFF_HR01003List = UWP__HR01003List.Except(DC_HR01003List).ToList();
				// UWP端無効データの削除
				for (int j = 0; j < DIFF_HR01003List.Count; j++)
				{
					await _hkksDb.Db.ExecuteAsync("delete from HR01ITEM where HR01001=? and HR01003=?", workCode, DIFF_HR01003List[j]);
					await _hkksDb.Db.ExecuteAsync("delete from HR02KSKK where HR02001=? and HR02002=?", workCode, DIFF_HR01003List[j]);
					await _hkksDb.Db.ExecuteAsync("delete from HR03SYAS where HR03001=? and HR03003=?", workCode, DIFF_HR01003List[j]);
					await _hkksDb.Db.ExecuteAsync("delete from HR05KOKUMINFO where HR05001=? and HR05002=?", workCode, DIFF_HR01003List[j]);
				}
			}

			if (downLoadTime.Trim() != "")
			{
				sql = $" SELECT * FROM HR02KSKK WHERE hr02001='{workCode}' and hr02015>'{downLoadTime}' ";
			}
			else
			{
				sql = $" SELECT * FROM HR02KSKK WHERE hr02001='{workCode}' ";
			}

			List<HR02KSKK> list_UWP_HR02KSKK = await _hkksDb.Db.QueryAsync<HR02KSKK>(sql);

			list_UWP_HR02KSKKAll = list_UWP_HR02KSKKAll.Concat(list_UWP_HR02KSKK).ToList();

			// 確認項目マスター
			sql = $" SELECT HM13001,HM13004 FROM HM13KNKM WHERE HM13001='{workCode}' ";
			List<HM13KNKM> hm13knkmList = await _hkksDb.Db.QueryAsync<HM13KNKM>(sql);

			foreach (HM13KNKM item in hm13knkmList)
			{
				hm13004List.Add(item.HM13001.Trim() + "," + item.HM13004.Trim());
			}
			await asyncMethod("", 3, 6);

			if (dataListHR02KSKK.Count > 0)
			{
				for (int i = list_UWP_HR02KSKKAll.Count - 1; i >= 0; i--)
				{
					string key = list_UWP_HR02KSKKAll[i].HR02001 + ";" + list_UWP_HR02KSKKAll[i].HR02002 + ";" + list_UWP_HR02KSKKAll[i].HR02003;

					if (dataListHR02KSKK.Keys.Contains(key))
					{
						HR02KSKK result = dataListHR02KSKK[key];
						if (DateTime.Compare(Convert.ToDateTime(result.HR02015), Convert.ToDateTime(list_UWP_HR02KSKKAll[i].HR02015)) < 0)
						{
							dataListHR02KSKK.Remove(key);
						}
						else if (DateTime.Compare(Convert.ToDateTime(result.HR02015), Convert.ToDateTime(list_UWP_HR02KSKKAll[i].HR02015)) == 0)
						{
							dataListHR02KSKK.Remove(key);
							list_UWP_HR02KSKKAll.Remove(list_UWP_HR02KSKKAll[i]);
						}
						else
						{
							list_UWP_HR02KSKKAll.Remove(list_UWP_HR02KSKKAll[i]);
						}
					}

					if (dataListHR02KSKK.Count() == 0)
					{
						break;
					}
				}
			}
			await asyncMethod("", 4, 6);
			if (dataListHR02KSKK.Count > 0)
			{
				await _hkksDb.Db.RunInTransactionAsync(tran =>
				{
					foreach (var item in dataListHR02KSKK.Values.ToList())
					{
						tran.InsertOrReplace(item);
					}
				});
			}
			await asyncMethod("", 5, 6);

			if (list_UWP_HR02KSKKAll.Count > 0)
			{
				for (int i = 0; i < list_UWP_HR02KSKKAll.Count; i++)
				{
					// 排除已经删除了的 確認項目
					string hm13key = list_UWP_HR02KSKKAll[i].HR02001.Trim() + "," + list_UWP_HR02KSKKAll[i].HR02003.Trim();
					if (!hm13004List.Contains(hm13key))
					{
						list_UWP_HR02KSKKAll.Remove(list_UWP_HR02KSKKAll[i]);
					}
				}

				//サーバーDBに更新する
				SetHR02Upload(list_UWP_HR02KSKKAll);
				//サーバーDBに更新する
				await SetHR04Async(list_UWP_HR02KSKKAll, workCode);
			}

			await asyncMethod("", 6, 6);
		}
		catch (Exception exp)
		{
			_logger.LogError(exp.ToString());
		}
	}

	/// <summary>
	/// 同期・写真テーブル
	/// </summary>
	/// <param name="strCode">工事コード</param>
	/// <returns></returns>
	public async Task<bool> SetHR03UPDATEAsync(List<HR03SYAS> ListSer, List<HR03SYAS> ListLod, string workCode)
	{
		try
		{
			SetHR03Upload(ListSer);
			if (ListLod.Count == 0) return false;
			int resultCount = 0;
			await _hkksDb.Db.RunInTransactionAsync(tran =>
			{
				foreach (var item in ListLod)
				{
					resultCount += tran.InsertOrReplace(item);
				}
			});

			var result = resultCount == ListLod.Count ? true : false;
			_hr03DataStatus = result;
			return result;
		}
		catch (Exception ex)
		{
			_hr03DataStatus = false;
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 同期・検査履歴テーブル
	/// </summary>
	/// <returns></returns>
	public async Task SetHR04Async(List<HR02KSKK> list_UWP_HR02KSKKAll, string workcode)
	{
		try
		{
			List<HR04KSHIS> list_UWP_HR04KSHISAll = new List<HR04KSHIS>();

			for (int i = 0; i < list_UWP_HR02KSKKAll.Count; i++)
			{
				//最大履歴Noの取得
				string strMaxCode = _serviceApi02.GetMaxHR04004ByCode(
										list_UWP_HR02KSKKAll[i].HR02001 + ";" + list_UWP_HR02KSKKAll[i].HR02002 + ";" + list_UWP_HR02KSKKAll[i].HR02003);

				List<HR04KSHIS> list_UWP_HR04KSHIS = await _hkksDb.GetHR04ListAsync(list_UWP_HR02KSKKAll[i]);

				int MaxCode = 0;
				if (strMaxCode != null)
				{
					MaxCode = Convert.ToInt32(strMaxCode);
				}

				for (int j = 0; j < list_UWP_HR04KSHIS.Count; j++)
				{
					list_UWP_HR04KSHIS[j].HR04004 = MaxCode + 1;
					MaxCode++;
				}

				list_UWP_HR04KSHISAll = list_UWP_HR04KSHISAll.Concat(list_UWP_HR04KSHIS).ToList();
			}

			if (list_UWP_HR04KSHISAll.Count > 0)
			{
				//サーバーDBに更新する。
				if (SetHR04Upload(list_UWP_HR04KSHISAll))
				{
					await _hkksDb.Db.ExecuteAsync("delete from HR04KSHIS where HR04001=? ", workcode);
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 検査履歴テーブル(アップロード)
	/// </summary>
	/// <param name="HR04">テーブルリスト</param>
	public bool SetHR04Upload(List<HR04KSHIS> HR04)
	{
		try
		{
			HR04KSHISLISTDC hr04List = new HR04KSHISLISTDC();
			ArrayList list = new ArrayList();
			//ClassをClassDCに転化する。
			for (int i = 0; i < HR04.Count; i++)
			{
				HR04KSHISDC hm04 = new HR04KSHISDC();
				hm04.HR04001 = HR04[i].HR04001;
				hm04.HR04002 = HR04[i].HR04002;
				hm04.HR04003 = HR04[i].HR04003;
				hm04.HR04004 = HR04[i].HR04004;
				hm04.HR04005 = HR04[i].HR04005;
				hm04.HR04006 = HR04[i].HR04006;
				hm04.HR04007 = HR04[i].HR04007;
				hm04.HR04008 = HR04[i].HR04008;
				hm04.HR04009 = HR04[i].HR04009;
				hm04.HR04010 = HR04[i].HR04010;
				hm04.HR04011 = HR04[i].HR04011;
				hm04.HR04012 = HR04[i].HR04012;
				hm04.HR04013 = HR04[i].HR04013;
				hm04.HR04014 = HR04[i].HR04014;
				hm04.HR04015 = Convert.ToDateTime(HR04[i].HR04015);
				hm04.HR04016 = HR04[i].HR04016;
				hm04.HMCHANGE = "UWP";
				list.Add(hm04);
			}
			hr04List.HR04ITEM = list;
			return _serviceApi02.UpdateMasterTable(hr04List);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// ペイントファイル画像のダウンロード
	/// </summary>
	public async Task DownloadPaintFilesAsync(string workCode, List<HM04MAPM> hm04List, bool isIncludeDrawingFile, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		try
		{
			if (hm04List.Count == 0 || !isIncludeDrawingFile) return;
			int allCount = hm04List.Count;
			int count = 0;
			foreach (var item in hm04List)
			{
				string message = $"ペイントファイル画像のダウンロード:{++count}/{allCount}";
				await asyncMethod(message, currentStep, totalSteps);
				using CancellationTokenSource cts = new CancellationTokenSource();
				string fileUri = $"{workCode}/paint/{item.HM04042.Trim()}.jpg";
				string localFilePath = Path.Combine(_appContext.AppDataFoler, workCode, "paint", $"{item.HM04042.Trim()}.jpg");
				bool result;
				if (_appContext.FileServerType == 2)
				{
					//ファイルサーバタイプ:webdav
					result = await _webDavClient.DownloadAsync(fileUri, localFilePath);
				}
				else
				{
					//ファイルサーバタイプ:ftp
					fileUri = $"/{_appContext.HC01013}/{fileUri}";
					result = await _fluentFtpClient.DownloadAsync(fileUri, localFilePath, ct: cts.Token);
				}
				if (!result)
				{
					_hm04FileStatus = false;
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 図面ファイル画像のダウンロード
	/// </summary>
	public async Task DownloadFilesAsync(string workCode, List<HM12FILE> hm12List, bool isIncludeDrawingFile, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		try
		{
			if (hm12List.Count == 0 || !isIncludeDrawingFile) return;
			int allCount = hm12List.Count;
			int count = 0;
			foreach (var item in hm12List)
			{
				string message = $"図面ファイル画像のダウンロード:{++count}/{allCount}";
				await asyncMethod(message, currentStep, totalSteps);
				string fileName = $"{item.HM12002.Trim()}{Path.GetExtension(item.HM12003.Trim())}";
				string fileUri = $"{workCode}/{fileName}";
				string localFilePath = Path.Combine(_appContext.AppDataFoler, workCode, fileName);
				using CancellationTokenSource cts = new CancellationTokenSource();
				bool result;
				if (_appContext.FileServerType == 2)
				{
					//ファイルサーバタイプ:webdav
					result = await _webDavClient.DownloadAsync(fileUri, localFilePath);
				}
				else
				{
					//ファイルサーバタイプ:ftp
					fileUri = $"/{_appContext.HC01013}/{fileUri}";
					result = await _fluentFtpClient.DownloadAsync(fileUri, localFilePath, ct: cts.Token);
				}
				if (!result)
				{
					_hm12FileStatus = false;
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	///  断面画像のダウンロード
	/// </summary>
	/// <returns></returns>
	public async Task DownloadDamnFilesAsync(string workCode, List<HM10DANM> hm10List, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		try
		{
			if (hm10List.Count == 0) return;
			int allCount = hm10List.Count;
			int count = 0;
			foreach (var item in hm10List)
			{
				string message = $"断面画像のダウンロード:{++count}/{allCount}";
				await asyncMethod(message, currentStep, totalSteps);
				string fileUri = $"{workCode}/danm/{item.HM10025.Trim()}.jpg";
				string localFilePath = Path.Combine(_appContext.AppDataFoler, workCode, "danm", $"{item.HM10025.Trim()}.jpg");
				using CancellationTokenSource cts = new CancellationTokenSource();
				bool result;
				if (_appContext.FileServerType == 2)
				{
					//ファイルサーバタイプ:webdav
					result = await _webDavClient.DownloadAsync(fileUri, localFilePath);
				}
				else
				{
					//ファイルサーバタイプ:ftp
					fileUri = $"/{_appContext.HC01013}/{fileUri}";
					result = await _fluentFtpClient.DownloadAsync(fileUri, localFilePath, ct: cts.Token);
				}
				if (!result)
				{
					_hm10FileStatus = false;
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 写真画像のダウンロード
	/// </summary>
	public async Task DownloadPhotoFilesAsync(string workCode, List<HR03SYAS> hr03List, bool isIncludePhotoFile, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		try
		{
			if (hr03List.Count == 0 || !isIncludePhotoFile) return;
			int allCount = hr03List.Count;
			int count = 0;
			foreach (var item in hr03List)
			{
				string message = $"写真画像のダウンロード:{++count}/{allCount}";
				foreach (var mimetype in _mimeTypes)
				{
					if (item.HR03017 == 0 && mimetype != ".jpg") continue;
					if (item.HR03017 == 1 && mimetype == ".jpg") continue;
					await asyncMethod(message, currentStep, totalSteps);
					string fileUri = $"{workCode}/photo/{item.HR03002.Trim()}{mimetype}";
					string localFilePath = Path.Combine(_appContext.AppDataFoler, workCode, "photo", $"{item.HR03002.Trim()}{mimetype}");

					if (mimetype == ".json" && !await JsonFileExistsAsync(fileUri)) continue;

					using CancellationTokenSource cts = new CancellationTokenSource();
					bool result;
					if (_appContext.FileServerType == 2)
					{
						//ファイルサーバタイプ:webdav
						result = await _webDavClient.DownloadAsync(fileUri, localFilePath);
					}
					else
					{
						//ファイルサーバタイプ:ftp
						fileUri = $"/{_appContext.HC01013}/{fileUri}";
						result = await _fluentFtpClient.DownloadAsync(fileUri, localFilePath, ct: cts.Token);
					}
					if (!result)
					{
						_hr03FileStatus = false;
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// サービス側jsonファイルが存在するかどうかを確認します
	/// </summary>
	/// <param name="fileUri">サービス側ファイルパス</param>
	/// <returns>true:存在、false:存在しない</returns>
	async Task<bool> JsonFileExistsAsync(string fileUri)
	{
		using CancellationTokenSource cts = new CancellationTokenSource(_timeout);
		bool result;
		if (_appContext.FileServerType == 2)
		{
			result = await _webDavClient.FileExistsAsync(fileUri, cts.Token);
		}
		else
		{
			fileUri = $"/{_appContext.HC01013}/{fileUri}";
			result = await _fluentFtpClient.FileExistsAsync(fileUri, cts.Token);
		}
		return result;
	}

	/// <summary>
	/// 写真ファイルをアップロード
	/// </summary>
	public async Task<bool> UploadPhotoFilesAsync(string workCode, List<HR03SYAS> hr03List, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		bool hasFaild = false;
		try
		{
			if (hr03List.Count == 0) return hasFaild;
			int allCount = hr03List.Count;
			int count = 0;
			foreach (var item in hr03List)
			{
				string message = $"写真画像のアップロード:{++count}/{allCount}";
				foreach (var mimetype in _mimeTypes)
				{
					if (item.HR03017 == 0 && mimetype != ".jpg") continue;
					if (item.HR03017 == 1 && mimetype == ".jpg") continue;
					await asyncMethod(message, currentStep, totalSteps);
					string remoteDirectory = $"{workCode}/photo";
					string remoteFileName = $"{item.HR03002.Trim()}{mimetype}";
					string localFilePath = Path.Combine(_appContext.AppDataFoler, workCode, "photo", $"{item.HR03002.Trim()}{mimetype}");

					if (mimetype == ".json" && !File.Exists(localFilePath)) continue;

					using CancellationTokenSource cts = new CancellationTokenSource(_timeout);
					if (File.Exists(localFilePath))
					{
						bool result;
						if (_appContext.FileServerType == 2)
						{
							//ファイルサーバタイプ:webdav
							result = await _webDavClient.UploadAsync(localFilePath, remoteDirectory, remoteFileName, ct: cts.Token);
						}
						else
						{
							//ファイルサーバタイプ:ftp
							string remotePath = $"/{_appContext.HC01013}/{remoteDirectory}/{remoteFileName}";
							result = await _fluentFtpClient.UploadFileAsync(localFilePath, remotePath, ct: cts.Token);
						}
						if (!result) hasFaild = true;
					}
					else
					{
						// 写真方式 0：JPG 1：SVG
						if ((item.HR03017 == 0 && mimetype == ".jpg") || (item.HR03017 == 1 && mimetype == ".svg"))
						{
							_logger.LogWarning($"UploadPhotoFilesAsync:{localFilePath}画像が存在しません");
						}
					}

				}
			}
		}
		catch (Exception ex)
		{
			hasFaild = true;
			_logger.LogError(ex.ToString());
		}
		return hasFaild;
	}

	/// <summary>
	/// 写真を削除
	/// </summary>
	/// <returns></returns>
	public async Task DeletePhotoFilesAsync(string workCode, List<HR06SYASDEL> hr06List, int currentStep, int totalSteps, Func<string, int, int, Task> asyncMethod)
	{
		try
		{
			if (hr06List.Count == 0) return;
			int allCount = hr06List.Count;
			int count = 0;
			foreach (var item in hr06List)
			{
				string message = $"写真画像のアップロード:{++count}/{allCount}";
				foreach (var mimetype in _mimeTypes)
				{
					await asyncMethod(message, currentStep, totalSteps);
					string fileUri = $"{workCode}/photo/{item.HR06002.Trim()}{mimetype}";
					using CancellationTokenSource cts = new CancellationTokenSource(_timeout);
					if (_appContext.FileServerType == 2)
					{
						//ファイルサーバタイプ:webdav
						await _webDavClient.DeleteAsync(fileUri, cts.Token);
					}
					else
					{
						//ファイルサーバタイプ:ftp
						fileUri = $"/{_appContext.HC01013}/{fileUri}";
						await _fluentFtpClient.DeleteFileAsync(fileUri, cts.Token);
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 保存日数以外の日付の画像を削除する
	/// </summary>
	/// <param name="passedDay"></param>
	/// <returns></returns>
	public async Task DeleteLocalExpiredImagesAsync(int passedDay)
	{
		List<string> hr03001List = new List<string>();
		hr03001List.Add(_appContext.WorkCD);
		List<HR03SYAS> hr03syas = await _hkksDb.GetHR03SYASListAsync(hr03001List);
		List<string> hr03002List = hr03syas.Count > 0
			? hr03syas.Where(x => x.HR03009 < passedDay).Select(x => x.HR03002).ToList()
			: new();

		string photoFolderPath = Path.Combine(_appContext.ConstructionSiteFolder, "photo");
		if (!Directory.Exists(photoFolderPath)) return;

		var files = Directory.GetFiles(photoFolderPath, "*", SearchOption.AllDirectories)
			.Where(path => _mimeTypes.Contains(Path.GetExtension(path).ToLowerInvariant()))
			.ToList();
		foreach (var file in files)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
			if (hr03002List.Contains(fileNameWithoutExtension))
			{
				File.Delete(file);
			}
		}

	}
	#endregion

	#region プライベート処理方法

	/// <summary>
	/// 写真テーブル(アップロード)
	/// </summary>
	/// <param name="HR02">テーブルリスト</param>
	bool SetHR02Upload(List<HR02KSKK> HR02)
	{
		try
		{
			HR02KSKKLISTDC hr02List = new HR02KSKKLISTDC();
			ArrayList list = new ArrayList();
			for (int i = 0; i < HR02.Count; i++)
			{
				HR02KSKKDC hm02 = new HR02KSKKDC();
				hm02.HR02001 = HR02[i].HR02001;
				hm02.HR02002 = HR02[i].HR02002;
				hm02.HR02003 = HR02[i].HR02003;
				hm02.HR02004 = HR02[i].HR02004;
				hm02.HR02005 = HR02[i].HR02005;
				hm02.HR02006 = HR02[i].HR02006;
				hm02.HR02007 = HR02[i].HR02007;
				hm02.HR02008 = HR02[i].HR02008;
				hm02.HR02009 = HR02[i].HR02009;
				hm02.HR02010 = HR02[i].HR02010;
				hm02.HR02011 = HR02[i].HR02011;
				hm02.HR02012 = HR02[i].HR02012;
				hm02.HR02013 = Convert.ToDateTime(HR02[i].HR02013);
				hm02.HR02014 = HR02[i].HR02014;
				hm02.HR02015 = Convert.ToDateTime(HR02[i].HR02015);
				hm02.HR02016 = HR02[i].HR02016;
				hm02.HR02017 = Convert.ToDateTime(HR02[i].HR02017);
				hm02.HR02018 = _appContext.Name;
				hm02.HR02019 = HR02[i].HR02019;
				hm02.HMCHANGE = "UWP";
				list.Add(hm02);
			}
			hr02List.HR02KSKK = list;
			return _serviceApi02.UpdateMasterTable(hr02List);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 検査結果テーブル(アップロード)
	/// </summary>
	/// <param name="HR03">テーブルリスト</param>
	bool SetHR03Upload(List<HR03SYAS> HR03)
	{
		try
		{
			HR03SYASLISTDC hr03List = new HR03SYASLISTDC();
			ArrayList list = new ArrayList();
			for (int i = 0; i < HR03.Count; i++)
			{
				HR03SYASDC hm03 = new HR03SYASDC();
				hm03.HR03001 = HR03[i].HR03001;
				hm03.HR03002 = HR03[i].HR03002.Trim();
				hm03.HR03003 = HR03[i].HR03003;
				hm03.HR03004 = HR03[i].HR03004;
				hm03.HR03005 = HR03[i].HR03005;
				hm03.HR03006 = HR03[i].HR03006;
				hm03.HR03007 = HR03[i].HR03007;
				hm03.HR03008 = HR03[i].HR03008;
				hm03.HR03009 = HR03[i].HR03009;
				hm03.HR03010 = HR03[i].HR03010;
				hm03.HR03011 = Convert.ToDateTime(HR03[i].HR03011);
				hm03.HR03012 = HR03[i].HR03012;
				hm03.HR03013 = Convert.ToDateTime(HR03[i].HR03013);
				hm03.HR03014 = HR03[i].HR03014;
				hm03.HR03015 = Convert.ToDateTime(HR03[i].HR03015);
				hm03.HR03016 = HR03[i].HR03016;
				hm03.HR03017 = HR03[i].HR03017;
				hm03.HR03018 = HR03[i].HR03018;
				hm03.HR03019 = HR03[i].HR03019;
				hm03.HR03020 = Convert.ToDateTime(HR03[i].HR03020);
				hm03.HMCHANGE = "UWP";
				list.Add(hm03);
			}
			hr03List.HR03ITEM = list;
			return _serviceApi02.UpdateMasterTable(hr03List);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// 検査履歴テーブル(アップロード)
	/// </summary>
	/// <param name="HR04">テーブルリスト</param>
	bool SetHR06Upload(List<HR06SYASDEL> HR06)
	{
		try
		{
			HR06SYASDELLISTDC hr06List = new HR06SYASDELLISTDC();
			System.Collections.ArrayList list = new();
			for (int i = 0; i < HR06.Count; i++)
			{
				HR06SYASDELDC hr06 = new HR06SYASDELDC();
				hr06.HR06001 = HR06[i].HR06001;
				hr06.HR06002 = HR06[i].HR06002;
				hr06.HR06003 = HR06[i].HR06003;
				hr06.HR06004 = HR06[i].HR06004;
				hr06.HR06005 = Convert.ToDateTime(HR06[i].HR06005);
				hr06.HR06006 = HR06[i].HR06006;
				hr06.HR06007 = Convert.ToDateTime(HR06[i].HR06007);
				hr06.HR06008 = HR06[i].HR06008;
				hr06.HR06009 = Convert.ToDateTime(HR06[i].HR06009);
				hr06.HR06010 = HR06[i].HR06010;
				hr06.HMCHANGE = "UWP";
				list.Add(hr06);
			}
			hr06List.HR06ITEM = list;
			return _serviceApi02.UpdateMasterTable(hr06List);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	int CheckWcfAsync(string server, string serverPort, string serverTimeOut)
	{
		try
		{
			_serviceApi02 = _aikoWcf.ServiceApi02(server, serverPort, serverTimeOut);
			return _serviceApi02.IpPortIsTrue();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return 0;
		}
	}

    /// <summary>
    /// 端末のserial numberまたはIMEI
    /// </summary>
    /// <returns></returns>
    async Task<string> GetDeviceId()
    {
        const string key = "app_device_id";

		var cached = await SecureStorage.Default.GetAsync(key);
		if (!string.IsNullOrWhiteSpace(cached))
			return cached;

		string id = string.Empty;

#if WINDOWS
    // 1. 先取 Windows 设备 UUID
    try
    {
        using var mc = new ManagementClass("Win32_ComputerSystemProduct");
        using var moc = mc.GetInstances();

        foreach (ManagementObject mo in moc)
        {
            id = mo.Properties["UUID"]?.Value?.ToString() ?? string.Empty;
            break;
        }

        // 有些机器会返回全 0 UUID，当作无效值处理
        if (string.Equals(id, "00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase))
        {
            id = string.Empty;
        }
    }
    catch
    {
        id = string.Empty;
    }

    // 2. 取不到时，回退到发布者级系统 ID
    if (string.IsNullOrWhiteSpace(id))
    {
        var info = SystemIdentification.GetSystemIdForPublisher();
        if (info?.Id != null)
        {
            id = CryptographicBuffer.EncodeToHexString(info.Id);
        }
    }

#elif IOS || MACCATALYST
        id = UIDevice.CurrentDevice.IdentifierForVendor?.AsString() ?? string.Empty;
#endif

        // 3. 最终还取不到，就生成一个本地稳定 ID
        if (string.IsNullOrWhiteSpace(id))
            id = Guid.NewGuid().ToString("N");

        await SecureStorage.Default.SetAsync(key, id);
        return id;
    }

    /// <summary>
    /// ClassDCをClassに転化する。
    /// </summary>
    /// <typeparam name="D">Class</typeparam>
    /// <typeparam name="S">ClassDC</typeparam>
    /// <param name="s">ClassDCのデータ</param>
    /// <returns></returns>
    D Mapper<D, S>(S s)
	{
		D d = Activator.CreateInstance<D>();
		try
		{
			//型を取得します
			var Types = s.GetType();
			var Typed = typeof(D);
			//プロパティの属性の取得
			foreach (PropertyInfo sp in Types.GetProperties())
			{
				foreach (PropertyInfo dp in Typed.GetProperties())
				{
					//属性名
					if (dp.Name == sp.Name)
					{
						if (sp.PropertyType == typeof(System.DateTime))
						{
							if (sp.GetValue(s, null) == null)
							{
								dp.SetValue(d, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), null);
							}
							else
							{
								dp.SetValue(d, Convert.ToDateTime(sp.GetValue(s, null)).ToString("yyyy-MM-dd HH:mm:ss"), null);
							}
						}
						else if (sp.PropertyType == typeof(System.Nullable<System.Int32>))
						{
							if (sp.GetValue(s, null) == null)
							{
								dp.SetValue(d, 0, null);
							}
							else
							{
								dp.SetValue(d, string.IsNullOrEmpty(sp.GetValue(s, null).ToString()) ? 0 : sp.GetValue(s, null), null);
							}
						}
						else
						{
							if (sp.GetValue(s, null) == null)
							{
								dp.SetValue(d, string.Empty, null);
							}
							else
							{
								dp.SetValue(d, sp.GetValue(s, null), null);
							}
						}
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
		return d;
	}
	#endregion

}
