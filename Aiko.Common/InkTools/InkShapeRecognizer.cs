using SkiaSharp;

namespace Aiko.Common.InkTools;

/// <summary>
/// 识别出的图形类型，命名参考 UWP InkAnalysisDrawingKind，便于后续和平台识别结果对齐。
/// </summary>
public enum InkRecognizedShapeType
{
    /// <summary>
    /// 未识别或未知图形。
    /// </summary>
    Unknown,

    /// <summary>
    /// 普通三角形。
    /// </summary>
    Triangle,

    /// <summary>
    /// 等腰三角形。
    /// </summary>
    IsoscelesTriangle,

    /// <summary>
    /// 等边三角形。
    /// </summary>
    EquilateralTriangle,

    /// <summary>
    /// 直角三角形。
    /// </summary>
    RightTriangle,

    /// <summary>
    /// 普通四边形。
    /// </summary>
    Quadrilateral,

    /// <summary>
    /// 矩形。
    /// </summary>
    Rectangle,

    /// <summary>
    /// 正方形。
    /// </summary>
    Square,

    /// <summary>
    /// 菱形。
    /// </summary>
    Diamond,

    /// <summary>
    /// 梯形。
    /// </summary>
    Trapezoid,

    /// <summary>
    /// 平行四边形。
    /// </summary>
    Parallelogram,

    /// <summary>
    /// 五边形。
    /// </summary>
    Pentagon,

    /// <summary>
    /// 六边形。
    /// </summary>
    Hexagon,

    /// <summary>
    /// 圆形。
    /// </summary>
    Circle,

    /// <summary>
    /// 椭圆形。
    /// </summary>
    Ellipse,

    /// <summary>
    /// 其他自由多边形。
    /// </summary>
    Polygon
}

/// <summary>
/// 图形识别结果，包含识别类型、匹配分数和转换后的绘图坐标集合。
/// </summary>
public sealed class InkShapeRecognitionResult
{
    /// <summary>
    /// 识别出的图形类型。
    /// </summary>
    public InkRecognizedShapeType ShapeType { get; init; }

    /// <summary>
    /// 图形类型名称，用于 UI 显示或后续业务判断。
    /// </summary>
    public string ShapeName => ShapeType.ToString();

    /// <summary>
    /// $1 模板匹配分数，值越接近 1 表示越接近模板。
    /// </summary>
    public double Score { get; init; }

    /// <summary>
    /// 规则化后的坐标集合；调用方可用这些点重建 SKPath。
    /// </summary>
    public IReadOnlyList<SKPoint> Points { get; init; } = Array.Empty<SKPoint>();
}

/// <summary>
/// 基于 $1 Unistroke Recognizer 思路的闭合图形识别器。
/// </summary>
public static class InkShapeRecognizer
{
    /// <summary>
    /// $1 算法统一重采样点数；模板和候选笔迹都采样为相同数量便于逐点比较。
    /// </summary>
    private const int NumPoints = 64;

    /// <summary>
    /// $1 算法归一化后的模板方形尺寸。
    /// </summary>
    private const float SquareSize = 250f;

    /// <summary>
    /// 归一化方形半对角线，用于把路径距离转换成 0 到 1 的匹配分数。
    /// </summary>
    private const double HalfDiagonal = 0.5 * 353.5533905932738;

    /// <summary>
    /// 黄金分割搜索常量，用于在角度范围内寻找最佳旋转匹配。
    /// </summary>
    private const double Phi = 0.5 * (-1.0 + 2.23606797749979);

    /// <summary>
    /// $1 模板匹配时允许候选笔迹旋转搜索的最大角度范围，当前为正负 45 度。
    /// </summary>
    private const double AngleRange = Math.PI / 4.0;

    /// <summary>
    /// 黄金分割搜索停止阈值，当前约等于 2 度。
    /// </summary>
    private const double AnglePrecision = Math.PI / 90.0;

    /// <summary>
    /// 自然角点数量上限。超过这个数量时更像随手曲线，不再强制转换为多边形。
    /// </summary>
    private const int MaximumNaturalPolygonCorners = 12;

    /// <summary>
    /// 自由多边形的最小角点数。3~6 分别留给标准三角形/四边形/五边形/六边形。
    /// </summary>
    private const int MinimumFreePolygonCorners = 7;

    /// <summary>
    /// 预构建的图形模板集合。模板已做标准化，识别时只需要和候选笔迹比较距离。
    /// </summary>
    private static readonly IReadOnlyList<Template> Templates = BuildTemplates();

    /// <summary>
    /// 识别闭合笔迹，并返回识别分类与规则化后的坐标集合；识别失败时返回 null，调用方保持原笔迹。
    /// </summary>
    /// <param name="points">原始手绘笔迹点集合，坐标使用 SkiaSharp 的 SKPoint。</param>
    /// <param name="minimumScore">模板匹配最低分数，低于该分数时只尝试自由多边形兜底。</param>
    /// <returns>识别成功时返回图形识别结果；不能可靠识别时返回 null。</returns>
    public static InkShapeRecognitionResult? Recognize(IReadOnlyList<SKPoint> points, double minimumScore = 0.78)
    {
        if (points.Count < 2)
        {
            return null;
        }

        // cleaned：去掉距离过近的重复点，减少触摸抖动和无意义采样点。
        var cleaned = RemoveDuplicatePoints(points);
        var pathLength = PathLength(cleaned);
        if (cleaned.Count < 2 || pathLength < 8)
        {
            return null;
        }

        // 当前只识别闭合图形，非闭合线条不参与图形转换。
        bool isClosed = IsClosedShape(cleaned);
        if (!isClosed)
        {
            return null;
        }

        // 新流程：先用 $1 模板只做粗分类，再按粗分类进入几何细分。
        // 粗分类范围固定为 triangle / rectangle / circle / caret。
        return CreateTemplateCandidate(cleaned, minimumScore);
    }

    /// <summary>
    /// 使用 $1 模板做粗分类，并根据粗分类结果进入对应的几何细分逻辑。
    /// </summary>
    /// <param name="points">已经去重且确认闭合的笔迹点集合。</param>
    /// <param name="minimumScore">$1 模板匹配最低分数。</param>
    /// <returns>识别成功时返回规则化图形；匹配分数不足或无法生成图形点时返回 null。</returns>
    private static InkShapeRecognitionResult? CreateTemplateCandidate(IReadOnlyList<SKPoint> points, double minimumScore)
    {
        // 【$1 模板识别入口】
        // 这里不是按图片像素比较，而是把当前手绘轨迹归一化后，与 BuildTemplates() 中的
        // triangle / rectangle / circle / caret 四个粗分类模板逐个比较轨迹距离。
        var normalized = NormalizeClosedForMatching(points);
        Template? bestTemplate = null;
        double bestDistance = double.MaxValue;

        foreach (var template in Templates)
        {
            // 【$1 相似度距离】
            // GoldenSectionSearch 会在一定旋转角度范围内寻找最小距离。
            // distance 越小，表示当前轨迹越像这个 template.Kind 粗分类模板。
            double distance = GoldenSectionSearch(normalized, template.Points, -AngleRange, AngleRange, AnglePrecision);

            //if (template.Kind == DollarTemplateKind.Circle) distance = distance * 1.2;
            //if (template.Kind == DollarTemplateKind.Caret) distance = distance * 1.5;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTemplate = template;
            }
        }

        if (bestTemplate == null)
        {
            return null;
        }

        // 【$1 相似度分数】
        // bestDistance 是当前轨迹到最佳模板的平均路径距离。
        // score 越接近 1 越相似；低于 minimumScore 时认为模板粗分类不可靠。
        double score = 1.0 - bestDistance / HalfDiagonal;
        if (score < minimumScore)
        {
            return null;
        }

        // 【$1 粗分类后的几何细分】
        // 例如：
        // - $1 粗分为 Triangle 后，会进入 ResolveTriangleType，细分普通/等腰/等边/直角三角形。
        // - $1 粗分为 Rectangle 后，会进入 ResolveQuadrilateralType，细分普通四边形/矩形/正方形等。
        // - $1 粗分为 Circle 后，会在 ResolveShapeType 里按长短轴比例细分圆形/椭圆形。
        // - $1 粗分为 Caret 后，会进入 ResolveCaretType，细分五边形/六边形/自由多边形。
        var shapeType = ResolveShapeType(bestTemplate.Kind, points);

        var shapePoints = BuildRecognizedShapePoints(shapeType, points);
        if (shapePoints.Count < 2 && bestTemplate.Kind == DollarTemplateKind.Caret && shapeType == InkRecognizedShapeType.Polygon)
        {
            shapePoints = BuildRelaxedFreePolygonPoints(points);
        }

        if (shapePoints.Count < 2)
        {
            return null;
        }

        double geometryScore = CalculateClosedPathScore(points, shapePoints);
        score = Math.Min(score, geometryScore);
        return new InkShapeRecognitionResult
        {
            ShapeType = shapeType,
            Score = score,
            Points = shapePoints,
        };
    }

    /// <summary>
    /// 判断多边形是否存在过短边。
    /// </summary>
    /// <param name="corners">按相邻顺序排列的角点集合。</param>
    /// <param name="minimumLength">允许的最小边长。</param>
    /// <returns>存在短于阈值的边时返回 true。</returns>
    private static bool HasShortEdge(IReadOnlyList<SKPoint> corners, float minimumLength)
    {
        for (int i = 0; i < corners.Count; i++)
        {
            if (Distance(corners[i], corners[(i + 1) % corners.Count]) < minimumLength)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 将点集合补成首尾闭合的路径点集合。
    /// </summary>
    /// <param name="points">未必闭合的点集合。</param>
    /// <returns>首尾闭合后的点集合。</returns>
    private static List<SKPoint> BuildClosedPoints(IReadOnlyList<SKPoint> points)
    {
        var result = new List<SKPoint>(points);
        if (result.Count > 0 && Distance(result[0], result[^1]) > 0.5f)
        {
            result.Add(result[0]);
        }

        return result;
    }

    /// <summary>
    /// 计算原始笔迹与候选闭合图形之间的几何贴合分数。
    /// </summary>
    /// <param name="points">原始笔迹点集合。</param>
    /// <param name="candidatePoints">候选规则图形点集合。</param>
    /// <returns>0 到 1 之间的贴合分数，越接近 1 表示越贴合。</returns>
    private static double CalculateClosedPathScore(IReadOnlyList<SKPoint> points, IReadOnlyList<SKPoint> candidatePoints)
    {
        if (points.Count < 2 || candidatePoints.Count < 2)
        {
            return 0;
        }

        var bounds = GetBounds(points);
        double diagonal = Math.Max(bounds.Diagonal, 1f);
        var forward = CalculatePolylineError(points, candidatePoints, diagonal);
        var reverse = CalculatePolylineError(candidatePoints, points, diagonal);

        double averageError = (forward.Average + reverse.Average) * 0.5;
        double maxError = Math.Max(forward.Maximum, reverse.Maximum);
        double penalty = averageError * 2.2 + maxError * 0.35;
        return 1.0 - Math.Clamp(penalty, 0.0, 1.0);
    }

    /// <summary>
    /// 计算一组点到目标折线的平均误差和最大误差。
    /// </summary>
    /// <param name="source">需要评估的点集合。</param>
    /// <param name="target">目标折线点集合。</param>
    /// <param name="normalizationLength">归一化长度，通常取图形对角线。</param>
    /// <returns>归一化后的平均误差和最大误差。</returns>
    private static (double Average, double Maximum) CalculatePolylineError(
        IReadOnlyList<SKPoint> source,
        IReadOnlyList<SKPoint> target,
        double normalizationLength)
    {
        double total = 0;
        double maximum = 0;
        foreach (var point in source)
        {
            double normalizedDistance = DistanceToPolyline(point, target) / normalizationLength;
            total += normalizedDistance;
            maximum = Math.Max(maximum, normalizedDistance);
        }

        return (total / source.Count, maximum);
    }

    /// <summary>
    /// 计算点到折线的最短距离。
    /// </summary>
    /// <param name="point">待测点。</param>
    /// <param name="polyline">目标折线点集合。</param>
    /// <returns>点到任一线段的最短距离。</returns>
    private static double DistanceToPolyline(SKPoint point, IReadOnlyList<SKPoint> polyline)
    {
        double bestDistance = double.MaxValue;
        for (int i = 1; i < polyline.Count; i++)
        {
            bestDistance = Math.Min(bestDistance, DistanceToSegment(point, polyline[i - 1], polyline[i]));
        }

        if (polyline.Count > 2 && Distance(polyline[0], polyline[^1]) > 0.5f)
        {
            bestDistance = Math.Min(bestDistance, DistanceToSegment(point, polyline[^1], polyline[0]));
        }

        return bestDistance;
    }

    /// <summary>
    /// 根据最终识别类型生成规则化坐标集合。
    /// </summary>
    /// <param name="shapeType">最终识别出的图形类型。</param>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>用于重建 SKPath 的规则化坐标集合。</returns>
    private static List<SKPoint> BuildRecognizedShapePoints(InkRecognizedShapeType shapeType, IReadOnlyList<SKPoint> points)
    {
        // 不同分类输出不同的规则化点集；三角形/多边形保留角点，矩形/椭圆会做姿态修正。
        return shapeType switch
        {
            InkRecognizedShapeType.Triangle => BuildPolygonPoints(points, 3),
            InkRecognizedShapeType.IsoscelesTriangle => BuildPolygonPoints(points, 3),
            InkRecognizedShapeType.EquilateralTriangle => BuildPolygonPoints(points, 3),
            InkRecognizedShapeType.RightTriangle => BuildPolygonPoints(points, 3),
            InkRecognizedShapeType.Quadrilateral => BuildPolygonPoints(points, 4),
            InkRecognizedShapeType.Rectangle => BuildRectanglePoints(points),
            InkRecognizedShapeType.Square => BuildSquarePoints(points),
            InkRecognizedShapeType.Diamond => BuildPolygonPoints(points, 4),
            InkRecognizedShapeType.Trapezoid => BuildPolygonPoints(points, 4),
            InkRecognizedShapeType.Parallelogram => BuildPolygonPoints(points, 4),
            InkRecognizedShapeType.Pentagon => BuildPolygonPoints(points, 5),
            InkRecognizedShapeType.Hexagon => BuildPolygonPoints(points, 6),
            InkRecognizedShapeType.Circle => BuildEllipsePoints(points),
            InkRecognizedShapeType.Ellipse => BuildEllipsePoints(points),
            InkRecognizedShapeType.Polygon => BuildFreePolygonPoints(points),
            _ => new List<SKPoint>(points)
        };
    }

    /// <summary>
    /// 将模板匹配得到的粗分类进一步细分为 UWP 风格的具体图形类型。
    /// </summary>
    /// <param name="shapeType">$1 模板匹配得到的粗分类。</param>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>几何细分后的图形类型。</returns>
    private static InkRecognizedShapeType ResolveShapeType(DollarTemplateKind shapeType, IReadOnlyList<SKPoint> points)
    {
        // $1 模板负责粗识别，这里按 UWP 风格进一步细分形状。
        if (shapeType == DollarTemplateKind.Triangle)
        {
            // 【三角形判断位置】
            // $1 模板只给出 Triangle 粗分类；这里再用边长、面积、角度判断
            // 普通三角形、等腰三角形、等边三角形、直角三角形。
            var resolved = ResolveTriangleType(points);
            if (resolved == InkRecognizedShapeType.Polygon)
            {
                return InkRecognizedShapeType.Triangle;
            }
            return resolved;
        }

        if (shapeType == DollarTemplateKind.Rectangle)
        {
            // 【四边形判断位置】
            // $1 模板用 Rectangle 代表四边形粗分类；这里再用四个角点、边长、
            // 直角和对边平行关系判断普通四边形、矩形、正方形、菱形、梯形、平行四边形。
            var resolved = ResolveQuadrilateralType(points);
            if (resolved == InkRecognizedShapeType.Polygon)
            {
                return InkRecognizedShapeType.Quadrilateral;
            }
            return resolved;
        }

        if (shapeType == DollarTemplateKind.Caret)
        {
            // 【Caret 判断位置】
            // $1 粗分为 Caret 后，不直接输出 Caret，而是按角点数量细分为
            // 五边形、六边形或其他自由多边形。
            var resolved = ResolveCaretType(points);
            return resolved;
        }

        if (shapeType != DollarTemplateKind.Circle)
        {
            return InkRecognizedShapeType.Unknown;
        }

        // 【圆形判断位置】
        // $1 模板只给出 Circle 粗分类；这里根据点云主轴和副轴的半径比例，
        // 决定最终是 Circle 还是 Ellipse。
        // 圆和椭圆按主轴投影半径比例区分，避免旋转椭圆被水平外接框误判。
        // center：点云中心，用于计算点到主轴的投影半径。
        var center = GetCentroid(points);

        // axisX：点云主轴方向，旋转椭圆会沿该方向展开。
        var axisX = GetPrincipalAxis(points);

        // axisY：与主轴垂直的副轴方向。
        var axisY = new SKPoint(-axisX.Y, axisX.X);

        // radiusX/radiusY：点云在主轴/副轴上的最大投影半径。
        float radiusX = 0f;
        float radiusY = 0f;

        foreach (var point in points)
        {
            // relative：当前点相对点云中心的向量。
            var relative = new SKPoint(point.X - center.X, point.Y - center.Y);
            radiusX = Math.Max(radiusX, Math.Abs(Dot(relative, axisX)));
            radiusY = Math.Max(radiusY, Math.Abs(Dot(relative, axisY)));
        }

        // maxSide/minSide：两个投影半径中的长轴和短轴，用于区分圆与椭圆。
        float maxSide = Math.Max(radiusX, radiusY);
        float minSide = Math.Min(radiusX, radiusY);
        if (maxSide <= 0)
        {
            return InkRecognizedShapeType.Circle;
        }

        var ovalType = minSide / maxSide >= 0.88f ? InkRecognizedShapeType.Circle : InkRecognizedShapeType.Ellipse;
        return ovalType;
    }

    /// <summary>
    /// 将三角形粗分类细分为普通三角形、等腰三角形、等边三角形或直角三角形。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>三角形细分类；几何质量不足时返回 Polygon。</returns>
    private static InkRecognizedShapeType ResolveTriangleType(IReadOnlyList<SKPoint> points)
    {
        // 三角形需要能稳定简化为 3 个角点，且边长、面积、角度都在合理范围内。
        if (!TryGetPolygonCorners(points, 3, out var corners))
        {
            corners = SimplifyClosedPolygon(CreateOpenClosedContour(points), 3);
            if (corners.Count != 3)
            {
                return InkRecognizedShapeType.Polygon;
            }
        }

        // sideLengths：三条边的长度，用于判断等腰、等边和最小尺寸。
        float[] sideLengths =
        [
            Distance(corners[0], corners[1]),
            Distance(corners[1], corners[2]),
            Distance(corners[2], corners[0])
        ];

        var bounds = GetBounds(points);
        float minimumSide = Math.Max(bounds.Diagonal * 0.06f, 6f);
        float minimumArea = Math.Max(bounds.Width * bounds.Height * 0.08f, 20f);
        if (sideLengths.Any(length => length < minimumSide) || GetTriangleArea(corners) < minimumArea)
        {
            return InkRecognizedShapeType.Polygon;
        }

        // angles：三个内角，过小的角通常代表误识别或笔迹噪声。
        var angles = GetTriangleAngles(corners);
        if (angles.Any(angle => angle < 15.0))
        {
            return InkRecognizedShapeType.Polygon;
        }

        // equilateral：三边长度都接近时视为等边三角形。
        bool equilateral =
            AreSimilar(sideLengths[0], sideLengths[1], 0.18f) &&
            AreSimilar(sideLengths[1], sideLengths[2], 0.18f);
        if (equilateral)
        {
            return InkRecognizedShapeType.EquilateralTriangle;
        }

        // right：任意一个内角接近 90 度时视为直角三角形。
        bool right = angles.Any(angle => Math.Abs(angle - 90.0) <= 15.0);
        if (right)
        {
            return InkRecognizedShapeType.RightTriangle;
        }

        // isosceles：任意两边长度接近时视为等腰三角形。
        bool isosceles =
            AreSimilar(sideLengths[0], sideLengths[1], 0.2f) ||
            AreSimilar(sideLengths[1], sideLengths[2], 0.2f) ||
            AreSimilar(sideLengths[2], sideLengths[0], 0.2f);

        var triangleType = isosceles ? InkRecognizedShapeType.IsoscelesTriangle : InkRecognizedShapeType.Triangle;
        return triangleType;
    }

    /// <summary>
    /// 将四边形粗分类细分为正方形、矩形、菱形、平行四边形、梯形或普通四边形。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>四边形细分类；几何质量不足时返回 Polygon。</returns>
    private static InkRecognizedShapeType ResolveQuadrilateralType(IReadOnlyList<SKPoint> points)
    {
        // 四边形先取 4 个角点，再按直角、边长相似、对边平行等几何特征细分。
        if (!TryGetPolygonCorners(points, 4, out var corners))
        {
            return InkRecognizedShapeType.Polygon;
        }

        // sideLengths：四条边的长度，用于判断正方形和菱形。
        float[] sideLengths =
        [
            Distance(corners[0], corners[1]),
            Distance(corners[1], corners[2]),
            Distance(corners[2], corners[3]),
            Distance(corners[3], corners[0])
        ];

        // allSidesSimilar：四边长度都接近，用于识别正方形或菱形。
        bool allSidesSimilar =
            AreSimilar(sideLengths[0], sideLengths[1], 0.18f) &&
            AreSimilar(sideLengths[1], sideLengths[2], 0.18f) &&
            AreSimilar(sideLengths[2], sideLengths[3], 0.18f);

        if (IsRectangleLike(corners))
        {
            var rectangleType = allSidesSimilar ? InkRecognizedShapeType.Square : InkRecognizedShapeType.Rectangle;
            return rectangleType;
        }

        // firstOppositeParallel/secondOppositeParallel：两组对边是否接近平行。
        bool firstOppositeParallel = AreSegmentsParallel(corners[0], corners[1], corners[3], corners[2], 15.0);
        bool secondOppositeParallel = AreSegmentsParallel(corners[1], corners[2], corners[0], corners[3], 15.0);

        if (allSidesSimilar)
        {
            return InkRecognizedShapeType.Diamond;
        }

        if (firstOppositeParallel && secondOppositeParallel)
        {
            return InkRecognizedShapeType.Parallelogram;
        }

        if (firstOppositeParallel || secondOppositeParallel)
        {
            return InkRecognizedShapeType.Trapezoid;
        }
        return InkRecognizedShapeType.Quadrilateral;
    }

    /// <summary>
    /// 将 $1 caret 粗分类细分为五边形、六边形或其他自由多边形。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>caret 对应的多边形细分类。</returns>
    private static InkRecognizedShapeType ResolveCaretType(IReadOnlyList<SKPoint> points)
    {
        if (TryGetPolygonCorners(points, 5, out _))
        {
            return InkRecognizedShapeType.Pentagon;
        }

        if (TryGetPolygonCorners(points, 6, out _))
        {
            return InkRecognizedShapeType.Hexagon;
        }

        var polygonPoints = BuildFreePolygonPoints(points);
        int cornerCount = polygonPoints.Count - 1;
        if (cornerCount >= MinimumFreePolygonCorners && cornerCount <= MaximumNaturalPolygonCorners)
        {
            return InkRecognizedShapeType.Polygon;
        }
        return InkRecognizedShapeType.Polygon;
    }

    /// <summary>
    /// 将手绘闭合笔迹转换为指定角点数量的多边形坐标。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <param name="cornerCount">期望输出的角点数量，例如三角形为 3、四边形为 4。</param>
    /// <returns>闭合后的多边形坐标集合；失败时返回对应外接图形或自由多边形兜底。</returns>
    private static List<SKPoint> BuildPolygonPoints(IReadOnlyList<SKPoint> points, int cornerCount)
    {
        // 识别阶段只使用自然角点，不再强制把任意笔迹压成 3/4/5/6 个角点。
        // 这样可以避免圆被压成六边形，或自由多边形被压成四边形。
        if (TryGetPolygonCorners(points, cornerCount, out var corners))
        {
            return BuildClosedPoints(corners);
        }

        return cornerCount switch
        {
            3 => BuildFallbackTrianglePoints(points),
            4 => BuildBoundingRectanglePoints(points),
            _ => BuildFreePolygonPoints(points)
        };
    }

    /// <summary>
    /// 将矩形笔迹转换为带旋转角度的规则矩形坐标。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>闭合矩形坐标；无法稳定提取矩形角点时返回自由多边形。</returns>
    private static List<SKPoint> BuildRectanglePoints(IReadOnlyList<SKPoint> points)
    {
        // corners：通过几何规则提取出的 4 个矩形角点。
        return TryGetRectangleCorners(points, out var corners)
            ? BuildOrientedRectanglePoints(corners, false)
            : BuildFreePolygonPoints(points);
    }

    /// <summary>
    /// 将正方形笔迹转换为带旋转角度的规则正方形坐标。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>闭合正方形坐标；无法稳定提取矩形角点时返回自由多边形。</returns>
    private static List<SKPoint> BuildSquarePoints(IReadOnlyList<SKPoint> points)
    {
        // corners：通过几何规则提取出的 4 个正方形候选角点。
        return TryGetRectangleCorners(points, out var corners)
            ? BuildOrientedRectanglePoints(corners, true)
            : BuildFreePolygonPoints(points);
    }

    /// <summary>
    /// 根据四个角点生成保留手绘倾斜角度的矩形或正方形。
    /// </summary>
    /// <param name="corners">已经按绘制顺序排列的四个角点。</param>
    /// <param name="forceSquare">是否强制宽高相等；true 时输出正方形。</param>
    /// <returns>闭合的旋转矩形或正方形坐标集合。</returns>
    private static List<SKPoint> BuildOrientedRectanglePoints(IReadOnlyList<SKPoint> corners, bool forceSquare)
    {
        // 按手绘角点的方向生成旋转矩形/正方形，避免转换后被水平拉正。
        // center：四个角点的中心点，作为规则矩形的旋转中心。
        var center = GetCentroid(corners);

        // axisX：矩形第一条边的单位方向向量，用来保留手绘角度。
        var axisX = NormalizeVector(new SKPoint(corners[1].X - corners[0].X, corners[1].Y - corners[0].Y));
        if (axisX.X == 0 && axisX.Y == 0)
        {
            axisX = new SKPoint(1, 0);
        }

        // axisY：与 axisX 垂直的方向向量，表示矩形高度方向。
        var axisY = new SKPoint(-axisX.Y, axisX.X);

        // halfWidth/halfHeight：角点投影到两条轴后的半宽和半高。
        float halfWidth = 0;
        float halfHeight = 0;

        foreach (var corner in corners)
        {
            // relative：当前角点相对中心点的向量。
            var relative = new SKPoint(corner.X - center.X, corner.Y - center.Y);
            halfWidth = Math.Max(halfWidth, Math.Abs(Dot(relative, axisX)));
            halfHeight = Math.Max(halfHeight, Math.Abs(Dot(relative, axisY)));
        }

        if (forceSquare)
        {
            // side：正方形半边长，取宽高较大值避免覆盖原始笔迹。
            float side = Math.Max(halfWidth, halfHeight);
            halfWidth = side;
            halfHeight = side;
        }

        return
        [
            AddScaled(center, axisX, -halfWidth, axisY, -halfHeight),
            AddScaled(center, axisX, halfWidth, axisY, -halfHeight),
            AddScaled(center, axisX, halfWidth, axisY, halfHeight),
            AddScaled(center, axisX, -halfWidth, axisY, halfHeight),
            AddScaled(center, axisX, -halfWidth, axisY, -halfHeight)
        ];
    }

    /// <summary>
    /// 尝试从笔迹中提取满足矩形条件的四个角点。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <param name="corners">成功时输出四个矩形角点；失败时为空集合。</param>
    /// <returns>可以稳定识别为矩形时返回 true。</returns>
    private static bool TryGetRectangleCorners(IReadOnlyList<SKPoint> points, out List<SKPoint> corners)
    {
        return TryGetPolygonCorners(points, 4, out corners) && IsRectangleLike(corners);
    }

    /// <summary>
    /// 尝试从闭合笔迹中提取指定数量的多边形角点。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <param name="cornerCount">目标角点数量。</param>
    /// <param name="corners">成功时输出角点集合；失败时为空集合或数量不匹配。</param>
    /// <returns>角点数量正好等于目标数量时返回 true。</returns>
    private static bool TryGetPolygonCorners(IReadOnlyList<SKPoint> points, int cornerCount, out List<SKPoint> corners)
    {
        // 先提取“自然角点数量”，再判断是否等于目标边数。
        // 这里故意不再调用 SimplifyClosedPolygon(..., targetCount)，避免把圆/椭圆/自由多边形强行压成目标边数。
        corners = GetNaturalPolygonCorners(points);
        return corners.Count == cornerCount && IsAcceptablePolygon(points, corners, cornerCount);
    }

    /// <summary>
    /// 判断四个角点是否符合“矩形”几何条件。
    /// </summary>
    /// <param name="corners">四边形角点，要求按相邻顺序排列。</param>
    /// <returns>对边长度接近且四个角接近直角时返回 true。</returns>
    private static bool IsRectangleLike(IReadOnlyList<SKPoint> corners)
    {
        // 矩形判定：对边长度接近，四个角都接近 90 度。
        if (corners.Count != 4)
        {
            return false;
        }

        // sideLengths：四条边的长度，既用于最小尺寸过滤，也用于对边相似判断。
        float[] sideLengths =
        [
            Distance(corners[0], corners[1]),
            Distance(corners[1], corners[2]),
            Distance(corners[2], corners[3]),
            Distance(corners[3], corners[0])
        ];

        float minimumSide = Math.Max(GetBounds(corners).Diagonal * 0.05f, 6f);
        if (sideLengths.Any(length => length < minimumSide))
        {
            return false;
        }

        // oppositeSidesSimilar：两组对边长度是否分别接近。
        bool oppositeSidesSimilar =
            AreSimilar(sideLengths[0], sideLengths[2], 0.35f) &&
            AreSimilar(sideLengths[1], sideLengths[3], 0.35f);

        if (!oppositeSidesSimilar)
        {
            return false;
        }

        for (int i = 0; i < corners.Count; i++)
        {
            // previous/current/next：组成当前内角的三个相邻角点。
            var previous = corners[(i - 1 + corners.Count) % corners.Count];
            var current = corners[i];
            var next = corners[(i + 1) % corners.Count];

            // angleDegrees：当前角的角度，矩形要求接近 90 度。
            double angleDegrees = AngleBetween(previous, current, next) * 180.0 / Math.PI;

            if (Math.Abs(angleDegrees - 90.0) > 22.0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 判断两个数值是否在指定相对误差范围内。
    /// </summary>
    /// <param name="first">第一个待比较数值。</param>
    /// <param name="second">第二个待比较数值。</param>
    /// <param name="tolerance">允许的相对误差，例如 0.2 表示 20%。</param>
    /// <returns>相对误差小于等于容差时返回 true。</returns>
    private static bool AreSimilar(float first, float second, float tolerance)
    {
        // max：用较大的值作为分母，避免较小边导致比例误差被放大。
        float max = Math.Max(first, second);
        if (max <= 0)
        {
            return true;
        }

        return Math.Abs(first - second) / max <= tolerance;
    }

    /// <summary>
    /// 将向量归一化为单位向量。
    /// </summary>
    /// <param name="vector">输入向量。</param>
    /// <returns>单位向量；零向量会返回 (0, 0)。</returns>
    private static SKPoint NormalizeVector(SKPoint vector)
    {
        // length：向量长度，用于把 X/Y 分量缩放到单位长度。
        float length = MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        return length <= 0 ? new SKPoint(0, 0) : new SKPoint(vector.X / length, vector.Y / length);
    }

    /// <summary>
    /// 计算两个二维向量的点积。
    /// </summary>
    /// <param name="first">第一个向量。</param>
    /// <param name="second">第二个向量。</param>
    /// <returns>两个向量的点积结果。</returns>
    private static float Dot(SKPoint first, SKPoint second)
    {
        return first.X * second.X + first.Y * second.Y;
    }

    /// <summary>
    /// 从中心点出发，沿两条单位轴分别移动指定距离后得到新点。
    /// </summary>
    /// <param name="center">中心点。</param>
    /// <param name="axisX">第一条单位轴方向。</param>
    /// <param name="x">沿第一条轴移动的距离。</param>
    /// <param name="axisY">第二条单位轴方向。</param>
    /// <param name="y">沿第二条轴移动的距离。</param>
    /// <returns>移动后的坐标点。</returns>
    private static SKPoint AddScaled(SKPoint center, SKPoint axisX, float x, SKPoint axisY, float y)
    {
        return new SKPoint(
            center.X + axisX.X * x + axisY.X * y,
            center.Y + axisX.Y * x + axisY.Y * y);
    }

    /// <summary>
    /// 计算点云的主轴方向，用于保留旋转椭圆和旋转圆的角度。
    /// </summary>
    /// <param name="points">笔迹点集合。</param>
    /// <returns>点云变化最大的单位方向向量。</returns>
    private static SKPoint GetPrincipalAxis(IReadOnlyList<SKPoint> points)
    {
        // 用点云协方差估计主方向，供旋转椭圆和圆/椭圆分类使用。
        // center：点云中心，计算协方差前先把坐标转成相对坐标。
        var center = GetCentroid(points);

        // sumXX/sumXY/sumYY：二维协方差矩阵的三个独立分量。
        double sumXX = 0;
        double sumXY = 0;
        double sumYY = 0;

        foreach (var point in points)
        {
            // x/y：当前点相对中心点的坐标。
            double x = point.X - center.X;
            double y = point.Y - center.Y;
            sumXX += x * x;
            sumXY += x * y;
            sumYY += y * y;
        }

        // angle：协方差矩阵最大特征方向对应的旋转角。
        double angle = 0.5 * Math.Atan2(2.0 * sumXY, sumXX - sumYY);
        return NormalizeVector(new SKPoint((float)Math.Cos(angle), (float)Math.Sin(angle)));
    }

    /// <summary>
    /// 判断两条线段方向是否接近平行。
    /// </summary>
    /// <param name="firstStart">第一条线段的起点。</param>
    /// <param name="firstEnd">第一条线段的终点。</param>
    /// <param name="secondStart">第二条线段的起点。</param>
    /// <param name="secondEnd">第二条线段的终点。</param>
    /// <param name="toleranceDegrees">允许的角度误差，单位为度。</param>
    /// <returns>两条线段夹角小于等于容差时返回 true。</returns>
    private static bool AreSegmentsParallel(SKPoint firstStart, SKPoint firstEnd, SKPoint secondStart, SKPoint secondEnd, double toleranceDegrees)
    {
        // firstAngle/secondAngle：两条线段相对于 X 轴的方向角。
        double firstAngle = Math.Atan2(firstEnd.Y - firstStart.Y, firstEnd.X - firstStart.X);
        double secondAngle = Math.Atan2(secondEnd.Y - secondStart.Y, secondEnd.X - secondStart.X);

        // difference：把方向角差折算到 0 到 90 度附近，忽略正反方向。
        double difference = Math.Abs((firstAngle - secondAngle) * 180.0 / Math.PI) % 180.0;
        difference = Math.Min(difference, 180.0 - difference);
        return difference <= toleranceDegrees;
    }

    /// <summary>
    /// 计算三角形面积。
    /// </summary>
    /// <param name="corners">三个三角形角点。</param>
    /// <returns>三角形面积，单位为坐标平方。</returns>
    private static float GetTriangleArea(IReadOnlyList<SKPoint> corners)
    {
        return Math.Abs(
            corners[0].X * (corners[1].Y - corners[2].Y) +
            corners[1].X * (corners[2].Y - corners[0].Y) +
            corners[2].X * (corners[0].Y - corners[1].Y)) / 2f;
    }

    /// <summary>
    /// 计算三角形三个内角的角度值。
    /// </summary>
    /// <param name="corners">三个三角形角点。</param>
    /// <returns>三个内角，单位为度。</returns>
    private static double[] GetTriangleAngles(IReadOnlyList<SKPoint> corners)
    {
        return
        [
            AngleBetween(corners[2], corners[0], corners[1]) * 180.0 / Math.PI,
            AngleBetween(corners[0], corners[1], corners[2]) * 180.0 / Math.PI,
            AngleBetween(corners[1], corners[2], corners[0]) * 180.0 / Math.PI
        ];
    }

    /// <summary>
    /// 在模板识别不够可靠时，按自由多边形方式保留手绘轮廓。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>4 到 12 个角点组成的闭合多边形；无法稳定简化时返回空集合。</returns>
    private static List<SKPoint> BuildFreePolygonPoints(IReadOnlyList<SKPoint> points)
    {
        // 自由多边形只接收 7~12 个自然角点。
        // 3~6 个角点分别交给三角形、四边形、五边形、六边形处理。
        var corners = GetNaturalPolygonCorners(points);
        if (corners.Count < MinimumFreePolygonCorners || corners.Count > MaximumNaturalPolygonCorners)
        {
            return new List<SKPoint>();
        }

        if (!IsAcceptableFreePolygon(points, corners))
        {
            return new List<SKPoint>();
        }

        return BuildClosedPoints(corners);
    }

    /// <summary>
    /// 在 $1 已经粗分为 caret 的情况下，宽松生成自由多边形显示点，避免识别成功后无点可替换。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>闭合多边形坐标；仍无法简化时返回空集合。</returns>
    private static List<SKPoint> BuildRelaxedFreePolygonPoints(IReadOnlyList<SKPoint> points)
    {
        var contour = CreateOpenClosedContour(points);
        if (contour.Count < 3)
        {
            return new List<SKPoint>();
        }

        var bounds = GetBounds(contour);
        float diagonal = Math.Max(bounds.Diagonal, 1f);
        float tolerance = Math.Max(diagonal * 0.025f, 2f);
        float maximumTolerance = Math.Max(diagonal * 0.20f, tolerance);
        List<SKPoint> best = new();

        while (tolerance <= maximumTolerance)
        {
            var simplified = SimplifyClosedContourNaturally(contour, tolerance, diagonal);
            if (simplified.Count >= 3 && simplified.Count <= MaximumNaturalPolygonCorners && !HasSelfIntersection(simplified))
            {
                best = simplified;
                if (simplified.Count >= MinimumFreePolygonCorners)
                {
                    break;
                }
            }

            tolerance *= 1.25f;
        }

        if (best.Count < 3)
        {
            return new List<SKPoint>();
        }
        return BuildClosedPoints(best);
    }

    /// <summary>
    /// 根据笔迹点云生成保留旋转角度的圆或椭圆坐标。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>由固定采样点组成的闭合圆/椭圆坐标集合。</returns>
    private static List<SKPoint> BuildEllipsePoints(IReadOnlyList<SKPoint> points)
    {
        // 按主轴生成旋转圆/椭圆点集，避免斜着画的椭圆被转换成水平椭圆。
        // center：点云中心，作为圆/椭圆中心。
        var center = GetCentroid(points);

        // axisX：点云主轴方向，作为椭圆长轴或短轴之一。
        var axisX = GetPrincipalAxis(points);

        // axisY：与主轴垂直的副轴方向。
        var axisY = new SKPoint(-axisX.Y, axisX.X);

        // radiusX/radiusY：点云在主轴和副轴方向上的最大投影半径。
        float radiusX = 1f;
        float radiusY = 1f;

        foreach (var point in points)
        {
            // relative：当前点相对中心点的向量。
            var relative = new SKPoint(point.X - center.X, point.Y - center.Y);
            radiusX = Math.Max(radiusX, Math.Abs(Dot(relative, axisX)));
            radiusY = Math.Max(radiusY, Math.Abs(Dot(relative, axisY)));
        }

        // result：规则化后的椭圆采样点集合，最后一个点会回到起点。
        var result = new List<SKPoint>(NumPoints + 1);
        for (int i = 0; i <= NumPoints; i++)
        {
            // angle：当前采样点在椭圆参数方程中的角度。
            double angle = Math.PI * 2.0 * i / NumPoints;

            // x/y：当前采样点在局部椭圆坐标系中的坐标。
            float x = radiusX * (float)Math.Cos(angle);
            float y = radiusY * (float)Math.Sin(angle);
            result.Add(AddScaled(center, axisX, x, axisY, y));
        }

        return result;
    }

    /// <summary>
    /// 根据外接矩形生成水平闭合矩形坐标，作为矩形角点提取失败时的兜底。
    /// </summary>
    /// <param name="points">原始笔迹点集合。</param>
    /// <returns>外接矩形的闭合坐标集合。</returns>
    private static List<SKPoint> BuildBoundingRectanglePoints(IReadOnlyList<SKPoint> points)
    {
        // bounds：笔迹的水平外接范围。
        var bounds = GetBounds(points);
        return
        [
            new SKPoint(bounds.Left, bounds.Top),
            new SKPoint(bounds.Right, bounds.Top),
            new SKPoint(bounds.Right, bounds.Bottom),
            new SKPoint(bounds.Left, bounds.Bottom),
            new SKPoint(bounds.Left, bounds.Top)
        ];
    }

    /// <summary>
    /// 根据原始轮廓生成保留朝向的三角形，作为三角形角点提取失败时的兜底。
    /// </summary>
    /// <param name="points">原始笔迹点集合。</param>
    /// <returns>外接范围内的闭合三角形坐标集合。</returns>
    private static List<SKPoint> BuildFallbackTrianglePoints(IReadOnlyList<SKPoint> points)
    {
        // 优先从原始闭合轮廓强制简化出 3 个主要角点，这样倒三角、斜三角不会被翻成固定朝上的三角形。
        var contour = CreateOpenClosedContour(points);
        var simplified = SimplifyClosedPolygon(contour, 3);
        if (simplified.Count == 3)
        {
            return BuildClosedPoints(simplified);
        }

        // 极端情况下简化失败时才使用外接范围兜底。
        var bounds = GetBounds(points);
        bool pointsMostlyInBottomHalf = points.Count(point => point.Y > bounds.Top + bounds.Height * 0.5f) > points.Count / 2;
        if (!pointsMostlyInBottomHalf)
        {
            return
            [
                new SKPoint(bounds.Left, bounds.Top),
                new SKPoint(bounds.Right, bounds.Top),
                new SKPoint(bounds.Left + bounds.Width / 2f, bounds.Bottom),
                new SKPoint(bounds.Left, bounds.Top)
            ];
        }

        return
        [
            new SKPoint(bounds.Left + bounds.Width / 2f, bounds.Top),
            new SKPoint(bounds.Right, bounds.Bottom),
            new SKPoint(bounds.Left, bounds.Bottom),
            new SKPoint(bounds.Left + bounds.Width / 2f, bounds.Top)
        ];
    }

    /// <summary>
    /// 提取手绘闭合轮廓的自然角点数量。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>未重复闭合的角点集合。</returns>
    private static List<SKPoint> GetNaturalPolygonCorners(IReadOnlyList<SKPoint> points)
    {
        if (points.Count < 3)
        {
            return new List<SKPoint>();
        }

        var polygon = CreateOpenClosedContour(points);
        if (polygon.Count < 3)
        {
            return new List<SKPoint>();
        }

        var bounds = GetBounds(polygon);
        if (bounds.Diagonal < 12f)
        {
            return new List<SKPoint>();
        }

        float tolerance = Math.Max(bounds.Diagonal * 0.03f, 2f);
        float maximumTolerance = Math.Max(bounds.Diagonal * 0.12f, tolerance);
        List<SKPoint> corners = new();

        while (tolerance <= maximumTolerance)
        {
            corners = SimplifyClosedContourNaturally(polygon, tolerance, bounds.Diagonal);
            if (corners.Count <= MaximumNaturalPolygonCorners)
            {
                break;
            }

            tolerance *= 1.25f;
        }

        if (corners.Count < 3 || corners.Count > MaximumNaturalPolygonCorners)
        {
            return new List<SKPoint>();
        }

        return corners;
    }

    /// <summary>
    /// 将闭合笔迹转换为不重复首尾点的轮廓。
    /// </summary>
    /// <param name="points">原始闭合或近似闭合点集合。</param>
    /// <returns>去掉重复闭合点后的轮廓点集合。</returns>
    private static List<SKPoint> CreateOpenClosedContour(IReadOnlyList<SKPoint> points)
    {
        var polygon = new List<SKPoint>(points);
        if (polygon.Count < 2)
        {
            return polygon;
        }

        var bounds = GetBounds(polygon);
        float closeDistance = Math.Max(bounds.Diagonal * 0.12f, 6f);
        if (Distance(polygon[0], polygon[^1]) <= closeDistance)
        {
            polygon.RemoveAt(polygon.Count - 1);
        }

        return polygon;
    }

    /// <summary>
    /// 使用 RDP 简化并清理闭合轮廓中的自然角点。
    /// </summary>
    /// <param name="polygon">不重复首尾点的闭合轮廓。</param>
    /// <param name="tolerance">RDP 简化容差。</param>
    /// <param name="diagonal">图形外接范围对角线。</param>
    /// <returns>清理后的自然角点集合。</returns>
    private static List<SKPoint> SimplifyClosedContourNaturally(
        IReadOnlyList<SKPoint> polygon,
        float tolerance,
        float diagonal)
    {
        var closed = new List<SKPoint>(polygon.Count + 1);
        closed.AddRange(polygon);
        closed.Add(polygon[0]);

        var simplified = RamerDouglasPeucker(closed, tolerance);
        if (simplified.Count > 1 && Distance(simplified[0], simplified[^1]) <= Math.Max(tolerance, 1f))
        {
            simplified.RemoveAt(simplified.Count - 1);
        }

        simplified = MergeCloseCorners(simplified, Math.Max(diagonal * 0.035f, 4f));
        simplified = RemoveNearlyStraightCorners(simplified, diagonal);
        return simplified;
    }

    /// <summary>
    /// 合并距离过近的相邻角点。
    /// </summary>
    /// <param name="corners">候选角点集合。</param>
    /// <param name="threshold">合并距离阈值。</param>
    /// <returns>合并后的角点集合。</returns>
    private static List<SKPoint> MergeCloseCorners(List<SKPoint> corners, float threshold)
    {
        if (corners.Count <= 1)
        {
            return corners;
        }

        bool changed = true;
        while (changed && corners.Count > 3)
        {
            changed = false;
            for (int i = 0; i < corners.Count; i++)
            {
                int nextIndex = (i + 1) % corners.Count;
                if (Distance(corners[i], corners[nextIndex]) <= threshold)
                {
                    var merged = new SKPoint(
                        (corners[i].X + corners[nextIndex].X) * 0.5f,
                        (corners[i].Y + corners[nextIndex].Y) * 0.5f);
                    corners[i] = merged;
                    corners.RemoveAt(nextIndex);
                    changed = true;
                    break;
                }
            }
        }

        return corners;
    }

    /// <summary>
    /// 删除接近直线的弱角点。
    /// </summary>
    /// <param name="corners">候选角点集合。</param>
    /// <param name="diagonal">图形外接范围对角线，用于计算距离阈值。</param>
    /// <returns>删除弱角点后的角点集合。</returns>
    private static List<SKPoint> RemoveNearlyStraightCorners(List<SKPoint> corners, float diagonal)
    {
        if (corners.Count <= 3)
        {
            return corners;
        }

        bool changed = true;
        while (changed && corners.Count > 3)
        {
            changed = false;
            for (int i = 0; i < corners.Count; i++)
            {
                var previous = corners[(i - 1 + corners.Count) % corners.Count];
                var current = corners[i];
                var next = corners[(i + 1) % corners.Count];
                double angleDegrees = AngleBetween(previous, current, next) * 180.0 / Math.PI;
                double lineDistance = DistanceToSegment(current, previous, next);

                if (angleDegrees >= 166.0 || lineDistance <= Math.Max(diagonal * 0.012f, 2f))
                {
                    corners.RemoveAt(i);
                    changed = true;
                    break;
                }
            }
        }

        return corners;
    }

    /// <summary>
    /// 判断指定角点数量的多边形是否足够可靠。
    /// </summary>
    /// <param name="strokePoints">原始笔迹点集合。</param>
    /// <param name="corners">候选多边形角点集合。</param>
    /// <param name="cornerCount">期望角点数量。</param>
    /// <returns>边长、面积、角度和贴合误差都合理时返回 true。</returns>
    private static bool IsAcceptablePolygon(IReadOnlyList<SKPoint> strokePoints, IReadOnlyList<SKPoint> corners, int cornerCount)
    {
        if (corners.Count != cornerCount || cornerCount < 3)
        {
            return false;
        }

        var bounds = GetBounds(strokePoints);
        float diagonal = Math.Max(bounds.Diagonal, 1f);
        float minimumEdge = Math.Max(diagonal * GetMinimumEdgeRatio(cornerCount), 6f);
        if (HasShortEdge(corners, minimumEdge))
        {
            return false;
        }

        if (HasSelfIntersection(corners))
        {
            return false;
        }

        float polygonArea = Math.Abs(GetPolygonArea(corners));
        float minimumArea = Math.Max(bounds.Width * bounds.Height * 0.06f, diagonal * diagonal * 0.006f);
        if (polygonArea < minimumArea)
        {
            return false;
        }

        if (!HasReasonableCornerAngles(corners, cornerCount))
        {
            return false;
        }

        var shapePoints = BuildClosedPoints(corners);
        var error = CalculatePolylineError(strokePoints, shapePoints, diagonal);
        double averageLimit = cornerCount <= 4 ? 0.045 : 0.05;
        double maximumLimit = cornerCount <= 4 ? 0.16 : 0.18;
        return error.Average <= averageLimit && error.Maximum <= maximumLimit;
    }

    /// <summary>
    /// 判断自由多边形角点集合是否可靠。
    /// </summary>
    /// <param name="strokePoints">原始笔迹点集合。</param>
    /// <param name="corners">候选自由多边形角点集合。</param>
    /// <returns>角点数量、边长、角度、自交和贴合误差都合理时返回 true。</returns>
    private static bool IsAcceptableFreePolygon(IReadOnlyList<SKPoint> strokePoints, IReadOnlyList<SKPoint> corners)
    {
        if (corners.Count < MinimumFreePolygonCorners || corners.Count > MaximumNaturalPolygonCorners)
        {
            return false;
        }

        var bounds = GetBounds(strokePoints);
        float diagonal = Math.Max(bounds.Diagonal, 1f);
        if (HasShortEdge(corners, Math.Max(diagonal * 0.035f, 5f)) || HasSelfIntersection(corners))
        {
            return false;
        }

        if (!HasReasonableCornerAngles(corners, corners.Count))
        {
            return false;
        }

        var error = CalculatePolylineError(strokePoints, BuildClosedPoints(corners), diagonal);
        return error.Average <= 0.055 && error.Maximum <= 0.20;
    }

    /// <summary>
    /// 根据角点数量获取最小边长比例。
    /// </summary>
    /// <param name="cornerCount">多边形角点数量。</param>
    /// <returns>相对于外接对角线的最小边长比例。</returns>
    private static float GetMinimumEdgeRatio(int cornerCount)
    {
        return cornerCount switch
        {
            3 => 0.06f,
            4 => 0.055f,
            5 => 0.045f,
            6 => 0.04f,
            _ => 0.035f
        };
    }

    /// <summary>
    /// 判断多边形所有角度是否处于合理范围。
    /// </summary>
    /// <param name="corners">按相邻顺序排列的角点集合。</param>
    /// <param name="cornerCount">角点数量。</param>
    /// <returns>所有角度均在允许范围内时返回 true。</returns>
    private static bool HasReasonableCornerAngles(IReadOnlyList<SKPoint> corners, int cornerCount)
    {
        double minimumAngle = cornerCount <= 3 ? 15.0 : 18.0;
        double maximumAngle = cornerCount <= 6 ? 172.0 : 176.0;

        for (int i = 0; i < corners.Count; i++)
        {
            var previous = corners[(i - 1 + corners.Count) % corners.Count];
            var current = corners[i];
            var next = corners[(i + 1) % corners.Count];
            double angleDegrees = AngleBetween(previous, current, next) * 180.0 / Math.PI;
            if (angleDegrees < minimumAngle || angleDegrees > maximumAngle)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 计算多边形有向面积。
    /// </summary>
    /// <param name="corners">按相邻顺序排列的角点集合。</param>
    /// <returns>多边形有向面积。</returns>
    private static float GetPolygonArea(IReadOnlyList<SKPoint> corners)
    {
        double sum = 0;
        for (int i = 0; i < corners.Count; i++)
        {
            var current = corners[i];
            var next = corners[(i + 1) % corners.Count];
            sum += current.X * next.Y - next.X * current.Y;
        }

        return (float)(sum * 0.5);
    }

    /// <summary>
    /// 判断多边形是否存在自交。
    /// </summary>
    /// <param name="corners">按相邻顺序排列的角点集合。</param>
    /// <returns>任意非相邻边相交时返回 true。</returns>
    private static bool HasSelfIntersection(IReadOnlyList<SKPoint> corners)
    {
        for (int i = 0; i < corners.Count; i++)
        {
            var a1 = corners[i];
            var a2 = corners[(i + 1) % corners.Count];
            for (int j = i + 1; j < corners.Count; j++)
            {
                if (Math.Abs(i - j) <= 1 || (i == 0 && j == corners.Count - 1))
                {
                    continue;
                }

                var b1 = corners[j];
                var b2 = corners[(j + 1) % corners.Count];
                if (SegmentsIntersect(a1, a2, b1, b2))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 判断两条线段是否相交。
    /// </summary>
    /// <param name="a1">第一条线段起点。</param>
    /// <param name="a2">第一条线段终点。</param>
    /// <param name="b1">第二条线段起点。</param>
    /// <param name="b2">第二条线段终点。</param>
    /// <returns>两条线段严格相交时返回 true。</returns>
    private static bool SegmentsIntersect(SKPoint a1, SKPoint a2, SKPoint b1, SKPoint b2)
    {
        double d1 = Cross(a1, a2, b1);
        double d2 = Cross(a1, a2, b2);
        double d3 = Cross(b1, b2, a1);
        double d4 = Cross(b1, b2, a2);
        return d1 * d2 < 0 && d3 * d4 < 0;
    }

    /// <summary>
    /// 计算点相对有向线段的叉积。
    /// </summary>
    /// <param name="start">线段起点。</param>
    /// <param name="end">线段终点。</param>
    /// <param name="point">待测点。</param>
    /// <returns>二维叉积值。</returns>
    private static double Cross(SKPoint start, SKPoint end, SKPoint point)
    {
        return (end.X - start.X) * (point.Y - start.Y) -
               (end.Y - start.Y) * (point.X - start.X);
    }

    /// <summary>
    /// 将闭合轮廓简化为指定数量的角点。
    /// </summary>
    /// <param name="points">去掉重复闭合点后的轮廓点集合。</param>
    /// <param name="targetCount">目标角点数量。</param>
    /// <returns>目标数量的角点集合；无法稳定简化时返回空集合。</returns>
    private static List<SKPoint> SimplifyClosedPolygon(IReadOnlyList<SKPoint> points, int targetCount)
    {
        // RDP 简化后若角点仍过多，逐步删除转角贡献最小的点，直到目标角点数。
        if (points.Count <= targetCount)
        {
            return new List<SKPoint>(points);
        }

        // bounds：轮廓外接范围，用于让简化容差跟随图形大小变化。
        var bounds = GetBounds(points);

        // tolerance：RDP 初始简化容差。
        float tolerance = Math.Max(bounds.Diagonal * 0.03f, 2f);

        // simplified：RDP 简化后的候选角点集合。
        var simplified = RamerDouglasPeucker(points, tolerance);

        if (simplified.Count > 1 && Distance(simplified[0], simplified[^1]) < tolerance)
        {
            simplified.RemoveAt(simplified.Count - 1);
        }

        while (simplified.Count > targetCount)
        {
            // removeIndex：转角变化最小的角点索引，优先删除它以保留主要形状。
            int removeIndex = FindLeastImportantCorner(simplified);
            simplified.RemoveAt(removeIndex);
        }

        return simplified.Count == targetCount ? simplified : new List<SKPoint>();
    }

    /// <summary>
    /// 在多边形角点中查找对整体形状贡献最小的点。
    /// </summary>
    /// <param name="points">按相邻顺序排列的多边形角点。</param>
    /// <returns>应优先删除的角点索引。</returns>
    private static int FindLeastImportantCorner(IReadOnlyList<SKPoint> points)
    {
        // result：当前找到的最不重要角点索引。
        int result = 0;

        // smallestTurn：当前最小的转角变化量，越小越接近直线。
        double smallestTurn = double.MaxValue;

        for (int i = 0; i < points.Count; i++)
        {
            // previous/current/next：用于计算 current 这个角点转角的相邻点。
            var previous = points[(i - 1 + points.Count) % points.Count];
            var current = points[i];
            var next = points[(i + 1) % points.Count];

            // turn：当前点相对直线的弯折程度，越小越不重要。
            double turn = Math.Abs(Math.PI - AngleBetween(previous, current, next));

            if (turn < smallestTurn)
            {
                smallestTurn = turn;
                result = i;
            }
        }

        return result;
    }

    /// <summary>
    /// 使用 Ramer-Douglas-Peucker 算法简化折线。
    /// </summary>
    /// <param name="points">需要简化的折线点集合。</param>
    /// <param name="tolerance">点到线段的最大允许偏差。</param>
    /// <returns>简化后的折线点集合。</returns>
    private static List<SKPoint> RamerDouglasPeucker(IReadOnlyList<SKPoint> points, float tolerance)
    {
        if (points.Count < 3)
        {
            return new List<SKPoint>(points);
        }

        // maxDistance：当前折线中距离首尾连线最远的点距离。
        double maxDistance = 0;

        // index：距离首尾连线最远的点索引，用于递归切分折线。
        int index = 0;
        for (int i = 1; i < points.Count - 1; i++)
        {
            // distance：当前点到首尾连线的距离。
            double distance = DistanceToSegment(points[i], points[0], points[^1]);
            if (distance > maxDistance)
            {
                index = i;
                maxDistance = distance;
            }
        }

        if (maxDistance <= tolerance)
        {
            return [points[0], points[^1]];
        }

        // first/second：以最远点为切分点递归简化得到的两段折线。
        var first = RamerDouglasPeucker(points.Take(index + 1).ToList(), tolerance);
        var second = RamerDouglasPeucker(points.Skip(index).ToList(), tolerance);
        first.RemoveAt(first.Count - 1);
        first.AddRange(second);
        return first;
    }

    /// <summary>
    /// 对笔迹执行 $1 算法要求的标准化处理。
    /// </summary>
    /// <param name="points">去重后的原始笔迹点集合。</param>
    /// <returns>统一采样数、方向、尺寸和中心点后的点集合。</returns>
    private static List<SKPoint> Normalize(IReadOnlyList<SKPoint> points)
    {
        // $1 标准化流程：等距采样、按起点方向旋转、缩放到统一大小、平移到原点。
        // resampled：等距重采样后的点集合。
        var resampled = Resample(points, NumPoints);

        // radians：从点云中心指向起点的指示角。
        double radians = IndicativeAngle(resampled);

        // rotated：把指示角旋转到标准方向后的点集合。
        var rotated = RotateBy(resampled, -radians);

        // scaled：缩放到统一模板方形尺寸后的点集合。
        var scaled = ScaleToSquare(rotated, SquareSize);
        return TranslateCentroidToOrigin(scaled);
    }

    /// <summary>
    /// 对闭合图形执行用于模板匹配的标准化处理。
    /// </summary>
    /// <param name="points">原始闭合图形点集合。</param>
    /// <returns>重采样、缩放并平移到原点后的点集合。</returns>
    private static List<SKPoint> NormalizeClosedForMatching(IReadOnlyList<SKPoint> points)
    {
        var resampled = Resample(points, NumPoints);
        var scaled = ScaleToSquare(resampled, SquareSize);
        return TranslateCentroidToOrigin(scaled);
    }

    /// <summary>
    /// 按路径长度等距重采样点集合。
    /// </summary>
    /// <param name="points">原始点集合。</param>
    /// <param name="targetCount">目标采样点数量。</param>
    /// <returns>采样点数量固定的点集合。</returns>
    private static List<SKPoint> Resample(IReadOnlyList<SKPoint> points, int targetCount)
    {
        // interval：相邻采样点之间的目标路径长度。
        float interval = PathLength(points) / (targetCount - 1);

        // distanceSoFar：从上一个采样点开始累计的路径长度。
        float distanceSoFar = 0;

        // source：可插入新采样点的源点集合。
        var source = new List<SKPoint>(points);

        // result：重采样后的固定数量点集合。
        var result = new List<SKPoint>(targetCount) { source[0] };

        for (int i = 1; i < source.Count; i++)
        {
            // distance：当前相邻两个源点之间的距离。
            float distance = Distance(source[i - 1], source[i]);
            if (distance <= 0)
            {
                continue;
            }

            if (distanceSoFar + distance >= interval)
            {
                // ratio：新采样点落在当前线段上的插值比例。
                float ratio = (interval - distanceSoFar) / distance;

                // point：按 interval 插值得到的新采样点。
                var point = new SKPoint(
                    source[i - 1].X + ratio * (source[i].X - source[i - 1].X),
                    source[i - 1].Y + ratio * (source[i].Y - source[i - 1].Y));

                result.Add(point);
                source.Insert(i, point);
                distanceSoFar = 0;
            }
            else
            {
                distanceSoFar += distance;
            }
        }

        while (result.Count < targetCount)
        {
            result.Add(source[^1]);
        }

        return result;
    }

    /// <summary>
    /// 使用黄金分割搜索寻找候选笔迹和模板之间的最佳旋转匹配距离。
    /// </summary>
    /// <param name="points">已经标准化的候选笔迹点集合。</param>
    /// <param name="template">已经标准化的模板点集合。</param>
    /// <param name="lower">搜索角度下界，单位为弧度。</param>
    /// <param name="upper">搜索角度上界，单位为弧度。</param>
    /// <param name="threshold">搜索停止阈值，单位为弧度。</param>
    /// <returns>最佳旋转角度下的平均路径距离。</returns>
    private static double GoldenSectionSearch(IReadOnlyList<SKPoint> points, IReadOnlyList<SKPoint> template, double lower, double upper, double threshold)
    {
        // 【$1 旋转匹配】
        // 同一个图形可能被用户斜着画，所以这里在 lower~upper 的角度范围内旋转候选轨迹，
        // 找到与模板平均距离最小的角度。
        // 在限定角度范围内寻找候选笔迹与模板的最小路径距离。
        // x1/x2：黄金分割搜索在当前区间内的两个候选角度。
        double x1 = Phi * lower + (1 - Phi) * upper;

        // f1/f2：两个候选角度下，候选笔迹和模板的路径距离。
        double f1 = PathDistance(RotateBy(points, x1), template);
        double x2 = (1 - Phi) * lower + Phi * upper;
        double f2 = PathDistance(RotateBy(points, x2), template);

        while (Math.Abs(upper - lower) > threshold)
        {
            if (f1 < f2)
            {
                upper = x2;
                x2 = x1;
                f2 = f1;
                x1 = Phi * lower + (1 - Phi) * upper;
                f1 = PathDistance(RotateBy(points, x1), template);
            }
            else
            {
                lower = x1;
                x1 = x2;
                f1 = f2;
                x2 = (1 - Phi) * lower + Phi * upper;
                f2 = PathDistance(RotateBy(points, x2), template);
            }
        }

        return Math.Min(f1, f2);
    }

    /// <summary>
    /// 计算两个同序点集合之间的平均路径距离。
    /// </summary>
    /// <param name="first">第一个点集合。</param>
    /// <param name="second">第二个点集合。</param>
    /// <returns>逐点距离的平均值。</returns>
    private static double PathDistance(IReadOnlyList<SKPoint> first, IReadOnlyList<SKPoint> second)
    {
        // 【$1 逐点距离】
        // 两条轨迹已经重采样成相同数量的点后，按相同下标逐点计算欧氏距离并取平均值。
        // count：参与比较的点数量，取两个集合的较小长度避免越界。
        int count = Math.Min(first.Count, second.Count);

        // distance：逐点距离累计值。
        double distance = 0;

        for (int i = 0; i < count; i++)
        {
            distance += Distance(first[i], second[i]);
        }

        return distance / count;
    }

    /// <summary>
    /// 计算闭合图形的起点无关路径距离。
    /// </summary>
    /// <param name="first">第一个标准化点集合。</param>
    /// <param name="second">第二个标准化点集合。</param>
    /// <returns>枚举起点和方向后的最小平均距离。</returns>
    private static double StartIndependentPathDistance(IReadOnlyList<SKPoint> first, IReadOnlyList<SKPoint> second)
    {
        // 【$1 闭合图形起点无关距离】
        // 闭合图形可能从任意角/任意圆周位置开始画，所以这里枚举模板起点偏移，
        // 同时比较正向和反向，取最小平均距离。
        int count = Math.Min(first.Count, second.Count);
        if (count == 0)
        {
            return double.MaxValue;
        }

        double bestDistance = double.MaxValue;
        for (int shift = 0; shift < count; shift++)
        {
            double forward = 0;
            double reverse = 0;
            for (int i = 0; i < count; i++)
            {
                forward += Distance(first[i], second[(i + shift) % count]);
                reverse += Distance(first[i], second[(shift - i + count) % count]);
            }

            bestDistance = Math.Min(bestDistance, Math.Min(forward, reverse) / count);
        }

        return bestDistance;
    }

    /// <summary>
    /// 围绕点云中心旋转整个点集合。
    /// </summary>
    /// <param name="points">待旋转的点集合。</param>
    /// <param name="radians">旋转角度，单位为弧度。</param>
    /// <returns>旋转后的点集合。</returns>
    private static List<SKPoint> RotateBy(IReadOnlyList<SKPoint> points, double radians)
    {
        // centroid：旋转中心，使用点云中心而不是坐标原点。
        var centroid = GetCentroid(points);

        // cos/sin：旋转矩阵需要的三角函数值。
        double cos = Math.Cos(radians);
        double sin = Math.Sin(radians);

        // result：旋转后的点集合。
        var result = new List<SKPoint>(points.Count);

        foreach (var point in points)
        {
            // dx/dy：当前点相对于旋转中心的坐标。
            float dx = point.X - centroid.X;
            float dy = point.Y - centroid.Y;
            result.Add(new SKPoint(
                (float)(dx * cos - dy * sin + centroid.X),
                (float)(dx * sin + dy * cos + centroid.Y)));
        }

        return result;
    }

    /// <summary>
    /// 将点集合按外接矩形缩放到指定方形尺寸。
    /// </summary>
    /// <param name="points">待缩放的点集合。</param>
    /// <param name="size">目标方形边长。</param>
    /// <returns>缩放后的点集合。</returns>
    private static List<SKPoint> ScaleToSquare(IReadOnlyList<SKPoint> points, float size)
    {
        // bounds：点集合的外接范围，用于计算 X/Y 缩放比例。
        var bounds = GetBounds(points);

        // result：缩放后的点集合。
        var result = new List<SKPoint>(points.Count);

        foreach (var point in points)
        {
            result.Add(new SKPoint(
                bounds.Width == 0 ? point.X : point.X * (size / bounds.Width),
                bounds.Height == 0 ? point.Y : point.Y * (size / bounds.Height)));
        }

        return result;
    }

    /// <summary>
    /// 将点集合平移到以质心为原点的位置。
    /// </summary>
    /// <param name="points">待平移的点集合。</param>
    /// <returns>质心位于 (0, 0) 的点集合。</returns>
    private static List<SKPoint> TranslateCentroidToOrigin(IReadOnlyList<SKPoint> points)
    {
        // centroid：点集合质心，所有点都会减去该坐标。
        var centroid = GetCentroid(points);
        return points.Select(point => new SKPoint(point.X - centroid.X, point.Y - centroid.Y)).ToList();
    }

    /// <summary>
    /// 计算 $1 算法使用的指示角。
    /// </summary>
    /// <param name="points">候选笔迹点集合。</param>
    /// <returns>从质心指向第一个点的角度，单位为弧度。</returns>
    private static double IndicativeAngle(IReadOnlyList<SKPoint> points)
    {
        // centroid：点集合质心，用于确定起点方向。
        var centroid = GetCentroid(points);
        return Math.Atan2(points[0].Y - centroid.Y, points[0].X - centroid.X);
    }

    /// <summary>
    /// 计算点集合的几何中心。
    /// </summary>
    /// <param name="points">点集合。</param>
    /// <returns>所有点坐标的平均位置。</returns>
    private static SKPoint GetCentroid(IReadOnlyList<SKPoint> points)
    {
        // x/y：所有点的 X/Y 坐标累计值。
        float x = 0;
        float y = 0;

        foreach (var point in points)
        {
            x += point.X;
            y += point.Y;
        }

        return new SKPoint(x / points.Count, y / points.Count);
    }

    /// <summary>
    /// 计算点集合的水平外接范围。
    /// </summary>
    /// <param name="points">点集合。</param>
    /// <returns>包含所有点的外接矩形范围。</returns>
    private static Bounds GetBounds(IReadOnlyList<SKPoint> points)
    {
        // left/top/right/bottom：当前扫描到的外接矩形边界。
        float left = float.MaxValue;
        float top = float.MaxValue;
        float right = float.MinValue;
        float bottom = float.MinValue;

        foreach (var point in points)
        {
            left = Math.Min(left, point.X);
            top = Math.Min(top, point.Y);
            right = Math.Max(right, point.X);
            bottom = Math.Max(bottom, point.Y);
        }

        return new Bounds(left, top, right, bottom);
    }

    /// <summary>
    /// 计算折线的总路径长度。
    /// </summary>
    /// <param name="points">按绘制顺序排列的点集合。</param>
    /// <returns>相邻点距离之和。</returns>
    private static float PathLength(IReadOnlyList<SKPoint> points)
    {
        // length：累计的折线路径长度。
        float length = 0;
        for (int i = 1; i < points.Count; i++)
        {
            length += Distance(points[i - 1], points[i]);
        }

        return length;
    }

    /// <summary>
    /// 判断笔迹首尾是否足够接近，从而视为闭合图形。
    /// </summary>
    /// <param name="points">去重后的笔迹点集合。</param>
    /// <returns>首尾距离小于闭合阈值时返回 true。</returns>
    private static bool IsClosedShape(IReadOnlyList<SKPoint> points)
    {
        // 首尾距离足够近才认为是闭合图形；阈值随图形尺寸放大。
        // bounds：笔迹外接范围，用于根据图形大小调整闭合阈值。
        var bounds = GetBounds(points);

        // closeThreshold：闭合判断阈值，保证小图形也有最低 10 像素容差。
        float closeThreshold = Math.Max(bounds.Diagonal * 0.18f, 10f);
        return Distance(points[0], points[^1]) <= closeThreshold;
    }

    /// <summary>
    /// 去除连续距离过近的重复点。
    /// </summary>
    /// <param name="points">原始笔迹点集合。</param>
    /// <returns>去重后的点集合。</returns>
    private static List<SKPoint> RemoveDuplicatePoints(IReadOnlyList<SKPoint> points)
    {
        // result：保留的有效点集合，先放入第一个点作为基准。
        var result = new List<SKPoint>(points.Count) { points[0] };
        for (int i = 1; i < points.Count; i++)
        {
            if (Distance(result[^1], points[i]) > 0.5f)
            {
                result.Add(points[i]);
            }
        }

        return result;
    }

    /// <summary>
    /// 计算两个点之间的欧氏距离。
    /// </summary>
    /// <param name="first">第一个点。</param>
    /// <param name="second">第二个点。</param>
    /// <returns>两点之间的直线距离。</returns>
    private static float Distance(SKPoint first, SKPoint second)
    {
        // dx/dy：两点在 X/Y 方向上的差值。
        float dx = second.X - first.X;
        float dy = second.Y - first.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// 计算点到线段的最短距离。
    /// </summary>
    /// <param name="point">待测点。</param>
    /// <param name="start">线段起点。</param>
    /// <param name="end">线段终点。</param>
    /// <returns>点到线段的最短距离。</returns>
    private static double DistanceToSegment(SKPoint point, SKPoint start, SKPoint end)
    {
        // dx/dy：线段方向向量。
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        if (dx == 0 && dy == 0)
        {
            return Distance(point, start);
        }

        // t：点在线段方向上的投影比例，限制在 0 到 1 表示投影在线段内。
        float t = ((point.X - start.X) * dx + (point.Y - start.Y) * dy) / (dx * dx + dy * dy);
        t = Math.Clamp(t, 0, 1);

        // projection：待测点在线段上的最近投影点。
        var projection = new SKPoint(start.X + t * dx, start.Y + t * dy);
        return Distance(point, projection);
    }

    /// <summary>
    /// 计算 previous-current-next 三点形成的夹角。
    /// </summary>
    /// <param name="previous">角的一侧点。</param>
    /// <param name="current">角点。</param>
    /// <param name="next">角的另一侧点。</param>
    /// <returns>夹角大小，单位为弧度。</returns>
    private static double AngleBetween(SKPoint previous, SKPoint current, SKPoint next)
    {
        // first/second：以 current 为起点的两条边向量。
        var first = new SKPoint(previous.X - current.X, previous.Y - current.Y);
        var second = new SKPoint(next.X - current.X, next.Y - current.Y);

        // dot：两条边向量的点积。
        double dot = first.X * second.X + first.Y * second.Y;

        // firstLength/secondLength：两条边向量的长度。
        double firstLength = Math.Sqrt(first.X * first.X + first.Y * first.Y);
        double secondLength = Math.Sqrt(second.X * second.X + second.Y * second.Y);
        if (firstLength == 0 || secondLength == 0)
        {
            return 0;
        }

        return Math.Acos(Math.Clamp(dot / (firstLength * secondLength), -1.0, 1.0));
    }

    /// <summary>
    /// 构建所有用于 $1 匹配的标准图形模板。
    /// </summary>
    /// <returns>已经标准化的模板集合。</returns>
    private static List<Template> BuildTemplates()
    {
        // 模板包含多个起笔点和顺/逆时针方向，降低 $1 对画法顺序的敏感度。
        // templates：最终用于识别比较的模板列表。
        var templates = new List<Template>();

        // 【$1 triangle 粗模板】
        // CreateTemplateCandidate 会拿当前轨迹与这里生成的 Triangle 模板比较相似度。
        // 如果 Triangle 模板距离最小，后续会进入 ResolveTriangleType 做三角形细分。
        foreach (var points in BuildClosedShapeTemplates(BuildTriangleTemplate()))
        {
            templates.Add(new Template(DollarTemplateKind.Triangle, NormalizeClosedForMatching(points)));
        }

        // 【$1 rectangle 粗模板】
        // 这里的 Rectangle 是 $1 的四边形粗模板。
        // 命中后不一定最终就是矩形，还会进入 ResolveQuadrilateralType 判断矩形/正方形/菱形/梯形/平行四边形。
        foreach (var points in BuildClosedShapeTemplates(BuildRectangleTemplate()))
        {
            templates.Add(new Template(DollarTemplateKind.Rectangle, NormalizeClosedForMatching(points)));
        }

        // 【$1 caret 粗模板】
        // 五边形、六边形和更多角的闭合多边形都先归到 caret 粗分类，
        // 后续再由 ResolveCaretType 细分为五边形、六边形或自由多边形。
        foreach (var points in BuildClosedShapeTemplates(BuildRegularPolygonTemplate(5)))
        {
            templates.Add(new Template(DollarTemplateKind.Caret, NormalizeClosedForMatching(points)));
        }

        foreach (var points in BuildClosedShapeTemplates(BuildRegularPolygonTemplate(6)))
        {
            templates.Add(new Template(DollarTemplateKind.Caret, NormalizeClosedForMatching(points)));
        }

        foreach (var points in BuildClosedShapeTemplates(BuildRegularPolygonTemplate(7)))
        {
            templates.Add(new Template(DollarTemplateKind.Caret, NormalizeClosedForMatching(points)));
        }

        foreach (var points in BuildClosedShapeTemplates(BuildRegularPolygonTemplate(8)))
        {
            templates.Add(new Template(DollarTemplateKind.Caret, NormalizeClosedForMatching(points)));
        }

        // 【$1 circle 粗模板】
        // 命中 Circle 模板后，还会在 ResolveShapeType 中按长短轴比例区分圆形和椭圆形。
        foreach (var points in BuildCircleTemplates())
        {
            templates.Add(new Template(DollarTemplateKind.Circle, NormalizeClosedForMatching(points)));
        }

        return templates;
    }

    /// <summary>
    /// 为闭合多边形模板生成不同起点和方向的变体。
    /// </summary>
    /// <param name="closedPoints">首尾闭合的标准多边形模板点。</param>
    /// <returns>多个起点和顺/逆时针方向的模板点集合。</returns>
    private static IEnumerable<List<SKPoint>> BuildClosedShapeTemplates(IReadOnlyList<SKPoint> closedPoints)
    {
        // 闭合多边形模板：枚举每个角作为起点，并同时生成反向路径。
        // corners：去掉最后一个重复闭合点后的角点集合。
        var corners = closedPoints.Take(closedPoints.Count - 1).ToList();

        for (int startIndex = 0; startIndex < corners.Count; startIndex++)
        {
            yield return RotateClosedTemplate(corners, startIndex, false);
            yield return RotateClosedTemplate(corners, startIndex, true);
        }
    }

    /// <summary>
    /// 重新排列闭合模板的起点，并可选反转绘制方向。
    /// </summary>
    /// <param name="corners">未重复闭合的模板角点集合。</param>
    /// <param name="startIndex">新的起点索引。</param>
    /// <param name="reverse">是否反转模板方向。</param>
    /// <returns>重新排列并闭合后的模板点集合。</returns>
    private static List<SKPoint> RotateClosedTemplate(IReadOnlyList<SKPoint> corners, int startIndex, bool reverse)
    {
        // ordered：根据 reverse 决定顺时针或逆时针顺序后的角点。
        var ordered = reverse ? corners.AsEnumerable().Reverse().ToList() : corners.ToList();

        // result：重新选择起点并补上闭合点后的模板。
        var result = new List<SKPoint>(ordered.Count + 1);

        for (int i = 0; i < ordered.Count; i++)
        {
            result.Add(ordered[(startIndex + i) % ordered.Count]);
        }

        result.Add(result[0]);
        return result;
    }

    /// <summary>
    /// 生成圆形模板的多个起点和方向变体。
    /// </summary>
    /// <returns>用于匹配圆形/椭圆的模板点集合。</returns>
    private static IEnumerable<List<SKPoint>> BuildCircleTemplates()
    {
        // 圆形模板使用多个起点，支持从任意位置开始画圆。
        // points：首尾闭合的基础圆形模板。
        var points = BuildCircleTemplate();

        // variants：圆形模板起点变体数量。
        const int variants = 8;

        // step：每个起点变体在采样点中的偏移步长。
        int step = NumPoints / variants;

        for (int i = 0; i < variants; i++)
        {
            yield return RotateClosedTemplate(points.Take(points.Count - 1).ToList(), i * step, false);
            yield return RotateClosedTemplate(points.Take(points.Count - 1).ToList(), i * step, true);
        }
    }

    /// <summary>
    /// 构建基础三角形模板。
    /// </summary>
    /// <returns>首尾闭合的等腰三角形模板点集合。</returns>
    private static List<SKPoint> BuildTriangleTemplate()
    {
        // 【基础三角形模板坐标】
        // 这是 $1 用来比较的标准三角形轨迹，后续会生成不同起点和方向的变体。
        return
        [
            new SKPoint(SquareSize / 2f, 0),
            new SKPoint(SquareSize, SquareSize),
            new SKPoint(0, SquareSize),
            new SKPoint(SquareSize / 2f, 0)
        ];
    }

    /// <summary>
    /// 构建基础矩形模板。
    /// </summary>
    /// <returns>首尾闭合的正方形模板点集合，用于矩形/正方形粗识别。</returns>
    private static List<SKPoint> BuildRectangleTemplate()
    {
        // 【基础四边形模板坐标】
        // 这里用标准正方形作为 Rectangle 粗模板；最终具体类型由 ResolveQuadrilateralType 再判断。
        return
        [
            new SKPoint(0, 0),
            new SKPoint(SquareSize, 0),
            new SKPoint(SquareSize, SquareSize),
            new SKPoint(0, SquareSize),
            new SKPoint(0, 0)
        ];
    }

    /// <summary>
    /// 构建基础圆形模板。
    /// </summary>
    /// <returns>按固定采样点数生成的首尾闭合圆形模板。</returns>
    private static List<SKPoint> BuildCircleTemplate()
    {
        // 【基础圆形模板坐标】
        // 这里按圆周采样生成 $1 圆形模板；椭圆不单独建模板，而是在命中圆模板后再几何细分。
        // points：圆周采样点集合，容量包含最后一个闭合点。
        var points = new List<SKPoint>(NumPoints + 1);

        // radius：模板圆半径，使用标准方形尺寸的一半。
        float radius = SquareSize / 2f;
        for (int i = 0; i <= NumPoints; i++)
        {
            // angle：当前圆周采样点的参数角度。
            double angle = Math.PI * 2.0 * i / NumPoints;
            points.Add(new SKPoint(
                radius + radius * (float)Math.Cos(angle),
                radius + radius * (float)Math.Sin(angle)));
        }

        return points;
    }

    /// <summary>
    /// 构建基础正多边形模板。
    /// </summary>
    /// <param name="sideCount">多边形边数。</param>
    /// <returns>首尾闭合的正多边形模板点集合。</returns>
    private static List<SKPoint> BuildRegularPolygonTemplate(int sideCount)
    {
        // points：正多边形角点集合，容量包含最后一个闭合点。
        var points = new List<SKPoint>(sideCount + 1);

        // center：模板正方形中心坐标。
        float center = SquareSize / 2f;

        // radius：正多边形外接圆半径。
        float radius = SquareSize / 2f;

        // startAngle：起始角度，让第一个角点位于上方。
        double startAngle = -Math.PI / 2.0;

        for (int i = 0; i < sideCount; i++)
        {
            // angle：当前角点在外接圆上的角度。
            double angle = startAngle + Math.PI * 2.0 * i / sideCount;
            points.Add(new SKPoint(
                center + radius * (float)Math.Cos(angle),
                center + radius * (float)Math.Sin(angle)));
        }

        points.Add(points[0]);
        return points;
    }

    /// <summary>
    /// $1 模板阶段只输出的粗分类。
    /// </summary>
    private enum DollarTemplateKind
    {
        Triangle,
        Rectangle,
        Circle,
        Caret
    }

    /// <summary>
    /// $1 图形模板，保存模板所属粗分类和标准化后的点集合。
    /// </summary>
    /// <param name="Kind">模板对应的粗分类。</param>
    /// <param name="Points">模板标准化后的点集合。</param>
    private sealed record Template(DollarTemplateKind Kind, IReadOnlyList<SKPoint> Points);

    /// <summary>
    /// 点集合的水平外接范围。
    /// </summary>
    /// <param name="Left">外接范围左边界。</param>
    /// <param name="Top">外接范围上边界。</param>
    /// <param name="Right">外接范围右边界。</param>
    /// <param name="Bottom">外接范围下边界。</param>
    private readonly record struct Bounds(float Left, float Top, float Right, float Bottom)
    {
        /// <summary>
        /// 外接范围宽度。
        /// </summary>
        public float Width => Right - Left;

        /// <summary>
        /// 外接范围高度。
        /// </summary>
        public float Height => Bottom - Top;

        /// <summary>
        /// 外接范围对角线长度，用于按图形尺寸缩放各类阈值。
        /// </summary>
        public float Diagonal => MathF.Sqrt(Width * Width + Height * Height);
    }
}




