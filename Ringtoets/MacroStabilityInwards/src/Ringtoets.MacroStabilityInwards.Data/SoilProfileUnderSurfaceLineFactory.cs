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
    public class SoilProfileUnderSurfaceLineFactory
    {
        public static SoilProfileUnderSurfaceLine Create(MacroStabilityInwardsSoilProfile soilProfile, RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }
            Point2D[] localizedSurfaceLine = surfaceLine.ProjectGeometryToLZ().ToArray();

            IEnumerable<Point2D> surfaceLineGeometry = CreateSurfaceLineAreaToDepth(localizedSurfaceLine, soilProfile.Bottom);
            IEnumerable<TempSoilLayerGeometry> layerGeometries = soilProfile.Layers.Select(l => As2DGeometry(l, soilProfile, localizedSurfaceLine.First().X, localizedSurfaceLine.Last().X));

            return GeometriesToIntersections(layerGeometries, surfaceLineGeometry);
        }

        private static SoilProfileUnderSurfaceLine GeometriesToIntersections(IEnumerable<TempSoilLayerGeometry> layerGeometries, IEnumerable<Point2D> surfaceLineGeometry)
        {
            var collection = new Collection<SoilLayerUnderSurfaceLine>();

            IEnumerable<Point2D> surfaceLineGeometryArray = surfaceLineGeometry.ToArray();

            foreach (TempSoilLayerGeometry layer in layerGeometries)
            {
                foreach (Point2D[] soilLayerArea in CreateSoilLayerAreas(surfaceLineGeometryArray, layer.OuterLoop))
                {
                    collection.Add(new SoilLayerUnderSurfaceLine(soilLayerArea, layer.Properties));
                }
            }

            return new SoilProfileUnderSurfaceLine(collection);
        }

        private static TempSoilLayerGeometry As2DGeometry(MacroStabilityInwardsSoilLayer layer, MacroStabilityInwardsSoilProfile soilProfile, double minX, double maxX)
        {
            double top = layer.Top;
            double bottom = layer.Top - soilProfile.GetLayerThickness(layer);

            return new TempSoilLayerGeometry(new[]
            {
                new Point2D(minX, top),
                new Point2D(maxX, top),
                new Point2D(maxX, bottom),
                new Point2D(minX, bottom)
            }, layer.Properties);
        }

        private static IEnumerable<Point2D> CreateSurfaceLineAreaToDepth(Point2D[] localizedSurfaceLine, double soilProfileBottom)
        {
            foreach (Point2D point in localizedSurfaceLine)
            {
                yield return point;
            }
            double geometryBottom = Math.Min(soilProfileBottom, localizedSurfaceLine.Min(p => p.Y)) - 1;
            yield return new Point2D(localizedSurfaceLine.Last().X, geometryBottom);
            yield return new Point2D(localizedSurfaceLine.First().X, geometryBottom);
        }

        private static IEnumerable<Point2D[]> CreateSoilLayerAreas(IEnumerable<Point2D> surfaceLineGeometry, IEnumerable<Point2D> soilLayerGeometry)
        {
            return GetSoilLayerWithSurfaceLineIntersection(surfaceLineGeometry, soilLayerGeometry);
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

    public class SoilProfileUnderSurfaceLine
    {
        public SoilProfileUnderSurfaceLine(IEnumerable<SoilLayerUnderSurfaceLine> layersUnderSurfaceLine)
        {
            if (layersUnderSurfaceLine == null)
            {
                throw new ArgumentNullException(nameof(layersUnderSurfaceLine));
            }
            LayersUnderSurfaceLine = layersUnderSurfaceLine;
        }

        public IEnumerable<SoilLayerUnderSurfaceLine> LayersUnderSurfaceLine { get; }
    }

    public class SoilLayerUnderSurfaceLine
    {
        public SoilLayerUnderSurfaceLine(Point2D[] outerLoop, SoilLayerProperties properties)
        {
            if (outerLoop == null)
            {
                throw new ArgumentNullException(nameof(outerLoop));
            }
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            OuterLoop = outerLoop;
            InnerLoops = Enumerable.Empty<Point2D[]>();
            Properties = properties;
        }

        public Point2D[] OuterLoop { get; }
        public IEnumerable<Point2D[]> InnerLoops { get; }
        public SoilLayerProperties Properties { get; }
    }
}