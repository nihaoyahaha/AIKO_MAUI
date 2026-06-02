using Aiko.Common;
using Aiko.UI.ViewModels.UserControlVms;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static FFImageLoading.Work.ImageInformation;
using Color = Microsoft.Maui.Graphics.Color;
using Colors = Microsoft.Maui.Graphics.Colors;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class SystemOptionServerPageVM : ObservableValidator
{
	readonly IPopupService _popupService;

	const string BlackCameraButtonColorKey = "CameraButtonColor_Black";
	const string ReplaceCameraButtonColorKey = "CameraButtonColor_Replace";
	const string DefaultCameraButtonColor = "#FFFFFF";

	public SystemOptionServerPageVM(IPopupService popupService)
	{
		_popupService = popupService;
	}

	[ObservableProperty]
	public partial bool IsJepg { get; set; }

	[ObservableProperty]
	public partial string ImageQuality { get; set; }

	[ObservableProperty]
	public partial string BlackFontSize { get; set; }

	[ObservableProperty]
	public partial string SaveDays { get; set; }

	[ObservableProperty]
	public partial Color BlackCameraButtonColor { get; set; } = Colors.White;

	[ObservableProperty]
	public partial Color ReplaceCameraButtonColor { get; set; } = Colors.White;

	[ObservableProperty]
	public partial ObservableCollection<string> FrontResolutions { get; set; } = new();

	[ObservableProperty]
	public partial int SelectedFrontResolutionIndex { get; set; } = -1;

	[ObservableProperty]
	public partial ObservableCollection<string> RearResolutions { get; set; } = new();

	[ObservableProperty]
	public partial int SelectedRearResolutionIndex { get; set; } = -1;

	partial void OnIsJepgChanged(bool value)
	{
		if (value)
		{
			ImageQuality = Preferences.Default.Get("ImageQuality_JPEG", "100");
		}
		else
		{
			ImageQuality = Preferences.Default.Get("ImageQuality_SVG", "80");
		}
	}

	[RelayCommand]
	private void PageLoad()
	{
		IsJepg = Preferences.Default.Get("PhotoType", "JPEG") == "JPEG" ? true : false;
		if (IsJepg)
		{
			ImageQuality = Preferences.Default.Get("ImageQuality_JPEG", "100");
		}
		else
		{
			ImageQuality = Preferences.Default.Get("ImageQuality_SVG", "80");
		}

		BlackFontSize = Preferences.Default.Get("BlackFontSize", "14");
		SaveDays = Preferences.Default.Get("SaveDays", "14");
		BlackCameraButtonColor = ReadColorPreference(BlackCameraButtonColorKey);
		ReplaceCameraButtonColor = ReadColorPreference(ReplaceCameraButtonColorKey);
		LoadCameraResolutionDisplayList();

	}

	[RelayCommand]
	private async Task Save()
	{
		if (!await ValidateBeforeSave()) return;
		Preferences.Default.Set("PhotoType", IsJepg ? "JPEG" : "SVG");
		if (IsJepg)
		{
			Preferences.Default.Set("ImageQuality_JPEG", ImageQuality);
		}
		else
		{
			Preferences.Default.Set("ImageQuality_SVG", ImageQuality);
		}
		Preferences.Default.Set("BlackFontSize", BlackFontSize);
		Preferences.Default.Set("SaveDays", SaveDays);
		Preferences.Default.Set(BlackCameraButtonColorKey, ToHex(BlackCameraButtonColor));
		Preferences.Default.Set(ReplaceCameraButtonColorKey, ToHex(ReplaceCameraButtonColor));

		if (FrontResolutions.Count > 0 && SelectedFrontResolutionIndex >= 0)
		{
			Preferences.Default.Set("CameraResolution_Front_Selected", FrontResolutions[SelectedFrontResolutionIndex]);
		}
		if (RearResolutions.Count > 0 && SelectedRearResolutionIndex >= 0)
		{
			Preferences.Default.Set("CameraResolution_Rear_Selected", RearResolutions[SelectedRearResolutionIndex]);
		}

		DialogHelper.MessageDialogOk("保存に成功しました。");
	}

	[RelayCommand]
	private async Task SelectBlackCameraButtonColor()
	{
		BlackCameraButtonColor = await SelectCameraButtonColorAsync(BlackCameraButtonColor);
	}

	[RelayCommand]
	private async Task SelectReplaceCameraButtonColor()
	{
		ReplaceCameraButtonColor = await SelectCameraButtonColorAsync(ReplaceCameraButtonColor);
	}

	async Task<bool> ValidateBeforeSave()
	{
		if (!int.TryParse(ImageQuality, out int qualityValue) || qualityValue < 10 || qualityValue > 100)
		{
			await DialogHelper.MessageDialogButton1("10~100数字を入力してください。");
			return false;
		}
		if (!int.TryParse(BlackFontSize, out int fontSize) || fontSize < 14 || fontSize > 25)
		{
			await DialogHelper.MessageDialogButton1("14~25数字を入力してください。");
			return false;
		}
		if (!int.TryParse(SaveDays, out int days) || days < 1 || days > 180)
		{
			await DialogHelper.MessageDialogButton1("1~180数字を入力してください。");
			return false;
		}
		return true;
	}

	void LoadCameraResolutionDisplayList()
	{
		string frontList = Preferences.Default.Get("CameraResolution_Front", "");
		string rearList = Preferences.Default.Get("CameraResolution_Rear", "");
		string frontSelected = Preferences.Default.Get("CameraResolution_Front_Selected", "");
		string rearSelected = Preferences.Default.Get("CameraResolution_Rear_Selected", "");
		if (!string.IsNullOrEmpty(frontList))
		{
			FrontResolutions = new ObservableCollection<string>(frontList.Split(',').ToList());
			if (!string.IsNullOrEmpty(frontSelected))
			{
				SelectedFrontResolutionIndex = FrontResolutions.IndexOf(frontSelected);
			}
		}
		if (!string.IsNullOrEmpty(rearList))
		{
			RearResolutions = new ObservableCollection<string>(rearList.Split(',').ToList());
			if (!string.IsNullOrEmpty(rearSelected))
			{
				SelectedRearResolutionIndex = RearResolutions.IndexOf(rearSelected);
			}
		}
	}

	async Task<Color> SelectCameraButtonColorAsync(Color currentColor)
	{
		var queryAttributes = new Dictionary<string, object>
		{
			["selectedColor"] = ToHex(currentColor)
		};

		IPopupResult<string> popResult = await _popupService.ShowPopupAsync<ColorPickerPopupVM, string>(
			Shell.Current,
			options: new PopupOptions
			{
				PageOverlayColor = Colors.Transparent,
				Shape = new RoundRectangle
				{
					CornerRadius = new CornerRadius(8),
					Stroke = Colors.Transparent,
				}
			},
			shellParameters: queryAttributes);

		if (string.IsNullOrWhiteSpace(popResult.Result))
		{
			return currentColor;
		}

		return Color.FromArgb(popResult.Result);
	}

	Color ReadColorPreference(string key)
	{
		string hex = Preferences.Default.Get(key, DefaultCameraButtonColor);
		try
		{
			return Color.FromArgb(hex);
		}
		catch
		{
			return Color.FromArgb(DefaultCameraButtonColor);
		}
	}

	string ToHex(Color color)
	{
		int red = (int)Math.Round(color.Red * 255);
		int green = (int)Math.Round(color.Green * 255);
		int blue = (int)Math.Round(color.Blue * 255);
		return $"#{red:X2}{green:X2}{blue:X2}";
	}
}
