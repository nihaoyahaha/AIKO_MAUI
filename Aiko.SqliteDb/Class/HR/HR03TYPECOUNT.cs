using System;
namespace Aiko.SqliteDb;

public class HR03TYPECOUNT
{
    public string HM13004 { get; set; } = string.Empty;
    /// <summary>
    /// 各类型下，写真数量统计
    /// </summary>
    public int IDCOUNT { get; set; }

    /// <summary>
    /// 写真タイプ
    /// 0：不要　1：確認箇所ごと　2：工区・符号ごと　3：工区ごと
    /// </summary>
    public int STYPE { get; set; }
}

