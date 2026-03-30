namespace Aiko.SqliteDb;

public class HM02OPER
{
    /// <summary>
    /// オペレータコード
    /// </summary>
    public string HM02001 { get; set; }

    /// <summary>
    /// オペレータ名称
    /// </summary>
    public string HM02002 { get; set; }

    /// <summary>
    /// パスワード
    /// </summary>
    public string HM02003 { get; set; }

    /// <summary>
    /// 会社コード
    /// </summary>
    public string HM02004 { get; set; }

    /// <summary>
    /// 種別
    /// </summary>
    public int HM02005 { get; set; }

    /// <summary>
    /// 在職状態
    /// </summary>
    public int HM02006 { get; set; }

    /// <summary>
    /// 並び順
    /// </summary>
    public string HM02007 { get; set; }

    /// <summary>
    /// 最終ログイン日時
    /// </summary>
    public string HM02008 { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public string HM02009 { get; set; }

    /// <summary>
    /// 作成オペレータ
    /// </summary>
    public string HM02010 { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public string HM02011 { get; set; }

    /// <summary>
    /// 更新オペレータ
    /// </summary>
    public string HM02012 { get; set; }

    /// <summary>
    /// 同期日時
    /// </summary>
    public string HM02013 { get; set; }

    /// <summary>
    /// 同期オペレータ
    /// </summary>
    public string HM02014 { get; set; }

    /// <summary>
    /// オペレータID
    /// </summary>
    public string HM02015 { get; set; }

	/// <summary>
	/// ユーザ別IP制限 0:無効　1:有効
	/// </summary>
	public int HM02017 { get; set; } = 0;

	/// <summary>
	/// ユーザ別端末制限 0:無効　1:有効
	/// </summary>
	public int HM02018 { get; set; } = 0;

	/// <summary>
	/// 自働取得期間
	/// </summary>
	public string HM02019 { get; set; }
}

