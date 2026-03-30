using Microsoft.Maui.Platform;

namespace Aiko.Common.InkTools
{
    public static class InkUtils
    {
        public static double Density => DeviceDisplay.Current.MainDisplayInfo.Density > 0 ? DeviceDisplay.Current.MainDisplayInfo.Density : 1;

        public static Rect GetBoundingBox(this IView view)
        {
            var platformView = view.Handler?.PlatformView;
            if (platformView == null) return Rect.Zero;

#if WINDOWS
            var nativeView = (Microsoft.UI.Xaml.FrameworkElement)platformView;
            var ttv = nativeView.TransformToVisual(null);
            Microsoft.Maui.Graphics.Point point = ttv.TransformPoint(new Windows.Foundation.Point(0, 0)).ToPoint();
            return new Rect(point.X, point.Y, view.Width, view.Height);
#elif ANDROID
            var density = Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
            int[] location = new int[2];
            ((Android.Views.View)platformView).GetLocationOnScreen(location);
            return new Rect(location[0] / density, location[1] / density, view.Width, view.Height);
#elif IOS || MACCATALYST
            var nativeView = (UIKit.UIView)platformView;
            var rect = nativeView.ConvertRectToView(nativeView.Bounds, null);
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
#endif
            return Rect.Zero;
        }

        public static double GetFitScale((double width, double height) container, (double width, double height) content)
        {
            if (container.width < 0 || container.height < 0 || content.width < 0 || content.height < 0) return 1;

            double scaleX = container.width / content.width;
            double scaleY = container.height / content.height;
            return Math.Min(scaleX, scaleY);
        }
    }
}