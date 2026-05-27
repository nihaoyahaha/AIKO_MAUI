using Microsoft.Maui.Graphics;

namespace Aiko.Common;

public enum MapShapeType
{
    Image,
    Rectangle,
    Polygon,
    Label
}

// 用于描述图元在切换显示/尺寸时应如何重新计算布局。
public enum MapShapeLayoutRole
{
    None,
    CheckPointIcon,
    CheckPointLabel,
    CheckPointPhotoCountIcon,
    CheckPointPhotoCountLabel,
    AreaIcon,
    AreaLabel
}

public sealed class MapShape
{
    public MapShapeType Type { get; set; }

    // 标记该图元在地图中的业务身份，例如确认点编号或工区编号。
    public MapShapeLayoutRole LayoutRole { get; set; }

    public string Tag { get; set; } = string.Empty;

    // 保存未缩放前的布局锚点，后续切换 SizeFlag 时基于它重算 Bounds。
    public Point LayoutOrigin { get; set; }

    public Rect Bounds { get; set; }

    public List<Point> Points { get; set; } = new();

    public string Text { get; set; } = string.Empty;

    public double FontSize { get; set; }

    public Color StrokeColor { get; set; } = Colors.Red;

    public double StrokeThickness { get; set; } = 3d;

    public Color TextColor { get; set; } = Colors.Black;

    public string ImageSource { get; set; } = string.Empty;

    public bool IsVisible { get; set; } = true;

    public int ZIndex { get; set; }

    public object? CommandParameter { get; set; }
}
