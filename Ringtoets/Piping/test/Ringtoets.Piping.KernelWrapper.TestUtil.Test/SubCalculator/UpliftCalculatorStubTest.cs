// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class UpliftCalculatorStubTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var stub = new UpliftCalculatorStub();

            // Assert
            Assert.AreEqual(0, stub.EffectiveStress);
            Assert.AreEqual(0, stub.HExit);
            Assert.AreEqual(0, stub.HRiver);
            Assert.AreEqual(0, stub.ModelFactorUplift);
            Assert.AreEqual(0, stub.PhiExit);
            Assert.AreEqual(0, stub.PhiPolder);
            Assert.AreEqual(0, stub.RExit);
            Assert.AreEqual(0, stub.VolumetricWeightOfWater);

            Assert.AreEqual(0, stub.FoSu);
            Assert.AreEqual(0, stub.Zu);
        }

        [Test]
        public void Validate_Always_EmptyListValidatedTrue()
        {
            // Setup
            var stub = new UpliftCalculatorStub();

            // Call
            List<string> result = stub.Validate();

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.IsTrue(stub.Validated);
        }

        [Test]
        public void Calculate_Always_CalculatedTrue()
        {
            // Setup
            var stub = new UpliftCalculatorStub();

            // Call
            stub.Calculate();

            // Assert
            Assert.IsTrue(stub.Calculated);
        }
    }
}