
using System.Reflection;

namespace Aiko.Common;

/// <summary>
/// aikoアプリケーションコンテキスト
/// </summary>
public class AikoAppContext
{
	public static Assembly MainAssembly { get; set; }

	/// <summary>
	/// オペレータコード
	/// </summary>
	public string OperatorCD { get; set; } = string.Empty;

	/// <summary>
	/// オペレータ名
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// オペレータ権限
	/// </summary>
	public string PowerLevel { get; set; } = string.Empty;

	/// <summary>
	/// 工事コード
	/// </summary>
	public string WorkCD { get; set; } = string.Empty;

	/// <summary>
	/// 工事名(携帯工事コード)
	/// </summary>
	public string WorkName { get; set; } = string.Empty;

	/// <summary>
	/// 工事名(工事コードを携帯しない)
	/// </summary>
	public string WorkNameExcludeCode { get; set; } = string.Empty;

	/// <summary>
	/// オペレータID
	/// </summary>
	public string OperatorID { get; set; } = string.Empty;

	/// <summary>
	/// 所属会社コード
	/// </summary>
	public string CompanyID { get; set; } = string.Empty;

	/// <summary>
	/// 図面ファイルフォル
	/// </summary>
	public string HC01013 { get; set; } = string.Empty;

	/// <summary>
	/// ログインステータス
	/// </summary>
	public bool IsLogin { get; set; } = false;

	/// <summary>
	/// ファイルサーバタイプ 1:ftp、2:webdav
	/// </summary>
	public int FileServerType { get; set; } = 2;

	/// <summary>
	/// アプリケーションデータディレクトリ
	/// </summary>
	public string AppDataFoler
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(HC01013))
				return Path.Combine(FileSystem.AppDataDirectory, HC01013);
			else
				return FileSystem.AppDataDirectory;
		}
	}

	/// <summary>
	/// 工事現場フォルダ
	/// </summary>
	public string ConstructionSiteFolder
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(HC01013) && !string.IsNullOrWhiteSpace(WorkCD))
				return Path.Combine(FileSystem.AppDataDirectory, HC01013, WorkCD);
			else
				return FileSystem.AppDataDirectory;
		}
	}

	/// <summary>
	/// アプリケーションバージョンの取得
	/// </summary>
	public string AppVersion
	{
		get
		{
#if WINDOWS
			return AppInfo.Current.VersionString;
#elif IOS || MACCATALYST
			var version = AppInfo.Version;
			string buildVersion = AppInfo.Current.BuildString;
			return $"{version.Major}.{version.Minor}.{(version.Build == -1 ? "0" : version.Build)}.{buildVersion}";
#endif
		}
	}

	/// <summary>
	/// システム名の取得
	/// </summary>
	public string AppName
	{
		get
		{
			return AppInfo.Current.Name;
		}
	}

	/// <summary>
	/// 著作権表示
	/// </summary>
	public string AppCopyright
	{
		get 
		{
			return $"Copyright © {DateTime.Now.Year}";
		}
	}

	/// <summary>
	/// 会社
	/// </summary>
	public string CompanyName 
	{
		get 
		{
			return MainAssembly?
				.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Unknown";
		}
	}

	/// <summary>
	/// 商標
	/// </summary>
	public string Trademark
	{
		get 
		{
			return MainAssembly?
				.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark ?? "Unknown";
		}
	}

	/// <summary>
	/// 記述
	/// </summary>
	public string Description 
	{
		get 
		{
			return MainAssembly?
				.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "Unknown";
		}
	}

	/// <summary>
	/// 現在工事現場の撮影方向を取得
	/// </summary>
	public List<string> DirectionList 
	{
		get 
		{
			if(string.IsNullOrWhiteSpace(WorkCD)) return new List<string>();

			string prefDirsListStr = Preferences.Get($"DirsList-{WorkCD}", "");
			return prefDirsListStr.Split(',')
				.Where(s => !string.IsNullOrWhiteSpace(s))
				.ToList();
		}
	}

	/// <summary>
	/// 現在工事現場の撮影方向を追加
	/// </summary>
	/// <param name="directionText">撮影方向</param>
	public void AddPreferencesDirection(string directionText)
	{
		if (string.IsNullOrWhiteSpace(WorkCD)) return;
		if(string.IsNullOrWhiteSpace(directionText)) return;

		string key = $"DirsList-{WorkCD}";
		List<string> directions = DirectionList;
		directions.Add(directionText);

		Preferences.Set(key, string.Join(",", directions));
	}

}

