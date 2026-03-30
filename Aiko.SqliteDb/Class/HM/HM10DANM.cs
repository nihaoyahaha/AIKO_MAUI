using SQLite;

namespace Aiko.SqliteDb;

public class HM10DANM
{
	[PrimaryKey]
	/// <summary>
	/// 工事コード
	/// </summary>
	public string HM10001 { get; set; }

    /// <summary>
    ///  グループコード
    /// </summary>
    public string HM10002 { get; set; }

	[PrimaryKey]
	/// <summary>
	///  断面コード
	/// </summary>
	public string HM10003 { get; set; }

    /// <summary>
    ///  キャプション
    /// </summary>
    public string HM10004 { get; set; }

    /// <summary>
    ///  並び順
    /// </summary>
    public string HM10005 { get; set; }

    /// <summary>
    ///  ファイルコード
    /// </summary>
    public string HM10006 { get; set; }

    /// <summary>
    ///  表示範囲座標X
    /// </summary>
    public int HM10007 { get; set; }

    /// <summary>
    ///  表示範囲座標Y
    /// </summary>
    public int HM10008 { get; set; }

    /// <summary>
    ///  表示範囲Width
    /// </summary>
    public int HM10009 { get; set; }

    /// <summary>
    ///  表示範囲Height
    /// </summary>
    public int HM10010 { get; set; }

    /// <summary>
    ///  非表示フラグ
    /// </summary>
    public int HM10011 { get; set; }

    /// <summary>
    ///  説明
    /// </summary>
    public string HM10012 { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public string HM10013 { get; set; }

    /// <summary>
    /// 作成オペレータ
    /// </summary>
    public string HM10014 { get; set; }

    /// <summary>
    ///  更新日時
    /// </summary>
    public string HM10015 { get; set; }

    /// <summary>
    ///  更新オペレータ
    /// </summary>
    public string HM10016 { get; set; }

    /// <summary>
    ///  同期日時
    /// </summary>
    public string HM10017 { get; set; }

    /// <summary>
    ///  同期オペレータ
    /// </summary>
    public string HM10018 { get; set; }

    public int HM10019 { get; set; }

    public int HM10020 { get; set; }

    public int HM10021 { get; set; }

    public int HM10022 { get; set; }
    
    public string HM10023 { get; set; }

    public int HM10024 { get; set; }

    public string HM10025 { get; set; }
}


