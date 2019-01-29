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

using Core.Common.Base;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestCalculationWithForeshoreProfileTest
    {
        [Test]
        public void CreateDefaultCalculation_ReturnsExpectedValues()
        {
            // Call
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateDefaultCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<CloneableObservable>(calculation);
            Assert.IsTrue(calculation.ShouldCalculate);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments);

            TestCalculationWithForeshoreProfile.TestCalculationInputWithForeshoreProfile input =
                calculation.InputParameters;
            Assert.IsNull(input.ForeshoreProfile);
            Assert.IsFalse(input.UseForeshore);
            Assert.IsNull(input.ForeshoreGeometry);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCalculationWithOutput_WithOrWithoutForeshoreProfile_ReturnsExpectedValues(bool hasForeshoreProfile)
        {
            // Setup
            TestForeshoreProfile foreshoreProfile = null;

            if (hasForeshoreProfile)
            {
                foreshoreProfile = new TestForeshoreProfile();
            }

            // Call
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(foreshoreProfile);

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.IsFalse(calculation.ShouldCalculate);
            Assert.IsTrue(calculation.HasOutput);
            Assert.IsNull(calculation.Comments);

            TestCalculationWithForeshoreProfile.TestCalculationInputWithForeshoreProfile input =
                calculation.InputParameters;
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.IsFalse(input.UseForeshore);

            if (hasForeshoreProfile)
            {
                Assert.AreSame(foreshoreProfile.Geometry, input.ForeshoreGeometry);
            }
            else
            {
                Assert.IsNull(input.ForeshoreGeometry);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCalculationWithoutOutput_WithOrWithoutForeshoreProfile_ReturnsExpectedValues(bool hasForeshoreProfile)
        {
            // Setup
            TestForeshoreProfile foreshoreProfile = null;

            if (hasForeshoreProfile)
            {
                foreshoreProfile = new TestForeshoreProfile();
            }

            // Call
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithoutOutput(foreshoreProfile);

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.IsTrue(calculation.ShouldCalculate);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments);

            TestCalculationWithForeshoreProfile.TestCalculationInputWithForeshoreProfile input =
                calculation.InputParameters;
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.IsFalse(input.UseForeshore);

            if (hasForeshoreProfile)
            {
                Assert.AreSame(foreshoreProfile.Geometry, input.ForeshoreGeometry);
            }
            else
            {
                Assert.IsNull(input.ForeshoreGeometry);
            }
        }

        [Test]
        public void ClearOutput_CalculationWithOutput_OutputClear()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(foreshoreProfile);

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsTrue(calculation.ShouldCalculate);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void ClearOutput_WithoutOutput_DoesNotThrow()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithoutOutput(foreshoreProfile);

            // Call
            TestDelegate test = () => calculation.ClearOutput();

            // Assert
            Assert.DoesNotThrow(test);
            Assert.IsTrue(calculation.ShouldCalculate);
            Assert.IsFalse(calculation.HasOutput);
        }
    }
}