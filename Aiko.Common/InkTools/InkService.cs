using SkiaSharp;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aiko.Common.InkTools
{
    public class InkService
    {
        private static readonly Lazy<InkService> _instance = new(() => new InkService());
        public static InkService Instance => _instance.Value;

        private readonly Dictionary<string, IInkTool> _toolCache = new();

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private InkService()
        {
            InitializeTools();
        }

        private void InitializeTools()
        {
            // 获取当前程序集中所有继承自 InkTool 的非抽象类
            var toolTypes = Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.IsSubclassOf(typeof(InkTool)) && !t.IsAbstract);

            foreach (var type in toolTypes)
            {
                try
                {
                    // 传入 null 构造 manager，仅用于 Rebuild 和 Draw
                    var tool = (IInkTool?)Activator.CreateInstance(type, new object[] { null! });
                    if (tool != null && !_toolCache.ContainsKey(tool.Type))
                    {
                        _toolCache.Add(tool.Type, tool);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"初始化工具失败: {type.Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 获取指定类型的工具实例
        /// </summary>
        public IInkTool? GetTool(string type)
        {
            return _toolCache.TryGetValue(type, out var tool) ? tool : null;
        }

        /// <summary>
        /// 导出为 Base64 图片
        /// </summary>
        public string ExportToBase64(int width, int height, IEnumerable<InkStroke> strokes)
        {
            if (width <= 0 || height <= 0) return string.Empty;

            var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.Transparent);

            foreach (var stroke in strokes)
            {
                var tool = GetTool(stroke.Type);
                tool?.Draw(canvas, stroke);
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return Convert.ToBase64String(data.ToArray());
        }

        public string ToJson(List<InkStroke> strokes)
        {
            var dtos = strokes.Select(stroke => stroke.ToInkStrokeDTO()).ToList();
            return JsonSerializer.Serialize(dtos, _jsonOptions);
        }

        public T FromJson<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json)) return null;

            try
            {
                if (typeof(T) == typeof(InkStroke))
                {
                    var dto = JsonSerializer.Deserialize<InkStrokeDTO>(json, _jsonOptions);
                    var stroke = dto?.ToInkStroke();
                    return stroke as T;
                }

                if (typeof(T) == typeof(List<InkStroke>))
                {
                    var dtos = JsonSerializer.Deserialize<List<InkStrokeDTO>>(json, _jsonOptions);
                    var strokes = dtos?.Select(dto => dto.ToInkStroke()).Where(stroke => stroke != null).ToList();
                    return strokes as T;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解析Json失败: {ex.Message}");
            }

            return null;
        }
    }
}
