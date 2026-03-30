using Aiko.Common;
using Aiko.Common.Models;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Aiko.Services.Services
{
	public class CheckPointService : BaseService<CheckPointService>, ICheckPointService
	{
		private readonly ICameraService _cameraService;
		public CheckPointService(ServiceContext<CheckPointService> context,
			ICameraService cameraService)
		: base(context)
		{
			_cameraService = cameraService;
		}

		#region プライベートフィールド
		private HR01ITEM _hr01;

		/// <summary>
		/// 部位コード
		/// </summary>
		private string _buimCode = "";

		/// <summary>
		/// 階コード
		/// </summary>
		private string _floorCode = "";

		/// <summary>
		/// 断面コード
		/// </summary>
		private string _danmCode = "";

		/// <summary>
		/// 工区コード
		/// </summary>
		private string _kokuCode = "";

		/// <summary>
		/// 断面リスト
		/// </summary>
		private List<HM10DANM> _hm10List = new List<HM10DANM>();

		/// <summary>
		/// 工程コードリスト
		/// </summary>
		private ObservableCollection<ListItem> _projectCodes = new();

		/// <summary>
		/// 工程コードごとにグループ化された確認項目リスト
		/// </summary>
		private Dictionary<string, ObservableCollection<InspectionItem>> _inspectionGroups = new();

		/// <summary>
		/// 工程コードごとにグループ化された確認項目リストのコピー,データが変更されたかどうかを判断するために使用される
		/// </summary>
		private Dictionary<string, ObservableCollection<InspectionItem>> _originalInspectionGroups = new();

		/// <summary>
		/// 選択した断面図
		/// </summary>
		private ImageSource _selectedDanmImageSource;
		#endregion

		#region ビジネスの実現方法

		/// <summary>
		/// aikoApp実行中のアプリケーションコンテキスト
		/// </summary>
		public AikoAppContext AppContext => AikoAppContext;

		/// <summary>
		/// 初期化工程コードごとにグループ化された確認項目リスト
		/// </summary>
		public void InitializeInspectionItem()
		{
			_inspectionGroups = new();
			_originalInspectionGroups = new();
		}

		public void SetHR01ITEM(HR01ITEM hr01) => _hr01 = hr01;

		/// <summary>
		/// 配筋確認名を取得
		/// </summary>
		/// <returns></returns>
		public string GetDanmTitle() => _hr01.HR01003.Trim();

		/// <summary>
		/// 断面図ボタンの数を取得するには
		/// </summary>
		/// <returns></returns>
		public async Task<List<HM10DANM>> GetRowLayOutItemsAsync()
		{
			string hr01019 = string.IsNullOrEmpty(_buimCode) ? "" : _buimCode;
			HM10DANM hm10 = new HM10DANM();
			hm10.HM10001 = _hr01.HR01001;
			hm10.HM10002 = "0001";
			hm10.HM10003 = string.IsNullOrEmpty(_danmCode) ? string.Empty : _danmCode;
			_hm10List = await HkksDb.GetHM10DANMList3Async(hm10, hr01019);
			return _hm10List;
		}

		/// <summary>
		/// 部位名を取得
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetBuimAsync()
		{
			HM06BUIM hm06DC = new HM06BUIM();
			hm06DC.HM06001 = _hr01.HR01001;
			List<HM06BUIM> hm06List = await HkksDb.GetHM06ListAsync(hm06DC);
			var item = hm06List.FirstOrDefault(x => x.HM06002 == _hr01.HR01019);
			if (item != null)
			{
				_buimCode = item.HM06002;
				return item.HM06003.Trim();
			}
			return "";
		}

		/// <summary>
		/// 階名を取得
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetFloorAsync()
		{
			HM08GRPM hm08 = new HM08GRPM();
			hm08.HM08001 = _hr01.HR01001;
			if (!string.IsNullOrEmpty(_buimCode))
			{
				hm08.HM08005 = _buimCode;
			}
			List<HM08GRPM> hm08List = await HkksDb.GetHM08GRPMListAsync(hm08);
			if (hm08List.Count > 0)
			{
				HM10DANM hm10 = new HM10DANM();
				hm10.HM10001 = _hr01.HR01001;
				List<HM10DANM> hm10List = await HkksDb.GetHM10DANMListAsync(hm10, _hr01.HR01020);
				if (hm10List.Count > 0)
				{
					var item = hm08List.FirstOrDefault(x => x.HM08002.Trim() == hm10List[0].HM10002);
					if (item != null)
					{
						_floorCode = item.HM08002;
						return item.HM08003.Trim();
					}
					else
					{
						_floorCode = hm08List[0].HM08002;
						return hm08List[0].HM08003.Trim();
					}
				}
			}
			return " ";
		}

		/// <summary>
		/// 断面名を取得
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetDanmAsync()
		{
			HM10DANM hm10 = new HM10DANM();
			hm10.HM10001 = _hr01.HR01001;
			if (!string.IsNullOrEmpty(_floorCode))
			{
				hm10.HM10002 = _floorCode;
			}
			List<HM10DANM> hm10List = await HkksDb.GetHM10DANMListAsync(hm10, null);
			if (!string.IsNullOrEmpty(_hr01.HR01020) && hm10List.Count > 1)
			{
				var item = hm10List.FirstOrDefault(x => x.HM10003 == _hr01.HR01020);
				if (item == null)
				{
					_danmCode = hm10List[0].HM10003;
					return hm10List[0].HM10004.Trim();
				}
				else
				{
					_danmCode = item.HM10003;
					return item.HM10004.Trim();
				}
			}
			return "";
		}

		/// <summary>
		/// 工区名を取得
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetKoKuAsync()
		{
			HM07KOKU hm07 = new HM07KOKU();
			hm07.HM07001 = _hr01.HR01001;
			hm07.HM07004 = _hr01.HR01002;
			List<HM07KOKU> hm07List = await HkksDb.GetHM07codeListAsync(hm07);
			if (!string.IsNullOrEmpty(_hr01.HR01007) && hm07List.Count > 1)
			{
				var item = hm07List.FirstOrDefault(x => x.HM07002 == _hr01.HR01007);
				if (item == null)
				{
					_kokuCode = hm07List[0].HM07002;
					return hm07List[0].HM07003.Trim();
				}
				else
				{
					_kokuCode = item.HM07002;
					return item.HM07003.Trim();
				}
			}
			return "";
		}

		/// <summary>
		/// 位置
		/// </summary>
		/// <returns></returns>
		public string GetLocation() => _hr01.HR01006.Trim();

		/// <summary>
		/// 工程を取得
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public async Task<ObservableCollection<ListItem>> GetProjectsAsync()
		{
			HM09PROC hm09DC = new HM09PROC();
			// 工事コード
			hm09DC.HM09001 = AikoAppContext.WorkCD;
			List<HM09PROC> liDataHm09 = await HkksDb.GetHM09List_HM13002Async(hm09DC, _buimCode);
			_projectCodes = new ObservableCollection<ListItem>();
			foreach (var item in liDataHm09)
			{
				ListItem listItem = new ListItem(item.HM09003, item.HM09002);
				_projectCodes.Add(listItem);
			}
			return _projectCodes;
		}

		/// <summary>
		/// 工程コードによる確認項目リストの取得
		/// </summary>
		/// <param name="projectCode">工程コード</param>
		/// <returns>確認項目リスト</returns>
		public async Task<ObservableCollection<InspectionItem>> GetInspectionItemsByProjectCodeAsync(string projectCode)
		{
			if (_inspectionGroups.Keys.Contains(projectCode)) return _inspectionGroups[projectCode];

			string key_inspectionGroups = projectCode;

			HM13KNKM hm13DC = new HM13KNKM();
			hm13DC.HM13001 = _hr01.HR01001;
			//工程コード
			if (!string.IsNullOrWhiteSpace(projectCode))
			{
				hm13DC.HM13003 = projectCode;
			}
			//部位コード
			if (!string.IsNullOrWhiteSpace(_buimCode))
			{
				hm13DC.HM13002 = _buimCode;
			}

			HR01ITEM r01 = new HR01ITEM();
			r01.HR01001 = _hr01.HR01001.Trim();
			r01.HR01002 = _hr01.HR01002.Trim();
			r01.HR01003 = _hr01.HR01003.Trim();
			r01.HR01007 = _hr01.HR01007.Trim();
			r01.HR01019 = _hr01.HR01019.Trim();
			r01.HR01020 = _hr01.HR01020.Trim();

			HR02KSKK hr02DC = new HR02KSKK();
			hr02DC.HR02001 = _hr01.HR01001;
			hr02DC.HR02002 = _hr01.HR01003;

			List<HR03TYPECOUNT> hr03CountList = await HkksDb.GetHR03TYPECOUNTAsync(r01);
			List<HM13KNKM> hm13List = await HkksDb.GetHM13ListAsync(hm13DC, _hr01, false);
			List<HR02KSKK> hr02List = await HkksDb.GetHR02ListAsync(hr02DC);
			ObservableCollection<InspectionItem> inspectionItems = new();
			ObservableCollection<InspectionItem> inspectionCloneItems = new();
			for (int i = 0; i < hm13List.Count; i++)
			{
				HM09PROC hm09DC = new HM09PROC();
				hm09DC.HM09001 = _hr01.HR01001;
				hm09DC.HM09002 = hm13List[i].HM13003;
				string strHm09 = _projectCodes.FirstOrDefault(x => x.Value == hm13List[i].HM13003).DisplyName;
				var result = await GetInspectionRecordItemsAsync(hm13List[i].HM13004.TrimEnd());
				int m_intPicCount = result.m_intPicCount;
				int _localPicCount = result._localPicCount;
				int m_intUsedPicCount = result.m_intUsedPicCount;
				List<HR02KSKK> resultHr02 = hr02List.Where(p => p.HR02003 == hm13List[i].HM13004).ToList();

				for (int j = 0; j < resultHr02.Count; j++)
				{
					string checkResultImage = GetCheckResultImage(resultHr02[j].HR02005);
					//確認日
					string strHR02006 = string.Empty;
					if (!(resultHr02[j].HR02006 == 0 || string.IsNullOrEmpty(resultHr02[j].HR02006.ToString())))
					{
						strHR02006 = ToDate(resultHr02[j].HR02006.ToString(), string.Empty, 1).ToString("yyyy/MM/dd");
					}
					//指摘日
					string strHR02008 = string.Empty;
					if (!(resultHr02[j].HR02008 == 0 || string.IsNullOrEmpty(resultHr02[j].HR02008.ToString())))
					{
						strHR02008 = ToDate(resultHr02[j].HR02008.ToString(), string.Empty, 1).ToString("yyyy/MM/dd");
					}
					InspectionItem inspectionItem = new InspectionItem();
					//確認項目名
					inspectionItem.ArtistName = hm13List[i].HM13005;
					//工程名
					inspectionItem.CompositionName = strHm09;
					//工程コード
					inspectionItem.Composition = hm13List[i].HM13003;
					//検査結果画像
					inspectionItem.CheckResultImage = checkResultImage;
					//結果コード
					inspectionItem.HR02005 = resultHr02[j].HR02005;
					//確認項目コード
					inspectionItem.HM13004 = hm13List[i].HM13004;
					//確認日
					inspectionItem.HR02006 = strHR02006;
					//確認者
					inspectionItem.HR02007 = resultHr02[j].HR02007;
					//写真枚数
					inspectionItem.Num = m_intPicCount;
					inspectionItem.LocalPicNum = _localPicCount;
					inspectionItem.UsedNum = m_intUsedPicCount;
					//値の入力が可能
					inspectionItem.HM13010 = hm13List[i].HM13010;
					//値
					inspectionItem.HR02004 = resultHr02[j].HR02004;
					//指摘日
					inspectionItem.HR02008 = strHR02008;
					//指摘者
					inspectionItem.HR02009 = resultHr02[j].HR02009;
					//説明
					inspectionItem.HM13012 = hm13List[i].HM13012;
					//メモ
					inspectionItem.HR02010 = resultHr02[j].HR02010;
					//方法
					inspectionItem.HR02011 = resultHr02[j].HR02011;
					//作成日時
					inspectionItem.HR02013 = resultHr02[j].HR02013;
					//作成オペレータ
					inspectionItem.HR02014 = resultHr02[j].HR02014;
					//更新日時
					inspectionItem.HR02015 = resultHr02[j].HR02015;
					//同期日時
					inspectionItem.HR02017 = resultHr02[j].HR02017;
					//同期オペレータ
					inspectionItem.HR02018 = resultHr02[j].HR02018;
					//確認方法
					inspectionItem.HR02019 = resultHr02[j].HR02019;
					//写真タイプ 0：不要　1：確認箇所ごと　2：工区・符号ごと　3：工区ごと
					inspectionItem.HM13009 = hm13List[i].HM13009;
					if (inspectionItem.HM13009 == 1)
					{
						inspectionItem.HasSyas = inspectionItem.UsedNum > 0;
					}
					if (inspectionItem.HM13009 == 2)
					{
						inspectionItem.HasSyas = inspectionItem.UsedNum > 0 ? true : hr03CountList.FirstOrDefault(P => P.STYPE == 2 && P.HM13004 == inspectionItem.HM13004).IDCOUNT > 0;
					}
					if (inspectionItem.HM13009 == 3)
					{
						inspectionItem.HasSyas = inspectionItem.UsedNum > 0 ? true : hr03CountList.FirstOrDefault(P => P.STYPE == 3 && P.HM13004 == inspectionItem.HM13004).IDCOUNT > 0;
					}
					if (inspectionItem.HM13009 == 4)
					{
						inspectionItem.HasSyas = inspectionItem.UsedNum > 0 ? true : hr03CountList.FirstOrDefault(P => P.STYPE == 4 && P.HM13004 == inspectionItem.HM13004).IDCOUNT > 0;
					}
					inspectionItem.SetCameraColor();
					inspectionItems.Add(inspectionItem);
					inspectionCloneItems.Add(inspectionItem.Clone());
				}
			}
			_inspectionGroups.Add(projectCode, inspectionItems);
			_originalInspectionGroups.Add(projectCode, inspectionCloneItems);
			return _inspectionGroups[projectCode];
		}

		/// <summary>
		/// 断面図を取得
		/// </summary>
		/// <param name="index">ボタンインデックス</param>
		/// <returns></returns>
		public async Task<ImageSource?> GetImageSourceAsync(int index)
		{
			if (_hm10List.Count == 0) return null;
			try
			{
				string fileDirectory = AikoAppContext.ConstructionSiteFolder;
				if (index >= 0)
				{
					return GetImageSourceFormDbOrAppDataDirectory(index, fileDirectory);
				}
				else
				{
					HM12FILE hm12 = new HM12FILE();
					hm12.HM12001 = _hr01.HR01001;
					hm12.HM12002 = _hm10List[index].HM10006;
					var hm12List = await HkksDb.GetHM12FILEAsync(hm12);
					if (hm12List.Count == 0) return null;

					string fileName = hm12List[0].HM12002.Trim();
					string hm12003 = hm12List[0].HM12003.Trim();
					fileName = $"{fileName}{hm12003.Substring(hm12003.LastIndexOf('.'))}";
					string filePath = Path.Combine(fileDirectory, fileName);
					if (!File.Exists(filePath))
					{
						await Shell.Current.DisplayAlertAsync("Error", ErrorMessage.ERRORPOP("CM01026"), "OK");
						return null;
					}
					else
					{
						return GetImageSourceFormCroppedBitmapStream(index, filePath);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.ToString());
				return null;
			}
		}

		/// <summary>
		/// 緑色背景板実体類の設定
		/// </summary>
		/// <param name="model"></param>
		public async Task SetGreenBackgroundModelAsync(GreenBackgroundModel model)
		{
			HM03PROJ hm03 = new HM03PROJ();
			hm03.HM03001 = _hr01.HR01001;
			List<HM03PROJ> list = await HkksDb.GetHM03PROJAsync(hm03);
			hm03 = list.Count > 0 ? list[0] : null;
			//工事名
			model.ConstructionName = hm03 != null ? hm03.HM03002.Trim() : "";
			//断面図
			model.SectionalDrawing = _selectedDanmImageSource;
			//施工者
			model.Constructor = hm03 == null ? "" : hm03.HM03003;
			string strDateTime = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.CurrentCulture);
			//現在の領域の取得
			CultureInfo culture = CultureInfo.CurrentCulture;
			string dayOfWeekName = culture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
			if (culture.TwoLetterISOLanguageName == "ja")
			{
				dayOfWeekName = dayOfWeekName.Substring(0, 1);
			}
			//撮影日
			model.ShootingDate = $"{strDateTime} ({dayOfWeekName})";
			model.FontSize = double.Parse(Preferences.Default.Get("BlackFontSize", "14"));
			_cameraService.SetGreenBackgroundModel(model);
		}

		public void SetHM13004ToCameraService(string projectCode)
		{
			_cameraService.SetHM13004(projectCode);
		}

		/// <summary>
		/// カメラ画面にジャンプ
		/// </summary>
		public async Task GotoCameraPageAsync()
		{
			_cameraService.SetHR01ITEM(_hr01);

			await Shell.Current.GoToAsync($"Camera");
		}

		/// <summary>
		/// 現在の確認項目の写真集をcameraServiceに渡す 
		/// </summary>
		/// <param name="inspectionRecordItems">現在の確認項目の写真集</param>
		public void SetInspectionItemToCameraService(InspectionItem inspectionItem)
		{
			_cameraService.SetInspectionItem(inspectionItem);
		}

		/// <summary>
		/// 指摘事項文字列を取得
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public async Task<string> GetHM11003sAsync()
		{
			HM11MEMOCHECK hm11DC = new HM11MEMOCHECK();
			hm11DC.HM11001 = _hr01.HR01001;
			List<HM11MEMOCHECK> hm11List = await HkksDb.GetHM11ListAsync(hm11DC);
			return string.Join(",", hm11List.Select(x => x.HM11003.Trim()));
		}

		/// <summary>
		/// 画像を破棄
		/// </summary>
		/// <param name="projectCode">確認項目コード</param>
		/// <param name="collectionOfPhotosTaken">削除された画像</param>
		/// <returns></returns>
		public async Task DiscardImageAsync(string projectCode, List<string> photoPathList)
		{
			List<HR03SYAS> hr03List = new List<HR03SYAS>();
			foreach (var path in photoPathList)
			{
				HR03SYAS hr03DC = new HR03SYAS();
				hr03DC.HR03001 = _hr01.HR01001;
				hr03DC.HR03002 = Path.GetFileNameWithoutExtension(path);
				hr03List.Add(hr03DC);
				if (File.Exists(path)) File.Delete(path);
			}
			await HkksDb.UpdateHR02HR03Async(hr03List, 1);
			photoPathList.Clear();
			List<HR03SYAS> hr03syas = await HkksDb.GetHR03SYASListAsync(new List<string> { _hr01.HR01001 });
			var allowedExtensions = new[] { ".jpg", ".svg", ".inkcanvas", ".canvas" };
			string photoFolderPath = Path.Combine(AikoAppContext.ConstructionSiteFolder, "photo");
			var files = Directory.GetFiles(photoFolderPath, "*", SearchOption.AllDirectories)
				.Where(path => allowedExtensions.Contains(Path.GetExtension(path).ToLowerInvariant()))
				.ToList();
			if (files.Count == 0) return;
			foreach (var file in files)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
				if (hr03syas.Count(x=>x.HR03002.Trim() == fileNameWithoutExtension) ==0)
				{
					File.Delete(file);
				}
			}
		}

		/// <summary>
		/// データの保存
		/// </summary>
		/// <returns></returns>
		public async Task<bool> SaveAsync()
		{
			bool isChanged = CompareData();
			if (!isChanged) return true;
			List<HR02KSKK> hr02List = new List<HR02KSKK>();
			List<HR04KSHIS> hr04List = new List<HR04KSHIS>();
			foreach (var item in _inspectionGroups)
			{
				foreach (var inspectionItem in item.Value)
				{
					hr02List.Add(BuildHR02KSKK(inspectionItem));
					hr04List.Add(await BuildHR04KSHISAsync(inspectionItem));
				}
			}
			return await HkksDb.UpdateTableAsync(hr02List, null, null, hr04List, null);
		}

		#endregion

		#region プライベート処理方法

		/// <summary>
		/// 判定結果を取得した画像
		/// </summary>
		/// <param name="hr02005">判定</param>
		/// <returns></returns>
		string GetCheckResultImage(int hr02005)
		{
			switch (hr02005)
			{
				case 0:
					return "checkrsl1.png";
				case 1:
					return "checkrsl2.png";
				case 2:
					return "checkrsl3.png";
				case 3:
					return "checkrsl4.png";
				case 4:
					return "checkrsl5.png";
				default:
					return "checkrsl1.png";
			}
		}

		/// <summary>
		/// 検査記録を取得
		/// </summary>
		/// <param name="hm13004"></param>
		/// <returns></returns>
		async Task<(
			int m_intPicCount,
			int _localPicCount,
			int m_intUsedPicCount)> GetInspectionRecordItemsAsync(string hm13004)
		{
			//枚数
			int m_intPicCount = 0;
			//ローカル画像数
			int _localPicCount = 0;
			//枚数
			int m_intUsedPicCount = 0;

			HR03SYAS HR03DC = new HR03SYAS();
			HR03DC.HR03001 = _hr01.HR01001;
			HR03DC.HR03003 = _hr01.HR01003;
			HR03DC.HR03004 = hm13004;

			List<HR03SYAS> DSHr03 = await HkksDb.GetHR03PicAsync(HR03DC);

			int userPicCount = DSHr03.Count(p => p.HR03006 == 2);

			m_intPicCount = DSHr03.Count == 0 ? DSHr03.Count(p => p.CHANGE != "DELE") : DSHr03.Count;
			m_intUsedPicCount = userPicCount == 0 ? DSHr03.Count(p => p.HR03006 == 2 && p.CHANGE != "DELE") : userPicCount;

			for (int iCnt = 0; iCnt < DSHr03.Count; iCnt++)
			{
				string strDestSrc = Path.Combine(AikoAppContext.ConstructionSiteFolder, "photo");
				string imageType = DSHr03[iCnt].HR03017 == 0 ? ".jpg" : ".svg";
				string strPicPath = Path.Combine(strDestSrc, $"{DSHr03[iCnt].HR03002}{imageType}");

				if (File.Exists(strPicPath))
				{
					_localPicCount++;
				}
			}

			return (m_intPicCount, _localPicCount, m_intUsedPicCount);
		}

		/// <summary>
		/// 文字列を日付に転化する。（yyyyMMdd）
		/// </summary>
		/// <param name="ckstring">日付</param>
		/// <param name="intType">日付タイプ</param>
		/// <returns></returns>
		DateTime ToDate(string ckstring, string ckstringd, int intType)
		{
			DateTime dt = new DateTime();
			if (intType == 1)
			{
				ckstring = ckstring.PadLeft(8, '0');
				int year = Convert.ToInt32(ckstring.Substring(0, 4));
				int month = Convert.ToInt32(ckstring.Substring(4, 2));
				int day = Convert.ToInt32(ckstring.Substring(6, 2));
				dt = new DateTime(year, month, day);
			}
			else if (intType == 2)
			{
				ckstring = ckstring.PadLeft(8, '0');
				int year = Convert.ToInt32(ckstring.Substring(0, 4));
				int month = Convert.ToInt32(ckstring.Substring(4, 2));
				int day = Convert.ToInt32(ckstring.Substring(6, 2));
				ckstringd = ckstringd.PadLeft(6, '0');
				int hour = Convert.ToInt32(ckstringd.Substring(0, 2));
				int minute = Convert.ToInt32(ckstringd.Substring(2, 2));
				int second = Convert.ToInt32(ckstringd.Substring(4, 2));
				dt = new DateTime(year, month, day, hour, minute, second);
			}
			return dt;
		}

		/// <summary>
		/// 日付を文字列に転化する。
		/// </summary>
		/// <param name="ckDate">日付</param>
		/// <param name="intType">文字列タイプ</param>
		/// <returns></returns>
		string DtToString(DateTime ckDate, int intType)
		{
			string strToString = string.Empty;
			if (intType == 1)
			{
				int year = ckDate.Year;
				int month = ckDate.Month;
				int day = ckDate.Day;
				strToString = string.Format("{0}{1}{2}",
					year.ToString().PadLeft(4, '0'), month.ToString().PadLeft(2, '0'), day.ToString().PadLeft(2, '0'));
			}
			else if (intType == 2)
			{
				int hour = ckDate.Hour;
				int minute = ckDate.Minute;
				int second = ckDate.Second;
				strToString = string.Format("{0}{1}{2}",
					hour.ToString().PadLeft(2, '0'), minute.ToString().PadLeft(2, '0'), second.ToString().PadLeft(2, '0'));
			}
			return strToString;
		}

		/// <summary>
		/// データベースbase 64文字列またはローカルフォルダから画像を取得する
		/// </summary>
		/// <param name="index">ボタンインデックス</param>
		/// <param name="fileDirectory">ファイルパス</param>
		/// <returns></returns>
		ImageSource? GetImageSourceFormDbOrAppDataDirectory(int index, string fileDirectory)
		{
			try
			{
				if (_hm10List[index].HM10023 != null && !string.IsNullOrWhiteSpace(_hm10List[index].HM10023.Trim()))
				{
					byte[] btyes = Convert.FromBase64String(_hm10List[index].HM10023.Trim());
					_selectedDanmImageSource = ImageSource.FromStream(() => new MemoryStream(btyes));
					return _selectedDanmImageSource;
				}
				else if (_hm10List[index].HM10025.Trim() != "")
				{
					string filePath = Path.Combine(fileDirectory, "danm", $"{_hm10List[index].HM10025.Trim()}.jpg");
					_selectedDanmImageSource = File.Exists(filePath) ? ImageSource.FromFile(filePath) : null;
					return _selectedDanmImageSource;
				}
				else
				{
					_selectedDanmImageSource = null;
					return null;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.ToString());
				_selectedDanmImageSource = null;
				return null;
			}

		}

		/// <summary>
		/// トリミングビットマップストリームから画像ソースを取得するには
		/// </summary>
		/// <returns></returns>
		ImageSource? GetImageSourceFormCroppedBitmapStream(int index, string filePath)
		{
			try
			{
				using Stream stream = File.OpenRead(filePath);
				SKBitmap originalBitmap = SKBitmap.Decode(stream);

				int rectWidth = Convert.ToInt32(_hm10List[index].HM10009);
				int rectHeight = Convert.ToInt32(_hm10List[index].HM10010);
				int rectX = _hm10List[index].HM10007 < 0 ? 0 : Convert.ToInt32(_hm10List[index].HM10007);
				int rectY = _hm10List[index].HM10008 < 0 ? 0 : Convert.ToInt32(_hm10List[index].HM10008);
				using var croppedBitmap = new SKBitmap(rectWidth, rectHeight);
				using (var canvas = new SKCanvas(croppedBitmap))
				{
					var srcRect = new SKRectI(rectX, rectY, rectX + rectWidth, rectY + rectHeight);
					var dstRect = new SKRectI(0, 0, rectWidth, rectHeight);
					//originalBitmapのsrcRect領域を、croppedBitmapのdstRect領域に描画する
					canvas.DrawBitmap(originalBitmap, srcRect, dstRect);
				}
				using var image = SKImage.FromBitmap(croppedBitmap);
				using var data = image.Encode(SKEncodedImageFormat.Jpeg, 95);
				byte[] imageData = data.ToArray();
				_selectedDanmImageSource = ImageSource.FromStream(() => new MemoryStream(imageData));
				return _selectedDanmImageSource;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.ToString());
				_selectedDanmImageSource = null;
				return null;
			}
		}

		/// <summary>
		/// データが変化したかどうかを比較する
		/// </summary>
		/// <returns></returns>
		bool CompareData()
		{
			foreach (var key in _inspectionGroups.Keys)
			{
				var currentList = _inspectionGroups[key];
				var originalList = _originalInspectionGroups[key];
				for (int i = 0; i < currentList.Count; i++)
				{
					if (currentList[i].CheckResultImage != originalList[i].CheckResultImage) return true;
					if (currentList[i].HR02005 != originalList[i].HR02005) return true;
					if (currentList[i].HM13004 != originalList[i].HM13004) return true;
					if (currentList[i].ArtistName != originalList[i].ArtistName) return true;
					if (currentList[i].Composition != originalList[i].Composition) return true;
					if (currentList[i].CompositionName != originalList[i].CompositionName) return true;
					if (currentList[i].ReleaseDateTime != originalList[i].ReleaseDateTime) return true;
					if (currentList[i].HR02006 != originalList[i].HR02006) return true;
					if (currentList[i].HR02007 != originalList[i].HR02007) return true;
					if (currentList[i].HR02004 != originalList[i].HR02004) return true;
					if (currentList[i].HM13011 != originalList[i].HM13011) return true;
					if (currentList[i].HM13010 != originalList[i].HM13010) return true;
					if (currentList[i].HR02008 != originalList[i].HR02008) return true;
					if (currentList[i].HR02009 != originalList[i].HR02009) return true;
					if (currentList[i].HR02010 != originalList[i].HR02010) return true;
					if (currentList[i].HM13012 != originalList[i].HM13012) return true;
					if (currentList[i].HR02011 != originalList[i].HR02011) return true;
					if (currentList[i].HR02019 != originalList[i].HR02019) return true;
					if (currentList[i].HM13009 != originalList[i].HM13009) return true;
					if (currentList[i].Num != originalList[i].Num) return true;
					if (currentList[i].LocalPicNum != originalList[i].LocalPicNum) return true;
					if (currentList[i].UsedNum != originalList[i].UsedNum) return true;
					if (currentList[i].HR02013 != originalList[i].HR02013) return true;
					if (currentList[i].HR02014 != originalList[i].HR02014) return true;
					if (currentList[i].HR02015 != originalList[i].HR02015) return true;
					if (currentList[i].HR02017 != originalList[i].HR02017) return true;
					if (currentList[i].HR02018 != originalList[i].HR02018) return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 検査結果を作成
		/// </summary>
		/// <param name="inspectionItem"></param>
		/// <returns></returns>
		HR02KSKK BuildHR02KSKK(InspectionItem inspectionItem)
		{
			HR02KSKK hr02 = new HR02KSKK();
			hr02.HR02001 = _hr01.HR01001;
			hr02.HR02002 = _hr01.HR01003;
			hr02.HR02003 = inspectionItem.HM13004.TrimEnd();
			hr02.HR02004 = inspectionItem.HR02004.TrimEnd();
			hr02.HR02005 = inspectionItem.HR02005;
			string strHr02006 = "0";
			if (!string.IsNullOrWhiteSpace(inspectionItem.HR02006) &&
				inspectionItem.HR02006.TrimEnd() != "0")
			{
				strHr02006 = DtToString(Convert.ToDateTime(inspectionItem.HR02006), 1).TrimEnd();
			}
			hr02.HR02006 = Convert.ToInt32(strHr02006);
			hr02.HR02007 = inspectionItem.HR02007.TrimEnd();

			string strHr02008 = "0";
			if (!string.IsNullOrWhiteSpace(inspectionItem.HR02008) &&
				inspectionItem.HR02008.TrimEnd() != "0")
			{
				strHr02008 = DtToString(Convert.ToDateTime(
					inspectionItem.HR02008.PadLeft(8, '0')), 1).TrimEnd();
			}
			hr02.HR02008 = Convert.ToInt32(strHr02008);
			hr02.HR02009 = inspectionItem.HR02009.TrimEnd();
			hr02.HR02010 = inspectionItem.HR02010.TrimEnd();
			hr02.HR02012 = 0;
			hr02.HR02011 = inspectionItem.HR02011.TrimEnd();
			hr02.HR02019 = inspectionItem.HR02019 == 0 ? 1 : inspectionItem.HR02019;
			hr02.HR02013 = string.IsNullOrEmpty(inspectionItem.HR02013.TrimEnd()) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : inspectionItem.HR02013.TrimEnd();
			hr02.HR02014 = string.IsNullOrEmpty(inspectionItem.HR02014.TrimEnd()) ? AikoAppContext.Name : inspectionItem.HR02014.TrimEnd();
			hr02.HR02015 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			hr02.HR02016 = AikoAppContext.Name;
			hr02.HR02017 = string.IsNullOrEmpty(inspectionItem.HR02017.TrimEnd()) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : inspectionItem.HR02017.TrimEnd();
			hr02.HR02018 = string.IsNullOrEmpty(inspectionItem.HR02018.TrimEnd()) ? AikoAppContext.Name : inspectionItem.HR02018.TrimEnd();
			return hr02;
		}

		/// <summary>
		/// 検査履歴を作成
		/// </summary>
		/// <param name="inspectionItem"></param>
		/// <returns></returns>
		async Task<HR04KSHIS> BuildHR04KSHISAsync(InspectionItem inspectionItem)
		{
			HR04KSHIS hr04 = new HR04KSHIS();
			string strHM13004 = inspectionItem.HM13004.TrimEnd();
			hr04.HR04001 = _hr01.HR01001;
			hr04.HR04002 = _hr01.HR01003;
			hr04.HR04003 = strHM13004;
			int dsMaxCode = await HkksDb.GetMaxHR04004Async(hr04);
			hr04.HR04004 = dsMaxCode + 1;
			hr04.HR04005 = inspectionItem.HR02004.TrimEnd();
			hr04.HR04006 = inspectionItem.HR02005;
			string strHR02006 = "0";
			if (!string.IsNullOrEmpty(inspectionItem.HR02006) &&
					inspectionItem.HR02006.TrimEnd() != "0")
			{
				strHR02006 = DtToString(Convert.ToDateTime(
					inspectionItem.HR02006.PadLeft(8, '0')), 1).TrimEnd();
			}
			hr04.HR04007 = Convert.ToInt32(strHR02006);
			hr04.HR04008 = inspectionItem.HR02007.TrimEnd();
			string strHR02008 = "0";
			if (!string.IsNullOrEmpty(inspectionItem.HR02008) &&
					inspectionItem.HR02008.TrimEnd() != "0")
			{
				strHR02008 = DtToString(Convert.ToDateTime(
					inspectionItem.HR02008.PadLeft(8, '0')), 1).TrimEnd();
			}
			hr04.HR04009 = Convert.ToInt32(strHR02008);
			hr04.HR04010 = inspectionItem.HR02009.TrimEnd();
			hr04.HR04011 = inspectionItem.HR02010.TrimEnd();
			hr04.HR04012 = inspectionItem.HR02011.TrimEnd();
			hr04.HR04013 = inspectionItem.Num;
			hr04.HR04014 = inspectionItem.HR02019 == 0 ? 1 : inspectionItem.HR02019;
			hr04.HR04015 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			hr04.HR04016 = AikoAppContext.Name;
			return hr04;
		}

		#endregion


	}
}
