using Aiko.Common;
using Aiko.Common.Models;
using Aiko.IServices.IServices;
using Aiko.SqliteDb;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Aiko.UI.ViewModels.PageVMs;

public partial class CheckPointDetailPageVM : Observablebase<CheckPointDetailPageVM, ICheckPointDetailService>
{
    private readonly ICheckPointService _checkPointService;

    private bool _first = false;

    [ObservableProperty]
    public partial bool IsDarkenMode { get; set; } = true;

    // 图片存储路径
    public string ImageFolderPath;

    // 查询参数
    public Dictionary<string, object> Query = new Dictionary<string, object>();

    // 上下文数据
    private List<HR01ITEMPINFO> _hr01ItempInfoList = new List<HR01ITEMPINFO>();

    [ObservableProperty]
    public partial Toast Toast { get; set; } = new Toast();

    // 颜料版
    [ObservableProperty]
    public partial ObservableCollection<Color> ColorPalette { get; set; } = new ObservableCollection<Color>();

    // 图片集
    [ObservableProperty]
    public partial ObservableCollection<InkImage> ImageList { get; set; } = new ObservableCollection<InkImage>();
    [ObservableProperty]
    public partial List<InkImage> Original‌ImageList { get; set; } = new List<InkImage>();
    public string Summary => $"{SourceImageList.Count(image => image.IsVisualized)}/{ImagesCount}";

    [ObservableProperty]
    public partial int ImagesCount { get; set; } = 0;

    [ObservableProperty]
    public partial bool IsAscending { get; set; } = true;
    public string SortText => IsAscending ? "昇順 ▲" : "降順 ▼";

    public List<InkImage> SourceImageList = new List<InkImage>();
    private int _pageSize = 12;
    private bool _isImageLoading;
    [ObservableProperty]
    public partial bool IsFooterVisible { get; set; }

    public bool IsLoaded => SourceImageList.Count(image => image.IsVisualized) >= (SourceImageList.Count > _pageSize - 3 ? _pageSize - 3 : SourceImageList.Count);

    [ObservableProperty]
    public partial ImageSource DanmImageSource { get; set; }

    private int _selectImageIndex
    {
        get
        {
            InkImage? image = SourceImageList.Where(image => image.IsSelected).FirstOrDefault();
            if (image == null) return -1;

            return SourceImageList.IndexOf(image);

        }
    }

    /// <summary>
    /// 配筋確認
    /// </summary>
    [ObservableProperty]
    public partial string DanmTitle { get; set; } = "";
    /// <summary>
    /// 部位
    /// </summary>
    [ObservableProperty]
    public partial string Buim { get; set; } = "";
    /// <summary>
    /// 階・グループ
    /// </summary>
    [ObservableProperty]
    public partial string Grpl { get; set; } = "";
    /// <summary>
    /// 断面
    /// </summary>
    [ObservableProperty]
    public partial string Danm { get; set; } = "";
    /// <summary>
    /// 工区
    /// </summary>
    [ObservableProperty]
    public partial string Koku { get; set; } = "";
    /// <summary>
    /// 位置
    /// </summary>
    [ObservableProperty]
    public partial string Location { get; set; } = "";

    /// <summary>
    /// 工程
    /// </summary>
    [ObservableProperty]
    public partial ListItem Proc { get; set; }
    [ObservableProperty]
    public partial List<ListItem> ProcList { get; set; } = new List<ListItem>();
    /// <summary>
    /// 確認項目
    /// </summary>
    [ObservableProperty]
    public partial ListItem Proj { get; set; }
    [ObservableProperty]
    public partial List<ListItem> ProjList { get; set; } = new List<ListItem>();
    /// <summary>
    /// 判定基準
    /// </summary>
    [ObservableProperty]
    public partial string Hantei { get; set; }
    [ObservableProperty]
    public partial Dictionary<string, string> HanteiDic { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// 撮影方向
    /// </summary>
    [ObservableProperty]
    public partial string DirsDisplyName { get; set; }
    [ObservableProperty]
    public partial ListItem Dirs { get; set; }
    [ObservableProperty]
    public partial List<ListItem> DirsList { get; set; } = new List<ListItem>();
    [ObservableProperty]
    public partial List<ListItem> DbDirsList { get; set; } = new List<ListItem>();
    /// <summary>
    /// 備考
    /// </summary>
    [ObservableProperty]
    public partial string Comment { get; set; }
    /// <summary>
    /// 撮影日時
    /// </summary>
    [ObservableProperty]
    public partial string Date { get; set; }
    /// <summary>
    /// 撮影者
    /// </summary>
    [ObservableProperty]
    public partial string Author { get; set; }

    [ObservableProperty]
    public partial int Checkrsl1StrokeThickness { get; set; } = 0;
    [ObservableProperty]
    public partial int Checkrsl2StrokeThickness { get; set; } = 0;
    [ObservableProperty]
    public partial int Checkrsl4StrokeThickness { get; set; } = 0;
    [ObservableProperty]
    public partial int Checkrsl5StrokeThickness { get; set; } = 0;

    [ObservableProperty]
    public partial int Sign { get; set; }

    [ObservableProperty]
    public partial bool IsBlackboardVisible { get; set; }
    [ObservableProperty]
    public partial bool IsStrokesVisible { get; set; }

    [ObservableProperty]
    public partial bool IsSvg { get; set; }

    [ObservableProperty]
    public partial string SelectedTool { get; set; }
    [ObservableProperty]
    public partial bool BallpointPenSelected { get; set; }
    [ObservableProperty]
    public partial bool PencilSelected { get; set; }
    [ObservableProperty]
    public partial bool HighlighterSelected { get; set; }
    [ObservableProperty]
    public partial bool LineSelected { get; set; }
    [ObservableProperty]
    public partial bool FixedRectSelected { get; set; }
    [ObservableProperty]
    public partial bool FixedCircleSelected { get; set; }
    [ObservableProperty]
    public partial bool TextSelected { get; set; }
    [ObservableProperty]
    public partial bool CircleTextSelected { get; set; }
    [ObservableProperty]
    public partial bool RectTextSelected { get; set; }
    [ObservableProperty]
    public partial bool AcceptSelected { get; set; }
    [ObservableProperty]
    public partial bool MoveSelected { get; set; }
    [ObservableProperty]
    public partial bool EraserSelected { get; set; }
    [ObservableProperty]
    public partial bool EmptySelected { get; set; }

    public bool IsPreviousSwitchable => (!IsSvg || EmptySelected) && _selectImageIndex > 0;
    public bool IsNextSwitchable => (!IsSvg || EmptySelected) && (_selectImageIndex > -1 && _selectImageIndex < SourceImageList.Count - 1);

    [ObservableProperty]
    public partial bool IsImageGalleryVisible { get; set; } = true;
    [ObservableProperty]
    public partial bool IsImageGridVisible { get; set; } = false;

    private List<ProjectPhotoMessage> _projectPhotoList = new List<ProjectPhotoMessage>();

    public CheckPointDetailPageVM(ILogger<CheckPointDetailPageVM> logger, ICheckPointDetailService service, ICheckPointService checkPointService) : base(logger, service)
    {
        _checkPointService = checkPointService;

        InitViewModel();
    }

    public override async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ImageFolderPath = Path.Combine(Service.AppContext.ConstructionSiteFolder, "photo");

        ColorPalette = new ObservableCollection<Color>(GetColorPalette());

        if (query.ContainsKey("json"))
        {
            Query = JsonSerializer.Deserialize<Dictionary<string, object>>(query["json"]?.ToString());
            if (Query.ContainsKey("ProjectPhotoList"))
            {
                _projectPhotoList = JsonSerializer.Deserialize<List<ProjectPhotoMessage>>(Query["ProjectPhotoList"]?.ToString());
            }
        }

        if (Query == null || Query.Count == 0)
        {
#if DEBUG
            Logger.LogError("クエリパラメータを取得できませんでした / Failed to retrieve query parameters");
#endif
        }

        await LoadData();

        await LoadDanmImage();

        SetSelectedTool("Empty");

        _first = false;
    }

    private async Task LoadData(Dictionary<string, object>? query = null)
    {
        if (query == null) query = Query;

        _hr01ItempInfoList = await Service.GetHR01ITEMPINFO(query["HR01001"]?.ToString(), query["HR01003"]?.ToString(), ImageFolderPath);

        await SetDirs();

        DanmTitle = query["DanmTitle"]?.ToString();

        Buim = query["Buim"]?.ToString();
        Grpl = query["Grpl"]?.ToString();
        Danm = query["Danm"]?.ToString();
        Koku = query["Koku"]?.ToString();
        Location = query["Location"]?.ToString();

        SetProc(query["Proc"]?.ToString());
        SetProj(query["Proc"]?.ToString(), query["Proj"]?.ToString());
    }

    private void SetProc(string proc = "")
    {
        var queryProcList = _hr01ItempInfoList
            .GroupBy(p => new { Code = p.HM13003, Name = p.HM09003 })
            .Select(g => g.First())
            .ToList();

        List<ListItem> procList = queryProcList.Select(item => new ListItem(item.HM09003, item.HM13003)).ToList();

        if (procList.Count == 0) return;

        ProcList = procList;

        if (!string.IsNullOrEmpty(proc))
        {
            Proc = GetListItem(proc, ProcList);
        }
        else
        {
            Proc = ProcList[0];
        }
    }
    private void SetProj(string proc, string proj = "")
    {
        var queryProjList = _hr01ItempInfoList
            .Where(item => item.HM13003 == proc)
            .Select(c => new { c.HR03004, c.HM13005, c.HM13012 })
            .Distinct()
            .ToList();

        List<ListItem> projList = new List<ListItem>();
        HanteiDic.Clear();
        for (int i = 0; i < queryProjList.Count; i++)
        {
            projList.Add(new ListItem(queryProjList[i].HM13005, queryProjList[i].HR03004));
            HanteiDic.Add(queryProjList[i].HR03004, queryProjList[i].HM13012);
        }

        if (projList.Count == 0) return;

        ProjList = projList;

        if (!string.IsNullOrEmpty(proj))
        {
            Proj = GetListItem(proj, ProjList);
        }
        else
        {
            Proj = ProjList[0];
        }
    }
    private async Task SetDirs(Dictionary<string, object>? query = null)
    {
        if (query == null) query = Query;

        var queryDirsList = await Service.GetHM16List(query["HR01001"]?.ToString());

        List<ListItem> dirsList = queryDirsList.Select(item => new ListItem(item.HM16003, item.HM16002.ToString())).ToList();

        DbDirsList = dirsList.Select(dirs => new ListItem(dirs.DisplyName, dirs.Value)).ToList();

        foreach (var item in Service.AppContext.DirectionList)
        {
            if (!dirsList.Any(dirs => dirs.DisplyName == item))
            {
                dirsList.Add(new ListItem(item, item));
            }
        }

        DirsList = dirsList;
    }

    partial void OnImageListChanged(ObservableCollection<InkImage> value)
    {
        OnPropertyChanged(nameof(Summary));
    }

    partial void OnProcChanged(ListItem value)
    {
        if (_first) return;

        if (value != null && !string.IsNullOrEmpty(value.Value))
        {
            SetProj(value.Value);
        }
    }

    partial void OnProjChanged(ListItem value)
    {
        if (value != null && !string.IsNullOrEmpty(value.Value))
        {
            Hantei = HanteiDic[value.Value];
        }
    }

    partial void OnDirsChanged(ListItem value)
    {
        if (value != null)
        {
            DirsDisplyName = value.DisplyName;
        }
        else
        {
            DirsDisplyName = "";
        }
    }

    partial void OnDirsDisplyNameChanged(string value)
    {
        if (value.Trim() == "" && Dirs != null)
        {
            DirsDisplyName = Dirs.DisplyName;
        }
    }

    partial void OnCommentChanged(string value)
    {

    }

    partial void OnSignChanged(int value)
    {
        Checkrsl1StrokeThickness = 0;
        Checkrsl2StrokeThickness = 0;
        Checkrsl4StrokeThickness = 0;
        Checkrsl5StrokeThickness = 0;

        switch (value)
        {
            case 0:
                Checkrsl1StrokeThickness = 1;
                break;
            case 1:
                Checkrsl2StrokeThickness = 1;
                break;
            case 2:
                Checkrsl4StrokeThickness = 1;
                break;
            case 3:
                Checkrsl5StrokeThickness = 1;
                break;
            default:
                break;
        }
    }

    [RelayCommand]
    private void ToggleImageGallery()
    {
        IsImageGalleryVisible = true;
        IsImageGridVisible = false;
    }

    [RelayCommand]
    private void ToggleImageGrid()
    {
        IsImageGalleryVisible = false;
        IsImageGridVisible = true;
    }

    [RelayCommand]
    private async Task DeleteImageAsync()
    {
        InkImage? image = ImageList.Where(image => image.IsSelected).FirstOrDefault();
        if (image == null) return;

        int index = ImageList.IndexOf(image);

        ImageList.RemoveAt(index);
        SourceImageList.RemoveAt(index);

        var message = new AsyncRequestMessage("ClearImage");
        _ = WeakReferenceMessenger.Default.Send(message);
        await message.Tcs.Task;

        message = new AsyncRequestMessage("SwitchImage", new Dictionary<string, object?> { { "image", ImageList.Count > index ? ImageList[index] : ImageList.LastOrDefault() } });
        _ = WeakReferenceMessenger.Default.Send(message);
        await message.Tcs.Task;

        OnPropertyChanged(nameof(Summary));
    }

    [RelayCommand]
    private void ToggleSort()
    {
        IsAscending = !IsAscending;

        SourceImageList = IsAscending
        ? SourceImageList.OrderBy(x => x.Sort).ToList()
        : SourceImageList.OrderByDescending(x => x.Sort).ToList();

        ImageList.Clear();
        LoadNextPage();

        OnPropertyChanged(nameof(Summary));
        OnPropertyChanged(nameof(SortText));
    }

    /// <summary>
    /// 後方へ移動
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task BackAsync()
    {
        if (!await ValidateBeforeBackAsync()) return;

        var message = new AsyncRequestMessage("Back");
        _ = WeakReferenceMessenger.Default.Send(message);
        await message.Tcs.Task;

        bool? success = (bool?)message.Result["success"];
        if (success != false)
        {
            await Shell.Current.GoToAsync("..", CreateNavigationParameterForCheckPoint());

            InitViewModel();
        }
        else
        {
            await Toast.ShowToast("保存に失敗しました");
        }
    }
    [RelayCommand]
    private async Task SaveAsync()
    {
        RemoveProjectPhotosBeforeSave();

        var message = new AsyncRequestMessage("Save");
        _ = WeakReferenceMessenger.Default.Send(message);
        await message.Tcs.Task;

        bool? success = (bool?)message.Result["success"];
        if (success == true)
        {
            await Toast.ShowToast("正常に保存されました");
        }
        else if (success == false)
        {
            await Toast.ShowToast("保存に失敗しました");
        }
    }
    [RelayCommand]
    private async Task SaveBackAsync()
    {
        RemoveProjectPhotosBeforeSave();

        var message = new AsyncRequestMessage("SaveBack");
        _ = WeakReferenceMessenger.Default.Send(message);
        await message.Tcs.Task;

        bool? success = (bool?)message.Result["success"];
        if (success != false)
        {
            await Shell.Current.GoToAsync("..", CreateNavigationParameterForCheckPoint());

            InitViewModel();
        }
        else
        {
            await Toast.ShowToast("保存に失敗しました");
        }
    }

    [RelayCommand]
    private async Task PreviousImageAsync()
    {
        var image = ImageList.Where(image => image.IsSelected).FirstOrDefault();
        if (image == null) return;

        int index = ImageList.IndexOf(image) - 1;
        if (index < 0) return;

        if (ImageList.Count <= index)
        {
            LoadNextPage();
            if (ImageList.Count <= index)
            {
                return;
            }
        }

        var previousImage = ImageList.ElementAtOrDefault(index);
        if (previousImage == null) return;

        var message = new AsyncRequestMessage("SwitchImage", new Dictionary<string, object?> { { "image", previousImage } });
        _ = WeakReferenceMessenger.Default.Send(message);
        await message.Tcs.Task;
    }

    [RelayCommand]
    private async Task NextImageAsync()
    {
        var image = ImageList.Where(image => image.IsSelected).FirstOrDefault();
        if (image == null) return;

        int index = ImageList.IndexOf(image) + 1;

        if (ImageList.Count <= index)
        {
            LoadNextPage();
            if (ImageList.Count <= index)
            {
                return;
            }
        }

        var nextImage = ImageList.ElementAtOrDefault(index);
        if (nextImage == null) return;

        var message = new AsyncRequestMessage("SwitchImage", new Dictionary<string, object?> { { "image", nextImage } });
        _ = WeakReferenceMessenger.Default.Send(message);
        await message.Tcs.Task;
    }

    private void InitViewModel()
    {
        ImageList.Clear();
        SourceImageList.Clear();
        ImagesCount = 0;

        DanmImageSource = null;

        DanmTitle = "";
        Buim = "";
        Grpl = "";
        Danm = "";
        Koku = "";
        Location = "";

        Dirs = null;
        Comment = "";
        Date = "";
        Author = "";

        Proc = null;
        Proj = null;
        Hantei = "";

        Checkrsl1StrokeThickness = 0;
        Checkrsl2StrokeThickness = 0;
        Checkrsl4StrokeThickness = 0;
        Checkrsl5StrokeThickness = 0;
        Sign = -1;

        IsBlackboardVisible = false;
        IsStrokesVisible = false;
        IsSvg = false;

        ClearSelectedTool();

        ToggleImageGallery();

        _first = true;
    }

    public void LoadImageInfo(InkImage image)
    {
        if ((Dirs = GetListItem(image.Dirs, DirsList)) == null)
        {
            List<ListItem> dirsList = DirsList.Select(dirs => new ListItem(dirs.DisplyName, dirs.Value)).ToList();
            dirsList.Add(new ListItem(image.Dirs, image.Dirs));
            DirsList = dirsList;
            Dirs = GetListItem(image.Dirs, DirsList);
        }
        Comment = image.Comment.Trim();
        Date = image.Date;
        Author = image.Author;
        Sign = image.Sign;

        IsBlackboardVisible = image.IsBlackboardVisible;
        IsStrokesVisible = image.IsStrokesVisible;

        IsSvg = image.IsSvg;

        OnPropertyChanged(nameof(IsPreviousSwitchable));
        OnPropertyChanged(nameof(IsNextSwitchable));
    }

    public async Task<bool> SaveImageInfo(InkImage image)
    {
        if (DirsDisplyName != Dirs.DisplyName && GetListItemFromDisplyName(DirsDisplyName, DirsList) == null)
        {
            List<ListItem> dirsList = DirsList.Select(dirs => new ListItem(dirs.DisplyName, dirs.Value)).ToList();
            dirsList.Add(new ListItem(DirsDisplyName, DirsDisplyName));
            DirsList = dirsList;
            Dirs = GetListItem(DirsDisplyName, DirsList);
        }

        image.Dirs = Dirs.Value;
        image.DirsDisplyName = Dirs.DisplyName;
        image.Comment = Comment.Trim();
        image.Date = Date;
        image.Author = Author;

        image.IsBlackboardVisible = IsBlackboardVisible;
        image.IsStrokesVisible = IsStrokesVisible;

        HR03SYAS hr03 = CreateDbImageInfo(image);

        return await Service.UpdateHR03(new List<HR03SYAS> { hr03 }, ImageFolderPath);
    }

    public async Task<bool> SaveImageList(List<InkImage> imageList, List<InkImage>? changedImageList = null)
    {
        if (imageList == null) return true;

        var hr03List = CreateDbImageList(imageList, changedImageList);

        if (!await Service.UpdateHR03(hr03List, ImageFolderPath)) return false;

        if (changedImageList != null)
        {
            foreach (var image in changedImageList)
            {
                if (image.IsDeleted)
                {
                    if (File.Exists(image.FullName))
                    {
                        try
                        {
                            File.Delete(image.FullName);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Logger.LogError(ex, "指定されたパスの画像の削除に失敗しました / Failed to delete image at specified path: {Path}", image.FullName);
#endif
                            // 无法删除文件，请检查文件是否正在使用。
                            DialogHelper.MessageDialog("ファイルを削除できません。ファイルが他のプログラムで使用中ではないか確認してください。");

                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }
    public void ResetImageList()
    {
        if (SourceImageList != null)
        {
            Original‌ImageList.Clear();
            Original‌ImageList = SourceImageList.Select(image => image.Clone()).ToList();
        }
    }
    public void RevertImageList(InkImage? image = null)
    {
        if (Original‌ImageList != null)
        {
            if (image == null)
            {
                SourceImageList.Clear();
                SourceImageList = new List<InkImage>(Original‌ImageList.Select(image => image.Clone()).ToList());
            }
            else
            {
                int index = SourceImageList.IndexOf(image);
                if (index != -1)
                {
                    var originalImage = Original‌ImageList.FirstOrDefault(x => x.Code == image.Code);
                    if (originalImage != null)
                    {
                        SourceImageList[index] = originalImage.Clone();
                        SourceImageList[index].IsVisualized = true;
                    }
                }
            }
        }
    }
    public List<InkImage> GetChangedImageList()
    {
        var changedImageList = new List<InkImage>();

        if (OriginalImageList == null || SourceImageList == null) return changedImageList;

        foreach (var originalImage in OriginalImageList)
        {
            var current = SourceImageList.FirstOrDefault(x => x.Code == originalImage.Code);

            if (current == null)
            {
                var deletedImage = originalImage.Clone();
                deletedImage.IsDeleted = true;
                changedImageList.Add(deletedImage);
            }
            else if (current.Sign != originalImage.Sign)
            {
                changedImageList.Add(current);
            }
        }

        return changedImageList;
    }

    public List<HR03SYAS> CreateDbImageList(List<InkImage> imageList, List<InkImage>? changedImageList = null)
    {
        if (changedImageList == null) changedImageList = new List<InkImage>();

        List<HR03SYAS> hr03List = new List<HR03SYAS>();
        for (int i = 0; i < imageList.Count; i++)
        {
            var image = imageList[i];

            string sort = IsAscending ? (i + 1).ToString().PadLeft(4, '0') : (imageList.Count - i).ToString().PadLeft(4, '0');

            var changedImage = changedImageList.FirstOrDefault(x => x.Code == image.Code);
            if (changedImage != null)
            {
                changedImage.Sort = sort;
                hr03List.Add(CreateDbImageInfo(changedImage));
            }
            else if (sort != image.Sort)
            {
                image.Sort = sort;
                hr03List.Add(CreateDbImageInfo(image));
            }
        }

        hr03List.AddRange(changedImageList.Where(image => image.IsDeleted).Select(image => CreateDbImageInfo(image)));

        return hr03List;
    }
    public HR03SYAS CreateDbImageInfo(InkImage image)
    {
        HR03SYAS hr03 = new HR03SYAS();
        hr03.CHANGE = image.IsDeleted ? "DELETE" : "UPDATE";
        hr03.HR03001 = Query["HR01001"]?.ToString();
        hr03.HR03002 = image.Code.PadRight(46, ' ');
        hr03.HR03003 = Query["HR01003"]?.ToString();
        hr03.HR03004 = Proj.Value;
        hr03.HR03005 = image.Sort;
        hr03.HR03006 = image.Sign;
        hr03.HR03007 = DbDirsList.Any(dirs => dirs.Value == image.Dirs) ? int.Parse(image.Dirs) : -1;
        hr03.HR03008 = image.Comment.Trim();
        hr03.HR03009 = int.Parse(DateTime.Parse(image.Date).ToString("yyyyMMdd"));
        hr03.HR03010 = int.Parse(DateTime.Parse(image.Date).ToString("HHmmss"));
        hr03.HR03011 = image.CreationDate;
        hr03.HR03012 = image.Author;
        hr03.HR03013 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        hr03.HR03014 = Service.AppContext.Name;
        hr03.HR03015 = image.SyncDate;
        hr03.HR03016 = image.SyncAuthor;
        hr03.HR03017 = image.IsSvg ? 1 : 0;
        hr03.HR03018 = image.IsBlackboardVisible && image.IsStrokesVisible ? 7 :
                       image.IsBlackboardVisible ? 3 :
                       image.IsStrokesVisible ? 5 : 1;
        hr03.HR03019 = DbDirsList.Any(dirs => dirs.Value == image.Dirs) ? "" : image.DirsDisplyName;
        hr03.HR03020 = image.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
        return hr03;
    }

    public async Task<List<InkImage>> GetImageInfo(Dictionary<string, object> query, string proj)
    {
        var result = await Service.GetHR03Pic(query["HR01001"]?.ToString(), query["HR01003"]?.ToString(), proj);

        return result.Select(item => new InkImage
        {
            Name = $"{item.HR03002}{(item.HR03017 == 0 ? ".jpg" : ".svg")}",

            Dirs = item.HR03007 != -1 ? item.HR03007.ToString() : item.HR03019,
            DirsDisplyName = item.HR03007 != -1 ? GetDisplyName(item.HR03007.ToString(), DirsList) : item.HR03019,
            Comment = !string.IsNullOrEmpty(item.HR03008.Trim()) ? item.HR03008.Trim() : " ",
            Date = DateTime.ParseExact(item.HR03009.ToString("D8") + item.HR03010.ToString("D6"), "yyyyMMddHHmmss", null).ToString("yyyy/MM/dd HH:mm:ss"),
            CreationDate = item.HR03011,
            Author = item.HR03012,

            SyncDate = item.HR03015,
            SyncAuthor = item.HR03016,

            IsBlackboardVisible = item.HR03018 == 3 || item.HR03018 == 7,
            IsStrokesVisible = item.HR03018 == 5 || item.HR03018 == 7,

            Sign = item.HR03006,

            Sort = item.HR03005
        }).ToList();
    }

    public bool ImageInfoChanged(InkImage image)
    {
        return image.Dirs != Dirs.Value
            || image.DirsDisplyName != DirsDisplyName
            || image.Comment.Trim() != Comment.Trim()
            || DateTime.Parse(image.Date) != DateTime.Parse(Date)
            || image.Author != Author;
    }

    public bool SortChanged(List<InkImage> imageList)
    {
        List<string> originalSortList = OriginalImageList.Select(original => original.Sort).ToList();
        originalSortList = IsAscending ? originalSortList.OrderBy(x => x).ToList() : originalSortList.OrderByDescending(x => x).ToList();
        return imageList.Zip(originalSortList, (current, originalSort) => current.Sort != originalSort).Any(changed => changed);
    }

    public void UpdateDirsListPreferences(string value)
    {
        if (value != "" && !DbDirsList.Any(dirs => dirs.Value == value))
        {
            Service.AppContext.AddPreferencesDirection(value);
        }
    }

    public void SetSelectedImage(InkImage image)
    {
        ClearSelectedImage();
        image.IsSelected = true;
    }
    public void ClearSelectedImage()
    {
        foreach (var image in ImageList)
        {
            image.IsSelected = false;
        }
    }

    private ListItem? GetListItem(string value, List<ListItem> list)
    {
        return list.FirstOrDefault(e => e.Value == value);
    }
    private ListItem? GetListItemFromDisplyName(string displyName, List<ListItem> list)
    {
        return list.FirstOrDefault(e => e.DisplyName == displyName);
    }
    private string GetDisplyName(string value, List<ListItem> list)
    {
        ListItem? item = GetListItem(value, list);
        if (item == null) return string.Empty;
        return item.DisplyName;
    }

    public async Task LoadDanmImage()
    {
        DanmImageSource = await _checkPointService.GetImageSourceAsync(0);
    }

    public void LoadNextPage()
    {
        if (_isImageLoading || ImageList.Count >= SourceImageList.Count)
            return;

        _isImageLoading = true;

        try
        {
            var nextItems = SourceImageList.Skip(ImageList.Count).Take(_pageSize).ToList();

            foreach (var item in nextItems)
            {
                ImageList.Add(item);
            }
        }
        finally
        {
            _isImageLoading = false;
        }
    }

    public void NoditySummary()
    {
        OnPropertyChanged(nameof(Summary));
    }

    partial void OnSelectedToolChanged(string value)
    {
        ClearSelectedTool();

        if (!IsSvg) return;

        SetSelectedTool(value);
    }
    partial void OnIsSvgChanged(bool value)
    {
        ClearSelectedTool();

        if (!IsSvg) return;

        SetSelectedTool(SelectedTool);
    }
    private void ClearSelectedTool()
    {
        BallpointPenSelected = false;
        PencilSelected = false;
        HighlighterSelected = false;
        LineSelected = false;
        FixedRectSelected = false;
        FixedCircleSelected = false;
        TextSelected = false;
        CircleTextSelected = false;
        RectTextSelected = false;
        AcceptSelected = false;
        MoveSelected = false;
        EraserSelected = false;
        EmptySelected = false;
    }
    private void SetSelectedTool(string tool)
    {
        switch (tool)
        {
            case "BallpointPen":
                BallpointPenSelected = true; break;
            case "Pencil":
                PencilSelected = true; break;
            case "Highlighter":
                HighlighterSelected = true; break;
            case "Line":
                LineSelected = true; break;
            case "FixedRect":
                FixedRectSelected = true; break;
            case "FixedCircle":
                FixedCircleSelected = true; break;
            case "Text":
                TextSelected = true; break;
            case "CircleText":
                CircleTextSelected = true; break;
            case "RectText":
                RectTextSelected = true; break;
            case "Accept":
                AcceptSelected = true; break;
            case "Move":
                MoveSelected = true; break;
            case "Eraser":
                EraserSelected = true; break;
            case "Empty":
                EmptySelected = true; break;
        }

        OnPropertyChanged(nameof(IsPreviousSwitchable));
        OnPropertyChanged(nameof(IsNextSwitchable));
    }

    // 颜料版可选颜色集合
    public ObservableCollection<Color> GetColorPalette()
    {
        return new ObservableCollection<Color>()
            {
                Colors.Black, Color.FromArgb("#FFE600"), Color.FromArgb("#E61E1E"), Colors.Red, Colors.Orange, Colors.Yellow,
                Colors.Green, Colors.Blue, Colors.Purple, Colors.Pink, Colors.Brown, Colors.Cyan,
                Colors.Magenta, Colors.DarkBlue, Colors.DarkGreen, Colors.DarkRed, Color.FromArgb("#FFD700"), Color.FromArgb("#C0C0C0"),
                Color.FromArgb("#808000"), Color.FromArgb("#008080"), Color.FromArgb("#F7D7C4"), Color.FromArgb("#613D30"), Color.FromArgb("#FFFF81"), Color.FromArgb("#BCB3FF")
            };
    }

    // 工具的大小范围
    public (float Min, float Max) GetToolSizeRange(string type)
    {
        return type switch
        {
            "BallpointPen" => (1f, 24f),
            "Pencil" => (1f, 24f),
            "Highlighter" => (12f, 64f),
            "Line" => (1f, 24f),
            "FixedRect" => (1f, 100f),
            "FixedCircle" => (1f, 50f),
            _ => (1f, 24f)
        };
    }

    /// <summary>
    /// 確認项目画面のナビゲーションパラメータを作成する
    /// </summary>
    /// <returns></returns>
    Dictionary<string, object> CreateNavigationParameterForCheckPoint()
    {
        var obj = new
        {
            FromPage = "CheckPointDetailPage",
            ProjectCode = Proc.Value,        //工程コード		
            InspectionItemCode = Proj.Value, //確認项目コード
            ProjectPhotoList = _projectPhotoList //写真コレクション

        };
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        string jsonString = JsonSerializer.Serialize(obj, options);
        return new Dictionary<string, object> { { "json", jsonString } };
    }

    async Task<bool> ValidateBeforeBackAsync()
    {
        if (_projectPhotoList.Count == 0) return true;

        var photoPathList = _projectPhotoList
            .Where(x => x.ProjectCode == Proc.Value && x.InspectionItemCode == Proj.Value)
            .Select(x => x.PhotoPath)
            .ToList();

        if (photoPathList.Count == 0) return true;

        string ErrMsg = ErrorMessage.ERRORPOP("CM01031");
        var result = await DialogHelper.MessageDialogButton2(ErrMsg);

        if (result == NCDialogResult.No) return false;

        await _checkPointService.DiscardImageAsync(photoPathList);
        RemoveProjectPhotosBeforeSave();
        return true;
    }

    void RemoveProjectPhotosBeforeSave()
    {
        _projectPhotoList.RemoveAll(x => x.ProjectCode == Proc.Value && x.InspectionItemCode == Proj.Value);
    }

    public class AsyncRequestMessage : RequestMessage<bool>
    {
        public string Name { get; }
        public Dictionary<string, object?> Parameters { get; set; }
        public Dictionary<string, object> Result { get; set; }

        public TaskCompletionSource<bool> Tcs { get; } = new();

        public AsyncRequestMessage(string name)
        {
            Name = name;
            Parameters = new Dictionary<string, object?>();
            Result = new Dictionary<string, object>();
        }
        public AsyncRequestMessage(string name, Dictionary<string, object?> parameters)
        {
            Name = name;
            Parameters = parameters;
            Result = new Dictionary<string, object>();
        }
    }
}