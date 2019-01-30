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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;
using Riskeer.HydraRing.Calculation.Data.Variables;
using Riskeer.HydraRing.Calculation.TestUtil;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class DikeHeightCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const double norm = 1.0 / 10000;
            const int hydraulicBoundaryLocationId = 1000;

            const double sectionNormal = 21.1;
            const double modelFactorCriticalOvertopping = 1.1;
            const double factorFbMean = 2.2;
            const double factorFbStandardDeviation = 3.3;
            const double factorFbLowerBoundary = 4.4;
            const double factorFbUpperBoundary = 5.5;
            const double factorFnMean = 6.6;
            const double factorFnStandardDeviation = 7.7;
            const double factorFnLowerBoundary = 8.8;
            const double factorFnUpperBoundary = 9.9;
            const double modelFactorOvertopping = 10.0;
            const double criticalOvertoppingMean = 11.1;
            const double criticalOvertoppingStandardDeviation = 12.2;
            const double modelFactorFrunupMean = 13.3;
            const double modelFactorFrunupStandardDeviation = 14.4;
            const double modelFactorFrunupLowerBoundary = 15.5;
            const double modelFactorFrunupUpperBoundary = 16.6;
            const double exponentModelFactorShallowMean = 17.7;
            const double exponentModelFactorShallowStandardDeviation = 18.8;
            const double exponentModelFactorShallowLowerBoundary = 19.9;
            const double exponentModelFactorShallowUpperBoundary = 20.0;

            var expectedRingProfilePoints = new List<HydraRingRoughnessProfilePoint>
            {
                new HydraRingRoughnessProfilePoint(1.1, 2.2, 3.3)
            };
            var expectedRingForelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(2.2, 3.3)
            };
            var expectedRingBreakWater = new HydraRingBreakWater(2, 3.3);

            // Call
            var input = new DikeHeightCalculationInput(hydraulicBoundaryLocationId, norm, sectionNormal,
                                                       expectedRingProfilePoints, expectedRingForelandPoints, expectedRingBreakWater,
                                                       modelFactorCriticalOvertopping,
                                                       factorFbMean, factorFbStandardDeviation,
                                                       factorFbLowerBoundary, factorFbUpperBoundary,
                                                       factorFnMean, factorFnStandardDeviation,
                                                       factorFnLowerBoundary, factorFnUpperBoundary,
                                                       modelFactorOvertopping,
                                                       criticalOvertoppingMean, criticalOvertoppingStandardDeviation,
                                                       modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                                       modelFactorFrunupLowerBoundary, modelFactorFrunupUpperBoundary,
                                                       exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation,
                                                       exponentModelFactorShallowLowerBoundary, exponentModelFactorShallowUpperBoundary);

            // Assert
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(norm);
            Assert.IsInstanceOf<HydraulicLoadsCalculationInput>(input);
            Assert.AreEqual(9, input.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikeHeight, input.FailureMechanismType);
            Assert.AreEqual(1, input.VariableId);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultDikeHeightVariables().ToArray(), input.Variables.ToArray());
            CollectionAssert.AreEqual(expectedRingProfilePoints, input.ProfilePoints);
            CollectionAssert.AreEqual(expectedRingForelandPoints, input.ForelandsPoints);
            Assert.AreEqual(expectedRingBreakWater, input.BreakWater);
            Assert.AreEqual(expectedBeta, input.Beta);

            HydraRingSection hydraRingSection = input.Section;
            Assert.AreEqual(1, hydraRingSection.SectionId);
            Assert.IsNaN(hydraRingSection.SectionLength);
            Assert.AreEqual(sectionNormal, hydraRingSection.CrossSectionNormal);
        }

        [Test]
        [TestCase(101, null)]
        [TestCase(102, 94)]
        [TestCase(103, 95)]
        [TestCase(104, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Call
            var input = new DikeHeightCalculationInput(1, 1.0 / 1000, double.NaN,
                                                       new List<HydraRingRoughnessProfilePoint>(),
                                                       new List<HydraRingForelandPoint>(),
                                                       new HydraRingBreakWater(0, 1.1),
                                                       2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,
                                                       13, 14, 15, 16, 17, 18, 19, 20, 21);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, input.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable> GetDefaultDikeHeightVariables()
        {
            yield return new DeterministicHydraRingVariable(1, 0.0);
            yield return new DeterministicHydraRingVariable(8, 1.1);
            yield return new TruncatedNormalHydraRingVariable(10, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5);
            yield return new TruncatedNormalHydraRingVariable(11, HydraRingDeviationType.Standard, 6.6, 7.7, 8.8, 9.9);
            yield return new DeterministicHydraRingVariable(12, 10.0);
            yield return new LogNormalHydraRingVariable(17, HydraRingDeviationType.Standard, 11.1, 12.2);
            yield return new TruncatedNormalHydraRingVariable(120, HydraRingDeviationType.Standard, 13.3, 14.4, 15.5, 16.6);
            yield return new TruncatedNormalHydraRingVariable(123, HydraRingDeviationType.Standard, 17.7, 18.8, 19.9, 20.0);
        }
    }
}