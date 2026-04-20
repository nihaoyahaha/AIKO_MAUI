using Aiko.Common;
using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class LogPageVM : Observablebase<LogPageVM, ILogService>
{
	public LogPageVM(ILogger<LogPageVM> logger, ILogService service) : base(logger, service)
	{
	}

	[ObservableProperty]
	private ObservableCollection<LogItem> _logFiles = new();

	[ObservableProperty]
	private bool _isEnabled = true;


	[RelayCommand]
	private void PageLoaded()
	{
		LogFiles = Service.LogFiles();
	}

	[RelayCommand]
	private void ToggleSelection(LogItem item)
	{
		if (item != null) item.IsSelected = !item.IsSelected;
	}

	[RelayCommand]
	private async Task UploadLog()
	{
		IsEnabled = false;
		string msg;
		try
		{
			if (!ValidateBeforeUpload())
			{
				DialogHelper.MessageDialogOk("対象のログファイルが選択されていません");
				return;
			}
			string[] logFilePaths = LogFiles.Where(f => f.IsSelected).Select(f => f.FilePath).ToArray();
			var result = await Service.UploadLogFileAsync(logFilePaths);
			msg = ErrorMessage.ERRORPOP(result ? "CM01030" : "CM01035");
			DialogHelper.MessageDialogOk(msg);
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
		finally
		{
			IsEnabled = true;
		}
	}

	bool ValidateBeforeUpload()
	{
		string logDirectory = Path.Combine(FileSystem.AppDataDirectory, "Log");
		if (!Directory.Exists(logDirectory))
		{
			return false;
		}
		else if (LogFiles.Count == 0)
		{
			return false;
		}
		else if (!LogFiles.Any(f => f.IsSelected))
		{
			return false;
		}
		return true;
	}
}
