using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.Common.Models;

/// <summary>
/// 合成画像の詳細プロパティ
/// </summary>
public partial class PhotoLayer :ObservableObject
{
	public string? ImageUri { get; set; }
	public ImageSource? GreenBmp { get; set; }
	public ImageSource? PhotoBmp { get; set; }

	[ObservableProperty]
	public partial bool GreenBackgroundIsVisible { get; set; }

	[ObservableProperty]
	public partial bool GreenBackgroundIsChecked { get; set; } = false;

	[ObservableProperty]
	public partial bool PhotoIsChecked { get; set; } = false;

	public Thickness Margin { get; set; }
	public string GreenWidth { get; set; } = string.Empty;
	public string GreenHeight { get; set; } = string.Empty;
	public string PhotoWidth { get; set; } = string.Empty;
	public string PhotoHeight { get; set; } = string.Empty;
}
