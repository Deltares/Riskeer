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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresClosureLowSillCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            var section = new HydraRingSection(1, double.NaN, double.NaN);
            var forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 1.1);

            const double gravitationalAcceleration = 1.1;
            const double factorStormDurationOpenStructure = 2.2;
            const double failureProbabilityOpenStructure = 3.3;
            const double failureProbabilityReparation = 4.4;
            const int identicalApertures = 5;
            const double allowedLevelIncreaseStorageMean = 6.6;
            const double allowedLevelIncreaseStorageStandardDeviation = 7.7;
            const double modelFactorStorageVolumeMean = 8.8;
            const double modelFactorStorageVolumeStandardDeviation = 9.9;
            const double storageStructureAreaMean = 10.0;
            const double storageStructureAreaVariation = 11.1;
            const double modelFactorInflowVolume = 12.2;
            const double flowWidthAtBottomProtectionMean = 13.3;
            const double flowWidthAtBottomProtectionStandardDeviation = 14.4;
            const double criticalOvertoppingDischargeMean = 15.5;
            const double criticalOvertoppingDischargeVariation = 16.6;
            const double failureProbabilityStructureWithErosion = 17.7;
            const double stormDurationMean = 18.8;
            const double stormDurationVariation = 19.9;
            const double probabilityOpenStructureBeforeFlooding = 20.0;
            const double modelFactorSuperCriticalFlowMean = 21.1;
            const double modelFactorSuperCriticalFlowStandardDeviation = 22.2;
            const double modelFactorSubCriticalFlowMean = 23.3;
            const double modelFactorSubCriticalFlowVariation = 24.4;
            const double thresholdHeightOpenWeirMean = 25.5;
            const double thresholdHeightOpenWeirStandardDeviation = 26.6;
            const double insideWaterLevelMean = 27.7;
            const double insideWaterLevelStandardDeviation = 28.8;
            const double widthFlowAperturesMean = 29.9;
            const double widthFlowAperturesVariation = 30.0;

            // Call
            var input = new StructuresClosureLowSillCalculationInput(hydraulicBoundaryLocationId, section,
                                                                     forelandPoints, breakWater,
                                                                     gravitationalAcceleration,
                                                                     factorStormDurationOpenStructure,
                                                                     failureProbabilityOpenStructure,
                                                                     failureProbabilityReparation,
                                                                     identicalApertures,
                                                                     allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                                                                     modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                                                                     storageStructureAreaMean, storageStructureAreaVariation,
                                                                     modelFactorInflowVolume,
                                                                     flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                                                                     criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                                                                     failureProbabilityStructureWithErosion,
                                                                     stormDurationMean, stormDurationVariation,
                                                                     probabilityOpenStructureBeforeFlooding,
                                                                     modelFactorSuperCriticalFlowMean, modelFactorSuperCriticalFlowStandardDeviation,
                                                                     modelFactorSubCriticalFlowMean, modelFactorSubCriticalFlowVariation,
                                                                     thresholdHeightOpenWeirMean, thresholdHeightOpenWeirStandardDeviation,
                                                                     insideWaterLevelMean, insideWaterLevelStandardDeviation,
                                                                     widthFlowAperturesMean, widthFlowAperturesVariation);

            // Assert
            Assert.IsInstanceOf<StructuresClosureCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresClosure, input.FailureMechanismType);
            Assert.AreSame(section, input.Section);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
            Assert.AreSame(breakWater, input.BreakWater);
            HydraRingVariableAssert.AreEqual(GetDefaultVariables().ToArray(), input.Variables.ToArray());
        }

        [Test]
        [TestCase(423, null)]
        [TestCase(424, 106)]
        [TestCase(425, 111)]
        [TestCase(426, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Setup
            var input = new StructuresClosureLowSillCalculationInput(111, new HydraRingSection(1, double.NaN, double.NaN),
                                                                     Enumerable.Empty<HydraRingForelandPoint>(),
                                                                     new HydraRingBreakWater(1, 1.1),
                                                                     1.1, 222, 333, 5.5, 6, 7.7, 8.8, 11, 22, 33, 44, 55,
                                                                     66, 77, 88, 99, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
                                                                     22, 23, 24, 25);

            // Call
            int? actualSubmechanismModelId = input.GetSubMechanismModelId(subMechanismModelId);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, actualSubmechanismModelId);
        }

        private static IEnumerable<HydraRingVariable> GetDefaultVariables()
        {
            yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, 1.1, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(62, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 21.1, 22.2, double.NaN);
            yield return new HydraRingVariable(63, HydraRingDistributionType.Deterministic, 2.2, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(64, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 23.3, 24.4, double.NaN);
            yield return new HydraRingVariable(65, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 25.5, 26.6, double.NaN);
            yield return new HydraRingVariable(68, HydraRingDistributionType.Deterministic, 3.3, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(69, HydraRingDistributionType.Deterministic, 4.4, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(71, HydraRingDistributionType.Deterministic, 5, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(93, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 27.7, 28.8, double.NaN);
            yield return new HydraRingVariable(94, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 6.6, 7.7, double.NaN);
            yield return new HydraRingVariable(95, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 8.8, 9.9, double.NaN);
            yield return new HydraRingVariable(96, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 10.0, 11.1, double.NaN);
            yield return new HydraRingVariable(97, HydraRingDistributionType.Deterministic, 12.2, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(103, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 13.3, 14.4, double.NaN);
            yield return new HydraRingVariable(104, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 15.5, 16.6, double.NaN);
            yield return new HydraRingVariable(105, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 17.7, 0.0, double.NaN);
            yield return new HydraRingVariable(106, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 29.9, 30.0, double.NaN);
            yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 18.8, 19.9, double.NaN);
            yield return new HydraRingVariable(129, HydraRingDistributionType.Deterministic, 20.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
        }
    }
}