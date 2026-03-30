using Aiko.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static FFImageLoading.Work.ImageInformation;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class SystemOptionServerPageVM: ObservableValidator
{
    [ObservableProperty]
    private string photoType;

    // 当 PhotoType 改变时，此方法会自动触发
    partial void OnPhotoTypeChanged(string value)
    {
        // 在这里写切换事件的逻辑
        if (value == "SVG")
        {
            // 比如：清空某些设置或弹出提示
            ImageQuality = Preferences.Default.Get("ImageQuality_SVG", "80");
        }
        else if (value == "JPEG")
        {
            ImageQuality = Preferences.Default.Get("ImageQuality_JPEG", "100");
        }
    }

    
    [ObservableProperty]
	private string imageQuality;

    [ObservableProperty]
    private string blackFontSize;

    [ObservableProperty]
    private string saveDays;


    [RelayCommand]
    private void PageLoad()
    {
        PhotoType = Preferences.Default.Get("PhotoType", "JPEG");
        if (PhotoType == "JPEG")
        {
            ImageQuality = Preferences.Default.Get("ImageQuality_JPEG", "100");
        }
        else 
        {
            ImageQuality = Preferences.Default.Get("ImageQuality_SVG", "80");
        }

        BlackFontSize = Preferences.Default.Get("BlackFontSize", "14");
        SaveDays = Preferences.Default.Get("SaveDays", "14");
    }

    [RelayCommand]
    private async Task Save()
    {
        if (!await ValidateBeforeSave()) return;
        Preferences.Default.Set("PhotoType", PhotoType);
        if (PhotoType == "JPEG")
        {
            Preferences.Default.Set("ImageQuality_JPEG", ImageQuality);
        }
        else 
        {
            Preferences.Default.Set("ImageQuality_SVG", ImageQuality);
        }
        Preferences.Default.Set("BlackFontSize", BlackFontSize);
        Preferences.Default.Set("SaveDays", SaveDays);

        DialogHelper.MessageDialogOk("保存に成功しました。");
    }

    async Task<bool> ValidateBeforeSave()
    {
		if (!int.TryParse(ImageQuality, out int qualityValue) || qualityValue < 10 || qualityValue > 100)
		{
			await DialogHelper.MessageDialogButton1("10~100数字を入力してください。");
            return false;
		}
		if (!int.TryParse(BlackFontSize, out int fontSize) || fontSize < 14 || fontSize > 35)
		{
			await DialogHelper.MessageDialogButton1("14~35数字を入力してください。");
            return false;
		}
		if (!int.TryParse(SaveDays, out int days) || days < 1 || days > 180)
		{
			await DialogHelper.MessageDialogButton1("1~180数字を入力してください。");
            return false;
		}
        return true;
	}
}
