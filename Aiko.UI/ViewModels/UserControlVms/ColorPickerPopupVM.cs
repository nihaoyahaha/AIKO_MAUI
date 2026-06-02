using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Aiko.UI.ViewModels.UserControlVms;

public partial class ColorPickerPopupVM : ObservableObject, IQueryAttributable
{
	readonly IPopupService _popupService;

	[ObservableProperty]
	public partial string SelectedHex { get; set; } = "#FF00FF";

	[ObservableProperty]
	public partial Color SelectedColor { get; set; } = Colors.Magenta;

	[ObservableProperty]
	public partial ObservableCollection<ColorOption> ColorOptions { get; set; } = new()
	{
		new("黒", "#000000"),
		new("白", "#FFFFFF"),
		new("灰", "#808080"),
		new("赤", "#FF0000"),
		new("橙", "#FFA500"),
		new("黄", "#FFFF00"),
		new("緑", "#008000"),
		new("水", "#00FFFF"),
		new("青", "#0000FF"),
		new("紫", "#800080"),
		new("桃", "#FF00FF"),
		new("茶", "#8B4513"),
	};

	public ColorPickerPopupVM(IPopupService popupService)
	{
		_popupService = popupService;
	}

	partial void OnSelectedHexChanged(string value)
	{
		if (TryNormalizeHex(value, out string hex))
		{
			SelectedColor = Color.FromArgb(hex);
		}
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("selectedColor", out object? value) && value is string hex)
		{
			SetSelectedHex(hex);
		}
	}

	[RelayCommand]
	private void SelectColor(ColorOption option)
	{
		SetSelectedHex(option.Hex);
	}

	[RelayCommand]
	private async Task Add()
	{
		if (!TryNormalizeHex(SelectedHex, out string hex))
		{
			return;
		}

		await _popupService.ClosePopupAsync(Shell.Current, hex);
	}

	[RelayCommand]
	private async Task Cancel()
	{
		await _popupService.ClosePopupAsync<string>(Shell.Current, null);
	}

	void SetSelectedHex(string hex)
	{
		if (TryNormalizeHex(hex, out string normalizedHex))
		{
			SelectedHex = normalizedHex;
			SelectedColor = Color.FromArgb(normalizedHex);
		}
	}

	static bool TryNormalizeHex(string value, out string hex)
	{
		hex = string.Empty;
		if (string.IsNullOrWhiteSpace(value))
		{
			return false;
		}

		string candidate = value.Trim();
		if (!candidate.StartsWith('#'))
		{
			candidate = $"#{candidate}";
		}

		if (candidate.Length == 9)
		{
			candidate = $"#{candidate[3..]}";
		}

		if (candidate.Length != 7 || candidate.Skip(1).Any(x => !Uri.IsHexDigit(x)))
		{
			return false;
		}

		hex = candidate.ToUpperInvariant();
		return true;
	}
}

public partial class ColorOption : ObservableObject
{
	public ColorOption(string name, string hex)
	{
		Name = name;
		Hex = hex;
		Color = Color.FromArgb(hex);
	}

	public string Name { get; }

	public string Hex { get; }

	public Color Color { get; }
}
