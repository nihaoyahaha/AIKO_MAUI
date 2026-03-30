using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.Services.Services
{
    public class RemotingService : BaseService<RemotingService>, IRemotingService
    {
        public RemotingService(ServiceContext<RemotingService> context)
        : base(context)
        {

        }

        /// <summary>
        /// aikoApp実行中のアプリケーションコンテキスト
        /// </summary>
        public AikoAppContext AppContext => AikoAppContext;

        public int CheckWcfAsync(string server, string serverPort, string serverTimeOut)
        {
            try
            {
                return AikoWcf
                    .WcfConnectionTest(server, serverPort, serverTimeOut)
                    .IpPortIsTrue();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return 0;
            }
        }
    }
}
