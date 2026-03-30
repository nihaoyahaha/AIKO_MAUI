namespace Aiko.SqliteDb;

public class ITEMKSKKRESULT
{
    /// <summary>
    /// アイテムコード
    /// </summary>
    public string HR01003 { get; set; }

    /// <summary>
    /// 部位コード
    /// </summary>
    public string HR01019 { get; set; }

    /// <summary>
    /// 确认点检查结果值种类数量
    /// </summary>
    public int COUNT { get; set; }

    /// <summary>
    /// 确认点检查结果值种类合计
    /// </summary>
    public int SUM { get; set; }

    /// <summary>
    /// 确认点检查结果值种类 最小的值
    /// </summary>
    public int MIN { get; set; }

    /// <summary>
    /// 确认点检查结果值种类 最大的值
    /// </summary>
    public int MAX { get; set; }
}

