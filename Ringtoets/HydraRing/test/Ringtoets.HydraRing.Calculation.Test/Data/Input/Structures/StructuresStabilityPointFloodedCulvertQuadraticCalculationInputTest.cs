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
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresStabilityPointFloodedCulvertQuadraticCalculationInputTest
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
            const double drainCoefficientMean = 52.2;
            const double drainCoefficientStandardDeviation = 53.3;
            const double areaFlowAperturesMean = 54.4;
            const double areaFlowAperturesStandardDeviation = 55.5;
            const double constructiveStrengthQuadraticLoadModelMean = 56.6;
            const double constructiveStrengthQuadraticLoadModelVariation = 57.7;
            const double stabilityQuadraticLoadModelMean = 58.8;
            const double stabilityQuadraticLoadModelVariation = 59.9;

            // Call
            var input = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(hydraulicBoundaryLocationId, section,
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
                                                                                            modificationFactorDynamicOrImpulsivePressureComponent,
                                                                                            drainCoefficientMean, drainCoefficientStandardDeviation,
                                                                                            areaFlowAperturesMean, areaFlowAperturesStandardDeviation,
                                                                                            constructiveStrengthQuadraticLoadModelMean, constructiveStrengthQuadraticLoadModelVariation,
                                                                                            stabilityQuadraticLoadModelMean, stabilityQuadraticLoadModelVariation);

            // Assert
            Assert.IsInstanceOf<StructuresStabilityPointCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresStructuralFailure, input.FailureMechanismType);
            Assert.AreSame(section, input.Section);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
            Assert.AreSame(breakWater, input.BreakWater);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultVariables().ToArray(), input.NewVariables.ToArray());
        }

        [Test]
        [TestCase(423, null)]
        [TestCase(424, 107)]
        [TestCase(425, 113)]
        [TestCase(430, 115)]
        [TestCase(435, 117)]
        [TestCase(436, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Setup
            var input = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(111, new HydraRingSection(1, double.NaN, double.NaN),
                                                                                            Enumerable.Empty<HydraRingForelandPoint>(),
                                                                                            new HydraRingBreakWater(1, 1.1),
                                                                                            1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9, 10.10,
                                                                                            11.11, 12.12, 13.13, 14.14, 15.15, 16.16, 17.17,
                                                                                            18.18, 19.19, 20.20, 21.21, 22, 23.23, 24.24,
                                                                                            25.25, 26.26, 27.27, 28.28, 29.29, 30.30, 31.31,
                                                                                            32.32, 33.33, 34.34, 35.35, 36.36, 37.37, 38.38,
                                                                                            39.39, 40.40, 41.41, 42.42, 43.43, 44.44, 45.45,
                                                                                            46.46, 47.47, 48.48, 49.49, 50.50, 51.51, 52.52,
                                                                                            53.53, 54.54, 55.55, 56.56, 57.57, 58.58, 59.59);

            // Call
            int? actualSubmechanismModelId = input.GetSubMechanismModelId(subMechanismModelId);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, actualSubmechanismModelId);
        }

        private static IEnumerable<HydraRingVariable2> GetDefaultVariables()
        {
            yield return new DeterministicHydraRingVariable(43, 1.1);
            yield return new DeterministicHydraRingVariable(58, 2.2);
            yield return new NormalHydraRingVariable(60, HydraRingDeviationType.Standard, 3.3, 4.4);
            yield return new DeterministicHydraRingVariable(61, 5.5);
            yield return new DeterministicHydraRingVariable(63, 6.6);
            yield return new NormalHydraRingVariable(64, HydraRingDeviationType.Variation, 7.7, 8.8);
            yield return new NormalHydraRingVariable(65, HydraRingDeviationType.Standard, 9.9, 10.0);
            yield return new NormalHydraRingVariable(66, HydraRingDeviationType.Standard, 52.2, 53.3);
            yield return new LogNormalHydraRingVariable(67, HydraRingDeviationType.Standard, 54.4, 55.5);
            yield return new LogNormalHydraRingVariable(81, HydraRingDeviationType.Variation, 56.6, 57.7);
            yield return new NormalHydraRingVariable(82, HydraRingDeviationType.Standard, 11.1, 12.2);
            yield return new LogNormalHydraRingVariable(84, HydraRingDeviationType.Variation, 58.8, 59.9);
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
    }
}