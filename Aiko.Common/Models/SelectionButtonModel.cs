using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.Common.Models;

public partial class SelectionButtonModel : ObservableValidator
{
	/// <summary>
	/// 断面ボタンのテキスト
	/// </summary>
	public string Text { get; set; } = "";

	/// <summary>
	/// 断面ボタンのId
	/// </summary>
	public int Id { get; set; } = -1;

	/// <summary>
	/// 断面図コード
	/// </summary>
	public string ImageCode { get; set; } = "";

	/// <summary>
	/// 断面ボタンの背景色
	/// </summary>
	[ObservableProperty]
	private Color _backgroundColor = Color.FromRgba(153, 201, 239, 250);
}