using Aiko.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.IServices.IServices;

public interface IDeleteService: IServiceBase
{
	public Task<ObservableCollection<ListItem>> GetProjects();
}
