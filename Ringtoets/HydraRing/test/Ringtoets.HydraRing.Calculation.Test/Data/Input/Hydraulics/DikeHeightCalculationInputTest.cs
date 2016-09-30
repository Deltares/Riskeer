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
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class DikeHeightCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            var norm = 10000;
            int hydraulicBoundaryLocationId = 1000;
            HydraRingSection section = new HydraRingSection(1, double.NaN, double.NaN);

            const double modelFactorCriticalOvertopping = 1;
            const double factorFbMean = 4.75;
            const double factorFbStandardDeviation = 0.5;
            const double factorFnMean = 2.6;
            const double factorFnStandardDeviation = 0.35;
            const double modelFactorOvertopping = 1;
            const double criticalOvertoppingMean = 22.22;
            const double criticalOvertoppingStandardDeviation = 33.33;
            const double modelFactorFrunupMean = 1;
            const double modelFactorFrunupStandardDeviation = 0.07;
            const double exponentModelFactorShallowMean = 0.92;
            const double exponentModelFactorShallowStandardDeviation = 0.24;
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
            DikeHeightCalculationInput dikeHeightCalculationInput = new DikeHeightCalculationInput(hydraulicBoundaryLocationId, norm, section,
                                                                                                   expectedRingProfilePoints, expectedRingForelandPoints, expectedRingBreakWater,
                                                                                                   modelFactorCriticalOvertopping,
                                                                                                   factorFbMean, factorFbStandardDeviation,
                                                                                                   factorFnMean, factorFnStandardDeviation,
                                                                                                   modelFactorOvertopping,
                                                                                                   criticalOvertoppingMean, criticalOvertoppingStandardDeviation,
                                                                                                   modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                                                                                   exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation);

            // Assert
            const int expectedCalculationTypeId = 2;
            const int expectedVariableId = 1;
            double expectedBeta = StatisticsConverter.NormToBeta(norm);
            Assert.IsInstanceOf<ReliabilityIndexCalculationInput>(dikeHeightCalculationInput);
            Assert.AreEqual(expectedCalculationTypeId, dikeHeightCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, dikeHeightCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesHeight, dikeHeightCalculationInput.FailureMechanismType);
            Assert.AreEqual(expectedVariableId, dikeHeightCalculationInput.VariableId);
            Assert.IsNotNull(dikeHeightCalculationInput.Section);
            HydraRingVariableAssert.AreEqual(GetDefaultDikeHeightVariables().ToArray(), dikeHeightCalculationInput.Variables.ToArray());
            CollectionAssert.AreEqual(expectedRingProfilePoints, dikeHeightCalculationInput.ProfilePoints);
            CollectionAssert.AreEqual(expectedRingForelandPoints, dikeHeightCalculationInput.ForelandsPoints);
            Assert.AreEqual(expectedRingBreakWater, dikeHeightCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, dikeHeightCalculationInput.Beta);
            Assert.AreSame(section, dikeHeightCalculationInput.Section);
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
            DikeHeightCalculationInput dikeHeightCalculationInput = new DikeHeightCalculationInput(1, 1000, section,
                                                                                                   new List<HydraRingRoughnessProfilePoint>(),
                                                                                                   new List<HydraRingForelandPoint>(),
                                                                                                   new HydraRingBreakWater(0, 1.1),
                                                                                                   2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, dikeHeightCalculationInput.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable> GetDefaultDikeHeightVariables()
        {
            yield return new HydraRingVariable(1, HydraRingDistributionType.Deterministic, 0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
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