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
using Core.Common.Base.Geometry;
using Deltares.MacroStability.CSharpWrapper.Input;
using CSharpWrapperPoint2D = Deltares.MacroStability.CSharpWrapper.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="SoilProfile"/> instances which are required in a calculation.
    /// </summary>
    internal static class SoilProfileCreator
    {
        /// <summary>
        /// Creates a <see cref="SoilProfile"/> based on <paramref name="layersWithSoil"/>.
        /// </summary>
        /// <param name="layersWithSoil">The layer data to use in the <see cref="SoilProfile"/>.</param>
        /// <returns>A new <see cref="SoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SoilProfile Create(IEnumerable<LayerWithSoil> layersWithSoil)
        {
            if (layersWithSoil == null)
            {
                throw new ArgumentNullException(nameof(layersWithSoil));
            }

            var profile = new SoilProfile();

            var alreadyCreatedPoints = new List<CSharpWrapperPoint2D>();
            var alreadyCreatedCurves = new List<Curve>();
            var alreadyCreatedLoops = new List<Loop>();

            foreach (LayerWithSoil layerWithSoil in layersWithSoil)
            {
                profile.SoilSurfaces.Add(new SoilProfileSurface
                {
                    Name = layerWithSoil.Soil.Name,
                    IsAquifer = layerWithSoil.IsAquifer,
                    Soil = layerWithSoil.Soil,
                    Surface = CreateSurface(layerWithSoil, alreadyCreatedPoints, alreadyCreatedCurves, alreadyCreatedLoops),
                    WaterPressureInterpolationModel = layerWithSoil.WaterPressureInterpolationModel
                });
            }

            profile.Geometry = CreateGeometry(profile);

            return profile;
        }

        private static Surface CreateSurface(LayerWithSoil layer, List<CSharpWrapperPoint2D> alreadyCreatedPoints, List<Curve> alreadyCreatedCurves, List<Loop> alreadyCreatedLoops)
        {
            return new Surface
            {
                OuterLoop = CreateLoop(layer.OuterRing, alreadyCreatedPoints, alreadyCreatedCurves, alreadyCreatedLoops),
                InnerLoops = layer.InnerRings.Select(ir => CreateLoop(ir, alreadyCreatedPoints, alreadyCreatedCurves, alreadyCreatedLoops)).ToArray()
            };
        }

        private static Loop CreateLoop(IEnumerable<Point2D> points, List<CSharpWrapperPoint2D> alreadyCreatedPoints, List<Curve> alreadyCreatedCurves, List<Loop> alreadyCreatedLoops)
        {
            Curve[] curves = CreateCurves(points, alreadyCreatedPoints, alreadyCreatedCurves);
            Loop loop = alreadyCreatedLoops.FirstOrDefault(l => l.Curves.SequenceEqual(curves));

            if (loop == null)
            {
                loop = new Loop
                {
                    Curves = curves
                };

                alreadyCreatedLoops.Add(loop);
            }

            return loop;
        }

        private static Curve[] CreateCurves(IEnumerable<Point2D> points, List<CSharpWrapperPoint2D> alreadyCreatedPoints, List<Curve> alreadyCreatedCurves)
        {
            var curves = new List<Curve>();

            CSharpWrapperPoint2D[] stabilityPoints = points.Select(p => GetPoint(p, alreadyCreatedPoints)).ToArray();
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

        private static CSharpWrapperPoint2D GetPoint(Point2D point2D, List<CSharpWrapperPoint2D> alreadyCreatedPoints)
        {
            CSharpWrapperPoint2D stabilityPoint = alreadyCreatedPoints.FirstOrDefault(p => p.X.Equals(point2D.X) && p.Z.Equals(point2D.Y));
            if (stabilityPoint == null)
            {
                stabilityPoint = new CSharpWrapperPoint2D(point2D.X, point2D.Y);

                alreadyCreatedPoints.Add(stabilityPoint);
            }

            return stabilityPoint;
        }

        private static Curve GetCurve(CSharpWrapperPoint2D headPoint, CSharpWrapperPoint2D endPoint, List<Curve> alreadyCreatedCurves)
        {
            Curve curve = alreadyCreatedCurves.FirstOrDefault(c => ReferenceEquals(c.HeadPoint, headPoint) && ReferenceEquals(c.EndPoint, endPoint)
                                                                   || ReferenceEquals(c.HeadPoint, endPoint) && ReferenceEquals(c.EndPoint, headPoint));
            if (curve == null)
            {
                curve = new Curve
                {
                    HeadPoint = headPoint,
                    EndPoint = endPoint
                };

                alreadyCreatedCurves.Add(curve);
            }

            return curve;
        }

        private static Geometry CreateGeometry(SoilProfile profile)
        {
            var geometry = new Geometry
            {
                Surfaces = profile.SoilSurfaces.Select(s => s.Surface).ToArray()
            };

            geometry.Loops = geometry.Surfaces
                                     .Select(s => s.OuterLoop)
                                     .ToArray();

            geometry.Curves = geometry.Loops
                                      .SelectMany(l => l.Curves)
                                      .Distinct()
                                      .ToArray();
            geometry.Points = geometry.Curves
                                      .SelectMany(c => new[]
                                      {
                                          c.HeadPoint,
                                          c.EndPoint
                                      })
                                      .Distinct()
                                      .ToArray();

            return geometry;
        }
    }
}