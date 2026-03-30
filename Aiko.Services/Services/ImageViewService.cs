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

public class ImageViewService : BaseService<ImageViewService>, IImageViewService
{
	public ImageViewService(ServiceContext<ImageViewService> context) 
		: base(context)
	{
	}

	/// <summary>
	/// aikoApp実行中のアプリケーションコンテキスト
	/// </summary>
	public AikoAppContext AppContext => AikoAppContext;

	private ObservableCollection<PhotoPreviewModel> _photoPreviewModels = new ObservableCollection<PhotoPreviewModel>();

	private InspectionItem _inspectionItem;

	/// <summary>
	/// 写真プレビューコレクションの初期化
	/// </summary>
	public void InitializePhotoPreviews() => _photoPreviewModels = new();

	/// <summary>
	/// 写真プレビューコレクションを取得するには
	/// </summary>
	/// <returns></returns>
	public ObservableCollection<PhotoPreviewModel> GetPhotoPreviewModels() => _photoPreviewModels;

	/// <summary>
	/// 写真プレビューコレクション要素の追加
	/// </summary>
	/// <param name="model"></param>
	public void AddPhotoPreviewModel(PhotoPreviewModel model) => _photoPreviewModels.Add(model);

	/// <summary>
	/// 現在の確認項目の設定
	/// </summary>
	/// <param name="inspectionItem"></param>
	public void SetInspectionItem(InspectionItem inspectionItem) => _inspectionItem = inspectionItem;

	/// <summary>
	/// プレビュー画像を削除
	/// </summary>
	/// <param name="previewModel"></param>
	/// <exception cref="NotImplementedException"></exception>
	public async Task RemovePreviewPhotoAsync(PhotoPreviewModel previewModel)
	{
		List<HR03SYAS> hr03List = new List<HR03SYAS>();
		HR03SYAS hr03DC = new HR03SYAS();
		hr03DC.HR03001 = previewModel.HR03001;
		hr03DC.HR03002 = previewModel.HR03002;
		hr03DC.HR03003 = previewModel.HR03003;
		hr03DC.HR03004 = previewModel.HR03004;
		hr03List.Add(hr03DC);

		await HkksDb.UpdateHR02HR03Async(hr03List,1);
		_inspectionItem.Num -= 1;
		_inspectionItem.LocalPicNum -= 1;
	}
}
