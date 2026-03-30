using System;
namespace Aiko.UI;

/// <summary>
/// 聚焦图形时的状态
/// </summary>
public enum CustomShapeFocusType
{
    /// <summary>
    /// 未聚焦
    /// </summary>
    Unfocused,
    /// <summary>
    /// 顶部左侧
    /// </summary>
    TopLeft,
    /// <summary>
    /// 顶部中央
    /// </summary>
    TopCenter,
    /// <summary>
    /// 顶部右侧
    /// </summary>
    TopRight,
    /// <summary>
    /// 左侧中央
    /// </summary>
    MiddleLeft,
    /// <summary>
    /// 右侧中央
    /// </summary>
    MiddleRight,
    /// <summary>
    /// 底部左侧
    /// </summary>
    BottomLeft,
    /// <summary>
    /// 底部中央
    /// </summary>
    BottomCenter,
    /// <summary>
    /// 底部右侧
    /// </summary>
    BottomRight,
    /// <summary>
    /// 移动
    /// </summary>
    Move,
    /// <summary>
    /// 多边形点移动
    /// </summary>
    PolygonPointMove,
    /// <summary>
    /// 工区图片点移动
    /// </summary>
    KokuImgPointMove,
    /// <summary>
    /// 红色点线移动
    /// </summary>
    RedPointLineMove,
    /// <summary>
    /// 蓝色点线移动
    /// </summary>
    BluePointLineMove,
    /// <summary>
    /// 红色点线旋转
    /// </summary>
    RedPointLineRotate,
    /// <summary>
    /// 蓝色点线旋转
    /// </summary>
    BluePointLineRotate
}


