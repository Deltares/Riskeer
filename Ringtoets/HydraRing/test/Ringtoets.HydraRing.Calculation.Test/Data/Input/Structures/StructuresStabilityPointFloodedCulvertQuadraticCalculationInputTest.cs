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
            const double drainCoefficientMean = 1.0;
            const double drainCoefficientStandardDeviation = 0.2;
            const double areaFlowAperturesMean = 21.21;
            const double areaFlowAperturesStandardDeviation = 0.01;
            const double stabilityQuadraticLoadModelMean = 22.22;
            const double stabilityQuadraticLoadModelVariation = 0.1;
            const double constructiveStrengthQuadraticLoadModelMean = 23.23;
            const double constructiveStrengthQuadraticLoadModelVariation = 0.1;

            // Call
            var input = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(hydraulicBoundaryLocationId, section, forelandPoints,
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
                                                                                            modificationFactorDynamicOrImpulsivePressureComponent,
                                                                                            drainCoefficientMean, drainCoefficientStandardDeviation,
                                                                                            areaFlowAperturesMean, areaFlowAperturesStandardDeviation,
                                                                                            stabilityQuadraticLoadModelMean, stabilityQuadraticLoadModelVariation,
                                                                                            constructiveStrengthQuadraticLoadModelMean, constructiveStrengthQuadraticLoadModelVariation);

            // Assert
            Assert.IsInstanceOf<StructuresStabilityPointCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresStructuralFailure, input.FailureMechanismType);
            Assert.AreSame(section, input.Section);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
            HydraRingVariableAssert.AreEqual(GetDefaultVariables().ToArray(), input.Variables.ToArray());
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
                                                                                            1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9, 10.10,
                                                                                            11.11, 12.12, 13.13, 14.14, 15.15, 16.16, 17.17,
                                                                                            18.18, 19.19, 20.20, 21.21, 22.22, 23.23, 24.24,
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

        private static IEnumerable<HydraRingVariable> GetDefaultVariables()
        {
            yield return new HydraRingVariable(43, HydraRingDistributionType.Deterministic, 9.81, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, 9.81, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(60, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1.1, 0.05, double.NaN);
            yield return new HydraRingVariable(61, HydraRingDistributionType.Deterministic, 2.2, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(63, HydraRingDistributionType.Deterministic, 3.3, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(64, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 1.0, 0.1, double.NaN);
            yield return new HydraRingVariable(65, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 4.4, 0.1, double.NaN);
            yield return new HydraRingVariable(66, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1.0, 0.2, double.NaN);
            yield return new HydraRingVariable(67, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 21.21, 0.01, double.NaN);
            yield return new HydraRingVariable(81, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 22.22, 0.1, double.NaN);
            yield return new HydraRingVariable(82, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 5.5, 0.1, double.NaN);
            yield return new HydraRingVariable(84, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 23.23, 0.1, double.NaN);
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
    }
}