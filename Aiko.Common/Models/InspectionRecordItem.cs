using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.Common.Models;

/// <summary>
/// 建築検査記録の1レコード（写真・図面・メタデータを含む）
/// </summary>
public class InspectionRecordItem 
{
	/// <summary>
	/// 撮影日時
	/// </summary>
	public DateTime? CreateTime { get; set; }

	/// <summary>
	/// 方向
	/// </summary>
	public int Direction { get; set; }

	/// <summary>
	/// 方向
	/// </summary>
	public string? DirectionText { get; set; }

	/// <summary>
	/// 備考
	/// </summary>
	public string? Comment { get; set; }

	/// <summary>
	/// 写真のフルパス
	/// </summary>
	public string? FilePath { get; set; }

	/// <summary>
	/// 写真コード
	/// </summary>
	public string? HR03002 { get; set; }

	/// <summary>
	/// 写真方式 0：JPG 1：SVG
	/// </summary>
	public int HR03017 { get; set; } = 0;

	/// <summary>
	/// SVGレイヤ表示
	/// 　　　写真　　 黒板　　 注釈
	///   7：　〇      〇      〇
	///   3：　〇      〇      ×
	///   5：　〇      ×       〇
	///   1：　〇      ×       ×
	/// </summary>
	public int HR03018 { get; set; } = 3;

}
