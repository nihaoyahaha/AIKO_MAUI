using System;
namespace Aiko.SqliteDb;

public class HM14GUIDANDHM20
{
    /// <summary>
    /// 工事コード
    /// </summary>
    public string HM14001 { get; set; } = string.Empty;

    /// <summary>
    /// マップコード
    /// </summary>
    public string HM14002 { get; set; } = string.Empty;

    /// <summary>
    /// ガイドコード
    /// </summary>
    public string HM14003 { get; set; } = string.Empty;

    /// <summary>
    /// ガイドタイプ
    /// </summary>
    public int HM14004 { get; set; }

    /// <summary>
    /// 並び順
    /// </summary>
    public string HM14005 { get; set; } = string.Empty;

    /// <summary>
    /// 座標種別
    /// </summary>
    public string HM14006 { get; set; } = string.Empty;

    /// <summary>
    /// 間隔
    /// </summary>
    public int HM14007 { get; set; }

    /// <summary>
    /// 論理座標
    /// </summary>
    public int HM14008 { get; set; }

    //ガイド番号
    public int HM14015 { get; set; }

    /// <summary>
    /// 角度
    /// </summary>
    public decimal HM20005 { get; set; }

    /// <summary>
    /// 制御点1X
    /// </summary>
    public decimal HM20006 { get; set; }

    /// <summary>
    /// 制御点1Y
    /// </summary>
    public decimal HM20007 { get; set; }

    /// <summary>
    /// 制御点2X
    /// </summary>
    public decimal HM20008 { get; set; }

    /// <summary>
    /// 制御点2Y
    /// </summary>
    public decimal HM20009 { get; set; }
}

