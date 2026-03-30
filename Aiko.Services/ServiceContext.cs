using Aiko.Common;
using Aiko.Services.Services;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Services;

/// <summary>
/// データベース、ログ、通信プロトコルスタックなど、すべてのサービスに共通して必要な下位コンポーネントを統合した統合コンテナ
/// </summary>
/// <typeparam name="TService"></typeparam>
public class ServiceContext<TService> where TService : class
{
	/// <summary>
	/// アプリケーションのローカル SQLite データベースへのアクセスを提供するデータアクセス層インスタンス。
	/// </summary>
	public readonly HkksDatabase HkksDb;

	/// <summary>
	/// wcf通信クライアント
	/// </summary>
	public readonly AikoWcf AikoWcf;

	/// <summary>
	/// サービスの操作ログおよびエラーログを記録するロガー。
	/// </summary>
	public readonly ILogger<TService> Logger;

	/// <summary>
	/// aikoアプリケーションコンテキスト
	/// </summary>
	public readonly AikoAppContext AikoAppContext;

	/// <summary>
	/// ftpファイル転送クライアント
	/// </summary>
	public readonly FluentFtpClient FluentFtpClient;

	/// <summary>
	/// webdavファイル転送クライアント
	/// </summary>
	public readonly WebDavClient WebDavClient;

	/// <summary>
	/// データ同期時間
	/// </summary>
	public readonly DownLoadTimeUtils DownLoadTimeUtils;

	public ServiceContext(HkksDatabase hkksDb,
		ILogger<TService> logger,
		AikoAppContext appContext,
		AikoWcf aikoWcf,
		FluentFtpClient fluentFtpClient,
		WebDavClient webDavClient,
		DownLoadTimeUtils downLoadTimeUtils)
	{
		HkksDb = hkksDb;
		Logger = logger;
		AikoAppContext = appContext;
		AikoWcf = aikoWcf;
		FluentFtpClient = fluentFtpClient;
		WebDavClient = webDavClient;
		DownLoadTimeUtils = downLoadTimeUtils;
	}
}
