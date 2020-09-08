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

using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators.UpliftVan.Input
{
    [TestFixture]
    public class UpliftVanCalculatorInputTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnsUpliftVanCalculatorInput()
        {
            // Call
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            // Assert
            Assert.AreEqual(-1, input.AssessmentLevel);
            Assert.AreEqual(-1, input.WaterLevelRiverAverage);
            Assert.AreEqual(-1, input.WaterLevelPolderExtreme);
            Assert.AreEqual(-1, input.WaterLevelPolderDaily);
            Assert.AreEqual(0.1, input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(0.2, input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(1.3, input.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(1.4, input.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(1.5, input.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(1.6, input.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(0.3, input.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(0.4, input.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(0.5, input.PenetrationLengthExtreme);
            Assert.AreEqual(0.6, input.PenetrationLengthDaily);
            Assert.IsTrue(input.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, input.DikeSoilScenario);
            Assert.IsFalse(input.MoveGrid);
            Assert.AreEqual(1, input.MaximumSliceWidth);

            Assert.IsNotNull(input.SurfaceLine);
            Assert.IsNotNull(input.SoilProfile);
            Assert.IsNotNull(input.DrainageConstruction);
            Assert.IsNotNull(input.PhreaticLineOffsetsDaily);
            Assert.IsNotNull(input.PhreaticLineOffsetsExtreme);
            Assert.IsNotNull(input.SlipPlane);
            Assert.IsNotNull(input.SlipPlaneConstraints);

            Assert.AreEqual(1, input.SlipPlaneConstraints.SlipPlaneMinimumDepth);
            Assert.AreEqual(0.7, input.SlipPlaneConstraints.SlipPlaneMinimumLength);
        }
    }
}