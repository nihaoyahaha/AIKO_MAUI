using Aiko.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static FFImageLoading.Work.ImageInformation;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class SystemOptionServerPageVM : ObservableValidator
{
	[ObservableProperty]
	public partial bool IsJepg { get; set; }

	[ObservableProperty]
	public partial string ImageQuality { get; set; }

	[ObservableProperty]
	public partial string BlackFontSize { get; set; }

	[ObservableProperty]
	public partial string SaveDays { get; set; }

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

	async Task<bool> ValidateBeforeSave()
	{
		if (!int.TryParse(ImageQuality, out int qualityValue) || qualityValue < 10 || qualityValue > 100)
		{
			await DialogHelper.MessageDialogButton1("10~100数字を入力してください。");
			return false;
		}
		if (!int.TryParse(BlackFontSize, out int fontSize) || fontSize < 10 || fontSize > 16)
		{
			await DialogHelper.MessageDialogButton1("10~16数字を入力してください。");
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
}
