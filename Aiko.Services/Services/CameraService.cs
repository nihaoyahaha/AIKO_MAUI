using Aiko.Common;
using Aiko.Common.Models;
using Aiko.Common.Models.ImageView;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.Services.Services;

public class CameraService : BaseService<CameraService>, ICameraService
{
	public CameraService(ServiceContext<CameraService> context,
		IImageViewService imageViewService)
		: base(context)
	{
		_imageViewService = imageViewService;
	}

	/// <summary>
	/// aikoApp実行中のアプリケーションコンテキスト
	/// </summary>
	public AikoAppContext AppContext => AikoAppContext;

	private HR01ITEM _hr01;

	/// <summary>
	/// 工程コード
	/// </summary>
	private string _projectCode;

	/// <summary>
	/// 確認項目コード
	/// </summary>
	private string _hm13004;

	private List<HM16SHDIR> _hm16List = new();

	private GreenBackgroundModel _greenBackground;

	/// <summary>
	/// 現在の確認項目
	/// </summary>
	private InspectionItem _inspectionItem;

	private IImageViewService _imageViewService;

	/// <summary>
	/// 工程コード
	/// </summary>
	public string ProjectCode => _projectCode;

	/// <summary>
	/// /// <summary>
	/// 確認項目コード
	/// </summary>
	/// </summary>
	public string InspectionItemCode => _hm13004;

	public List<HM16SHDIR> HM16List => _hm16List;

	public void SetHR01ITEM(HR01ITEM hr01) => _hr01 = hr01;

	/// <summary>
	/// 工程コードと確認項目コードの設定
	/// </summary>
	/// <param name="projectCode">工程コード</param>
	/// <param name="hm13004">確認項目コード</param>
	public void SetProjectCodeAndInspectionItemCode(string projectCode, string hm13004) 
	{
		_projectCode = projectCode;
		_hm13004 = hm13004; 
	}

	/// <summary>
	/// 緑の背景版のデータソースを取得
	/// </summary>
	/// <returns></returns>
	public GreenBackgroundModel GetGreenBackgroundModel() => _greenBackground;

	/// <summary>
	/// 緑の背景版のデータソースの設定
	/// </summary>
	/// <param name="model"></param>
	public void SetGreenBackgroundModel(GreenBackgroundModel model) => _greenBackground = model;

	/// <summary>
	/// 撮影方向を取得
	/// </summary>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public async Task<ObservableCollection<ListItem>> GetReinforcementTypesAsync()
	{
		HM16SHDIR hm16DC = new HM16SHDIR();
		hm16DC.HM16001 = _hr01.HR01001;
		_hm16List = await HkksDb.GetHM16ListAsync(hm16DC);
		ObservableCollection<ListItem> listItems = new();
		foreach (var item in _hm16List)
		{
			ListItem listItem = new ListItem(item.HM16003.Trim(), item.HM16002.ToString());
			listItems.Add(listItem);
		}
		string prefDirsListStr = Preferences.Get("DirsList", "");
		List<string> prefDirsList = prefDirsListStr.Split(',').ToList();
		foreach (var item in prefDirsList)
		{
			if (!listItems.Any(dirs => dirs.DisplyName == item))
			{
				listItems.Add(new ListItem(item, item));
			}
		}
		return listItems;
	}

	/// <summary>
	/// 写真パスの取得
	/// </summary>
	/// <returns></returns>
	public string GetPhotoName() => $"{_hr01.HR01003}{_hm13004}{AikoAppContext.OperatorCD}{Guid.NewGuid().ToString("N")}";

	/// <summary>
	/// 現在の確認項目の写真集の設定
	/// </summary>
	/// <param name="inspectionRecordItems"></param>
	/// <exception cref="NotImplementedException"></exception>
	public void SetInspectionItem(InspectionItem inspectionItem)
	{
		_inspectionItem = inspectionItem;
		_imageViewService.SetInspectionItem(_inspectionItem);
	}

	/// <summary>
	/// 並び順を取得
	/// </summary>
	/// <returns></returns>
	public async Task<string> GetHr03005Async()
	{
		HR03SYAS hr03DC = new HR03SYAS();
		hr03DC.HR03001 = _hr01.HR01001;
		hr03DC.HR03003 = _hr01.HR01003;
		hr03DC.HR03004 = _hm13004;
		List<HR03SYAS> hr03List = await HkksDb.GetHR03PicAsync(hr03DC);
		return (hr03List.Count + 1).ToString().PadLeft(4, '0');
	}

	/// <summary>
	/// sqliteデータベースへの写真データの保存
	/// </summary>
	/// <param name="photo">写真</param>
	public async Task SavePhotoDataToSqliteDbAsync(InspectionRecordItem photo)
	{
		HR03SYAS hr03DC = new HR03SYAS();
		hr03DC.HR03001 = _hr01.HR01001;
		hr03DC.HR03003 = _hr01.HR01003;
		hr03DC.HR03004 = _hm13004;
		hr03DC.HR03002 = photo.HR03002;
		hr03DC.HR03005 = await GetHr03005Async();
		hr03DC.HR03006 = 0;
		hr03DC.HR03007 = photo.Direction;
		hr03DC.HR03008 = photo.Comment;
		hr03DC.HR03009 = Convert.ToInt32(DtToString(photo.CreateTime.Value, 1));
		hr03DC.HR03010 = Convert.ToInt32(DtToString(photo.CreateTime.Value, 2));
		hr03DC.HR03011 = photo.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
		hr03DC.HR03012 = AppContext.Name;
		hr03DC.HR03013 = photo.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
		hr03DC.HR03014 = AikoAppContext.Name;
		hr03DC.HR03015 = photo.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
		hr03DC.HR03016 = AppContext.Name;
		hr03DC.HR03017 = photo.HR03017;
		hr03DC.HR03018 = photo.HR03018;
		hr03DC.HR03019 = photo.DirectionText;
		hr03DC.HR03020 = GetFileLastWriteTime(photo.FilePath);
		List<HR03SYAS> hr03List = new List<HR03SYAS>();
		hr03List.Add(hr03DC);
		await HkksDb.UpdateHR02HR03Async(hr03List, 0);
		_inspectionItem.Num += 1;
		_inspectionItem.LocalPicNum += 1;
	}

	/// <summary>
	/// ファイルの最終変更時刻の取得
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns></returns>
	public string GetFileLastWriteTime(string filePath)
	{
		if (!File.Exists(filePath)) return "";
		var fileInfo = new FileInfo(filePath);
		DateTime lastWriteTime = fileInfo.LastWriteTime;
		return lastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
	}

	/// <summary>
	/// 画像プレビューデータの追加
	/// </summary>
	/// <param name="filePath"></param>
	public void AddImageViewPhotoPreviewModel(string filePath,byte[] bytes ,PhotoLayer layer)
	{
		_imageViewService.AddPhotoPreviewModel(
			new PhotoPreviewModel()
			{
#if WINDOWS
				ImageUrl = new Uri(filePath).AbsoluteUri,
#else
         		ImageUrl = filePath,
#endif
				JPgImageSource= ImageSource.FromStream(() => new MemoryStream(bytes)),
				HR03001 = _hr01.HR01001,
				HR03002 = Path.GetFileNameWithoutExtension(filePath),
				HR03003 = _hr01.HR01003,
				HR03004 = _hm13004,
				PhotoLayer = layer,
				HR03017 = Preferences.Default.Get("PhotoType", "JPEG") == "SVG" ? 1 : 0
			});
	}

	/// <summary>
	/// 写真プレビューコレクションの初期化
	/// </summary>
	public void InitializePhotoPreviews()
	{
		_imageViewService.InitializePhotoPreviews();
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


}
