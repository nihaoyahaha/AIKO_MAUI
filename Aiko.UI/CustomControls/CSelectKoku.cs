namespace Aiko.UI;

//矩形(工区)
public class CSelectKoku : ICustomShape
{
    #region 属性

    /// <summary>
    /// 点靠近图形时可，改变焦点的距离
    /// </summary>
    private const float _distance = 10;
    private string _id;
    public string Id { get => _id; }
    
    /// <summary>
    /// 图形在可编辑状态下，四周圆点的半径
    /// </summary>
    private const float _radius = 6;

    /// <summary>
    /// 锁定
    /// </summary>
    private bool _blLocked = false;
    public bool BlLocked { get => _blLocked; set => _blLocked = value; }

    private CSelectKokuImg _kokuImg;
    public CSelectKokuImg KokuImg { get => _kokuImg; set => _kokuImg = value; }

    private float _x;
    public virtual float X { get => _x; set => _x = value; }

    private float _y;
    public virtual float Y { get => _y; set => _y = value; }

    private float _width;
    public  float Width { get => _width; set => _width = value; }

    private float _height;
    public  float Height { get => _height; set => _height = value; }

    private Color _strokeColor;
    public  Color StrokeColor { get => _strokeColor; set => _strokeColor = value; }

    private Color _focusedColor;

    private Color _backgroundColor;
    public  Color BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

    private float _strokeSize;
    public  float StrokeSize { get => _strokeSize; set => _strokeSize = value; }

    private bool _selected;
    public  bool Selected { get => _selected; set => _selected = value; }

    private bool _kokuImgSelected;
    public bool KokuImgSelected { get => _kokuImgSelected; set => _kokuImgSelected = value; }

    private CustomShapeType _type = CustomShapeType.Rectangle;
    public  CustomShapeType Type { get => _type; }

    private CustomShapeFocusType _focuseType = CustomShapeFocusType.Unfocused;
    public  CustomShapeFocusType FocusType { get => _focuseType; set => _focuseType = value; }

    private bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set => _isVisible = value; }

    #endregion

    public CSelectKoku()
    {
        _id = Guid.NewGuid().ToString();
        _focusedColor = _strokeColor;
    }

    /// <summary>
    /// 矩形的可编辑点坐标和位置
    /// </summary>
    /// <returns></returns>
    private IEnumerable<(Point editPoint, CustomShapeFocusType focusType)> GetPointCollection()
    {
        yield return (new(_x, _y), CustomShapeFocusType.TopLeft);
        yield return (new(_x + _width / 2, _y), CustomShapeFocusType.TopCenter);
        yield return (new(_x + _width, _y), CustomShapeFocusType.TopRight);

        yield return (new(_x, _y + _height / 2), CustomShapeFocusType.MiddleLeft);
        yield return (new(_x + _width, _y + _height / 2), CustomShapeFocusType.MiddleRight);

        yield return (new(_x, _y + _height), CustomShapeFocusType.BottomLeft);
        yield return (new(_x + _width / 2, _y + _height), CustomShapeFocusType.BottomCenter);
        yield return (new(_x + _width, _y + _height), CustomShapeFocusType.BottomRight);
    }

    /// <summary>
    /// 点在图形上的位置
    /// </summary>
    /// <param name="pt"></param>
    /// <returns></returns>
    private (bool isNear, CustomShapeFocusType focusType) Near_Transform(Microsoft.Maui.Graphics.PointF pt)
    {
        var points = GetPointCollection();
        foreach (var focusPoint in points)
        {
            double distance = Math.Sqrt(Math.Pow(pt.X - focusPoint.editPoint.X, 2) + Math.Pow(pt.Y - focusPoint.editPoint.Y, 2));
            if (distance <= _radius)
            {
                return (true, focusPoint.focusType);
            }
        }

        return (false, CustomShapeFocusType.Unfocused);
    }

    /// <summary>
    /// 画布画矩形
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="shape"></param>
    private void DrawRectangle(ICanvas canvas)
    {
        if (_blLocked)
        {
            canvas.StrokeDashPattern = new float[] { 2, 2 };
        }
        canvas.StrokeColor = _focusedColor;
        canvas.StrokeSize = _strokeSize;
        canvas.DrawRectangle(_x, _y, _width, _height);

        if (_selected && !_kokuImgSelected)
        {
            foreach (var ellipse in GetPointCollection())
            {
                canvas.FillColor = Colors.Red;
                canvas.FillCircle((float)ellipse.editPoint.X, (float)ellipse.editPoint.Y, _radius);
            }
        }
    }

    /// <summary>
    /// 指针移动
    /// </summary>
    /// <param name="pt"></param>
    public void MoveHoverInteraction(Microsoft.Maui.Graphics.PointF pt)
    {
        RectF rectf = new(_x, _y, _width, _height);

        float outerLeftBoundary = rectf.Left - _distance;
        float innerLeftBoundary = rectf.Left + _distance;

        float outerRightBoundary = rectf.Right + _distance;
        float innerRightBoundary = rectf.Right - _distance;

        float outerTopBoundary = rectf.Top - _distance;
        float innerTopBoundary = rectf.Top + _distance;

        float outerBottomBoundary = rectf.Bottom + _distance;
        float innerBottomBoundary = rectf.Bottom - _distance;

        bool nearLeft = pt.X >= outerLeftBoundary
                     && pt.X <= innerLeftBoundary
                     && pt.Y >= outerTopBoundary
                     && pt.Y <= outerBottomBoundary;

        bool nearRight = pt.X >= innerRightBoundary
                     && pt.X <= outerRightBoundary
                     && pt.Y >= outerTopBoundary
                     && pt.Y <= outerBottomBoundary;

        bool nearTop = pt.Y >= outerTopBoundary
                     && pt.Y <= innerTopBoundary
                     && pt.X >= outerLeftBoundary
                     && pt.X <= outerRightBoundary;

        bool nearBottom = pt.Y >= innerBottomBoundary
                     && pt.Y <= outerBottomBoundary
                     && pt.X >= outerLeftBoundary
                     && pt.X <= outerRightBoundary;

        //判断点是否在矩形可缩放位置
        var tuple = Near_Transform(pt);
        _kokuImg?.MoveHoverInteraction(pt);
        if (_kokuImg?.FocusType == CustomShapeFocusType.Move)
        {
            _focuseType = CustomShapeFocusType.KokuImgPointMove;
        }
        else if (_selected && tuple.isNear)
        {
            _focuseType = tuple.focusType;
            _focusedColor = Colors.Yellow;
        }
        //判断点是否在矩形可移动位置
        else if (nearLeft || nearRight || nearTop || nearBottom)
        {
            _focuseType = CustomShapeFocusType.Move;
            _focusedColor = Colors.Blue;
        }
        //无焦点
        else
        {
            _focuseType = CustomShapeFocusType.Unfocused;
            _focusedColor = _strokeColor;
        }
        
    }

    /// <summary>
    /// 按下
    /// </summary>
    public virtual void StartInteraction()
    {
        _selected = _focuseType != CustomShapeFocusType.Unfocused ? true : false;
        _kokuImgSelected = _focuseType == CustomShapeFocusType.KokuImgPointMove ? true : false;
        _kokuImg?.StartInteraction();
    }

    /// <summary>
    /// 拖拽移动
    /// </summary>
    /// <param name="xAxisDistance">x轴移动距离</param>
    /// <param name="yAxisDistance">y轴移动距离</param>
    public void DragInteraction(Microsoft.Maui.Graphics.PointF pt, float horizontalDistance, float verticalDistance)
    {
        if (_blLocked) return;
        if (_selected)
        {
            switch (_focuseType)
            {
                case CustomShapeFocusType.Move:
                    _x += horizontalDistance;
                    _y += verticalDistance;
                    break;
                case CustomShapeFocusType.TopLeft:
                    _x += horizontalDistance;
                    _y += verticalDistance;
                    _width -= horizontalDistance;
                    _height -= verticalDistance;
                    break;
                case CustomShapeFocusType.TopCenter:
                    _y += verticalDistance;
                    _height -= verticalDistance;
                    break;
                case CustomShapeFocusType.TopRight:
                    _y += verticalDistance;
                    _width += horizontalDistance;
                    _height -= verticalDistance;
                    break;
                case CustomShapeFocusType.MiddleLeft:
                    _x += horizontalDistance;
                    _width -= horizontalDistance;
                    break;
                case CustomShapeFocusType.MiddleRight:
                    _width += horizontalDistance;
                    break;
                case CustomShapeFocusType.BottomLeft:
                    _x += horizontalDistance;
                    _width -= horizontalDistance;
                    _height += verticalDistance;
                    break;
                case CustomShapeFocusType.BottomCenter:
                    _height += verticalDistance;
                    break;
                case CustomShapeFocusType.BottomRight:
                    _width += horizontalDistance;
                    _height += verticalDistance;
                    break;
                case CustomShapeFocusType.KokuImgPointMove:
                    _kokuImg?.DragInteraction(pt, horizontalDistance, verticalDistance);
                    break;
            }
        }
        
    }

    /// <summary>
    /// 释放(无论用户如何拉框，释放后矩形长宽总是整数)
    /// </summary>
    /// <param name="pt"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void EndInteraction(PointF pt)
    {
        float x = _x;
        float y = _y;
        float width = _width;
        float height = _height;
        if (width < 0)
        {
            _x = x + width;
            _width = Math.Abs(width);
        }
        if (height < 0)
        {
            _y = y + height;
            _height = Math.Abs(height);
        }
        _kokuImg?.EndInteraction(pt);
    }

    /// <summary>
    /// 画图
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="dirtyRect"></param>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        DrawRectangle(canvas);
        _kokuImg?.Draw(canvas, dirtyRect);
    }

    /// <summary>
    /// 矩形转多边形
    /// </summary>
    /// <returns></returns>
    public CSelectPolygon GetPolygon()
    {
        PathF pathF = new(_x, _y);
        //顶中
        pathF.InsertLineTo(new(_x + _width / 2, _y), 1);
        //顶右
        pathF.InsertLineTo(new(_x + _width, _y), 2);
        //右中
        pathF.InsertLineTo(new(_x + _width, _y + _height / 2), 3);
        //右下
        pathF.InsertLineTo(new(_x + _width, _y + _height), 4);
        //下中
        pathF.InsertLineTo(new(_x + _width / 2, _y + _height), 5);
        //下左
        pathF.InsertLineTo(new(_x, _y + _height), 6);
        //左中
        pathF.InsertLineTo(new(_x, _y + _height / 2), 7);

        pathF.Close();

        return new CSelectPolygon(_id)
        {
            StrokeColor = _strokeColor,
            StrokeSize = _strokeSize,
            X = _x,
            Y = _y,
            Width = _width,
            Height = _height,
            GetPathF = pathF,
            BlLocked = _blLocked
        };
    }

    /// <summary>
    /// 自定义实例业务
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public ICustomShape SetAttributeValue(Func<ICustomShape, ICustomShape> func) => func(this);

    /// <summary>
    /// 判断点是否在工区内
    /// </summary>
    /// <param name="pt"></param>
    /// <returns></returns>
    public bool IsContainsPoint(Point pt)
    {
        if (new RectF(_x, _y, _width, _height).Contains(pt))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 返回矩形位置信息
    /// </summary>
    /// <returns></returns>
    public Microsoft.Maui.Graphics.RectF GetRectF() {
        return new RectF(_x,_y,_width,_height);
    }
}

