using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Aiko.Common;

public class DownLoadTime
{
	[JsonPropertyName("ProjectCode")]
	public string ProjectCode { get; set; } = string.Empty;

	[JsonPropertyName("TableName")]
	public string TableName { get; set; } = string.Empty;

	[JsonPropertyName("Time")]
	public string Time { get; set; } = string.Empty;
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<DownLoadTime>))]
internal partial class DownLoadTimeContext : JsonSerializerContext
{
}