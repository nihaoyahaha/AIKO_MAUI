using Aiko.Common.Models;
using Aiko.Common.Models.ImageView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.IServices.IServices;

public interface IImageViewService : IServiceBase
{
	//写真プレビューコレクションの初期化
	public void InitializePhotoPreviews();
	public void AddPhotoPreviewModel(PhotoPreviewModel dto);
	public ObservableCollection<PhotoPreviewModel> GetPhotoPreviewModels();

	//現在の確認項目の設定
	public void SetInspectionItem(InspectionItem inspectionItem);

	//プレビュー画像を削除
	public Task RemovePreviewPhotoAsync(PhotoPreviewModel previewModel);
}
