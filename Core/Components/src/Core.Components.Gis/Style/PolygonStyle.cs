using System.Drawing;

namespace Core.Components.Gis.Style
{
    public class PolygonStyle
    {
        public Color FillColor { get; private set; }
        public Color StrokeColor { get; private set; }
        public int Width { get; private set; }

        public PolygonStyle(Color fillColor, Color strokeColor, int width)
        {
            FillColor = fillColor;
            StrokeColor = strokeColor;
            Width = width;
        }
    }
}