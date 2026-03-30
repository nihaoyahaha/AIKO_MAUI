using Aiko.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.IServices.IServices;

public interface IUploadService:IServiceBase
{
	public string GetSystemDateTime();
	public Task UploadDataAsync(IProgress<DownloadProgressArgs> progress);
	public Task UploadFileAsync(IProgress<DownloadProgressArgs> progress,bool checkDrawing);
}
