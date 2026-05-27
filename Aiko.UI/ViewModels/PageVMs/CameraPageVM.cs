using Aiko.Common;
using Aiko.Common.Models;
using Aiko.Common.Models.ImageView;
using Aiko.IServices.IServices;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using Path = System.IO.Path;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class CameraPageVM : Observablebase<CameraPageVM, ICameraService>
{
	readonly ICameraProvider _cameraProvider;
	public CameraPageVM(ICameraProvider cameraProvider,
		ILogger<CameraPageVM> logger,
		ICameraService service) : base(logger, service)
	{
		_cameraProvider = cameraProvider;
	}

	#region 観察可能属性
	/// <summary>
	/// ありボタンの有効状態
	/// </summary>
	[ObservableProperty]
	public partial bool BgRectTypeRadioButtonIsEnable { get; set; } = true;

	/// <summary>
	/// 緑の背景版の注釈テキスト
	/// </summary>
	[ObservableProperty]
	public partial string Remark { get; set; } = string.Empty;

	/// <summary>
	/// 緑の背景版のデータソース
	/// </summary>
	[ObservableProperty]
	public partial GreenBackgroundModel GreenBackgroundModel { get; set; }

	/// <summary>
	/// 緑の背景版の表示隠し
	/// </summary>
	[ObservableProperty]
	public partial bool BlackboardDisplay { get; set; } = true;

	/// <summary>
	/// カメラのズーム率
	/// </summary>
	[ObservableProperty]
	public partial float CurrentZoom { get; set; }

	/// <summary>
	/// 写真のスクリーンショット
	/// </summary>
	[ObservableProperty]
	public partial ImageSource PhotoScreenshot { get; set; }

	/// <summary>
	/// UIの緑の背景版
	/// </summary>
	[ObservableProperty]
	public partial Grid RectGrid { get; set; }

	/// <summary>
	/// グリッド
	/// </summary>
	[ObservableProperty]
	public partial Grid GridLayer { get; set; }

	/// <summary>
	/// 撮影されたフォトパス
	/// </summary>
	[ObservableProperty]
	public partial string PhotoScreenshotPath { get; set; } = string.Empty;

	/// <summary>
	/// 緑の背景版の容器の表示状態
	/// </summary>
	[ObservableProperty]
	public partial bool BackgroundRectHsStackIsVisible { get; set; } = false;

	/// <summary>
	/// 解像度の容器の表示状態
	/// </summary>
	[ObservableProperty]
	public partial bool ResolutionHsStackIsVisible { get; set; } = false;

	/// <summary>
	/// カメラ切り替えの容器の表示状態
	/// </summary>
	[ObservableProperty]
	public partial bool CameraPositionHsStackIsVisible { get; set; } = false;

	/// <summary>
	/// 緑の背景版の断面図の表示非表示
	/// </summary>
	[ObservableProperty]
	public partial bool GreenBackgroundImageIsVisible { get; set; } = true;

	/// <summary>
	/// 撮影方向
	/// </summary>
	[ObservableProperty]
	public partial ObservableCollection<ListItem> ReinforcementTypeList { get; set; } = new();

	/// <summary>
	/// 撮影方向の選択項目
	/// </summary>
	[ObservableProperty]
	public partial ListItem ReinforcementTypeSelectedItem { get; set; } = null;

	/// <summary>
	/// カメラ機器に関する情報を示します。
	/// </summary>
	[ObservableProperty]
	public partial CameraInfo? SelectedCamera { get; set; }

	/// <summary>
	/// 現在選択されているカメラの解像度
	/// </summary>
	[ObservableProperty]
	public partial Size SelectedResolution { get; set; }

	[ObservableProperty]
	public partial bool IsRearCamera { get; set; }

	/// <summary>
	/// グリッドの表示
	/// </summary>
	[ObservableProperty]
	public partial bool GridVisible { get; set; } = false;

	/// <summary>
	/// 撮影方向の文字
	/// </summary>
	[ObservableProperty]
	public partial string ReinforcementTypeText { get; set; } = "";
	#endregion

	#region プライベートフィールド

	/// <summary>
	/// cameraview uiサイズ
	/// </summary>
	private Rect CameraViewBoundRect;

	/// <summary>
	/// 前のページの名前
	/// </summary>
	private string _fromPage = "";
	#endregion

	#region 公開フィールド

	/// <summary>
	/// cameraview黒辺除去の可視化寸法
	/// </summary>
	public Rect CameraViewActualVideoRect;

	/// <summary>
	/// 緑の背景版のオフセット値
	/// </summary>
	public double panX, panY;

	public CancellationToken Token => CancellationToken.None;

	/// <summary>
	/// カメラの最大ズーム倍率
	/// </summary>
	public float DisplayMaxZoom
	{
		get
		{
#if WINDOWS
		  return Math.Min(50.0f, SelectedCamera?.MaximumZoomFactor ?? 50.0f);
#else
		  return Math.Min(5.0f, SelectedCamera?.MaximumZoomFactor ?? 5.0f);
#endif
		}
	}

	/// <summary>
	/// 緑の背景版の幅
	/// </summary>
	public double EditViewWidth;
	#endregion

	/// <summary>
	/// 前の画面で渡されたパラメータの処理
	/// </summary>
	/// <param name="query"></param>
	public override void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.Keys.Contains("FromPage"))
		{
			_fromPage = query["FromPage"].ToString();
		}
	}

	#region コマンドハンドラ 

	async partial void OnIsRearCameraChanged(bool value)
	{
		await RefreshCameras(CancellationToken.None);
		if (value)
		{
			SelectedCamera = _cameraProvider?.AvailableCameras?
				.FirstOrDefault(x => x.Position == CameraPosition.Rear);
		}
		else
		{
			SelectedCamera = _cameraProvider?.AvailableCameras?
				.FirstOrDefault(x => x.Position == CameraPosition.Front);
		}
		SaveCameraResolutionDisplayList();
	}

	/// <summary>
	/// 画面ロード処理
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	async private Task PageLoaded()
	{
		Remark = "";
		if (_fromPage == "CheckPointPage") PhotoScreenshot = null;
		await RefreshCameras(CancellationToken.None);
		GreenBackgroundModel = Service.GetGreenBackgroundModel();
		//断面図の表示
		GreenBackgroundModel.IsSectionalDrawingVisible = GreenBackgroundImageIsVisible;
		//撮影方向
		ReinforcementTypeList = await Service.GetReinforcementTypesAsync();
		if (ReinforcementTypeList.Count > 0)
		{
			ReinforcementTypeSelectedItem = ReinforcementTypeList[0];
		}
		InitSelectedCamera();
		SaveCameraResolutionDisplayList();
#if WINDOWS
		UpdateVideoDisplayLayoutForWindows();
#else
		UpdateVideoDisplayLayoutForIOS();
#endif
	}

	/// <summary>
	/// カメラ画面サイズ補正処理
	/// </summary>
	/// <param name="sender"></param>
	[RelayCommand]
	private void CameraViewSizeChanged(object sender)
	{
		var camera = sender as CameraView;
		if (camera == null || camera.Width <= 0) return;
#if WINDOWS
		CameraViewBoundRect = camera.Bounds;
		UpdateVideoDisplayLayoutForWindows();
#else
		CameraViewActualVideoRect = camera.Bounds;
		UpdateVideoDisplayLayoutForIOS();
#endif
	}

	/// <summary>
	/// 緑の背景版の表示非表示の設定
	/// </summary>
	[RelayCommand]
	private void SetBlackboardDisplay()
	{
		BlackboardDisplay = !BlackboardDisplay;
		BgRectTypeRadioButtonIsEnable = !BgRectTypeRadioButtonIsEnable;
	}

	/// <summary>
	/// 確認者チェックボックスの切り替え
	/// </summary>
	/// <param name="e"></param>
	[RelayCommand]
	private void ChangeRemarkOrConfirmer(EventArgs e)
	{
		var value = ((CheckedChangedEventArgs)e).Value;
		GreenBackgroundModel.Describe = value ? "確認者" : "備考";
		if (value)
		{
			GreenBackgroundModel.Describe = "確認者";
			GreenBackgroundModel.DescribeDetail = GreenBackgroundModel.Confirmer;
		}
		else
		{
			GreenBackgroundModel.Describe = "備考";
			GreenBackgroundModel.DescribeDetail = Remark;
		}
	}

	/// <summary>
	/// 緑の背景版の断面図の表示非表示を設定
	/// </summary>
	/// <param name="e"></param>
	[RelayCommand]
	private void IsSectionalDrawingVisibleChanged(EventArgs e)
	{
		GreenBackgroundImageIsVisible = ((CheckedChangedEventArgs)e).Value;
		GreenBackgroundModel.IsSectionalDrawingVisible = GreenBackgroundImageIsVisible;
	}

	/// <summary>
	/// カメラの切り替え
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	[RelayCommand]
	private async Task ChangeCamera(EventArgs e)
	{
		var value = ((CheckedChangedEventArgs)e).Value;
		await RefreshCameras(CancellationToken.None);
		if (value)
		{
			SelectedCamera = _cameraProvider?.AvailableCameras?
				.FirstOrDefault(x => x.Position == CameraPosition.Rear);
		}
		else
		{
			SelectedCamera = _cameraProvider?.AvailableCameras?
				.FirstOrDefault(x => x.Position == CameraPosition.Front);
		}
		SaveCameraResolutionDisplayList();
	}

	/// <summary>
	/// 画像プレビュー画面にジャンプ
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task GoToImageViewPage()
	{
		try
		{
			await Shell.Current.GoToAsync($"ImageView");
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 後方へ移動
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	public async Task Back()
	{
		Service.InitializePhotoPreviews();
		await Shell.Current.GoToAsync("..", CreateNavigationParameterForCheckPoint());
	}

	/// <summary>
	/// 備考のテキスト変更処理
	/// </summary>
	[RelayCommand]
	private void RemarkTextChanged()
	{
		if (GreenBackgroundModel == null) return;
		GreenBackgroundModel.Remark = Remark;
		if (GreenBackgroundModel.Describe == "備考")
		{
			GreenBackgroundModel.DescribeDetail = Remark;
		}
	}

	/// <summary>
	/// 黒のクリックコマンド
	/// </summary>
	[RelayCommand]
	private void BackgroundRectClick()
	{
		BackgroundRectHsStackIsVisible = !BackgroundRectHsStackIsVisible;
	}

	/// <summary>
	/// 画のクリックコマンド
	/// </summary>
	[RelayCommand]
	private void ResolutionClick()
	{
		ResolutionHsStackIsVisible = !ResolutionHsStackIsVisible;
	}

	/// <summary>
	/// 替のクリックコマンド
	/// </summary>
	[RelayCommand]
	private void CameraPositionClick()
	{
		CameraPositionHsStackIsVisible = !CameraPositionHsStackIsVisible;
	}

	/// <summary>
	/// 写真を撮る
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	[RelayCommand]
	private async Task MediaCaptured(MediaCapturedEventArgs e)
	{
		string photoType = Preferences.Default.Get("PhotoType", "JPEG");
		try
		{
			if (e.Media == null) return;
			if (!await ValidateBeforeSavingAsync()) return;
			UpdateDirsListPreferences();
			using MemoryStream stream = new();
			await e.Media.CopyToAsync(stream);
			stream.Position = 0;
			if (stream == null) return;

			var tuple = await GetPhotoAndOverlayUIAsync(stream);

			if (photoType == "JPEG")
			{
				SaveJpg(tuple.PhotoBitmap, tuple.OverlayUIBitmap, tuple.OverlayRect);
			}
			else
			{
				await SaveSvgAsync(tuple.PhotoBitmap, tuple.OverlayUIBitmap, tuple.OverlayRect);
			}
			InspectionRecordItem inspectionRecordItem = CreateInspectionRecordItem(photoType);
			await Service.SavePhotoDataToSqliteDbAsync(inspectionRecordItem);
			SendTakePhotoMessage();
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// グリッドの表示切り替え
	/// </summary>
	/// <param name="e"></param>
	[RelayCommand]
	private void ChangeGridIsVisible(EventArgs e)
	{
		var value = ((CheckedChangedEventArgs)e).Value;
		GridVisible = value;
	}

	/// <summary>
	/// 撮影方向の選択変更イベント処理
	/// </summary>
	[RelayCommand]
	private void ReinforcementTypeSelectedIndexChanged()
	{
		ReinforcementTypeText = ReinforcementTypeSelectedItem.DisplyName;
	}

	[RelayCommand]
	private void ExpandReinforcementTypeList(Picker picker)
	{
		if (picker != null)
		{
			picker.Focus();
#if WINDOWS
			var nativePicker = (Microsoft.UI.Xaml.Controls.ComboBox)picker.Handler.PlatformView;
			nativePicker.IsDropDownOpen = true;
#endif
		}
	}

	#endregion

	#region 処理方法

	/// <summary>
	/// 写真、緑の背景版、緑の背景版座標を取得
	/// </summary>
	/// <param name="stream"></param>
	/// <returns></returns>
	async Task<(SKBitmap PhotoBitmap, SKBitmap OverlayUIBitmap, Rect OverlayRect)> GetPhotoAndOverlayUIAsync(Stream stream)
	{
		//解码原始大图
		SKBitmap originalBitmap = SKBitmap.Decode(stream);
		//裁剪成预览比例
		var cropRect = CalculatePreviewCropRect(originalBitmap.Width, originalBitmap.Height);
		var photoBitmap = new SKBitmap(cropRect.Width, cropRect.Height);
		using (var canvas = new SKCanvas(photoBitmap))
		{
			var src = new SKRect(cropRect.Left, cropRect.Top, cropRect.Right, cropRect.Bottom);
			var dest = new SKRect(0, 0, photoBitmap.Width, photoBitmap.Height);
			canvas.DrawBitmap(originalBitmap, src, dest);
		}
		//绿板屏幕截图
		var overlayUI = await GetOverlayUI();
		//绿板所在位置
		var overlayRect = CalculateRedBoxInPhoto(photoBitmap.Width, photoBitmap.Height);
		return (photoBitmap, overlayUI, overlayRect);
	}

	/// <summary>
	/// jpgピクチャを生成する
	/// </summary>
	/// <param name="photoBitmap"></param>
	/// <param name="overlayUIBitmap"></param>
	/// <param name="overlayRect"></param>
	void SaveJpg(SKBitmap photoBitmap, SKBitmap overlayUIBitmap, Rect overlayRect)
	{
		PhotoLayer photoLayer = new();
		try
		{
			using var compositeBitmap = new SKBitmap(photoBitmap.Width, photoBitmap.Height);
			using (var canvas = new SKCanvas(compositeBitmap))
			{
				// 底面写真を描く
				canvas.DrawBitmap(photoBitmap, 0, 0);
				// 緑の背景版を描く
				var destRect = new SKRect(
						(float)overlayRect.X,
						(float)overlayRect.Y,
						(float)(overlayRect.X + overlayRect.Width),
						(float)(overlayRect.Y + overlayRect.Height));
				canvas.DrawBitmap(overlayUIBitmap, destRect);
			}
			string imageQuality = Preferences.Default.Get("ImageQuality_JPEG", "100");
			using (var data = compositeBitmap.Encode(SKEncodedImageFormat.Jpeg, int.Parse(imageQuality)))
			{
				if (data == null) throw new Exception("スキャップ画像のエンコードに失敗しました");
				byte[] imageBytes = data.ToArray();
				string fileName = $"{Service.GetPhotoName()}.jpg";
				PhotoScreenshotPath = PrepareImagePath(fileName);
				File.WriteAllBytes(PhotoScreenshotPath, imageBytes);
				PhotoScreenshot = ImageSource.FromStream(() => new MemoryStream(imageBytes));
				photoLayer.ImageUri = PhotoScreenshotPath;
				photoLayer.PhotoBmp = ImageSource.FromStream(() => new MemoryStream(imageBytes));
				photoLayer.PhotoWidth = photoBitmap.Width.ToString();
				photoLayer.PhotoHeight = photoBitmap.Height.ToString();
				photoLayer.PhotoIsChecked = true;
				AddImageViewPhotoPreviewDto(PhotoScreenshotPath, imageBytes, photoLayer);
			}
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
		finally
		{
			photoBitmap?.Dispose();
			overlayUIBitmap?.Dispose();
		}
	}

	/// <summary>
	/// 画像保存パスを準備します。ディレクトリが存在しない場合は、自動的に作成されます。
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns></returns>
	string PrepareImagePath(string fileName)
	{
		string directoryPath = Path.Combine(Service.AppContext.ConstructionSiteFolder, "photo");
		if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
		return Path.Combine(directoryPath, fileName);
	}

	/// <summary>
	/// svgピクチャを生成する
	/// </summary>
	/// <param name="photoBitmap"></param>
	/// <param name="overlayUIBitmap"></param>
	/// <param name="overlayRect"></param>
	/// <returns></returns>
	async Task SaveSvgAsync(SKBitmap photoBitmap, SKBitmap overlayUIBitmap, Rect overlayRect)
	{
		PhotoLayer photoLayer = new PhotoLayer();
		try
		{
			string imageQuality = Preferences.Default.Get("ImageQuality_SVG", "80");
			using var photoStream = new MemoryStream();
			photoBitmap.Encode(photoStream, SKEncodedImageFormat.Jpeg, int.Parse(imageQuality));
			string photoBase64 = Convert.ToBase64String(photoStream.ToArray());

			var strBuild = new System.Text.StringBuilder();
			strBuild.AppendLine($@"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" version=""1.1"" width=""{photoBitmap.Width}"" height=""{photoBitmap.Height}"" viewBox=""0 0 {photoBitmap.Width} {photoBitmap.Height}"">");
			strBuild.AppendLine($@"<image id=""photo"" width=""{photoBitmap.Width}"" height=""{photoBitmap.Height}"" x=""0"" y=""0"" opacity=""1"" xlink:href=""data:image/png;base64,{photoBase64}"" />");
			
			if (BlackboardDisplay)
			{
				using var overlayUIStream = new MemoryStream();
				overlayUIBitmap.Encode(overlayUIStream, SKEncodedImageFormat.Jpeg, int.Parse(imageQuality));
				string overlayUIBase64 = Convert.ToBase64String(overlayUIStream.ToArray());

				strBuild.AppendLine($@"<image id=""blackboard"" width=""{overlayRect.Width}"" height=""{overlayRect.Height}"" x=""{overlayRect.X}"" y=""{overlayRect.Y}"" opacity=""1"" xlink:href=""data:image/png;base64,{overlayUIBase64}"" />");

				photoLayer.GreenBackgroundIsChecked = true;
				photoLayer.GreenBmp = ImageSource.FromStream(() => new MemoryStream(overlayUIStream.ToArray()));
				photoLayer.GreenWidth = overlayRect.Width.ToString();
				photoLayer.GreenHeight = overlayRect.Height.ToString();
				photoLayer.Margin = new Thickness(Convert.ToDouble(overlayRect.X), Convert.ToDouble(overlayRect.Y), 0, 0);
			}
			strBuild.AppendLine("</svg>");

			string fileName = $"{Service.GetPhotoName()}.svg";
			PhotoScreenshotPath = PrepareImagePath(fileName);

			await File.WriteAllTextAsync(PhotoScreenshotPath, strBuild.ToString());
			PhotoScreenshot = ImageSource.FromFile(PhotoScreenshotPath);

			photoLayer.ImageUri = PhotoScreenshotPath;
			photoLayer.GreenBackgroundIsVisible = BlackboardDisplay;
			photoLayer.PhotoIsChecked = true;
			photoLayer.PhotoBmp = ImageSource.FromStream(() => new MemoryStream(photoStream.ToArray()));
			photoLayer.PhotoWidth = photoBitmap.Width.ToString();
			photoLayer.PhotoHeight = photoBitmap.Height.ToString();

			byte[] bytes = await File.ReadAllBytesAsync(PhotoScreenshotPath);
			AddImageViewPhotoPreviewDto(PhotoScreenshotPath, bytes, photoLayer);
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
		finally
		{
			photoBitmap?.Dispose();
			overlayUIBitmap?.Dispose();
		}
	}

	/// <summary>
	/// 緑の背景版のUIスクリーンショットを取得する
	/// </summary>
	/// <returns></returns>
	async Task<SKBitmap> GetOverlayUI()
	{
		try
		{
			IScreenshotResult? screen = await RectGrid.CaptureAsync();
			using Stream rectGridStream = await screen.OpenReadAsync();
			return SKBitmap.Decode(rectGridStream);
		}
		catch (Exception ex)
		{
			Logger.LogError($"CameraPageVM:{ex.ToString()}");
			return null;
		}
	}

	/// <summary>
	/// グリーンボードの位置を計算する
	/// </summary>
	/// <param name="photoWidth"></param>
	/// <param name="photoHeight"></param>
	/// <returns></returns>
	Rect CalculateRedBoxInPhoto(int photoWidth, int photoHeight)
	{
		double relativeX = RectGrid.TranslationX - CameraViewActualVideoRect.X;
		double relativeY = RectGrid.TranslationY - CameraViewActualVideoRect.Y;
		double scaleX = photoWidth / CameraViewActualVideoRect.Width;
		double scaleY = photoHeight / CameraViewActualVideoRect.Height;
		double photoX = Math.Max(0, relativeX * scaleX);
		double photoY = Math.Max(0, relativeY * scaleY);
		double photoW = RectGrid.Width * scaleX;
		double photoH = RectGrid.Height * scaleY;
		photoX = Math.Min(photoX, photoWidth - photoW);
		photoY = Math.Min(photoY, photoHeight - photoH);
		return new Rect(photoX, photoY, photoW, photoH);
	}

	/// <summary>
	/// トリミングが必要な矩形範囲の計算
	/// </summary>
	/// <param name="photoWidth"></param>
	/// <param name="photoHeight"></param>
	/// <returns></returns>
	SKRectI CalculatePreviewCropRect(int photoWidth, int photoHeight)
	{
#if WINDOWS
		double PreviewRatio = 16.0 / 9.0;
#else
		double PreviewRatio = CameraViewActualVideoRect.Width / CameraViewActualVideoRect.Height;
#endif
		double photoRatio = (double)photoWidth / photoHeight;
		int cropX, cropY, cropW, cropH;

		if (photoRatio > PreviewRatio)
		{
			cropH = photoHeight;
			cropW = (int)(photoHeight * PreviewRatio);
			cropX = (photoWidth - cropW) / 2;
			cropY = 0;
		}
		else
		{
			cropW = photoWidth;
			cropH = (int)(photoWidth / PreviewRatio);
			cropX = 0;
			cropY = (photoHeight - cropH) / 2;
		}

		return new SKRectI(cropX, cropY, cropX + cropW, cropY + cropH);
	}

	/// <summary>
	/// カメラの更新
	/// </summary>
	/// <param name="token"></param>
	/// <returns></returns>
	async Task RefreshCameras(CancellationToken token) => await _cameraProvider.RefreshAvailableCameras(token);

	/// <summary>
	/// 画像プレビューデータの追加
	/// </summary>
	/// <param name="path"></param>
	void AddImageViewPhotoPreviewDto(string path, byte[] bytes, PhotoLayer layer)
	{
		Service.AddImageViewPhotoPreviewModel(path, bytes, layer);
	}

	/// <summary>
	/// windowsプラットフォームのビデオ表示領域を計算する
	/// </summary>
	void UpdateVideoDisplayLayoutForWindows()
	{
		//摄像头分辨率
		double previewRatio = 16.0 / 9.0;

		if (CameraViewBoundRect.Width <= 0 || CameraViewBoundRect.Height <= 0) return;

		double screenAspectRatio = CameraViewBoundRect.Width / CameraViewBoundRect.Height;
		double previewWidth;
		double previewHeight;
		double offsetX = 0;
		double offsetY = 0;

		if (screenAspectRatio > previewRatio)
		{
			// 左右黑边
			previewHeight = CameraViewBoundRect.Height;
			previewWidth = previewHeight * previewRatio;
			offsetX = CameraViewBoundRect.X + (CameraViewBoundRect.Width - previewWidth) / 2.0;
			offsetY = CameraViewBoundRect.Y;
		}
		else
		{
			// 上下黑边
			previewWidth = CameraViewBoundRect.Width;
			previewHeight = previewWidth / previewRatio;
			offsetX = CameraViewBoundRect.X;
			offsetY = CameraViewBoundRect.Y + (CameraViewBoundRect.Height - previewHeight) / 2;
		}

		CameraViewActualVideoRect = new Rect(offsetX, offsetY, previewWidth, previewHeight);

		if (CameraViewActualVideoRect.Width <= RectGrid.WidthRequest)
		{
			return;
		}
		if (CameraViewActualVideoRect.Height <= RectGrid.HeightRequest)
		{
			return;
		}
		
		var measureResult = RectGrid.Measure(EditViewWidth, double.PositiveInfinity);
		
		RectGrid.TranslationX = offsetX;
		RectGrid.TranslationY = previewHeight - measureResult.Height + offsetY;
		panX = RectGrid.TranslationX;
		panY = RectGrid.TranslationY;

		GridLayer.WidthRequest = CameraViewActualVideoRect.Width;
		GridLayer.HeightRequest = CameraViewActualVideoRect.Height;
	}

	/// <summary>
	/// IOSプラットフォームのビデオ表示領域を計算する
	/// </summary>
	void UpdateVideoDisplayLayoutForIOS()
	{
		if (CameraViewActualVideoRect.Height <= 0) return;
		RectGrid.TranslationX = 0;

		var measureResult = RectGrid.Measure(EditViewWidth, double.PositiveInfinity);

		RectGrid.TranslationY = CameraViewActualVideoRect.Height - measureResult.Height;
		panX = RectGrid.TranslationX;
		panY = RectGrid.TranslationY;

		GridLayer.WidthRequest = CameraViewActualVideoRect.Width;
		GridLayer.HeightRequest = CameraViewActualVideoRect.Height;
	}

	/// <summary>
	/// カメラ解像度表示リストの保存
	/// </summary>
	void SaveCameraResolutionDisplayList()
	{
		if (SelectedCamera == null) return;
		if (SelectedCamera.Position == CameraPosition.Unknown) return;

		string key = SelectedCamera.Position == CameraPosition.Front ? "CameraResolution_Front" : "CameraResolution_Rear";

		if (Preferences.Default.ContainsKey(key))
		{
			string selectedValue = Preferences.Default.Get($"{key}_Selected", "");
			if (!string.IsNullOrEmpty(selectedValue))
			{
				string[] arry = selectedValue.Split('×');
				SelectedResolution = new Size(int.Parse(arry[0]), int.Parse(arry[1]));
			}
		}
		else
		{
			var resolutions = SelectedCamera.SupportedResolutions.Select(x => $"{x.Width}×{x.Height}").ToList();
			if (resolutions.Count > 0)
			{
				SelectedResolution = SelectedCamera.SupportedResolutions[resolutions.Count - 1];
				string value = string.Join(",", resolutions);
				Preferences.Default.Set(key, value);
				Preferences.Default.Set($"{key}_Selected", resolutions[resolutions.Count - 1]);
			}
		}
	}

	void InitSelectedCamera()
	{
		if (SelectedCamera == null) return;
		if (SelectedCamera.Position == CameraPosition.Unknown) return;
		IsRearCamera = SelectedCamera.Position == CameraPosition.Rear ? true : false;
	}

	/// <summary>
	/// 検査記録を作成
	/// </summary>
	/// <returns></returns>
	InspectionRecordItem CreateInspectionRecordItem(string imageType)
	{
		InspectionRecordItem inspectionRecordItem = new();
		inspectionRecordItem.HR03002 = Path.GetFileNameWithoutExtension(PhotoScreenshotPath);
		inspectionRecordItem.FilePath = PhotoScreenshotPath;
		inspectionRecordItem.Comment = GreenBackgroundModel.Remark;
		inspectionRecordItem.CreateTime = DateTime.Now;
		inspectionRecordItem.HR03017 = imageType == "JPEG" ? 0 : 1;
		if (imageType == "JPEG")//JPEG
		{
			inspectionRecordItem.HR03018 = 1;
		}
		else//SVG
		{
			inspectionRecordItem.HR03018 = BlackboardDisplay ? 3 : 1;
		}
		inspectionRecordItem.Direction = int.Parse(ReinforcementTypeSelectedItem.Value);
		inspectionRecordItem.DirectionText = inspectionRecordItem.Direction == -1 ? ReinforcementTypeSelectedItem.DisplyName : string.Empty;
		return inspectionRecordItem;
	}

	/// <summary>
	/// 確認項目画面のナビゲーションパラメータを作成する
	/// </summary>
	/// <returns></returns>
	Dictionary<string, object> CreateNavigationParameterForCheckPoint()
	{
		var obj = new
		{
			FromPage = "CameraPage",
		};
		var options = new JsonSerializerOptions
		{
			WriteIndented = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};
		string jsonString = JsonSerializer.Serialize(obj, options);
		return new Dictionary<string, object>
		{
			{ "json", jsonString }
		};
	}

	/// <summary>
	/// 写真撮影のメッセージを送る
	/// </summary>
	void SendTakePhotoMessage()
	{
		ProjectPhotoMessage message = new ProjectPhotoMessage
		{
			ProjectCode = Service.ProjectCode,
			InspectionItemCode = Service.InspectionItemCode,
			PhotoPath = PhotoScreenshotPath
		};
		WeakReferenceMessenger.Default.Send(message, "TakePhotosToken");
	}

	/// <summary>
	/// 写真撮影前のデータ検証
	/// </summary>
	/// <returns></returns>
	async Task<bool> ValidateBeforeSavingAsync()
	{
		if (ReinforcementTypeList.Count == 0 && string.IsNullOrWhiteSpace(ReinforcementTypeText))
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM01037");
			DialogHelper.MessageDialogClose(string.Format(ErrMsg, "撮影方向"));

			return false;
		}
		return true;
	}

	/// <summary>
	/// 設定の撮影方向を更新
	/// </summary>
	void UpdateDirsListPreferences()
	{
		if (string.IsNullOrWhiteSpace(ReinforcementTypeText) && ReinforcementTypeSelectedItem != null)
		{
			ReinforcementTypeText = ReinforcementTypeSelectedItem.DisplyName;
			return;
		}

		if (!ReinforcementTypeList.Any(x => x.DisplyName.Trim() == ReinforcementTypeText.Trim()))
		{
			Service.AppContext.AddPreferencesDirection(ReinforcementTypeText);
			var listItem = new ListItem(ReinforcementTypeText, "-1");
			ReinforcementTypeList.Add(listItem);
			ReinforcementTypeSelectedItem = listItem;
		}
		else
		{
			ReinforcementTypeSelectedItem = ReinforcementTypeList.FirstOrDefault(x => x.DisplyName.Trim() == ReinforcementTypeText.Trim());
		}
	}
	#endregion
}

