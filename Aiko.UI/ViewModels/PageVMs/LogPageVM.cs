using Aiko.Common;
using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class LogPageVM: Observablebase<LogPageVM,ILogService>
{
	public LogPageVM(ILogger<LogPageVM> logger, ILogService service) : base(logger, service)
	{
	}

	[ObservableProperty]
	private bool _isEnabled = true;

	[RelayCommand]
	private async Task UploadLog()
	{
		IsEnabled = false;
		string ErrMsg;
		string logDirectory = Path.Combine(FileSystem.AppDataDirectory, "Log");
		if (!Directory.Exists(logDirectory)) return;
		try
		{
			var result = await Service.UploadLogFileAsync();
			if (result)
			{
				ErrMsg = ErrorMessage.ERRORPOP("CM01030");
			}
			else
			{
				ErrMsg = ErrorMessage.ERRORPOP("CM01035");
			}
			DialogHelper.MessageDialogOk(ErrMsg);
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
}
