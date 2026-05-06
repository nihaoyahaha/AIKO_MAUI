using Aiko.Common;
using Aiko.Common.Models.Login;
using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class LoginPageVM :Observablebase<LoginPageVM, ILoginService>
{
    public LoginPageVM(ILogger<LoginPageVM> logger, ILoginService service) :base(logger,service)
    {
	}

    /// <summary>
    /// 会社ID
    /// </summary>
    [ObservableProperty]
    public partial string CompanyID { get; set; } = string.Empty;

    /// <summary>
    /// 会社名
    /// </summary>
    [ObservableProperty]
    public partial string CompanyName { get;set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    [ObservableProperty]
    public partial string UserID { get; set; } = string.Empty;

	/// <summary>
	/// 用户名
	/// </summary>
	[ObservableProperty]
    public partial string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 现场集合
    /// </summary>
    [ObservableProperty]
    public partial ObservableCollection<ListItem> Projects { get; set; } = new();

    /// <summary>
    /// 选择的现场索引
    /// </summary>
    [ObservableProperty]
    public partial int ProjectSelectedIndex { get; set; } = -1;

    /// <summary>
    /// 密码
    /// </summary>
    [ObservableProperty]
    public partial string Pwd { get; set; } = "";

	/// <summary>
	/// 是否记录会社ID、用户名ID
	/// </summary>
	[ObservableProperty]
    public partial bool IsSaveLoginInfo { get; set; } = true;

	/// <summary>
	/// 会社code
	/// </summary>
	private string companyCode;

	/// <summary>
	/// 用户code
	/// </summary>
	private string userCode;

	/// <summary>
	/// 页面加载
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task PageLoadedAsync()
	{
		try
		{
			Pwd = "";
			IsSaveLoginInfo = Preferences.Default.Get("SaveLoginFlag", false);
			if (IsSaveLoginInfo)
			{
				CompanyID = Preferences.Default.Get("CompanyID", "");
				await CompanyIDUnfocused();
				UserID = Preferences.Default.Get("UserID", "");
				await UserIDUnfocused();
			}
			Logger.LogInformation("appが正常に起動しました");
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());
		}
	}

	/// <summary>
	/// 会社IDがフォーカスを失う
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
    private async Task CompanyIDUnfocused()
    {
        var result = await Service.GetCompanyValue(CompanyID);
        CompanyName = result.companyName;
        companyCode = result.companyCode;
    }

	/// <summary>
	/// ユーザIDがフォーカスを失う
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
    private async Task UserIDUnfocused()
    {
        var result = await Service.GetProjects(UserID, companyCode);
        UserName = result.userName;
        userCode = result.userCode;
        Projects = result.projects;
        ProjectSelectedIndex = Projects.Count > 0 ? 0 : -1;
    }

	/// <summary>
	/// ログイン
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
    private async Task LoginAsync()
    {
        if (ProjectSelectedIndex < 0) return;

        CheckLoginDto checkLoginDto = new(
            UserCode: userCode,
            UserID:UserID,
            Pwd: Pwd,
            CompanyCode: companyCode,
            CompanyID:CompanyID,
            WorkName: Projects[ProjectSelectedIndex].DisplyName,
            WorkCode: Projects[ProjectSelectedIndex].Value,
            IsSaveLoginInfo: IsSaveLoginInfo);

        var result = await Service.CheckLoginAsync(checkLoginDto);
        if (result)
        {
            try
            {
				WeakReferenceMessenger.Default.Send("login", "LoginOrLogoutToken");
                await Shell.Current.GoToAsync("MapView?FromPage=LoginPage");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
			}         
        }
        else
        {
            Pwd = "";
            await Shell.Current.DisplayAlertAsync("Error", ErrorMessage.ERRORPOP("CM00003"), "OK");
        }
    }

    /// <summary>
    /// 退出应用
    /// </summary>
    [RelayCommand]
    private void CloseApp()
    {
        Application.Current.Quit();
    }

}

