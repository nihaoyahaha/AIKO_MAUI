using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Common;

public partial class LogItem : ObservableObject
{
	public string FileName { get; set; } = string.Empty;
	public string FilePath { get; set; } = string.Empty;
	public string FileSize { get; set; } = string.Empty;
	public DateTime CreatedDate { get; set; }

	[ObservableProperty]
	public partial bool IsSelected { get; set; }
}