using Aiko.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.IServices.IServices;

public interface ILogService : IServiceBase
{
	public Task<bool> UploadLogFileAsync();
}
