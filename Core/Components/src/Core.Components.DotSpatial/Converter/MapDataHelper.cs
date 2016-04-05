using System;
using Core.Components.Gis.Style;
using DotSpatial.Symbology;

namespace Core.Components.DotSpatial.Converter
{
    public static class MapDataHelper
    {
        public static PointShape Convert(PointSymbol symbol)
        {
            PointShape shape;
            switch (symbol)
            {
                case PointSymbol.Circle:
                    shape = PointShape.Ellipse;
                    break;
                case PointSymbol.Square:
                    shape = PointShape.Rectangle;
                    break;
                case PointSymbol.Triangle:
                    shape = PointShape.Triangle;
                    break;
                default:
                    throw new NotSupportedException();
            }
            return shape;
        }
    }
}