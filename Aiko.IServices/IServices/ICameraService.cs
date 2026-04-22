using Aiko.Common;
using Aiko.Common.Models;
using Aiko.SqliteDb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.IServices.IServices;

public interface ICameraService:IServiceBase
{
	/// <summary>
	/// 工程コード
	/// </summary>
	public string ProjectCode { get; }

	/// <summary>
	/// 確認項目コード
	/// </summary>
	public string InspectionItemCode { get;}

	/// <summary>
	/// db内の撮影方向集合
	/// </summary>
	public List<HM16SHDIR> HM16List { get; }

	//工程コードと確認項目コードの設定
	public void SetProjectCodeAndInspectionItemCode(string projectCode, string hm13004);
	public void SetHR01ITEM(HR01ITEM hr01);
	public void SetGreenBackgroundModel(GreenBackgroundModel model);

	//aのデータソースを取得
	public GreenBackgroundModel GetGreenBackgroundModel();

	//撮影方向を取得
	public Task<ObservableCollection<ListItem>> GetReinforcementTypesAsync();

	//写真パスの取得
	public string GetPhotoName();

	//現在の確認項目の設定
	public void SetInspectionItem(InspectionItem inspectionItem);

	//並び順を取得
	public Task<string> GetHr03005Async();

	//sqliteデータベースへの写真データの保存
	public Task SavePhotoDataToSqliteDbAsync(InspectionRecordItem photo);

	//ファイルの最終変更時刻の取得
	public string GetFileLastWriteTime(string filePath);

	//画像プレビューデータの追加
	public void AddImageViewPhotoPreviewModel(string filePath, byte[] bytes, PhotoLayer layer);

	//写真プレビューコレクションの初期化
	public void InitializePhotoPreviews();
}
