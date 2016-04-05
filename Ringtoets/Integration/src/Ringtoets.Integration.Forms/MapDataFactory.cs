using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// This factory is used to create <see cref="MapData"/> with default styling based on differnt
    /// types of data.
    /// </summary>
    public static class MapDataFactory
    {
        public static MapData Create(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException();
            }
            var features = GetMapFeature(referenceLine.Points);

            return new MapLineData(features, RingtoetsCommonDataResources.ReferenceLine_DisplayName)
            {
                Style = new LineStyle(Color.Red, 3, DashStyle.Solid)
            };
        }

        public static MapData Create(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            IEnumerable<Point2D> locations = hydraulicBoundaryDatabase.Locations.Select(h => h.Location).ToArray();

            var features = GetMapFeature(locations);

            return new MapPointData(features, RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName)
            {
                Style = new PointStyle(Color.DarkBlue, 6, PointSymbol.Circle)
            };
        }

        private static IEnumerable<MapFeature> GetMapFeature(IEnumerable<Point2D> points)
        {
            var features = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(points)
                })
            };
            return features;
        }

        public static MapData CreateEmptyLineData(string name)
        {
            return new MapLineData(Enumerable.Empty<MapFeature>(), name);
        }

        public static MapData CreateEmptyPointData(string name)
        {
            return new MapPointData(Enumerable.Empty<MapFeature>(), name);
        }
    }
}