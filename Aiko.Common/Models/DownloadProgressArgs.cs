using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Common;

/// <summary>
/// ダウンロード進捗変更イベントのデータパラメータ
/// </summary>
public class DownloadProgressArgs
{
	/// <summary>
	/// 現在のステータスメッセージ
	/// </summary>
	public string Message { get; set; } = string.Empty;

	/// <summary>
	/// 現在のダウンロード進捗値(0.0-1.0)
	/// </summary>
	public double Progress { get; set; }

	/// <summary>
	/// ダウンロードの進捗率テキスト
	/// </summary>
	public string PercentText { get; set; } = string.Empty;
}
