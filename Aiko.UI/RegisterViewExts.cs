using Aiko.UI.CustomControls;
using Aiko.UI.Pages;
using Aiko.UI.ViewModels.PageVMs;
using Aiko.UI.ViewModels.UserControlVms;
using CommunityToolkit.Maui;

namespace Aiko.UI;

public static class RegisterViewExts
{
	public static IServiceCollection AddViews(this IServiceCollection services) {
        services.AddScopedWithShellRoute<MapViewPage, MapViewPageVM>("MapView");
        services.AddScopedWithShellRoute<MapListPage, MapListPageVM>("MapList");
        services.AddScopedWithShellRoute<CameraPage, CameraPageVM>("Camera");
		services.AddScopedWithShellRoute<CheckPointPage, CheckPointPageVM>("CheckPoint");
        services.AddScopedWithShellRoute<CheckPointDetailPage, CheckPointDetailPageVM>("CheckPointDetailPage");
        services.AddScoped<EditView_IncludingImage>();
		services.AddTransientPopup<PunchListPopup, PunchListPopupVM>();
		services.AddScopedWithShellRoute<ImageViewPage, ImageViewPageVM>("ImageView");
		
		services.AddScoped<LoginOutPage, LoginOutPageVM>();
		services.AddScoped<LoginPage, LoginPageVM>();
		services.AddScoped<SystemOptionServerPage, SystemOptionServerPageVM>();
        services.AddScoped<RemotingFtpServerPage, RemotingFtpServerPageVM>();
        services.AddScoped<RemotingServerPage, RemotingServerPageVM>();
        services.AddScoped<DownloadPage, DownloadPageVM>();
		services.AddScoped<UploadPage, UploadPageVM>();
		services.AddScoped<LogPage, LogPageVM>();
		services.AddScoped<DeletePage, DeletePageVM>();
		services.AddScoped<SwitchThemePage, SwitchThemePageVM>();
		return services;
	}
}

