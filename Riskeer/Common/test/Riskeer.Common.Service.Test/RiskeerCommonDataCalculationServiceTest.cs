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
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class RiskeerCommonDataCalculationServiceTest
    {
        [Test]
        [TestCase(true, CalculationConvergence.CalculatedConverged)]
        [TestCase(false, CalculationConvergence.CalculatedNotConverged)]
        [TestCase(null, CalculationConvergence.NotCalculated)]
        public void GetCalculationConvergence_WithDifferentValues_ReturnsExpectedConvergence(bool? value, CalculationConvergence expectedConvergence)
        {
            // Call
            CalculationConvergence calculationConverged = RiskeerCommonDataCalculationService.GetCalculationConvergence(value);

            // Assert
            Assert.AreEqual(expectedConvergence, calculationConverged);
        }

        [Test]
        [Combinatorial]
        public void ProfileSpecificRequiredProbability_WithValidParameters_ReturnSpecificProbability(
            [Values(1, 0.5, 0)] double norm,
            [Values(100, 50, 0)] double failureMechanismContribution,
            [Values(10, 1)] double n)
        {
            // Call
            double probability = RiskeerCommonDataCalculationService.ProfileSpecificRequiredProbability(norm, failureMechanismContribution, (RoundedDouble) n);

            // Assert
            double expectedProfileSpecificRequiredProbability = norm * (failureMechanismContribution / 100) / n;
            Assert.AreEqual(expectedProfileSpecificRequiredProbability, probability);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ProfileSpecificRequiredProbability_WithInvalidNorm_ThrowsArgumentException(
            [Values(150, 1 + 1e-6, -1e-6, -150, double.NaN)]
            double norm)
        {
            // Setup
            const double failureMechanismContribution = 50;
            var n = (RoundedDouble) 10;

            // Call
            TestDelegate action = () => RiskeerCommonDataCalculationService.ProfileSpecificRequiredProbability(norm, failureMechanismContribution, n);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
            Assert.AreEqual(norm, exception.ActualValue);
            Assert.AreEqual("norm", exception.ParamName);
            StringAssert.StartsWith("De norm moet in het bereik [0,0, 1,0] liggen." +
                                    Environment.NewLine, exception.Message);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ProfileSpecificRequiredProbability_WithInvalidFailureMechanismContribution_ThrowsArgumentException(
            [Values(150, 100 + 1e-6, -1e-6, -150, double.NaN)]
            double failureMechanismContribution)
        {
            // Setup
            const double norm = 0.5;
            var n = (RoundedDouble) 10;

            // Call
            TestDelegate action = () => RiskeerCommonDataCalculationService.ProfileSpecificRequiredProbability(norm, failureMechanismContribution, n);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
            Assert.AreEqual(failureMechanismContribution, exception.ActualValue);
            Assert.AreEqual("failureMechanismContribution", exception.ParamName);
            StringAssert.StartsWith("De bijdrage van dit toetsspoor moet in het bereik [0,0, 100,0] liggen." +
                                    Environment.NewLine, exception.Message);
        }

        [Test]
        public void ProfileSpecificRequiredProbability_WithInvalidN_ThrowsArgumentException([Values(0, -1)] double n)
        {
            // Setup
            const double norm = 0.5;
            const double failureMechanismContribution = 50;

            // Call
            TestDelegate action = () => RiskeerCommonDataCalculationService.ProfileSpecificRequiredProbability(norm, failureMechanismContribution, (RoundedDouble) n);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
            Assert.AreEqual(n, exception.ActualValue);
            Assert.AreEqual("n", exception.ParamName);
            StringAssert.StartsWith("De N-waarde van dit toetsspoor moet groter zijn dan 0." +
                                    Environment.NewLine, exception.Message);
        }
    }
}