using Aiko.Common;
using Aiko.IServices.IServices;

namespace Aiko.UI;

public partial class App : Application
{
	readonly IAppInitializationService _appInitializationService;
	public App(IAppInitializationService appInitializationService)
	{
		InitializeComponent();
		_appInitializationService = appInitializationService;
	}

	protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
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
		string theme = Preferences.Default.Get("Theme", "Light");
		ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
		if (mergedDictionaries != null)
		{
			mergedDictionaries.Clear();
			switch (theme)
			{
				case "Dark":
					mergedDictionaries.Add(new DarkTheme());
					break;
				case "Light":
				default:
					mergedDictionaries.Add(new LightTheme());
					break;
			}
		}
	}

}

