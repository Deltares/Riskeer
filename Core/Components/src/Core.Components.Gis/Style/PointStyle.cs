using System.Drawing;

namespace Core.Components.Gis.Style
{
    public class PointStyle {

        public Color Color { get; private set; }
        public double Size { get; private set; }
        public PointSymbol Symbol { get; private set; }

        public PointStyle(Color color, int size, PointSymbol symbol)
        {
            Color = color;
            Size = size;
            Symbol = symbol;
        }
    }
}