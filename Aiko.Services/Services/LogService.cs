using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Services.Services;

public class LogService : BaseService<LogService>, ILogService
{
	public LogService(ServiceContext<LogService> context) : base(context)
	{
	}

	public AikoAppContext AppContext => AikoAppContext;

	/// <summary>
	/// ログファイルをサーバにアップロードする
	/// </summary>
	/// <returns></returns>
	public async Task<bool> UploadLogFileAsync()
	{
		try
		{
			string logDirectory = Path.Combine(FileSystem.AppDataDirectory, "Log");
			var directory = new DirectoryInfo(logDirectory);

			var latestFile = directory.GetFiles("Log - *.log")
				.OrderByDescending(f => f.Name)
				.FirstOrDefault();
			if (latestFile == null)
			{
				Logger.LogWarning("ローカルにアップロード可能なログファイルがありません!");
				return false;
			}
			string remoteFileName = $"{DateTime.Now.ToString("yyyyMMdd")}_{DateTime.Now.ToString("hhmmssfff")}_MAUILog.txt";
			string remoteDirectory = "Log";

			var fileStream = await GetMemoryStreamAsync(latestFile.FullName);

			using CancellationTokenSource cts = new CancellationTokenSource(15000);
			bool result;
			if (AikoAppContext.FileServerType == 2)
			{
				//ファイルサーバタイプ:webdav
				result = await WebDavClient.UploadStreamAsync(fileStream, remoteDirectory, remoteFileName, ct: cts.Token);
			}
			else
			{
				//ファイルサーバタイプ:ftp
				string remotePath = $"/{AikoAppContext.HC01013}/{remoteDirectory}/{remoteFileName}";
				result = await FluentFtpClient.UploadStreamAsync(fileStream, remotePath, ct: cts.Token);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logger.LogError($"ログアップロードに失敗しました:{ex.ToString()}");
			return false;
		}
	}

	/// <summary>
	/// ファイルパスからメモリストリームを取得するには
	/// </summary>
	/// <param name="filePath">ファイルパス</param>
	/// <returns>メモリフロー</returns>
	async Task<MemoryStream> GetMemoryStreamAsync(string filePath)
	{
		var memoryStream = new MemoryStream();
		if (!File.Exists(filePath))
		{
			Logger.LogWarning($"ログファイルが存在しません:{filePath}");
			return memoryStream;
		}
		using (var fileStream = new FileStream(
			filePath,
			FileMode.Open,
			FileAccess.Read,
			FileShare.ReadWrite))
		{
			await fileStream.CopyToAsync(memoryStream);
		}
		memoryStream.Position = 0;
		return memoryStream;
	}
}
