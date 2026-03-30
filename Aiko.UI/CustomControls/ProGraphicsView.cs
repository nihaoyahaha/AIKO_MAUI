namespace Aiko.UI;

public class ProGraphicsView : GraphicsView
{
    #region 属性
    private GraphicsDrawable _graphicsDrawable = new();

    /// <summary>
    /// 获取图形信息
    /// </summary>
    public List<ICustomShape> Shapes
    {
        get {
            return _graphicsDrawable.Shapes;
        }
        set
        {
            _graphicsDrawable.Shapes = value;
        }
    }

    /// <summary>
    /// 显示隐藏确认点文本
    /// </summary>
    private bool _kokuImgTextIsVisible;
    public static readonly BindableProperty KokuImgTextIsVisibleProperty = BindableProperty.Create(nameof(KokuImgTextIsVisible), typeof(bool), typeof(ProGraphicsView), true);
    public bool KokuImgTextIsVisible
    {
        get => (bool)GetValue(KokuImgTextIsVisibleProperty);
        set
        {
            _kokuImgTextIsVisible = value;
            SetValue(KokuImgTextIsVisibleProperty, value);
            SetImgTextVisible();
        }
    }

    private CustomShapeType _ctmShapeType ;
    /// <summary>
    /// 自定义形状类型
    /// </summary>
    public static readonly BindableProperty ShapeTypeProperty = BindableProperty.Create(nameof(ShapeType), typeof(CustomShapeType), typeof(ProGraphicsView));
    public CustomShapeType ShapeType
    {
        get => (CustomShapeType)GetValue(ShapeTypeProperty);
        set
        {
            _ctmShapeType = value;
            _graphicsDrawable.ShapeType = _ctmShapeType;
            SetValue(ShapeTypeProperty, value);
        }
    }

    /// <summary>
    /// 是否锁定工区
    /// </summary>
    public static readonly BindableProperty IsLockKoKuProperty = BindableProperty.Create(nameof(IsLockKoKu),typeof(bool),typeof(ProGraphicsView));
    public bool IsLockKoKu
    {
        get => (bool)GetValue(IsLockKoKuProperty);
        set
        {
            SetValue(IsLockKoKuProperty, value);
            LockCSelectKoku(value);
        }
    }

    /// <summary>
    /// 是否锁定确认点 
    /// </summary>
    public static readonly BindableProperty IsLockCheckPointProperty = BindableProperty.Create(nameof(IsLockCheckPoint), typeof(bool), typeof(ProGraphicsView));
    public bool IsLockCheckPoint
    {
        get => (bool)GetValue(IsLockCheckPointProperty);
        set
        {
            SetValue(IsLockCheckPointProperty, value);
            LockCCheckPoint(value);
        }
    }

    #endregion

    public ProGraphicsView()
    {
        this.GestureRecognizers.Add(new TapGestureRecognizer {  }) ;

        //this.StartInteraction += ProGraphicsView_StartInteraction;
        //this.DragInteraction += ProGraphicsView_DragInteraction;
        //this.EndInteraction += ProGraphicsView_EndInteraction;
        //this.MoveHoverInteraction += ProGraphicsView_MoveHoverInteraction;

        this.Drawable = _graphicsDrawable;
        _graphicsDrawable.ShapeType = _ctmShapeType;
        
        this.Invalidate();
        //SetContextMenu();
    }

    /// <summary>
    /// 设置右键上下文菜单
    /// </summary>
    private void SetContextMenu() {
        MenuFlyout menu = new MenuFlyout();
        MenuFlyoutItem item = new MenuFlyoutItem { Text = "123" };
        MenuFlyoutItem item2 = new MenuFlyoutItem { Text = "456" };
        if (_ctmShapeType == CustomShapeType.Polygon){
            menu.Add(item);
        }
        
        menu.Add(item2);
        menu.Add(new MenuFlyoutSeparator());
        FlyoutBase.SetContextFlyout(this, menu);
    }

    //隐藏显示确认点文本
    private void SetImgTextVisible()
    {
        try
        {
            _graphicsDrawable.CtmImgTextVisible = _kokuImgTextIsVisible;
            _graphicsDrawable?.Shapes?.Where(o => o.Type == CustomShapeType.PointTextImage).ToList().ForEach(o =>
            {
                if (o is CCheckPoint ctmPointText)
                    ctmPointText.IsTextVisible = _kokuImgTextIsVisible;
            });
        }
        catch (Exception ex)
        {
            // _logger.Error(ex.ToString());
        }
        this.Invalidate();
    }

    //锁定工区
    private void LockCSelectKoku(bool blLocked)
    {
        try
        {
            _graphicsDrawable.KokuLocked = blLocked;
            _graphicsDrawable.Shapes
                .Where(o => o.Type == CustomShapeType.Rectangle || o.Type == CustomShapeType.Polygon)
                .ToList()
                .ForEach(o =>
                {
                    if (o is CSelectKoku cSelectKoku)
                    {
                        cSelectKoku.BlLocked = blLocked;
                    }

                    if (o is CSelectPolygon cSelectPolygon)
                    {
                        cSelectPolygon.BlLocked = blLocked;
                    }
                });
            this.Invalidate();
        }
        catch (Exception ex)
        {
            //_logger.Error(ex.ToString());
        }
    }

    //锁定确认点
    private void LockCCheckPoint(bool blLocked)
    {
        _graphicsDrawable.KokuImgLocked = blLocked;
        _graphicsDrawable.Shapes
            .Where(o => o.Type == CustomShapeType.PointTextImage)
            .ToList()
            .ForEach(o => {
                o.BlLocked = blLocked;
            });
    }

    //指针移动
    private void ProGraphicsView_MoveHoverInteraction(object sender, TouchEventArgs e)
    {
        _graphicsDrawable.MoveHoverInteraction(e.Touches[0]);
        this.Invalidate();
    }

    //按下
    private void ProGraphicsView_StartInteraction(object sender, TouchEventArgs e)
    {
        _graphicsDrawable.StartInteraction(e.Touches[0]);
        this.Invalidate();
    }

    //拖拽移动
    private void ProGraphicsView_DragInteraction(object sender, TouchEventArgs e)
    {
        _graphicsDrawable.DragInteraction(e.Touches[0]);
        this.Invalidate();
    }

    //释放
    private void ProGraphicsView_EndInteraction(object sender, TouchEventArgs e)
    {
        _graphicsDrawable.EndInteraction(e.Touches[0]);
        this.Invalidate();
    }

    /// <summary>
    /// 初始化确认点
    /// </summary>
    /// <param name="cCheckPoints"></param>
    public void InitCCheckPoint(IEnumerable<CCheckPoint> cCheckPoints) => _graphicsDrawable.InitCCheckPoint(cCheckPoints);

    /// <summary>
    /// 初始化工区
    /// </summary>
    public void InitCSelectKoku(IEnumerable<CSelectKoku> cSelectKokus)  => _graphicsDrawable.InitCSelectKoku(cSelectKokus);

    /// <summary>
    /// 初始化多边形工区
    /// </summary>
    /// <param name="polygons"></param>
    public void InitCSelectPolygon(IEnumerable<CSelectPolygon> polygons) => _graphicsDrawable.InitCSelectPolygon(polygons);

    /// <summary>
    /// 初始化红蓝点线
    /// </summary>
    /// <param name="cControlPoint"></param>
    public void InitCControlPoint(CControlPoint cControlPoint) => _graphicsDrawable.InitCControlPoint(cControlPoint);

    //矩形转换为多边形
    public void ConvertRectToPolygon()
    {
        try
        {
            if (_graphicsDrawable.KokuLocked) return;
            var obj = _graphicsDrawable.Shapes.FirstOrDefault(o => o.Type == CustomShapeType.Rectangle && o.Selected);
            if (obj is not null && obj is CSelectKoku ctmRectangle)
            {
                var ctmPolygon = ctmRectangle.GetPolygon();
                _graphicsDrawable.Shapes.Remove(obj);
                _graphicsDrawable.Shapes.Add(ctmPolygon);
            }
        }
        catch (Exception ex)
        {
            // _logger.Error(ex.ToString());
        }
    }

    //清空
    public void clear()
    {
        _graphicsDrawable.Clear();
        this.Invalidate();
    }

    //多边形删除点
    public void DelPoint()
    {
        try
        {
            var obj = _graphicsDrawable.Shapes.FirstOrDefault(o => o.Type == CustomShapeType.Polygon && o.Selected);
            if (obj is not null && obj is CSelectPolygon ctmPolygon)
            {
                if (ctmPolygon.BlLocked) return;
                ctmPolygon.DelPoint();
            }
        }
        catch (Exception ex)
        {
            // _logger.Error(ex.ToString());
        }
    }

    //多边形增加点
    public void AddPoint()
    {
        try
        {
            var obj = _graphicsDrawable.Shapes.FirstOrDefault(o => o.Type == CustomShapeType.Polygon && o.Selected);
            if (obj is not null && obj is CSelectPolygon ctmPolygon)
            {
                if (ctmPolygon.BlLocked) return;
                ctmPolygon.AddPoint();
            }
        }
        catch (Exception ex)
        {
            // _logger.Error(ex.ToString());
        }
    }

    //删除选中图形
    public void DelSelectedShape()
    {
        try
        {
            var selectedItem = _graphicsDrawable.Shapes.Where(o => o.Selected && o.Type is not CustomShapeType.AreaPointLine).ToList();
            foreach (var item in selectedItem)
            {
                if (item.BlLocked) continue;
                _graphicsDrawable.Shapes.Remove(item);
            }
        }
        catch (Exception ex)
        {
            //_logger.Error(ex.ToString());
        }

    }

    

    
}
