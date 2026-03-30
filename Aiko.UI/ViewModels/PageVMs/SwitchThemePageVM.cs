using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class SwitchThemePageVM: ObservableValidator
{
	[ObservableProperty]
	private ObservableCollection<string> _themes = new() { "Light", "Dark" };

	[ObservableProperty]
	private string _selectedTheme ="Light";

	[RelayCommand]
	private void PageLoaded()
	{
		SelectedTheme =  Preferences.Default.Get("Theme", "Light");
	}

	[RelayCommand]
	private void ThemeSelectedIndexChanged()
	{
		ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
		if (mergedDictionaries != null)
		{
			mergedDictionaries.Clear();
			switch (SelectedTheme)
			{
				case "Dark":
					mergedDictionaries.Add(new DarkTheme());
					break;
				case "Light":
				default:
					mergedDictionaries.Add(new LightTheme());
					break;
			}
			Preferences.Default.Set("Theme", SelectedTheme);
		}
	}
}
