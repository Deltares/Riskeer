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

using System;
using NUnit.Framework;

namespace Ringtoets.Common.Data.TestUtil.Test
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
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation.Comments);
        }

        [Test]
        public void CreateCalculationWithOutput_ForeshoreProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("foreshoreProfile", exception.ParamName);
        }

        [Test]
        public void CreateCalculationWithOutput_WithForeshoreProfile_ReturnsExpectedValues()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();

            // Call
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithOutput(foreshoreProfile);

            // Assert
            Assert.IsTrue(calculation.HasOutput);
            Assert.AreSame(foreshoreProfile, calculation.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation.Comments);
        }

        [Test]
        public void CreateCalculationWithoutOutput_WithForeshoreProfile_ReturnsExpectedValues()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();

            // Call
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithoutOutput(foreshoreProfile);

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.AreSame(foreshoreProfile, calculation.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation.Comments);
        }

        [Test]
        public void CreateCalculationWithoutOutput_ForeshoreProfileNull_ReturnsExpectedValues()
        {
            // Call
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithoutOutput(null);

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation.Comments);
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
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void ClearOutput_WithoutOutput_OutputClear()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            TestCalculationWithForeshoreProfile calculation =
                TestCalculationWithForeshoreProfile.CreateCalculationWithoutOutput(foreshoreProfile);

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
        }
    }
}