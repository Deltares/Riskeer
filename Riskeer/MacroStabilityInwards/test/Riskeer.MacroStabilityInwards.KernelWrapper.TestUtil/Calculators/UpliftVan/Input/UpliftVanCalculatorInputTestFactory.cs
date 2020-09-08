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
using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Input
{
    /// <summary>
    /// Factory to create <see cref="UpliftVanCalculatorInput"/> instances that can be used for testing.
    /// </summary>
    public static class UpliftVanCalculatorInputTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanCalculatorInput"/>.
        /// </summary>
        /// <returns>The created <see cref="UpliftVanCalculatorInput"/>.</returns>
        public static UpliftVanCalculatorInput Create()
        {
            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();

            return new UpliftVanCalculatorInput(new UpliftVanCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = -1,
                SurfaceLine = surfaceLine,
                SoilProfile = CreateValidSoilProfile(),
                DrainageConstruction = new DrainageConstruction(),
                PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                SlipPlane = new UpliftVanSlipPlane(),
                SlipPlaneConstraints = new UpliftVanSlipPlaneConstraints(1, 0.7),
                WaterLevelRiverAverage = -1,
                WaterLevelPolderExtreme = -1,
                WaterLevelPolderDaily = -1,
                MinimumLevelPhreaticLineAtDikeTopRiver = 0.1,
                MinimumLevelPhreaticLineAtDikeTopPolder = 0.2,
                LeakageLengthOutwardsPhreaticLine3 = 1.3,
                LeakageLengthInwardsPhreaticLine3 = 1.4,
                LeakageLengthOutwardsPhreaticLine4 = 1.5,
                LeakageLengthInwardsPhreaticLine4 = 1.6,
                PiezometricHeadPhreaticLine2Outwards = 0.3,
                PiezometricHeadPhreaticLine2Inwards = 0.4,
                PenetrationLengthExtreme = 0.5,
                PenetrationLengthDaily = 0.6,
                AdjustPhreaticLine3And4ForUplift = true,
                DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay,
                MoveGrid = false,
                MaximumSliceWidth = 1
            });
        }

        private static SoilProfile CreateValidSoilProfile()
        {
            return new SoilProfile(new[]
            {
                new SoilLayer(
                    new[]
                    {
                        new Point2D(-5, 0),
                        new Point2D(0, 5),
                        new Point2D(5, 0)
                    },
                    new SoilLayer.ConstructionProperties(),
                    Enumerable.Empty<SoilLayer>()),
                new SoilLayer(
                    new[]
                    {
                        new Point2D(-10, 0),
                        new Point2D(10, 0),
                        new Point2D(10, -5),
                        new Point2D(-10, -5)
                    },
                    new SoilLayer.ConstructionProperties
                    {
                        MaterialName = "Material 1",
                        UsePop = true,
                        Pop = new Random(21).NextDouble()
                    },
                    Enumerable.Empty<SoilLayer>()),
                new SoilLayer(
                    new[]
                    {
                        new Point2D(-10, -5),
                        new Point2D(10, -5),
                        new Point2D(10, -10),
                        new Point2D(-10, -10)
                    },
                    new SoilLayer.ConstructionProperties
                    {
                        IsAquifer = true
                    },
                    Enumerable.Empty<SoilLayer>())
            }, new[]
            {
                new PreconsolidationStress(new Point2D(0, 0), 1.1)
            });
        }

        private static MacroStabilityInwardsSurfaceLine CreateValidSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(-10, 0, 0),
                new Point3D(-5, 0, 0),
                new Point3D(0, 0, 5),
                new Point3D(5, 0, 0),
                new Point3D(10, 0, 0)
            });

            surfaceLine.SetSurfaceLevelOutsideAt(new Point3D(-10, 0, 0));
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(-5, 0, 0));
            surfaceLine.SetDikeTopAtRiverAt(new Point3D(0, 0, 5));
            surfaceLine.SetDikeTopAtPolderAt(new Point3D(0, 0, 5));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(5, 0, 0));
            surfaceLine.SetSurfaceLevelInsideAt(new Point3D(10, 0, 0));

            return surfaceLine;
        }
    }
}