using Aiko.Common;
using Aiko.Common.InkTools;
using Aiko.Common.Models;
using Aiko.UI.ViewModels.PageVMs;
using CommunityToolkit.Mvvm.Messaging;

namespace Aiko.UI.Pages;

// 图像处理部分 - CheckPointDetailPage.xaml.Image.cs
// 笔迹处理部分 - CheckPointDetailPage.xaml.Ink.cs
public partial class CheckPointDetailPage : ContentPage
{
    private CheckPointDetailPageVM _vm;

    private InkToolManager _toolManager;

    private InkImage _currentImage;

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

        if (_vm.ImageList.Count > 0)
        {
            List<InkImage> images = await _vm.GetImageInfo(_vm.Query, _vm.Proj.Value);
            await LoadGalleryImages(_vm.ImageFolderPath, images);
            if (_vm.ImageList.Count > 0) await LoadCanvasViewImage(_vm.ImageList[0]);
        }

        // 默认选中状态
        SwitchTool("Empty");
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

        MainGridView.SizeChanged += (s, e) => UpdateImageDimensions();

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
        if (string.IsNullOrEmpty(_vm.ImageFolderPath) || string.IsNullOrEmpty(_vm.Proj.Value)) return;

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
                double borderMidPoint = border.Width / 2; // TO DO: border.Width 目前得到的值总是-1，导致距离判断逻辑错误，待后续解决
                if (relativePoint.Value.X < borderMidPoint)
                {
                    // 距离左边更近
                    if (leftIndicator != null) leftIndicator.Color = Colors.DeepSkyBlue;
                    if (rightIndicator != null) rightIndicator.Color = Colors.Transparent;
                    _insertPosition = InsertPosition.Front; // 插入到前面
                }
                else
                {
                    // 距离右边更近
                    if (leftIndicator != null) leftIndicator.Color = Colors.Transparent;
                    if (rightIndicator != null) rightIndicator.Color = Colors.DeepSkyBlue;
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

        if (leftIndicator != null) leftIndicator.Color = Colors.Transparent;
        if (rightIndicator != null) rightIndicator.Color = Colors.Transparent;
    }

    private void OnDrop(object sender, DropEventArgs e)
    {
        var border = sender as Border;
        var targetItem = border?.BindingContext as InkImage;

        OnDragLeave(sender, null);

        if (_draggedItem == null || targetItem == null || _draggedItem == targetItem)
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
            }
        }

        _draggedItem = null;
    }

    // --- 其他交互 ---

    private async void OnGalleryItemTapped(object sender, EventArgs e)
    {
        bool? success = await Save(true, false);
        if (success != false)
        {
            var border = sender as Border;
            var image = border?.BindingContext as InkImage;

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
        var sign = e.Parameter as string;
        if (!string.IsNullOrEmpty(sign))
        {
            _currentImage.Sign = int.Parse(sign);
        }
    }

    private async Task<bool?> Save(bool confirm, bool all)
    {
        bool imageChanged = _currentImage != null && (_toolManager.StrokesChanged || CanvasViewVisibleChanged || _vm.ImageInfoChanged(_currentImage));

        var changedImageList = all ? _vm.GetChangedImageList() : new List<InkImage>();
        bool imageListChanged = all && (changedImageList.Count > 0 || _vm.SortChanged(new List<InkImage>(_vm.ImageList)));

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

                if (_toolManager.StrokesChanged || CanvasViewVisibleChanged)
                {
                    await SaveImage(_currentImage, _toolManager.CompletedStrokes.Select(stroke => stroke.Clone()).ToList());
                }
                if (_toolManager.StrokesChanged || CanvasViewVisibleChanged || _vm.ImageInfoChanged(_currentImage))
                {
                    if (!await _vm.SaveImageInfo(_currentImage))
                    {
                        success = false;
                    }
                }

                if (imageListChanged)
                {
                    if (!await _vm.SaveImageList(new List<InkImage>(_vm.ImageList), changedImageList))
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