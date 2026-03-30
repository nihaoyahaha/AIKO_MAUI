using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Media;

namespace Aiko.UI;

public partial class CtmGraphicsView : ContentView
{
    //    #region 附加属性
    /// <summary>
    /// 背景图片
    /// </summary>
    public static readonly BindableProperty BackgroundImageProperty = BindableProperty.Create(nameof(BackgroundImage), typeof(ImageSource), typeof(CtmGraphicsView), null);
    public ImageSource BackgroundImage
    {
        get => (ImageSource)GetValue(CtmGraphicsView.BackgroundImageProperty);
        set => SetValue(CtmGraphicsView.BackgroundImageProperty, value);
    }

    /// <summary>
    /// 背景图片可见性
    /// </summary>
    public static readonly BindableProperty BackgroundImageIsVisibleProperty = BindableProperty.Create(nameof(BackgroundImageIsVisible), typeof(bool), typeof(CtmGraphicsView), true);
    public bool BackgroundImageIsVisible
    {
        get => (bool)GetValue(CtmGraphicsView.BackgroundImageIsVisibleProperty);
        set => SetValue(CtmGraphicsView.BackgroundImageIsVisibleProperty, value);
    }

    //    /// <summary>
    //    /// 确认点文本可见性
    //    /// </summary>
    //    public static readonly BindableProperty KokuImgTextIsVisibleProperty = BindableProperty.Create(nameof(KokuImgTextIsVisible), typeof(bool), typeof(CtmGraphicsView), true, propertyChanged: OnKokuImgTextIsVisibleChanged);
    //    public bool KokuImgTextIsVisible
    //    {
    //        get => (bool)GetValue(KokuImgTextIsVisibleProperty);
    //        set => SetValue(KokuImgTextIsVisibleProperty, value);
    //    }
    //    static void OnKokuImgTextIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        CtmGraphicsView view = bindable as CtmGraphicsView;
    //        view.canvas.KokuImgTextIsVisible = (bool)newValue;
    //    }

    //    /// <summary>
    //    /// 自定义形状类型
    //    /// </summary>
    //    public static readonly BindableProperty ShapeTypeProperty = BindableProperty.Create(nameof(ShapeType), typeof(CustomShapeType), typeof(CtmGraphicsView), CustomShapeType.PointTextImage, propertyChanged: OnShapeTypeChanged);
    //    public CustomShapeType ShapeType
    //    {
    //        get => (CustomShapeType)GetValue(ShapeTypeProperty);
    //        set => SetValue(ShapeTypeProperty, value);
    //    }
    //    static void OnShapeTypeChanged(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        CtmGraphicsView view = bindable as CtmGraphicsView;
    //        view.canvas.ShapeType = (CustomShapeType)newValue;
    //    }

    //    /// <summary>
    //    /// 是否锁定工区
    //    /// </summary>
    //    public static readonly BindableProperty IsLockKoKuProperty = BindableProperty.Create(nameof(IsLockKoKu), typeof(bool), typeof(CtmGraphicsView), propertyChanged: OnIsLockKoKuChanged);
    //    public bool IsLockKoKu
    //    {
    //        get => (bool)GetValue(IsLockKoKuProperty);
    //        set => SetValue(IsLockKoKuProperty, value);
    //    }
    //    static void OnIsLockKoKuChanged(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        CtmGraphicsView view = bindable as CtmGraphicsView;
    //        view.canvas.IsLockKoKu = (bool)newValue;
    //    }

    //    /// <summary>
    //    /// 是否锁定确认点 
    //    /// </summary>
    //    public static readonly BindableProperty IsLockCheckPointProperty = BindableProperty.Create(nameof(IsLockCheckPoint), typeof(bool), typeof(CtmGraphicsView), propertyChanged: OnIsLockCheckPointChanged);
    //    public bool IsLockCheckPoint
    //    {
    //        get => (bool)GetValue(IsLockCheckPointProperty);
    //        set => SetValue(IsLockCheckPointProperty, value);
    //    }
    //    static void OnIsLockCheckPointChanged(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        CtmGraphicsView view = bindable as CtmGraphicsView;
    //        view.canvas.IsLockCheckPoint = (bool)newValue;
    //    }

    //#endregion

    ///// <summary>
    ///// 获取已绘制元素
    ///// </summary>
    //public List<ICustomShape> Shapes
    //{
    //    get
    //    {
    //        return canvas.Shapes;
    //    }
    //    set
    //    {
    //        canvas.Shapes = value;
    //    }
    //}

    public CtmGraphicsView()
	{
	

		InitializeComponent();
        //canvas.ShapeType = ShapeType;
        //canvas.KokuImgTextIsVisible = KokuImgTextIsVisible;
        //canvas.IsLockKoKu = IsLockKoKu;
        //canvas.IsLockCheckPoint = IsLockCheckPoint;
    }

	private void this_Loaded(object sender, EventArgs e)
    {
        var ff = scrol.Width;

Ellipse ss =new Ellipse();
        ss.Fill = Colors.Red;
        ss.HeightRequest = 50;
        ss.HorizontalOptions = LayoutOptions.Start;
        ss.WidthRequest = 150;
        ss.TranslationX = 0;
        ss.TranslationY = 0;

        

	//	root.Children.Add(ss);


	}

	private void root_SizeChanged(object sender, EventArgs e)
	{

	}



	///// <summary>
	///// 清空图形
	///// </summary>
	//public void Clear() => canvas.clear();

	///// <summary>
	///// 删除多边形顶点
	///// </summary>
	//public void DelPolygonPoint() => canvas.DelPoint();

	///// <summary>
	///// 增加多边形顶点
	///// </summary>
	//public void AddPolygonPoint() => canvas.AddPoint();

	///// <summary>
	///// 删除选中图形
	///// </summary>
	//public void DelSelectedShape() => canvas.DelSelectedShape();

	///// <summary>
	///// 初始化确认点
	///// </summary>
	///// <param name="cCheckPoints"></param>
	//public void InitCCheckPoint(IEnumerable<CCheckPoint> cCheckPoints) => canvas.InitCCheckPoint(cCheckPoints);

	///// <summary>
	///// 初始化矩形工区
	///// </summary>
	///// <param name="cSelectKokus"></param>
	//public void InitCSelectKoku(IEnumerable<CSelectKoku> cSelectKokus) => canvas.InitCSelectKoku(cSelectKokus);

	///// <summary>
	///// 初始化多边形工区
	///// </summary>
	///// <param name="polygons"></param>
	//public void InitCSelectPolygon(IEnumerable<CSelectPolygon> polygons) => canvas.InitCSelectPolygon(polygons);

	///// <summary>
	///// 初始化红蓝点线
	///// </summary>
	///// <param name="cControlPoint"></param>
	//public void InitCControlPoint(CControlPoint cControlPoint) => canvas.InitCControlPoint(cControlPoint);




}
