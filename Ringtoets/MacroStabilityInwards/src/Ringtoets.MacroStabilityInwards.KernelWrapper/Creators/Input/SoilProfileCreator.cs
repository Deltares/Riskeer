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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Point2D = Core.Common.Base.Geometry.Point2D;
using SoilLayer = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;
using WtiStabilityPoint2D = Deltares.WTIStability.Data.Geo.Point2D;
using WtiStabilitySoilProfile = Deltares.WTIStability.Data.Geo.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="WtiStabilitySoilProfile"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    internal static class SoilProfileCreator
    {
        /// <summary>
        /// Creates a <see cref="SoilProfile2D"/> with the given <paramref name="layersWithSoils"/>
        /// which can be used by <see cref="IUpliftVanKernel"/>.
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

            foreach (KeyValuePair<SoilLayer, Soil> layerWithSoil in layersWithSoils)
            {
                profile.Surfaces.Add(new SoilLayer2D
                {
                    IsAquifer = layerWithSoil.Key.IsAquifer,
                    Soil = layerWithSoil.Value,
                    GeometrySurface = CreateGeometrySurface(layerWithSoil.Key),
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

        private static GeometrySurface CreateGeometrySurface(SoilLayer layer)
        {
            var surface = new GeometrySurface
            {
                OuterLoop = CreateGeometryLoop(layer.OuterRing)
            };

            surface.InnerLoops.AddRange(layer.Holes.Select(CreateGeometryLoop).ToArray());

            return surface;
        }

        private static GeometryLoop CreateGeometryLoop(Point2D[] points)
        {
            var loop = new GeometryLoop();

            loop.CurveList.AddRange(CreateGeometryCurves(points));

            return loop;
        }

        private static GeometryCurve[] CreateGeometryCurves(Point2D[] points)
        {
            var curves = new List<GeometryCurve>();
            var firstPoint = new WtiStabilityPoint2D(points[0].X, points[0].Y);
            WtiStabilityPoint2D lastPoint = null;

            for (var i = 0; i < points.Length - 1; i++)
            {
                WtiStabilityPoint2D headPoint = i == 0 ? firstPoint : lastPoint;

                var endPoint = new WtiStabilityPoint2D(points[i + 1].X, points[i + 1].Y);

                curves.Add(new GeometryCurve
                {
                    HeadPoint = headPoint,
                    EndPoint = endPoint
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
                                                     .SelectMany(l => l.CurveList));
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