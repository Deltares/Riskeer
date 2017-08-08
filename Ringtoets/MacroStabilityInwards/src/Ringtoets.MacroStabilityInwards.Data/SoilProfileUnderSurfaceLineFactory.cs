// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Geometry;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Class for constructing soil profiles for which the geometry of the layers lay under a surface line.
    /// </summary>
    public static class SoilProfileUnderSurfaceLineFactory
    {
        /// <summary>
        /// Creates a new <see cref="SoilProfileUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing layers under the <paramref name="surfaceLine"/>.</param>
        /// <param name="surfaceLine">The surface line for which determines the top of the <paramref name="soilProfile"/>.</param>
        /// <returns>A new <see cref="SoilProfileUnderSurfaceLine"/> containing geometries from the 
        /// <paramref name="soilProfile"/> under the <paramref name="surfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SoilProfileUnderSurfaceLine Create(MacroStabilityInwardsSoilProfile1D soilProfile, MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }
            Point2D[] localizedSurfaceLine = surfaceLine.LocalGeometry.ToArray();

            double geometryBottom = Math.Min(soilProfile.Bottom, localizedSurfaceLine.Min(p => p.Y)) - 1;
            IEnumerable<Point2D> surfaceLineGeometry = AdvancedMath2D.CompleteLineToPolygon(localizedSurfaceLine, geometryBottom);
            IEnumerable<TempSoilLayerGeometry> layerGeometries = soilProfile.Layers.Select(
                layer => As2DGeometry(
                    layer,
                    soilProfile,
                    localizedSurfaceLine.First().X,
                    localizedSurfaceLine.Last().X));

            return GeometriesToIntersections(layerGeometries, surfaceLineGeometry);
        }

        /// <summary>
        /// Creates a new <see cref="SoilProfileUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile containing layers.</param>
        /// <returns>A new <see cref="SoilProfileUnderSurfaceLine"/> containing geometries from the 
        /// <paramref name="soilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public static SoilProfileUnderSurfaceLine Create(MacroStabilityInwardsSoilProfile2D soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            IEnumerable<SoilLayerUnderSurfaceLine> layersUnderSurfaceLine = soilProfile.Layers.Select(
                layer => new SoilLayerUnderSurfaceLine(
                    RingToPoints(layer.OuterRing),
                    layer.Holes.Select(RingToPoints),
                    layer.Properties));

            return new SoilProfileUnderSurfaceLine(layersUnderSurfaceLine);
        }

        private static Point2D[] RingToPoints(Ring ring)
        {
            return ring.Points.ToArray();
        }

        private static SoilProfileUnderSurfaceLine GeometriesToIntersections(IEnumerable<TempSoilLayerGeometry> layerGeometries, IEnumerable<Point2D> surfaceLineGeometry)
        {
            var collection = new Collection<SoilLayerUnderSurfaceLine>();

            IEnumerable<Point2D> surfaceLineGeometryArray = surfaceLineGeometry.ToArray();

            foreach (TempSoilLayerGeometry layer in layerGeometries)
            {
                foreach (Point2D[] soilLayerArea in GetSoilLayerWithSurfaceLineIntersection(surfaceLineGeometryArray, layer.OuterLoop))
                {
                    collection.Add(new SoilLayerUnderSurfaceLine(soilLayerArea, layer.Properties));
                }
            }

            return new SoilProfileUnderSurfaceLine(collection);
        }

        private static TempSoilLayerGeometry As2DGeometry(MacroStabilityInwardsSoilLayer1D layer, MacroStabilityInwardsSoilProfile1D soilProfile, double minX, double maxX)
        {
            double top = layer.Top;
            double bottom = top - soilProfile.GetLayerThickness(layer);

            return new TempSoilLayerGeometry(new[]
            {
                new Point2D(minX, top),
                new Point2D(maxX, top),
                new Point2D(maxX, bottom),
                new Point2D(minX, bottom)
            }, layer.Properties);
        }

        private static IEnumerable<Point2D[]> GetSoilLayerWithSurfaceLineIntersection(IEnumerable<Point2D> surfaceLineGeometry, IEnumerable<Point2D> soilLayerGeometry)
        {
            return AdvancedMath2D.PolygonIntersectionWithPolygon(surfaceLineGeometry, soilLayerGeometry).Where(arr => arr.Length > 2);
        }

        private class TempSoilLayerGeometry
        {
            public TempSoilLayerGeometry(Point2D[] outerLoop, SoilLayerProperties properties)
            {
                OuterLoop = outerLoop;
                Properties = properties;
                InnerLoops = Enumerable.Empty<Point2D[]>();
            }

            public Point2D[] OuterLoop { get; }
            public SoilLayerProperties Properties { get; }
            public IEnumerable<Point2D[]> InnerLoops { get; }
        }
    }
}