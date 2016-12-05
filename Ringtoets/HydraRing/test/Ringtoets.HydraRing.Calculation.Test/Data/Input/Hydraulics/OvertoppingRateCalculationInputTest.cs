﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class OvertoppingRateCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const double norm = 1.0/10000;
            const int hydraulicBoundaryLocationId = 1000;
            HydraRingSection section = new HydraRingSection(1, double.NaN, double.NaN);

            const double dikeHeight = 1.1;
            const double modelFactorCriticalOvertopping = 2.2;
            const double factorFbMean = 3.3;
            const double factorFbStandardDeviation = 4.4;
            const double factorFnMean = 5.5;
            const double factorFnStandardDeviation = 6.6;
            const double modelFactorOvertopping = 7.7;
            const double modelFactorFrunupMean = 8.8;
            const double modelFactorFrunupStandardDeviation = 9.9;
            const double exponentModelFactorShallowMean = 10.0;
            const double exponentModelFactorShallowStandardDeviation = 11.1;
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
            var input = new OvertoppingRateCalculationInput(hydraulicBoundaryLocationId, norm, section,
                                                            expectedRingProfilePoints, expectedRingForelandPoints, expectedRingBreakWater,
                                                            dikeHeight,
                                                            modelFactorCriticalOvertopping,
                                                            factorFbMean, factorFbStandardDeviation,
                                                            factorFnMean, factorFnStandardDeviation,
                                                            modelFactorOvertopping,
                                                            modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                                            exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation);

            // Assert
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(norm);
            Assert.IsInstanceOf<HydraulicLoadsCalculationInput>(input);
            Assert.AreEqual(9, input.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.HydraulicLoads, input.FailureMechanismType);
            Assert.AreEqual(17, input.VariableId);
            Assert.IsNotNull(input.Section);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultOvertoppingRateVariables().ToArray(), input.NewVariables.ToArray());
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
            var input = new OvertoppingRateCalculationInput(1, 1000, section,
                                                            new List<HydraRingRoughnessProfilePoint>(),
                                                            new List<HydraRingForelandPoint>(),
                                                            new HydraRingBreakWater(0, 1.1),
                                                            2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, input.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable2> GetDefaultOvertoppingRateVariables()
        {
            yield return new DeterministicHydraRingVariable(1, 1.1);
            yield return new DeterministicHydraRingVariable(8, 2.2);
            yield return new NormalHydraRingVariable(10, HydraRingDeviationType.Standard, 3.3, 4.4);
            yield return new NormalHydraRingVariable(11, HydraRingDeviationType.Standard, 5.5, 6.6);
            yield return new DeterministicHydraRingVariable(12, 7.7);
            yield return new DeterministicHydraRingVariable(17, 0.0);
            yield return new NormalHydraRingVariable(120, HydraRingDeviationType.Standard, 8.8, 9.9);
            yield return new NormalHydraRingVariable(123, HydraRingDeviationType.Standard, 10.0, 11.1);
        }
    }
}