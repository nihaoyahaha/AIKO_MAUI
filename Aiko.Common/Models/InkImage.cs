using Aiko.Common.InkTools;
using CommunityToolkit.Mvvm.ComponentModel;
using FFImageLoading.Maui;
using SkiaSharp;
using Svg;

namespace Aiko.Common.Models;

public partial class InkImage : ObservableObject, IDisposable
{
    [ObservableProperty]
    public partial string Name { get; set; }
    [ObservableProperty]
    public partial string FullName { get; set; }
    public string Code => Path.GetFileNameWithoutExtension(Name);
    public string Extension => Path.GetExtension(Name).ToLower();
    [ObservableProperty]
    public partial DateTime CreationTime { get; set; }
    [ObservableProperty]
    public partial DateTime LastWriteTime { get; set; }

    [ObservableProperty]
    public partial string Dirs { get; set; }
    [ObservableProperty]
    public partial string DirsDisplyName { get; set; }
    [ObservableProperty]
    public partial string Comment { get; set; }
    [ObservableProperty]
    public partial string Date { get; set; }
    [ObservableProperty]
    public partial string CreationDate { get; set; }
    [ObservableProperty]
    public partial string Author { get; set; }
    [ObservableProperty]
    public partial string SyncDate { get; set; }
    [ObservableProperty]
    public partial string SyncAuthor { get; set; }

    [ObservableProperty]
    public partial string Sort { get; set; }

    // 笔迹数据
    [ObservableProperty]
    public partial List<InkStroke> Strokes { get; set; }
    [ObservableProperty]
    public partial double Width { get; set; } = 0;
    [ObservableProperty]
    public partial double Height { get; set; } = 0;

    // 运行时渲染资源
    [ObservableProperty]
    public partial SKBitmap? Bitmap { get; set; }
    [ObservableProperty]
    public partial SKBitmap? PhotoBitmap { get; set; }
    [ObservableProperty]
    public partial SKBitmap? BlackboardBitmap { get; set; }
    [ObservableProperty]
    public partial (float X, float Y, float Width, float Height) BlackboardBitmapRect { get; set; }
    [ObservableProperty]
    public partial bool IsBlackboardVisible { get; set; } = true;
    [ObservableProperty]
    public partial bool IsStrokesVisible { get; set; } = true;
    public bool IsSvg => Extension == ".svg";

    [ObservableProperty]
    public partial int Sign { get; set; } = 1;
    [ObservableProperty]
    public partial bool IsSelected { get; set; } = false;

    [ObservableProperty]
    public partial bool IsDeleted { get; set; } = false;

    [ObservableProperty]
    public partial bool IsVisualized { get; set; } = false;

    // 缓存刷新机制
    [ObservableProperty]
    public partial string CacheBuster { get; set; } = Guid.NewGuid().ToString();
    [ObservableProperty]
    public partial bool CacheUpdater { get; set; } = true;

    // UI 绑定的图片源
    public ImageSource InkImageSource
    {
        get
        {
            if (string.IsNullOrEmpty(FullName) || !File.Exists(FullName)) return null;

            try
            {
                string saltedKey = $"{FullName}?v={CacheBuster}";

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
                System.Diagnostics.Debug.WriteLine($"Source generation failed: {ex.Message}");
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

        Width = 0;
        Height = 0;

        if (Extension == ".svg")
        {
            byte[] svgBytes = await File.ReadAllBytesAsync(FullName);
            var svgDoc = SvgDocument.Open<SvgDocument>(new MemoryStream(svgBytes));

            if (svgDoc == null) return;

            var photoElement = svgDoc.GetElementById("photo") as SvgImage;
            if (photoElement != null)
            {
                PhotoBitmap = GetBitmapFromSvgImage(photoElement);
            }

            var blackboardElement = svgDoc.GetElementById("blackboard") as SvgImage;
            if (blackboardElement != null)
            {
                BlackboardBitmap = GetBitmapFromSvgImage(blackboardElement);
                BlackboardBitmapRect = (
                    blackboardElement.X.Value,
                    blackboardElement.Y.Value,
                    blackboardElement.Width.Value,
                    blackboardElement.Height.Value
                );
            }

            if (photoElement != null)
            {
                Width = photoElement.Width.Value;
                Height = photoElement.Height.Value;
            }
        }
        else
        {
            Bitmap = SKBitmap.Decode(FullName);

            Width = Bitmap.Width;
            Height = Bitmap.Height;
        }

        if (Width == 0 || Height == 0)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to retrieve image dimensions (Image Path: {FullName})");
        }
    }

    private SKBitmap? GetBitmapFromSvgImage(SvgImage svgImage)
    {
        try
        {
            string href = svgImage.Href.ToString();

            if (href.Contains(","))
            {
                string base64Data = href.Split(',')[1];
                byte[] imageBytes = Convert.FromBase64String(base64Data);
                return SKBitmap.Decode(imageBytes);
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error decoding SvgImage: {ex.Message}");
            return null;
        }
    }

    public void ClearBitmap()
    {
        Bitmap?.Dispose();
        Bitmap = null;
        PhotoBitmap?.Dispose();
        PhotoBitmap = null;
        BlackboardBitmap?.Dispose();
        BlackboardBitmap = null;
        BlackboardBitmapRect = (0, 0, 0, 0);
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
            CreationDate = this.CreationDate,
            Author = this.Author,
            SyncDate = this.SyncDate,
            SyncAuthor = this.SyncAuthor,
            Sort = this.Sort,
            IsBlackboardVisible = this.IsBlackboardVisible,
            IsStrokesVisible = this.IsStrokesVisible,
            Sign = this.Sign,
            IsSelected = this.IsSelected,
            IsDeleted = this.IsDeleted,
            CacheBuster = Guid.NewGuid().ToString(),
            CacheUpdater = this.CacheUpdater,

            Strokes = this.Strokes?.Select(stroke => stroke.Clone()).ToList()
        };

        if (this.Bitmap != null || this.PhotoBitmap != null)
        {
            _ = clone.SetBitmap();
        }

        return clone;
    }

    public void Dispose()
    {
        ClearBitmap();
        GC.SuppressFinalize(this);
    }
}