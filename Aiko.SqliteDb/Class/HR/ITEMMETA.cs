namespace Aiko.SqliteDb;

public class ITEMMETA : HR01ITEM
{
    /// <summary>
    /// キャプション 断面名
    /// </summary>
    public string HM10004 { get; set; } = string.Empty;

    /// <summary>
    /// 工区名
    /// </summary>
    public string HM07003 { get; set; } = string.Empty;

    /// <summary>
    /// 部位图片
    /// </summary>
    public string HM06016 { get; set; } = string.Empty;

    /// <summary>
    /// マップに表示位置X
    /// </summary>
    public string HM07014 { get; set; } = string.Empty;

    /// <summary>
    /// マップに表示位置Y
    /// </summary>
    public string HM07015 { get; set; } = string.Empty;
}

