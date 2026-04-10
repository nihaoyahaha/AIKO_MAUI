using Aiko.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class AppInfoPageVM:ObservableValidator 
{
	readonly AikoAppContext _appContext;

	/// <summary>
	/// app名
	/// </summary>
	[ObservableProperty]
	private string _appName;

	/// <summary>
	/// appバージョン
	/// </summary>
	[ObservableProperty]
	private string _appVersion;

	/// <summary>
	/// app著作権表示
	/// </summary>
	[ObservableProperty]
	private string _appCopyright;

	/// <summary>
	/// app会社
	/// </summary>
	[ObservableProperty]
	private string _appCompany;

	/// <summary>
	/// app記述
	/// </summary>
	[ObservableProperty]
	private string _appDescription;

	/// <summary>
	/// app商標
	/// </summary>
	[ObservableProperty]
	private string _appTrademark;

	public AppInfoPageVM(AikoAppContext appContext)
	{
		_appContext = appContext;
	}

	[RelayCommand]
	private void PageLoaded()
	{
		AppName = _appContext.AppName;
		AppVersion = $"バージョン {_appContext.AppVersion}";
		AppCopyright = _appContext.AppCopyright;
		AppCompany = _appContext.CompanyName;
		AppDescription = _appContext.Description;
		AppTrademark = _appContext.Trademark;
	}
}
