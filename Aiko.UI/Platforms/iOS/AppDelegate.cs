using Foundation;
using UIKit;

namespace Aiko.UI
{
	[Register("AppDelegate")]
	public class AppDelegate : MauiUIApplicationDelegate
	{
		protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

		// 添加以下代码强制控制方向
		[Export("application:supportedInterfaceOrientationsForWindow:")]
		public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
		{
			// 强制只返回横屏掩码，即使 Info.plist 里有竖屏，也会被这里拦截
			return UIInterfaceOrientationMask.Landscape;
		}
	}
}
