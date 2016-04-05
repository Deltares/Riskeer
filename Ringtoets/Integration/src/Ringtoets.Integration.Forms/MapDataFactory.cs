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
        /// <summary>
        /// Create <see cref="MapData"/> with default styling based on the <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="referenceLine">The <see cref="ReferenceLine"/> for which to create <see cref="MapData"/>.</param>
        /// <returns><see cref="MapData"/> based on the <paramref name="referenceLine"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="referenceLine"/> is <c>null</c>.</exception>
        public static MapData Create(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException("referenceLine");
            }
            var features = GetMapFeature(referenceLine.Points);

            return new MapLineData(features, RingtoetsCommonDataResources.ReferenceLine_DisplayName)
            {
                Style = new LineStyle(Color.Red, 3, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="MapData"/> with default styling based on the locations of <paramref name="hydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> for which to create <see cref="MapData"/>.</param>
        /// <returns><see cref="MapData"/> based on the locations in the <paramref name="hydraulicBoundaryDatabase"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public static MapData Create(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryDatabase");
            }

            IEnumerable<Point2D> locations = hydraulicBoundaryDatabase.Locations.Select(h => h.Location).ToArray();

            var features = GetMapFeature(locations);

            return new MapPointData(features, RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName)
            {
                Style = new PointStyle(Color.DarkBlue, 6, PointSymbol.Circle)
            };
        }

        /// <summary>
        /// Create a <see cref="MapLineData"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapLineData"/>.</param>
        /// <returns>An empty <see cref="MapLineData"/> object.</returns>
        public static MapLineData CreateEmptyLineData(string name)
        {
            return new MapLineData(Enumerable.Empty<MapFeature>(), name);
        }

        /// <summary>
        /// Create a <see cref="MapPointData"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapPointData"/>.</param>
        /// <returns>An empty <see cref="MapPointData"/> object.</returns>
        public static MapPointData CreateEmptyPointData(string name)
        {
            return new MapPointData(Enumerable.Empty<MapFeature>(), name);
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
    }
}