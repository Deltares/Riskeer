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

            const double dikeHeight = 11.11;
            const double modelFactorCriticalOvertopping = 1;
            const double factorFnMean = 4.75;
            const double factorFnStandardDeviation = 0.5;
            const double hydraRingFactorFnMean = 2.6;
            const double hydraRingFactorFnStandardDeviation = 0.35;
            const double hydraRingmodelFactorOvertopping = 1;
            const double criticalOvertoppingMean = 22.22;
            const double criticalOvertoppingStandardDeviation = 33.33;
            const double hydraRingModelFactorFrunupMean = 1;
            const double hydraRingModelFactorFrunupStandardDeviation = 0.07;
            const double hydraRingExponentModelFactorShallowMean = 0.92;
            const double hydraRingExponentModelFactorShallowStandardDeviation = 0.24;
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
                                                                                                      dikeHeight, modelFactorCriticalOvertopping, factorFnMean, factorFnStandardDeviation,
                                                                                                      hydraRingFactorFnMean, hydraRingFactorFnStandardDeviation, hydraRingmodelFactorOvertopping,
                                                                                                      criticalOvertoppingMean, criticalOvertoppingStandardDeviation,
                                                                                                      hydraRingModelFactorFrunupMean, hydraRingModelFactorFrunupStandardDeviation,
                                                                                                      hydraRingExponentModelFactorShallowMean, hydraRingExponentModelFactorShallowStandardDeviation);

            // Assert
            const int expectedCalculationTypeId = 1;
            const int expectedVariableId = 1;
            Assert.AreEqual(expectedCalculationTypeId, overtoppingCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, overtoppingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesOvertopping, overtoppingCalculationInput.FailureMechanismType);
            Assert.AreEqual(expectedVariableId, overtoppingCalculationInput.VariableId);
            Assert.IsNotNull(overtoppingCalculationInput.Section);
            HydraRingVariableAssert.AreEqual(GetDefaultOvertoppingVariables().ToArray(), overtoppingCalculationInput.Variables.ToArray());
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
            yield return new HydraRingVariable(1, HydraRingDistributionType.Deterministic, 11.11, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(8, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(10, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 4.75, 0.5, double.NaN);
            yield return new HydraRingVariable(11, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 2.6, 0.35, double.NaN);
            yield return new HydraRingVariable(12, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(17, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 22.22, 33.33, double.NaN);
            yield return new HydraRingVariable(120, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1, 0.07, double.NaN);
            yield return new HydraRingVariable(123, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 0.92, 0.24, double.NaN);
        }
    }
}