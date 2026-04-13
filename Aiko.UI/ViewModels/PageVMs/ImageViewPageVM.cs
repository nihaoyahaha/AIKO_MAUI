using Aiko.Common.Models.ImageView;
using Aiko.IServices.IServices;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class ImageViewPageVM : Observablebase<ImageViewPageVM, IImageViewService>
{
	public ImageViewPageVM(ILogger<ImageViewPageVM> logger, IImageViewService service) : base(logger, service)
	{

	}

	[ObservableProperty]
	private bool _greenBackgroundCheckBoxLabelIsVisible = false;

	[ObservableProperty]
	private bool _photoCheckBoxLabelIsVisible = false;

	[ObservableProperty]
	private ObservableCollection<PhotoPreviewModel> _photoPreviews = new ObservableCollection<PhotoPreviewModel>();

	[ObservableProperty]
	private PhotoPreviewModel _photoPreview;

	[ObservableProperty]
	private bool _canGoPrevious = false;

	[ObservableProperty]
	private bool _canGoNext = false;

	[ObservableProperty]
	private string _pageInfo;

	public PinchToZoomContainer ZoomContainer;

	private Rect _mainGridBoundRect;

	private int _currentPosition = -1;

	[RelayCommand]
	private void PageLoaded()
	{
		try
		{
			InitializePage();
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
	}

	[RelayCommand]
	private void MainGridViewSizeChanged(object sender)
	{
		var grid = sender as Grid;
		_mainGridBoundRect = grid.Bounds;
	}

	[RelayCommand]
	private void PreviousImage(object? parameter)
	{
		string? parm = parameter as string;
		if(parm == "swipeGesture" && !ZoomContainer.IsAtInitialScale) return;

		if (_currentPosition == 0) return;

		_currentPosition--;
		PositionChanged();
	}

	[RelayCommand]
	private void NextImage(object? parameter)
	{
		string? parm = parameter as string;
		if (parm == "swipeGesture" && !ZoomContainer.IsAtInitialScale) return;

		if (_currentPosition == PhotoPreviews.Count - 1) return;

		_currentPosition++;
		PositionChanged();
	}

	[RelayCommand]
	private async Task RemoveCurrentItem()
	{
		try
		{
			if (PhotoPreviews.Count == 0) return;

			string? filePath = PhotoPreview.ImageUrl;
			Uri uri = new Uri(filePath);
			string localPath = uri.LocalPath;
			if (File.Exists(localPath)) File.Delete(localPath);
			await Service.RemovePreviewPhotoAsync(PhotoPreview);
			PhotoPreviews.RemoveAt(_currentPosition);
			WeakReferenceMessenger.Default.Send($"{localPath}", "DeletePhotosToken");

			var index = _currentPosition >= PhotoPreviews.Count
						   ? Math.Max(0, PhotoPreviews.Count - 1)
						   : _currentPosition;

			_currentPosition = PhotoPreviews.Count == 0 ? -1 : index;

			PositionChanged();
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
	}

	[RelayCommand]
	private async Task Back()
	{
		await Shell.Current.GoToAsync("..?FromPage=ImageViewPage");
	}

	void InitializePage()
	{
		string photoType = Preferences.Default.Get("PhotoType", "JPEG");
		PhotoPreviews = Service.GetPhotoPreviewModels();
		_currentPosition = PhotoPreviews.Count > 0 ? 0 : -1;
		SetPageInfo();
		SetGoToImageEnable();
		if (photoType == "SVG")
		{
			PhotoCheckBoxLabelIsVisible = true;
			GreenBackgroundCheckBoxLabelIsVisible = PhotoPreviews[_currentPosition].PhotoLayer.GreenBackgroundIsVisible;
		}
		else
		{
			GreenBackgroundCheckBoxLabelIsVisible = false;
			PhotoCheckBoxLabelIsVisible = false;
		}
		foreach (var photo in PhotoPreviews)
		{
			photo.ChangePhotoSize(_mainGridBoundRect.Width, _mainGridBoundRect.Height);
		}
		PhotoPreview = PhotoPreviews[0];
		ZoomContainer.SetBaseSize(double.Parse(PhotoPreview.PhotoLayer.PhotoWidth), double.Parse(PhotoPreview.PhotoLayer.PhotoHeight));
	}

	void PositionChanged()
	{
		try
		{
			SetGoToImageEnable();
			SetPageInfo();
			if (_currentPosition == -1)
			{
				PhotoPreview = null;
			}
			else
			{
				PhotoPreview = PhotoPreviews[_currentPosition];
				GreenBackgroundCheckBoxLabelIsVisible = PhotoPreview.PhotoLayer.GreenBackgroundIsVisible;
			}
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
	}

	void SetGoToImageEnable()
	{
		CanGoPrevious = _currentPosition > 0 ? true : false;
		CanGoNext = _currentPosition == PhotoPreviews.Count - 1 ? false : true;
	}

	void SetPageInfo()
	{
		int currentPage = PhotoPreviews.Count == 0 ? 0 : _currentPosition + 1;
		int totalPages = PhotoPreviews.Count;
		PageInfo = $"{currentPage}/{totalPages}";
	}
}
