using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Aiko.Common;
/// <summary>
/// データ同期時間を設定または取得するためのヘルプクラス
/// </summary>
public class DownLoadTimeUtils
{
	private readonly ILogger<DownLoadTimeUtils> _logger;

	public DownLoadTimeUtils(ILogger<DownLoadTimeUtils> logger)
	{
		_logger = logger;
	}

	private string FilePath => Path.Combine(FileSystem.AppDataDirectory, "settingDownLoadTime.json");

	public List<DownLoadTime> GetDownLoadTimeList()
	{
		try
		{
			if (!File.Exists(FilePath)) return new List<DownLoadTime>();

			string json = File.ReadAllText(FilePath);

			return JsonSerializer.Deserialize(json, DownLoadTimeContext.Default.ListDownLoadTime)
				   ?? new List<DownLoadTime>();
		}
		catch (Exception ex)
		{
			_logger.LogError($"[Utils]JSONの読み取りに失敗しました:{ex.Message}");
			return new List<DownLoadTime>();
		}
	}

	public string GetDownLoadTime(string projectCode, string tableName)
	{
		var list = GetDownLoadTimeList();
		return list.FirstOrDefault(x => x.ProjectCode == projectCode && x.TableName == tableName)?.Time ?? "";
	}

	public async Task SaveTimesAsync(string projectCode, List<string> tableNames, string timeStr)
	{
		var dataList = GetDownLoadTimeList();

		foreach (var table in tableNames)
		{
			dataList.RemoveAll(x => x.ProjectCode == projectCode && x.TableName == table);
			dataList.Add(new DownLoadTime
			{
				ProjectCode = projectCode,
				TableName = table,
				Time = timeStr
			});
		}

		try
		{
			string json = JsonSerializer.Serialize(dataList, DownLoadTimeContext.Default.ListDownLoadTime);
			await File.WriteAllTextAsync(FilePath, json);
		}
		catch (Exception ex)
		{
			_logger.LogError($"[Utils]JSONの保存に失敗しました:{ex.Message}");
		}
	}
}
