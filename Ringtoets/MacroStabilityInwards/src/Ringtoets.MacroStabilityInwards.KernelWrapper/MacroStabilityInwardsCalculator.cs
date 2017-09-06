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
using Deltares.WTIStability.Data.Standard;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// This class represents a combination of macro stability inwards sub calculations, which together can be used
    /// to assess based on macro stability inwards.
    /// </summary>
    public class MacroStabilityInwardsCalculator
    {
        private readonly MacroStabilityInwardsCalculatorInput input;
        private readonly IMacroStabilityInwardsSubCalculatorFactory factory;
        private GeometryData geometryData;

        /// <summary>
        /// Constructs a new <see cref="MacroStabilityInwardsCalculator"/>. The <paramref name="input"/> is used to
        /// obtain the parameters used in the different sub calculations.
        /// </summary>
        /// <param name="input">The <see cref="MacroStabilityInwardsCalculatorInput"/> containing all the values required
        /// for performing a macro stability inwards calculation.</param>
        /// <param name="factory">The factory responsible for creating the sub calculators.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> or <paramref name="factory"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculator(MacroStabilityInwardsCalculatorInput input, IMacroStabilityInwardsSubCalculatorFactory factory)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), @"MacroStabilityInwardsCalculatorInput required for creating a MacroStabilityInwardsCalculator.");
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory), @"IMacroStabilityInwardsSubCalculatorFactory required for creating a MacroStabilityInwardsCalculator.");
            }
            this.input = input;
            this.factory = factory;
        }

        /// <summary>
        /// Performs the actual sub calculations and returns a <see cref="MacroStabilityInwardsCalculatorResult"/>, which
        /// contains the results of all sub calculations.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculatorResult"/> containing the results of the sub calculations.</returns>
        public MacroStabilityInwardsCalculatorResult Calculate()
        {
            IUpliftVanCalculator upliftVanCalculator = CalculateUpliftVan();
            return new MacroStabilityInwardsCalculatorResult();
        }

        private IUpliftVanCalculator CalculateUpliftVan()
        {
            IUpliftVanCalculator upliftVanCalculator = CreateUpliftVanCalculator();
            
            upliftVanCalculator.Calculate();

            return upliftVanCalculator;
        }

        private IUpliftVanCalculator CreateUpliftVanCalculator()
        {
            IUpliftVanCalculator calculator = factory.CreateUpliftVanCalculator();
            Soil[] soils = MacroStabilityInwardsSoilCreator.Create(input.SoilProfile);
            calculator.SoilModel = MacroStabilityInwardsSoilModelCreator.Create(soils);
            GeometrySurface[] surfaces = CreateSurfaces(input.SoilProfile);
            calculator.SoilProfile = CreateSoilProfile(input.SoilProfile.LayersUnderSurfaceLine.ToArray(), soils, surfaces);
            return calculator;
        }

        private GeometrySurface[] CreateSurfaces(MacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile)
        {
            geometryData = new GeometryData();

            var surfaces = new List<GeometrySurface>();
            foreach (MacroStabilityInwardsSoilLayerUnderSurfaceLine layer in soilProfile.LayersUnderSurfaceLine)
            {
                var outerLoop = new GeometryLoop();
                GeometryCurve[] curves = ToCurveList(layer.OuterRing);

                geometryData.Curves.AddRange(curves);

                outerLoop.CurveList.AddRange(curves);

                geometryData.Loops.Add(outerLoop);

                IEnumerable<GeometryLoop> innerloops = layer.Holes.Select(h =>
                {
                    GeometryCurve[] geometryCurves = ToCurveList(h);
                    geometryData.Curves.AddRange(geometryCurves);
                    return geometryCurves;
                }).Select(CurvesToLoop);

                geometryData.Loops.AddRange(innerloops);

                var surface = new GeometrySurface
                {
                    OuterLoop = outerLoop
                };
                surface.InnerLoops.AddRange(innerloops);

                surfaces.Add(surface);
            }

            geometryData.Surfaces.AddRange(surfaces);

            return surfaces.ToArray();
        }

        private static GeometryLoop CurvesToLoop(GeometryCurve[] curves)
        {
            var loop = new GeometryLoop();
            loop.CurveList.AddRange(curves);
            return loop;
        }

        private GeometryCurve[] ToCurveList(Core.Common.Base.Geometry.Point2D[] points)
        {
            var geometryPoints = (List<Point2D>) geometryData.Points;

            var curves = new List<GeometryCurve>();

            var firstPoint = new Point2D(points[0].X, points[0].Y);
            Point2D lastPoint = null;

            for (int i = 0; i < points.Length-1; i++)
            {
                Point2D headPoint = i == 0
                                        ? firstPoint
                                        : new Point2D(points[i].X, points[i].Y);

                var endPoint = new Point2D(points[i+1].X, points[i+1].Y);

                geometryPoints.Add(headPoint);
                geometryPoints.Add(endPoint);

                curves.Add(new GeometryCurve
                {
                    HeadPoint = headPoint,
                    EndPoint = endPoint,
                });

                lastPoint = endPoint;
            }

            curves.Add(new GeometryCurve
            {
                HeadPoint = lastPoint,
                EndPoint = firstPoint
            });

            return curves.ToArray();
        }

        private SoilProfile2D CreateSoilProfile(IList<MacroStabilityInwardsSoilLayerUnderSurfaceLine> layers, IList<Soil> soils, IList<GeometrySurface> surfaces)
        {
            var profile = new SoilProfile2D();

            for (int i = 0; i < layers.Count; i++)
            {
                profile.Surfaces.Add(new SoilLayer2D
                {
                    IsAquifer = layers[i].Properties.IsAquifer,
                    Soil = soils[i],
                    GeometrySurface = surfaces[i]
                });
            }

            profile.Geometry = geometryData;

            return profile;
        }

        /// <summary>
        /// Returns a list of validation messages. The validation messages are based on the values of the <see cref="MacroStabilityInwardsCalculatorInput"/>
        /// which was provided to this <see cref="MacroStabilityInwardsCalculator"/> and are determined by the kernel.
        /// </summary>
        public List<string> Validate()
        {
            return new List<string>();
        }
    }
}