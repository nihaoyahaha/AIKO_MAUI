using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;

namespace Aiko.Services.Services;

public class LoginOutService : BaseService<LoginOutService>, ILoginOutService
{
	public LoginOutService(ServiceContext<LoginOutService> context) : base(context)
	{
	}

	public AikoAppContext AppContext => AikoAppContext;
}
