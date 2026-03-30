using Aiko.Common;
using Aiko.Common.Models.Login;
using System.Collections.ObjectModel;

namespace Aiko.IServices.IServices;

public interface ILoginService: IServiceBase
{
    public Task<bool> CheckLoginAsync(CheckLoginDto checkLoginDto);
    public Task<(string companyName, string companyCode)> GetCompanyValue(string companyID);
    public Task<(string userName, string userCode, ObservableCollection<ListItem> projects)> GetProjects(string userID, string companyCode);
}

