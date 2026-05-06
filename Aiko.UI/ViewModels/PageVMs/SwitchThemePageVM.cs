using Aiko.UI.Themes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class SwitchThemePageVM: ObservableValidator
{
	[ObservableProperty]
	public partial ObservableCollection<string> Themes { get; set; } = new() { "Light", "Dark" };

	[ObservableProperty]
	public partial string SelectedTheme { get; set; } = "Light";

	[RelayCommand]
	private void PageLoaded()
	{
		SelectedTheme = ThemeManager.GetSavedTheme();
	}

	[RelayCommand]
	private void ThemeSelectedIndexChanged()
	{
		ThemeManager.ApplyTheme(SelectedTheme);
	}
}
