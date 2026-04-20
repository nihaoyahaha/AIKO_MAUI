using Aiko.Common.Models.CheckPoint;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiko.SqliteDb;
using Aiko.Common;
using Aiko.Common.Models;

namespace Aiko.IServices.IServices;

public interface ICheckPointService: IServiceBase
{
	//初期化工程コードごとにグループ化された確認項目リスト
	public void InitializeInspectionItem();
	public void SetHR01ITEM(HR01ITEM hr01);

	//配筋確認名を取得
	public string GetDanmTitle();

	//断面図ボタンの数を取得するには
	public Task<List<HM10DANM>> GetRowLayOutItemsAsync();
	
	//部位名を取得
	public Task<string> GetBuimAsync();
	
	//階名を取得
	public Task<string> GetFloorAsync();

	//断面名を取得
	public Task<string> GetDanmAsync();

	//工区名を取得
	public Task<string> GetKoKuAsync();

	//位置
	public string GetLocation();

	//工程を取得
	public Task<ObservableCollection<ListItem>> GetProjectsAsync();

	//工程コードによる確認項目リストの取得
	public Task<ObservableCollection<InspectionItem>> GetInspectionItemsByProjectCodeAsync(string projectCode);

	//断面図を取得
	public Task<ImageSource?> GetImageSourceAsync(int index);

	//カメラ画面にジャンプ
	public Task GotoCameraPageAsync();

	//緑色背景板実体類の設定
	public Task SetGreenBackgroundModelAsync(GreenBackgroundModel model);

	//確認項目コードの設定
	public void SetHM13004ToCameraService(string projectCode);

	// 現在の確認項目をcameraServiceに渡す 
	public void SetInspectionItemToCameraService(InspectionItem inspectionItem);

	//指摘事項文字列を取得
	public Task<string> GetHM11003sAsync();

	//画像を破棄
	public Task DiscardImageAsync(string projectCode, List<string> photoPathList);

	//データの保存
	public Task<bool> SaveAsync();

	//データの直接保存
	public Task<bool> SaveDirectlyAsync();

	//データが変更されたかどうか
	public bool IsDataChanged();
}
