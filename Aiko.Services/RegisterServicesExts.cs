using Aiko.Common;
using Aiko.IServices;
using Aiko.IServices.IServices;
using Aiko.Services;
using Aiko.Services.Services;
using Aiko.SqliteDb;

namespace Microsoft.Extensions.DependencyInjection;

public static class RegisterServicesExts
{
	public static IServiceCollection AddServices(this IServiceCollection services) {
        services.AddData();
        services.AddSingleton<AikoWcf>();
        services.AddSingleton<AikoAppContext>();
	    services.AddSingleton<DownLoadTimeUtils>();
		
		services.AddScoped(typeof(ServiceContext<>));
		services.AddScoped<ILoginService,LoginService>();
		services.AddScoped<IMapViewService, MapViewService>();
        services.AddScoped<IMapListService, MapListService>();
        services.AddScoped<ICameraService, CameraService>();
		services.AddScoped<ICheckPointService, CheckPointService>();
        services.AddScoped<ICheckPointDetailService, CheckPointDetailService>();
        services.AddScoped<IImageViewService,ImageViewService>();
        services.AddScoped<IRemotingService, RemotingService>();
		services.AddScoped<IDownloadService, DownloadService>();
		services.AddScoped<ILoginOutService, LoginOutService>();
		services.AddScoped<IAppInitializationService, AppInitializationService>();
		services.AddScoped<IUploadService, UploadService>();
		services.AddScoped<ILogService, LogService>();
		services.AddScoped<IDeleteService, DeleteService>();
		services.AddScoped<DataSyncService>();
		return services;
	}
}

