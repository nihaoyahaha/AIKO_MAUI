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
	public partial string AppName { get; set; } = string.Empty;

	/// <summary>
	/// appバージョン
	/// </summary>
	[ObservableProperty]
	public partial string AppVersion { get; set; } = string.Empty;

	/// <summary>
	/// app著作権表示
	/// </summary>
	[ObservableProperty]
	public partial string AppCopyright { get; set; } = string.Empty;

	/// <summary>
	/// app会社
	/// </summary>
	[ObservableProperty]
	public partial string AppCompany { get; set; } = string.Empty;

	/// <summary>
	/// app記述
	/// </summary>
	[ObservableProperty]
	public partial string AppDescription { get; set; } = string.Empty;

	/// <summary>
	/// app商標
	/// </summary>
	[ObservableProperty]
	public partial string AppTrademark { get; set; } = string.Empty;

	/// <summary>
	/// バージョン更新情報
	/// </summary>
	[ObservableProperty]
	public partial string ReleaseNotes { get; set; } = string.Empty;

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
