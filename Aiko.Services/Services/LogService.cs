using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.Services.Services;

public class LogService : BaseService<LogService>, ILogService
{
	public LogService(ServiceContext<LogService> context) : base(context)
	{
	}

	public AikoAppContext AppContext => AikoAppContext;

	/// <summary>
	/// ログファイルの取得
	/// </summary>
	/// <returns></returns>
	public ObservableCollection<LogItem> LogFiles()
	{
		ObservableCollection<LogItem> logFiles = new();
		string logDirectory = Path.Combine(FileSystem.AppDataDirectory, "Log");
		if (!Directory.Exists(logDirectory))
		{
			return logFiles;
		}
		var directory = new DirectoryInfo(logDirectory);
		var files = directory.GetFiles("Log - *.log")
			.OrderByDescending(f => f.LastWriteTime)
			.Take(5)
			.Select(f => new LogItem
			{
				FileName = f.Name,
				FilePath = f.FullName,
				FileSize = $"{f.Length / 1024.0:F2} KB",
				CreatedDate = f.CreationTime,
				IsSelected = false
			})
			.ToList();
		foreach (var file in files)
		{
			logFiles.Add(file);
		}
		return logFiles;
	}


	/// <summary>
	/// ログファイルをサーバにアップロードする
	/// </summary>
	/// <returns></returns>
	public async Task<bool> UploadLogFileAsync(IEnumerable<string> logItems)
	{
		try
		{
			string remoteFileName = $"{DateTime.Now.ToString("yyyyMMdd")}_{DateTime.Now.ToString("hhmmssfff")}_MAUILog.txt";
			string remoteDirectory = "Log";

			var fileStream = await GetMemoryStreamAsync(logItems);

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
	async Task<MemoryStream> GetMemoryStreamAsync(IEnumerable<string> filePaths )
	{
		var memoryStream = new MemoryStream();
		var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
		try
		{
			foreach (string filePath in filePaths)
			{
				if (!File.Exists(filePath))
				{
					Logger.LogWarning($"ログファイルが存在しません:{filePath}");
					continue;
				}
				using (var fileStream = new FileStream(
					filePath,
					FileMode.Open,
					FileAccess.Read,
					FileShare.ReadWrite))
				{
					await fileStream.CopyToAsync(memoryStream);
				}
				await writer.WriteLineAsync();
				await writer.FlushAsync();
			}
		}
		catch (Exception ex)
		{
			Logger.LogError($"メモリストリームの取得に失敗しました:{ex.ToString()}");
			throw;
		}
		memoryStream.Position = 0;
		return memoryStream;
	}
}
