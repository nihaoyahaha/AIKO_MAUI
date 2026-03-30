using Aiko.Common.InkTools;
using Aiko.Common.Models;
using Svg;

namespace Aiko.UI.Pages;

public partial class CheckPointDetailPage : ContentPage
{
    // --- 图像 ---

    private async Task<InkImage> CreateInkImage(FileInfo file, InkImage? image = null)
    {
        List<InkStroke> strokes = new List<InkStroke>();
        string jsonPath = Path.ChangeExtension(file.FullName, ".json");

        if (File.Exists(jsonPath))
        {
            try
            {
                string jsonContent = await File.ReadAllTextAsync(jsonPath);
                strokes = InkService.Instance.FromJson<List<InkStroke>>(jsonContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadImageError: {ex.Message}");
            }
        }

        return new InkImage
        {
            Name = file.Name,
            FullName = file.FullName,
            CreationTime = file.CreationTime,
            LastWriteTime = file.LastWriteTime,
            Strokes = strokes,

            Dirs = image != null ? image.Dirs : "-1",
            DirsDisplyName = image != null ? image.DirsDisplyName : "",
            Comment = image != null ? image.Comment : "This is a comment ",
            Date = image != null ? image.Date : file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss"),
            Author = image != null ? image.Author : "",

            SyncDate = image != null ? image.SyncDate : file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss"),
            SyncAuthor = image != null ? image.SyncAuthor : "",

            IsBlackboardVisible = image != null ? image.IsBlackboardVisible : false,
            IsStrokesVisible = image != null ? image.IsStrokesVisible : false,

            Sign = image != null ? image.Sign : 1,

            Sort = image != null ? image.Sort : ""
        };
    }

    private async Task LoadGalleryImages(string path, List<InkImage>? images = null)
    {
        if (!Directory.Exists(path)) return;

        var extensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };

        var files = Directory.EnumerateFiles(path)
            .Where(file => extensions.Any(ext => string.Equals(ext, Path.GetExtension(file), StringComparison.OrdinalIgnoreCase)))
            .Select(file => new FileInfo(file))
            .ToList();

        if (images != null)
        {
            List<string> imageNames = images.Select(image => image.Name).ToList();
            for (int i = files.Count - 1; i >= 0; i--)
            {
                var file = files[i];

                if (!imageNames.Contains(file.Name))
                {
                    files.Remove(file);
                }
            }
        }

        var missingImages = _vm.ImageList.Where(image => files.All(file => file.FullName != image.FullName)).ToList();
        foreach (var image in missingImages)
        {
            _vm.ImageList.Remove(image);
        }

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            var existingImage = _vm.ImageList.FirstOrDefault(image => image.FullName == file.FullName);

            if (existingImage != null)
            {
                if (existingImage.LastWriteTime != file.LastWriteTime)
                {
                    existingImage.LastWriteTime = file.LastWriteTime;

                    string jsonPath = Path.ChangeExtension(file.FullName, ".json");
                    if (File.Exists(jsonPath))
                    {
                        var jsonContent = await File.ReadAllTextAsync(jsonPath);
                        var newStrokes = InkService.Instance.FromJson<List<InkStroke>>(jsonContent);
                        existingImage.Strokes = newStrokes ?? new List<InkStroke>();
                    }

                    existingImage.RefreshPreview();

                    if (_currentImage == existingImage)
                    {
                        await existingImage.SetBitmap();
                        _toolManager.LoadStrokes(existingImage.Strokes);

                        PhotoCanvasView.InvalidateSurface();
                        BlackboardCanvasView.InvalidateSurface();
                        InkCanvasView.InvalidateSurface();
                    }
                }
            }
            else
            {
                var newImage = await CreateInkImage(file, images?.Where(image => image.Name == file.Name).First());

                if (i < _vm.ImageList.Count)
                    _vm.ImageList.Insert(i, newImage);
                else
                    _vm.ImageList.Add(newImage);

                newImage.RefreshPreview();
            }
        }

        var sortedImageList = _vm.IsAscending ? _vm.ImageList.OrderBy(x => x.Sort).ToList() : _vm.ImageList.OrderByDescending(x => x.Sort).ToList();
        for (int i = 0; i < sortedImageList.Count; i++)
        {
            var oldIndex = _vm.ImageList.IndexOf(sortedImageList[i]);
            if (oldIndex != i)
            {
                _vm.ImageList.Move(oldIndex, i);
            }
        }

        _vm.ResetImageList();
    }

    private async Task LoadGalleryImage(string fullName)
    {
        if (string.IsNullOrEmpty(fullName) || !File.Exists(fullName)) return;

        var image = _vm.ImageList.FirstOrDefault(image => image.FullName == fullName);

        if (image != null)
        {
            string jsonPath = Path.ChangeExtension(fullName, ".json");
            if (File.Exists(jsonPath))
            {
                string jsonContent = await File.ReadAllTextAsync(jsonPath);
                foreach (var stroke in image.Strokes) stroke?.Dispose();
                image.Strokes.Clear();
                var strokes = InkService.Instance.FromJson<List<InkStroke>>(jsonContent);
                if (strokes != null) image.Strokes.AddRange(strokes);
            }
            image.RefreshPreview();
        }
        else
        {
            _vm.ImageList.Insert(0, await CreateInkImage(new FileInfo(fullName)));
        }
    }

    private async Task LoadCanvasViewImage(InkImage? image)
    {
        if (image == null || string.IsNullOrEmpty(image.FullName))
            return;

        _vm.SetSelectedImage(image);

        _toolManager.ResetStrokesChange();

        if (_currentImage == image && (image.Bitmap != null || image.PhotoSvg != null || image.BlackboardSvg != null))
            return;

        _currentImage?.ClearBitmap();

        await image.SetBitmap();
        _currentImage = image;

        InkControl.BindingContext = _currentImage;

        _toolManager.LoadStrokes(image.Strokes);

        _vm.LoadImageInfo(image);

        UpdateImageDimensions();

        PhotoCanvasView.InvalidateSurface();
        BlackboardCanvasView.InvalidateSurface();
        InkCanvasView.InvalidateSurface();
    }

    private async Task SaveImage(InkImage image, List<InkStroke>? strokes = null)
    {
        if (image == null || image.Extension != ".svg") return;

        if (strokes != null) image.Strokes = strokes;

        try
        {
            var oldDoc = SvgDocument.Open<SvgDocument>(image.FullName);

            var newDoc = new SvgDocument
            {
                ViewBox = oldDoc.ViewBox,
                Width = oldDoc.Width,
                Height = oldDoc.Height
            };

            // 仅获取标签为 image 且 id 为 photo 和 blackboard 的元素
            foreach (var child in oldDoc.Children)
            {
                if (child is SvgImage img)
                {
                    if (img.ID == "photo")
                    {
                        var _img = (SvgElement)img.DeepCopy();
                        newDoc.Children.Add(_img);
                    }
                    if (img.ID == "blackboard")
                    {
                        var _img = (SvgElement)img.DeepCopy();
                        _img.Display = _vm.IsBlackboardVisible ? "inline" : "none";
                        newDoc.Children.Add(_img);
                    }
                }
            }

            int canvasWidth = newDoc.ViewBox.Width > 0 ? (int)newDoc.ViewBox.Width : (int)newDoc.Width.Value;
            int canvasHeight = newDoc.ViewBox.Height > 0 ? (int)newDoc.ViewBox.Height : (int)newDoc.Height.Value;

            string base64 = InkService.Instance.ExportToBase64(canvasWidth, canvasHeight, image.Strokes);
            if (!string.IsNullOrEmpty(base64))
            {
                var notes = new SvgImage
                {
                    ID = "notes",
                    Href = $"data:image/png;base64,{base64}",
                    X = new SvgUnit(0f),
                    Y = new SvgUnit(0f),
                    Width = new SvgUnit(SvgUnitType.Percentage, 100f),
                    Height = new SvgUnit(SvgUnitType.Percentage, 100f),
                    AspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.none),
                    Display = _vm.IsStrokesVisible ? "inline" : "none"
                };
                newDoc.Children.Add(notes);
            }

            using (var fs = new FileStream(image.FullName, FileMode.Create, FileAccess.Write))
            {
                newDoc.Write(fs);
            }

            var fileInfo = new FileInfo(image.FullName);
            if (fileInfo.Exists)
            {
                image.LastWriteTime = fileInfo.LastWriteTime;
            }

            SaveJson(image);

            _toolManager.ResetStrokesChange();

            await LoadGalleryImage(image.FullName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SaveImageError: {ex.Message}");
        }
    }

    private void SaveJson(InkImage image)
    {
        if (image == null) return;

        try
        {
            string jsonPath = Path.ChangeExtension(image.FullName, ".json");
            string jsonContent = InkService.Instance.ToJson(image.Strokes);
            File.WriteAllText(jsonPath, jsonContent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SaveJsonError: {ex.Message}");
        }
    }
}