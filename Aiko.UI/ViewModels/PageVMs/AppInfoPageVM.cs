using Aiko.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;

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

	/// <summary>
	/// バージョン更新情報
	/// </summary>
	[ObservableProperty]
	private string _releaseNotes;

	public AppInfoPageVM(AikoAppContext appContext)
	{
		_appContext = appContext;
	}

	[RelayCommand]
	async private Task PageLoaded()
	{
		AppName = _appContext.AppName;
		AppVersion = $"バージョン {_appContext.AppVersion}";
		AppCopyright = _appContext.AppCopyright;
		AppCompany = _appContext.CompanyName;
		AppDescription = _appContext.Description;
		AppTrademark = _appContext.Trademark;

#if WINDOWS
        using var stream = Assembly.Load("Aiko.UI").GetManifestResourceStream("ReleaseNotes.txt");
        using var reader = new StreamReader(stream);
		ReleaseNotes = await reader.ReadToEndAsync();
#else
		using var stream = await FileSystem.OpenAppPackageFileAsync("ReleaseNotes.txt");
		using var reader = new StreamReader(stream);
		ReleaseNotes = await reader.ReadToEndAsync();
#endif

	}
}
