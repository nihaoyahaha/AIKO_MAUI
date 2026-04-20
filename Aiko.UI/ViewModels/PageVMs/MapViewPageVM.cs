using Aiko.Common;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using Aiko.UI;
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
    private int floorSelectIndex = -1;

    [ObservableProperty]
    private ObservableCollection<ListItem> floors = new();

    /// <summary>
    /// マップ
    /// </summary>
    [ObservableProperty]
    private int mapSelectIndex = -1;

    [ObservableProperty]
    private ObservableCollection<ListItem> maps = new();

    /// <summary>
    /// 工区リスト
    /// </summary>
    [ObservableProperty]
    private int areaSelectIndex = -1;

    [ObservableProperty]
    private bool areaEnable = true;

    [ObservableProperty]
    private ObservableCollection<ListItem> areas = new();

    // 在 ViewModel 顶部定义
    private bool _isInitialingAreas = false;

    /// <summary>
    /// 部位リスト
    /// </summary>
    [ObservableProperty]
    private int positionSelectIndex = -1;

    [ObservableProperty]
    private bool positionEnable = true;

    [ObservableProperty]
    private ObservableCollection<ListItem> positions = new();

    // 在 ViewModel 顶部定义
    private bool _isInitialingPositions = false;

    /// <summary>
    /// 工程リスト
    /// </summary>
    [ObservableProperty]
    private int prockSelectIndex = -1;

    [ObservableProperty]
    private bool prockEnable = true;

    [ObservableProperty]
    private ObservableCollection<ListItem> procks = new();

    // 在 ViewModel 顶部定义
    private bool _isInitialingProcks = false;

    /// <summary>
    /// 构造图
    /// </summary>
    [ObservableProperty]
    private int classSelectIndex = -1;

    [ObservableProperty]
    private ObservableCollection<ListItem> classs = new();

    /// <summary>
    /// 底图数据源
    /// </summary>
    [ObservableProperty]
    private ImageSource imageSource = "";

    /// <summary>
    /// 每个确认点的图标颜色状态
    /// </summary>
    private Dictionary<string, int> dicHR02005 = new Dictionary<string, int>();

    /// <summary>
    /// 底图裁剪缓存。
    /// Key 使用地图编码，Value 保存最近一次裁剪后的图片信息，
    /// 这样在地图之间来回切换时可以直接复用结果，避免重复裁剪。
    /// </summary>
    private readonly Dictionary<string, CroppedMapImageCache> _croppedMapImageCache = new();

    // アイテムテーブルを取得する
    private List<ITEMMETA> hr01ItemList = new List<ITEMMETA>();

    // 採用写真枚数/写真枚数
    private Dictionary<string, string> dicPicCount = new Dictionary<string, string>();



    // 存储地图上的动态图元数据
    public List<MapShape> dynamicShapes = new();

    private Dictionary<string, string> dictionaryHR01003 = new Dictionary<string, string>();
    private Dictionary<string, string> dictionaryHM10004 = new Dictionary<string, string>();
    private Dictionary<string, string> dictionaryKoKu = new Dictionary<string, string>();

    /// <summary>
    /// 控制弹窗显示隐藏
    /// </summary>
    [ObservableProperty]
    private bool isSettingVisible = false;

    // テーマ
    [ObservableProperty]
    private bool themeFlag = false;

    // アイテムNo
    [ObservableProperty]
    private bool hr01003Flag = false;

    // 断面符号
    [ObservableProperty]
    private bool hr01020Flag = true;

    // 工区名
    [ObservableProperty]
    private bool hr01007Flag = false;

    // サイズ拡大
    [ObservableProperty]
    private bool sizeFlag = false;

    // 採用枚数/枚数
    [ObservableProperty]
    private bool photoCountFlag = false;

    // ガイド
    [ObservableProperty]
    private int guideSelectIndex = -1;

    [ObservableProperty]
    private ObservableCollection<ListItem> guide = new();

    /// <summary>
    /// 
    /// </summary>
    private List<HM04MAPM> hm04List = new List<HM04MAPM>();





    [ObservableProperty]
    private List<GuideDrawItem> guideXItems = new();
    [ObservableProperty]
    private List<GuideDrawItem> guideYItems  = new();
    [ObservableProperty]
    private List<GuideDrawItem> guideY2Items  = new();
    [ObservableProperty]
    private HM04MAPM? currentMap;



    private List<HM14GUIDANDHM20> _guideRawX = new();
    private List<HM14GUIDANDHM20> _guideRawY = new();

    /// <summary>
    /// Guide 延迟加载版本号。
    /// 每次切换地图时递增，用来丢弃已经过期的异步加载结果。
    /// </summary>
    private int _guideLoadVersion = 0;

    /// <summary>
    /// 裁剪后底图缓存实体。
    /// </summary>
    private sealed class CroppedMapImageCache
    {
        /// <summary>
        /// 当前缓存对应的原始文件路径。
        /// </summary>
        public string FilePath { get; init; } = string.Empty;

        /// <summary>
        /// 当前缓存对应的文件最后写入时间，用于文件变更时自动失效。
        /// </summary>
        public DateTime LastWriteTimeUtc { get; init; }

        /// <summary>
        /// 当前缓存对应的裁剪区域 X。
        /// </summary>
        public int X { get; init; }

        /// <summary>
        /// 当前缓存对应的裁剪区域 Y。
        /// </summary>
        public int Y { get; init; }

        /// <summary>
        /// 当前缓存对应的裁剪宽度。
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// 当前缓存对应的裁剪高度。
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        /// 裁剪后的 PNG 字节数据。
        /// 这里缓存字节而不是直接缓存 ImageSource，避免重复使用同一个流对象。
        /// </summary>
        public byte[] ImageBytes { get; init; } = Array.Empty<byte>();
    }



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
            if (fromPage == "LoginPage")
            {
                //层
                await InitCboHM05Async();

                //工程
                await InitCboHM09Async();

                //构造图
                await InitCboHM12Async();

                // テーマ
                ThemeFlag = Preferences.Default.Get("ThemeFlag", false);
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
                await GetdataSource();

                // 3. 回到主线程发送消息（UI 刷新通常需要主线程）
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeakReferenceMessenger.Default.Send("", "RefreshMapContentToken");
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
                await GetdataSource();

                // 3. 回到主线程发送消息（UI 刷新通常需要主线程）
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeakReferenceMessenger.Default.Send("", "RefreshMapContentToken");
                });
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
        Preferences.Default.Set("ThemeFlag", ThemeFlag);
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
                if (dictionaryKoKu.ContainsKey(shape.Tag))
                {
                    // 工区标签只受工区名开关控制。
                    shape.Text = dictionaryKoKu[shape.Tag];
                    shape.IsVisible = Hr01007Flag;
                }
                else
                {
                    // 确认点标签由多个开关组合生成文本。
                    shape.Text = BuildCheckPointLabelText(shape.Tag);
                    shape.IsVisible = !string.IsNullOrEmpty(shape.Text);
                }
            }
            else if (shape.Type == MapShapeType.Image && string.Equals(shape.ImageSource, "smallarea.png", StringComparison.OrdinalIgnoreCase))
            {
                // smallarea.png 只用于工区标记图标。
                shape.IsVisible = Hr01007Flag;
            }
        }
    }

    /// <summary>
    /// 按当前 SizeFlag 更新图标尺寸、标签偏移和字号。
    /// </summary>
    private void ApplySizeFlagToDynamicShapes()
    {
        int iconSize = SizeFlag ? 32 : 16;
        int fontSize = SizeFlag ? 24 : 12;

        foreach (var shape in dynamicShapes)
        {
            switch (shape.LayoutRole)
            {
                case MapShapeLayoutRole.CheckPointIcon:
                case MapShapeLayoutRole.AreaIcon:
                    // 图标基于锚点只改宽高。
                    shape.Bounds = new Rect(shape.LayoutOrigin.X, shape.LayoutOrigin.Y, iconSize, iconSize);
                    break;

                case MapShapeLayoutRole.CheckPointLabel:
                case MapShapeLayoutRole.AreaLabel:
                    // 标签始终跟在图标右侧，偏移量等于当前图标尺寸。
                    shape.Bounds = new Rect(shape.LayoutOrigin.X + iconSize, shape.LayoutOrigin.Y, 0, 0);
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

        if (PhotoCountFlag && dicPicCount != null && dicPicCount.TryGetValue(tag, out var picCount))
        {
            parts.Add(picCount);
        }

        return string.Concat(parts);
    }

    /// <summary>
    /// 跳转到确认点页面。
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task GoToCheckPointPage(HR01ITEM item)
    {
        _checkPointService.SetHR01ITEM(item);
        string projectSelectedItemCode = prockSelectIndex > -1 ? Procks[prockSelectIndex].Value : "";
        await Shell.Current.GoToAsync("CheckPoint", CreateNavigationParameterForCheckPoint(projectSelectedItemCode));
    }

	/// <summary>
	/// 確認项目画面のナビゲーションパラメータを作成する
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
        string floorCode = Floors[FloorSelectIndex].Value;
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
        int currentGuideLoadVersion = ++_guideLoadVersion;

        ProckSelectIndex = Procks.Count > 0 ? 0 : -1;

        await InitCboHM07Async();

        //部位
        await InitCboHM06Async();

        SetPickersEnable();

        string mapCode = Maps[MapSelectIndex].Value;

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
                ImageSource = await GetOrCreateCroppedMapImageAsync(mapCode, imagePath, drSelect);

                // 「マップビュー」を表示する。
                //imgMap.Visibility = Visibility.Visible;
            }
            else
            {
                ImageSource = "";
            }
        }
        else
        {
            ImageSource = "";
        }

        // 先清空上一张地图残留的 Guide，让底图和点位优先显示。
        ClearGuideDisplayState();

        await GetdataSource();

        WeakReferenceMessenger.Default.Send("", "RefreshMapToken");

        // 首屏渲染完成后再异步加载 Guide，避免地图切换时首屏等待标尺数据。
        _ = LoadGuideAfterMapRenderedAsync(mapCode, currentGuideLoadVersion);
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
    /// 工程
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ClassSelectIndexChanged() 
    {
        string classCode = Classs[ClassSelectIndex].Value;
        if (classCode != "") 
        {
            await Shell.Current.GoToAsync($"MapList?ClassCode=" + classCode);
        }
    }

    /// <summary>
    /// 根据地图信息获取裁剪后的底图，命中缓存时直接复用。
    /// </summary>
    /// <param name="mapCode">地图编码</param>
    /// <param name="filePath">原始底图路径</param>
    /// <param name="mapInfo">当前地图信息</param>
    /// <returns></returns>
    private async Task<ImageSource> GetOrCreateCroppedMapImageAsync(string mapCode, string filePath, HM04MAPM mapInfo)
    {
        (int x, int y, int width, int height) = GetMapCropArea(mapInfo);
        DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(filePath);

        if (_croppedMapImageCache.TryGetValue(mapCode, out CroppedMapImageCache cache)
            && string.Equals(cache.FilePath, filePath, StringComparison.OrdinalIgnoreCase)
            && cache.LastWriteTimeUtc == lastWriteTimeUtc
            && cache.X == x
            && cache.Y == y
            && cache.Width == width
            && cache.Height == height
            && cache.ImageBytes.Length > 0)
        {
            return CreateImageSourceFromBytes(cache.ImageBytes);
        }

        byte[] imageBytes = await CropImageToBytesAsync(filePath, x, y, width, height);

        _croppedMapImageCache[mapCode] = new CroppedMapImageCache
        {
            FilePath = filePath,
            LastWriteTimeUtc = lastWriteTimeUtc,
            X = x,
            Y = y,
            Width = width,
            Height = height,
            ImageBytes = imageBytes
        };

        return CreateImageSourceFromBytes(imageBytes);
    }

    /// <summary>
    /// 根据地图显示范围计算裁剪区域。
    /// </summary>
    /// <param name="mapInfo">当前地图信息</param>
    /// <returns></returns>
    private static (int X, int Y, int Width, int Height) GetMapCropArea(HM04MAPM mapInfo)
    {
        int x = 0;
        int y = 0;

        // 表示範囲のwidth
        int width = Convert.ToInt32(mapInfo.HM04009);
        // 表示範囲のheight
        int height = Convert.ToInt32(mapInfo.HM04010);

        // 表示範囲のstart位置のX
        if (mapInfo.HM04007 < 0)
        {
            x = 0;
            width = Convert.ToInt32(mapInfo.HM04009 + mapInfo.HM04007);
        }
        else
        {
            x = Convert.ToInt32(mapInfo.HM04007);
        }

        // 表示範囲のstart位置のY
        if (mapInfo.HM04008 < 0)
        {
            y = 0;
            height = Convert.ToInt32(mapInfo.HM04010 + mapInfo.HM04008);
        }
        else
        {
            y = Convert.ToInt32(mapInfo.HM04008);
        }

        return (x, y, width, height);
    }

    /// <summary>
    /// 根据缓存的字节数据创建可重复使用的 ImageSource。
    /// </summary>
    /// <param name="imageBytes">图片字节</param>
    /// <returns></returns>
    private static ImageSource CreateImageSourceFromBytes(byte[] imageBytes)
    {
        return ImageSource.FromStream(() => new MemoryStream(imageBytes, writable: false));
    }

    /// <summary>
    /// 清空当前 Guide 的显示状态，避免切图过程中继续显示上一张地图的标尺。
    /// </summary>
    private void ClearGuideDisplayState()
    {
        Guide = new ObservableCollection<ListItem>();
        GuideSelectIndex = -1;
        GuideXItems = new List<GuideDrawItem>();
        GuideYItems = new List<GuideDrawItem>();
        GuideY2Items = new List<GuideDrawItem>();
        _guideRawX.Clear();
        _guideRawY.Clear();

        WeakReferenceMessenger.Default.Send("", "RefreshGuideToken");
    }

    /// <summary>
    /// 在地图首屏渲染后异步加载 Guide 数据。
    /// </summary>
    /// <param name="mapCode">当前地图编码</param>
    /// <param name="guideLoadVersion">本次加载版本号</param>
    /// <returns></returns>
    private async Task LoadGuideAfterMapRenderedAsync(string mapCode, int guideLoadVersion)
    {
        // 先让出当前消息循环，优先把底图和图元刷新到界面上。
        await Task.Yield();

        if (guideLoadVersion != _guideLoadVersion)
            return;

        var guideHeaders = await Service.GetHM20GUIDHEADNUMList(mapCode);

        if (guideLoadVersion != _guideLoadVersion)
            return;

        Guide = guideHeaders;
        GuideSelectIndex = guideHeaders.Count > 0 ? 0 : -1;

        await LoadGuideDataAsync(guideLoadVersion);

        if (guideLoadVersion != _guideLoadVersion)
            return;

        WeakReferenceMessenger.Default.Send("", "RefreshGuideToken");
    }

    /// <summary>
    /// 使用 SkiaSharp 裁剪图片
    /// </summary>
    /// <param name="filePath">原始图片路径</param>
    /// <param name="x">裁剪起点 X</param>
    /// <param name="y">裁剪起点 Y</param>
    /// <param name="width">裁剪宽度</param>
    /// <param name="height">裁剪高度</param>
    /// <returns></returns>
    private async Task<byte[]> CropImageToBytesAsync(string filePath, int x, int y, int width, int height)
    {
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

            // 创建裁剪区域
            var cropRect = new SKRectI(x, y, x + width, y + height);

            // 提取裁剪部分
            using var croppedBitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(croppedBitmap);

            var sourceRect = new SKRect(x, y, x + width, y + height);
            var destRect = new SKRect(0, 0, width, height);

            canvas.DrawBitmap(original, sourceRect, destRect);

            // 转换为 ImageSource
            using var image = SKImage.FromBitmap(croppedBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            var memoryStream = new MemoryStream();
            data.SaveTo(memoryStream);
            return memoryStream.ToArray();
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task GetdataSource()
    {
        dynamicShapes.Clear();

        dictionaryHR01003.Clear();
        dictionaryHM10004.Clear();
        dictionaryKoKu.Clear();

        // マップコード
        string mapCode = Maps.Count > 0 ? Maps[MapSelectIndex].Value : "";

        // 工程Code
        string procksCode = Procks.Count > 0 ? Procks[ProckSelectIndex].Value : "";

        HR01ITEM hr01DC = new HR01ITEM();

        //工事コード
        hr01DC.HR01001 = Service.AppContext.WorkCD;

        //マップコード
        hr01DC.HR01002 = mapCode;

        // 这四组数据彼此独立，改为并行查询以缩短地图切换等待时间。
        var hr02005Task = Service.GetHR02005(hr01DC, procksCode.Trim());
        var hr01ItemListTask = Service.GetHR01ITEMcodeList(hr01DC);
        var picCountTask = Service.GetPicCount(procksCode.Trim());
        var polygonListTask = Service.GetHR05KOKUMINFOListByMap(mapCode);

        await Task.WhenAll(hr02005Task, hr01ItemListTask, picCountTask, polygonListTask);

        // 获取每个确认点的图标颜色状态
        dicHR02005 = await hr02005Task;

        // アイテムテーブルを取得する
        hr01ItemList = await hr01ItemListTask;

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
        dicPicCount = await picCountTask;

        // 预先把当前地图的工区多边形按确认点编码分组，避免 foreach 内部反复查库。
        Dictionary<string, List<HR05KOKUMINFO>> polygonDictionary = (await polygonListTask)
            .GroupBy(p => p.HR05002?.Trim() ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.ToList());

        Assembly assembly = GetType().GetTypeInfo().Assembly;

        foreach (var item in dataList)
        {
            string strName = string.Empty;

            // 0：配筋確認　1：工区
            if (item.HR01004 == 0)
            {
                string strBuimName = item.HM06016.Trim();
                strBuimName = strBuimName.Substring(0, strBuimName.Length - 4);

                // 検査結果テーブルリモートサービス
                int intStatus = 0;
                if (dicHR02005 != null)
                {
                    string key = item.HR01003.TrimEnd() + "-" + item.HR01019.TrimEnd();
                    if (dicHR02005.ContainsKey(key)) intStatus = dicHR02005[key];
                    else intStatus = 2;
                }
                else
                {
                    intStatus = 2;
                }

                //0:绿色; 1:红色; 2:白色; 3:黄色; 4:蓝色
                strBuimName = intStatus switch
                {
                    0 => $"p{strBuimName}g.png",
                    1 => $"p{strBuimName}r.png",
                    2 => $"p{strBuimName}w.png",
                    3 => $"p{strBuimName}y.png",
                    4 => $"p{strBuimName}b.png",
                    _ => strBuimName
                };

                try
                {
                    strName = Convert.ToInt32(item.HR01003.TrimEnd()).ToString();
                }
                catch (Exception exp)
                {
                    strName = item.HR01003.TrimEnd();
                    Logger.LogError(exp, "MapViewPageVM");
                }

                dictionaryHR01003.Add(item.HR01003, strName);

                if (!string.IsNullOrEmpty(item.HM10004))
                {
                    dictionaryHM10004.Add(item.HR01003, item.HM10004.Trim());
                }
                else
                {
                    dictionaryHM10004.Add(item.HR01003, "");
                }

                strName = "";

                if (Hr01003Flag)
                {
                    strName += dictionaryHR01003[item.HR01003.TrimEnd()];
                }
                if (Hr01020Flag)
                {
                    strName += dictionaryHM10004[item.HR01003.TrimEnd()];
                }

                if (PhotoCountFlag)
                {
                    //採用写真枚数/写真枚数
                    if (dicPicCount != null)
                    {
                        strName += dicPicCount[item.HR01003.TrimEnd()];
                    }
                }


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

                // 直接从当前地图的多边形缓存中取值，不再逐个工区查询数据库。
                string polygonKey = item.HR01003.TrimEnd();
                polygonDictionary.TryGetValue(polygonKey, out List<HR05KOKUMINFO> datalist05);
                datalist05 ??= new List<HR05KOKUMINFO>();

                //itemDictionary.Add(item.HR01003.TrimEnd(), new ControlObject(strName, null, 0, datalist05));

                //if (string.IsNullOrEmpty(strWorkArea.Trim()) || item.HR01007.Trim() == strWorkArea.Trim())
                //{
                    //if (!Hr01007Flag)
                    //{
                    //    strName = "";
                    //}

                    if (datalist05.Count > 0)
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
    /// 確認点追加
    /// </summary>
    private void AddCheckPoint(string strTag, string strName, Point e, int intStatus, string strBuimName,HR01ITEM item)
    {
        //string mapCode = Maps[MapSelectIndex].Value;
        //HM04MAPM drSelect = hm04List.Where(p => p.HM04002.Equals(mapCode)).FirstOrDefault();

        //double width = Convert.ToInt32(drSelect.HM04009);
        //double height = Convert.ToInt32(drSelect.HM04010);

        int imgWidth = SizeFlag ? 32 : 16;
        int imgHeight = SizeFlag ? 32 : 16;

        dynamicShapes.Add(new MapShape
        {
            Type = MapShapeType.Image,
            LayoutRole = MapShapeLayoutRole.CheckPointIcon,
            Tag = strTag.Trim(),
            LayoutOrigin = e,
            Bounds = new Rect(e.X, e.Y, imgWidth, imgHeight),
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
            Bounds = new Rect(e.X + (SizeFlag ? 32 : 16), e.Y, 0, 0),
            Text = strName.TrimEnd(),
            FontSize = SizeFlag ? 24 : 12,
            TextColor = Colors.Black,
            IsVisible = Hr01020Flag || Hr01003Flag || PhotoCountFlag
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
        await LoadGuideDataAsync(_guideLoadVersion);
        WeakReferenceMessenger.Default.Send("", "RefreshGuideToken");
    }


    /// <summary>
    /// 读取并生成当前地图的全部标尺绘制数据。
    /// </summary>
    /// <returns></returns>
    public async Task LoadGuideDataAsync()
    {
        await LoadGuideDataAsync(_guideLoadVersion);
    }

    /// <summary>
    /// 读取并生成当前地图的全部标尺绘制数据。
    /// 带版本号校验，避免旧地图的异步结果覆盖当前界面。
    /// </summary>
    /// <param name="guideLoadVersion">本次加载版本号</param>
    /// <returns></returns>
    private async Task LoadGuideDataAsync(int guideLoadVersion)
    {
        List<GuideDrawItem> guideXItems = new();
        List<GuideDrawItem> guideYItems = new();
        List<GuideDrawItem> guideY2Items = new();

        if (CurrentMap == null || MapSelectIndex < 0 || GuideSelectIndex < 0 || Maps.Count == 0 || Guide.Count == 0)
        {
            GuideXItems = guideXItems;
            GuideYItems = guideYItems;
            GuideY2Items = guideY2Items;
            return;
        }

        string mapCode = Maps[MapSelectIndex].Value;
        int guideNo = int.Parse(Guide[GuideSelectIndex].Value);

        // X
        if (Convert.ToInt32(CurrentMap.HM04015) == 0)
        {
            var hm14X = new HM14GUID
            {
                HM14001 = Service.AppContext.WorkCD,
                HM14002 = mapCode.Trim(),
                HM14004 = 0,
                HM14015 = guideNo
            };

            _guideRawX = await Service.GetHM14GUIDCODEList(hm14X);

            if (guideLoadVersion != _guideLoadVersion)
                return;

            var xStyle = await GetGuideStyleAsync(mapCode, 0, guideNo);

            if (guideLoadVersion != _guideLoadVersion)
                return;

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
                    ? CalcXGuidePositionWhenAngleZero(item, CurrentMap)
                    : CalcXGuidePositionWhenAngleNotZero(item, CurrentMap, picScaleX);

                guideXItems.Add(new GuideDrawItem
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
        if (Convert.ToInt32(CurrentMap.HM04020) == 0)
        {
            var hm14Y = new HM14GUID
            {
                HM14001 = Service.AppContext.WorkCD,
                HM14002 = mapCode.Trim(),
                HM14004 = 1,
                HM14015 = guideNo
            };

            _guideRawY = await Service.GetHM14GUIDCODEList(hm14Y);

            if (guideLoadVersion != _guideLoadVersion)
                return;

            var yStyle = await GetGuideStyleAsync(mapCode, 1, guideNo);

            if (guideLoadVersion != _guideLoadVersion)
                return;

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
                    ? CalcYGuidePositionWhenAngleZero(item, CurrentMap)
                    : CalcYGuidePositionWhenAngleNotZero(item, CurrentMap, picScaleY);

                // Y方向也先不过滤，交给页面判断显示位置
                guideYItems.Add(new GuideDrawItem
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

        if (guideLoadVersion != _guideLoadVersion)
            return;

        GuideXItems = guideXItems;
        GuideYItems = guideYItems;
        GuideY2Items = guideY2Items;
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

            return (
                Convert.ToDouble(row.HM19008),
                row.HM19007 ?? "",
                color
            );
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

