using Aiko.Common;
using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class DeletePageVM : Observablebase<DeletePageVM, IDeleteService>
{
	public DeletePageVM(ILogger<DeletePageVM> logger, IDeleteService service) : base(logger, service)
	{
	}

	/// <summary>
	/// 現場集合
	/// </summary>
	[ObservableProperty]
	public partial ObservableCollection<ListItem> Projects { get; set; } = new();

	/// <summary>
	/// 選択した要素インデックス
	/// </summary>
	[ObservableProperty]
	public partial int SelectedIndex { get; set; } = 0;

	/// <summary>
	/// 削除ボタンが有効かどうか
	/// </summary>
	[ObservableProperty]
	public partial bool IsEnabled { get; set; } = false;

	[RelayCommand]
	private async Task PageLoaded()
	{
		Projects = await Service.GetProjects();
	}

	/// <summary>
	/// 現場選択変更処理
	/// </summary>
	[RelayCommand]
	private void ProjectsSelectedIndexChanged()
	{
		IsEnabled = SelectedIndex == 0 ?false : true;
	}

	/// <summary>
	/// フィールドデータの削除
	/// </summary>
	[RelayCommand]
	private async Task DeleteProject()
	{
		try
		{
			string ErrMsg = ErrorMessage.ERRORPOP("CM01027");
			var result = await DialogHelper.MessageDialogButton2(ErrMsg);
			if (result == NCDialogResult.No) return;
			string projectCode = Projects[SelectedIndex].Value.Trim();
			string path = Path.Combine(Service.AppContext.AppDataFoler, projectCode);
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			Logger.LogInformation($"フィールド{projectCode}フォルダの削除に成功しました");
			DialogHelper.MessageDialogOk("現場削除に成功しました");
		}
		catch (Exception ex)
		{
			Logger.LogError($"現場フォルダの削除に失敗しました:{ex.ToString()}");
			string ErrMsg = ErrorMessage.ERRORPOP("CM00001");
			DialogHelper.MessageDialogOk(ErrMsg);
		}
		
	}
}
