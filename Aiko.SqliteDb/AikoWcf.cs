using Microsoft.Extensions.Logging;
using System.ServiceModel;

namespace Aiko.SqliteDb;

public class AikoWcf
{
	private readonly ILogger<AikoWcf> _logger;

	private IUWPServiceAPI? _serviceApi02;

	public AikoWcf(ILogger<AikoWcf> logger)
	{
		_logger = logger;
	}

	public IUWPServiceAPI? ServiceApi02(string server = "", string serverPort = "", string serverTimeOut = "")
	{
		try
		{
			if (_serviceApi02 == null)
			{
				if (string.IsNullOrEmpty(server))
				{
					server = Preferences.Default.Get("Server", "");
				}
				if (string.IsNullOrEmpty(serverPort))
				{
					serverPort = Preferences.Default.Get("ServerPort", "");
				}

				if (string.IsNullOrEmpty(serverTimeOut))
				{
					serverTimeOut = Preferences.Default.Get("ServerTimeOut", "300");
				}

#if ENCRYPTION
                string serverUri = String.Format("https://{0}:{1}/WcfSample/UWPServiceAPI", server, serverPort);
#else
				string serverUri = String.Format("http://{0}:{1}/WcfSample/UWPServiceAPI", server, serverPort);
#endif
				EndpointAddress address = new EndpointAddress(serverUri);
				BasicHttpBinding binding = new BasicHttpBinding();
				binding.TransferMode = TransferMode.Buffered;
				binding.SendTimeout = new TimeSpan(0, 0, 0, Convert.ToInt32(serverTimeOut));
				binding.MaxReceivedMessageSize = (long)1073741824;
#if ENCRYPTION
    		    binding.Security.Mode = BasicHttpSecurityMode.Transport;
    	        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
#endif
				ChannelFactory<IUWPServiceAPI> factory = new ChannelFactory<IUWPServiceAPI>(binding, address);
				_serviceApi02 = factory.CreateChannel();
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
		return _serviceApi02;
	}

	/// <summary>
	/// wcf通信が正常かどうかをテストする
	/// </summary>
	/// <param name="server">サーバー</param>
	/// <param name="serverPort">ポート</param>
	/// <param name="serverTimeOut">タイムアウト時間（秒）</param>
	/// <returns></returns>
	public IUWPServiceAPI? WcfConnectionTest(string server = "", string serverPort = "", string serverTimeOut = "")
	{
		try
		{
#if ENCRYPTION
       string serverUri = String.Format("https://{0}:{1}/WcfSample/UWPServiceAPI", server, serverPort);
#else
			string serverUri = String.Format("http://{0}:{1}/WcfSample/UWPServiceAPI", server, serverPort);
#endif
			EndpointAddress address = new EndpointAddress(serverUri);
			BasicHttpBinding binding = new BasicHttpBinding();
			binding.TransferMode = TransferMode.Buffered;
			binding.SendTimeout = new TimeSpan(0, 0, 0, Convert.ToInt32(serverTimeOut));
			binding.MaxReceivedMessageSize = (long)1073741824;
#if ENCRYPTION
    		    binding.Security.Mode = BasicHttpSecurityMode.Transport;
    	        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
#endif
			ChannelFactory<IUWPServiceAPI> factory = new ChannelFactory<IUWPServiceAPI>(binding, address);
			_serviceApi02 = factory.CreateChannel();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
		}
		return _serviceApi02;
	}

}
