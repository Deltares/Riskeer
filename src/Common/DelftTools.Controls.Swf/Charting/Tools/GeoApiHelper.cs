using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public class GeoApiHelper
    {
        public static IGeometry GetBoundedSeriesPolygon(Steema.TeeChart.Styles.Series series, double leftXValue, double rightXValue, double bottomYValue, double topYValue)
        {
            var seriesCoordinates = new List<ICoordinate>();
            var startIndex = Math.Max(0, series.FirstVisibleIndex - 1);
            var endIndex = series.LastVisibleIndex;

            var xmin = int.MaxValue;
            var xmax = int.MinValue;
            var ymin = int.MaxValue;
            var ymax = int.MinValue;

            if (series.FirstVisibleIndex != -1)
            {
                for (var i = startIndex; i <= endIndex; i++)
                {
                    ICoordinate lastCoordinate = GeometryFactory.CreateCoordinate(series.CalcXPos(i), series.CalcYPos(i));
                    seriesCoordinates.Add(lastCoordinate);
                    xmin = (int) Math.Min(xmin, lastCoordinate.X);
                    xmax = (int) Math.Max(xmax, lastCoordinate.X);
                    ymin = (int) Math.Min(ymin, lastCoordinate.Y);
                    ymax = (int) Math.Max(ymax, lastCoordinate.Y);
                }
                ymin -= 20;
                seriesCoordinates.Add(GeometryFactory.CreateCoordinate(seriesCoordinates[seriesCoordinates.Count - 1].X,
                                                                       ymin));
                seriesCoordinates.Add(GeometryFactory.CreateCoordinate(seriesCoordinates[0].X,
                                                                       ymin));
                seriesCoordinates.Add(GeometryFactory.CreateCoordinate(seriesCoordinates[0].X,
                                                                       seriesCoordinates[0].Y));
            }
            var geomFactory = new GeometryFactory();
            var linearRing = geomFactory.CreateLinearRing(seriesCoordinates.ToArray());
            IPolygon seriesPolygon = geomFactory.CreatePolygon(linearRing, null);

            var coordinates = new List<ICoordinate>();

            var left = Math.Max(xmin, series.CalcXPosValue(leftXValue));
            var right = Math.Min(xmax, series.CalcXPosValue(rightXValue));
            var bottom = Math.Max(ymin, series.CalcYPosValue(bottomYValue));
            var top = Math.Min(ymax, series.CalcYPosValue(topYValue));
            //waterlevel line

            coordinates.Add(GeometryFactory.CreateCoordinate(left, top));
            coordinates.Add(GeometryFactory.CreateCoordinate(left, bottom));

            coordinates.Add(GeometryFactory.CreateCoordinate(left, bottom + 1));

            coordinates.Add(GeometryFactory.CreateCoordinate(right, bottom));
            coordinates.Add(GeometryFactory.CreateCoordinate(right, top));

            coordinates.Add(GeometryFactory.CreateCoordinate(left + (right - top)/2, top - 1));

            coordinates.Add(GeometryFactory.CreateCoordinate(left, top));
            linearRing = geomFactory.CreateLinearRing(coordinates.ToArray());
            IPolygon polygon = geomFactory.CreatePolygon(linearRing, null);

            if (seriesPolygon.IsEmpty)
            {
                return Polygon.Empty;
            }

            var result = polygon.Intersection(seriesPolygon);

            return result is IPolygon ? result : Polygon.Empty;
        }
    }
}