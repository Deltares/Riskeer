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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators.Waternet.Input
{
    [TestFixture]
    public class WaternetCalculatorInputTestFactoryTest
    {
        [Test]
        public void CreateCompleteCalculatorInput_Always_ReturnWaternetCalculatorInput()
        {
            // Call
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateCompleteCalculatorInput();

            // Assert
            Assert.IsNotNull(input.DrainageConstruction);
            Assert.IsNotNull(input.PhreaticLineOffsets);
            Assert.IsFalse(double.IsNaN(input.AssessmentLevel));
            Assert.IsFalse(double.IsNaN(input.WaterLevelRiverAverage));
            Assert.IsFalse(double.IsNaN(input.WaterLevelPolder));
            Assert.IsFalse(double.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopRiver));
            Assert.IsFalse(double.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopPolder));
            Assert.IsFalse(double.IsNaN(input.LeakageLengthOutwardsPhreaticLine3));
            Assert.IsFalse(double.IsNaN(input.LeakageLengthInwardsPhreaticLine3));
            Assert.IsFalse(double.IsNaN(input.LeakageLengthOutwardsPhreaticLine4));
            Assert.IsFalse(double.IsNaN(input.LeakageLengthInwardsPhreaticLine4));
            Assert.IsFalse(double.IsNaN(input.PiezometricHeadPhreaticLine2Outwards));
            Assert.IsFalse(double.IsNaN(input.PiezometricHeadPhreaticLine2Inwards));
            Assert.IsFalse(double.IsNaN(input.PenetrationLength));

            AssertSurfaceLine(input.SurfaceLine);
            AssertSoilProfile(input.SoilProfile, input.SurfaceLine);

            Assert.AreEqual(WaternetCreationMode.CreateWaternet, input.WaternetCreationMode);
            Assert.AreEqual(PlLineCreationMethod.RingtoetsWti2017, input.PlLineCreationMethod);
            Assert.AreEqual(LandwardDirection.PositiveX, input.LandwardDirection);
            Assert.IsTrue(Enum.IsDefined(typeof(MacroStabilityInwardsDikeSoilScenario), input.DikeSoilScenario));
        }

        [Test]
        public void CreateValidCalculatorInput_Always_ReturnWaternetCalculatorInput()
        {
            // Call
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            // Assert
            Assert.IsNotNull(input.DrainageConstruction);
            Assert.IsNotNull(input.PhreaticLineOffsets);
            Assert.IsNaN(input.AssessmentLevel);
            Assert.IsNaN(input.WaterLevelRiverAverage);
            Assert.IsNaN(input.WaterLevelPolder);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNaN(input.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNaN(input.LeakageLengthInwardsPhreaticLine3);
            Assert.IsNaN(input.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNaN(input.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNaN(input.PiezometricHeadPhreaticLine2Outwards);
            Assert.IsNaN(input.PiezometricHeadPhreaticLine2Inwards);
            Assert.IsNaN(input.PenetrationLength);

            AssertSurfaceLine(input.SurfaceLine);
            AssertSoilProfile(input.SoilProfile, input.SurfaceLine);

            Assert.AreEqual(WaternetCreationMode.CreateWaternet, input.WaternetCreationMode);
            Assert.AreEqual(PlLineCreationMethod.RingtoetsWti2017, input.PlLineCreationMethod);
            Assert.AreEqual(LandwardDirection.PositiveX, input.LandwardDirection);
            Assert.IsTrue(Enum.IsDefined(typeof(MacroStabilityInwardsDikeSoilScenario), input.DikeSoilScenario));
        }

        private static void AssertSoilProfile(SoilProfile soilProfile, MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            Assert.AreEqual(4, soilProfile.Layers.Count());

            foreach (SoilLayer layer in soilProfile.Layers)
            {
                CollectionAssert.AreEqual(new[]
                {
                    surfaceLine.LocalGeometry.First(),
                    surfaceLine.LocalGeometry.Last()
                }, layer.OuterRing);
                CollectionAssert.IsEmpty(layer.NestedLayers);
            }

            CollectionAssert.AreEqual(new[]
            {
                false,
                true,
                false,
                false
            }, soilProfile.Layers.Select(l => l.IsAquifer));

            Assert.AreEqual(1, soilProfile.PreconsolidationStresses.Count());
            PreconsolidationStress preconsolidationStress = soilProfile.PreconsolidationStresses.First();

            Assert.AreEqual(new Point2D(0, 0), preconsolidationStress.Coordinate);
            Assert.AreEqual(1.1, preconsolidationStress.Stress);
        }

        private static void AssertSurfaceLine(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            Assert.AreEqual(string.Empty, surfaceLine.Name);
            CollectionAssert.AreEqual(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, 8),
                new Point3D(2, 0, -1)
            }, surfaceLine.Points);

            Assert.AreEqual(new Point3D(1, 0, 8), surfaceLine.DikeToeAtRiver);
        }
    }
}