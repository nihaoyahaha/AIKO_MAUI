using System.Collections.ObjectModel;

namespace Aiko.Common.InkTools
{
    public static class InkToolConfig
    {
        // 可被配置的工具
        public static bool IsConfigurable(string type)
        {
            return type != "Move" && type != "Empty";
        }

        public static bool IsStroke(string type)
        {
            return type == "BallpointPen"
                || type == "Pencil"
                || type == "Highlighter"
                || type == "Line"
                || type == "FixedRect"
                || type == "FixedCircle";
        }
        public static bool IsFont(string type)
        {
            return type == "Text"
                || type == "CircleText"
                || type == "RectText"
                || type == "Accept";
        }
    }
}