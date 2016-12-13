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
using Ringtoets.HydraRing.Calculation.Data.Variables;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class HydraulicLoadsCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const double norm = 1.0/10000;
            const int hydraulicBoundaryLocationId = 1000;
            HydraRingSection section = new HydraRingSection(1, double.NaN, double.NaN);

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
            const double modelFactorFrunupMean = 11.1;
            const double modelFactorFrunupStandardDeviation = 12.2;
            const double modelFactorFrunupLowerBoundary = 13.3;
            const double modelFactorFrunupUpperBoundary = 14.4;
            const double exponentModelFactorShallowMean = 15.5;
            const double exponentModelFactorShallowStandardDeviation = 16.6;
            const double exponentModelFactorShallowLowerBoundary = 17.7;
            const double exponentModelFactorShallowUpperBoundary = 18.8;

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
            var input = new HydraulicLoadsCalculationInputImplementation(hydraulicBoundaryLocationId, norm, section,
                                                                         expectedRingProfilePoints, expectedRingForelandPoints, expectedRingBreakWater,
                                                                         modelFactorCriticalOvertopping,
                                                                         factorFbMean, factorFbStandardDeviation,
                                                                         factorFbLowerBoundary, factorFbUpperBoundary,
                                                                         factorFnMean, factorFnStandardDeviation,
                                                                         factorFnLowerBoundary, factorFnUpperBoundary,
                                                                         modelFactorOvertopping,
                                                                         modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                                                         modelFactorFrunupLowerBoundary, modelFactorFrunupUpperBoundary,
                                                                         exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation,
                                                                         exponentModelFactorShallowLowerBoundary, exponentModelFactorShallowUpperBoundary);

            // Assert
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(norm);
            Assert.IsInstanceOf<ReliabilityIndexCalculationInput>(input);
            Assert.AreEqual(9, input.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.HydraulicLoads, input.FailureMechanismType);
            Assert.IsNotNull(input.Section);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultHydraulicLoadsVariables().ToArray(), input.Variables.ToArray());
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
            var input = new HydraulicLoadsCalculationInputImplementation(1, 1.0/1000, section,
                                                                         new List<HydraRingRoughnessProfilePoint>(),
                                                                         new List<HydraRingForelandPoint>(),
                                                                         new HydraRingBreakWater(0, 1.1),
                                                                         2, 3, 4, 5, 6, 7, 8, 9, 10, 11,
                                                                         12, 13, 14, 15, 16, 17, 18, 19);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, input.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable> GetDefaultHydraulicLoadsVariables()
        {
            yield return new DeterministicHydraRingVariable(8, 1.1);
            yield return new TruncatedNormalHydraRingVariable(10, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5);
            yield return new TruncatedNormalHydraRingVariable(11, HydraRingDeviationType.Standard, 6.6, 7.7, 8.8, 9.9);
            yield return new DeterministicHydraRingVariable(12, 10.0);
            yield return new TruncatedNormalHydraRingVariable(120, HydraRingDeviationType.Standard, 11.1, 12.2, 13.3, 14.4);
            yield return new TruncatedNormalHydraRingVariable(123, HydraRingDeviationType.Standard, 15.5, 16.6, 17.7, 18.8);
        }

        private class HydraulicLoadsCalculationInputImplementation : HydraulicLoadsCalculationInput
        {
            public HydraulicLoadsCalculationInputImplementation(long hydraulicBoundaryLocationId, double norm,
                                                                HydraRingSection section,
                                                                IEnumerable<HydraRingRoughnessProfilePoint> profilePoints,
                                                                IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                                HydraRingBreakWater breakWater,
                                                                double modelFactorCriticalOvertopping,
                                                                double factorFbMean, double factorFbStandardDeviation,
                                                                double factorFbLowerBoundary, double factorFbUpperBoundary,
                                                                double factorFnMean, double factorFnStandardDeviation,
                                                                double factorFnLowerBoundary, double factorFnUpperBoundary,
                                                                double modelFactorOvertopping,
                                                                double modelFactorFrunupMean, double modelFactorFrunupStandardDeviation,
                                                                double modelFactorFrunupLowerBoundary, double modelFactorFrunupUpperBoundary,
                                                                double exponentModelFactorShallowMean, double exponentModelFactorShallowStandardDeviation,
                                                                double exponentModelFactorShallowLowerBoundary, double exponentModelFactorShallowUpperBoundary)
                : base(hydraulicBoundaryLocationId, norm,
                       section,
                       profilePoints,
                       forelandPoints,
                       breakWater,
                       modelFactorCriticalOvertopping,
                       factorFbMean, factorFbStandardDeviation,
                       factorFbLowerBoundary, factorFbUpperBoundary,
                       factorFnMean, factorFnStandardDeviation,
                       factorFnLowerBoundary, factorFnUpperBoundary,
                       modelFactorOvertopping,
                       modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                       modelFactorFrunupLowerBoundary, modelFactorFrunupUpperBoundary,
                       exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation,
                       exponentModelFactorShallowLowerBoundary, exponentModelFactorShallowUpperBoundary) {}

            public override int VariableId
            {
                get
                {
                    return -1;
                }
            }
        }
    }
}