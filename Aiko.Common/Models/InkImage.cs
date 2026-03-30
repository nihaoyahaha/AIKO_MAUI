using Aiko.Common.InkTools;
using CommunityToolkit.Mvvm.ComponentModel;
using FFImageLoading.Maui;
using SkiaSharp;
using Svg;
using Svg.Skia;

namespace Aiko.Common.Models;

public partial class InkImage : ObservableObject, IDisposable
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _fullName;
    public string Code => Path.GetFileNameWithoutExtension(Name);
    public string Extension => Path.GetExtension(Name).ToLower();
    [ObservableProperty]
    private DateTime _creationTime;
    [ObservableProperty]
    private DateTime _lastWriteTime;

    [ObservableProperty]
    private string _dirs;
    [ObservableProperty]
    private string _dirsDisplyName;
    [ObservableProperty]
    private string _comment;
    [ObservableProperty]
    private string _date;
    [ObservableProperty]
    private string _author;

    [ObservableProperty]
    private string _syncDate;
    [ObservableProperty]
    private string _syncAuthor;

    [ObservableProperty]
    private string _sort;

    // 笔迹数据
    [ObservableProperty]
    private List<InkStroke> _strokes;

    public double Width => Bitmap?.Width ?? PhotoSvg?.Picture.CullRect.Width ?? 0;
    public double Height => Bitmap?.Height ?? PhotoSvg?.Picture.CullRect.Height ?? 0;

    // 运行时渲染资源
    [ObservableProperty]
    private SKBitmap _bitmap;
    [ObservableProperty]
    private SKSvg _photoSvg;
    [ObservableProperty]
    private SKSvg _blackboardSvg;

    [ObservableProperty]
    private bool _isBlackboardVisible = true;
    [ObservableProperty]
    private bool _isStrokesVisible = true;

    public bool IsSvg => Extension == ".svg";

    [ObservableProperty]
    private int _sign = 1;

    [ObservableProperty]
    private bool _isSelected = false;

    [ObservableProperty]
    private bool _isDeleted = false;

    // 缓存刷新机制
    [ObservableProperty]
    private string _cacheBuster = Guid.NewGuid().ToString();
    [ObservableProperty]
    private bool _cacheUpdater = true;

    [ObservableProperty]
    private FFImageLoading.Work.LoadingPriority _priority = FFImageLoading.Work.LoadingPriority.Normal;

    // UI 绑定的图片源
    public ImageSource InkImageSource
    {
        get
        {
            if (string.IsNullOrEmpty(FullName) || !File.Exists(FullName)) return null;

            try
            {
                string saltedKey = $"{FullName}?v={_cacheBuster}";

                if (Extension == ".svg")
                {
                    return SvgImageSource.FromStream(() => { return File.OpenRead(FullName); }, 0, 0);
                }
                else
                {
                    return ImageSource.FromStream(() =>
                    {
                        return new FileStream(FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Source生成失败: {ex.Message}");
                return null;
            }
        }
    }

    public void RefreshPreview()
    {
        CacheBuster = Guid.NewGuid().ToString();
        CacheUpdater = false; // 触发 UI 卸载
        OnPropertyChanged(nameof(InkImageSource));

        MainThread.BeginInvokeOnMainThread(() =>
        {
            CacheUpdater = true; // 触发 UI 重载
            OnPropertyChanged(nameof(InkImageSource));
        });
    }

    public async Task SetBitmap()
    {
        ClearBitmap();
        await Task.Run(() =>
        {
            if (Extension == ".svg")
            {
                byte[] svgBytes = File.ReadAllBytes(FullName);

                var photoSvgDoc = SvgDocument.Open<SvgDocument>(new MemoryStream(svgBytes));
                var blackboardSvgDoc = SvgDocument.Open<SvgDocument>(new MemoryStream(svgBytes));

                FilterElements(photoSvgDoc, new[] { "photo" });
                SetElementVisible(photoSvgDoc, "photo");
                PhotoSvg = new SKSvg();
                PhotoSvg.FromSvgDocument(photoSvgDoc);

                FilterElements(blackboardSvgDoc, new[] { "blackboard" });
                SetElementVisible(blackboardSvgDoc, "blackboard");
                BlackboardSvg = new SKSvg();
                BlackboardSvg.FromSvgDocument(blackboardSvgDoc);
            }
            else
            {
                Bitmap = SKBitmap.Decode(FullName);
            }
        });
    }

    private void SetElementVisible(SvgDocument doc, string id)
    {
        foreach (var child in doc.Children)
        {
            if (child is SvgImage img && img.ID?.ToLower() == id.ToLower())
            {
                img.Display = "inline"; // 强制设为可见，以便 Skia 能够绘制它
            }
        }
    }

    public void ClearBitmap()
    {
        Bitmap?.Dispose();
        Bitmap = null;
        PhotoSvg?.Dispose();
        PhotoSvg = null;
        BlackboardSvg?.Dispose();
        BlackboardSvg = null;
    }

    private void FilterElements(SvgElement element, string[] ids)
    {
        if (ids == null || ids.Length == 0) return;

        ids = ids.Select(id => id.ToLower()).ToArray();

        for (int i = element.Children.Count - 1; i >= 0; i--)
        {
            if (element.Children[i] is SvgImage img)
            {
                if (img.ID == null || !ids.Contains(img.ID.ToLower()))
                {
                    element.Children.RemoveAt(i);
                }
            }
            else
            {
                element.Children.RemoveAt(i);
            }
        }
    }

    public InkImage Clone()
    {
        var clone = new InkImage
        {
            Name = this.Name,
            FullName = this.FullName,
            CreationTime = this.CreationTime,
            LastWriteTime = this.LastWriteTime,
            Dirs = this.Dirs,
            DirsDisplyName = this.DirsDisplyName,
            Comment = this.Comment,
            Date = this.Date,
            Author = this.Author,
            SyncDate = this.SyncDate,
            SyncAuthor = this.SyncAuthor,
            Sort = this.Sort,
            Sign = this.Sign,
            IsBlackboardVisible = this.IsBlackboardVisible,
            IsStrokesVisible = this.IsStrokesVisible,
            IsSelected = this.IsSelected,
            IsDeleted = this.IsDeleted,
            CacheBuster = Guid.NewGuid().ToString(),
            CacheUpdater = this.CacheUpdater,

            Strokes = this.Strokes?.Select(stroke => stroke.Clone()).ToList()
        };

        if (this.Bitmap != null && this.PhotoSvg != null)
        {
            _ = clone.SetBitmap();
        }

        return clone;
    }

    public void Dispose() => ClearBitmap();
}