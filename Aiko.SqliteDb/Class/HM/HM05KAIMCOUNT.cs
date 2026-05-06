namespace Aiko.SqliteDb;

public class HM05KAIMCOUNT
{
    /// <summary>
    /// 工事コード
    /// </summary>
    public string HM05001 { get; set; } = string.Empty;

    /// <summary>
    /// 階コード
    /// </summary>
    public string HM05002 { get; set; } = string.Empty;

    /// <summary>
    /// 階名
    /// </summary>
    public string HM05003 { get; set; } = string.Empty;

    /// <summary>
    /// 並び順
    /// </summary>
    public string HM05004 { get; set; } = string.Empty;

    /// <summary>
    /// 非表示フラグ
    /// </summary>
    public int HM05005 { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public string HM05006 { get; set; } = string.Empty;

    /// <summary>
    /// 作成オペレータ
    /// </summary>
    public string HM05007 { get; set; } = string.Empty;

    /// <summary>
    /// 更新日時
    /// </summary>
    public string HM05008 { get; set; } = string.Empty;

    /// <summary>
    /// 更新オペレータ
    /// </summary>
    public string HM05009 { get; set; } = string.Empty;

    /// <summary>
    /// 同期日時
    /// </summary>
    public string HM05010 { get; set; } = string.Empty;

    /// <summary>
    /// 同期オペレータ
    /// </summary>
    public string HM05011 { get; set; } = string.Empty;

    /// <summary>
    /// 階関連MAPカウント
    /// </summary>
    public int MapCount { get; set; }
}

