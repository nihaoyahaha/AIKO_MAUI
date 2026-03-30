using Aiko.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.IServices.IServices
{
    public interface IDownloadService : IServiceBase
    {
		public Task<ObservableCollection<ListItem>> InitializeConstructionListAsync();

        public Task InitializeCommunicationServiceAsync();

		public Task DownLoadAsync(string workCode, bool isIncludeDrawingFile, bool isIncludePhotoFile, IProgress<DownloadProgressArgs> progress);
	}
}
