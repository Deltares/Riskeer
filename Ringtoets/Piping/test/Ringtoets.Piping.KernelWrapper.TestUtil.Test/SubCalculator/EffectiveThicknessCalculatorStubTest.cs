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

using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class EffectiveThicknessCalculatorStubTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var stub = new EffectiveThicknessCalculatorStub();

            // Assert
            Assert.AreEqual(0, stub.ExitPointXCoordinate);
            Assert.AreEqual(0, stub.PhreaticLevel);
            Assert.IsNull(stub.SoilProfile);
            Assert.IsNull(stub.SurfaceLine);
            Assert.AreEqual(0, stub.VolumicWeightOfWater);

            Assert.AreEqual(0.1, stub.EffectiveHeight);
        }

        [Test]
        public void Calculate_Always_DoesNotThrow()
        {
            // Setup
            var stub = new EffectiveThicknessCalculatorStub();

            // Call
            TestDelegate call = () => stub.Calculate();

            // Assert
            Assert.DoesNotThrow(call);
        }
    }
}