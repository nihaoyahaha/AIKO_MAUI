using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Common;

public partial class LogItem : ObservableObject
{
	public string FileName { get; set; }
	public string FilePath { get; set; }
	public string FileSize { get; set; }
	public DateTime CreatedDate { get; set; }

	[ObservableProperty]
	private bool _isSelected;
}