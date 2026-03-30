namespace Aiko.UI;

//工区文本图片
public class CSelectKokuImg : ICustomShape
{
    #region 属性
    private float _x;
    public virtual float X { get => _x; set => _x = value; }

    private float _y;
    public virtual float Y { get => _y; set => _y = value; }

    private float _width;
    public virtual float Width { get => _width; set => _width = value; }

    private float _height;
    public virtual float Height { get => _height; set => _height = value; }

    private bool _selected;
    public virtual bool Selected { get => _selected; set => _selected = value; }

    private CustomShapeType _type = CustomShapeType.AreaTextImage;
    public virtual CustomShapeType Type { get => _type; }

    private CustomShapeFocusType _focuseType;
    public virtual CustomShapeFocusType FocusType { get => _focuseType; set => _focuseType = value; }

    private Microsoft.Maui.Graphics.IImage _source;
    public virtual Microsoft.Maui.Graphics.IImage Source { get => _source; set => _source = value; }

    private bool _isVisible = true;
    public bool IsVisible { get => _isVisible; set => _isVisible = value; }

    private string _text;
    public string Text { get => _text; set => _text = value; }

    private System.Drawing.RectangleF _rectBorder;
    public System.Drawing.RectangleF RectBorder { get => _rectBorder; }

    private Color _fontColor = Colors.Black;
    public Color FontColor { get => _fontColor; set => _fontColor = value; }

    private float _fontSize = 18;
    public float FontSize { get => _fontSize; set => _fontSize = value; }

    private IFont _font = Microsoft.Maui.Graphics.Font.Default;
    public IFont Font { get => _font; set => _font = value; }

    private bool _blLocked = false;
    public bool BlLocked { get => _blLocked; set => _blLocked = value; }

    #endregion

    public CSelectKokuImg(Microsoft.Maui.Graphics.IImage source)
    {
        _source = source;
    }

    public CSelectKokuImg()
    {

    }

    public ICustomShape SetAttributeValue(Func<ICustomShape, ICustomShape> func) => func.Invoke(this);

    private void SetRectBorderSize(SizeF stringSize)
    {
        if (!string.IsNullOrEmpty(_text))
            _rectBorder = new(_x - 3, _y - 3, _source.Width + stringSize.Width, stringSize.Height);
        else
            _rectBorder = new System.Drawing.RectangleF(_x - 3, _y - 3, _source.Width + 6, _source.Height + 6);

    }

    //绘图
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (_source == null) return;

        SizeF size = string.IsNullOrEmpty(_text) ? new() : canvas.GetStringSize(_text, _font, _fontSize, HorizontalAlignment.Left, VerticalAlignment.Center);
        SetRectBorderSize(size);

        canvas.FontColor = _fontColor;
        canvas.FontSize = _fontSize;
        canvas.Font = _font;

        if (_selected)
        {
            canvas.FillColor = Colors.LightBlue;
            canvas.FillRoundedRectangle(_rectBorder.X, _rectBorder.Y, _rectBorder.Width, _rectBorder.Height, 0);
        }
        canvas.DrawImage(_source, _x, _y, _source.Width, _source.Height);
        canvas.DrawString(_text, _x + _source.Width + 1, _y - 2, size.Width, size.Height, HorizontalAlignment.Left, VerticalAlignment.Center);
    }

    //指针移动
    public void MoveHoverInteraction(Microsoft.Maui.Graphics.PointF pt)
    {
        if (_rectBorder.Contains(new System.Drawing.PointF(pt.X, pt.Y)))
            _focuseType = CustomShapeFocusType.Move;
        else
            _focuseType = CustomShapeFocusType.Unfocused;
    }

    //按下
    public void StartInteraction() => _selected = _focuseType != CustomShapeFocusType.Unfocused ? true : false;
    
    //拖拽移动
    public void DragInteraction(Microsoft.Maui.Graphics.PointF pt, float horizontalDistance, float verticalDistance)
    {
        if (!_selected) return;
        if (_focuseType != CustomShapeFocusType.Move) return;

        _x += horizontalDistance;
        _y += verticalDistance;

    }

    //释放
    public void EndInteraction(PointF pt)
    {

    }
}

