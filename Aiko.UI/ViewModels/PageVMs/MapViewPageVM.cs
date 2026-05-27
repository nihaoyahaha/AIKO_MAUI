using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Aiko.UI.Themes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExCSS;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Color = Microsoft.Maui.Graphics.Color;
using Colors = Microsoft.Maui.Graphics.Colors;
using Path = System.IO.Path;
using Point = Microsoft.Maui.Graphics.Point;
using SKCanvas = SkiaSharp.SKCanvas;
using SKImage = SkiaSharp.SKImage;
using SKRect = SkiaSharp.SKRect;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class MapViewPageVM : Observablebase<MapViewPageVM, IMapViewService>
{
    private readonly ICheckPointService _checkPointService;
    private MapViewportState? _pendingViewportState;
    private string _pendingViewportMapCode = "";

    /// <summary>
    /// 地图页面视图模型构造函数。
    /// </summary>
    /// <param name="service"></param>
    /// <param name="logger"></param>
    /// <param name="checkPointService"></param>
    public MapViewPageVM(IMapViewService service,
        ILogger<MapViewPageVM> logger,
        ICheckPointService checkPointService) : base(logger, service)
    {
        _checkPointService = checkPointService;
    }

    /// <summary>
    /// 階マスタ
    /// </summary>
    [ObservableProperty]
    public partial int FloorSelectIndex { get; set; } = -1;

    [ObservableProperty]
    public partial ObservableCollection<ListItem> Floors { get; set; } = new();

    /// <summary>
    /// マップ
    /// </summary>
    [ObservableProperty]
    public partial int MapSelectIndex { get; set; } = -1;

    [ObservableProperty]
    public partial ObservableCollection<ListItem> Maps { get; set; } = new();

    /// <summary>
    /// 工区リスト
    /// </summary>
    [ObservableProperty]
    public partial int AreaSelectIndex { get; set; } = -1;

    [ObservableProperty]
    public partial bool AreaEnable { get; set; } = true;

    [ObservableProperty]
    public partial ObservableCollection<ListItem> Areas { get; set; } = new();

    // 在 ViewModel 顶部定义
    private bool _isInitialingAreas = false;

    /// <summary>
    /// 部位リスト
    /// </summary>
    [ObservableProperty]
    public partial int PositionSelectIndex { get; set; } = -1;

    [ObservableProperty]
    public partial bool PositionEnable { get; set; } = true;

    [ObservableProperty]
    public partial ObservableCollection<ListItem> Positions { get; set; } = new();

    // 在 ViewModel 顶部定义
    private bool _isInitialingPositions = false;

    /// <summary>
    /// 工程リスト
    /// </summary>
    [ObservableProperty]
    public partial int ProckSelectIndex { get; set; } = -1;

    [ObservableProperty]
    public partial bool ProckEnable { get; set; } = true;

    [ObservableProperty]
    public partial ObservableCollection<ListItem> Procks { get; set; } = new();

    // 在 ViewModel 顶部定义
    private bool _isInitialingProcks = false;

    /// <summary>
    /// 构造图
    /// </summary>
    [ObservableProperty]
    public partial int ClassSelectIndex { get; set; } = -1;

    [ObservableProperty]
    public partial ObservableCollection<ListItem> Classs { get; set; } = new();

    /// <summary>
    /// 底图数据源
    /// </summary>
    [ObservableProperty]
    public partial ImageSource ImageSource { get; set; } = "";

    /// <summary>
    /// 每个确认点的图标颜色状态
    /// </summary>
    private Dictionary<string, int> dicHR02005 = new Dictionary<string, int>();

    // アイテムテーブルを取得する
    private List<ITEMMETA> hr01ItemList = new List<ITEMMETA>();

    // 採用写真枚数/写真枚数
    private Dictionary<string, string> dicPicCount = new Dictionary<string, string>();

    // 底图裁剪结果的小型 LRU 缓存。
    // 这里缓存的是最终字节数组，不直接缓存 ImageSource，目的是避免流对象生命周期不好控。
    private readonly object _croppedImageCacheLock = new();
    private readonly Dictionary<string, byte[]> _croppedImageCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly LinkedList<string> _croppedImageCacheKeys = new();
    private const int CroppedImageCacheLimit = 3;

    // 存储地图上的动态图元数据
    public List<MapShape> dynamicShapes = new();

    private Dictionary<string, string> dictionaryHR01003 = new Dictionary<string, string>();
    private Dictionary<string, string> dictionaryHM10004 = new Dictionary<string, string>();
    private Dictionary<string, string> dictionaryKoKu = new Dictionary<string, string>();

    /// <summary>
    /// 控制弹窗显示隐藏
    /// </summary>
    [ObservableProperty]
    public partial bool IsSettingVisible { get; set; } = false;

    // テーマ
    [ObservableProperty]
    public partial bool ThemeFlag { get; set; } = false;

    // アイテムNo
    [ObservableProperty]
    public partial bool Hr01003Flag { get; set; } = false;

    // 断面符号
    [ObservableProperty]
    public partial bool Hr01020Flag { get; set; } = true;

    // 工区名
    [ObservableProperty]
    public partial bool Hr01007Flag { get; set; } = false;

    // サイズ拡大
    [ObservableProperty]
    public partial bool SizeFlag { get; set; } = false;

    // 採用枚数/枚数
    [ObservableProperty]
    public partial bool PhotoCountFlag { get; set; } = false;

    // ガイド
    [ObservableProperty]
    public partial int GuideSelectIndex { get; set; } = -1;

    [ObservableProperty]
    public partial ObservableCollection<ListItem> Guide { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    private List<HM04MAPM> hm04List = new List<HM04MAPM>();


    [ObservableProperty]
    public partial List<GuideDrawItem> GuideXItems { get; set; } = new();
    [ObservableProperty]
    public partial List<GuideDrawItem> GuideYItems { get; set; } = new();
    [ObservableProperty]
    public partial List<GuideDrawItem> GuideY2Items { get; set; } = new();
    [ObservableProperty]
    public partial HM04MAPM? CurrentMap { get; set; }

    private sealed class GuideLoadResult
    {
        public List<GuideDrawItem> XItems { get; init; } = new();
        public List<GuideDrawItem> YItems { get; init; } = new();
    }



    private List<HM14GUIDANDHM20> _guideRawX = new();
    private List<HM14GUIDANDHM20> _guideRawY = new();
    // 程序内部批量设置 GuideSelectIndex 时屏蔽选择变化事件，避免重复加载 Guide 数据。
    private bool _suppressGuideSelectionChanged;
    // 跳转到确认点页面前记录当前配筋点编号，返回时只刷新这个点的图标和文字。
    private string _pendingRefreshItemCode = "";
    // 跳转到确认点页面前记录当前配筋点所属部位编号，用于返回时重新计算该点状态。
    private string _pendingRefreshPositionCode = "";

    // 默认配筋点图标的高度
    private const int CheckPointIconHeight = 20;
    // 默认配筋点图标的宽度
    private const int CheckPointIconWidth = 15;

    private const int CheckPointLabelDownOffset = 6;
    // 相机图标
    private const string PhotoCountIconImageSource = "pca.png";
    // 相机图标尺寸
    private const int PhotoCountIconImageSize = 10;
    // 照片枚数图标相对于确认点图标的垂直偏移，确保在默认尺寸下两者之间有适当间距；放大后保持同样的视觉效果。
    private const int PhotoCountIconVerticalOffset = 2;
    // 相机图标与照片枚数作为一个整体显示，这里不再单独做 1px 微调。
    private const int PhotoCountIconVisualOffset = 0;
    // 确认点标签下方的照片枚数标签与图标的高度差，确保两者之间有适当间距。
    private const int CheckPointPhotoCountLabelHeight = 12;

    private int PhotoCountLabelHeight => SizeFlag ? 2 * CheckPointPhotoCountLabelHeight : CheckPointPhotoCountLabelHeight;

    private int PhotoCountIconOffset => SizeFlag ? 2 * PhotoCountIconVerticalOffset : PhotoCountIconVerticalOffset;

    private int PhotoCountIconVisualOffsetY => SizeFlag ? 2 * PhotoCountIconVisualOffset : PhotoCountIconVisualOffset;

    private int CheckPointLabelOffset => SizeFlag ? 2 * CheckPointLabelDownOffset : CheckPointLabelDownOffset;



    /// <summary>
    /// 接收导航参数并触发页面初始化流程。
    /// </summary>
    /// <param name="query"></param>
    public override async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        string fromPage = "";

        if (query.Keys.Contains("FromPage"))
        {
            fromPage = query["FromPage"].ToString();
        }

        await PageLoadedAsync(fromPage);
    }


    /// <summary>
    /// 页面加载
    /// </summary>
    /// <returns></returns>
    private async Task PageLoadedAsync(string fromPage)
    {
        try
        {
            // テーマ
            ThemeFlag = ThemeManager.GetSavedThemeFlag();

            if (fromPage == "LoginPage")
            {
                //层
                await InitCboHM05Async();

                //工程
                await InitCboHM09Async();

                //构造图
                await InitCboHM12Async();

                // アイテムNo显示隐藏
                Hr01003Flag = Preferences.Default.Get("Hr01003Flag", false);
                // 断面符号显示隐藏
                Hr01020Flag = Preferences.Default.Get("Hr01020Flag", true);
                // 工区名显示隐藏
                Hr01007Flag = Preferences.Default.Get("Hr01007Flag", false);
                // サイズ拡大
                SizeFlag = Preferences.Default.Get("SizeFlag", false);
                // 採用枚数/枚数
                PhotoCountFlag = Preferences.Default.Get("PhotoCountFlag", false);
            }
            else if (fromPage == "CheckPointPage")
            {
                await RefreshPendingCheckPointAsync();

                // 3. 回到主线程发送消息（UI 刷新通常需要主线程）
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeakReferenceMessenger.Default.Send("", "RefreshMapDisplayToken");
                    WeakReferenceMessenger.Default.Send("", "RestoreMapViewportStateToken");
                });
            }
            else if (fromPage == "LoginOutPage")
            {
                if (Floors.Count <= 1) 
                {
                    //层
                    await InitCboHM05Async();
                }

                //工程
                await InitCboHM09Async();

                //构造图
                await InitCboHM12Async();

                // マップ
                await MapSelectedIndexChanged();
            }
            else if (fromPage == "MapListPage") 
            {
                // 从构造图预览页返回时，MapViewPage 仍在导航栈中，保留原来的底图、图元和缩放状态即可。
                // 不重新取数、不刷新图元；只恢复跳转前捕获的视口，防止 iOS 返回时布局重算把缩放/拖动位置重置。
                ClassSelectIndex = 0;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeakReferenceMessenger.Default.Send("", "RestoreMapViewportStateToken");
                });
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }
    }

    /// <summary>
    ///  階マスターの初期化
    /// </summary>
    /// <returns></returns>
    private async Task InitCboHM05Async()
    {
        Floors = await Service.GetHM05DataSource();
        FloorSelectIndex = 0;
    }

    /// <summary>
    /// 工区リストの初期化
    /// </summary>
    /// <returns></returns>
    async private Task InitCboHM07Async()
    {
        _isInitialingAreas = true; // 【开启屏蔽】
        try
        {
            string mapCode = Maps[MapSelectIndex].Value;
            Areas = await Service.GetHM07DataSource(mapCode);
            AreaSelectIndex = 0;
        }
        finally
        {
            _isInitialingAreas = false; // 【务必在 finally 恢复】
        }
    }

    /// <summary>
    /// 部位リストの初期化
    /// </summary>
    private async Task InitCboHM06Async()
    {
        _isInitialingPositions = true; // 【开启屏蔽】
        try 
        {
            string mapCode = Maps.Count > 0 && MapSelectIndex > -1 ? Maps[MapSelectIndex].Value : "";
            string areaCode = Areas.Count > 0 && AreaSelectIndex > -1 ? Areas[AreaSelectIndex].Value : "";
            Positions = await Service.GetHM06DataSource(mapCode, areaCode);
            PositionSelectIndex = 0;
        }
        finally
        {
            _isInitialingPositions = false; // 【务必在 finally 恢复】
        }

    }

    /// <summary>
    /// 工程リストの初期化
    /// </summary>
    async private Task InitCboHM09Async()
    {

        _isInitialingProcks = true; // 【开启屏蔽】

        try
        {
            Procks = await Service.GetHM09DataSource();
            ProckSelectIndex = 0;
        }
        finally 
        {
            _isInitialingProcks = false; // 【务必在 finally 恢复】
        }
    }

    /// <summary>
    /// 構造図リストの初期化
    /// </summary>
    /// <returns></returns>
    private async Task InitCboHM12Async()
    {
        Classs = await Service.GetHM12DataSource();
        ClassSelectIndex = 0;
    }

    /// <summary>
    /// 返回首页
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task Home()
    {
        WeakReferenceMessenger.Default.Send("Enter", "EnterOrLeaveLogoutPageToken");
        await Shell.Current.GoToAsync("//Logout");
    }

    /// <summary>
    /// 控制弹窗显示隐藏
    /// </summary>
    [RelayCommand]
    private async Task ToggleSetting()
    {
        IsSettingVisible = !IsSettingVisible;
    }

    /// <summary>
    /// テーマ
    /// </summary>
    partial void OnThemeFlagChanged(bool value)
    {
        Preferences.Default.Set("ThemeFlag", value);
        ThemeManager.ApplyThemeByFlag(value);
    }

    /// <summary>
    /// アイテムNo显示隐藏
    /// </summary>
    partial void OnHr01003FlagChanged(bool value)
    {
        Preferences.Default.Set("Hr01003Flag", Hr01003Flag);
        // 仅更新现有图元的文本与显隐状态，不再重新查库。
        RefreshDynamicShapeDisplayState();
    }

    /// <summary>
    /// 断面符号显示隐藏
    /// </summary>
    partial void OnHr01020FlagChanged(bool value)
    {
        Preferences.Default.Set("Hr01020Flag", Hr01020Flag);
        // 仅更新现有图元的文本与显隐状态，不再重新查库。
        RefreshDynamicShapeDisplayState();
    }

    /// <summary>
    /// 工区名显示隐藏
    /// </summary>
    partial void OnHr01007FlagChanged(bool value)
    {
        Preferences.Default.Set("Hr01007Flag", Hr01007Flag);
        // 仅更新现有图元的文本与显隐状态，不再重新查库。
        RefreshDynamicShapeDisplayState();
    }

    /// <summary>
    /// サイズ拡大
    /// </summary>
    partial void OnSizeFlagChanged(bool value)
    {
        Preferences.Default.Set("SizeFlag", SizeFlag);
        // 仅重算图标尺寸、文字偏移和字号，不再重新查库。
        RefreshDynamicShapeLayoutState();
    }

    /// <summary>
    /// 採用枚数/枚数
    /// </summary>
    partial void OnPhotoCountFlagChanged(bool value)
    {
        Preferences.Default.Set("PhotoCountFlag", PhotoCountFlag);
        // 仅更新现有图元的文本与显隐状态，不再重新查库。
        RefreshDynamicShapeDisplayState();
    }

    /// <summary>
    /// 刷新图元的显示内容与显隐状态，并通知页面做轻量重绘。
    /// </summary>
    private void RefreshDynamicShapeDisplayState()
    {
        // 先同步文本和显隐，再同步字号/偏移，确保轻量刷新时数据一致。
        ApplyDisplayFlagsToDynamicShapes();
        ApplySizeFlagToDynamicShapes();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            WeakReferenceMessenger.Default.Send("", "RefreshMapDisplayToken");
        });
    }

    /// <summary>
    /// 刷新图元的布局信息，并通知页面做轻量重绘。
    /// </summary>
    private void RefreshDynamicShapeLayoutState()
    {
        // SizeFlag 变化时只改布局相关字段，避免整页重建。
        ApplySizeFlagToDynamicShapes();
        ApplyDisplayFlagsToDynamicShapes();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            WeakReferenceMessenger.Default.Send("", "RefreshMapDisplayToken");
        });
    }

    /// <summary>
    /// 按当前设置开关更新现有图元的文本内容与显隐状态。
    /// </summary>
    private void ApplyDisplayFlagsToDynamicShapes()
    {
        foreach (var shape in dynamicShapes)
        {
            if (shape.Type == MapShapeType.Label)
            {
                switch (shape.LayoutRole)
                {
                    case MapShapeLayoutRole.AreaLabel:
                        // 工区标签只受工区名开关控制。
                        shape.Text = dictionaryKoKu.TryGetValue(shape.Tag, out string? areaName) ? areaName : "";
                        shape.IsVisible = Hr01007Flag && !string.IsNullOrEmpty(shape.Text);
                        break;

                    case MapShapeLayoutRole.CheckPointPhotoCountLabel:
                        shape.Text = BuildCheckPointPhotoCountText(shape.Tag);
                        shape.IsVisible = !string.IsNullOrEmpty(shape.Text);
                        break;

                    case MapShapeLayoutRole.CheckPointLabel:
                        shape.Text = BuildCheckPointLabelText(shape.Tag);
                        shape.IsVisible = !string.IsNullOrEmpty(shape.Text);
                        break;
                }
            }
            else if (shape.Type == MapShapeType.Image && string.Equals(shape.ImageSource, "smallarea.png", StringComparison.OrdinalIgnoreCase))
            {
                // smallarea.png 只用于工区标记图标。
                shape.IsVisible = Hr01007Flag;
            }
            else if (shape.Type == MapShapeType.Image && shape.LayoutRole == MapShapeLayoutRole.CheckPointPhotoCountIcon)
            {
                shape.IsVisible = !string.IsNullOrEmpty(BuildCheckPointPhotoCountText(shape.Tag));
            }
        }
    }

    /// <summary>
    /// 按当前 SizeFlag 更新图标尺寸、标签偏移和字号。
    /// </summary>
    private void ApplySizeFlagToDynamicShapes()
    {
        int iconHeight = SizeFlag ? 40 : CheckPointIconHeight;
        int iconWidth = SizeFlag ? 40 : CheckPointIconWidth;

        int photoCountIconSize = SizeFlag ? 2 * PhotoCountIconImageSize : PhotoCountIconImageSize;
        int photoCountLabelHeight = PhotoCountLabelHeight;
        int photoCountIconOffset = PhotoCountIconOffset;
        int photoCountIconVisualOffsetY = PhotoCountIconVisualOffsetY;
        int checkPointLabelOffset = CheckPointLabelOffset;

        int areaIconSize = SizeFlag ? 32 : 16;
        int fontSize = SizeFlag ? 20 : 10;

        foreach (var shape in dynamicShapes)
        {
            switch (shape.LayoutRole)
            {
                case MapShapeLayoutRole.CheckPointIcon:
                    shape.Bounds = new Rect(shape.LayoutOrigin.X, shape.LayoutOrigin.Y - iconHeight, iconWidth, iconHeight);
                    break;

                case MapShapeLayoutRole.AreaIcon:
                    // 图标基于锚点只改宽高。
                    shape.Bounds = new Rect(shape.LayoutOrigin.X, shape.LayoutOrigin.Y, areaIconSize, areaIconSize);
                    break;

                case MapShapeLayoutRole.CheckPointLabel:
                    shape.Bounds = new Rect(shape.LayoutOrigin.X + iconWidth, shape.LayoutOrigin.Y - iconHeight + checkPointLabelOffset, 0, 0);
                    shape.FontSize = fontSize;
                    break;

                case MapShapeLayoutRole.CheckPointPhotoCountLabel:
                    shape.Bounds = new Rect(shape.LayoutOrigin.X + iconWidth, shape.LayoutOrigin.Y - iconHeight + checkPointLabelOffset - photoCountLabelHeight - photoCountIconOffset + photoCountIconVisualOffsetY, photoCountIconSize, photoCountLabelHeight);
                    shape.FontSize = fontSize;
                    break;

                case MapShapeLayoutRole.CheckPointPhotoCountIcon:
                    shape.Bounds = new Rect(shape.LayoutOrigin.X + iconWidth, shape.LayoutOrigin.Y - iconHeight + checkPointLabelOffset - photoCountLabelHeight - photoCountIconOffset + photoCountIconVisualOffsetY, photoCountIconSize, photoCountIconSize);
                    break;

                case MapShapeLayoutRole.AreaLabel:
                    // 标签始终跟在图标右侧，偏移量等于当前图标尺寸。
                    shape.Bounds = new Rect(shape.LayoutOrigin.X + areaIconSize, shape.LayoutOrigin.Y, 0, 0);
                    shape.FontSize = fontSize;
                    break;
            }
        }
    }

    /// <summary>
    /// 根据确认点编号拼接当前应显示的标签文本。
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    private string BuildCheckPointLabelText(string tag)
    {
        // 根据当前设置开关拼出确认点标签内容。
        var parts = new List<string>();

        if (Hr01003Flag && dictionaryHR01003.TryGetValue(tag, out var itemNo))
        {
            parts.Add(itemNo);
        }

        if (Hr01020Flag && dictionaryHM10004.TryGetValue(tag, out var section))
        {
            parts.Add(section);
        }

        return string.Concat(parts);
    }

    private string BuildCheckPointPhotoCountText(string tag)
    {
        if (PhotoCountFlag && dicPicCount != null && dicPicCount.TryGetValue(tag, out var picCount))
        {
            return picCount;
        }

        return "";
    }

    /// <summary>
    /// 保存当前地图页的视口状态，供从确认点页返回时恢复。
    /// 只在地图代码一致时才允许恢复，避免把旧地图的缩放/平移误套到新地图上。
    /// </summary>
    /// <param name="state"></param>
    public void SavePendingViewportState(MapViewportState? state)
    {
        _pendingViewportState = state;
        _pendingViewportMapCode = Maps.Count > 0 && MapSelectIndex > -1 ? Maps[MapSelectIndex].Value : "";
    }

    public bool TryGetPendingViewportState(out MapViewportState? state)
    {
        string currentMapCode = Maps.Count > 0 && MapSelectIndex > -1 ? Maps[MapSelectIndex].Value : "";

        if (_pendingViewportState != null &&
            !string.IsNullOrWhiteSpace(_pendingViewportMapCode) &&
            string.Equals(_pendingViewportMapCode, currentMapCode, StringComparison.OrdinalIgnoreCase))
        {
            state = _pendingViewportState;
            return true;
        }

        state = null;
        return false;
    }

    public void ClearPendingViewportState()
    {
        _pendingViewportState = null;
        _pendingViewportMapCode = "";
    }

    /// <summary>
    /// 跳转到确认点页面。
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task GoToCheckPointPage(HR01ITEM item)
    {
        // 跳转前先让页面记录当前缩放和平移视口，返回时再恢复。
        WeakReferenceMessenger.Default.Send("", "CaptureMapViewportStateToken");
        _pendingRefreshItemCode = item.HR01003?.TrimEnd() ?? "";
        _pendingRefreshPositionCode = item.HR01019?.TrimEnd() ?? "";
        _checkPointService.SetHR01ITEM(item);
        string projectSelectedItemCode = ProckSelectIndex > -1 ? Procks[ProckSelectIndex].Value : "";
        await Shell.Current.GoToAsync("CheckPoint", CreateNavigationParameterForCheckPoint(projectSelectedItemCode));
    }

    /// <summary>
    /// 確認項目画面のナビゲーションパラメータを作成する
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, object> CreateNavigationParameterForCheckPoint(string projectCode)
    {
        var obj = new
        {
            FromPage = "MapViewPage",
            ProjectCode = projectCode //工程コード
        };
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        string jsonString = JsonSerializer.Serialize(obj, options);
        return new Dictionary<string, object>
        {
            { "json", jsonString }
        };
    }

    /// <summary>
    /// 控制工区、部位、工程的picker是否可用
    /// </summary>
    private void SetPickersEnable()
    {
        bool isEnable = MapSelectIndex > 0;
        AreaEnable = isEnable;
        PositionEnable = isEnable;
        ProckEnable = isEnable;
    }

    /// <summary>
    /// 階マスター
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task FloorSelectedIndexChanged()
    {
        ClearPendingViewportState();
        string floorCode = Floors[FloorSelectIndex].Value;
        MapSelectIndex = -1;
        (Maps, hm04List) = await Service.GetHM04DataSource(floorCode);
        MapSelectIndex = 0;
    }

    /// <summary>
    /// マップ
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task MapSelectedIndexChanged()
    {
        ClearPendingViewportState();
        ProckSelectIndex = Procks.Count > 0 ? 0 : -1;

        string mapCode = Maps.Count > 0 && MapSelectIndex > -1 ? Maps[MapSelectIndex].Value : "";

        // 切图前先清空旧 Guide 和旧图元显示，避免新底图自适应时仍看到上一张地图的数据。
        Guide = new ObservableCollection<ListItem>();
        GuideSelectIndex = -1;
        GuideXItems = new List<GuideDrawItem>();
        GuideYItems = new List<GuideDrawItem>();
        GuideY2Items = new List<GuideDrawItem>();

        if (string.IsNullOrWhiteSpace(mapCode))
        {
            // 空地图只做界面清空，不再继续查工区、部位、Guide 和图元数据，
            // 避免页面首次进入时白跑一轮完整刷新。
            CurrentMap = null;
            ImageSource = "";
            WeakReferenceMessenger.Default.Send("", "RefreshMapToken");

            Areas = new ObservableCollection<ListItem> { new("工区", " ") };
            AreaSelectIndex = 0;
            Positions = new ObservableCollection<ListItem> { new("部　位", "") };
            PositionSelectIndex = 0;

            dynamicShapes.Clear();
            dictionaryHR01003.Clear();
            dictionaryHM10004.Clear();
            dictionaryKoKu.Clear();
            dicHR02005.Clear();
            hr01ItemList.Clear();
            dicPicCount.Clear();

            SetPickersEnable();
            return;
        }

        if (mapCode != "")
        {
            HM04MAPM drSelect = hm04List.Where(p => p.HM04002.Equals(mapCode)).FirstOrDefault();

            CurrentMap = drSelect;

            string fileDirectory = Service.AppContext.ConstructionSiteFolder;

            string imagePath = "";

            // 变更后的底图
            if (drSelect.HM04042.Trim() != "")
            {
                imagePath = Path.Combine(Path.Combine(fileDirectory, "paint"), drSelect.HM04042.Trim() + ".jpg");
            }
            // 变更前的底图
            else
            {
                // ファイルマスターリモートサービス
                HM12FILE hm12DC = new HM12FILE();
                // 工事コード
                hm12DC.HM12001 = Service.AppContext.WorkCD;
                // 選択したファイルコード
                hm12DC.HM12002 = drSelect.HM04005.Trim();

                // ファイル情報
                var hm12File = await Service.GetHM12FILEcodeList(hm12DC);

                // 图片名
                string fileName = String.Format(@"{0}{1}", hm12File.HM12002.Trim(), hm12File.HM12003.Trim().Substring(hm12File.HM12003.Trim().LastIndexOf('.')));

                // 变更前的底图
                imagePath = Path.Combine(fileDirectory, fileName);
            }

            if (File.Exists(imagePath))
            {
                int x = 0;
                int y = 0;
                // 表示範囲のwidth
                int width = Convert.ToInt32(drSelect.HM04009);
                // 表示範囲のheight
                int height = Convert.ToInt32(drSelect.HM04010);

                // 表示範囲のstart位置のX
                if (drSelect.HM04007 < 0)
                {
                    x = 0;
                    width = Convert.ToInt32(drSelect.HM04009 + drSelect.HM04007);
                }
                else
                {
                    x = Convert.ToInt32(drSelect.HM04007);
                }
                // 表示範囲のstart位置のY
                if (drSelect.HM04008 < 0)
                {
                    y = 0;
                    height = Convert.ToInt32(drSelect.HM04010 + drSelect.HM04008);
                }
                else
                {
                    y = Convert.ToInt32(drSelect.HM04008);
                }

                // 先切换底图，让页面优先完成一次自适应。
                ImageSource = await CropImageAsync(imagePath, x, y, width, height);
                WeakReferenceMessenger.Default.Send("", "RefreshMapToken");

                // 「マップビュー」を表示する。
                //imgMap.Visibility = Visibility.Visible;
            }
            else
            {
                ImageSource = "";
                WeakReferenceMessenger.Default.Send("", "RefreshMapToken");
            }
        }
        else
        {
            ImageSource = "";
            WeakReferenceMessenger.Default.Send("", "RefreshMapToken");
        }

        await InitCboHM07Async();

        // 部位依赖工区，在底图切换完成后再初始化。
        await InitCboHM06Async();

        SetPickersEnable();

        // マップガイドヘッダマスター
        Guide = await Service.GetHM20GUIDHEADNUMList(mapCode);
        _suppressGuideSelectionChanged = true;
        try
        {
            GuideSelectIndex = Guide.Count > 0 ? 0 : -1;
            await GetdataSource();
            // 图元数据准备完成后，仅刷新覆盖内容，不再重新触发底图自适应。
            WeakReferenceMessenger.Default.Send("", "RefreshMapContentToken");

            _suppressGuideSelectionChanged = false;

            if (GuideSelectIndex >= 0 && Guide.Count > 0)
            {
                await LoadGuideDataAsync();

                WeakReferenceMessenger.Default.Send("", "RefreshGuideToken");
            }
        }
        finally
        {
            _suppressGuideSelectionChanged = false;
        }
    }

    /// <summary>
    /// 工区リスト
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task AreaSelectedIndexChanged()
    {
        // 【拦截逻辑】如果正在初始化，直接跳过业务逻辑
        if (_isInitialingAreas) return;

        await InitCboHM06Async();

        await GetdataSource();

        // 3. 回到主线程发送消息（UI 刷新通常需要主线程）
        MainThread.BeginInvokeOnMainThread(() =>
        {
            WeakReferenceMessenger.Default.Send("", "RefreshMapContentToken");
        });
    }

    /// <summary>
    /// 部位
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task PositionSelectIndexChanged() 
    {
        // 【拦截逻辑】如果正在初始化，直接跳过业务逻辑
        if (_isInitialingPositions) return;

        await GetdataSource();

        // 3. 回到主线程发送消息（UI 刷新通常需要主线程）
        MainThread.BeginInvokeOnMainThread(() =>
        {
            WeakReferenceMessenger.Default.Send("", "RefreshMapContentToken");
        });
    }

    /// <summary>
    /// 工程
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ProckSelectIndexChanged() 
    {
        // 【拦截逻辑】如果正在初始化，直接跳过业务逻辑
        if (_isInitialingProcks) return;
        
        await GetdataSource();

        // 3. 回到主线程发送消息（UI 刷新通常需要主线程）
        MainThread.BeginInvokeOnMainThread(() =>
        {
            WeakReferenceMessenger.Default.Send("", "RefreshMapContentToken");
        });
    }

    /// <summary>
    /// 构造图
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ClassSelectIndexChanged() 
    {
        string classCode = Classs[ClassSelectIndex].Value;
        if (classCode != "") 
        {
            // 跳转到构造图预览页前保存当前视口；返回时只恢复缩放/拖动位置，不刷新地图数据。
            WeakReferenceMessenger.Default.Send("", "CaptureMapViewportStateToken");
            await Shell.Current.GoToAsync($"MapList?ClassCode=" + classCode);
        }
    }

    #region 底图裁剪处理
    /// <summary>
    /// 使用 SkiaSharp 裁剪图片
    /// </summary>
    private async Task<ImageSource> CropImageAsync(string filePath, int x, int y, int width, int height)
    {
        // 缓存键格式：文件路径 + 裁剪参数，确保同一张图不同裁剪结果也能分开缓存。
        string cacheKey = BuildCropCacheKey(filePath, x, y, width, height);
        // 先尝试从缓存拿裁剪结果，命中后直接返回构造好的 ImageSource，避免重复裁剪和磁盘 IO。
        if (TryGetCroppedImageCache(cacheKey, out var cachedBytes))
        {
            // 命中后重新构造一个 ImageSource，避免复用同一个流对象。
            return CreateImageSourceFromBytes(cachedBytes);
        }

        return await Task.Run(() =>
        {
            // 加载原始图片
            using var inputStream = File.OpenRead(filePath);
            using var original = SKBitmap.Decode(inputStream);

            // 确保裁剪区域不超出图片边界
            x = Math.Max(0, Math.Min(x, original.Width - 1));
            y = Math.Max(0, Math.Min(y, original.Height - 1));
            width = Math.Min(width, original.Width - x);
            height = Math.Min(height, original.Height - y);

            // 提取裁剪部分
            using var croppedBitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(croppedBitmap);

            var sourceRect = new SKRect(x, y, x + width, y + height);
            var destRect = new SKRect(0, 0, width, height);

            canvas.DrawBitmap(original, sourceRect, destRect);

            // 转换为 ImageSource
            string extension = Path.GetExtension(filePath);
            bool useJpeg = extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase);
            SKEncodedImageFormat encodedFormat = useJpeg ? SKEncodedImageFormat.Jpeg : SKEncodedImageFormat.Png;
            int encodedQuality = useJpeg ? 90 : 100;
            using var image = SKImage.FromBitmap(croppedBitmap);
            using var data = image.Encode(encodedFormat, encodedQuality);

            var memoryStream = new MemoryStream();
            data.SaveTo(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            // 把裁剪结果存到缓存，供下一次快速访问。
            SetCroppedImageCache(cacheKey, imageBytes);
            // 返回新的 ImageSource，供当前使用。
            return CreateImageSourceFromBytes(imageBytes);
        });
    }

    /// <summary>
    /// 生成底图裁剪缓存键。
    /// 把文件路径、最后修改时间和裁剪区域一起作为键，避免图片被替换后仍命中旧缓存。
    /// </summary>
    private static string BuildCropCacheKey(string filePath, int x, int y, int width, int height)
    {
        long fileStamp = File.GetLastWriteTimeUtc(filePath).Ticks;
        return $"{filePath}|{fileStamp}|{x}|{y}|{width}|{height}";
    }

    /// <summary>
    ///  从缓存拿裁剪结果，命中后直接返回构造好的 ImageSource，避免重复裁剪和磁盘 IO。
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="imageBytes"></param>
    /// <returns></returns>
    private bool TryGetCroppedImageCache(string cacheKey, out byte[] imageBytes)
    {
        lock (_croppedImageCacheLock)
        {
            if (_croppedImageCache.TryGetValue(cacheKey, out imageBytes!))
            {
                // 命中后把键移到链表尾部，维持一个简单的 LRU 顺序。
                _croppedImageCacheKeys.Remove(cacheKey);
                _croppedImageCacheKeys.AddLast(cacheKey);
                return true;
            }
        }

        imageBytes = Array.Empty<byte>();
        return false;
    }

    /// <summary>
    /// 把裁剪结果存到缓存，供下一次快速访问。
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="imageBytes"></param>
    private void SetCroppedImageCache(string cacheKey, byte[] imageBytes)
    {
        lock (_croppedImageCacheLock)
        {
            if (_croppedImageCache.ContainsKey(cacheKey))
            {
                // 已存在时只更新内容并刷新最近使用顺序。
                _croppedImageCache[cacheKey] = imageBytes;
                _croppedImageCacheKeys.Remove(cacheKey);
                _croppedImageCacheKeys.AddLast(cacheKey);
                return;
            }

            _croppedImageCache[cacheKey] = imageBytes;
            _croppedImageCacheKeys.AddLast(cacheKey);

            while (_croppedImageCacheKeys.Count > CroppedImageCacheLimit)
            {
                // 超出上限就淘汰最久未使用的裁剪结果，避免图片字节长期占内存。
                string oldestKey = _croppedImageCacheKeys.First!.Value;
                _croppedImageCacheKeys.RemoveFirst();
                _croppedImageCache.Remove(oldestKey);
            }
        }
    }

    /// <summary>
    /// 返回新的 ImageSource，供当前使用。
    /// </summary>
    /// <param name="imageBytes"></param>
    /// <returns></returns>
    private static ImageSource CreateImageSourceFromBytes(byte[] imageBytes)
    {
        return ImageSource.FromStream(() => new MemoryStream(imageBytes, writable: false));
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task GetdataSource()
    {
        // 每次重建地图前先清空运行时图元和辅助字典。
        dynamicShapes.Clear();

        dictionaryHR01003.Clear();
        dictionaryHM10004.Clear();
        dictionaryKoKu.Clear();

        // マップコード
        string mapCode = Maps.Count > 0 ? Maps[MapSelectIndex].Value : "";

        // 工程Code
        string procksCode = Procks.Count > 0 ? Procks[ProckSelectIndex].Value : "";

        if (string.IsNullOrWhiteSpace(mapCode))
        {
            dicHR02005.Clear();
            hr01ItemList.Clear();
            dicPicCount.Clear();
            return;
        }

        HR01ITEM hr01DC = new HR01ITEM();

        //工事コード
        hr01DC.HR01001 = Service.AppContext.WorkCD;

        //マップコード
        hr01DC.HR01002 = mapCode;

        // 获取每个确认点的图标颜色状态
        dicHR02005 = await Service.GetHR02005(hr01DC, procksCode.Trim());

        // アイテムテーブルを取得する
        hr01ItemList = await Service.GetHR01ITEMcodeList(hr01DC);

        List<ITEMMETA> dataList = hr01ItemList.ToList();

        // 工区
        if (AreaSelectIndex!=-1 && AreaSelectIndex != 0)
        {
            dataList = dataList.Where(p => p.HR01007 == Areas[AreaSelectIndex].Value).ToList();
        }

        // 部位
        if (PositionSelectIndex != -1 && PositionSelectIndex != 0)
        {
            dataList = dataList.Where(p => p.HR01019 == Positions[PositionSelectIndex].Value || p.HR01004 == 1).ToList();
        }

        // 採用写真枚数/写真枚数
        dicPicCount = await Service.GetPicCount(procksCode.Trim());

        Dictionary<string, List<HR05KOKUMINFO>> polygonLookup = new(StringComparer.OrdinalIgnoreCase);

        if (dataList.Any(p => p.HR01004 != 0))
        {
            List<HR05KOKUMINFO> polygonList = await Service.GetHR05KOKUMINFOListByMap(mapCode);

            polygonLookup = polygonList
                .Where(p => !string.IsNullOrWhiteSpace(p.HR05002))
                .GroupBy(p => p.HR05002.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);
        }

        foreach (var item in dataList)
        {
            string strName = string.Empty;

            // 0：配筋確認　1：工区
            if (item.HR01004 == 0)
            {
                RegisterCheckPointLabelSources(item);
                // アイテムコード
                string itemCode = item.HR01003.TrimEnd();
                // 确认点当前应该显示的状态颜色编号。
                int intStatus = GetCheckPointStatus(item);
                // 确认点状态生成地图上显示的部位图标文件名。
                string strBuimName = BuildCheckPointImageSource(item, intStatus);
                // 确认点编号拼接当前应显示的标签文本。
                strName = BuildCheckPointLabelText(itemCode);

                AddCheckPoint(item.HR01003.TrimEnd(), strName, new Point(item.HR01008, item.HR01009), intStatus, strBuimName, item);

                //itemDictionary.Add(item.HR01003.TrimEnd(), new ControlObject(strName, strBuimName, intStatus, null));

                //if (string.IsNullOrEmpty(strPosition.Trim()) || item.HR01019.Trim() == strPosition.Trim())
                //{
                //    確認点追加
                //    AddCheckPoint(item.HR01003.TrimEnd(), strName, new Point(Convert.ToInt32(item.HR01008),
                //        Convert.ToInt32(item.HR01009)), intStatus, strBuimName);
                //}
                //}
 
            }
            else
            {
                strName = item.HM07003 == null ? "" : item.HM07003.TrimEnd();

                dictionaryKoKu.Add(item.HR01003.TrimEnd(), strName);

                //List<int> lst = numberToArgb(item.HR01021);
                string polygonKey = item.HR01003.TrimEnd();
                bool hasPolygon = polygonLookup.TryGetValue(polygonKey, out List<HR05KOKUMINFO>? datalist05);

                //itemDictionary.Add(item.HR01003.TrimEnd(), new ControlObject(strName, null, 0, datalist05));

                //if (string.IsNullOrEmpty(strWorkArea.Trim()) || item.HR01007.Trim() == strWorkArea.Trim())
                //{
                    //if (!Hr01007Flag)
                    //{
                    //    strName = "";
                    //}

                    if (hasPolygon && datalist05!.Count > 0)
                    {
                        AddPolygon(item.HR01003.TrimEnd(), datalist05,  strName, Color.FromUint((uint)item.HR01021), Convert.ToInt32(item.HR01010), Convert.ToInt32(item.HR01011),
                            Convert.ToInt32(item.HR01008), Convert.ToInt32(item.HR01009), new Point(Convert.ToInt32(item.HM07014), Convert.ToInt32(item.HM07015)));
                    }
                    else
                    {
                        // 工区追加
                         AddKoku(item.HR01003.TrimEnd(), strName, Color.FromUint((uint)item.HR01021), Convert.ToInt32(item.HR01010), Convert.ToInt32(item.HR01011),
                            new Point(Convert.ToInt32(item.HR01008), Convert.ToInt32(item.HR01009)), new Point(Convert.ToInt32(item.HM07014), Convert.ToInt32(item.HM07015)));
                    }
                //}
            }

        }
    }

    /// <summary>
    /// 从确认点页面返回时，只刷新刚才进入的配筋点图标和文字，不重建整张地图。
    /// </summary>
    private async Task RefreshPendingCheckPointAsync()
    {
        string itemCode = _pendingRefreshItemCode.Trim();
        string positionCode = _pendingRefreshPositionCode.Trim();

        if (string.IsNullOrWhiteSpace(itemCode))
            return;

        ITEMMETA? item = hr01ItemList
            .FirstOrDefault(p => string.Equals(p.HR01003?.TrimEnd(), itemCode, StringComparison.OrdinalIgnoreCase));

        if (item == null)
            return;

        string mapCode = Maps.Count > 0 && MapSelectIndex > -1 ? Maps[MapSelectIndex].Value : "";
        string procksCode = Procks.Count > 0 && ProckSelectIndex > -1 ? Procks[ProckSelectIndex].Value : "";

        if (string.IsNullOrWhiteSpace(mapCode))
            return;

        HR01ITEM hr01DC = new()
        {
            HR01001 = Service.AppContext.WorkCD,
            HR01002 = mapCode
        };

        // 复用既有接口刷新状态/照片数字典，随后只应用到当前确认点。
        dicHR02005 = await Service.GetHR02005(hr01DC, procksCode.Trim());
        dicPicCount = await Service.GetPicCount(procksCode.Trim());

        RegisterCheckPointLabelSources(item);
        int status = GetCheckPointStatus(item, positionCode);
        string imageSource = BuildCheckPointImageSource(item, status);
        string labelText = BuildCheckPointLabelText(itemCode);
        string photoCountText = BuildCheckPointPhotoCountText(itemCode);

        foreach (var shape in dynamicShapes.Where(p => string.Equals(p.Tag, itemCode, StringComparison.OrdinalIgnoreCase)))
        {
            if (shape.LayoutRole == MapShapeLayoutRole.CheckPointIcon)
            {
                shape.ImageSource = imageSource;
                shape.CommandParameter = item;
            }
            else if (shape.LayoutRole == MapShapeLayoutRole.CheckPointLabel)
            {
                shape.Text = labelText;
                shape.IsVisible = !string.IsNullOrEmpty(shape.Text);
            }
            else if (shape.LayoutRole == MapShapeLayoutRole.CheckPointPhotoCountLabel)
            {
                shape.Text = photoCountText;
                shape.IsVisible = !string.IsNullOrEmpty(shape.Text);
            }
            else if (shape.LayoutRole == MapShapeLayoutRole.CheckPointPhotoCountIcon)
            {
                shape.IsVisible = !string.IsNullOrEmpty(photoCountText);
            }
        }
    }

    /// <summary>
    /// 取得指定确认点当前应该显示的状态颜色编号。
    /// </summary>
    private int GetCheckPointStatus(ITEMMETA item, string positionCode = "")
    {
        if (dicHR02005 == null)
            return 2;

        string resolvedPositionCode = string.IsNullOrWhiteSpace(positionCode)
            ? item.HR01019?.TrimEnd() ?? ""
            : positionCode;

        string key = $"{item.HR01003.TrimEnd()}-{resolvedPositionCode}";
        return dicHR02005.TryGetValue(key, out int status) ? status : 2;
    }

    /// <summary>
    /// 登记确认点标签拼接所需的基础信息，供全量构建和单点刷新共用。
    /// </summary>
    private void RegisterCheckPointLabelSources(ITEMMETA item)
    {
        string itemCode = item.HR01003.TrimEnd();

        dictionaryHR01003[itemCode] = BuildCheckPointItemNoText(item);
        dictionaryHM10004[itemCode] = string.IsNullOrEmpty(item.HM10004) ? "" : item.HM10004.Trim();
    }

    /// <summary>
    /// 生成确认点编号显示文本；数字编号去掉前导零，非数字则原样显示。
    /// </summary>
    private string BuildCheckPointItemNoText(ITEMMETA item)
    {
        string itemCode = item.HR01003.TrimEnd();

        try
        {
            return Convert.ToInt32(itemCode).ToString();
        }
        catch (Exception exp)
        {
            Logger.LogError(exp, "MapViewPageVM");
            return itemCode;
        }
    }

    /// <summary>
    /// 根据确认点状态生成地图上显示的部位图标文件名。
    /// </summary>
    private static string BuildCheckPointImageSource(ITEMMETA item, int status)
    {
        return status switch
        {
            0 => "pg.png",
            1 => "pr.png",
            2 => "pw.png",
            3 => "py.png",
            4 => "pb.png",
            _ => "pw.png"
        };
    }

    /// <summary>
    /// 確認点追加
    /// </summary>
    private void AddCheckPoint(string strTag, string strName, Point e, int intStatus, string strBuimName,HR01ITEM item)
    {
        //string mapCode = Maps[MapSelectIndex].Value;
        //HM04MAPM drSelect = hm04List.Where(p => p.HM04002.Equals(mapCode)).FirstOrDefault();

        //double width = Convert.ToInt32(drSelect.HM04009);
        //double height = Convert.ToInt32(drSelect.HM04010);
        int iconHeight = SizeFlag ? 40 : CheckPointIconHeight;
        int iconWidth = SizeFlag ? 40 : CheckPointIconWidth;

        int photoCountIconSize = SizeFlag ? 2 * PhotoCountIconImageSize : PhotoCountIconImageSize;
        int photoCountLabelHeight = PhotoCountLabelHeight;
        int photoCountIconOffset = PhotoCountIconOffset;
        int photoCountIconVisualOffsetY = PhotoCountIconVisualOffsetY;
        int checkPointLabelOffset = CheckPointLabelOffset;
        int fontSize = SizeFlag ? 20 : 10;

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Image,
            LayoutRole = MapShapeLayoutRole.CheckPointIcon,
            Tag = strTag.Trim(),
            LayoutOrigin = e,
            Bounds = new Rect(e.X, e.Y - iconHeight, iconWidth, iconHeight),
            ImageSource = strBuimName,
            ZIndex = 999,
            CommandParameter = item
        });

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Label,
            LayoutRole = MapShapeLayoutRole.CheckPointLabel,
            Tag = strTag.Trim(),
            LayoutOrigin = e,
            Bounds = new Rect(e.X + iconWidth, e.Y - iconHeight + checkPointLabelOffset, 0, 0),
            Text = strName.TrimEnd(),
            FontSize = fontSize,
            TextColor = Colors.Black,
            IsVisible = !string.IsNullOrEmpty(strName.TrimEnd())
        });

        string photoCountText = BuildCheckPointPhotoCountText(strTag.Trim());

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Label,
            LayoutRole = MapShapeLayoutRole.CheckPointPhotoCountLabel,
            Tag = strTag.Trim(),
            LayoutOrigin = e,
            Bounds = new Rect(e.X + iconWidth, e.Y - iconHeight + checkPointLabelOffset - photoCountLabelHeight - photoCountIconOffset + photoCountIconVisualOffsetY, photoCountIconSize, photoCountLabelHeight),
            Text = photoCountText,
            FontSize = fontSize,
            TextColor = Colors.Black,
            ImageSource = PhotoCountIconImageSource,
            IsVisible = !string.IsNullOrEmpty(photoCountText)
        });
    }

    /// <summary>
    /// 工区追加
    /// </summary>
    private void AddKoku(string strTag, string strName, Color corKOKU, int intWidth, int intHeight, Point e, Point img, bool blRemove = true)
    {
        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Rectangle,
            Tag = strTag.Trim(),
            Bounds = new Rect(e.X, e.Y, intWidth, intHeight),
            StrokeColor = corKOKU,
            StrokeThickness = 3d
        });

        int imgWidth = SizeFlag ? 32 : 16;
        int imgHeight = SizeFlag ? 32 : 16;

        Rect rectImageBounds;
        if (img.X <= 0 && img.Y <= 0)
        {
            rectImageBounds = new Rect(e.X + intWidth / 2, e.Y + intHeight / 2, imgWidth, imgHeight);
        }
        else 
        {
            rectImageBounds = new Rect(img.X, img.Y, imgWidth, imgHeight);
        }

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Image,
            LayoutRole = MapShapeLayoutRole.AreaIcon,
            Tag = strTag.Trim(),
            LayoutOrigin = new Point(rectImageBounds.X, rectImageBounds.Y),
            Bounds = rectImageBounds,
            ImageSource = "smallarea.png",
            IsVisible = Hr01007Flag
        });

        Rect rectTextBounds;
        if (img.X <= 0 && img.Y <= 0)
        {
            rectTextBounds = new Rect(e.X + intWidth / 2 + (SizeFlag ? 32 : 16), e.Y + intHeight / 2, 0, 0);
        }
        else 
        {
            rectTextBounds = new Rect(img.X + (SizeFlag ? 32 : 16), img.Y, 0, 0);
        }

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Label,
            LayoutRole = MapShapeLayoutRole.AreaLabel,
            Tag = strTag.Trim(),
            LayoutOrigin = new Point(rectImageBounds.X, rectImageBounds.Y),
            Bounds = rectTextBounds,
            Text = strName.TrimEnd(),
            FontSize = SizeFlag ? 24 : 12,
            TextColor = Colors.Black,
            IsVisible = Hr01007Flag
        });
    }

    /// <summary>
    /// 添加工区多边形图元及其对应的图标和文字。
    /// </summary>
    /// <param name="strTag"></param>
    /// <param name="datalist05"></param>
    /// <param name="strName"></param>
    /// <param name="cor"></param>
    /// <param name="hr01010"></param>
    /// <param name="hr01011"></param>
    /// <param name="hr01008"></param>
    /// <param name="hr01009"></param>
    /// <param name="img"></param>
    private void AddPolygon(string strTag, List<HR05KOKUMINFO> datalist05, string strName, Color cor, int hr01010, int hr01011, int hr01008, int hr01009, Point img)
    {
        //double width = Convert.ToUInt32(m_drSelectInfo.HM04009);
        //double height = Convert.ToUInt32(m_drSelectInfo.HM04010);

        int max_x = 0;
        int max_y = 0;
        int min_x = 0;
        int min_y = 0;

        var points = new List<Point>();

        foreach (var itempoints in datalist05)
        {
            max_x = max_x < itempoints.HR05004 ? itempoints.HR05004 : max_x;
            max_y = max_y < itempoints.HR05005 ? itempoints.HR05005 : max_y;
            min_x = min_x == 0 ? itempoints.HR05004 : min_x > itempoints.HR05004 ? itempoints.HR05004 : min_x;
            min_y = min_y == 0 ? itempoints.HR05005 : min_y > itempoints.HR05005 ? itempoints.HR05005 : min_y;

            points.Add(new Point(itempoints.HR05004 + 1.5, itempoints.HR05005 + 1.5));
        }

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Polygon,
            Tag = strTag.Trim(),
            Points = points,
            StrokeColor = cor,
            StrokeThickness = 3d
        });

        int imgWidth = SizeFlag ? 32 : 16;
        int imgHeight = SizeFlag ? 32 : 16;

        Rect polygonImageBounds;
        if (img.X <= 0 && img.Y <= 0)
        {
            polygonImageBounds = new Rect((min_x + max_x) / 2.0, (max_y + min_y) / 2.0, imgWidth, imgHeight);
        }
        else
        {
            polygonImageBounds = new Rect(img.X, img.Y, imgWidth, imgHeight);
        }

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Image,
            LayoutRole = MapShapeLayoutRole.AreaIcon,
            Tag = strTag.Trim(),
            LayoutOrigin = new Point(polygonImageBounds.X, polygonImageBounds.Y),
            Bounds = polygonImageBounds,
            ImageSource = "smallarea.png",
            IsVisible = Hr01007Flag
        });

        Rect polygonTextBounds;
        if (img.X <= 0 && img.Y <= 0)
        {
            polygonTextBounds = new Rect((min_x + max_x) / 2.0 + (SizeFlag ? 32 : 16), (max_y + min_y) / 2.0, 0, 0);
        }
        else
        {
            polygonTextBounds = new Rect(img.X + (SizeFlag ? 32 : 16), img.Y, 0, 0);
        }

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Label,
            LayoutRole = MapShapeLayoutRole.AreaLabel,
            Tag = strTag.Trim(),
            LayoutOrigin = new Point(polygonImageBounds.X, polygonImageBounds.Y),
            Bounds = polygonTextBounds,
            Text = strName.TrimEnd(),
            FontSize = SizeFlag ? 24 : 12,
            TextColor = Colors.Black,
            IsVisible = Hr01007Flag
        });
    }

    /// <summary>
    /// 色変換
    /// </summary>
    /// <param name="Number"></param>
    /// <returns></returns>
    public List<int> numberToArgb(int Number)
    {
        List<int> lst = new List<int>();
        lst.Add((Number >> 24) & 0xFF);
        lst.Add((Number >> 16) & 0xFF);
        lst.Add((Number >> 8) & 0xFF);
        lst.Add(Number & 0xFF);
        return lst;
    }

    /// <summary>
    /// ガイド
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task GuideSelectedIndexChanged()
    {
        if (_suppressGuideSelectionChanged)
        {
            return;
        }

        await LoadGuideDataAsync();
        WeakReferenceMessenger.Default.Send("", "RefreshGuideToken");
    }


    /// <summary>
    /// 读取并生成当前地图的全部标尺绘制数据。
    /// </summary>
    /// <returns></returns>
    public async Task LoadGuideDataAsync()
    {
        GuideXItems = new List<GuideDrawItem>();
        GuideYItems = new List<GuideDrawItem>();
        GuideY2Items = new List<GuideDrawItem>();

        if (CurrentMap == null || MapSelectIndex < 0 || GuideSelectIndex < 0 || Maps.Count == 0 || Guide.Count == 0)
        {
            return;
        }

        string mapCode = Maps[MapSelectIndex].Value;
        int guideNo = int.Parse(Guide[GuideSelectIndex].Value);
        var result = await LoadGuideDataCoreAsync(mapCode, guideNo, CurrentMap);

        GuideXItems = result.XItems;
        GuideYItems = result.YItems;
        GuideY2Items = new List<GuideDrawItem>();
    }

    private async Task<GuideLoadResult> LoadGuideDataCoreAsync(string mapCode, int guideNo, HM04MAPM currentMap)
    {
        bool loadXEnabled = Convert.ToInt32(currentMap.HM04015) == 0;
        bool loadYEnabled = Convert.ToInt32(currentMap.HM04020) == 0;
        List<GuideDrawItem> xItems = new();
        List<GuideDrawItem> yItems = new();

        Task<List<HM14GUIDANDHM20>>? loadXTask = null;
        Task<(double FontSize, string FontFamily, Color Color)>? loadXStyleTask = null;

        Task<List<HM14GUIDANDHM20>>? loadYTask = null;
        Task<(double FontSize, string FontFamily, Color Color)>? loadYStyleTask = null;

        // X
        if (loadXEnabled)
        {
            var hm14X = new HM14GUID
            {
                HM14001 = Service.AppContext.WorkCD,
                HM14002 = mapCode.Trim(),
                HM14004 = 0,
                HM14015 = guideNo
            };

            loadXTask = Service.GetHM14GUIDCODEList(hm14X);
            loadXStyleTask = GetGuideStyleAsync(mapCode, 0, guideNo);
        }

        // Y
        if (loadYEnabled)
        {
            var hm14Y = new HM14GUID
            {
                HM14001 = Service.AppContext.WorkCD,
                HM14002 = mapCode.Trim(),
                HM14004 = 1,
                HM14015 = guideNo
            };

            loadYTask = Service.GetHM14GUIDCODEList(hm14Y);
            loadYStyleTask = GetGuideStyleAsync(mapCode, 1, guideNo);
        }

        // X
        if (loadXTask != null && loadXStyleTask != null)
        {
            _guideRawX = await loadXTask;

            var xStyle = await loadXStyleTask;

            double picScaleX = GetPicScaleX(_guideRawX);
            picScaleX = double.IsInfinity(picScaleX) ? 0 : picScaleX;

            foreach (var item in _guideRawX)
            {
                bool isAngleZero =
                           Convert.ToDecimal(item.HM20005) == 0 &&
                           Convert.ToDecimal(item.HM20006) == 0 &&
                           Convert.ToDecimal(item.HM20007) == 0 &&
                           Convert.ToDecimal(item.HM20008) == 0 &&
                           Convert.ToDecimal(item.HM20009) == 0;

                int intEX = isAngleZero
                    ? CalcXGuidePositionWhenAngleZero(item, currentMap)
                    : CalcXGuidePositionWhenAngleNotZero(item, currentMap, picScaleX);

                xItems.Add(new GuideDrawItem
                {
                    Text = item.HM14006?.Trim() ?? "",
                    LogicalValue = intEX,
                    Width = 80,
                    Height = 48,
                    FontSize = xStyle.FontSize,
                    FontFamily = xStyle.FontFamily.Trim(),
                    Color = xStyle.Color,
                    Side = 0,
                    Angle = Convert.ToDouble(item.HM20005)
                });
            }
        }

        // Y
        if (loadYTask != null && loadYStyleTask != null)
        {
            _guideRawY = await loadYTask;

            var yStyle = await loadYStyleTask;

            double picScaleY = GetPicScaleY(_guideRawY);
            picScaleY = double.IsInfinity(picScaleY) ? 0 : picScaleY;

            foreach (var item in _guideRawY)
            {
                bool isAngleZero =
                    Convert.ToDecimal(item.HM20005) == 0 &&
                    Convert.ToDecimal(item.HM20006) == 0 &&
                    Convert.ToDecimal(item.HM20007) == 0 &&
                    Convert.ToDecimal(item.HM20008) == 0 &&
                    Convert.ToDecimal(item.HM20009) == 0;

                int intEY = isAngleZero
                    ? CalcYGuidePositionWhenAngleZero(item, currentMap)
                    : CalcYGuidePositionWhenAngleNotZero(item, currentMap, picScaleY);

                // Y方向也先不过滤，交给页面判断显示位置
                yItems.Add(new GuideDrawItem
                {
                    Text = item.HM14006?.Trim() ?? "",
                    LogicalValue = intEY,
                    Width = 80,
                    Height = 22,
                    FontSize = yStyle.FontSize,
                    FontFamily = yStyle.FontFamily.Trim(),
                    Color = yStyle.Color,
                    Side = 1,
                    Angle = Convert.ToDouble(item.HM20005)
                });
            }
        }

        var result = new GuideLoadResult
        {
            XItems = xItems,
            YItems = yItems
        };
        return result;
    }

    /// <summary>
    /// 获取指定标尺类型的字体、字体名和颜色配置。
    /// </summary>
    /// <param name="mapCode"></param>
    /// <param name="type"></param>
    /// <param name="guideNo"></param>
    /// <returns></returns>
    private async Task<(double FontSize, string FontFamily, Color Color)> GetGuideStyleAsync(string mapCode, int type, int guideNo)
    {
        var list = await Service.GetHM19GUIDCOLORList(mapCode, type, guideNo);

        if (list != null && list.Count > 0)
        {
            var row = list[0];
            byte[] intBuff = BitConverter.GetBytes(row.HM19006);
            var color = Color.FromRgba(intBuff[2], intBuff[1], intBuff[0], intBuff[3]);

            var style = (
                Convert.ToDouble(row.HM19008),
                row.HM19007.Trim() ?? "",
                color
            );

            return style;
        }

        return type == 0
            ? (18, "", Colors.Lime)
            : (18, "", Colors.Fuchsia);
    }

    /// <summary>
    /// 计算 X 方向水平标尺在角度为 0 时的逻辑位置。
    /// </summary>
    /// <param name="item"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    private int CalcXGuidePositionWhenAngleZero(HM14GUIDANDHM20 item, HM04MAPM map)
    {
        int intEX = 0;

        double hm04007 = Convert.ToDouble(map.HM04007);
        double hm04009 = Convert.ToDouble(map.HM04009);
        double hm04011 = Convert.ToDouble(map.HM04011);
        double hm04013 = Convert.ToDouble(map.HM04013);
        double hm04035 = Convert.ToDouble(map.HM04035);
        double hm04037 = Convert.ToDouble(map.HM04037);
        double hm14008 = Convert.ToDouble(item.HM14008);

        // 当前显示范围与控制点范围完全无交集
        if (hm04007 > hm04013 || (hm04007 + hm04009) < hm04011)
        {
            return -1;
        }
        // 控制点1不在当前显示范围内
        else if (hm04007 > hm04011 || (hm04007 + hm04009) < hm04011)
        {
            if (hm04037 - hm04035 != 0)
            {
                intEX = Convert.ToInt32(Math.Round(
                    hm04011 - hm04007 +
                    hm14008 * (hm04013 - hm04011) / (hm04037 - hm04035)
                ));
            }
            else
            {
                intEX = item.HM14008;
            }
        }
        else
        {
            if (hm04037 - hm04035 != 0)
            {
                intEX = Convert.ToInt32(Math.Round(
                    Math.Abs(hm04007 - hm04011) +
                    hm14008 * (hm04013 - hm04011) / (hm04037 - hm04035)
                ));
            }
            else
            {
                intEX = item.HM14008;
            }
        }

        // 超出显示范围宽度则不显示
        if (intEX < 0 || intEX > Convert.ToDouble(map.HM04009))
        {
            return -1;
        }

        return intEX;
    }

    /// <summary>
    /// 计算 X 方向倾斜标尺在非 0 角度时的逻辑位置。
    /// </summary>
    /// <param name="item"></param>
    /// <param name="map"></param>
    /// <param name="picScaleX"></param>
    /// <returns></returns>
    private int CalcXGuidePositionWhenAngleNotZero(HM14GUIDANDHM20 item, HM04MAPM map, double picScaleX)
    {
        int xx = Convert.ToInt32(Math.Ceiling(
            Convert.ToDouble(item.HM14008) *
            Math.Cos(Math.Abs(Convert.ToDouble(item.HM20005)) * Math.PI / 180) *
            picScaleX));

        int yy = Convert.ToInt32(Math.Ceiling(
            Convert.ToDouble(item.HM14008) *
            Math.Sin(Math.Abs(Convert.ToDouble(item.HM20005)) * Math.PI / 180) *
            picScaleX));

        Point point = new Point();
        int intEX = 0;

        if (Convert.ToDouble(item.HM20005) < 0)
        {
            point = new Point(
                Convert.ToInt32(item.HM20006) + xx,
                Convert.ToInt32(item.HM20007) - yy);

            int offX = Convert.ToInt32(Math.Ceiling(
                (point.Y - (map.HM04008 > 0 ? map.HM04008 : 0)) *
                Math.Tan(Math.Abs(Convert.ToDouble(item.HM20005)) * Math.PI / 180)));

            intEX = Convert.ToInt32(point.X - (map.HM04007 > 0 ? map.HM04007 : 0) - offX);
        }
        else
        {
            point = new Point(
                Convert.ToInt32(item.HM20006) + xx,
                Convert.ToInt32(item.HM20007) + yy);

            int offX = Convert.ToInt32(Math.Ceiling(
                (point.Y - (map.HM04008 > 0 ? map.HM04008 : 0)) *
                Math.Tan(Math.Abs(Convert.ToDouble(item.HM20005)) * Math.PI / 180)));

            intEX = Convert.ToInt32(point.X - (map.HM04007 > 0 ? map.HM04007 : 0) + offX);
        }

        return intEX;
    }

    /// <summary>
    /// 计算 X 方向标尺的图片缩放比。
    /// </summary>
    /// <param name="dr"></param>
    /// <returns></returns>
    private double GetPicScaleX(List<HM14GUIDANDHM20> dr)
    {
        if (dr == null || dr.Count == 0)
            return 0;

        int redX = Convert.ToInt32(dr[0].HM20006);
        int redY = Convert.ToInt32(dr[0].HM20007);
        int blueX = Convert.ToInt32(dr[0].HM20008);
        int blueY = Convert.ToInt32(dr[0].HM20009);

        double x = blueX - redX;
        double y = blueY - redY;

        if (x == 0)
            return 0;

        // 正切值
        double y_x = y / x;
        // 求弧度值
        double y_x_tan = Math.Atan(y_x);
        // 根据弧度值求角度值
        double y_x_jd = Convert.ToDouble(Math.Round(
            y_x_tan / Math.PI * 180,
            2,
            MidpointRounding.AwayFromZero));

        // 斜边长度
        double x2_y2 = Math.Sqrt(x * x + y * y);

        // 对边
        double width1 = 0;
        if (Convert.ToDouble(dr[0].HM20005) <= 0)
        {
            width1 = x2_y2 * Math.Sin(
                (90 - Math.Abs(Convert.ToDouble(dr[0].HM20005)) - y_x_jd) * Math.PI / 180);
        }
        else
        {
            width1 = x2_y2 * Math.Sin(
                (90 - Math.Abs(Convert.ToDouble(dr[0].HM20005)) + y_x_jd) * Math.PI / 180);
        }

        double width2 = Convert.ToInt32(dr.Max(r => r.HM14008));

        if (width2 == 0)
            return 0;

        return width1 / width2;
    }

    /// <summary>
    /// 计算 Y 方向标尺的图片缩放比。
    /// </summary>
    /// <param name="dr"></param>
    /// <returns></returns>
    private double GetPicScaleY(List<HM14GUIDANDHM20> dr)
    {
        if (dr == null || dr.Count == 0)
            return 0;

        int redX = Convert.ToInt32(dr[0].HM20006);
        int redY = Convert.ToInt32(dr[0].HM20007);
        int blueX = Convert.ToInt32(dr[0].HM20008);
        int blueY = Convert.ToInt32(dr[0].HM20009);

        double x = blueX - redX;
        double y = blueY - redY;

        if (x == 0)
            return 0;

        double y_x = y / x;
        double y_x_tan = Math.Atan(y_x);
        double y_x_jd = Convert.ToDouble(Math.Round(
            y_x_tan / Math.PI * 180,
            2,
            MidpointRounding.AwayFromZero));

        double x2_y2 = Math.Sqrt(x * x + y * y);

        double width1 = 0;

        if (Convert.ToDouble(dr[0].HM20005) <= 0)
        {
            width1 = x2_y2 * Math.Sin(
                (Math.Abs(Convert.ToDouble(dr[0].HM20005)) + y_x_jd) * Math.PI / 180);
        }
        else
        {
            width1 = x2_y2 * Math.Sin(
                (Math.Abs(Convert.ToDouble(dr[0].HM20005) - y_x_jd)) * Math.PI / 180);
        }

        double width2 = Convert.ToInt32(dr.Max(r => r.HM14008));

        if (width2 == 0)
            return 0;

        return width1 / width2;
    }

    /// <summary>
    /// 计算 Y 方向水平标尺在角度为 0 时的逻辑位置。
    /// </summary>
    /// <param name="item"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    private int CalcYGuidePositionWhenAngleZero(HM14GUIDANDHM20 item, HM04MAPM map)
    {
        int intEY = 0;

        double hm04008 = Convert.ToDouble(map.HM04008);
        double hm04010 = Convert.ToDouble(map.HM04010);
        double hm04012 = Convert.ToDouble(map.HM04012);
        double hm04014 = Convert.ToDouble(map.HM04014);
        double hm04036 = Convert.ToDouble(map.HM04036);
        double hm04038 = Convert.ToDouble(map.HM04038);
        double hm14008 = Convert.ToDouble(item.HM14008);

        if (hm04008 > hm04014 ||
            (hm04008 + hm04010) < hm04012)
        {
            return -1;
        }
        else if (hm04008 > hm04014 ||
                 (hm04008 + hm04010) < hm04014)
        {
            if (hm04038 - hm04036 != 0)
            {
                intEY = Convert.ToInt32(Math.Round(
                    hm04010 -
                    (
                        hm04008 + hm04010 - hm04014 +
                        hm14008 * (hm04014 - hm04012) / (hm04038 - hm04036)
                    )
                ));
            }
            else
            {
                intEY = item.HM14008;
            }
        }
        else
        {
            if (hm04038 - hm04036 != 0)
            {
                intEY = Convert.ToInt32(Math.Round(
                    hm04010 -
                    (
                        hm04008 + hm04010 - hm04014 +
                        hm14008 * (hm04014 - hm04012) / (hm04038 - hm04036)
                    )
                ));
            }
            else
            {
                intEY = item.HM14008;
            }
        }

        if (intEY < 0 || intEY > Convert.ToDouble(map.HM04010))
        {
            return -1;
        }

        return intEY;
    }

    /// <summary>
    /// 计算 Y 方向倾斜标尺在非 0 角度时的逻辑位置。
    /// </summary>
    /// <param name="item"></param>
    /// <param name="map"></param>
    /// <param name="picScaleY"></param>
    /// <returns></returns>
    private int CalcYGuidePositionWhenAngleNotZero(HM14GUIDANDHM20 item, HM04MAPM map, double picScaleY)
    {
        int yy = Convert.ToInt32(Math.Ceiling(
            Convert.ToDouble(item.HM14008) *
            Math.Cos(Math.Abs(Convert.ToDouble(item.HM20005)) * Math.PI / 180) *
            picScaleY));

        int xx = Convert.ToInt32(Math.Ceiling(
            Convert.ToDouble(item.HM14008) *
            Math.Sin(Math.Abs(Convert.ToDouble(item.HM20005)) * Math.PI / 180) *
            picScaleY));

        Point point = new Point();
        int intEY = 0;

        if (Convert.ToDouble(item.HM20005) < 0)
        {
            point = new Point(
                Convert.ToInt32(item.HM20008) - xx,
                Convert.ToInt32(item.HM20009) - yy);

            int offY = Convert.ToInt32(Math.Ceiling(
                (point.X - (map.HM04007 > 0 ? map.HM04007 : 0)) *
                Math.Tan(Math.Abs(Convert.ToDouble(item.HM20005)) * Math.PI / 180)));

            intEY = Convert.ToInt32(point.Y - (map.HM04008 > 0 ? map.HM04008 : 0) + offY);
        }
        else
        {
            point = new Point(
                Convert.ToInt32(item.HM20008) + xx,
                Convert.ToInt32(item.HM20009) - yy);

            int offY = Convert.ToInt32(Math.Ceiling(
                (point.X - (map.HM04007 > 0 ? map.HM04007 : 0)) *
                Math.Tan(Math.Abs(Convert.ToDouble(item.HM20005)) * Math.PI / 180)));

            intEY = Convert.ToInt32(point.Y - (map.HM04008 > 0 ? map.HM04008 : 0) - offY);
        }

        return intEY;
    }

}

public class GuideDrawItem
{
    public string Text { get; set; } = "";
    public double LogicalValue { get; set; }   // HM14008 逻辑坐标
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 80;
    public double Height { get; set; } = 48;
    public double FontSize { get; set; } = 18;
    public string FontFamily { get; set; } = "";
    public Color Color { get; set; } = Colors.Lime;
    public int Side { get; set; } // 0=Top,1=Left,2=Right

    public double Angle { get; set; } // HM20005
}

