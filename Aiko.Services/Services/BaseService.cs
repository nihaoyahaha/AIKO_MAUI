using Aiko.Common;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.Services.Services;

public abstract class BaseService<TService> where TService : class
{
	/// <summary>
	/// サービス集約コンテキスト
	/// </summary>
	private readonly ServiceContext<TService> _serviceContext;

	/// <summary>
	/// アプリケーションのローカル SQLite データベースへのアクセスを提供するデータアクセス層インスタンス。
	/// </summary>
	protected HkksDatabase HkksDb => _serviceContext.HkksDb;

    protected AikoWcf AikoWcf => _serviceContext.AikoWcf;

    /// <summary>
    /// サービスの操作ログおよびエラーログを記録するロガー。
    /// </summary>
    protected ILogger<TService> Logger => _serviceContext.Logger;

	/// <summary>
	/// aikoアプリケーションコンテキスト
	/// </summary>
	protected AikoAppContext AikoAppContext => _serviceContext.AikoAppContext;

	/// <summary>
	/// ftpファイル転送クライアント
	/// </summary>
	protected FluentFtpClient FluentFtpClient => _serviceContext.FluentFtpClient;

	/// <summary>
	/// webdavファイル転送クライアント
	/// </summary>
	protected WebDavClient WebDavClient => _serviceContext.WebDavClient;

	/// <summary>
	/// データ同期時間
	/// </summary>
	protected DownLoadTimeUtils DownLoadTimeUtils => _serviceContext.DownLoadTimeUtils;

	protected BaseService(ServiceContext<TService> context)
	{
		_serviceContext = context;
	}
}
