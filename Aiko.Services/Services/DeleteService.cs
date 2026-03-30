using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.Services.Services;

public class DeleteService : BaseService<DeleteService>, IDeleteService
{
	public DeleteService(ServiceContext<DeleteService> context) : base(context)
	{
	}

	public AikoAppContext AppContext => AikoAppContext;

	public async Task<ObservableCollection<ListItem>> GetProjects()
	{
		ObservableCollection<ListItem> projects = new();
		List<ListItem> list = new List<ListItem>();
		ListItem listItem;
		List<HM03PROJ> hm03List = await HkksDb.GetHM03PROJListAsync();
		List<HM15OSUB> hm15List = await HkksDb.GetHM15ListAsync(AikoAppContext.OperatorCD, AikoAppContext.CompanyID);

		listItem = new ListItem(" ", " ");
		projects.Add(listItem);
		for (int iHm15 = 0; iHm15 < hm15List.Count; iHm15++)
		{
			for (int iHm03 = 0; iHm03 < hm03List.Count; iHm03++)
			{
				if (hm15List[iHm15].HM15002 == hm03List[iHm03].HM03001)
				{
					listItem = new ListItem(hm03List[iHm03].HM03001 + "-" + hm03List[iHm03].HM03002, hm03List[iHm03].HM03001);
					projects.Add(listItem);
				}
			}
		}

		return projects;
	}
}
