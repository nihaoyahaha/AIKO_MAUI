using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.UI.Themes;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Animations;

namespace Aiko.UI;

public partial class App : Application
{
	readonly ILogger<App> _logger;
	readonly IAppInitializationService _appInitializationService;
	readonly AppShell _shell;
	public App(IAppInitializationService appInitializationService, 
		AppShell shell, 
		ILogger<App> logger)
	{
		InitializeComponent();
		Current.UserAppTheme = AppTheme.Light;
		_appInitializationService = appInitializationService;
		_shell = shell;
		_logger = logger;
		AppDomain.CurrentDomain.UnhandledException += (s, e) =>
		{
			var exception = e.ExceptionObject as Exception;
			_logger.LogCritical($"appに未処理異常が発生しました:{exception.ToString()}");
		};
	}

	protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_shell);
    }

    protected override async void OnStart()
    {
        base.OnStart();
		InitializeTheme();
		await _appInitializationService.InitializeAppCommunicationServiceFromSqliteAsync();
	}

	/// <summary>
	/// トピックの初期化
	/// </summary>
	void InitializeTheme()
	{
		ThemeManager.ApplySavedTheme(notify: false);
	}

}

