using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.UI.Platforms.Windows;

public static class CameraCaptureHelper
{
	// 获取 Logger 实例
	private static ILogger _logger = IPlatformApplication.Current.Services.GetService<ILoggerFactory>().CreateLogger("CameraCaptureHelper");
	private static KeyEventHandler? _handler;
	private static FrameworkElement? _currentRoot;

	public static void AddVolumeKeyInteraction(FrameworkElement pageView, Action onCapture)
	{
		var nativeWindow = Microsoft.Maui.Controls.Application.Current?.Windows[0].Handler?.PlatformView as Microsoft.UI.Xaml.Window;
		if (nativeWindow == null) return;

		nativeWindow.DispatcherQueue.TryEnqueue(() =>
		{
			try
			{
				var root = nativeWindow.Content as FrameworkElement;
				if (root == null) return;
				RemoveInteraction();

				_currentRoot = root;
				_handler = new KeyEventHandler((s, e) =>
				{
					if ((int)e.Key == 175)
					{
						e.Handled = true;
						onCapture?.Invoke();
					}
				});

				_currentRoot.AddHandler(
					UIElement.PreviewKeyDownEvent,
					_handler,
					handledEventsToo: true);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "ウィンドウズ追加ボリュームキーインターセプト異常"); 
			}
		});
	}

	public static void RemoveInteraction()
	{
		try
		{
			if (_currentRoot != null && _handler != null)
			{
				_currentRoot.RemoveHandler(UIElement.PreviewKeyDownEvent, _handler);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "ウィンドウズ移除ボリュームキーインターセプト異常");
		}
		finally
		{
			_handler = null;
			_currentRoot = null;
		}
	}
}
