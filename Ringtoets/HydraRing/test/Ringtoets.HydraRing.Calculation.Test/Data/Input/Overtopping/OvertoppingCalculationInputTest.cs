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
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Overtopping
{
    [TestFixture]
    public class OvertoppingCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            int hydraulicBoundaryLocationId = 1000;
            HydraRingSection expectedHydraRingSection = new HydraRingSection(1, double.NaN, double.NaN);

            const double dikeHeight = 1.1;
            const double modelFactorCriticalOvertopping = 2.2;
            const double factorFbMean = 3.3;
            const double factorFbStandardDeviation = 4.4;
            const double factorFnMean = 5.5;
            const double factorFnStandardDeviation = 6.6;
            const double modelFactorOvertopping = 7.7;
            const double criticalOvertoppingMean = 8.8;
            const double criticalOvertoppingStandardDeviation = 9.9;
            const double modelFactorFrunupMean = 10.0;
            const double modelFactorFrunupStandardDeviation = 11.1;
            const double exponentModelFactorShallowMean = 12.2;
            const double exponentModelFactorShallowStandardDeviation = 13.3;
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

            OvertoppingCalculationInput overtoppingCalculationInput = new OvertoppingCalculationInput(hydraulicBoundaryLocationId, expectedHydraRingSection,
                                                                                                      expectedRingProfilePoints, expectedRingForelandPoints, expectedRingBreakWater,
                                                                                                      dikeHeight,
                                                                                                      modelFactorCriticalOvertopping,
                                                                                                      factorFbMean, factorFbStandardDeviation,
                                                                                                      factorFnMean, factorFnStandardDeviation,
                                                                                                      modelFactorOvertopping,
                                                                                                      criticalOvertoppingMean, criticalOvertoppingStandardDeviation,
                                                                                                      modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                                                                                      exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation);

            // Assert
            const int expectedCalculationTypeId = 1;
            const int expectedVariableId = 1;
            Assert.IsInstanceOf<ExceedanceProbabilityCalculationInput>(overtoppingCalculationInput);
            Assert.AreEqual(expectedCalculationTypeId, overtoppingCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, overtoppingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesOvertopping, overtoppingCalculationInput.FailureMechanismType);
            Assert.AreEqual(expectedVariableId, overtoppingCalculationInput.VariableId);
            Assert.IsNotNull(overtoppingCalculationInput.Section);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultOvertoppingVariables().ToArray(), overtoppingCalculationInput.Variables.ToArray());
            CollectionAssert.AreEqual(expectedRingProfilePoints, overtoppingCalculationInput.ProfilePoints);
            CollectionAssert.AreEqual(expectedRingForelandPoints, overtoppingCalculationInput.ForelandsPoints);
            Assert.AreEqual(expectedRingBreakWater, overtoppingCalculationInput.BreakWater);
            Assert.IsNaN(overtoppingCalculationInput.Beta);

            var section = overtoppingCalculationInput.Section;
            Assert.AreEqual(expectedHydraRingSection, section);
        }

        [Test]
        [TestCase(101, null)]
        [TestCase(102, 94)]
        [TestCase(103, 95)]
        [TestCase(104, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Setup 
            HydraRingSection section = new HydraRingSection(1, double.NaN, double.NaN);

            // Call
            OvertoppingCalculationInput overtoppingCalculationInput = new OvertoppingCalculationInput(1, section,
                                                                                                      new List<HydraRingRoughnessProfilePoint>(),
                                                                                                      new List<HydraRingForelandPoint>(),
                                                                                                      new HydraRingBreakWater(0, 1.1),
                                                                                                      2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, overtoppingCalculationInput.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable> GetDefaultOvertoppingVariables()
        {
            yield return new DeterministicHydraRingVariable(1, 1.1);
            yield return new DeterministicHydraRingVariable(8, 2.2);
            yield return new NormalHydraRingVariable(10, HydraRingDeviationType.Standard, 3.3, 4.4);
            yield return new NormalHydraRingVariable(11, HydraRingDeviationType.Standard, 5.5, 6.6);
            yield return new DeterministicHydraRingVariable(12, 7.7);
            yield return new LogNormalHydraRingVariable(17, HydraRingDeviationType.Standard, 8.8, 9.9);
            yield return new NormalHydraRingVariable(120, HydraRingDeviationType.Standard, 10.0, 11.1);
            yield return new NormalHydraRingVariable(123, HydraRingDeviationType.Standard, 12.2, 13.3);
        }
    }
}