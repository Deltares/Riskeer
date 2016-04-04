using System.Drawing;
using System.Drawing.Drawing2D;

namespace Core.Components.Gis.Style
{
    public class LineStyle
    {
        public Color Color { get; private set; }
        public int Width { get; private set; }
        public DashStyle Style { get; private set; }

        public LineStyle(Color color, int width, DashStyle style)
        {
            Color = color;
            Width = width;
            Style = style;
        }
    }
}