using ExCSS;
using Microsoft.Maui.Controls;

namespace Aiko.UI.CustomControls;

public partial class EditView_IncludingImage : ContentView
{
    private double _scale = 1;

    /// <summary>
    /// 行の高さ
    /// </summary>
    public static readonly BindableProperty GlobalLabelHeightProperty =
        BindableProperty.Create(nameof(GlobalLabelHeight), typeof(double), typeof(EditView_IncludingImage), 30.0);
    public double GlobalLabelHeight
    {
        get => (double)GetValue(GlobalLabelHeightProperty);
        set => SetValue(GlobalLabelHeightProperty, value);
    }

    /// <summary>
    /// フォントサイズ
    /// </summary>
    public static readonly BindableProperty GlobalFontSizeProperty =
    BindableProperty.Create(nameof(GlobalFontSize), typeof(double), typeof(EditView_IncludingImage), 14.0);
    public double GlobalFontSize
    {
        get => (double)GetValue(GlobalFontSizeProperty);
        set => SetValue(GlobalFontSizeProperty, value);
    }

    /// <summary>
    /// 工事名
    /// </summary>
    public static readonly BindableProperty ConstructionNameProperty =
        BindableProperty.Create(nameof(ConstructionName), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnConstructionNameChanged);
    public string ConstructionName
    {
        get => (string)GetValue(ConstructionNameProperty);
        set => SetValue(ConstructionNameProperty, value);
    }
    static void OnConstructionNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_ConstructionName.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// 工区名
    /// </summary>
    public static readonly BindableProperty WorkAreaNameProperty = BindableProperty.Create(nameof(WorkAreaName), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnWorkAreaNameChanged);
    public string WorkAreaName
    {
        get => (string)GetValue(WorkAreaNameProperty);
        set => SetValue(WorkAreaNameProperty, value);
    }
    static void OnWorkAreaNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_WorkAreaName.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// 部位名
    /// </summary>
    public static readonly BindableProperty RegionalNameProperty = BindableProperty.Create(nameof(RegionalName), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnRegionalNameChanged);
    public string RegionalName
    {
        get => (string)GetValue(RegionalNameProperty);
        set => SetValue(RegionalNameProperty, value);
    }
    static void OnRegionalNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_RegionalName.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    ///断面名
    /// </summary>
    public static readonly BindableProperty SectionNameProperty = BindableProperty.Create(nameof(SectionName), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnSectionNameChanged);
    public string SectionName
    {
        get => (string)GetValue(SectionNameProperty);
        set => SetValue(SectionNameProperty, value);
    }
    static void OnSectionNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_SectionName.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    ///位置
    /// </summary>
    public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnPositionChanged);
    public string Position
    {
        get => (string)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }
    static void OnPositionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_Position.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    ///工程名
    /// </summary>
    public static readonly BindableProperty ProjectNameProperty = BindableProperty.Create(nameof(ProjectName), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnProjectNameChanged);
    public string ProjectName
    {
        get => (string)GetValue(ProjectNameProperty);
        set => SetValue(ProjectNameProperty, value);
    }
    static void OnProjectNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_ProjectName.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    ///確認項目
    /// </summary>
    public static readonly BindableProperty ConfirmProjectProperty = BindableProperty.Create(nameof(ConfirmProject), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnConfirmProjectChanged);
    public string ConfirmProject
    {
        get => (string)GetValue(ConfirmProjectProperty);
        set => SetValue(ConfirmProjectProperty, value);
    }
    static void OnConfirmProjectChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_ConfirmProject.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    //撮影日
    /// </summary>
    public static readonly BindableProperty ShootingDateProperty = BindableProperty.Create(nameof(ShootingDate), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnShootingDateChanged);
    public string ShootingDate
    {
        get => (string)GetValue(ShootingDateProperty);
        set => SetValue(ShootingDateProperty, value);
    }
    static void OnShootingDateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_ShootingDate.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    //施工者
    /// </summary>
    public static readonly BindableProperty ConstructorProperty = BindableProperty.Create(nameof(Constructor), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnConstructorChanged);
    public string Constructor
    {
        get => (string)GetValue(ConstructorProperty);
        set => SetValue(ConstructorProperty, value);
    }
    static void OnConstructorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_Constructor.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// 断面図
    /// </summary>
    public static readonly BindableProperty SectionalDrawingProperty = BindableProperty.Create(nameof(SectionalDrawing), typeof(ImageSource), typeof(EditView_IncludingImage), null, propertyChanged: OnSectionalDrawingChanged);
    public ImageSource SectionalDrawing
    {
        get => (ImageSource)GetValue(SectionalDrawingProperty);
        set => SetValue(SectionalDrawingProperty, value);
    }
    static void OnSectionalDrawingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.img_SectionalDrawing.Source = newValue is ImageSource ? (ImageSource)newValue : null;
        }
    }


    /// <summary>
    /// 備考と確認者の切り替え展示
    /// </summary>
    public static readonly BindableProperty DescribeProperty = BindableProperty.Create(nameof(Describe), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnDescribeChanged);
    public string Describe
    {
        get => (string)GetValue(DescribeProperty);
        set => SetValue(DescribeProperty, value);
    }
    static void OnDescribeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_Describe.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// 備考と確認者の切り替え展示
    /// </summary>
    public static readonly BindableProperty DescribeDetailProperty = BindableProperty.Create(nameof(DescribeDetail), typeof(string), typeof(EditView_IncludingImage), string.Empty, propertyChanged: OnDescribeDetailChanged);
    public string DescribeDetail
    {
        get => (string)GetValue(DescribeDetailProperty);
        set => SetValue(DescribeDetailProperty, value);
    }
    static void OnDescribeDetailChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.lb_DescribeDetail.Text = newValue?.ToString() ?? string.Empty;
        }
    }

    /// <summary>
    /// 非表示を表示
    /// </summary>
    public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(EditView_IncludingImage), true, propertyChanged: OnIsVisibleChanged);
    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }
    static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EditView_IncludingImage view)
        {
            view.grid_root.IsVisible = (bool)newValue;
        }
    }

    /// <summary>
    /// 断面図の表示
    /// </summary>
    public static readonly BindableProperty IsSectionalDrawingVisibleProperty = BindableProperty.Create(nameof(IsSectionalDrawingVisible), typeof(bool), typeof(EditView_IncludingImage), true, propertyChanged: OnIsSectionalDrawingVisibleChanged);
    public bool IsSectionalDrawingVisible
    {
        get => (bool)GetValue(IsSectionalDrawingVisibleProperty);
        set => SetValue(IsSectionalDrawingVisibleProperty, value);
    }
    static void OnIsSectionalDrawingVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not EditView_IncludingImage view)
            return;

        bool isVisible = false;

        if (newValue is bool value)
        {
            isVisible = value;
        }

        int columnSpan = isVisible ? 1 : 2;

        SetBorderColumSpan(view, columnSpan, isVisible);
    }

    private static void SetBorderColumSpan(EditView_IncludingImage view, int value, bool isVisible)
    {
        view.border_SectionalDrawing.IsVisible = isVisible;
        Grid.SetColumnSpan(view.border_WorkArea, value);
        Grid.SetColumnSpan(view.border_Regional, value);
        Grid.SetColumnSpan(view.border_SectionName, value);
        Grid.SetColumnSpan(view.border_Position, value);
        Grid.SetColumnSpan(view.border_ProjectName, value);
        view.WidthRequest = isVisible ? 430 * view._scale : 340 * view._scale;
    }

    public EditView_IncludingImage()
    {
        InitializeComponent();
    }

    private void this_Loaded(object sender, EventArgs e)
    {
        CalculateSectionalWidth();
        GlobalLabelHeight = 30 * _scale;
        img_SectionalDrawing.HeightRequest = 30 * _scale * 5;
    }

    public double CalculateSectionalWidth()
    {
        double fontSize = double.Parse(Preferences.Default.Get("BlackFontSize", "14"));
        _scale = fontSize / 14 > 2.5 ? 2.5 : fontSize / 14;
        WidthRequest = border_SectionalDrawing.IsVisible ? 430 * _scale : 340 * _scale;
        return WidthRequest;
    }

}
