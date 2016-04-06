using System;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using DotSpatial.Symbology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// Helper methods related to <see cref="MapData"/> instancees.
    /// </summary>
    public static class MapDataHelper
    {
        /// <summary>
        /// Converts <see cref="PointSymbol"/> to <see cref="PointShape"/>.
        /// </summary>
        /// <param name="symbol">The symbol to convert.</param>
        /// <returns>The converted <see cref="PointShape"/>.</returns>
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