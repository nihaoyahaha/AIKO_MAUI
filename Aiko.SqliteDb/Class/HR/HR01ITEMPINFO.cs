using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.SqliteDb;

public class HR01ITEMPINFO
{
	/// <summary>
	/// アイテムコード
	/// </summary>
	public string HR03003 { get; set; }

	/// <summary>
	/// 工程コード
	/// </summary>
	public string HM13003 { get; set; }
	
	/// <summary>
	/// 工程名
	/// </summary>
	public string HM09003 { get; set; }
	
	/// <summary>
	/// 確認項目コード
	/// </summary>
	public string HR03004 { get; set; }
	
	/// <summary>
	/// 確認項目名
	/// </summary>
	public string HM13005 { get; set; }
	
	/// <summary>
	/// 写真コード
	/// </summary>
	public string HR03002 { get; set; }
	
	/// <summary>
	/// 写真方式
	/// </summary>
	public int HR03017 { get; set; }
	
	/// <summary>
	/// 判定基準
	/// </summary>
	public string HM13012 { get; set; }
}
