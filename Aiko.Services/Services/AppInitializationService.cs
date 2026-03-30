using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using ExCSS;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Services.Services;

public class AppInitializationService : IAppInitializationService
{
	readonly HkksDatabase _hkksDb;
	readonly WebDavClient _webDavClient;
	readonly FluentFtpClient _ftpClient;
	readonly AikoAppContext _appContext;
	readonly ILogger<AppInitializationService> _logger;

	public AppInitializationService(
		HkksDatabase database,
		WebDavClient webDavClient,
		ILogger<AppInitializationService> logger,
		FluentFtpClient ftpClient,
		AikoAppContext appContext)
	{
		_hkksDb = database;
		_webDavClient = webDavClient;
		_logger = logger;
		_ftpClient = ftpClient;
		_appContext = appContext;
	}

	/// <summary>
	/// sqliteデータベースから通信設定を取得するには
	/// </summary>
	/// <returns></returns>
	public async Task<bool> InitializeAppCommunicationServiceFromSqliteAsync()
	{
		try
		{
			await _hkksDb.InitAsync();
			var hc01 = await _hkksDb.GetHC01ListAsync(null);
			if (string.IsNullOrWhiteSpace(hc01.HC01010)) return false;
			InitializeWebDavClient(hc01);
			InitializeFluentFtpClient(hc01);
			_appContext.HC01013 = hc01.HC01013.Trim();
			_appContext.FileServerType = hc01.HC01009 == 2 ? 2 : 1;
			return true;
		}
		catch (System.Exception ex)
		{
			_logger.LogError(ex.ToString());
			return false;
		}
	}

	/// <summary>
	/// WebDavクライアントの初期化
	/// </summary>
	/// <returns></returns>
	void InitializeWebDavClient(HC01CONT hc01)
	{
		WebDavOptions webDavOptions = new WebDavOptions();
		string protocol = hc01.HC01021 == 1 ? "https" : "http";
		webDavOptions.BaseUrl = $"{protocol}://{hc01.HC01010.TrimEnd()}:{hc01.HC01020}/{hc01.HC01013.TrimEnd()}/";
		webDavOptions.Username = hc01.HC01011.TrimEnd();
		webDavOptions.Password = hc01.HC01012.TrimEnd();
		_webDavClient.InitializeWebDavClient(webDavOptions);
	}

	/// <summary>
	/// FluentFtpクライアントの初期化
	/// </summary>
	/// <param name="hc01"></param>
	/// <returns></returns>
	void InitializeFluentFtpClient(HC01CONT hc01)
	{
		FluentFtpOptions options = new FluentFtpOptions();
		options.Host = hc01.HC01010.TrimEnd();
		options.Port = hc01.HC01020;
		options.Username = hc01.HC01011.TrimEnd();
		options.Password = hc01.HC01012.TrimEnd();
		options.UseSsl = hc01.HC01021 == 1;
		_ftpClient.InitializeFluentFtpClient(options);
	}

}
