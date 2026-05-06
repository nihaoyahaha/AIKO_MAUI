using FluentFTP;
using Microsoft.Extensions.Logging;

namespace Aiko.SqliteDb;

public class FluentFtpClient
{
	readonly ILogger<FluentFtpClient> _logger;
	AsyncFtpClient _client = new AsyncFtpClient();
	public FluentFtpClient(ILogger<FluentFtpClient> logger)
	{
		_logger = logger;
	}

	/// <summary>
	/// FluentFtpClientクライアントの初期化
	/// </summary>
	public void InitializeFluentFtpClient(FluentFtpOptions options) 
	{
		_client = new AsyncFtpClient(options.Host , options.Username, options.Password , options.Port);
		_client.Config.ConnectTimeout = 15000;
		_client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
		if (options.UseSsl)
		{
			_client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
			_client.Config.ValidateAnyCertificate = true;
		}
	}

	/// <summary>
	/// ファイルのアップロード（リモートディレクトリの自動作成をサポート）
	/// </summary>
	/// <param name="localPath">ローカルファイルパス</param>
	/// <param name="remotePath">リモートファイルパス</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：アップロード成功、false：アップロード失敗</returns>
	public async Task<bool> UploadFileAsync(string localPath, string remotePath, CancellationToken ct = default)
	{
		try
		{
			if (!File.Exists(localPath))
			{
				_logger?.LogWarning($"ローカルファイルは存在しません: {localPath}");
				return false;
			}
			await EnsureConnectedAsync(ct);
			
			var status = await _client.UploadFile(
				localPath,
				remotePath,
				FtpRemoteExists.Overwrite,
				createRemoteDir: true,
				token: ct);

			return status == FtpStatus.Success;
		}
		catch (Exception ex)
		{
			_logger?.LogError($"FTPアップロードに失敗しました: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// ファイルのアップロード（リモートディレクトリの自動作成をサポート）
	/// </summary>
	/// <param name="stream">ファイルストリーム</param>
	/// <param name="remotePath">リモートサーバファイルパス</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：アップロード成功、false：アップロード失敗</returns>
	public async Task<bool> UploadStreamAsync(Stream stream, string remotePath, CancellationToken ct = default)
	{
		try
		{
			if (stream == null)
			{
				_logger.LogWarning("ftp-アップロードされたログファイルストリームが空です!");
				return false;
			}

			await EnsureConnectedAsync(ct);

			var status = await _client.UploadStream(
				stream,
				remotePath,
				FtpRemoteExists.Overwrite, 
				createRemoteDir: true,
				token: ct);

			return status == FtpStatus.Success;
		}
		catch (Exception ex)
		{
			_logger?.LogError($"FTPアップロードに失敗しました: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// ファイルのダウンロード
	/// </summary>
	/// <param name="remotePath">リモートファイルパス</param>
	/// <param name="localPath">ローカルファイルパス</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：ダウンロード成功、false：ダウンロード失敗</returns>
	public async Task<bool> DownloadAsync(string remotePath, string localPath,CancellationToken ct = default)
	{
		try
		{
			await EnsureConnectedAsync(ct);

			var localDir = Path.GetDirectoryName(localPath);
			if (!string.IsNullOrEmpty(localDir) && !Directory.Exists(localDir))
			{
				Directory.CreateDirectory(localDir);
			}
			if (!await FileExistsAsync(remotePath,ct)) 
			{
				_logger.LogWarning($"ftp-ダウンロード対象のファイルが見つかりません:{remotePath}");
				return false;
			}
			var status = await _client.DownloadFile(
				localPath,
				remotePath,
				FtpLocalExists.Overwrite,
				token: ct);

			return status == FtpStatus.Success;
		}
		catch (Exception ex)
		{
			_logger?.LogError($"FTPダウンロードに失敗しました: {ex.Message}");
			return false;
		}
	}

	/// <summary>
	/// リモートファイルの削除
	/// </summary>
	/// <param name="remotePath">リモートファイルパス</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：削除成功、false：削除失敗</returns>
	public async Task<bool> DeleteFileAsync(string remotePath, CancellationToken ct = default)
	{
		try
		{
			await EnsureConnectedAsync(ct);
			if (!await _client.FileExists(remotePath, ct))
			{
				return true;
			}
			await _client.DeleteFile(remotePath, ct);
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError($"ftp-ファイル削除に失敗しました:{remotePath},{ex.ToString()}");
			return false;
		}
	}

	/// <summary>
	/// ファイルが存在するかどうかをチェッ
	/// </summary>
	/// <param name="remotePath">リモートファイルパス</param>
	/// <param name="ct">キャンセルトークン</param>
	/// <returns>true：存在、false：存在しない</returns>
	public async Task<bool> FileExistsAsync(string remotePath, CancellationToken ct = default)
	{
		await EnsureConnectedAsync(ct);
		return await _client.FileExists(remotePath, ct);
	}

	/// <summary>
	/// 接続が確立されていることを確認します
	/// </summary>
	/// <param name="ct">キャンセルトークン</param>
	async Task EnsureConnectedAsync(CancellationToken ct)
	{
		if (!_client.IsConnected)
		{
			await _client.AutoConnect(ct);
		}
	}

	/// <summary>
	/// 接続解除
	/// </summary>
	public async Task DisconnectAsync()
	{
		if (_client.IsConnected)
		{
			await _client.Disconnect();
		}
	}
}

public class FluentFtpOptions
{
	public string Host { get; set; } = string.Empty;
	public int Port { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public bool UseSsl { get; set; } = false;
}
