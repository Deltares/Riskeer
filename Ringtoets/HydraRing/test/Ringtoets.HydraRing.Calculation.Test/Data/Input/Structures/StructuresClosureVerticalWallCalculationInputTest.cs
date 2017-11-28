﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Variables;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresClosureVerticalWallCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            IEnumerable<HydraRingForelandPoint> forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 1.1);

            const double sectionNormal = 31.1;
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
            const double probabilityOrFrequencyOpenStructureBeforeFlooding = 20.0;
            const double modelFactorOvertoppingFlowMean = 21.1;
            const double modelFactorOvertoppingFlowStandardDeviation = 22.2;
            const double structureNormalOrientation = 23.3;
            const double modelFactorSuperCriticalFlowMean = 24.4;
            const double modelFactorSuperCriticalFlowStandardDeviation = 25.5;
            const double levelCrestStructureNotClosingMean = 26.6;
            const double levelCrestStructureNotClosingStandardDeviation = 27.7;
            const double widthFlowAperturesMean = 28.8;
            const double widthFlowAperturesStandardDeviation = 29.9;
            const double deviationWaveDirection = 30.0;

            // Call
            var input = new StructuresClosureVerticalWallCalculationInput(hydraulicBoundaryLocationId,
                                                                          sectionNormal,
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
                                                                          probabilityOrFrequencyOpenStructureBeforeFlooding,
                                                                          modelFactorOvertoppingFlowMean, modelFactorOvertoppingFlowStandardDeviation,
                                                                          structureNormalOrientation,
                                                                          modelFactorSuperCriticalFlowMean, modelFactorSuperCriticalFlowStandardDeviation,
                                                                          levelCrestStructureNotClosingMean, levelCrestStructureNotClosingStandardDeviation,
                                                                          widthFlowAperturesMean, widthFlowAperturesStandardDeviation,
                                                                          deviationWaveDirection);

            // Assert
            Assert.IsInstanceOf<StructuresClosureCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresClosure, input.FailureMechanismType);

            HydraRingSection section = input.Section;
            Assert.AreEqual(1, section.SectionId);
            Assert.IsNaN(section.SectionLength);
            Assert.AreEqual(sectionNormal, section.CrossSectionNormal);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
            Assert.AreSame(breakWater, input.BreakWater);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultVariables().ToArray(), input.Variables.ToArray());
        }

        [Test]
        [TestCase(423, null)]
        [TestCase(424, 105)]
        [TestCase(425, 109)]
        [TestCase(426, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Setup
            var input = new StructuresClosureVerticalWallCalculationInput(111, double.NaN,
                                                                          Enumerable.Empty<HydraRingForelandPoint>(),
                                                                          new HydraRingBreakWater(1, 1.1),
                                                                          1.1, 222, 333, 5.5, 6, 7.7, 8.8, 11, 22, 33, 44, 55,
                                                                          66, 77, 88, 99, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
                                                                          22, 23, 24, 25);

            // Call
            int? actualSubMechanismModelId = input.GetSubMechanismModelId(subMechanismModelId);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, actualSubMechanismModelId);
        }

        private static IEnumerable<HydraRingVariable> GetDefaultVariables()
        {
            yield return new DeterministicHydraRingVariable(58, 1.1);
            yield return new LogNormalHydraRingVariable(59, HydraRingDeviationType.Standard, 21.1, 22.2);
            yield return new DeterministicHydraRingVariable(61, 23.3);
            yield return new NormalHydraRingVariable(62, HydraRingDeviationType.Standard, 24.4, 25.5);
            yield return new DeterministicHydraRingVariable(63, 2.2);
            yield return new DeterministicHydraRingVariable(68, 3.3);
            yield return new DeterministicHydraRingVariable(69, 4.4);
            yield return new DeterministicHydraRingVariable(71, 5);
            yield return new NormalHydraRingVariable(72, HydraRingDeviationType.Standard, 26.6, 27.7);
            yield return new LogNormalHydraRingVariable(94, HydraRingDeviationType.Standard, 6.6, 7.7);
            yield return new LogNormalHydraRingVariable(95, HydraRingDeviationType.Standard, 8.8, 9.9);
            yield return new LogNormalHydraRingVariable(96, HydraRingDeviationType.Variation, 10.0, 11.1);
            yield return new DeterministicHydraRingVariable(97, 12.2);
            yield return new LogNormalHydraRingVariable(103, HydraRingDeviationType.Standard, 13.3, 14.4);
            yield return new LogNormalHydraRingVariable(104, HydraRingDeviationType.Variation, 15.5, 16.6);
            yield return new DeterministicHydraRingVariable(105, 17.7);
            yield return new NormalHydraRingVariable(106, HydraRingDeviationType.Standard, 28.8, 29.9);
            yield return new DeterministicHydraRingVariable(107, 30.0);
            yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, 18.8, 19.9);
            yield return new DeterministicHydraRingVariable(129, 20.0);
        }
    }
}