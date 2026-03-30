namespace Aiko.Common.Models.Login;

/// <summary>
/// loginPageVM 往 LoginService.CheckLoginAsync 传递的参数
/// </summary>
/// <param name="UserCode">用户code</param>
/// <param name="UserID">用户ID</param>
/// <param name="Pwd">密码</param>
/// <param name="CompanyCode">公司code</param>
/// <param name="CompanyID">公司ID</param>
/// <param name="WorkName">现场名</param>
/// <param name="WorkCode">现场code</param>
/// <param name="IsSaveLoginInfo">是否保存登录信息</param>
public record CheckLoginDto(string UserCode,string UserID ,string Pwd, string CompanyCode,string CompanyID ,string WorkName, string WorkCode, bool IsSaveLoginInfo);

