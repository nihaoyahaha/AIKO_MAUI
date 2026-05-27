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
                    System.Diagnostics.Debug.WriteLine($"Initialization tool failed: {type.Name}: {ex.Message}");
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

        public void LoadTypefaces(Dictionary<string, SKTypeface> typefaces)
        {
            foreach (var tool in _toolCache.Values)
            {
                if (tool is TextTool textTool)
                {
                    textTool.Typefaces = typefaces;
                }
            }
        }

        /// <summary>
        /// 导出为 Base64 图片
        /// </summary>
        public string ExportToBase64(int width, int height, IEnumerable<InkStroke> strokes, string[]? allowedTypes, string[]? bannedTypes)
        {
            if (width <= 0 || height <= 0) return string.Empty;

            var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.Transparent);

            foreach (var stroke in strokes)
            {
                if (!((allowedTypes == null || allowedTypes.Contains(stroke.Type)) && (bannedTypes == null || !bannedTypes.Contains(stroke.Type)))) continue;

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
                System.Diagnostics.Debug.WriteLine($"JSON parsing failed: {ex.Message}");
            }

            return null;
        }

        // --- 平滑插值算法 ---

        /// <summary>
        /// 使用 Catmull-Rom 样条算法平滑点集并增加长度
        /// </summary>
        /// <param name="points">原始点集</param>
        /// <param name="subdivisions">每两个相邻点之间插入的细分点数量（值越大，结果越长越平滑）</param>
        /// <returns>更长、更平滑的新点集</returns>
        public static List<SKPoint> SmoothLine(List<SKPoint> points, int subdivisions = 10)
        {
            if (points == null || points.Count < 2)
            {
                return points ?? new List<SKPoint>();
            }

            List<SKPoint> smoothedPoints = new List<SKPoint>();

            // 为了计算边界点的曲率，我们在首尾各虚拟复制一个点
            List<SKPoint> extendedPoints = new List<SKPoint>();
            extendedPoints.Add(points[0]); // 虚拟起点
            extendedPoints.AddRange(points);
            extendedPoints.Add(points[points.Count - 1]); // 虚拟终点

            // 遍历所有原始线段
            for (int i = 1; i < extendedPoints.Count - 2; i++)
            {
                SKPoint p0 = extendedPoints[i - 1];
                SKPoint p1 = extendedPoints[i];
                SKPoint p2 = extendedPoints[i + 1];
                SKPoint p3 = extendedPoints[i + 2];

                // 在 p1 和 p2 之间进行插值
                for (int j = 0; j < subdivisions; j++)
                {
                    float t = (float)j / subdivisions;
                    SKPoint interpolatedPoint = GetCatmullRomPosition(t, p0, p1, p2, p3);
                    smoothedPoints.Add(interpolatedPoint);
                }
            }

            // 把最后一个原始点加进去，确保曲线完整
            smoothedPoints.Add(points[points.Count - 1]);

            return smoothedPoints;
        }

        /// <summary>
        /// Catmull-Rom 公式计算
        /// </summary>
        private static SKPoint GetCatmullRomPosition(float t, SKPoint p0, SKPoint p1, SKPoint p2, SKPoint p3)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            // Catmull-Rom 特征矩阵系数计算
            float x = 0.5f * ((2f * p1.X) +
                             (-p0.X + p2.X) * t +
                             (2f * p0.X - 5f * p1.X + 4f * p2.X - p3.X) * t2 +
                             (-p0.X + 3f * p1.X - 3f * p2.X + p3.X) * t3);

            float y = 0.5f * ((2f * p1.Y) +
                             (-p0.Y + p2.Y) * t +
                             (2f * p0.Y - 5f * p1.Y + 4f * p2.Y - p3.Y) * t2 +
                             (-p0.Y + 3f * p1.Y - 3f * p2.Y + p3.Y) * t3);

            return new SKPoint(x, y);
        }
    }
}
