using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aiko.SqliteDb;

public class WebDavClient
{
	WebDavOptions _options = new();
    string _authValue = string.Empty;
    readonly IHttpClientFactory _httpClientFactory;
	readonly ILogger<WebDavClient> _logger;
	public WebDavClient(IHttpClientFactory httpClientFactory,
		ILogger<WebDavClient> logger)
	{
		_httpClientFactory = httpClientFactory;
		_logger = logger;
	}

	/// <summary>
	/// webdavクライアントの初期化
	/// </summary>
	/// <param name="webDavOptions"></param>
	public void InitializeWebDavClient(WebDavOptions webDavOptions)
	{
		_options = webDavOptions;
		_authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.Username}:{_options.Password}"));
	}

	/// <summary>
	/// WebDAVサーバー上にファイルが存在するかどうかをチェックする
	/// </summary>
	/// <param name="remotePath">リモートファイルのパス</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：存在、false：存在しない</returns>
	public async Task<bool> FileExistsAsync(string remotePath, CancellationToken ct = default)
	{
		try
		{
			var client = _httpClientFactory.CreateClient("WebDavClient");
			//デフォルトのタイムアウト制限をオフにする
			client.Timeout = Timeout.InfiniteTimeSpan;

			string url = BuildUrl(remotePath);
			var request = new HttpRequestMessage(HttpMethod.Head, url);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authValue);

			using var response = await client.SendAsync(request, ct);

			if (response.IsSuccessStatusCode)
			{
				return true;
			}

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}
			return false;
		}
		catch (Exception ex)
		{
			_logger.LogError($"webdav-ファイル存在確認に失敗しました: {remotePath}. エラー: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// ローカル指定パスへのリモートファイルのダウンロード
	/// </summary>
	/// <param name="remotePath">リモートファイルパス</param>
	/// <param name="localFilePath">ローカルパス</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：ダウンロード成功、false：ダウンロード失敗</returns>
	public async Task<bool> DownloadAsync(string remotePath, string localFilePath)
	{
		//非アクティブタイムアウト時間
		TimeSpan inactivityTimeout = TimeSpan.FromMinutes(2);
		try
		{
			string? directory = Path.GetDirectoryName(localFilePath);
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var client = _httpClientFactory.CreateClient("WebDavClient");
			//デフォルトのタイムアウト制限をオフにする
			client.Timeout = Timeout.InfiniteTimeSpan;
			var request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(remotePath))
			{
				Version = HttpVersion.Version11
			};
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authValue);
			using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				_logger.LogWarning($"webdav-ダウンロード対象のファイルが見つかりません: {remotePath}");
				return false;
			}
			response.EnsureSuccessStatusCode();

			using var remoteStream = await response.Content.ReadAsStreamAsync();
			using var localStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true);

			byte[] buffer = new byte[8192];
			int bytesRead;

			while (true)
			{
				using var cts = new CancellationTokenSource(inactivityTimeout);
				try
				{
					bytesRead = await remoteStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
				}
				catch (OperationCanceledException)
				{
					_logger.LogError($"webdav-ダウンロード中にネットワークが長時間無応答でした: {remotePath}");
					return false;
				}

				if (bytesRead == 0) break;

				await localStream.WriteAsync(buffer, 0, bytesRead);
			}

			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError($"webdav-リモートファイルのダウンロードに失敗しました,ファイル{remotePath},{ex.ToString()}");
			return false;
		}
	}

	/// <summary>
	/// 指定したリモートフォルダにローカルファイルをアップロードする
	/// </summary>
	/// <param name="localFilePath">ローカルファイルのフルパス</param>
	/// <param name="remoteDirectory">リモートターゲットフォルダ</param>
	/// <param name="remoteFileName">アップロードファイルの名前</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：アップロード成功、false：アップロード失敗</returns>
	public async Task<bool> UploadAsync(string localFilePath, string remoteDirectory, string remoteFileName, CancellationToken ct = default)
	{
		try
		{
			if (!File.Exists(localFilePath))
			{
				_logger.LogWarning($"ローカルファイルは存在しません: {localFilePath}");
				return false;
			}
			await EnsureRemoteDirectoryExistsAsync(remoteDirectory, ct);

			string fileName = Path.GetFileName(localFilePath);
			string remoteFilePath = remoteDirectory.TrimEnd('/') + "/" + remoteFileName;
			string url = BuildUrl(remoteFilePath);

			var client = _httpClientFactory.CreateClient("WebDavClient");
			//デフォルトのタイムアウト制限をオフにする
			client.Timeout = Timeout.InfiniteTimeSpan;

			using var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, useAsync: true);
			
			var content = new StreamContent(fileStream);
			var request = new HttpRequestMessage(HttpMethod.Put, url)
			{
				Content = content,
				Version = HttpVersion.Version11
			};

			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authValue);

			_logger.LogInformation($"アップロード中 {fileName} へ {remoteDirectory}...");
			using var response = await client.SendAsync(request, ct);

			response.EnsureSuccessStatusCode();
			_logger.LogInformation("アップロード成功！！");
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError($"webdav-アップロード失敗: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// 指定したストリームをリモートフォルダにアップロードする
	/// </summary>
	/// <param name="contentStream">ファイルストリーム</param>
	/// <param name="remoteDirectory">リモートターゲットフォルダ</param>
	/// <param name="remoteFileName">アップロードファイルの名前</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：アップロード成功、false：アップロード失敗</returns>
	public async Task<bool> UploadStreamAsync(Stream contentStream, string remoteDirectory, string remoteFileName, CancellationToken ct = default)
	{
		try
		{
			if (contentStream == null)
			{
				_logger.LogWarning("webdav-アップロードされたログファイルストリームが空です!");
				return false;
			}
			await EnsureRemoteDirectoryExistsAsync(remoteDirectory, ct);

			string remoteFilePath = remoteDirectory.TrimEnd('/') + "/" + remoteFileName;
			string url = BuildUrl(remoteFilePath);

			var client = _httpClientFactory.CreateClient("WebDavClient");
			//デフォルトのタイムアウト制限をオフにする
			client.Timeout = Timeout.InfiniteTimeSpan;

			if (contentStream.CanSeek)
			{
				contentStream.Position = 0;
			}

			using var content = new StreamContent(contentStream);
			var request = new HttpRequestMessage(HttpMethod.Put, url)
			{
				Content = content,
				Version = HttpVersion.Version11
			};
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authValue);

			_logger.LogInformation($"ストリームをアップロード中: {remoteFileName} へ {remoteDirectory}...");

			using var response = await client.SendAsync(request, ct);

			response.EnsureSuccessStatusCode();
			_logger.LogInformation("ストリームのアップロード成功！！");
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError($"webdav-ストリームアップロード失敗: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// WebDAVサーバー上の指定されたファイルを削除する
	/// </summary>
	/// <param name="remotePath">リモートファイルのパス</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：削除成功、false：削除失敗</returns>
	public async Task<bool> DeleteAsync(string remotePath, CancellationToken ct = default)
	{
		try
		{
			var client = _httpClientFactory.CreateClient("WebDavClient");
			//デフォルトのタイムアウト制限をオフにする
			client.Timeout = Timeout.InfiniteTimeSpan;
			string url = BuildUrl(remotePath);
			var request = new HttpRequestMessage(HttpMethod.Delete, url);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authValue);

			_logger.LogInformation($"ファイルを削除中: {remotePath}");

			using var response = await client.SendAsync(request, ct);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return true;
			}
			response.EnsureSuccessStatusCode();

			_logger.LogInformation("ファイル削除成功");
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError($"webdav-ファイルの削除に失敗しました: {remotePath}. エラー: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// リモートフォルダが存在するかどうかをチェックし、存在しない場合は再帰的に作成する
	/// </summary>
	/// <param name="remoteDirectory">リモートフォルダパス</param>
	/// <param name="ct">キャンセルトークン</param>
	async Task EnsureRemoteDirectoryExistsAsync(string remoteDirectory, CancellationToken ct)
	{
		var client = _httpClientFactory.CreateClient("WebDavClient");
		string fullFolderUrl = BuildUrl(remoteDirectory.TrimEnd('/')) + "/";

		var quickCheck = new HttpRequestMessage(HttpMethod.Head, fullFolderUrl);
		quickCheck.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authValue);

		using var quickRes = await client.SendAsync(quickCheck, ct);
		if (quickRes.IsSuccessStatusCode)
		{
			return;
		}
		_logger.LogInformation($"ターゲットディレクトリが存在せず、再帰検査を開始します: {remoteDirectory}");
		string cleanDirectory = remoteDirectory.Trim('/');
		string[] folders = cleanDirectory.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		string currentPath = "";

		foreach (var folder in folders)
		{
			currentPath = string.IsNullOrEmpty(currentPath) ? folder : $"{currentPath}/{folder}";
			string folderUrl = BuildUrl(currentPath) + "/";
			var checkRequest = new HttpRequestMessage(HttpMethod.Head, folderUrl);
			checkRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authValue);
			using var checkRes = await client.SendAsync(checkRequest, ct);
			if (checkRes.StatusCode == HttpStatusCode.NotFound)
			{
				_logger.LogInformation($"フォルダレベルの作成: {currentPath}");
				var mkcolRequest = new HttpRequestMessage(new HttpMethod("MKCOL"), folderUrl);
				mkcolRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", _authValue);

				using var mkcolRes = await client.SendAsync(mkcolRequest, ct);
				if (mkcolRes.StatusCode != HttpStatusCode.Created && mkcolRes.StatusCode != HttpStatusCode.MethodNotAllowed)
				{
					mkcolRes.EnsureSuccessStatusCode();
				}
			}
		}
	}

	/// <summary>
	/// パスの構築
	/// </summary>
	/// <param name="remotePath">リモートファイルパス</param>
	/// <returns>リモートファイルパス</returns>
	string BuildUrl(string remotePath)
	{
		string cleanPath = remotePath.TrimStart('/');
		var segments = cleanPath.Split('/');
		for (int i = 0; i < segments.Length; i++)
		{
			segments[i] = Uri.EscapeDataString(segments[i]);
		}
		string encodedPath = string.Join("/", segments);
		return $"{_options.BaseUrl}/{encodedPath}";
	}
}

public class WebDavOptions
{
	/// <summary>
	/// サーバー終了点
	/// </summary>
	public string BaseUrl { get; set; } = string.Empty;

	/// <summary>
	/// アカウント名
	/// </summary>
	public string Username { get; set; } = string.Empty;

	/// <summary>
	/// パスワード
	/// </summary>
	public string Password { get; set; } = string.Empty;

	/// <summary>
	/// 
	/// </summary>
	public bool IgnoreSslErrors { get; set; } = false;

	/// <summary>
	/// タイムアウト時間（秒）
	/// </summary>
	public int TimeoutSeconds { get; set; } = 30;
}
