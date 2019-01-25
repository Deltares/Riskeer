// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.Data.Variables;

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
            IEnumerable<HydraRingForelandPoint> forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 1.1);

            const double sectionNormal = 52.2;
            const double volumicWeightWater = 1.1;
            const double gravitationalAcceleration = 2.2;
            const double levelCrestStructureMean = 3.3;
            const double levelCrestStructureStandardDeviation = 4.4;
            const double structureNormalOrientation = 5.5;
            const double factorStormDurationOpenStructure = 6.6;
            const double thresholdHeightOpenWeirMean = 7.7;
            const double thresholdHeightOpenWeirStandardDeviation = 8.8;
            const double insideWaterLevelFailureConstructionMean = 9.9;
            const double insideWaterLevelFailureConstructionStandardDeviation = 10.10;
            const double failureProbabilityRepairClosure = 11.11;
            const double failureCollisionEnergyMean = 12.12;
            const double failureCollisionEnergyVariation = 13.13;
            const double modelFactorCollisionLoadMean = 14.14;
            const double modelFactorCollisionLoadVariation = 15.15;
            const double shipMassMean = 16.16;
            const double shipMassVariation = 17.17;
            const double shipVelocityMean = 18.18;
            const double shipVelocityVariation = 19.19;
            const int levellingCount = 20;
            const double probabilityCollisionSecondaryStructure = 21.21;
            const double flowVelocityStructureClosableMean = 22.22;
            const double flowVelocityStructureClosableVariation = 23.23;
            const double insideWaterLevelMean = 24.24;
            const double insideWaterLevelStandardDeviation = 25.25;
            const double allowedLevelIncreaseStorageMean = 26.26;
            const double allowedLevelIncreaseStorageStandardDeviation = 27.27;
            const double modelFactorStorageVolumeMean = 28.28;
            const double modelFactorStorageVolumeStandardDeviation = 29.29;
            const double storageStructureAreaMean = 30.30;
            const double storageStructureAreaVariation = 31.31;
            const double modelFactorInflowVolume = 32.32;
            const double flowWidthAtBottomProtectionMean = 33.33;
            const double flowWidthAtBottomProtectionStandardDeviation = 34.34;
            const double criticalOvertoppingDischargeMean = 35.35;
            const double criticalOvertoppingDischargeVariation = 36.36;
            const double failureProbabilityStructureWithErosion = 37.37;
            const double stormDurationMean = 38.38;
            const double stormDurationVariation = 39.39;
            const double modelFactorLongThresholdMean = 40.40;
            const double modelFactorLongThresholdStandardDeviation = 41.41;
            const double bankWidthMean = 42.42;
            const double bankWidthStandardDeviation = 43.43;
            const double evaluationLevel = 44.44;
            const double modelFactorLoadEffectMean = 45.45;
            const double modelFactorLoadEffectStandardDeviation = 46.46;
            const double waveRatioMaxHN = 47.47;
            const double waveRatioMaxHStandardDeviation = 48.48;
            const double verticalDistance = 49.49;
            const double modificationFactorWavesSlowlyVaryingPressureComponent = 50.50;
            const double modificationFactorDynamicOrImpulsivePressureComponent = 51.51;

            // Call
            var input = new TestStructuresStabilityPointCalculationInput(hydraulicBoundaryLocationId, sectionNormal,
                                                                         forelandPoints, breakWater,
                                                                         volumicWeightWater,
                                                                         gravitationalAcceleration,
                                                                         levelCrestStructureMean, levelCrestStructureStandardDeviation,
                                                                         structureNormalOrientation,
                                                                         factorStormDurationOpenStructure,
                                                                         thresholdHeightOpenWeirMean, thresholdHeightOpenWeirStandardDeviation,
                                                                         insideWaterLevelFailureConstructionMean, insideWaterLevelFailureConstructionStandardDeviation,
                                                                         failureProbabilityRepairClosure,
                                                                         failureCollisionEnergyMean, failureCollisionEnergyVariation,
                                                                         modelFactorCollisionLoadMean, modelFactorCollisionLoadVariation,
                                                                         shipMassMean, shipMassVariation,
                                                                         shipVelocityMean, shipVelocityVariation,
                                                                         levellingCount,
                                                                         probabilityCollisionSecondaryStructure,
                                                                         flowVelocityStructureClosableMean, flowVelocityStructureClosableVariation,
                                                                         insideWaterLevelMean, insideWaterLevelStandardDeviation,
                                                                         allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                                                                         modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                                                                         storageStructureAreaMean, storageStructureAreaVariation,
                                                                         modelFactorInflowVolume,
                                                                         flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                                                                         criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                                                                         failureProbabilityStructureWithErosion,
                                                                         stormDurationMean, stormDurationVariation,
                                                                         modelFactorLongThresholdMean, modelFactorLongThresholdStandardDeviation,
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
            Assert.AreEqual(6, input.IterationMethodId);

            HydraRingSection section = input.Section;
            Assert.AreEqual(1, section.SectionId);
            Assert.IsNaN(section.SectionLength);
            Assert.AreEqual(sectionNormal, section.CrossSectionNormal);
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
            yield return new NormalHydraRingVariable(65, HydraRingDeviationType.Standard, 7.7, 8.8);
            yield return new NormalHydraRingVariable(82, HydraRingDeviationType.Standard, 9.9, 10.10);
            yield return new DeterministicHydraRingVariable(85, 11.11);
            yield return new LogNormalHydraRingVariable(86, HydraRingDeviationType.Variation, 12.12, 13.13);
            yield return new NormalHydraRingVariable(87, HydraRingDeviationType.Variation, 14.14, 15.15);
            yield return new NormalHydraRingVariable(88, HydraRingDeviationType.Variation, 16.16, 17.17);
            yield return new NormalHydraRingVariable(89, HydraRingDeviationType.Variation, 18.18, 19.19);
            yield return new DeterministicHydraRingVariable(90, 20);
            yield return new DeterministicHydraRingVariable(91, 21.21);
            yield return new NormalHydraRingVariable(92, HydraRingDeviationType.Variation, 22.22, 23.23);
            yield return new NormalHydraRingVariable(93, HydraRingDeviationType.Standard, 24.24, 25.25);
            yield return new LogNormalHydraRingVariable(94, HydraRingDeviationType.Standard, 26.26, 27.27);
            yield return new LogNormalHydraRingVariable(95, HydraRingDeviationType.Standard, 28.28, 29.29);
            yield return new LogNormalHydraRingVariable(96, HydraRingDeviationType.Variation, 30.30, 31.31);
            yield return new DeterministicHydraRingVariable(97, 32.32);
            yield return new LogNormalHydraRingVariable(103, HydraRingDeviationType.Standard, 33.33, 34.34);
            yield return new LogNormalHydraRingVariable(104, HydraRingDeviationType.Variation, 35.35, 36.36);
            yield return new DeterministicHydraRingVariable(105, 37.37);
            yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, 38.38, 39.39);
            yield return new NormalHydraRingVariable(125, HydraRingDeviationType.Standard, 40.40, 41.41);
            yield return new NormalHydraRingVariable(130, HydraRingDeviationType.Standard, 42.42, 43.43);
            yield return new DeterministicHydraRingVariable(131, 44.44);
            yield return new NormalHydraRingVariable(132, HydraRingDeviationType.Standard, 45.45, 46.46);
            yield return new RayleighNHydraRingVariable(133, HydraRingDeviationType.Standard, 47.47, 48.48);
            yield return new DeterministicHydraRingVariable(134, 49.49);
            yield return new DeterministicHydraRingVariable(135, 50.50);
            yield return new DeterministicHydraRingVariable(136, 51.51);
        }

        private class TestStructuresStabilityPointCalculationInput : StructuresStabilityPointCalculationInput
        {
            public TestStructuresStabilityPointCalculationInput(long hydraulicBoundaryLocationId, double sectionNormal,
                                                                IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                                HydraRingBreakWater breakWater,
                                                                double volumicWeightWater,
                                                                double gravitationalAcceleration,
                                                                double levelCrestStructureMean, double levelCrestStructureStandardDeviation,
                                                                double structureNormalOrientation,
                                                                double factorStormDurationOpenStructure,
                                                                double thresholdHeightOpenWeirMean, double thresholdHeightOpenWeirStandardDeviation,
                                                                double insideWaterLevelFailureConstructionMean, double insideWaterLevelFailureConstructionStandardDeviation,
                                                                double failureProbabilityRepairClosure,
                                                                double failureCollisionEnergyMean, double failureCollisionEnergyVariation,
                                                                double modelFactorCollisionLoadMean, double modelFactorCollisionLoadVariation,
                                                                double shipMassMean, double shipMassVariation,
                                                                double shipVelocityMean, double shipVelocityVariation,
                                                                int levellingCount,
                                                                double probabilityCollisionSecondaryStructure,
                                                                double flowVelocityStructureClosableMean, double flowVelocityStructureClosableVariation,
                                                                double insideWaterLevelMean, double insideWaterLevelStandardDeviation,
                                                                double allowedLevelIncreaseStorageMean, double allowedLevelIncreaseStorageStandardDeviation,
                                                                double modelFactorStorageVolumeMean, double modelFactorStorageVolumeStandardDeviation,
                                                                double storageStructureAreaMean, double storageStructureAreaVariation,
                                                                double modelFactorInflowVolume,
                                                                double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                                                                double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeVariation,
                                                                double failureProbabilityStructureWithErosion,
                                                                double stormDurationMean, double stormDurationVariation,
                                                                double modelFactorLongThresholdMean, double modelFactorLongThresholdStandardDeviation,
                                                                double bankWidthMean, double bankWidthStandardDeviation,
                                                                double evaluationLevel,
                                                                double modelFactorLoadEffectMean, double modelFactorLoadEffectStandardDeviation,
                                                                double waveRatioMaxHN, double waveRatioMaxHStandardDeviation,
                                                                double verticalDistance,
                                                                double modificationFactorWavesSlowlyVaryingPressureComponent,
                                                                double modificationFactorDynamicOrImpulsivePressureComponent)
                : base(hydraulicBoundaryLocationId,
                       sectionNormal,
                       forelandPoints, breakWater,
                       volumicWeightWater,
                       gravitationalAcceleration,
                       levelCrestStructureMean, levelCrestStructureStandardDeviation,
                       structureNormalOrientation,
                       factorStormDurationOpenStructure,
                       thresholdHeightOpenWeirMean, thresholdHeightOpenWeirStandardDeviation,
                       insideWaterLevelFailureConstructionMean, insideWaterLevelFailureConstructionStandardDeviation,
                       failureProbabilityRepairClosure,
                       failureCollisionEnergyMean, failureCollisionEnergyVariation,
                       modelFactorCollisionLoadMean, modelFactorCollisionLoadVariation,
                       shipMassMean, shipMassVariation,
                       shipVelocityMean, shipVelocityVariation,
                       levellingCount,
                       probabilityCollisionSecondaryStructure,
                       flowVelocityStructureClosableMean, flowVelocityStructureClosableVariation,
                       insideWaterLevelMean, insideWaterLevelStandardDeviation,
                       allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                       modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                       storageStructureAreaMean, storageStructureAreaVariation,
                       modelFactorInflowVolume,
                       flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                       criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                       failureProbabilityStructureWithErosion,
                       stormDurationMean, stormDurationVariation,
                       modelFactorLongThresholdMean, modelFactorLongThresholdStandardDeviation,
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