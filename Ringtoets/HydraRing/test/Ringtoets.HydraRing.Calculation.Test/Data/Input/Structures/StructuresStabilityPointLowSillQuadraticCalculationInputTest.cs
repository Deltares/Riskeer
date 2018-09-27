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
    public class StructuresStabilityPointLowSillQuadraticCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            IEnumerable<HydraRingForelandPoint> forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 1.1);

            const double sectionNormal = 60.0;
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
            const double bankWidthMean = 40.40;
            const double bankWidthStandardDeviation = 41.41;
            const double evaluationLevel = 42.42;
            const double modelFactorLoadEffectMean = 43.43;
            const double modelFactorLoadEffectStandardDeviation = 44.44;
            const double waveRatioMaxHN = 45.45;
            const double waveRatioMaxHStandardDeviation = 46.46;
            const double verticalDistance = 47.47;
            const double modificationFactorWavesSlowlyVaryingPressureComponent = 48.48;
            const double modificationFactorDynamicOrImpulsivePressureComponent = 49.49;
            const double constructiveStrengthQuadraticLoadModelMean = 50.50;
            const double constructiveStrengthQuadraticLoadModelVariation = 51.51;
            const double stabilityQuadraticLoadModelMean = 52.52;
            const double stabilityQuadraticLoadModelVariation = 53.53;
            const double widthFlowAperturesMean = 54.54;
            const double widthFlowAperturesStandardDeviation = 55.55;
            const double modelFactorLongThresholdMean = 56.56;
            const double modelFactorLongThresholdStandardDeviation = 57.57;

            // Call
            var input = new StructuresStabilityPointLowSillQuadraticCalculationInput(hydraulicBoundaryLocationId, sectionNormal,
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
                                                                                     bankWidthMean, bankWidthStandardDeviation,
                                                                                     evaluationLevel,
                                                                                     modelFactorLoadEffectMean, modelFactorLoadEffectStandardDeviation,
                                                                                     waveRatioMaxHN, waveRatioMaxHStandardDeviation,
                                                                                     verticalDistance,
                                                                                     modificationFactorWavesSlowlyVaryingPressureComponent,
                                                                                     modificationFactorDynamicOrImpulsivePressureComponent,
                                                                                     constructiveStrengthQuadraticLoadModelMean, constructiveStrengthQuadraticLoadModelVariation,
                                                                                     stabilityQuadraticLoadModelMean, stabilityQuadraticLoadModelVariation,
                                                                                     widthFlowAperturesMean, widthFlowAperturesStandardDeviation,
                                                                                     modelFactorLongThresholdMean, modelFactorLongThresholdStandardDeviation);

            // Assert
            Assert.IsInstanceOf<StructuresStabilityPointCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresStructuralFailure, input.FailureMechanismType);

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
        [TestCase(424, 106)]
        [TestCase(425, 111)]
        [TestCase(430, 115)]
        [TestCase(435, 117)]
        [TestCase(436, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Setup
            var input = new StructuresStabilityPointLowSillQuadraticCalculationInput(111, double.NaN,
                                                                                     Enumerable.Empty<HydraRingForelandPoint>(),
                                                                                     new HydraRingBreakWater(1, 1.1),
                                                                                     1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8, 9.9, 10.10,
                                                                                     11.11, 12.12, 13.13, 14.14, 15.15, 16.16, 17.17,
                                                                                     18.18, 19.19, 20, 21.21, 22.22, 23.23, 24.24,
                                                                                     25.25, 26.26, 27.27, 28.28, 29.29, 30.30, 31.31,
                                                                                     32.32, 33.33, 34.34, 35.35, 36.36, 37.37, 38.38,
                                                                                     39.39, 40.40, 41.41, 42.42, 43.43, 44.44, 45.45,
                                                                                     46.46, 47.47, 48.48, 49.49, 50.50, 51.51, 52.52,
                                                                                     53.53, 54.54, 55.55, 56.56, 57.57);

            // Call
            int? actualSubMechanismModelId = input.GetSubMechanismModelId(subMechanismModelId);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, actualSubMechanismModelId);
        }

        private static IEnumerable<HydraRingVariable> GetDefaultVariables()
        {
            yield return new DeterministicHydraRingVariable(43, 1.1);
            yield return new DeterministicHydraRingVariable(58, 2.2);
            yield return new NormalHydraRingVariable(60, HydraRingDeviationType.Standard, 3.3, 4.4);
            yield return new DeterministicHydraRingVariable(61, 5.5);
            yield return new DeterministicHydraRingVariable(63, 6.6);
            yield return new NormalHydraRingVariable(65, HydraRingDeviationType.Standard, 7.7, 8.8);
            yield return new LogNormalHydraRingVariable(81, HydraRingDeviationType.Variation, 50.50, 51.51);
            yield return new NormalHydraRingVariable(82, HydraRingDeviationType.Standard, 9.9, 10.10);
            yield return new LogNormalHydraRingVariable(84, HydraRingDeviationType.Variation, 52.52, 53.53);
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
            yield return new NormalHydraRingVariable(106, HydraRingDeviationType.Standard, 54.54, 55.55);
            yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, 38.38, 39.39);
            yield return new NormalHydraRingVariable(125, HydraRingDeviationType.Standard, 56.56, 57.57);
            yield return new NormalHydraRingVariable(130, HydraRingDeviationType.Standard, 40.40, 41.41);
            yield return new DeterministicHydraRingVariable(131, 42.42);
            yield return new NormalHydraRingVariable(132, HydraRingDeviationType.Standard, 43.43, 44.44);
            yield return new RayleighNHydraRingVariable(133, HydraRingDeviationType.Standard, 45.45, 46.46);
            yield return new DeterministicHydraRingVariable(134, 47.47);
            yield return new DeterministicHydraRingVariable(135, 48.48);
            yield return new DeterministicHydraRingVariable(136, 49.49);
        }
    }
}