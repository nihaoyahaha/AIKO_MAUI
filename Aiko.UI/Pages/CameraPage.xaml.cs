using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class CameraPage : ContentPage
{
	CameraPageVM _vm;
	public CameraPage(CameraPageVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
		vm.RectGrid = rectGrid;
		_vm = vm;
	}

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
#if IOS
		if (Handler?.PlatformView == null) return;

		Action captureAction = async () =>
		{
			await camera.CaptureImage(CancellationToken.None);
		};

		var nativeView = camera.Handler?.PlatformView as UIKit.UIView;
		if (nativeView != null)
		{
			Platforms.iOS.CameraCaptureHelper.AddCaptureInteraction(nativeView, captureAction);
		}       
#endif
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
#if WINDOWS
    // 使用 Dispatcher 延迟到下一帧执行
    Dispatcher.Dispatch(() =>
    {
        if (this.Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement winView)
        {
            Action captureAction = async () =>
            {
                await camera.CaptureImage(CancellationToken.None);
            };
            Platforms.Windows.CameraCaptureHelper.AddVolumeKeyInteraction(winView, captureAction);
        }
    });
#endif
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
#if WINDOWS
        Platforms.Windows.CameraCaptureHelper.RemoveInteraction();
#endif
	}

	private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
	{
		switch (e.StatusType)
		{
			case GestureStatus.Running:
				rectGrid.TranslationX = Math.Clamp(_vm.panX + e.TotalX, _vm.CameraViewActualVideoRect.X, _vm.CameraViewActualVideoRect.X + _vm.CameraViewActualVideoRect.Width - rectGrid.Width);
				rectGrid.TranslationY = Math.Clamp(_vm.panY + e.TotalY, _vm.CameraViewActualVideoRect.Y, _vm.CameraViewActualVideoRect.Y + _vm.CameraViewActualVideoRect.Height - rectGrid.Height);
				break;

			case GestureStatus.Completed:
				_vm.panX = rectGrid.TranslationX;
				_vm.panY = rectGrid.TranslationY;
				break;
		}
	}

}