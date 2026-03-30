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
	private bool _greenBackgroundIsVisible;

	[ObservableProperty]
	private bool _greenBackgroundIsChecked = false;

	[ObservableProperty]
	private bool _photoIsChecked = false;

	public Thickness? Margin { get; set; }
	public string? GreenWidth { get; set; }
	public string? GreenHeight { get; set; }
	public string? PhotoWidth { get; set; }
	public string? PhotoHeight { get; set; }
}
