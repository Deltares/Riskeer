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
using System.Linq;
using Deltares.WTIStability.Data.Geo;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="SoilProfile"/> instances which are required by the <see cref="MacroStabilityInwardsCalculator"/>.
    /// </summary>
    internal static class SoilProfileCreator
    {
        private static GeometryData geometryData;

        /// <summary>
        /// Creates a <see cref="SoilProfile2D"/> with the given <paramref name="layersWithSoils"/>
        /// which can be used in the <see cref="MacroStabilityInwardsCalculator"/>.
        /// </summary>
        /// <param name="layersWithSoils">The data to use in the <see cref="SoilProfile2D"/>.</param>
        /// <returns>A new <see cref="SoilProfile2D"/> with the <paramref name="layersWithSoils"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layersWithSoils"/> is <c>null</c>.</exception>
        public static SoilProfile2D Create(IDictionary<MacroStabilityInwardsSoilLayerUnderSurfaceLine, Soil> layersWithSoils)
        {
            if (layersWithSoils == null)
            {
                throw new ArgumentNullException(nameof(layersWithSoils));
            }

            geometryData = new GeometryData();

            var profile = new SoilProfile2D();

            foreach (KeyValuePair<MacroStabilityInwardsSoilLayerUnderSurfaceLine, Soil> layerWithSoil in layersWithSoils)
            {
                profile.Surfaces.Add(new SoilLayer2D
                {
                    IsAquifer = layerWithSoil.Key.Properties.IsAquifer,
                    Soil = layerWithSoil.Value,
                    GeometrySurface = CreateGeometrySurface(layerWithSoil.Key)
                });
            }

            geometryData.Left = geometryData.Points.Min(gp => gp.X);
            geometryData.Right = geometryData.Points.Max(gp => gp.X);
            geometryData.Bottom = geometryData.Points.Min(gp => gp.Z);

            profile.Geometry = geometryData;

            return profile;
        }

        private static GeometrySurface CreateGeometrySurface(MacroStabilityInwardsSoilLayerUnderSurfaceLine layer)
        {
            var outerLoop = new GeometryLoop();
            GeometryCurve[] curves = ToCurveList(layer.OuterRing);

            geometryData.Curves.AddRange(curves);
            outerLoop.CurveList.AddRange(curves);

            geometryData.Loops.Add(outerLoop);

            var innerloops = new List<GeometryLoop>();
            foreach (Core.Common.Base.Geometry.Point2D[] layerHole in layer.Holes)
            {
                GeometryCurve[] holeCurves = ToCurveList(layerHole);
                geometryData.Curves.AddRange(holeCurves);
                innerloops.Add(CurvesToLoop(holeCurves));
            }

            geometryData.Loops.AddRange(innerloops);

            var surface = new GeometrySurface
            {
                OuterLoop = outerLoop
            };
            surface.InnerLoops.AddRange(innerloops);

            geometryData.Surfaces.Add(surface);

            return surface;
        }

        private static GeometryLoop CurvesToLoop(GeometryCurve[] curves)
        {
            var loop = new GeometryLoop();
            loop.CurveList.AddRange(curves);
            return loop;
        }

        private static GeometryCurve[] ToCurveList(Core.Common.Base.Geometry.Point2D[] points)
        {
            var geometryPoints = (List<Point2D>)geometryData.Points;

            var curves = new List<GeometryCurve>();

            var firstPoint = new Point2D(points[0].X, points[0].Y);
            Point2D lastPoint = null;

            for (var i = 0; i < points.Length - 1; i++)
            {
                Point2D headPoint;

                if (i == 0)
                {
                    headPoint = firstPoint;
                    geometryPoints.Add(headPoint);
                }
                else
                {
                    headPoint = lastPoint;
                }

                var endPoint = new Point2D(points[i + 1].X, points[i + 1].Y);

                geometryPoints.Add(endPoint);

                curves.Add(new GeometryCurve
                {
                    HeadPoint = headPoint,
                    EndPoint = endPoint,
                });

                lastPoint = endPoint;
            }

            if (lastPoint != null && (Math.Abs(lastPoint.X - firstPoint.X) > 1e-6 || Math.Abs(lastPoint.Z - firstPoint.Z) > 1e-6))
            {
                curves.Add(new GeometryCurve
                {
                    HeadPoint = lastPoint,
                    EndPoint = firstPoint
                });
            }

            return curves.ToArray();
        }
    }
}