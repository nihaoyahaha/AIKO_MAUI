using Aiko.Common;
using Aiko.Common.InkTools;
using Aiko.Common.Models;
using Aiko.UI.ViewModels.PageVMs;
using CommunityToolkit.Mvvm.Messaging;
using FFImageLoading.Maui;
using Point = Microsoft.Maui.Graphics.Point;
using Size = Microsoft.Maui.Graphics.Size;

namespace Aiko.UI.Pages;

// 图像处理部分 - CheckPointDetailPage.xaml.Image.cs
// 笔迹处理部分 - CheckPointDetailPage.xaml.Ink.cs
public partial class CheckPointDetailPage : ContentPage
{
    private CheckPointDetailPageVM _vm;

    private InkToolManager _toolManager;

    private InkImage? _currentImage;

    private bool CanvasViewVisibleChanged =>
        (_vm.IsBlackboardVisible != _currentImage?.IsBlackboardVisible) ||
        (_vm.IsStrokesVisible != _currentImage?.IsStrokesVisible);

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetTitleView(this, null);

        _toolManager = new InkToolManager();
        _toolManager.TextEditRequested += OnTextEditRequested;

        _vm.ClearSelectedImage();

        _currentImage = null;
        PhotoCanvasView.InvalidateSurface();
        BlackboardCanvasView.InvalidateSurface();
        InkCanvasView.InvalidateSurface();

        if (_vm.Proj != null)
        {
            List<InkImage> images = await _vm.GetImageInfo(_vm.Query, _vm.Proj.Value);
            await LoadGalleryImages(_vm.ImageFolderPath, images);
            if (_vm.ImageList.Count > 0) await LoadCanvasViewImage(_vm.ImageList[0]);
        }

        // 默认选中状态
        SwitchTool("Empty");

        await _vm.LoadDanmImage();
        UpdateDanmImageDimensions();
    }

    public CheckPointDetailPage(CheckPointDetailPageVM checkPointDetailPageVM)
    {
        InitializeComponent();

#if ANDROID

#elif IOS

#elif WINDOWS
        Shell.SetNavBarIsVisible(this, false);
#endif

        _vm = checkPointDetailPageVM;
        BindingContext = checkPointDetailPageVM;

        // 订阅 VM 的属性变更通知
        _vm.PropertyChanged += OnViewModelPropertyChanged;

        MainGridView.SizeChanged += (s, e) =>
        {
            if (MainGridView.Width > 0 && MainGridView.Height > 0)
                UpdateImageDimensions();
        };
        ImageGrid.SizeChanged += (s, e) =>
        {
            if (ImageGrid.Width > 0 && ImageGrid.Height > 0)
                UpdateDanmImageDimensions();
        };

        WeakReferenceMessenger.Default.Register<CheckPointDetailPage, CheckPointDetailPageVM.AsyncRequestMessage>(this, async (page, message) =>
        {
            switch (message.Name)
            {
                case "Save":
                    message.Result.Add("success", await Save(false, true));
                    break;
                case "Back":
                    message.Result.Add("success", await Save(true, true));
                    break;
                case "SaveBack":
                    message.Result.Add("success", await Save(false, true));
                    break;
                case "SwitchImage":
                    var image = message.Parameters["image"] as InkImage;
                    await SwitchImage(image);
                    break;
                case "ClearImage":
                    _currentImage = null;
                    break;
                default:
                    break;
            }

            message.Tcs.TrySetResult(true);
        });
    }

    private async void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // 监听 Proj 属性变化，加载指定的图片集
        if (e.PropertyName == nameof(_vm.Proj))
        {
            await OnProjChanged();
        }
    }

    private async Task OnProjChanged()
    {
        if (string.IsNullOrEmpty(_vm.ImageFolderPath) || _vm.Proj == null || string.IsNullOrEmpty(_vm.Proj.Value)) return;

        if (!Directory.Exists(_vm.ImageFolderPath))
        {
            // 图片文件夹不存在
            DialogHelper.MessageDialog("画像フォルダが見つかりません。");
            return;
        }

        _vm.ClearSelectedImage();

        List<InkImage> images = await _vm.GetImageInfo(_vm.Query, _vm.Proj.Value);
        await LoadGalleryImages(_vm.ImageFolderPath, images);
        if (_vm.ImageList.Count > 0) await LoadCanvasViewImage(_vm.ImageList[0]);
    }

    private void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        double lastVisibleItemIndex = ((e.VerticalOffset + (sender as CollectionView).Height) / ((sender as CollectionView).Height / 3)) * 3; // 这个比 e.LastVisibleItemIndex 稍微准些

        if (lastVisibleItemIndex >= _vm.ImageList.Count - 3)
        {
            _vm.LoadNextPage();
        }
    }

    private void OnImageVisualized(object sender, CachedImageEvents.SuccessEventArgs e)
    {
        var cachedImage = sender as CachedImage;
        var model = cachedImage?.BindingContext as InkImage;
        if (model != null)
        {
            model.IsVisualized = true;
            _vm.NoditySummary();
        }
    }

    // --- 断面页签 ---

    private async void UpdateDanmImageDimensions()
    {
        if (ImageGrid.Width <= 0 || ImageGrid.Height <= 0) return;

        if (DanmImage.Source == null) return;

        Size size = Size.Zero;
        for (int i = 0; i < 10; i++)
        {
            size = await GetOriginalImageSize(DanmImage);

            if (size.Width > 0 && size.Height > 0) break;

            await Task.Delay(50);
        }

        double fitScale = Math.Min(1d, Math.Min(ImageGrid.Width / size.Width, ImageGrid.Height / size.Height));

        double fitWidth = size.Width * fitScale;
        double fitHeight = size.Height * fitScale;

        DanmImageZoomLayer.WidthRequest = fitWidth;
        DanmImageZoomLayer.HeightRequest = fitHeight;

        DanmImage.WidthRequest = fitWidth;
        DanmImage.HeightRequest = fitHeight;

        DanmImagePinchToZoom.SetBaseSize(fitWidth, fitHeight);
    }
    private static async Task<Size> GetOriginalImageSize(Image image)
    {
#if ANDROID
        var handler = image.Handler?.PlatformView as Android.Widget.ImageView;
        if (handler?.Drawable != null)
        {
            return new Size(handler.Drawable.IntrinsicWidth, handler.Drawable.IntrinsicHeight);
        }
#elif IOS
        var handler = image.Handler?.PlatformView as UIKit.UIImageView;
        if (handler?.Image != null)
        {
            return new Size(handler.Image.Size.Width, handler.Image.Size.Height);
        }
#elif WINDOWS
        var handler = image.Handler?.PlatformView as Microsoft.UI.Xaml.Controls.Image;
        if (handler?.Source is Microsoft.UI.Xaml.Media.Imaging.BitmapSource bs)
        {
            return new Size(bs.PixelWidth, bs.PixelHeight);
        }
#endif
        return Size.Zero;
    }

    // --- 拖拽重排 ---

    // 被拖拽的图片
    private InkImage _draggedItem;
    // 插入位置
    private enum InsertPosition { Front, Behind };
    private InsertPosition _insertPosition = InsertPosition.Behind;

    private void OnDragStarting(object sender, DragStartingEventArgs e)
    {
        var border = sender as Border;
        _draggedItem = border?.BindingContext as InkImage;
        if (_draggedItem != null)
        {
            e.Data.Properties.Add("InkImage", _draggedItem);

            // 震动反馈
            try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); } catch { }
        }
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        if (_draggedItem == null) return;

        var border = sender as Border;
        if (border?.BindingContext is InkImage targetItem && targetItem != _draggedItem)
        {
            // 获取当前触摸点相对于 Border 的位置
            Point? relativePoint = e.GetPosition(border);

            if (relativePoint.HasValue)
            {
                var leftIndicator = border.FindByName<BoxView>("LeftDropIndicator");
                var rightIndicator = border.FindByName<BoxView>("RightDropIndicator");

                // 判断距离哪边更近

                double borderMidPoint = 0;
                if (border.Width > 0)
                {
                    borderMidPoint = border.Width / 2;
                }
                else if (border.Content != null)
                {
                    borderMidPoint = border.Content.DesiredSize.Width / 2;
                }

                if (relativePoint.Value.X < borderMidPoint)
                {
                    // 距离左边更近
                    leftIndicator?.Color = Colors.DeepSkyBlue;
                    rightIndicator?.Color = Colors.Transparent;
                    _insertPosition = InsertPosition.Front; // 插入到前面
                }
                else
                {
                    // 距离右边更近
                    leftIndicator?.Color = Colors.Transparent;
                    rightIndicator?.Color = Colors.DeepSkyBlue;
                    _insertPosition = InsertPosition.Behind; // 插入到后面
                }
            }
        }
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        var border = sender as Border;
        var leftIndicator = border?.FindByName<BoxView>("LeftDropIndicator");
        var rightIndicator = border?.FindByName<BoxView>("RightDropIndicator");

        leftIndicator?.Color = Colors.Transparent;
        rightIndicator?.Color = Colors.Transparent;
    }

    private void OnDrop(object sender, DropEventArgs e)
    {
        var border = sender as Border;

        OnDragLeave(sender, null);

        if (_draggedItem == null || border?.BindingContext is not InkImage targetItem || _draggedItem == targetItem)
        {
            _draggedItem = null;
            return;
        }

        int oldIndex = _vm.ImageList.IndexOf(_draggedItem);
        int targetIndex = _vm.ImageList.IndexOf(targetItem);
        if (oldIndex != -1 && targetIndex != -1)
        {
            int newIndex = (_insertPosition == InsertPosition.Behind) ? targetIndex + 1 : targetIndex;
            if (newIndex > oldIndex)
            {
                newIndex--;
            }
            if (oldIndex != newIndex)
            {
                _vm.ImageList.Move(oldIndex, Math.Clamp(newIndex, 0, _vm.ImageList.Count - 1));

                var itemToMove = _vm.SourceImageList[oldIndex];
                _vm.SourceImageList.RemoveAt(oldIndex);
                int clampedIndex = Math.Clamp(newIndex, 0, _vm.SourceImageList.Count);
                _vm.SourceImageList.Insert(clampedIndex, itemToMove);
            }
        }

        _draggedItem = null;
    }

    // --- 其他交互 ---

    private void OnArrowClicked(object sender, EventArgs e)
    {
        HiddenPicker.Focus();

#if WINDOWS
        var nativePicker = (Microsoft.UI.Xaml.Controls.ComboBox)HiddenPicker.Handler.PlatformView;
        nativePicker.IsDropDownOpen = true;
#endif
    }

    private async void OnGalleryItemTapped(object sender, EventArgs e)
    {
        if (!_vm.IsLoaded) return;

        var border = sender as Border;
        var image = border?.BindingContext as InkImage;

        if (image != null) await SwitchImage(image);
    }

    private async Task SwitchImage(InkImage? image)
    {
        bool? success = await Save(true, false);
        if (success != false)
        {
            await LoadCanvasViewImage(image);

            SwitchTool("Empty");
        }
        else
        {
            DialogHelper.MessageDialog("保存に失敗しました");
        }
    }

    private void OnSignOptionTapped(object sender, TappedEventArgs e)
    {
        if (_currentImage == null) return;

        var sign = e.Parameter as string;
        if (!string.IsNullOrEmpty(sign))
        {
            _currentImage.Sign = int.Parse(sign);
            _vm.Sign = _currentImage.Sign;
        }
    }

    private async Task<bool?> Save(bool confirm, bool all)
    {
        bool strokesChanged = _toolManager.StrokesChanged;

        bool imageChanged = _currentImage != null && (strokesChanged || CanvasViewVisibleChanged || _vm.ImageInfoChanged(_currentImage));

        var changedImageList = all ? _vm.GetChangedImageList() : new List<InkImage>();
        bool imageListChanged = all && (changedImageList.Count > 0 || _vm.SortChanged(_vm.SourceImageList));

        if (imageChanged || imageListChanged)
        {
            var result = NCDialogResult.Yes;
            if (confirm)
            {
                result = await DialogHelper.MessageDialogButton2("未保存の内容があります。保存しますか？");
            }
            if (result == NCDialogResult.Yes)
            {
                bool success = true;

                if (_currentImage != null)
                {
                    if (strokesChanged || CanvasViewVisibleChanged)
                    {
                        await SaveImage(_currentImage, _toolManager.CompletedStrokes.Select(stroke => stroke.Clone()).ToList());
                    }
                    if (strokesChanged || CanvasViewVisibleChanged || _vm.ImageInfoChanged(_currentImage))
                    {
                        if (!await _vm.SaveImageInfo(_currentImage))
                        {
                            success = false;
                        }
                    }
                }

                if (imageListChanged)
                {
                    if (!await _vm.SaveImageList(_vm.SourceImageList, changedImageList))
                    {
                        success = false;
                    }
                    else
                    {
                        _vm.ResetImageList();
                    }
                }

                _vm.UpdateDirsListPreferences(_vm.Dirs.Value);

                return success;
            }
            else
            {
                _vm.RevertImageList();
            }
        }

        return null;
    }
}