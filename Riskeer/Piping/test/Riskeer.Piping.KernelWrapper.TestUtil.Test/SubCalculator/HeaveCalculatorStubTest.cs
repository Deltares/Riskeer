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

using System.Collections.Generic;
using NUnit.Framework;
using Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Riskeer.Piping.KernelWrapper.TestUtil.Test.SubCalculator
{
    [TestFixture]
    public class HeaveCalculatorStubTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var heaveCalculator = new HeaveCalculatorStub();

            // Assert
            Assert.AreEqual(0, heaveCalculator.DTotal);
            Assert.AreEqual(0, heaveCalculator.HExit);
            Assert.AreEqual(0, heaveCalculator.Ich);
            Assert.AreEqual(0, heaveCalculator.PhiExit);
            Assert.AreEqual(0, heaveCalculator.PhiPolder);
            Assert.AreEqual(0, heaveCalculator.RExit);

            Assert.AreEqual(0, heaveCalculator.Gradient);
            Assert.AreEqual(0, heaveCalculator.FoSh);
            Assert.AreEqual(0, heaveCalculator.Zh);
        }

        [Test]
        public void Validate_Always_EmptyListValidatedTrue()
        {
            // Setup
            var heaveCalculator = new HeaveCalculatorStub();

            // Call
            List<string> result = heaveCalculator.Validate();

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.IsTrue(heaveCalculator.Validated);
        }

        [Test]
        public void Calculate_Always_CalculatedTrue()
        {
            // Setup
            var heaveCalculator = new HeaveCalculatorStub();

            // Call
            heaveCalculator.Calculate();

            // Assert
            Assert.IsTrue(heaveCalculator.Calculated);
        }
    }
}