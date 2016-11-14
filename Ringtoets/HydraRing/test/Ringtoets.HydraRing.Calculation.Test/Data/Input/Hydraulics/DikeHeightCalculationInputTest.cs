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

            const double modelFactorCriticalOvertopping = 1.1;
            const double factorFbMean = 2.2;
            const double factorFbStandardDeviation = 3.3;
            const double factorFnMean = 4.4;
            const double factorFnStandardDeviation = 5.5;
            const double modelFactorOvertopping = 6.6;
            const double criticalOvertoppingMean = 7.7;
            const double criticalOvertoppingStandardDeviation = 8.8;
            const double modelFactorFrunupMean = 9.9;
            const double modelFactorFrunupStandardDeviation = 10.0;
            const double exponentModelFactorShallowMean = 11.1;
            const double exponentModelFactorShallowStandardDeviation = 12.2;
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
            var input = new DikeHeightCalculationInput(hydraulicBoundaryLocationId, norm, section,
                                                       expectedRingProfilePoints, expectedRingForelandPoints, expectedRingBreakWater,
                                                       modelFactorCriticalOvertopping,
                                                       factorFbMean, factorFbStandardDeviation,
                                                       factorFnMean, factorFnStandardDeviation,
                                                       modelFactorOvertopping,
                                                       criticalOvertoppingMean, criticalOvertoppingStandardDeviation,
                                                       modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                                       exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation);

            // Assert
            double expectedBeta = StatisticsConverter.NormToBeta(norm);
            Assert.IsInstanceOf<HydraulicLoadsCalculationInput>(input);
            Assert.AreEqual(9, input.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.DikesHeight, input.FailureMechanismType);
            Assert.AreEqual(1, input.VariableId);
            Assert.IsNotNull(input.Section);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultDikeHeightVariables().ToArray(), input.Variables.ToArray());
            CollectionAssert.AreEqual(expectedRingProfilePoints, input.ProfilePoints);
            CollectionAssert.AreEqual(expectedRingForelandPoints, input.ForelandsPoints);
            Assert.AreEqual(expectedRingBreakWater, input.BreakWater);
            Assert.AreEqual(expectedBeta, input.Beta);
            Assert.AreSame(section, input.Section);
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
            var input = new DikeHeightCalculationInput(1, 1000, section,
                                                       new List<HydraRingRoughnessProfilePoint>(),
                                                       new List<HydraRingForelandPoint>(),
                                                       new HydraRingBreakWater(0, 1.1),
                                                       2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, input.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable> GetDefaultDikeHeightVariables()
        {
            yield return new HydraRingVariable(1, HydraRingDistributionType.Deterministic, 0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(8, HydraRingDistributionType.Deterministic, 1.1, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(10, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 2.2, 3.3, double.NaN);
            yield return new HydraRingVariable(11, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 4.4, 5.5, double.NaN);
            yield return new HydraRingVariable(12, HydraRingDistributionType.Deterministic, 6.6, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(17, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 7.7, 8.8, double.NaN);
            yield return new HydraRingVariable(120, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 9.9, 10.0, double.NaN);
            yield return new HydraRingVariable(123, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 11.1, 12.2, double.NaN);
        }
    }
}