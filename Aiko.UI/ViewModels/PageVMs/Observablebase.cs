using Aiko.IServices.IServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Aiko.UI.ViewModels.PageVMs;

public abstract partial class Observablebase<TViewModel,TService> : ObservableValidator, IQueryAttributable 
	where TViewModel : class 
	where TService : IServiceBase 
{
	/// <summary>
	/// 現在のViewModelのログレコーダ
	/// </summary>
	protected readonly ILogger<TViewModel> Logger;

	/// <summary>
	/// 現在のViewModelが依存しているコアビジネスサービスで、ページ論理に関連するデータ操作やAPI呼び出しを実行できます。
	/// </summary>
	protected readonly TService Service;

	protected Observablebase(ILogger<TViewModel> logger,TService service)
	{
		Logger = logger;
		Service = service;
	}

	/// <summary>
	/// ページがシェルを介してナビゲートされ、クエリパラメータが渡されると呼び出されます。
	/// サブクラスは、このメソッドを書き換えて、入力されたパラメータを処理します。
	/// </summary>
	/// <param name="query"></param>
	public virtual void ApplyQueryAttributes(IDictionary<string, object> query) 
	{ 
	}
}


