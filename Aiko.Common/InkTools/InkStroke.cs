using SkiaSharp;

namespace Aiko.Common.InkTools
{
    public class InkStroke : IDisposable
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Type { get; set; } = "Empty";

        public SKPath Path { get; set; }
        public List<SKPoint> Points { get; set; } = new List<SKPoint>();

        public SKColor Color { get; set; }
        public float Size { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public string Text { get; set; } = string.Empty;
        public SKRect TextBounds { get; set; }
        public string Font { get; set; } = string.Empty;

        public InkStrokeDTO ToInkStrokeDTO()
        {
            return new InkStrokeDTO
            {
                Id = this.Id.ToString(),
                Type = this.Type,
                Points = this.Points.Select(point => new PointDTO { X = point.X, Y = point.Y }).ToList(),
                Color = $"#{this.Color.Red:X2}{this.Color.Green:X2}{this.Color.Blue:X2}",
                Size = this.Size,
                Width = this.Width,
                Height = this.Height,
                Text = this.Text,
                Font = this.Font
            };
        }

        public InkStroke Clone()
        {
            return new InkStroke
            {
                Id = this.Id,
                Type = this.Type,
                Path = this.Path != null ? new SKPath(this.Path) : null,
                Points = new List<SKPoint>(this.Points),
                Color = this.Color,
                Size = this.Size,
                Width = this.Width,
                Height = this.Height,
                Text = this.Text,
                TextBounds = this.TextBounds,
                Font = this.Font
            };
        }

        public void Dispose() => Path?.Dispose();
    }

    public class InkStrokeDTO
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public List<PointDTO> Points { get; set; }
        public string Color { get; set; }
        public float Size { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Text { get; set; }
        public string Font { get; set; }

        public InkStroke ToInkStroke()
        {
            var tool = InkService.Instance.GetTool(this.Type);

            if (tool != null)
            {
                try
                {
                    return tool.Rebuild(this);
                }
                catch
                {
                    // Rebuild 失败时暂时跳过
                }
            }

            return null;
        }
    }
    public class PointDTO
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
}