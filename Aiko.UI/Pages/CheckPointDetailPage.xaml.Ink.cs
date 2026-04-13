using Aiko.Common.InkTools;
using Aiko.Common.InkTools.InkToolsList;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Aiko.UI.Pages;

public partial class CheckPointDetailPage : ContentPage
{
    private IInkTool _currentTool => _toolManager.CurrentTool;

    // 文本编辑状态
    private bool _isTextEditing = false;
    private SKPoint _currentTextPosition;
    private SKColor _currentTextColor;
    private float _currentTextSize;
    private string _currentTextFont;
    private SKColor? _currentBackgoundColor;

    // 存储当前选中的颜色 Border，用于视觉状态切换
    private Border _selectedColorBorder;

    // --- 画布 ---

    private void OnTouch(object sender, SKTouchEventArgs e)
    {
        if (_isTextEditing && !InkToolConfig.IsFont(_currentTool.Type))
            FinishTextEdit();

        _toolManager.HandleTouch(e, InkCanvasView, null, null);
    }

    private void OnPhotoCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Clear(SKColors.Transparent);

        if (_currentImage != null)
        {
            if (_currentImage.Bitmap != null)
            {
                canvas.DrawBitmap(_currentImage.Bitmap, 0, 0);
            }
            else if (_currentImage.PhotoSvg != null)
            {
                canvas.DrawPicture(_currentImage.PhotoSvg.Picture);
            }
        }

        using (var paint = new SKPaint())
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = Color.FromArgb("#CCCCCC").ToSKColor();
            paint.StrokeWidth = 1;

            canvas.DrawRect(new SKRect(0, 0, e.Info.Width, e.Info.Height), paint);
        }
    }
    private void OnBlackboardCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Clear(SKColors.Transparent);

        if (_currentImage != null)
        {
            if (_currentImage.Bitmap != null)
            {
                canvas.DrawBitmap(_currentImage.Bitmap, 0, 0);
            }
            else if (_currentImage.PhotoSvg != null)
            {
                canvas.DrawPicture(_currentImage.PhotoSvg.Picture);
            }

            if (_currentImage.BlackboardSvg != null)
            {
                canvas.DrawPicture(_currentImage.BlackboardSvg.Picture);
            }
        }
    }
    private void OnInkCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Clear(SKColors.Transparent);

        if (_currentImage != null)
        {
            _toolManager.HandleDrawing(canvas, null, null);
        }
    }

    // --- 画布浏览 ---

    private string _lastCurrentImageName;
    private void UpdateImageDimensions()
    {
        if (_currentImage == null) return;

        if (_lastCurrentImageName == _currentImage.Name) return;
        _lastCurrentImageName = _currentImage.Name;

        double mauiWidth = _currentImage.Width / InkUtils.Density;
        double mauiHeight = _currentImage.Height / InkUtils.Density;

        ZoomLayer.WidthRequest = mauiWidth;
        ZoomLayer.HeightRequest = mauiHeight;

        var rect = new Rect(0, 0, mauiWidth, mauiHeight);
        AbsoluteLayout.SetLayoutBounds(PhotoCanvasView, rect);
        AbsoluteLayout.SetLayoutBounds(BlackboardCanvasView, rect);
        AbsoluteLayout.SetLayoutBounds(InkCanvasView, rect);

        PinchToZoom.SetBaseSize(mauiWidth, mauiHeight);
    }

    // --- 画布按钮 ---

    private void OnBtnBlackboardClicked(object sender, EventArgs e)
    {
        _vm.IsBlackboardVisible = !_vm.IsBlackboardVisible;
    }
    private void OnBtnInkClicked(object sender, EventArgs e)
    {
        _vm.IsStrokesVisible = !_vm.IsStrokesVisible;
    }
    private async void OnBtnSaveClicked(object sender, EventArgs e)
    {
        bool? success = await Save(false, false);
        if (success == true)
        {
            await _vm.Toast.ShowToast("正常に保存されました");
        }
        else if (success == false)
        {
            await _vm.Toast.ShowToast("保存に失敗しました");
        }
    }

    // --- 工具切换 ---

    private void OnToolClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton btn && btn.CommandParameter is string toolType)
        {
            if (_currentTool.Type == toolType)
            {
                if (InkToolConfig.IsConfigurable(toolType))
                {
                    OpenSettingsPanel(btn);
                }
            }
            else
            {
                SwitchTool(toolType);
            }
        }
    }

    private void SwitchTool(string toolType)
    {
        _toolManager.SwitchTool(toolType);
        UpdateVisualSelection(toolType);
        FinishTextEdit();
        CloseSettingsPanel();

        if (_currentTool.Type == "Empty")
        {
            InkCanvasView.InputTransparent = true;
            PinchToZoom.IsEnabled = true;
        }
        else if (InkToolConfig.IsFont(_currentTool.Type))
        {
            InkCanvasView.InputTransparent = false;
            PinchToZoom.IsEnabled = true;
        }
        else
        {
            InkCanvasView.InputTransparent = false;
            PinchToZoom.IsEnabled = false;
        }
    }

    private void OnUndoClicked(object sender, EventArgs e)
    {
        _toolManager.Undo();

        InkCanvasView.InvalidateSurface();
    }

    private void UpdateVisualSelection(string selectedToolName)
    {
        _vm.SelectedTool = selectedToolName;
    }

    // --- 设置面板 ---

    private void OpenSettingsPanel(VisualElement senderButton)
    {
        SettingsOverlay.IsVisible = true;

        // 调整面板的位置
        var rect = senderButton.GetBoundingBox();
        double buttonCenterX = rect.X + (senderButton.Width / 2);
        double panelWidth = SettingsPanel.Width > 0 ? SettingsPanel.Width : SettingsPanel.WidthRequest;
        double leftMargin = SettingsPanel.Margin.Left;
        double targetX = buttonCenterX - (panelWidth / 2) - leftMargin;
        double minX = 10 - leftMargin;
        double maxX = 780 - panelWidth - 10 - leftMargin;

        SettingsPanel.TranslationX = Math.Clamp(targetX, minX, maxX);

        // 控制面板内容的显隐
        ColorSection.IsVisible = InkToolConfig.IsStroke(_currentTool.Type) || InkToolConfig.IsFont(_currentTool.Type);

        StrokePreviewSection.IsVisible = InkToolConfig.IsStroke(_currentTool.Type);
        SizeSliderSection.IsVisible = InkToolConfig.IsStroke(_currentTool.Type) && _currentTool.Type != "FixedRect";
        FixedRectSettingsSection.IsVisible = _currentTool.Type == "FixedRect";
        FontPreviewSection.IsVisible = InkToolConfig.IsFont(_currentTool.Type);
        EraserSection.IsVisible = _currentTool.Type == "Eraser";

        // 设置范围
        if (SizeSliderSection.IsVisible)
        {
            var range = _vm.GetToolSizeRange(_currentTool.Type);
            SizeSlider.ValueChanged -= OnSizeChanged;
            SizeSlider.Minimum = range.Min;
            SizeSlider.Maximum = range.Max;
            SizeSlider.ValueChanged += OnSizeChanged;

            SizeSlider.Value = _currentTool.Size;
        }

        if (FixedRectSettingsSection.IsVisible)
        {
            var range = _vm.GetToolSizeRange(_currentTool.Type);
            FixedRectWidthSlider.ValueChanged -= OnFixedRectDimensionChanged;
            FixedRectWidthSlider.Minimum = range.Min;
            FixedRectWidthSlider.Maximum = range.Max;
            FixedRectWidthSlider.ValueChanged += OnFixedRectDimensionChanged;
            FixedRectHeightSlider.ValueChanged -= OnFixedRectDimensionChanged;
            FixedRectHeightSlider.Minimum = range.Min;
            FixedRectHeightSlider.Maximum = range.Max;
            FixedRectHeightSlider.ValueChanged += OnFixedRectDimensionChanged;
        }

        if (FontPreviewSection.IsVisible)
        {
            TextTool textTool = _currentTool as TextTool;
            FontPicker.SelectedItem = textTool.Font;
            FontSizePicker.SelectedItem = (double)textTool.Size;
        }

        // 同步当前颜色到 FlexLayout 的选中状态
        SyncSelectedColorUI(_currentTool.Color.ToMauiColor());

        // 更新笔迹预览
        UpdateStrokePreview();
        UpdateFontPreview();
    }

    private void SyncSelectedColorUI(Color currentColor)
    {
        // 遍历 FlexLayout 的子元素找到匹配颜色的 Border
        foreach (var child in ColorFlexLayout.Children)
        {
            if (child is Border border && border.BindingContext is Color color)
            {
                if (color.ToHex() == currentColor.ToHex()) // 使用 Hex 比较避免对象引用不同
                {
                    VisualStateManager.GoToState(border, "Selected");
                    _selectedColorBorder = border;
                }
                else
                {
                    VisualStateManager.GoToState(border, "Normal");
                }
            }
        }
    }

    private void CloseSettingsPanel()
    {
        SettingsOverlay.IsVisible = false;
    }

    private void OnOverlayTapped(object sender, TappedEventArgs e)
    {
        CloseSettingsPanel();
    }

    // --- 设置面板 - 颜色设置 ---

    private void OnColorTapped(object sender, EventArgs e)
    {
        if (sender is Border tappedBorder && tappedBorder.BindingContext is Color selectedColor)
        {
            // 视觉状态切换
            if (_selectedColorBorder != null)
                VisualStateManager.GoToState(_selectedColorBorder, "Normal");

            VisualStateManager.GoToState(tappedBorder, "Selected");
            _selectedColorBorder = tappedBorder;

            // 更新工具颜色
            _currentTool.Color = selectedColor.ToSKColor();

            if (_isTextEditing)
            {
                TextEditor.TextColor = selectedColor;
                _currentTextColor = selectedColor.ToSKColor();
            }

            UpdateStrokePreview();
            UpdateFontPreview();
        }
    }

    // --- 设置面板 - 参数设置 ---

    private void OnSizeChanged(object sender, ValueChangedEventArgs e)
    {
        _currentTool.Size = (float)e.NewValue;

        UpdateStrokePreview();
        UpdateFontPreview();
    }

    private void OnFixedRectDimensionChanged(object sender, ValueChangedEventArgs e)
    {
        if (_currentTool is FixedRectTool fixedRectTool)
        {
            fixedRectTool.Width = (float)FixedRectWidthSlider.Value;
            fixedRectTool.Height = (float)FixedRectHeightSlider.Value;

            UpdateStrokePreview();
        }
    }

    private void OnFontChanged(object sender, EventArgs e)
    {
        if (FontPicker.SelectedItem is string font && _currentTool is TextTool textTool)
        {
            textTool.Font = font;
            if (_isTextEditing)
            {
                TextEditor.FontFamily = font;
                _currentTextFont = font;
                AdjustTextEditorSize();
            }

            UpdateFontPreview();
        }
    }
    private void OnFontSizeChanged(object sender, EventArgs e)
    {
        if (FontSizePicker.SelectedItem is double size)
        {
            _currentTool.Size = (float)size;
            if (_isTextEditing)
            {
                TextEditor.FontSize = size / InkUtils.Density;
                _currentTextSize = (float)size;
                AdjustTextEditorSize();
            }

            UpdateFontPreview();
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        FinishTextEdit();
        CloseSettingsPanel();

        _toolManager.ClearStrokes();

        InkCanvasView.InvalidateSurface();
    }

    private void UpdateStrokePreview()
    {
        if (!InkToolConfig.IsStroke(_currentTool.Type)) return;

        UpdatePreviewLayout();

        StrokePreview.InvalidateSurface();
    }

    private void UpdateFontPreview()
    {
        if (!InkToolConfig.IsFont(_currentTool.Type)) return;

        FontPreview.InvalidateSurface();
    }

    private void UpdatePreviewLayout()
    {
        if (_currentTool is FixedRectTool fixedRectTool)
        {
            StrokePreviewGrid.HeightRequest = Math.Max(fixedRectTool.Height, 80);
        }
        if (_currentTool is FixedCircleTool fixedCircleTool)
        {
            StrokePreviewGrid.HeightRequest = Math.Max(fixedCircleTool.Size * 2, 80);
        }
    }

    private void OnStrokePreviewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (!InkToolConfig.IsStroke(_currentTool.Type)) return;

        var info = e.Info;
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        float width = info.Width;
        float height = info.Height;
        float midY = height / 2;
        float margin = 15;

        // 构造预览笔迹
        var previewStroke = new InkStroke
        {
            Type = _currentTool.Type,
            Color = _currentTool.Color,
            Size = _currentTool.Size,
            Points = new List<SKPoint>()
        };

        if (_currentTool is LineTool lineTool)
        {
            // 直线仅起点和终点
            var p1 = new SKPoint(margin, midY);
            var p2 = new SKPoint(width - margin, midY);
            previewStroke.Points.Add(p1);
            previewStroke.Points.Add(p2);

            var linePath = new SKPath();
            linePath.MoveTo(p1);
            linePath.LineTo(p2);
            previewStroke.Path = linePath;
        }
        else if (_currentTool is FixedRectTool fixedRectTool)
        {
            previewStroke.Width = fixedRectTool.Width;
            previewStroke.Height = fixedRectTool.Height;

            float rectWidth = (float)FixedRectWidthSlider.Value;
            float rectHeight = (float)FixedRectHeightSlider.Value;

            float startX = (width / 2) - (rectWidth / 2);
            float startY = (height / 2) - (rectHeight / 2);

            previewStroke.Points = new List<SKPoint> { new SKPoint(startX, startY) };
        }
        else if (_currentTool is FixedCircleTool fixedCircleTool)
        {
            previewStroke.Points = new List<SKPoint> { new SKPoint(width / 2, height / 2) };
        }
        else
        {
            // 曲线工具：生成正弦波点集 [0, 2PI]
            float amplitude = height * 0.3f; // 振幅
            float frequency = 1.0f; // 周期
            var wavePath = new SKPath();

            for (float x = margin; x <= width - margin; x += 2)
            {
                float t = (x - margin) / (width - 2 * margin);

                float radians = t * (float)Math.PI * 2 * frequency;
                float y = midY - (float)Math.Sin(radians) * amplitude;

                var pt = new SKPoint(x, y);
                previewStroke.Points.Add(pt);

                if (x == margin) wavePath.MoveTo(pt);
                else wavePath.LineTo(pt);
            }
            previewStroke.Path = wavePath;
        }

        _currentTool.Draw(canvas, previewStroke);

        // 显式释放路径和滤镜资源，防止滑动 Slider 时内存波动
        previewStroke.Path?.Dispose();
    }

    private void OnFontPreviewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (!InkToolConfig.IsFont(_currentTool.Type)) return;

        var info = e.Info;
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        TextTool textTool = _currentTool as TextTool;

        using var typeface = SKTypeface.FromFamilyName(textTool.Font, SKFontStyle.Normal);
        using var font = new SKFont
        {
            Typeface = typeface,
            Size = textTool.Size,
            Edging = SKFontEdging.Antialias
        };
        using var paint = new SKPaint
        {
            Color = textTool.Color,
            IsAntialias = true
        };
        string previewText = textTool.Font; // 预览文本
        if (textTool is AcceptTool acceptTool) previewText = acceptTool.AcceptSymbol; // 确认符号的特殊预览文本

        float textWidth = font.MeasureText(previewText, paint);
        font.GetFontMetrics(out var metrics);
        float textCenterOffset = (metrics.Ascent + metrics.Descent) / 2;
        float x = (info.Width - textWidth) / 2;
        float y = (info.Height / 2) - textCenterOffset;

        canvas.DrawText(previewText, x, y, font, paint);
    }

    // --- 文本 ---

    private void OnTextEditRequested(SKPoint position, SKColor color, float size, string font, SKColor? bgColor)
    {
        if (_isTextEditing) FinishTextEdit();

        StartTextEdit(position, color, size, font, bgColor);
    }

    private void StartTextEdit(SKPoint position, SKColor color, float size, string font, SKColor? bgColor)
    {
        _isTextEditing = true;
        _currentTextPosition = position;
        _currentTextColor = color;
        _currentTextSize = size;
        _currentTextFont = font;
        _currentBackgoundColor = bgColor;

        TextEditor.Text = "";
        TextEditor.TextColor = color.ToMauiColor();
        TextEditor.FontSize = size / InkUtils.Density;
        TextEditor.FontFamily = font;
        TextEditor.IsVisible = true;
        TextEditor.Focus();

        Rect textEditorRect = new Rect((float)((position.X - 10) / InkUtils.Density), (float)((position.Y - 10) / InkUtils.Density), 100, 50);
        AbsoluteLayout.SetLayoutBounds(TextEditor, textEditorRect);
    }

    private void FinishTextEdit()
    {
        if (!_isTextEditing) return;

        var text = TextEditor.Text?.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            if (_currentTool.Type == "textTool")
            {
                _toolManager.AddTextStroke(_currentTool.Type, text, _currentTextPosition, _currentTextColor, _currentTextSize, _currentTextFont);
            }
            else
            {
                _toolManager.AddTextStroke(_currentTool.Type, text, _currentTextPosition, _currentBackgoundColor ?? SKColors.Transparent, _currentTextSize, _currentTextFont);
            }

            InkCanvasView.InvalidateSurface();
        }

        TextEditor.IsVisible = false;
        TextEditor.Text = "";
        _isTextEditing = false;
    }

    private void OnTextEditorUnfocused(object sender, FocusEventArgs e)
    {
        if (!SettingsOverlay.IsVisible)
            FinishTextEdit();
    }

    private void OnTextEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isTextEditing && TextEditor.IsVisible)
            AdjustTextEditorSize();
    }

    private void AdjustTextEditorSize()
    {
        var sizeRequest = TextEditor.Measure(double.PositiveInfinity, double.PositiveInfinity);
        double newWidth = Math.Max(100, sizeRequest.Width + 20);
        double newHeight = Math.Max(40, sizeRequest.Height);

        AbsoluteLayout.SetLayoutBounds(TextEditor, new Rect(
            (_currentTextPosition.X - 10) / InkUtils.Density,
            (_currentTextPosition.Y - 10) / InkUtils.Density,
            newWidth,
            newHeight));
    }
}