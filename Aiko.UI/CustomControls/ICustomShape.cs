namespace Aiko.UI;

public interface ICustomShape
{
    /// <summary>
    /// 形状类型
    /// </summary>
    CustomShapeType Type { get; }
    /// <summary>
    /// 横坐标
    /// </summary>
    float X { get; set; }
    /// <summary>
    /// 纵坐标
    /// </summary>
    float Y { get; set; }
    /// <summary>
    /// 宽度
    /// </summary>
    float Width { get; set; }
    /// <summary>
    /// 高度
    /// </summary>
    float Height { get; set; }
    /// <summary>
    /// 是否被选中
    /// </summary>
    bool Selected { get; set; }
    /// <summary>
    /// 是否被锁定
    /// </summary>
    bool BlLocked { get; set; }
    /// <summary>
    /// 焦点状态
    /// </summary>
    CustomShapeFocusType FocusType { get; set; }
    /// <summary>
    /// 显示隐藏
    /// </summary>
    bool IsVisible { get; set; }
    /// <summary>
    /// 指针移动(判断点是否在图形附近)
    /// </summary>
    /// <param name="pt"></param>
    void MoveHoverInteraction(Microsoft.Maui.Graphics.PointF pt);
    /// <summary>
    /// 按下
    /// </summary>
    void StartInteraction();
    /// <summary>
    /// 拖拽移动
    /// </summary>
    /// <param name="pt">点所在位置</param>
    /// <param name="horizontalDistance">水平移动距离</param>
    /// <param name="verticalDistance">垂直移动距离</param>
    void DragInteraction(Microsoft.Maui.Graphics.PointF pt, float horizontalDistance, float verticalDistance);
    /// <summary>
    /// 释放
    /// </summary>
    /// <param name="pt"></param>
    void EndInteraction(Microsoft.Maui.Graphics.PointF pt);
    /// <summary>
    /// 在画布上绘制形状
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="dirtyRect"></param>
    void Draw(ICanvas canvas, RectF dirtyRect);
    /// <summary>
    /// 属性赋值
    /// </summary>
    /// <param name="func">具体操作行为</param>
    /// <returns>继承自借口的对象</returns>
    ICustomShape SetAttributeValue(Func<ICustomShape, ICustomShape> func);
}

