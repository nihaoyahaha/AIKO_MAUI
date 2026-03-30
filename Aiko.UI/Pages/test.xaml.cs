using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Layouts;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static Microsoft.Maui.ApplicationModel.Permissions;
using Size = Microsoft.Maui.Graphics.Size;

namespace Aiko.UI;

public partial class test : ContentPage
{
	Rect _previewRectInParent;
	bool _previewRectReady;

	// 保存格式控制
	private bool _saveAsJpg = true;
	private bool _saveAsSvg = true;

	// 🎯 浮层图片路径（后续改为自定义控件渲染）
	private string _overlayImagePath = "";  // 设置你的浮层图片路径

	public class ResolutionItem
	{
		public CameraResolution Resolution { get; set; }
		public string Display => $"{Resolution.Width} x {Resolution.Height}";
	}

	ObservableCollection<ResolutionItem> _resolutions = new ObservableCollection<ResolutionItem>();
	CameraResolution _selectedResolution;
	CancellationTokenSource captureImageCTS = new CancellationTokenSource(TimeSpan.FromSeconds(3));

	public test()
	{
		InitializeComponent();
		ResolutionPicker.ItemsSource = _resolutions;

		var pan = new PanGestureRecognizer();
		pan.PanUpdated += OnrectGridPanUpdated;
		rectGrid.GestureRecognizers.Add(pan);

		// 🎯 添加 PreviewImage 点击事件
		var tapGesture = new TapGestureRecognizer();
		tapGesture.Tapped += OnPreviewImageTapped;
		PreviewImage.GestureRecognizers.Add(tapGesture);
	}

	private async void ContentPage_Loaded(object sender, EventArgs e)
	{
		await InitCameraAsync();
	}

	async Task InitCameraAsync()
	{
		try
		{
			var cameras = await Camera.GetAvailableCameras(CancellationToken.None);
			var rearCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Rear)
							 ?? cameras.FirstOrDefault();
			if (rearCamera is null) return;

			Camera.SelectedCamera = rearCamera;
			_resolutions.Clear();

			foreach (var res in rearCamera.SupportedResolutions)
			{
				_resolutions.Add(new ResolutionItem
				{
					Resolution = new CameraResolution { Width = (int)res.Width, Height = (int)res.Height }
				});
			}

			var best = rearCamera.SupportedResolutions
				.OrderByDescending(r => r.Width * r.Height)
				.FirstOrDefault();

			if (best != null)
			{
				_selectedResolution = new CameraResolution { Width = (int)best.Width, Height = (int)best.Height };
				Camera.ImageCaptureResolution = new Microsoft.Maui.Graphics.Size(best.Width, best.Height);
			}

			_previewRectReady = true;
			//await Camera.StartCameraPreview(captureImageCTS.Token);
		}
		catch (Exception ex)
		{
			await DisplayAlert("错误", ex.ToString(), "OK");
		}
	}

	void OnResolutionChanged(object sender, EventArgs e)
	{
		try
		{
			if (ResolutionPicker.SelectedItem is ResolutionItem item)
			{
				_selectedResolution = item.Resolution;
				Camera.ImageCaptureResolution = new Size(_selectedResolution.Width, _selectedResolution.Height);
			}
		}
		catch { }
	}

	// 🎯 拍照主流程
	async void OnCaptureClicked(object sender, EventArgs e)
	{
		try
		{
			if (!_previewRectReady)
			{
				await DisplayAlert("错误", "相机未就绪", "OK");
				return;
			}

			// 1. 拍照
			var stream = await Camera.CaptureImage(CancellationToken.None);
			stream.Position = 0;
			using var originalBitmap = SKBitmap.Decode(stream);

			// 2. 裁剪成预览比例
			var cropRect = CalculatePreviewCropRect(originalBitmap.Width, originalBitmap.Height);
			using var photoBitmap = new SKBitmap(cropRect.Width, cropRect.Height);
			using (var canvas = new SKCanvas(photoBitmap))
			{
				var src = new SKRect(cropRect.Left, cropRect.Top, cropRect.Right, cropRect.Bottom);
				var dest = new SKRect(0, 0, photoBitmap.Width, photoBitmap.Height);
				canvas.DrawBitmap(originalBitmap, src, dest);
			}

			// 3. 将自定义控件渲染为图片
			string overlayImagePath = await RenderOverlayToImage();

			// 4. 计算浮层位置（红框映射到照片坐标）
			var overlayRect = CalculateRedBoxInPhoto(photoBitmap.Width, photoBitmap.Height);

			// 5. 生成文件名
			string filePrefix = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}";
			var savedFiles = new List<string>();

			// 6. 保存 JPG（照片 + 浮层合成）
			if (_saveAsJpg)
			{
				string jpgPath = Path.Combine(FileSystem.CacheDirectory, $"{filePrefix}.jpg");
				SaveCompositeJpg(photoBitmap, jpgPath, overlayImagePath, overlayRect);
				savedFiles.Add($"JPG: {jpgPath}");
				PreviewImage.Source = ImageSource.FromFile(jpgPath);
			}

			// 7. 保存 SVG（双层：照片 + 浮层）
			if (_saveAsSvg)
			{
				string svgPath = Path.Combine(FileSystem.CacheDirectory, $"{filePrefix}.svg");
				SaveSvg(photoBitmap, svgPath, overlayImagePath, overlayRect);
				savedFiles.Add($"SVG: {svgPath}");
			}

			// 8. 清理临时浮层图片
			if (File.Exists(overlayImagePath))
			{
				File.Delete(overlayImagePath);
			}

			if (savedFiles.Count > 0)
			{
				await DisplayAlert("拍照完成", string.Join("\n", savedFiles), "OK");
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("错误", ex.Message, "OK");
		}
	}

	async Task<string> RenderOverlayToImage()
	{
		try
		{
			// 找到 rectGrid 内的自定义控件
			var overlayView = rectGrid.Content as VisualElement;
			if (overlayView == null)
				throw new InvalidOperationException("未找到浮层控件");

			// 截图
			var screenshot = await overlayView.CaptureAsync();
			if (screenshot == null)
				throw new InvalidOperationException("截图失败");

			// 保存为临时 PNG 文件
			string tempPath = Path.Combine(FileSystem.CacheDirectory, $"overlay_temp_{Guid.NewGuid()}.png");

			using var sourceStream = await screenshot.OpenReadAsync();
			using var fileStream = File.Create(tempPath);
			await sourceStream.CopyToAsync(fileStream);

			return tempPath;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"渲染浮层失败: {ex.Message}");
			throw;
		}
	}

	// 🎯 保存 JPG（照片 + 浮层合成）
	void SaveCompositeJpg(SKBitmap photoBitmap, string jpgPath, string overlayImagePath, Rect overlayRect)
	{
		using var compositeBitmap = new SKBitmap(photoBitmap.Width, photoBitmap.Height);
		using var canvas = new SKCanvas(compositeBitmap);

		// 绘制底层照片
		canvas.DrawBitmap(photoBitmap, 0, 0);

		// 绘制浮层图片
		if (!string.IsNullOrEmpty(overlayImagePath) && File.Exists(overlayImagePath))
		{
			using var overlayBitmap = SKBitmap.Decode(overlayImagePath);
			var destRect = new SKRect(
				(float)overlayRect.X,
				(float)overlayRect.Y,
				(float)(overlayRect.X + overlayRect.Width),
				(float)(overlayRect.Y + overlayRect.Height)
			);
			canvas.DrawBitmap(overlayBitmap, destRect);
		}

		// 保存为 JPG
		using var output = File.Create(jpgPath);
		compositeBitmap.Encode(output, SKEncodedImageFormat.Jpeg, 95);
	}

	// 🎯 保存 SVG（双层：照片层 + 浮层图片）
	void SaveSvg(SKBitmap photoBitmap, string svgPath, string overlayImagePath, Rect overlayRect)
	{
		using var photoStream = new MemoryStream();
		photoBitmap.Encode(photoStream, SKEncodedImageFormat.Jpeg, 95);
		string photoBase64 = Convert.ToBase64String(photoStream.ToArray());

		var svg = new System.Text.StringBuilder();
		int width = photoBitmap.Width;
		int height = photoBitmap.Height;

		svg.AppendLine($@"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" 
     xmlns:xlink=""http://www.w3.org/1999/xlink""
     xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
     width=""{width}"" 
     height=""{height}"" 
     viewBox=""0 0 {width} {height}"">

  <!-- 第 1 层：照片 -->
  <g inkscape:groupmode=""layer"" inkscape:label=""照片层"" id=""photo-layer"">
    <image x=""0"" y=""0"" width=""{width}"" height=""{height}""
           xlink:href=""data:image/jpeg;base64,{photoBase64}""/>
  </g>

  <!-- 第 2 层：浮层 -->
  <g inkscape:groupmode=""layer"" inkscape:label=""浮层"" id=""overlay-layer"">");

		if (!string.IsNullOrEmpty(overlayImagePath) && File.Exists(overlayImagePath))
		{
			byte[] overlayBytes = File.ReadAllBytes(overlayImagePath);
			string overlayBase64 = Convert.ToBase64String(overlayBytes);

			// ✅ 关键修改：使用 overlayRect 的宽高，而不是截图的实际尺寸
			svg.AppendLine($@"    <image x=""{overlayRect.X:F2}"" y=""{overlayRect.Y:F2}"" 
           width=""{overlayRect.Width:F2}"" height=""{overlayRect.Height:F2}""
           xlink:href=""data:image/png;base64,{overlayBase64}""/>");
		}

		svg.AppendLine(@"  </g>
</svg>");

		File.WriteAllText(svgPath, svg.ToString());
	}

	// 🎯 计算红框在照片中的位置
	Rect CalculateRedBoxInPhoto(int photoWidth, int photoHeight)
	{
		var redBoxBounds = AbsoluteLayout.GetLayoutBounds(rectGrid);
		double relativeX = redBoxBounds.X - _previewRectInParent.X;
		double relativeY = redBoxBounds.Y - _previewRectInParent.Y;
		double scaleX = photoWidth / _previewRectInParent.Width;
		double scaleY = photoHeight / _previewRectInParent.Height;
		double photoX = Math.Max(0, relativeX * scaleX);
		double photoY = Math.Max(0, relativeY * scaleY);
		double photoW = redBoxBounds.Width * scaleX;
		double photoH = redBoxBounds.Height * scaleY;
		photoX = Math.Min(photoX, photoWidth - photoW);
		photoY = Math.Min(photoY, photoHeight - photoH);
		return new Rect(photoX, photoY, photoW, photoH);
	}

	SKRectI CalculatePreviewCropRect(int photoWidth, int photoHeight)
	{
		const double PreviewRatio = 16.0 / 9.0;
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

	void OnMediaCaptured(object sender, MediaCapturedEventArgs e) { }

	void OnCameraSizeChanged(object sender, EventArgs e)
	{
		try
		{
			const double PreviewRatio = 16.0 / 9.0;
			var camBounds = Camera.Bounds;
			if (camBounds.Width <= 0 || camBounds.Height <= 0) return;

			double viewRatio = camBounds.Width / camBounds.Height;
			double previewWidth, previewHeight, offsetX, offsetY;

			if (viewRatio > PreviewRatio)
			{
				previewHeight = camBounds.Height;
				previewWidth = previewHeight * PreviewRatio;
				offsetX = camBounds.X + (camBounds.Width - previewWidth) / 2.0;
				offsetY = camBounds.Y;
			}
			else
			{
				previewWidth = camBounds.Width;
				previewHeight = previewWidth / PreviewRatio;
				offsetX = camBounds.X;
				offsetY = camBounds.Y + (camBounds.Height - previewHeight) / 2.0;
			}

			_previewRectInParent = new Rect(offsetX, offsetY, previewWidth, previewHeight);
			_previewRectReady = true;

			double rectW = 340;
			double rectH = 310;
			AbsoluteLayout.SetLayoutFlags(rectGrid, AbsoluteLayoutFlags.None);
			AbsoluteLayout.SetLayoutBounds(rectGrid, new Rect(
				offsetX + (previewWidth - rectW) / 2,
				offsetY + (previewHeight - rectH) / 2,
				rectW, rectH));
		}
		catch { }
	}

	double _lastPanTotalX = 0;
	double _lastPanTotalY = 0;

	void OnrectGridPanUpdated(object? sender, PanUpdatedEventArgs e)
	{
		try
		{
			if (!_previewRectReady) return;
			var bounds = AbsoluteLayout.GetLayoutBounds(rectGrid);

			switch (e.StatusType)
			{
				case GestureStatus.Started:
					_lastPanTotalX = _lastPanTotalY = 0;
					break;
				case GestureStatus.Running:
					var newX = Math.Max(_previewRectInParent.X,
						Math.Min(bounds.X + (e.TotalX - _lastPanTotalX),
							_previewRectInParent.Right - bounds.Width));
					var newY = Math.Max(_previewRectInParent.Y,
						Math.Min(bounds.Y + (e.TotalY - _lastPanTotalY),
							_previewRectInParent.Bottom - bounds.Height));
					AbsoluteLayout.SetLayoutBounds(rectGrid, new Rect(newX, newY, bounds.Width, bounds.Height));
					_lastPanTotalX = e.TotalX;
					_lastPanTotalY = e.TotalY;
					break;
				case GestureStatus.Completed:
				case GestureStatus.Canceled:
					_lastPanTotalX = _lastPanTotalY = 0;
					break;
			}
		}
		catch { }
	}

	// 🎯 点击缩略图跳转到浏览页
	void OnPreviewImageTapped(object sender, EventArgs e)
	{
		
	}

	async private void btn_test_Clicked(object sender, EventArgs e)
	{
		var cameras = await Camera.GetAvailableCameras(CancellationToken.None);
		Camera.SelectedCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Rear);
	}
}

public class CameraResolution
{
	public int Width { get; set; }
	public int Height { get; set; }
}