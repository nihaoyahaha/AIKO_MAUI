using SkiaSharp;

namespace Aiko.Common.InkTools
{
    public enum InkActionType { Add, Erase, Move }

    public class InkAction
    {
        public InkActionType Type { get; set; }

        public List<Guid> TargetIds { get; set; } = new List<Guid>();

        public SKPoint Offset { get; set; }
    }
}