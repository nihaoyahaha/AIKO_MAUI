using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
namespace Aiko.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		Aiko.Common.AikoAppContext.MainAssembly = typeof(App).Assembly;
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.UseMauiCommunityToolkitCamera()
			.UseMauiCommunityToolkit()
			.UseFFImageLoading()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                //fonts.AddFont("msgothic.ttc", "ＭＳ ゴシック");

                fonts.AddFont("Hiragino-Kaku-Gothic.otf", "ヒラギノ角ゴシック");
                fonts.AddFont("Hiragino-Mincho-ProN.otf", "ヒラギノ明朝 ProN");
                fonts.AddFont("Hiragino-Maru-Go-ProN.otf", "ヒラギノ丸ゴ ProN");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddHttpClient("WebDavClient")
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				var handler = new HttpClientHandler
				{
					AllowAutoRedirect = true,
					UseDefaultCredentials = false
				};
				handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true;
				return handler;
			});
		builder.Services.AddViews();
		builder.Services.AddServices();
		builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
		builder.Logging.AddStreamingFileLogger(options =>
		 {
			 options.Layout = new MySimpleLayout();
			 options.MinLevel = LogLevel.Information;
			 options.MaxLevel = LogLevel.Critical;
			 options.RetainDays = 7;
			 options.FolderPath = Path.Combine(
				 FileSystem.AppDataDirectory,
				 "Log");
		 });
		return builder.Build();
	}
}

/// <summary>
/// MetroLogsのメッセージテンプレートの設定
/// </summary>
public class MySimpleLayout : MetroLog.Layouts.Layout
{
	public override string GetFormattedString(MetroLog.LogWriteContext context, MetroLog.LogEventInfo info)
	{
		return $"{info.TimeStamp.ToLocalTime():yyyy-MM-dd HH:mm:ss} [{info.Level}] {info.Message}";
	}
}