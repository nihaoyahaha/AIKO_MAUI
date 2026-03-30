using Aiko.Common.Models.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aiko.IServices.IServices
{
    public interface IRemotingService : IServiceBase
    {
        public int CheckWcfAsync(string server, string serverPort, string serverTimeOut);
    }
}
