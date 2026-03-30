namespace Aiko.UI;

//多边形(工区)
public class CSelectPolygon : ICustomShape
{
    #region 属性
    private const float _radius = 6;
    private int _focusedPointIndex = -1;
    private int _focusedLineIndex = -1;
    private PointF _focusedPoint;
    public string Id { get; init; }
    private PathF _pathF;
    public virtual PathF GetPathF { get => _pathF; set => _pathF = value; }

    private float _x;
    public virtual float X { get => _x; set => _x = value; }

    private float _y;
    public virtual float Y { get => _y; set => _y = value; }

    private float _width;
    public virtual float Width { get => _width; set => _width = value; }

    private float _height;
    public virtual float Height { get => _height; set => _height = value; }

    private CSelectKokuImg _kokuImg;
    public CSelectKokuImg KokuImg { get => _kokuImg; set => _kokuImg = value; }

    private Color _focusedColor;

    private Color _strokeColor;
    public virtual Color StrokeColor { get => _strokeColor; set => _strokeColor = value; }

    private Color _backgroundColor;
    public virtual Color BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

    private float _strokeSize;
    public virtual float StrokeSize { get => _strokeSize; set => _strokeSize = value; }

    private bool _selected;
    public virtual bool Selected { get => _selected; set => _selected = value; }

    private bool _kokuImgSelected;
    public bool KokuImgSelected { get => _kokuImgSelected; set => _kokuImgSelected = value; }

    private CustomShapeType _type = CustomShapeType.Polygon;
    public virtual CustomShapeType Type { get => _type; }

    private CustomShapeFocusType _focuseType;
    public virtual CustomShapeFocusType FocusType { get => _focuseType; set => _focuseType = value; }

    private bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set => _isVisible = value; }

    private bool _blLocked = false;
    public bool BlLocked { get => _blLocked; set => _blLocked = value; }
    #endregion

    public CSelectPolygon(string id)
    {
        Id = id;
        _focusedColor = _strokeColor;
    }

    //绘制多边形
    private void DrawPath(ICanvas canvas)
    {
        if (_blLocked)
        {
            canvas.StrokeDashPattern = new float[] { 2, 2 };
        }
        canvas.StrokeColor = _focusedColor;
        canvas.StrokeSize = _strokeSize;
        canvas.StrokeLineJoin = LineJoin.Round;
        canvas.DrawPath(_pathF);

        if (_selected && !_kokuImgSelected)
        {
            foreach (var ellipse in _pathF.Points)
            {
                canvas.FillColor = Colors.Red;
                canvas.FillCircle((float)ellipse.X, (float)ellipse.Y, _radius);
            }
        }
    }

    //点在图形上的位置
    private (bool isNear, CustomShapeFocusType focusType) Near_Transform(Microsoft.Maui.Graphics.PointF pt)
    {
        _focusedPointIndex = -1;
        var index = -1;
        foreach (var focusPoint in _pathF.Points)
        {
            index++;
            double distance = Math.Sqrt(Math.Pow(pt.X - focusPoint.X, 2) + Math.Pow(pt.Y - focusPoint.Y, 2));
            if (distance <= _radius)
            {
                _focusedPointIndex = index;
                return (true, CustomShapeFocusType.PolygonPointMove);
            }
        }

        return (false, CustomShapeFocusType.Unfocused);
    }

    //判断点是否在线上
    public bool GetPointIsInLine(PointF pf, PointF p1, PointF p2, double range)
    {
        pf = new(Convert.ToInt64(Math.Round(pf.X / 1, 0)), Convert.ToInt64(Math.Round(pf.Y / 1, 0)));
        double cross = (p2.X - p1.X) * (pf.X - p1.X) + (p2.Y - p1.Y) * (pf.Y - p1.Y);
        if (cross <= 0) return false;
        double d2 = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y);
        if (cross >= d2) return false;

        double r = cross / d2;
        double px = p1.X + (p2.X - p1.X) * r;
        double py = p1.Y + (p2.Y - p1.Y) * r;

        return Math.Sqrt((pf.X - px) * (pf.X - px) + (py - pf.Y) * (py - pf.Y)) <= range;
    }

    //操作本对象
    public ICustomShape SetAttributeValue(Func<ICustomShape, ICustomShape> func) => func.Invoke(this);

    //删除点
    public void DelPoint()
    {
        if (_pathF.GetSegmentPointIndex(_focusedPointIndex) != -1)
            _pathF.RemoveSegment(_focusedPointIndex);
    }

    //加点
    public void AddPoint()
    {
        if (_focusedLineIndex != -1)
        {
            if (_focusedLineIndex == 0)
                _pathF.InsertLineTo(_focusedPoint, _pathF.Points.Count());
            else
                _pathF.InsertLineTo(_focusedPoint, _focusedLineIndex);
        }
    }

    //指针移动
    public void MoveHoverInteraction(Microsoft.Maui.Graphics.PointF pt)
    {
        var have = false;
        _focusedLineIndex = -1;
        var index = -1;
        for (int i = 0; i < _pathF.Points.Count(); i++)
        {
            index++;
            if (i == 0)
            {
                if (GetPointIsInLine(pt, _pathF.Points.ToArray()[_pathF.Points.Count() - 1], _pathF.Points.ToArray()[i], 5.0))
                {
                    _focuseType = CustomShapeFocusType.Move;
                    _focusedColor = Colors.Blue;
                    have = true;
                    _focusedLineIndex = index;
                    break;
                }
            }
            else if (GetPointIsInLine(pt, _pathF.Points.ToArray()[i - 1], _pathF.Points.ToArray()[i], 5.0))
            {
                _focuseType = CustomShapeFocusType.Move;
                _focusedColor = Colors.Blue;
                have = true;
                _focusedLineIndex = index;
                break;
            }
        }
        var tuple = Near_Transform(pt);
        _kokuImg?.MoveHoverInteraction(pt);
        _focusedPoint = pt;
        if (_kokuImg?.FocusType == CustomShapeFocusType.Move)
        {
            _focuseType = CustomShapeFocusType.KokuImgPointMove;
        }
        else if (_selected && tuple.isNear)
        {
            _focuseType = tuple.focusType;
            _focusedColor = Colors.Yellow;
        }
        else if (have)
        {
            _focuseType = CustomShapeFocusType.Move;
            _focusedColor = Colors.Blue;
        }
        else
        {
            _focuseType = CustomShapeFocusType.Unfocused;
            _focusedColor = _strokeColor;
        }
    }

    //按下
    public virtual void StartInteraction()
    {
        _selected = _focuseType != CustomShapeFocusType.Unfocused ? true : false;
        _kokuImgSelected = _focuseType == CustomShapeFocusType.KokuImgPointMove ? true : false;
        _kokuImg?.StartInteraction();
    }

    //拖拽移动
    public void DragInteraction(Microsoft.Maui.Graphics.PointF pt, float horizontalDistance, float verticalDistance)
    {
        if (_blLocked) return;
        if (!_selected) return;

        switch (_focuseType) {
            case CustomShapeFocusType.Move:
                _pathF.Move(horizontalDistance, verticalDistance);
                break;
            case CustomShapeFocusType.PolygonPointMove:
                _pathF.SetPoint(_focusedPointIndex, pt.X, pt.Y);
                break;
            case CustomShapeFocusType.KokuImgPointMove:
                _kokuImg?.DragInteraction(pt, horizontalDistance, verticalDistance);
                break;
        }
    }

    //释放
    public void EndInteraction(PointF pt)
    {

    }

    //绘图
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        DrawPath(canvas);
        _kokuImg?.Draw(canvas, dirtyRect);
    }

}


