using Aiko.UI.ViewModels.PageVMs;

namespace Aiko.UI;

public partial class CameraPage : ContentPage
{
	double currentScale = 1, startScale = 1;
	CameraPageVM _vm;
	public CameraPage(CameraPageVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
		vm.RectGrid = rectGrid;
		_vm = vm;
	}

	private void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
	{
		rectGrid.CaptureAsync();
		if (currentScale > slider.Maximum)
		{
			currentScale = slider.Maximum;
			return;
		}
		if (currentScale < slider.Minimum)
		{
			currentScale = slider.Minimum;
			return;
		}

		if (e.Status == GestureStatus.Started)
		{
			startScale = camera.ZoomFactor;
		}
		if (e.Status == GestureStatus.Running)
		{
			currentScale += (e.Scale - 1) * startScale;
			currentScale = Math.Max(1, currentScale);
			camera.ZoomFactor = (float)currentScale;
		}
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