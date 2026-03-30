namespace Aiko.UI;

//区域红蓝坐标
public class CControlPoint : ICustomShape
{
    #region 属性
    private const float _distance = 10;
    /// <summary>
    /// 区域矩形在可编辑状态下，四周圆点的半径
    /// </summary>
    private const float _radius = 6;
    /// <summary>
    /// 红蓝点长宽
    /// </summary>
    private const float _pointSize = 10;
    private const float _pointSelectedDistance = 120;
    private const float _pointSelectedSize = 7;

    private CustomShapeType _type = CustomShapeType.AreaPointLine;
    public CustomShapeType Type { get => _type; }

    private Color _strokeColor = Colors.Red;
    public virtual Color StrokeColor { get => _strokeColor; set => _strokeColor = value; }

    //矩形x坐标
    private float _x;
    public float X { get => _x; set => _x = value; }

    //矩形y坐标
    private float _y;
    public float Y { get => _y; set => _y = value; }

    //矩形宽度
    private float _width;
    public float Width { get => _width; set => _width = value; }

    //矩形高度
    private float _height;
    public float Height { get => _height; set => _height = value; }

    //红点x坐标
    private float _redPointx;
    public float RedPointX { get => _redPointx; set => _redPointx = value; }

    //红点y坐标
    private float _redPointy;
    public float RedPointY { get => _redPointy; set => _redPointy = value; }

    //红点矩形边框
    private Microsoft.Maui.Graphics.RectF _rectRedPointBorder;
    //红点矩形旋转点边框
    private Microsoft.Maui.Graphics.RectF _rectRedPointLineRotateBorder;

    //蓝点x坐标
    private float _bluePointx;
    public float BluePointX { get => _bluePointx; set => _bluePointx = value; }

    //蓝点y坐标
    private float _bluePointy;
    public float BluePointY { get => _bluePointy; set => _redPointy = value; }

    //蓝点矩形边框
    private Microsoft.Maui.Graphics.RectF _rectBluePointBorder;
    //蓝点矩形旋转点边框
    private Microsoft.Maui.Graphics.RectF _rectBluePointLineRotateBorder;

    //矩形选中状态
    private bool _selected;
    public bool Selected { get => _selected; set => _selected = value; }

    //红点选中状态
    private bool _redPointSelected;
    public bool RedPointSelected { get => _redPointSelected; set => _redPointSelected = value; }

    //蓝点选中状态
    private bool _bluePointSelected;
    public bool BluePointSelected { get => _bluePointSelected; set => _bluePointSelected = value; }

    //焦点状态
    private CustomShapeFocusType _focusType;
    public CustomShapeFocusType FocusType { get => _focusType; set => _focusType = value; }

    //红蓝点旋转角度
    private float _angle;
    public float Angle { get => _angle; set => _angle = value; }

    private bool _isVisible;
    public bool IsVisible { get => _isVisible; set => _isVisible = value; }

    private bool _blLocked = false;
    public bool BlLocked { get => _blLocked; set => _blLocked = value; }
    #endregion

    public CControlPoint(Microsoft.Maui.Graphics.RectF rectF, float redPointx, float redPointy, float bluePointx, float bluePointy, float angle)
    {
        _x = rectF.X;
        _y = rectF.Y;
        _width = rectF.Width;
        _height = rectF.Height;
        _redPointx = redPointx;
        _redPointy = redPointy;
        _bluePointx = bluePointx;
        _bluePointy = bluePointy;
        _angle = angle;
        SetRedPointBorder();
        SetBluePointBorder();
    }

    private void SetRedPointBorder()
    {
        _rectRedPointBorder = new RectF(_redPointx - _pointSize, _redPointy - _pointSize, _pointSize * 2, _pointSize * 2);
    }

    private void SetBluePointBorder()
    {
        _rectBluePointBorder = new RectF(_bluePointx - _pointSize, _bluePointy - _pointSize, _pointSize * 2, _pointSize * 2);
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

    //画区域边框
    private void DrawAreaBorder(ICanvas canvas)
    {
        canvas.StrokeColor = _strokeColor;
        canvas.StrokeSize = 5;
        canvas.DrawRectangle(_x, _y, _width, _height);

        if (_selected && !_redPointSelected && !_bluePointSelected)
        {
            foreach (var ellipse in GetPointCollection())
            {
                canvas.FillColor = Colors.Red;
                canvas.FillCircle((float)ellipse.editPoint.X, (float)ellipse.editPoint.Y, _radius);
            }
        }
    }

    //画红色点线
    private void DrawRedPointLine(ICanvas canvas)
    {
        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 2;
        canvas.FillColor = Colors.Red;
        canvas.FillRectangle(_rectRedPointBorder.X, _rectRedPointBorder.Y, _rectRedPointBorder.Width, _rectRedPointBorder.Height);
        canvas.DrawLine(_redPointx, 0, _redPointx, float.MaxValue);
        canvas.DrawLine(0, _redPointy, float.MaxValue, _redPointy);
        if (_redPointSelected)
        {
            var x = _redPointx - _pointSelectedSize;
            var y = _redPointy - _pointSelectedDistance - _pointSelectedSize;
            var widthdOrHeight = _pointSelectedSize * 2;
            if (_focusType == CustomShapeFocusType.RedPointLineRotate)
            {
                canvas.FillColor = Colors.Black;
            }
            canvas.FillRectangle(x, y, widthdOrHeight, widthdOrHeight);
            _rectRedPointLineRotateBorder = new(x, y, widthdOrHeight, widthdOrHeight);
        }
        else
        {
            _rectRedPointLineRotateBorder = new();
        }
    }

    //画蓝色点线
    private void DrawBluePointLine(ICanvas canvas)
    {
        canvas.StrokeSize = 2;
        canvas.StrokeColor = Colors.Blue;
        canvas.FillColor = Colors.Blue;
        canvas.FillRectangle(_rectBluePointBorder.X, _rectBluePointBorder.Y, _rectBluePointBorder.Width, _rectBluePointBorder.Height);

        canvas.DrawLine(_bluePointx, 0, _bluePointx, float.MaxValue);
        canvas.DrawLine(0, _bluePointy, float.MaxValue, _bluePointy);
        if (_bluePointSelected)
        {
            var x = _bluePointx - _pointSelectedSize;
            var y = _bluePointy - _pointSelectedDistance - _pointSelectedSize;
            var widthOrHeight = _pointSelectedSize * 2;
            if (_focusType == CustomShapeFocusType.BluePointLineRotate)
            {
                canvas.FillColor = Colors.Black;
            }
            canvas.FillRectangle(x, y, widthOrHeight, widthOrHeight);
            _rectBluePointLineRotateBorder = new(x, y, widthOrHeight, widthOrHeight);
        }
        else
        {
            _rectBluePointLineRotateBorder = new();
        }
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

    //画图
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        DrawAreaBorder(canvas);
        DrawRedPointLine(canvas);
        DrawBluePointLine(canvas);
    }

    //指针移动
    public void MoveHoverInteraction(PointF pt)
    {
        RectF rectf = new(_x, _y, _width, _height);
        float outerLeftBoundary = rectf.Left - _distance;
        float innerLeftBoundary = rectf.Left;

        float outerRightBoundary = rectf.Right + _distance;
        float innerRightBoundary = rectf.Right;

        float outerTopBoundary = rectf.Top - _distance;
        float innerTopBoundary = rectf.Top;

        float outerBottomBoundary = rectf.Bottom + _distance;
        float innerBottomBoundary = rectf.Bottom;

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

        //红点
        if (_rectRedPointBorder.Contains(pt))
        {
            _focusType = CustomShapeFocusType.RedPointLineMove;
        }
        else if (_rectRedPointLineRotateBorder.Contains(pt))
        {
            _focusType = CustomShapeFocusType.RedPointLineRotate;
        }
        //蓝点
        else if (_rectBluePointBorder.Contains(pt))
        {
            _focusType = CustomShapeFocusType.BluePointLineMove;
        }
        else if (_rectBluePointLineRotateBorder.Contains(pt))
        {
            _focusType = CustomShapeFocusType.BluePointLineRotate;
        }
        //矩形框
        else
        {
            //判断点是否在矩形可缩放位置
            var tuple = Near_Transform(pt);
            if (_selected && tuple.isNear)
            {
                _focusType = tuple.focusType;
                _strokeColor = Colors.Yellow;
            }
            else if (new RectF(_x, _y, _width, _height).Contains(pt) || nearLeft || nearRight || nearTop || nearBottom)
            {
                _focusType = CustomShapeFocusType.Move;
                _strokeColor = Colors.Blue;
            }
            else
            {
                _focusType = CustomShapeFocusType.Unfocused;
                _strokeColor = Colors.Red;
            }
        }

    }

    //按下
    public void StartInteraction()
    {
        _selected = _focusType != CustomShapeFocusType.Unfocused ? true : false;
        _redPointSelected = _focusType == CustomShapeFocusType.RedPointLineMove ? true : false;
        _bluePointSelected = _focusType == CustomShapeFocusType.BluePointLineMove ? true : false;
    }

    //拖拽移动
    public void DragInteraction(PointF pt, float horizontalDistance, float verticalDistance)
    {
        if (_selected)
        {
            switch (_focusType)
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
                case CustomShapeFocusType.RedPointLineMove:
                    _redPointx += horizontalDistance;
                    _redPointy += verticalDistance;
                    SetRedPointBorder();
                    break;
                case CustomShapeFocusType.BluePointLineMove:
                    _bluePointx += horizontalDistance;
                    _bluePointy += verticalDistance;
                    SetBluePointBorder();
                    break;

            }
        }
    }

    //释放
    public void EndInteraction(PointF pt)
    {

    }

    public ICustomShape SetAttributeValue(Func<ICustomShape, ICustomShape> func) => func.Invoke(this);

}


