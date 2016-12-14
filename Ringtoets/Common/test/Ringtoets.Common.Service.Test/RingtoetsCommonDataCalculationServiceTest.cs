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

using System;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Output;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsCommonDataCalculationServiceTest
    {
        [Test]
        public void CalculationConverged_WithConvergedResults_CalculationConvergedTrue()
        {
            // Setup
            const double norm = 0.05;
            double reliabilityIndex = StatisticsConverter.ProbabilityToReliability(norm);

            // Call
            CalculationConvergence calculationConverged = RingtoetsCommonDataCalculationService.CalculationConverged(reliabilityIndex, norm);

            // Assert
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, calculationConverged);
        }

        [Test]
        public void CalculationConverged_WithoutConvergedResults_CalculationConvergedFalse()
        {
            // Setup
            var output = new ReliabilityIndexCalculationOutput(5.0e-3, 5.0e-3);
            const double norm = 1;

            // Call
            CalculationConvergence calculationConverged = RingtoetsCommonDataCalculationService.CalculationConverged(output.CalculatedReliabilityIndex, norm);

            // Assert
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, calculationConverged);
        }

        [Test]
        public void ProfileSpecificRequiredProbability_WithValidParameters_ReturnSpecificProbability()
        {
            // Setup
            const double norm = 1.0/200;
            const double contribution = 10;
            const int n = 2;

            // Call
            double probability = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(norm, contribution, n);

            // Assert
            Assert.AreEqual(0.00025, probability);
        }

        [Test]
        [TestCase(-1.0)]
        [TestCase(0.0)]
        public void ProfileSpecificRequiredProbability_WithZeroContribution_ThrowsArgumentException(double contribution)
        {
            // Setup
            const double norm = 1.0/200;
            const int n = 2;

            // Call
            TestDelegate action = () => RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(norm, contribution, n);

            // Assert
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(action);
            Assert.AreEqual(contribution, exception.ActualValue);
            Assert.AreEqual("failureMechanismContribution", exception.ParamName);
            StringAssert.StartsWith("De bijdrage van dit toetsspoor is nul of negatief. Daardoor is de doorsnede-eis onbepaald en kan de berekening niet worden uitgevoerd." +
                                    Environment.NewLine, exception.Message);
        }

        [Test]
        public void ProfileSpecificRequiredProbability_WithZeroN_ThrowsArgumentException()
        {
            // Setup
            const double norm = 1.0/200;
            const double contribution = 10;
            const int n = 0;

            // Call
            TestDelegate action = () => RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(norm, contribution, n);

            // Assert
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(action);
            Assert.AreEqual(n, exception.ActualValue);
            Assert.AreEqual("n", exception.ParamName);
            StringAssert.StartsWith("De N-waarde van dit toetsspoor is nul. Daardoor is de doorsnede-eis onbepaald en kan de berekening niet worden uitgevoerd." +
                                    Environment.NewLine, exception.Message);
        }
    }
}