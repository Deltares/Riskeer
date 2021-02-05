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

using System;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Helpers;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Helpers
{
    [TestFixture]
    public class CalculationScenarioHelperTest
    {
        [Test]
        public void ContributionNumberOfDecimalPlaces_Always_ReturnsExpectedValue()
        {
            // Call
            int numberOfDecimalPlaces = CalculationScenarioHelper.ContributionNumberOfDecimalPlaces;

            // Assert
            Assert.AreEqual(4, numberOfDecimalPlaces);
        }

        [Test]
        [TestCaseSource(typeof(CalculationScenarioTestHelper), nameof(CalculationScenarioTestHelper.GetInvalidScenarioContributionValues))]
        public void ValidateScenarioContribution_InvalidValue_ThrowsArgumentException(double newValue)
        {
            
            // Call
            void Call() => CalculationScenarioHelper.ValidateScenarioContribution((RoundedDouble) newValue);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, "De waarde moet binnen het bereik [0 en 100] liggen.");
        }

        [Test]
        [TestCase(-0.0)]
        [TestCase(0.0)]
        [TestCase(1.0)]
        public void ValidateScenarioContribution_ValidValue_DoesNotThrow(double newValue)
        {
            // Call
            void Call() => CalculationScenarioHelper.ValidateScenarioContribution((RoundedDouble) newValue);

            // Assert
            Assert.DoesNotThrow(Call);
        }
    }
}