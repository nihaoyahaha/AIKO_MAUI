using SQLite;

namespace Aiko.SqliteDb;

public class HR02KSKK
{
	[PrimaryKey]
	/// <summary>
	/// 工事コード
	/// </summary>
	public string HR02001 { get; set; }

	[PrimaryKey]
	/// <summary>
	/// アイテムコード
	/// </summary>
	public string HR02002 { get; set; }

	[PrimaryKey]
	/// <summary>
	/// 確認項目コード
	/// </summary>
	public string HR02003 { get; set; }

    /// <summary>
    /// 値
    /// </summary>
    public string HR02004 { get; set; }

    /// <summary>
    /// 判定
    /// </summary>
    public int HR02005 { get; set; }

    /// <summary>
    /// 確認日
    /// </summary>
    public int HR02006 { get; set; }

    /// <summary>
    /// 確認オペレータ
    /// </summary>
    public string HR02007 { get; set; }

    /// <summary>
    /// 指摘日
    /// </summary>
    public int HR02008 { get; set; }

    /// <summary>
    /// 指摘オペレータ
    /// </summary>
    public string HR02009 { get; set; }

    /// <summary>
    /// メモ
    /// </summary>
    public string HR02010 { get; set; }

    /// <summary>
    /// 備考
    /// </summary>
    public string HR02011 { get; set; }

    /// <summary>
    /// 写真枚数
    /// </summary>
    public int HR02012 { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public string HR02013 { get; set; }

    /// <summary>
    /// 作成オペレータ
    /// </summary>
    public string HR02014 { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public string HR02015 { get; set; }

    /// <summary>
    /// 更新オペレータ
    /// </summary>
    public string HR02016 { get; set; }

    /// <summary>
    /// 同期日時
    /// </summary>
    public string HR02017 { get; set; }

    /// <summary>
    /// 同期オペレータ
    /// </summary>
    public string HR02018 { get; set; }

    /// <summary>
    /// 確認方法
    /// </summary>
    public int HR02019 { get; set; }
}

