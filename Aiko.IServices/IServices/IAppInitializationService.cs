using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.IServices.IServices;

public interface IAppInitializationService
{
	/// <summary>
	/// sqliteデータベースから通信設定を取得するには
	/// </summary>
	/// <returns></returns>
	Task<bool> InitializeAppCommunicationServiceFromSqliteAsync();
}
