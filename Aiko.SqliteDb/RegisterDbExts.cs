using Aiko.SqliteDb;
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
	public static IServiceCollection AddData(this IServiceCollection service)
	{
		service.AddSingleton<HkksDatabase>();
		service.AddSingleton<WebDavClient>();
		service.AddSingleton<FluentFtpClient>();

		return service;
	}
}

