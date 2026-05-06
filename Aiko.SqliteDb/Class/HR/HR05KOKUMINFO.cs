using SQLite;

namespace Aiko.SqliteDb;

public class HR05KOKUMINFO
{
	[PrimaryKey]
	/// <summary>
	/// 工事コード
	/// </summary>
	public string HR05001 { get; set; } = string.Empty;

    [PrimaryKey]
	/// <summary>
	/// アイテムコード
	/// </summary>
	public string HR05002 { get; set; } = string.Empty;

    /// <summary>
    /// 頂点No
    /// </summary>
    public int HR05003 { get; set; }

    /// <summary>
    /// X座標
    /// </summary>
    public int HR05004 { get; set; }

    /// <summary>
    /// Y座標
    /// </summary>
    public int HR05005 { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public string HR05006 { get; set; } = string.Empty;

    /// <summary>
    /// 作成オペレータ
    /// </summary>
    public string HR05007 { get; set; } = string.Empty;

    /// <summary>
    /// 更新日時
    /// </summary>
    public string HR05008 { get; set; } = string.Empty;

    /// <summary>
    /// 更新オペレータ
    /// </summary>
    public string HR05009 { get; set; } = string.Empty;
}

