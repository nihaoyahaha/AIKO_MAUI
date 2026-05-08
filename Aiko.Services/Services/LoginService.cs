using Aiko.SqliteDb;
using Aiko.IServices.IServices;
using System.Collections.ObjectModel;
using Aiko.Common;
using Microsoft.Extensions.Logging;
using Aiko.Services.Services;
using Aiko.Common.Models.Login;

namespace Aiko.Services.Services;

public class LoginService :BaseService<LoginService>, ILoginService
{
	public LoginService(ServiceContext<LoginService> context)
		: base(context)
	{
	}

	/// <summary>
	/// aikoApp実行中のアプリケーションコンテキスト
	/// </summary>
	public AikoAppContext AppContext => AikoAppContext;

	/// <summary>
	/// 工事コードと工事名のマッピング
	/// </summary>
	private Dictionary<string, string> _projectsCache = new();

	async public Task<(string companyName, string companyCode)> GetCompanyValue(string companyID)
    {
        try
        {
            var data = await HkksDb.Db.Table<HM01KAIS>()
          .Where(hm01 => hm01.HM01016 == companyID)
          .ToListAsync();
            if (data is not null && data.Count() > 0)
            {
                return (data[0].HM01002.Trim(), data[0].HM01001);
            }
            else
            {
                return ("", "");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
            return ("", "");
        }
    }

    async public Task<(string userName, string userCode, ObservableCollection<ListItem> projects)> GetProjects(string userID, string companyCode)
    {
        string userName = "";
        string userCode = "";
        ObservableCollection<ListItem> items = new();
        _projectsCache = new();
		HM02OPER hm02 = new();
        try
        {
            hm02.HM02004 = companyCode;
            hm02.HM02015 = userID;
            var hm02pers = await HkksDb.GetHM02OPERcodeListAAsync(hm02);

            if (hm02pers.Count > 0)
            {
                userCode = hm02pers[0].HM02001;
                userName = hm02pers[0].HM02002;
                if (hm02pers[0].HM02005 != 3 && hm02pers[0].HM02005 != 4)
                {
                    HM15OSUB hm15 = new();
                    hm15.HM15001 = userCode;
                    hm15.HM15009 = companyCode;
                    var hm15OSUBs = await HkksDb.GetHM15OSUBcodeListAAsync(hm15);
                    HM03PROJ hm03 = new();
                    foreach (var item in hm15OSUBs)
                    {
                        hm03.HM03001 = item.HM15002;
                        var hm03PROJs = await HkksDb.GetHM03PROJcodeListAsync(hm03);
                        foreach (var obj in hm03PROJs)
                        {
                            items.Add(new ListItem(obj.HM03001 + "-" + obj.HM03002.Trim(), obj.HM03001));
                            _projectsCache.Add(obj.HM03001, obj.HM03002.Trim());
                        }
                    }
                }
            }
            return (userName, userCode, items);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
            return (userName, userCode, items);
        }
    }

    async public Task<bool> CheckLoginAsync(CheckLoginDto checkLoginDto)
    {
        HM02OPER hm02 = new();
        hm02.HM02001 = checkLoginDto.UserCode;
        hm02.HM02003 = checkLoginDto.Pwd;
        hm02.HM02004 = checkLoginDto.CompanyCode;
        try
        {
            var hm02OPERs = await HkksDb.GetHM02OPERcodeListAsync(hm02);
            if (hm02OPERs.Count() <= 0)
            {
                return false;
            }
            else
            {
                AikoAppContext.OperatorCD = checkLoginDto.UserCode.Trim();
                AikoAppContext.Name = hm02OPERs[0].HM02002.Trim();
                AikoAppContext.WorkCD = checkLoginDto.WorkCode.Trim();
                AikoAppContext.WorkName = checkLoginDto.WorkName.Trim();
                AikoAppContext.WorkNameExcludeCode = _projectsCache[AikoAppContext.WorkCD];
                AikoAppContext.PowerLevel = hm02OPERs[0].HM02005.ToString();
                AikoAppContext.OperatorID = hm02OPERs[0].HM02015.Trim();
				AikoAppContext.CompanyID = hm02OPERs[0].HM02004.Trim();
				AikoAppContext.IsLogin = true;
             
				//将公司ID和用户ID保存到首选项
				Preferences.Default.Set("SaveLoginFlag", checkLoginDto.IsSaveLoginInfo);
                Preferences.Default.Set("CompanyID", checkLoginDto.CompanyID);
                Preferences.Default.Set("UserID", checkLoginDto.UserID);
				return true;
            }  
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
            return false;
        }
    }
}

