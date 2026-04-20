using Aiko.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.IServices.IServices;

public interface ILogService : IServiceBase
{
	public ObservableCollection<LogItem> LogFiles();
	public Task<bool> UploadLogFileAsync(IEnumerable<string> logItems);
}
