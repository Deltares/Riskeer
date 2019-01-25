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
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Overtopping
{
    [TestFixture]
    public class OvertoppingCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;

            const double sectionNormal = 22.2;
            const double dikeHeight = 1.1;
            const double modelFactorCriticalOvertopping = 2.2;
            const double factorFbMean = 3.3;
            const double factorFbStandardDeviation = 4.4;
            const double factorFbLowerBoundary = 14.4;
            const double factorFbUpperBoundary = 15.5;
            const double factorFnMean = 5.5;
            const double factorFnStandardDeviation = 6.6;
            const double factorFnLowerBoundary = 16.6;
            const double factorFnUpperBoundary = 17.7;
            const double modelFactorOvertopping = 7.7;
            const double criticalOvertoppingMean = 8.8;
            const double criticalOvertoppingStandardDeviation = 9.9;
            const double modelFactorFrunupMean = 10.0;
            const double modelFactorFrunupStandardDeviation = 11.1;
            const double modelFactorFrunupLowerBoundary = 18.8;
            const double modelFactorFrunupUpperBoundary = 19.9;
            const double exponentModelFactorShallowMean = 12.2;
            const double exponentModelFactorShallowStandardDeviation = 13.3;
            const double exponentModelFactorShallowLowerBoundary = 20.0;
            const double exponentModelFactorShallowUpperBoundary = 21.1;

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

            var overtoppingCalculationInput = new OvertoppingCalculationInput(
                hydraulicBoundaryLocationId, sectionNormal,
                expectedRingProfilePoints, expectedRingForelandPoints, expectedRingBreakWater,
                dikeHeight,
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
            const int expectedCalculationTypeId = 1;
            const int expectedVariableId = 1;
            Assert.IsInstanceOf<ExceedanceProbabilityCalculationInput>(overtoppingCalculationInput);
            Assert.AreEqual(expectedCalculationTypeId, overtoppingCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, overtoppingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesOvertopping, overtoppingCalculationInput.FailureMechanismType);
            Assert.AreEqual(expectedVariableId, overtoppingCalculationInput.VariableId);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultOvertoppingVariables().ToArray(), overtoppingCalculationInput.Variables.ToArray());
            CollectionAssert.AreEqual(expectedRingProfilePoints, overtoppingCalculationInput.ProfilePoints);
            CollectionAssert.AreEqual(expectedRingForelandPoints, overtoppingCalculationInput.ForelandsPoints);
            Assert.AreEqual(expectedRingBreakWater, overtoppingCalculationInput.BreakWater);
            Assert.IsNaN(overtoppingCalculationInput.Beta);

            HydraRingSection hydraRingSection = overtoppingCalculationInput.Section;
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
            var overtoppingCalculationInput = new OvertoppingCalculationInput(
                1, double.NaN,
                new List<HydraRingRoughnessProfilePoint>(),
                new List<HydraRingForelandPoint>(),
                new HydraRingBreakWater(0, 1.1),
                2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
                15, 16, 17, 18, 19, 20, 21, 22);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, overtoppingCalculationInput.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable> GetDefaultOvertoppingVariables()
        {
            yield return new DeterministicHydraRingVariable(1, 1.1);
            yield return new DeterministicHydraRingVariable(8, 2.2);
            yield return new TruncatedNormalHydraRingVariable(10, HydraRingDeviationType.Standard, 3.3, 4.4, 14.4, 15.5);
            yield return new TruncatedNormalHydraRingVariable(11, HydraRingDeviationType.Standard, 5.5, 6.6, 16.6, 17.7);
            yield return new DeterministicHydraRingVariable(12, 7.7);
            yield return new LogNormalHydraRingVariable(17, HydraRingDeviationType.Standard, 8.8, 9.9);
            yield return new TruncatedNormalHydraRingVariable(120, HydraRingDeviationType.Standard, 10.0, 11.1, 18.8, 19.9);
            yield return new TruncatedNormalHydraRingVariable(123, HydraRingDeviationType.Standard, 12.2, 13.3, 20.0, 21.1);
        }
    }
}