// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Input
{
    /// <summary>
    /// Factory to create simple <see cref="WaternetCalculatorInput"/> instances that can be used for testing.
    /// </summary>
    public static class WaternetCalculatorInputTestFactory
    {
        /// <summary>
        /// Creates a new <see cref="WaternetCalculatorInput"/>.
        /// </summary>
        /// <returns>The created <see cref="WaternetCalculatorInput"/>.</returns>
        public static WaternetCalculatorInput CreateCompleteCalculatorInput()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();

            return new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = random.NextDouble(),
                SurfaceLine = surfaceLine,
                SoilProfile = CreateValidSoilProfile(surfaceLine),
                DrainageConstruction = new DrainageConstruction(),
                PhreaticLineOffsets = new PhreaticLineOffsets(),
                WaterLevelRiverAverage = random.Next(),
                WaterLevelPolder = random.Next(),
                MinimumLevelPhreaticLineAtDikeTopRiver = random.Next(),
                MinimumLevelPhreaticLineAtDikeTopPolder = random.Next(),
                LeakageLengthOutwardsPhreaticLine3 = random.Next(),
                LeakageLengthInwardsPhreaticLine3 = random.Next(),
                LeakageLengthOutwardsPhreaticLine4 = random.Next(),
                LeakageLengthInwardsPhreaticLine4 = random.Next(),
                PiezometricHeadPhreaticLine2Outwards = random.Next(),
                PiezometricHeadPhreaticLine2Inwards = random.Next(),
                PenetrationLength = random.Next(),
                AdjustPhreaticLine3And4ForUplift = random.NextBoolean(),
                DikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>()
            });
        }

        /// <summary>
        /// Creates a new <see cref="WaternetCalculatorInput"/>.
        /// </summary>
        /// <returns>The created <see cref="WaternetCalculatorInput"/>.</returns>
        public static WaternetCalculatorInput CreateValidCalculatorInput()
        {
            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();
            return new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
            {
                SurfaceLine = surfaceLine,
                SoilProfile = CreateValidSoilProfile(surfaceLine),
                DrainageConstruction = new DrainageConstruction(),
                PhreaticLineOffsets = new PhreaticLineOffsets()
            });
        }

        private static SoilProfile CreateValidSoilProfile(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return new SoilProfile(new[]
            {
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    new SoilLayer.ConstructionProperties(),
                    Enumerable.Empty<SoilLayer>()),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    new SoilLayer.ConstructionProperties
                    {
                        IsAquifer = true
                    },
                    Enumerable.Empty<SoilLayer>()),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    new SoilLayer.ConstructionProperties(),
                    Enumerable.Empty<SoilLayer>()),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    new SoilLayer.ConstructionProperties(),
                    Enumerable.Empty<SoilLayer>())
            }, new[]
            {
                new PreconsolidationStress(new Point2D(0, 0), 1.1)
            });
        }

        private static MacroStabilityInwardsSurfaceLine CreateValidSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var dikeToeAtRiver = new Point3D(1, 0, 8);

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                dikeToeAtRiver,
                new Point3D(2, 0, -1)
            });

            surfaceLine.SetDikeToeAtRiverAt(dikeToeAtRiver);

            return surfaceLine;
        }
    }
}