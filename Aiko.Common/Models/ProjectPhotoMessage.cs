using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Common;

public class ProjectPhotoMessage
{
	/// <summary>
	/// 工程コード
	/// </summary>
	public string ProjectCode { get; set; } = string.Empty;

	/// <summary>
	/// 確認項目コード
	/// </summary>
	public string InspectionItemCode { get; set; } = string.Empty;

	/// <summary>
	/// 写真パス
	/// </summary>
	public string PhotoPath { get; set; } = string.Empty;
}
