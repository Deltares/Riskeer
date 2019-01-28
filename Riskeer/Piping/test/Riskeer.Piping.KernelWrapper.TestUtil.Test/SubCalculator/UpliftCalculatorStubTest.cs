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
using System.Collections.Generic;
using Deltares.WTIPiping;
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
            var upliftCalculator = new UpliftCalculatorStub();

            // Assert
            Assert.IsFalse(upliftCalculator.ThrowExceptionOnCalculate);
            Assert.AreEqual(0, upliftCalculator.EffectiveStress);
            Assert.AreEqual(0, upliftCalculator.HExit);
            Assert.AreEqual(0, upliftCalculator.HRiver);
            Assert.AreEqual(0, upliftCalculator.ModelFactorUplift);
            Assert.AreEqual(0, upliftCalculator.PhiExit);
            Assert.AreEqual(0, upliftCalculator.PhiPolder);
            Assert.AreEqual(0, upliftCalculator.RExit);
            Assert.AreEqual(0, upliftCalculator.VolumetricWeightOfWater);
            Assert.AreEqual(0, upliftCalculator.FoSu);
            Assert.AreEqual(0, upliftCalculator.Zu);
        }

        [Test]
        public void Validate_Always_EmptyListValidatedTrue()
        {
            // Setup
            var upliftCalculator = new UpliftCalculatorStub();

            // Call
            List<string> result = upliftCalculator.Validate();

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.IsTrue(upliftCalculator.Validated);
        }

        [Test]
        public void Calculate_Always_CalculatedTrue()
        {
            // Setup
            var upliftCalculator = new UpliftCalculatorStub();

            // Call
            upliftCalculator.Calculate();

            // Assert
            Assert.IsTrue(upliftCalculator.Calculated);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateTrue_ThrowWTIUpliftCalculatorException()
        {
            // Setup
            var upliftCalculator = new UpliftCalculatorStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => upliftCalculator.Calculate();

            // Assert
            var exception = Assert.Throws<WTIUpliftCalculatorException>(test);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsFalse(upliftCalculator.Calculated);
        }
    }
}