using Aiko.Common;
using Aiko.Common.Models.Login;
using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

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
    private string companyID ;

    /// <summary>
    /// 会社名
    /// </summary>
    [ObservableProperty]
    private string companyName;

    /// <summary>
    /// 会社code
    /// </summary>
    private string companyCode;

    /// <summary>
    /// 用户ID
    /// </summary>
    [ObservableProperty]
    private string userID;

    /// <summary>
    /// 用户名
    /// </summary>
    [ObservableProperty]
    private string userName;

    /// <summary>
    /// 用户code
    /// </summary>
    private string userCode;

    /// <summary>
    /// 现场集合
    /// </summary>
    [ObservableProperty]
    ObservableCollection<ListItem> projects = new();

    /// <summary>
    /// 选择的现场索引
    /// </summary>
    [ObservableProperty]
    [CustomValidation(typeof(LoginPageVM), nameof(ValidateProjectSelectedIndex))]
    private int projectSelectedIndex = -1;

    /// <summary>
    /// 密码
    /// </summary>
    [ObservableProperty]
    [Required]
    private string pwd ="";

	/// <summary>
	/// 是否记录会社ID、用户名ID
	/// </summary>
	[ObservableProperty]
    private bool isSaveLoginInfo = true;

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
        ValidateAllProperties();
        if (HasErrors)
        {
            return;
        }
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
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
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

    /// <summary>
    /// 校验是否选择现场
    /// </summary>
    /// <param name="index">被验证的字段(ProjectSelectedIndex)</param>
    /// <param name="context">验证上下文</param>
    /// <returns></returns>
    public static ValidationResult ValidateProjectSelectedIndex(int index, ValidationContext context)
    {
        if (index != -1)
        {
            return ValidationResult.Success;
        }
        else
        {
            return new("未选择现场");
        }
    }

}

