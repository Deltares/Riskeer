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

            const double volumicWeightWater = 9.81;
            const double gravitationalAcceleration = 9.81;
            const double levelCrestStructureMean = 1.1;
            const double levelCrestStructureStandardDeviation = 0.05;
            const double structureNormalOrientation = 2.2;
            const double factorStormDurationOpenStructure = 3.3;
            const double modelFactorSubCriticalFlowMean = 1.0;
            const double modelFactorSubCriticalFlowVariation = 0.1;
            const double thresholdHeightOpenWeirMean = 4.4;
            const double thresholdHeightOpenWeirStandardDeviation = 0.1;
            const double insideWaterLevelFailureConstructionMean = 5.5;
            const double insideWaterLevelFailureConstructionStandardDeviation = 0.1;
            const double failureProbabilityRepairClosure = 6.6;
            const double failureCollisionEnergyMean = 7.7;
            const double failureCollisionEnergyVariation = 0.3;
            const double modelFactorCollisionLoadMean = 1.0;
            const double modelFactorCollisionLoadVariation = 0.2;
            const double shipMassMean = 8.8;
            const double shipMassVariation = 0.2;
            const double shipVelocityMean = 9.9;
            const double shipVelocityVariation = 0.2;
            const double levelingCount = 10.10;
            const double probabilityCollisionSecondaryStructure = 11.11;
            const double flowVelocityStructureClosableMean = 12.12;
            const double flowVelocityStructureClosableStandardDeviation = 1.0;
            const double insideWaterLevelMean = 13.13;
            const double insideWaterLevelStandardDeviation = 0.1;
            const double allowedLevelIncreaseStorageMean = 14.14;
            const double allowedLevelIncreaseStorageStandardDeviation = 0.1;
            const double modelFactorStorageVolumeMean = 1.0;
            const double modelFactorStorageVolumeStandardDeviation = 0.2;
            const double storageStructureAreaMean = 15.15;
            const double storageStructureAreaVariation = 0.1;
            const double modelFactorInflowVolume = 1.0;
            const double flowWidthAtBottomProtectionMean = 16.16;
            const double flowWidthAtBottomProtectionStandardDeviation = 0.05;
            const double criticalOvertoppingDischargeMean = 17.17;
            const double criticalOvertoppingDischargeVariation = 0.15;
            const double failureProbabilityStructureWithErosion = 18.18;
            const double stormDurationMean = 7.5;
            const double stormDurationVariation = 0.25;
            const double bermWidthMean = 18.18;
            const double bermWidthStandardDeviation = 19.19;
            const double evaluationLevel = 0.0;
            const double modelFactorLoadEffectMean = 1.0;
            const double modelFactorLoadEffectStandardDeviation = 0.05;
            const double waveRatioMaxHMean = 5000;
            const double waveRatioMaxHStandardDeviation = 0.5;
            const double verticalDistance = 20.20;
            const double modificationFactorWavesSlowlyVaryingPressureComponent = 1.0;
            const double modificationFactorDynamicOrImpulsivePressureComponent = 1.0;

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
                                                                         levelingCount,
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
                                                                         bermWidthMean, bermWidthStandardDeviation,
                                                                         evaluationLevel,
                                                                         modelFactorLoadEffectMean, modelFactorLoadEffectStandardDeviation,
                                                                         waveRatioMaxHMean, waveRatioMaxHStandardDeviation,
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
            HydraRingVariableAssert.AreEqual(GetDefaultVariables().ToArray(), input.Variables.ToArray());
        }

        private static IEnumerable<HydraRingVariable> GetDefaultVariables()
        {
            yield return new HydraRingVariable(43, HydraRingDistributionType.Deterministic, 9.81, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, 9.81, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(60, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1.1, 0.05, double.NaN);
            yield return new HydraRingVariable(61, HydraRingDistributionType.Deterministic, 2.2, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(63, HydraRingDistributionType.Deterministic, 3.3, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(64, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 1.0, 0.1, double.NaN);
            yield return new HydraRingVariable(65, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 4.4, 0.1, double.NaN);
            yield return new HydraRingVariable(82, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 5.5, 0.1, double.NaN);
            yield return new HydraRingVariable(85, HydraRingDistributionType.Deterministic, 6.6, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(86, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 7.7, 0.3, double.NaN);
            yield return new HydraRingVariable(87, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 1.0, 0.2, double.NaN);
            yield return new HydraRingVariable(88, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 8.8, 0.2, double.NaN);
            yield return new HydraRingVariable(89, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 9.9, 0.2, double.NaN);
            yield return new HydraRingVariable(90, HydraRingDistributionType.Deterministic, 10.10, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(91, HydraRingDistributionType.Deterministic, 11.11, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(92, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 12.12, 1.0, double.NaN);
            yield return new HydraRingVariable(93, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 13.13, 0.1, double.NaN);
            yield return new HydraRingVariable(94, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 14.14, 0.1, double.NaN);
            yield return new HydraRingVariable(95, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 1.0, 0.2, double.NaN);
            yield return new HydraRingVariable(96, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 15.15, 0.1, double.NaN);
            yield return new HydraRingVariable(97, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(103, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 16.16, 0.05, double.NaN);
            yield return new HydraRingVariable(104, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 17.17, 0.15, double.NaN);
            yield return new HydraRingVariable(105, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 18.18, 0.0, double.NaN);
            yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 7.5, 0.25, double.NaN);
            yield return new HydraRingVariable(130, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 18.18, 19.19, double.NaN);
            yield return new HydraRingVariable(131, HydraRingDistributionType.Deterministic, 0.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(132, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1.0, 0.05, double.NaN);
            yield return new HydraRingVariable(133, HydraRingDistributionType.RayleighN, double.NaN, HydraRingDeviationType.Standard, 5000, 0.5, double.NaN);
            yield return new HydraRingVariable(134, HydraRingDistributionType.Deterministic, 20.20, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(135, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(136, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
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
                                                                double levelingCount,
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
                                                                double bermWidthMean, double bermWidthStandardDeviation,
                                                                double evaluationLevel,
                                                                double modelFactorLoadEffectMean, double modelFactorLoadEffectStandardDeviation,
                                                                double waveRatioMaxHMean, double waveRatioMaxHStandardDeviation,
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
                       levelingCount,
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
                       bermWidthMean, bermWidthStandardDeviation,
                       evaluationLevel,
                       modelFactorLoadEffectMean, modelFactorLoadEffectStandardDeviation,
                       waveRatioMaxHMean, waveRatioMaxHStandardDeviation,
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