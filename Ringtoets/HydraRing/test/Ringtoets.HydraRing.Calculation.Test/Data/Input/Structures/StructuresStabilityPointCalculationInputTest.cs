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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresStabilityPointCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            var section = new HydraRingSection(1, double.NaN, double.NaN);
            var forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 1.1);

            const double volumicWeightWater = 1.1;
            const double gravitationalAcceleration = 2.2;
            const double levelCrestStructureMean = 3.3;
            const double levelCrestStructureStandardDeviation = 4.4;
            const double structureNormalOrientation = 5.5;
            const double factorStormDurationOpenStructure = 6.6;
            const double modelFactorSubCriticalFlowMean = 7.7;
            const double modelFactorSubCriticalFlowVariation = 8.8;
            const double thresholdHeightOpenWeirMean = 9.9;
            const double thresholdHeightOpenWeirStandardDeviation = 10.0;
            const double insideWaterLevelFailureConstructionMean = 11.1;
            const double insideWaterLevelFailureConstructionStandardDeviation = 12.2;
            const double failureProbabilityRepairClosure = 13.3;
            const double failureCollisionEnergyMean = 14.4;
            const double failureCollisionEnergyVariation = 15.5;
            const double modelFactorCollisionLoadMean = 16.6;
            const double modelFactorCollisionLoadVariation = 17.7;
            const double shipMassMean = 18.8;
            const double shipMassVariation = 19.9;
            const double shipVelocityMean = 20.0;
            const double shipVelocityVariation = 21.1;
            const int levellingCount = 22;
            const double probabilityCollisionSecondaryStructure = 23.3;
            const double flowVelocityStructureClosableMean = 24.4;
            const double flowVelocityStructureClosableStandardDeviation = 25.5;
            const double insideWaterLevelMean = 26.6;
            const double insideWaterLevelStandardDeviation = 27.7;
            const double allowedLevelIncreaseStorageMean = 28.8;
            const double allowedLevelIncreaseStorageStandardDeviation = 29.9;
            const double modelFactorStorageVolumeMean = 30.0;
            const double modelFactorStorageVolumeStandardDeviation = 31.1;
            const double storageStructureAreaMean = 32.2;
            const double storageStructureAreaVariation = 33.3;
            const double modelFactorInflowVolume = 34.4;
            const double flowWidthAtBottomProtectionMean = 35.5;
            const double flowWidthAtBottomProtectionStandardDeviation = 36.6;
            const double criticalOvertoppingDischargeMean = 37.7;
            const double criticalOvertoppingDischargeVariation = 38.8;
            const double failureProbabilityStructureWithErosion = 39.9;
            const double stormDurationMean = 40.0;
            const double stormDurationVariation = 41.1;
            const double bankWidthMean = 42.2;
            const double bankWidthStandardDeviation = 43.3;
            const double evaluationLevel = 44.4;
            const double modelFactorLoadEffectMean = 45.5;
            const double modelFactorLoadEffectStandardDeviation = 46.6;
            const double waveRatioMaxHN = 47.7;
            const double waveRatioMaxHStandardDeviation = 48.8;
            const double verticalDistance = 49.9;
            const double modificationFactorWavesSlowlyVaryingPressureComponent = 50.0;
            const double modificationFactorDynamicOrImpulsivePressureComponent = 51.1;

            // Call
            var input = new TestStructuresStabilityPointCalculationInput(hydraulicBoundaryLocationId, section,
                                                                         forelandPoints, breakWater,
                                                                         volumicWeightWater,
                                                                         gravitationalAcceleration,
                                                                         levelCrestStructureMean, levelCrestStructureStandardDeviation,
                                                                         structureNormalOrientation,
                                                                         factorStormDurationOpenStructure,
                                                                         modelFactorSubCriticalFlowMean, modelFactorSubCriticalFlowVariation,
                                                                         thresholdHeightOpenWeirMean, thresholdHeightOpenWeirStandardDeviation,
                                                                         insideWaterLevelFailureConstructionMean, insideWaterLevelFailureConstructionStandardDeviation,
                                                                         failureProbabilityRepairClosure,
                                                                         failureCollisionEnergyMean, failureCollisionEnergyVariation,
                                                                         modelFactorCollisionLoadMean, modelFactorCollisionLoadVariation,
                                                                         shipMassMean, shipMassVariation,
                                                                         shipVelocityMean, shipVelocityVariation,
                                                                         levellingCount,
                                                                         probabilityCollisionSecondaryStructure,
                                                                         flowVelocityStructureClosableMean, flowVelocityStructureClosableStandardDeviation,
                                                                         insideWaterLevelMean, insideWaterLevelStandardDeviation,
                                                                         allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                                                                         modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                                                                         storageStructureAreaMean, storageStructureAreaVariation,
                                                                         modelFactorInflowVolume,
                                                                         flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                                                                         criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                                                                         failureProbabilityStructureWithErosion,
                                                                         stormDurationMean, stormDurationVariation,
                                                                         bankWidthMean, bankWidthStandardDeviation,
                                                                         evaluationLevel,
                                                                         modelFactorLoadEffectMean, modelFactorLoadEffectStandardDeviation,
                                                                         waveRatioMaxHN, waveRatioMaxHStandardDeviation,
                                                                         verticalDistance,
                                                                         modificationFactorWavesSlowlyVaryingPressureComponent,
                                                                         modificationFactorDynamicOrImpulsivePressureComponent);

            // Assert
            Assert.IsInstanceOf<ExceedanceProbabilityCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresStructuralFailure, input.FailureMechanismType);
            Assert.AreSame(section, input.Section);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
            Assert.AreSame(breakWater, input.BreakWater);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultVariables().ToArray(), input.Variables.ToArray());
        }

        private static IEnumerable<HydraRingVariable> GetDefaultVariables()
        {
            yield return new DeterministicHydraRingVariable(43, 1.1);
            yield return new DeterministicHydraRingVariable(58, 2.2);
            yield return new NormalHydraRingVariable(60, HydraRingDeviationType.Standard, 3.3, 4.4);
            yield return new DeterministicHydraRingVariable(61, 5.5);
            yield return new DeterministicHydraRingVariable(63, 6.6);
            yield return new NormalHydraRingVariable(64, HydraRingDeviationType.Variation, 7.7, 8.8);
            yield return new NormalHydraRingVariable(65, HydraRingDeviationType.Standard, 9.9, 10.0);
            yield return new NormalHydraRingVariable(82, HydraRingDeviationType.Standard, 11.1, 12.2);
            yield return new DeterministicHydraRingVariable(85, 13.3);
            yield return new LogNormalHydraRingVariable(86, HydraRingDeviationType.Variation, 14.4, 15.5);
            yield return new NormalHydraRingVariable(87, HydraRingDeviationType.Variation, 16.6, 17.7);
            yield return new NormalHydraRingVariable(88, HydraRingDeviationType.Variation, 18.8, 19.9);
            yield return new NormalHydraRingVariable(89, HydraRingDeviationType.Variation, 20.0, 21.1);
            yield return new DeterministicHydraRingVariable(90, 22);
            yield return new DeterministicHydraRingVariable(91, 23.3);
            yield return new NormalHydraRingVariable(92, HydraRingDeviationType.Standard, 24.4, 25.5);
            yield return new NormalHydraRingVariable(93, HydraRingDeviationType.Standard, 26.6, 27.7);
            yield return new LogNormalHydraRingVariable(94, HydraRingDeviationType.Standard, 28.8, 29.9);
            yield return new LogNormalHydraRingVariable(95, HydraRingDeviationType.Standard, 30.0, 31.1);
            yield return new LogNormalHydraRingVariable(96, HydraRingDeviationType.Variation, 32.2, 33.3);
            yield return new DeterministicHydraRingVariable(97, 34.4);
            yield return new LogNormalHydraRingVariable(103, HydraRingDeviationType.Standard, 35.5, 36.6);
            yield return new LogNormalHydraRingVariable(104, HydraRingDeviationType.Variation, 37.7, 38.8);
            yield return new DeterministicHydraRingVariable(105, 39.9);
            yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, 40.0, 41.1);
            yield return new NormalHydraRingVariable(130, HydraRingDeviationType.Standard, 42.2, 43.3);
            yield return new DeterministicHydraRingVariable(131, 44.4);
            yield return new NormalHydraRingVariable(132, HydraRingDeviationType.Standard, 45.5, 46.6);
            yield return new RayleighNHydraRingVariable(133, HydraRingDeviationType.Standard, 48.8, 47.7);
            yield return new DeterministicHydraRingVariable(134, 49.9);
            yield return new DeterministicHydraRingVariable(135, 50.0);
            yield return new DeterministicHydraRingVariable(136, 51.1);
        }

        private class TestStructuresStabilityPointCalculationInput : StructuresStabilityPointCalculationInput
        {
            public TestStructuresStabilityPointCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection section,
                                                                IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                                HydraRingBreakWater breakWater,
                                                                double volumicWeightWater,
                                                                double gravitationalAcceleration,
                                                                double levelCrestStructureMean, double levelCrestStructureStandardDeviation,
                                                                double structureNormalOrientation,
                                                                double factorStormDurationOpenStructure,
                                                                double modelFactorSubCriticalFlowMean, double modelFactorSubCriticalFlowVariation,
                                                                double thresholdHeightOpenWeirMean, double thresholdHeightOpenWeirStandardDeviation,
                                                                double insideWaterLevelFailureConstructionMean, double insideWaterLevelFailureConstructionStandardDeviation,
                                                                double failureProbabilityRepairClosure,
                                                                double failureCollisionEnergyMean, double failureCollisionEnergyVariation,
                                                                double modelFactorCollisionLoadMean, double modelFactorCollisionLoadVariation,
                                                                double shipMassMean, double shipMassVariation,
                                                                double shipVelocityMean, double shipVelocityVariation,
                                                                int levellingCount,
                                                                double probabilityCollisionSecondaryStructure,
                                                                double flowVelocityStructureClosableMean, double flowVelocityStructureClosableStandardDeviation,
                                                                double insideWaterLevelMean, double insideWaterLevelStandardDeviation,
                                                                double allowedLevelIncreaseStorageMean, double allowedLevelIncreaseStorageStandardDeviation,
                                                                double modelFactorStorageVolumeMean, double modelFactorStorageVolumeStandardDeviation,
                                                                double storageStructureAreaMean, double storageStructureAreaVariation,
                                                                double modelFactorInflowVolume,
                                                                double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                                                                double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeVariation,
                                                                double failureProbabilityStructureWithErosion,
                                                                double stormDurationMean, double stormDurationVariation,
                                                                double bankWidthMean, double bankWidthStandardDeviation,
                                                                double evaluationLevel,
                                                                double modelFactorLoadEffectMean, double modelFactorLoadEffectStandardDeviation,
                                                                double waveRatioMaxHN, double waveRatioMaxHStandardDeviation,
                                                                double verticalDistance,
                                                                double modificationFactorWavesSlowlyVaryingPressureComponent,
                                                                double modificationFactorDynamicOrImpulsivePressureComponent)
                : base(hydraulicBoundaryLocationId, section,
                       forelandPoints, breakWater,
                       volumicWeightWater,
                       gravitationalAcceleration,
                       levelCrestStructureMean, levelCrestStructureStandardDeviation,
                       structureNormalOrientation,
                       factorStormDurationOpenStructure,
                       modelFactorSubCriticalFlowMean, modelFactorSubCriticalFlowVariation,
                       thresholdHeightOpenWeirMean, thresholdHeightOpenWeirStandardDeviation,
                       insideWaterLevelFailureConstructionMean, insideWaterLevelFailureConstructionStandardDeviation,
                       failureProbabilityRepairClosure,
                       failureCollisionEnergyMean, failureCollisionEnergyVariation,
                       modelFactorCollisionLoadMean, modelFactorCollisionLoadVariation,
                       shipMassMean, shipMassVariation,
                       shipVelocityMean, shipVelocityVariation,
                       levellingCount,
                       probabilityCollisionSecondaryStructure,
                       flowVelocityStructureClosableMean, flowVelocityStructureClosableStandardDeviation,
                       insideWaterLevelMean, insideWaterLevelStandardDeviation,
                       allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                       modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                       storageStructureAreaMean, storageStructureAreaVariation,
                       modelFactorInflowVolume,
                       flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                       criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                       failureProbabilityStructureWithErosion,
                       stormDurationMean, stormDurationVariation,
                       bankWidthMean, bankWidthStandardDeviation,
                       evaluationLevel,
                       modelFactorLoadEffectMean, modelFactorLoadEffectStandardDeviation,
                       waveRatioMaxHN, waveRatioMaxHStandardDeviation,
                       verticalDistance,
                       modificationFactorWavesSlowlyVaryingPressureComponent,
                       modificationFactorDynamicOrImpulsivePressureComponent) {}

            public override int? GetSubMechanismModelId(int subMechanismId)
            {
                throw new NotImplementedException();
            }
        }
    }
}