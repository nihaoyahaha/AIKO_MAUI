using Aiko.Common;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Aiko.UI.ViewModels.UserControlVms;

public partial class PunchListPopupVM : ObservableObject, IQueryAttributable
{
	readonly IPopupService _popupService;

	/// <summary>
	/// 指摘事項リスト
	/// </summary>
	[ObservableProperty]
	private ObservableCollection<PunchListModel> _punchList = new();

	public PunchListPopupVM(IPopupService popupService)
	{
		_popupService = popupService;
	}

	/// <summary>
	/// パラメータの取得
	/// </summary>
	/// <param name="query"></param>
	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		PunchList = new ObservableCollection<PunchListModel>();
		if (query.Keys.Contains("punchList"))
		{
			string[] punchListItems = query["punchList"].ToString().Split(',');
			foreach (var item in punchListItems)
			{
				PunchList.Add(new PunchListModel { Text = item });
			}
		}
	}

	/// <summary>
	/// 選択した指摘事項に戻る
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	private async Task Add()
	{
		string result = string.Join(",", PunchList.Where(x => x.IsSelected).Select(x => x.Text));
		await _popupService.ClosePopupAsync(Shell.Current, result);
	}
}
