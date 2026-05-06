using CommunityToolkit.Mvvm.Messaging;

namespace Aiko.UI.Themes;

/// <summary>
/// 统一管理应用主题的读取、切换与广播。
/// 页面或 ViewModel 不需要自己操作 ResourceDictionary，统一走这里即可。
/// </summary>
public static class ThemeManager
{
    /// <summary>
    /// 主题切换完成后发送的消息令牌。
    /// 已经打开的页面可以监听这个消息，主动刷新依赖主题的局部 UI。
    /// </summary>
    public const string ThemeChangedToken = "ThemeChangedToken";

    /// <summary>
    /// 读取本地保存的主题名称，并兜底修正为 Light / Dark 两种合法值。
    /// </summary>
    /// <returns></returns>
    public static string GetSavedTheme()
    {
        string theme = Preferences.Default.Get("Theme", "Light");
        return NormalizeTheme(theme);
    }

    /// <summary>
    /// 把已保存的主题转换成页面开关更容易使用的布尔值。
    /// true 表示 Dark，false 表示 Light。
    /// </summary>
    /// <returns></returns>
    public static bool GetSavedThemeFlag()
    {
        return string.Equals(GetSavedTheme(), "Dark", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 按当前本地保存的主题应用资源字典。
    /// 常用于应用启动时恢复上次主题。
    /// </summary>
    /// <param name="notify"></param>
    /// <returns></returns>
    public static bool ApplySavedTheme(bool notify = false)
    {
        return ApplyTheme(GetSavedTheme(), notify);
    }

    /// <summary>
    /// 按布尔开关切换主题。
    /// 主要给页面上的 Switch 之类的开关控件使用。
    /// </summary>
    /// <param name="useDarkTheme"></param>
    /// <param name="notify"></param>
    /// <returns></returns>
    public static bool ApplyThemeByFlag(bool useDarkTheme, bool notify = true)
    {
        return ApplyTheme(useDarkTheme ? "Dark" : "Light", notify);
    }

    /// <summary>
    /// 真正执行主题切换的核心入口。
    /// 会替换当前合并的主题资源字典、写回本地偏好，并按需广播主题变更消息。
    /// </summary>
    /// <param name="theme"></param>
    /// <param name="notify"></param>
    /// <returns></returns>
    public static bool ApplyTheme(string theme, bool notify = true)
    {
        string normalizedTheme = NormalizeTheme(theme);
        ICollection<ResourceDictionary>? mergedDictionaries = Application.Current?.Resources?.MergedDictionaries;

        if (mergedDictionaries == null)
        {
            return false;
        }

        mergedDictionaries.Clear();
        switch (normalizedTheme)
        {
            case "Dark":
                // 深色主题资源覆盖当前应用资源。
                mergedDictionaries.Add(new DarkTheme());
                break;
            case "Light":
            default:
                // 浅色主题作为默认主题。
                mergedDictionaries.Add(new LightTheme());
                break;
        }

        // 持久化主题名称，方便应用下次启动时恢复。
        Preferences.Default.Set("Theme", normalizedTheme);

        if (notify)
        {
            // 通知已打开页面刷新那些不会自动跟随 DynamicResource 更新的局部 UI。
            WeakReferenceMessenger.Default.Send(string.Empty, ThemeChangedToken);
        }

        return true;
    }

    /// <summary>
    /// 规范化主题名称，避免外部传入非法值时把偏好写乱。
    /// 当前只允许 Dark 和 Light，其他一律回退到 Light。
    /// </summary>
    /// <param name="theme"></param>
    /// <returns></returns>
    private static string NormalizeTheme(string? theme)
    {
        return string.Equals(theme, "Dark", StringComparison.OrdinalIgnoreCase)
            ? "Dark"
            : "Light";
    }
}
