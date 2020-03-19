// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;
using WtiStabilityPoint2D = Deltares.WTIStability.Data.Geo.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="SoilProfile2D"/> instances which are required in a calculation.
    /// </summary>
    internal static class SoilProfileCreator
    {
        /// <summary>
        /// Creates a <see cref="SoilProfile2D"/> based on <paramref name="preconsolidationStresses"/> and
        /// <paramref name="layersWithSoil"/>.
        /// </summary>
        /// <param name="preconsolidationStresses">The preconsolidation stresses to use in the <see cref="SoilProfile2D"/>.</param>
        /// <param name="layersWithSoil">The layer data to use in the <see cref="SoilProfile2D"/>.</param>
        /// <returns>A new <see cref="SoilProfile2D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SoilProfile2D Create(IEnumerable<PreconsolidationStress> preconsolidationStresses,
                                           IEnumerable<LayerWithSoil> layersWithSoil)
        {
            if (preconsolidationStresses == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStresses));
            }

            if (layersWithSoil == null)
            {
                throw new ArgumentNullException(nameof(layersWithSoil));
            }

            var profile = new SoilProfile2D();
            profile.PreconsolidationStresses.AddRange(CreatePreconsolidationStresses(preconsolidationStresses));

            var alreadyCreatedPoints = new List<WtiStabilityPoint2D>();
            var alreadyCreatedCurves = new List<GeometryCurve>();
            var alreadyCreatedLoops = new List<GeometryLoop>();

            foreach (LayerWithSoil layerWithSoil in layersWithSoil)
            {
                profile.Surfaces.Add(new SoilLayer2D
                {
                    IsAquifer = layerWithSoil.IsAquifer,
                    Soil = layerWithSoil.Soil,
                    GeometrySurface = CreateGeometrySurface(layerWithSoil, alreadyCreatedPoints, alreadyCreatedCurves, alreadyCreatedLoops),
                    WaterpressureInterpolationModel = layerWithSoil.WaterPressureInterpolationModel
                });
            }

            profile.Geometry = CreateGeometryData(profile);

            return profile;
        }

        private static IEnumerable<PreConsolidationStress> CreatePreconsolidationStresses(IEnumerable<PreconsolidationStress> preconsolidationStresses)
        {
            return preconsolidationStresses.Select(preconsolidationStress => new PreConsolidationStress
            {
                StressValue = preconsolidationStress.Stress,
                X = preconsolidationStress.Coordinate.X,
                Z = preconsolidationStress.Coordinate.Y
            }).ToArray();
        }

        private static GeometrySurface CreateGeometrySurface(LayerWithSoil layer, List<WtiStabilityPoint2D> alreadyCreatedPoints, List<GeometryCurve> alreadyCreatedCurves, List<GeometryLoop> alreadyCreatedLoops)
        {
            var surface = new GeometrySurface
            {
                OuterLoop = CreateGeometryLoop(layer.OuterRing, alreadyCreatedPoints, alreadyCreatedCurves, alreadyCreatedLoops)
            };

            surface.InnerLoops.AddRange(layer.InnerRings.Select(ir => CreateGeometryLoop(ir, alreadyCreatedPoints, alreadyCreatedCurves, alreadyCreatedLoops)).ToArray());

            return surface;
        }

        private static GeometryLoop CreateGeometryLoop(IEnumerable<Point2D> points, List<WtiStabilityPoint2D> alreadyCreatedPoints, List<GeometryCurve> alreadyCreatedCurves, List<GeometryLoop> alreadyCreatedLoops)
        {
            GeometryCurve[] geometryCurves = CreateGeometryCurves(points, alreadyCreatedPoints, alreadyCreatedCurves);
            GeometryLoop loop = alreadyCreatedLoops.FirstOrDefault(l => l.CurveList.SequenceEqual(geometryCurves));

            if (loop == null)
            {
                loop = new GeometryLoop();

                loop.CurveList.AddRange(geometryCurves);

                alreadyCreatedLoops.Add(loop);
            }

            return loop;
        }

        private static GeometryCurve[] CreateGeometryCurves(IEnumerable<Point2D> points, List<WtiStabilityPoint2D> alreadyCreatedPoints, List<GeometryCurve> alreadyCreatedCurves)
        {
            var curves = new List<GeometryCurve>();

            WtiStabilityPoint2D[] stabilityPoints = points.Select(p => GetPoint(p, alreadyCreatedPoints)).ToArray();
            int stabilityPointsLength = stabilityPoints.Length;

            for (var i = 0; i < stabilityPointsLength; i++)
            {
                curves.Add(GetCurve(stabilityPoints[i],
                                    i == stabilityPointsLength - 1
                                        ? stabilityPoints[0]
                                        : stabilityPoints[i + 1],
                                    alreadyCreatedCurves));
            }

            return curves.ToArray();
        }

        private static WtiStabilityPoint2D GetPoint(Point2D point2D, List<WtiStabilityPoint2D> alreadyCreatedPoints)
        {
            WtiStabilityPoint2D stabilityPoint = alreadyCreatedPoints.FirstOrDefault(p => p.X.Equals(point2D.X) && p.Z.Equals(point2D.Y));
            if (stabilityPoint == null)
            {
                stabilityPoint = new WtiStabilityPoint2D(point2D.X, point2D.Y);

                alreadyCreatedPoints.Add(stabilityPoint);
            }

            return stabilityPoint;
        }

        private static GeometryCurve GetCurve(WtiStabilityPoint2D headPoint, WtiStabilityPoint2D endPoint, List<GeometryCurve> alreadyCreatedCurves)
        {
            GeometryCurve curve = alreadyCreatedCurves.FirstOrDefault(c => ReferenceEquals(c.HeadPoint, headPoint) && ReferenceEquals(c.EndPoint, endPoint)
                                                                           || ReferenceEquals(c.HeadPoint, endPoint) && ReferenceEquals(c.EndPoint, headPoint));
            if (curve == null)
            {
                curve = new GeometryCurve
                {
                    HeadPoint = headPoint,
                    EndPoint = endPoint
                };

                alreadyCreatedCurves.Add(curve);
            }

            return curve;
        }

        private static GeometryData CreateGeometryData(SoilProfile2D profile)
        {
            var geometryData = new GeometryData();

            geometryData.Surfaces.AddRange(profile.Surfaces
                                                  .Select(s => s.GeometrySurface));
            geometryData.Loops.AddRange(geometryData.Surfaces
                                                    .Select(gs => gs.OuterLoop));
            geometryData.Curves.AddRange(geometryData.Loops
                                                     .SelectMany(l => l.CurveList)
                                                     .Distinct());
            geometryData.Points.AddRange(geometryData.Curves
                                                     .SelectMany(c => new[]
                                                     {
                                                         c.HeadPoint,
                                                         c.EndPoint
                                                     }).Distinct());

            geometryData.Rebox();

            return geometryData;
        }
    }
}