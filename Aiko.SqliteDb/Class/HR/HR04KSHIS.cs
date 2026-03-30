using SQLite;

namespace Aiko.SqliteDb;

public class HR04KSHIS
{
	[PrimaryKey]
	/// <summary>
	/// 工事コード
	/// </summary>
	public string HR04001 { get; set; }

	[PrimaryKey]
	/// <summary>
	/// アイテムコード
	/// </summary>
	public string HR04002 { get; set; }

	[PrimaryKey]
	/// <summary>
	/// 確認項目コード
	/// </summary>
	public string HR04003 { get; set; }

	[PrimaryKey]
	/// <summary>
	///  履歴No
	/// </summary>
	public int HR04004 { get; set; }

    /// <summary>
    ///  値
    /// </summary>
    public string HR04005 { get; set; }

    /// <summary>
    /// 判定
    /// </summary>
    public int HR04006 { get; set; }

    /// <summary>
    /// 確認日
    /// </summary>
    public int HR04007 { get; set; }

    /// <summary>
    /// 確認オペレータ
    /// </summary>
    public string HR04008 { get; set; }

    /// <summary>
    /// 指摘日
    /// </summary>
    public int HR04009 { get; set; }

    /// <summary>
    /// 指摘オペレータ
    /// </summary>
    public string HR04010 { get; set; }

    /// <summary>
    /// メモ
    /// </summary>
    public string HR04011 { get; set; }

    /// <summary>
    /// 処理方法
    /// </summary>
    public string HR04012 { get; set; }

    /// <summary>
    /// 写真枚数
    /// </summary>
    public int HR04013 { get; set; }

    /// <summary>
    /// 確認方法
    /// </summary>
    public int HR04014 { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public string HR04015 { get; set; }

    /// <summary>
    /// 作成オペレータ
    /// </summary>
    public string HR04016 { get; set; }
}

