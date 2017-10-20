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
using System.ComponentModel;
using System.Linq;
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.Data.Standard;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;
using SoilLayer = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;
using WtiStabilityPoint2D = Deltares.WTIStability.Data.Geo.Point2D;
using WtiStabilitySoilProfile = Deltares.WTIStability.Data.Geo.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="WtiStabilitySoilProfile"/> instances which are required in a calculation.
    /// </summary>
    internal static class SoilProfileCreator
    {
        /// <summary>
        /// Creates a <see cref="SoilProfile2D"/> with the given <paramref name="layersWithSoils"/>
        /// which can be used in a calculation.
        /// </summary>
        /// <param name="soilProfile">The soil profile to create the <see cref="SoilProfile2D"/> for.</param>
        /// <param name="layersWithSoils">The data to use in the <see cref="SoilProfile2D"/>.</param>
        /// <returns>A new <see cref="SoilProfile2D"/> with the <paramref name="layersWithSoils"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SoilLayer.WaterPressureInterpolationModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SoilLayer.WaterPressureInterpolationModel"/>
        /// is a valid value but unsupported.</exception>
        public static SoilProfile2D Create(SoilProfile soilProfile,
                                           IDictionary<SoilLayer, Soil> layersWithSoils)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }
            if (layersWithSoils == null)
            {
                throw new ArgumentNullException(nameof(layersWithSoils));
            }

            var profile = new SoilProfile2D();
            profile.PreconsolidationStresses.AddRange(CreatePreconsolidationStresses(soilProfile));

            var alreadyCreatedPoints = new List<WtiStabilityPoint2D>();
            var alreadyCreatedCurves = new List<GeometryCurve>();

            foreach (KeyValuePair<SoilLayer, Soil> layerWithSoil in layersWithSoils)
            {
                profile.Surfaces.Add(new SoilLayer2D
                {
                    IsAquifer = layerWithSoil.Key.IsAquifer,
                    Soil = layerWithSoil.Value,
                    GeometrySurface = CreateGeometrySurface(layerWithSoil.Key, alreadyCreatedPoints, alreadyCreatedCurves),
                    WaterpressureInterpolationModel = ConvertWaterPressureInterpolationModel(layerWithSoil.Key.WaterPressureInterpolationModel)
                });
            }

            profile.Geometry = CreateGeometryData(profile);

            return profile;
        }

        private static IEnumerable<PreConsolidationStress> CreatePreconsolidationStresses(SoilProfile soilProfile)
        {
            return soilProfile.PreconsolidationStresses.Select(preconsolidationStress => new PreConsolidationStress
            {
                StressValue = preconsolidationStress.Stress,
                X = preconsolidationStress.Coordinate.X,
                Z = preconsolidationStress.Coordinate.Y
            }).ToArray();
        }

        private static GeometrySurface CreateGeometrySurface(SoilLayer layer, List<WtiStabilityPoint2D> alreadyCreatedPoints, List<GeometryCurve> alreadyCreatedCurves)
        {
            var surface = new GeometrySurface
            {
                OuterLoop = CreateGeometryLoop(layer.OuterRing, alreadyCreatedPoints, alreadyCreatedCurves)
            };

            surface.InnerLoops.AddRange(layer.Holes.Select(h => CreateGeometryLoop(h, alreadyCreatedPoints, alreadyCreatedCurves)).ToArray());

            return surface;
        }

        private static GeometryLoop CreateGeometryLoop(Point2D[] points, List<WtiStabilityPoint2D> alreadyCreatedPoints, List<GeometryCurve> alreadyCreatedCurves)
        {
            var loop = new GeometryLoop();

            loop.CurveList.AddRange(CreateGeometryCurves(points, alreadyCreatedPoints, alreadyCreatedCurves));

            return loop;
        }

        private static GeometryCurve[] CreateGeometryCurves(Point2D[] points, List<WtiStabilityPoint2D> alreadyCreatedPoints, List<GeometryCurve> alreadyCreatedCurves)
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
                                                    .SelectMany(gs => new[]
                                                    {
                                                        gs.OuterLoop
                                                    }.Concat(gs.InnerLoops)));
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

        /// <summary>
        /// Converts a <see cref="WaterPressureInterpolationModel"/> into a <see cref="WaterpressureInterpolationModel"/>.
        /// </summary>
        /// <param name="waterPressureInterpolationModel">The <see cref="WaterPressureInterpolationModel"/> to convert.</param>
        /// <returns>A <see cref="WaterpressureInterpolationModel"/> based on <paramref name="waterPressureInterpolationModel"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="waterPressureInterpolationModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="waterPressureInterpolationModel"/>
        /// is a valid value but unsupported.</exception>
        private static WaterpressureInterpolationModel ConvertWaterPressureInterpolationModel(WaterPressureInterpolationModel waterPressureInterpolationModel)
        {
            if (!Enum.IsDefined(typeof(WaterPressureInterpolationModel), waterPressureInterpolationModel))
            {
                throw new InvalidEnumArgumentException(nameof(waterPressureInterpolationModel),
                                                       (int) waterPressureInterpolationModel,
                                                       typeof(WaterPressureInterpolationModel));
            }

            switch (waterPressureInterpolationModel)
            {
                case WaterPressureInterpolationModel.Automatic:
                    return WaterpressureInterpolationModel.Automatic;
                case WaterPressureInterpolationModel.Hydrostatic:
                    return WaterpressureInterpolationModel.Hydrostatic;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}