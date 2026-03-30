
using Microsoft.Maui.Graphics.Platform;
using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;
namespace Aiko.UI;

public class GraphicsDrawable : IDrawable
{
	#region 属性
	/// <summary>
	/// 鼠标拖拽时的参考点
	/// </summary>
	private PointF _tempMovePoint;
	/// <summary>
	/// 元素选中状态 
	/// </summary>
	private bool _selected;
	/// <summary>
	/// 起始点
	/// </summary>
	private PointF _startPoint;
	/// <summary>
	/// 新增图形时鼠标拉取范围的宽度
	/// </summary>
	private float _pullingWidth;
	/// <summary>
	/// 新增图形时鼠标拉取范围的高度
	/// </summary>
	private float _pullingHeight;

	/// <summary>
	/// 图形集合
	/// </summary>
	private List<ICustomShape> _shapes = new();
	public List<ICustomShape> Shapes { get => _shapes; set => _shapes = value; }

	/// <summary>
	/// 将要绘制的图形类型
	/// </summary>
	private CustomShapeType _shapeType;//= CustomShapeType.AreaPointLine;
	public CustomShapeType ShapeType
	{
		get => _shapeType;
		set => _shapeType = value;
	}

	/// <summary>
	/// 工区确认点隐藏显示
	/// </summary>
	private bool _ctmImgTextVisible = true;
	public bool CtmImgTextVisible { get => _ctmImgTextVisible; set => _ctmImgTextVisible = value; }

	//工区锁定
	private bool _kokuLocked = false;
	public bool KokuLocked { get => _kokuLocked; set => _kokuLocked = value; }

	//确认点锁定
	private bool _kokuImgLocked = false;
	public bool KokuImgLocked { get => _kokuImgLocked; set => _kokuImgLocked = value; }
	#endregion

	public GraphicsDrawable() { }

	public GraphicsDrawable(List<ICustomShape> shapes) => _shapes = shapes;

	private IImage GetImageFromStream(string imgName)
	{
		IImage image = null;
		try
		{
			Assembly assembly = GetType().GetTypeInfo().Assembly;
			using (Stream stream = assembly.GetManifestResourceStream($"Aiko.UI_NET9.Resources.Images.ConstIcons.{imgName}"))
			{
				image = PlatformImage.FromStream(stream);
			}
		}
		catch (Exception ex)
		{
			throw;
		}
		return image;
	}

	//确保鼠标往任意方向拉伸范围的长宽为正数
	private (float x, float y, float width, float height) ComputeRectRange(PointF endPoint, float width, float height)
	{
		//向起点的左上方拉动绘制矩形
		if (height < 0 && width < 0)
		{
			return (endPoint.X, endPoint.Y, Math.Abs(width), Math.Abs(height));
		}
		//向起点的右上方拉动绘制矩形
		else if (height < 0 && width > 0)
		{
			return (_startPoint.X, _startPoint.Y - Math.Abs(height), width, Math.Abs(height));
		}
		//向起点的左下方拉动绘制矩形
		else if (height > 0 && width < 0)
		{
			return (endPoint.X, endPoint.Y - height, Math.Abs(width), height);
		}
		//向起点的右下方拉动绘制矩形
		else
			return (_startPoint.X, _startPoint.Y, width, height);
	}

	//获取多边形工区
	private CSelectPolygon GetCSelectPolygon(Color strokeColor, float strokeSize, float x, float y, float width, float height, string text)
	{
		CSelectKoku cSelectKoku = new()
		{
			StrokeColor = strokeColor,
			StrokeSize = strokeSize,
			X = x,
			Y = y,
			Width = width,
			Height = height,
			BlLocked = _kokuLocked
		};
		CSelectPolygon cSelectPolygon = cSelectKoku.GetPolygon();
		CSelectKokuImg kokuImg = GetCSelectKokuImg(x + width / 2, y + height / 2, text);
		cSelectPolygon.KokuImg = kokuImg;
		return cSelectPolygon;
	}

	//获取确认点
	private CCheckPoint GetCCheckPoint(float x, float y, string text, string imgName)
	{
		IImage image = GetImageFromStream(imgName);
		string parentId = GetContainsRectId(x, y);
		return new CCheckPoint(image, parentId)
		{
			Text = text,
			IsTextVisible = _ctmImgTextVisible,
			X = x,
			Y = y,
			Width = image.Width,
			Height = image.Height,
			FontSize = 18,
			Font = Microsoft.Maui.Graphics.Font.Default,
			FontColor = Colors.Black,
			BlLocked = _kokuImgLocked
		};
	}

	//获取矩形工区
	private CSelectKoku GetCSelectKoku(Color strokeColor, float strokeSize, float x, float y, float width, float height, string text)
	{
		CSelectKoku rect = new()
		{
			StrokeColor = strokeColor,
			StrokeSize = strokeSize,
			X = x,
			Y = y,
			Width = width,
			Height = height,
			BlLocked = _kokuLocked
		};

		rect.KokuImg = GetCSelectKokuImg(x + width / 2, y + height / 2, text);
		return rect;
	}

	//获取工区图片
	private CSelectKokuImg GetCSelectKokuImg(float x, float y, string text)
	{
		IImage image = GetImageFromStream("smallarea.png");
		return new CSelectKokuImg(image)
		{
			Text = text,
			X = x,
			Y = y,
			Width = image.Width,
			Height = image.Height,
			FontSize = 18,
			Font = Microsoft.Maui.Graphics.Font.Default,
			FontColor = Colors.Black
		};
	}

	private string GetContainsRectId(float x, float y)
	{
		foreach (var item in Shapes.Where(o => o.Type == CustomShapeType.Rectangle).ToList())
		{
			if (((CSelectKoku)item).IsContainsPoint(new Point(x, y)))
			{
				return ((CSelectKoku)item).Id;
			}
		}
		return "";
	}

	//清空
	public void Clear() => _shapes = new();//_shapes.RemoveAll(o => o.Type is not CustomShapeType.AreaPointLine);

	/// <summary>
	/// 初始化确认点
	/// </summary>
	/// <param name="points"></param>
	public void InitCCheckPoint(IEnumerable<CCheckPoint> points)
	{
		if (points is not null)
		{
			foreach (var point in points)
			{
				if (point is null) continue;
				if (point.Source is null)
				{
					point.Source = GetImageFromStream(point.SourceName);
					point.Width = point.Source.Width;
					point.Height = point.Source.Height;
				}
				_shapes.Add(point);
			}
		}
	}

	/// <summary>
	/// 初始化工区
	/// </summary>
	public void InitCSelectKoku(IEnumerable<CSelectKoku> kokus)
	{
		if (kokus is not null)
		{
			foreach (var koku in kokus)
			{
				if (koku is not null)
				{
					if (koku.KokuImg is not null && koku.KokuImg.Source is null)
					{
						var img = GetImageFromStream("smallarea.png");
						koku.KokuImg.Source = img;
						koku.KokuImg.Width = img.Width;
						koku.KokuImg.Height = img.Height;
					}
					_shapes.Add(koku);
				}
			}
		}
	}

	/// <summary>
	/// 初始化多边形工区
	/// </summary>
	/// <param name="cSelectPolygon"></param>
	public void InitCSelectPolygon(IEnumerable<CSelectPolygon> polygons)
	{
		if (polygons is not null)
		{
			foreach (var polygon in polygons)
			{
				if (polygon is not null)
				{
					if (polygon.KokuImg is not null && polygon.KokuImg.Source is null)
					{
						var img = GetImageFromStream("smallarea.png");
						polygon.KokuImg.Source = img;
						polygon.KokuImg.Width = img.Width;
						polygon.KokuImg.Height = img.Height;
					}
					_shapes.Add(polygon);

				}
			}
		}
	}

	/// <summary>
	/// 初始化红蓝点线
	/// </summary>
	/// <param name="ctmAreaPointLine"></param>
	public void InitCControlPoint(CControlPoint cControlPoint)
	{
		if (cControlPoint is not null && _shapes.Count(o => o.Type == CustomShapeType.AreaPointLine) == 0)
		{
			_shapes.Add(cControlPoint);
		}
	}

	/// <summary>
	/// 指针移动（获取聚焦图形时的状态）
	/// </summary>
	/// <param name="pt">目标点</param>
	public void MoveHoverInteraction(Microsoft.Maui.Graphics.PointF pt)
	{
		_shapes.ForEach(shape =>
		{
			shape.MoveHoverInteraction(pt);
		});
	}

	/// <summary>
	/// 按下（选中，不选中）
	/// </summary>
	/// <param name="pt"></param>
	public void StartInteraction(Microsoft.Maui.Graphics.PointF pt)
	{
		if (_shapeType is CustomShapeType.PointTextImage && _shapes.Count(o => o.Selected || o.FocusType != CustomShapeFocusType.Unfocused /*o.FocusType == CustomShapeFocusType.Move*/ ) == 0)
		{
			_shapes.Add(GetCCheckPoint(pt.X, pt.Y, "hello", "pt.png"));
		}
		_startPoint = pt;
		_tempMovePoint = pt;
		_shapes.ForEach(o => o.StartInteraction());
		_selected = _shapes.Count(o => o.Selected) > 0 ? true : false;
	}

	/// <summary>
	/// 拖拽移动(计算新图形范围，改变位置，改变范围)
	/// </summary>
	/// <param name="pt"></param>
	public void DragInteraction(Microsoft.Maui.Graphics.PointF pt)
	{
		if (_selected)
		{
			//移动单个图形
			_shapes.FirstOrDefault(o => o.Selected)?.DragInteraction(pt, pt.X - _tempMovePoint.X, pt.Y - _tempMovePoint.Y);
		}
		else
		{
			//计算新图形范围
			_pullingWidth = pt.X - _startPoint.X;
			_pullingHeight = pt.Y - _startPoint.Y;
		}
		_tempMovePoint = pt;
	}

	/// <summary>
	/// 释放（增加图形，不增加图形）
	/// </summary>
	/// <param name="pt"></param>
	public void EndInteraction(Microsoft.Maui.Graphics.PointF pt)
	{
		if (_shapeType != CustomShapeType.PointTextImage
		   && _shapeType != CustomShapeType.AreaPointLine
		   && _shapes.Count(o => o.Selected) == 0
		   && _pullingWidth != 0
		   && _pullingHeight != 0)
		{
			var rang = ComputeRectRange(pt, pt.X - _startPoint.X, pt.Y - _startPoint.Y);

			if (_shapeType == CustomShapeType.Rectangle)
				_shapes.Add(GetCSelectKoku(Colors.Red, 5, rang.x, rang.y, rang.width, rang.height, ""));

			if (_shapeType == CustomShapeType.Polygon)
				_shapes.Add(GetCSelectPolygon(Colors.Red, 5, rang.x, rang.y, rang.width, rang.height, "hello"));

		}
		else
		{
			_shapes.FirstOrDefault(o => o.Selected)?.EndInteraction(pt);
		}
		_pullingHeight = 0;
		_pullingWidth = 0;
	}

	/// <summary>
	/// 画布上绘制图形
	/// </summary>
	/// <param name="canvas"></param>
	/// <param name="dirtyRect"></param>
	public void Draw(ICanvas canvas, RectF dirtyRect)
	{
		if (_shapes?.Count > 0)
		{
			foreach (var shape in _shapes.OrderBy(o => o.Selected).ToList())
			{
				shape.Draw(canvas, dirtyRect);
			}
		}

		if (_shapeType is CustomShapeType.PointTextImage) return;
		if (_shapeType is CustomShapeType.AreaPointLine) return;

		canvas.StrokeColor = Colors.Red;
		canvas.StrokeSize = 5;
		//canvas.DrawRectangle(_startPoint.X, _startPoint.Y, _pullingWidth, _pullingHeight);
	}

}


