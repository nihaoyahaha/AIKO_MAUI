using AVKit;
using UIKit;
using Foundation;

namespace Aiko.UI.Platforms.iOS;

public static class CameraCaptureHelper
{
	private static AVCaptureEventInteraction? _interaction;

	public static void AddCaptureInteraction(UIView nativeView, Action onCapture)
	{
		if (!OperatingSystem.IsIOSVersionAtLeast(17, 2)) return;

		if (_interaction != null) nativeView.RemoveInteraction(_interaction);

		Action<AVCaptureEvent> handleCapture = (evt) =>
		{
			if (evt.Phase == AVCaptureEventPhase.Ended)
			{
				onCapture?.Invoke();
			}
		};

		_interaction = new AVCaptureEventInteraction(
			primaryHandler: handleCapture,    
			secondaryHandler: _ => { }
			);

		nativeView.AddInteraction(_interaction);
	}
}