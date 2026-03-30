namespace Aiko.SqliteDb;

public class HM01KAIS
{
    /// <summary>
    /// 会社コード
    /// </summary>
    public string HM01001 { get; set; }

    /// <summary>
    /// 会社名
    /// </summary>
    public string HM01002 { get; set; }

    /// <summary>
    /// 郵便番号
    /// </summary>
    public string HM01003 { get; set; }

    /// <summary>
    /// 都道府県
    /// </summary>
    public string HM01004 { get; set; }

    /// <summary>
    /// 市区町村
    /// </summary>
    public string HM01005 { get; set; }

    /// <summary>
    /// 番地
    /// </summary>
    public string HM01006 { get; set; }

    /// <summary>
    /// 電話番号
    /// </summary>
    public string HM01007 { get; set; }

    /// <summary>
    /// FAX
    /// </summary>
    public string HM01008 { get; set; }

    /// <summary>
    /// 並び順
    /// </summary>
    public string HM01009 { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public string HM01010 { get; set; }

    /// <summary>
    /// 作成オペレータ
    /// </summary>
    public string HM01011 { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public string HM01012 { get; set; }

    /// <summary>
    /// 更新オペレータ
    /// </summary>
    public string HM01013 { get; set; }

    /// <summary>
    /// 同期日時
    /// </summary>
    public string HM01014 { get; set; }

    /// <summary>
    /// 同期オペレータ
    /// </summary>
    public string HM01015 { get; set; }

    /// <summary>
    ///会社ID 
    /// </summary>
    public string HM01016 { get; set; }

	/// <summary>
	/// 会社別IP制限 0:無効　1:有効
	/// </summary>
	public int HM01017 { get; set; } = 0;

	/// <summary>
	/// 会社別端末制限 0:無効　1:有効
	/// </summary>
	public int HM01018 { get; set; } = 0;
}

