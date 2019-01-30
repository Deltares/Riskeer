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
using Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Riskeer.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class EffectiveThicknessCalculatorStubTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var effectiveThicknessCalculator = new EffectiveThicknessCalculatorStub();

            // Assert
            Assert.AreEqual(0, effectiveThicknessCalculator.ExitPointXCoordinate);
            Assert.AreEqual(0, effectiveThicknessCalculator.PhreaticLevel);
            Assert.IsNull(effectiveThicknessCalculator.SoilProfile);
            Assert.IsNull(effectiveThicknessCalculator.SurfaceLine);
            Assert.AreEqual(0, effectiveThicknessCalculator.VolumicWeightOfWater);

            Assert.AreEqual(0.1, effectiveThicknessCalculator.EffectiveHeight);
        }

        [Test]
        public void Calculate_Always_DoesNotThrow()
        {
            // Setup
            var effectiveThicknessCalculator = new EffectiveThicknessCalculatorStub();

            // Call
            TestDelegate call = () => effectiveThicknessCalculator.Calculate();

            // Assert
            Assert.DoesNotThrow(call);
        }
    }
}