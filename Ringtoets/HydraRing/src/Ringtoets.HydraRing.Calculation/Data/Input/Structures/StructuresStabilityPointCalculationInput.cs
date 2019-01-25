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
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a structures stability point calculation via Hydra-Ring.
    /// </summary>
    public abstract class StructuresStabilityPointCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly double volumicWeightWater;
        private readonly double gravitationalAcceleration;
        private readonly double levelCrestStructureMean;
        private readonly double levelCrestStructureStandardDeviation;
        private readonly double structureNormalOrientation;
        private readonly double factorStormDurationOpenStructure;
        private readonly double thresholdHeightOpenWeirMean;
        private readonly double thresholdHeightOpenWeirStandardDeviation;
        private readonly double insideWaterLevelFailureConstructionMean;
        private readonly double insideWaterLevelFailureConstructionStandardDeviation;
        private readonly double failureProbabilityRepairClosure;
        private readonly double failureCollisionEnergyMean;
        private readonly double failureCollisionEnergyVariation;
        private readonly double modelFactorCollisionLoadMean;
        private readonly double modelFactorCollisionLoadVariation;
        private readonly double shipMassMean;
        private readonly double shipMassVariation;
        private readonly double shipVelocityMean;
        private readonly double shipVelocityVariation;
        private readonly int levellingCount;
        private readonly double probabilityCollisionSecondaryStructure;
        private readonly double flowVelocityStructureClosableMean;
        private readonly double flowVelocityStructureClosableVariation;
        private readonly double insideWaterLevelMean;
        private readonly double insideWaterLevelStandardDeviation;
        private readonly double allowedLevelIncreaseStorageMean;
        private readonly double allowedLevelIncreaseStorageStandardDeviation;
        private readonly double modelFactorStorageVolumeMean;
        private readonly double modelFactorStorageVolumeStandardDeviation;
        private readonly double storageStructureAreaMean;
        private readonly double storageStructureAreaVariation;
        private readonly double modelFactorInflowVolume;
        private readonly double flowWidthAtBottomProtectionMean;
        private readonly double flowWidthAtBottomProtectionStandardDeviation;
        private readonly double criticalOvertoppingDischargeMean;
        private readonly double criticalOvertoppingDischargeVariation;
        private readonly double failureProbabilityStructureWithErosion;
        private readonly double stormDurationMean;
        private readonly double stormDurationVariation;
        private readonly double modelFactorLongThresholdMean;
        private readonly double modelFactorLongThresholdStandardDeviation;
        private readonly double bankWidthMean;
        private readonly double bankWidthStandardDeviation;
        private readonly double evaluationLevel;
        private readonly double modelFactorLoadEffectMean;
        private readonly double modelFactorLoadEffectStandardDeviation;
        private readonly double waveRatioMaxHN;
        private readonly double waveRatioMaxHStandardDeviation;
        private readonly double verticalDistance;
        private readonly double modificationFactorWavesSlowlyVaryingPressureComponent;
        private readonly double modificationFactorDynamicOrImpulsivePressureComponent;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresStabilityPointCalculationInput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="sectionNormal">The normal of the section.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="volumicWeightWater">The volumic weight of water.</param>
        /// <param name="gravitationalAcceleration">The gravitational acceleration.</param>
        /// <param name="levelCrestStructureMean">The mean of the crest level of the structure.</param>
        /// <param name="levelCrestStructureStandardDeviation">The standard deviation of the crest level of the structure.</param>
        /// <param name="structureNormalOrientation">The orientation of the normal of the structure.</param>
        /// <param name="factorStormDurationOpenStructure">The factor of the storm duration for an open structure.</param>
        /// <param name="thresholdHeightOpenWeirMean">The mean of the threshold height open weir.</param>
        /// <param name="thresholdHeightOpenWeirStandardDeviation">The standard deviation of the threshold height open weir.</param>
        /// <param name="insideWaterLevelFailureConstructionMean">The mean of the inside water level at failure of construction.</param>
        /// <param name="insideWaterLevelFailureConstructionStandardDeviation">The standard deviation of the inside water level at failure of construction.</param>
        /// <param name="failureProbabilityRepairClosure">The failure probability repair closure.</param>
        /// <param name="failureCollisionEnergyMean">The mean of the failure collision energy.</param>
        /// <param name="failureCollisionEnergyVariation">The variation of the failure collision energy.</param>
        /// <param name="modelFactorCollisionLoadMean">The mean of the model factor collision load.</param>
        /// <param name="modelFactorCollisionLoadVariation">The variation of the model factor collision load.</param>
        /// <param name="shipMassMean">The mean of the ship mass.</param>
        /// <param name="shipMassVariation">The variation of the ship mass.</param>
        /// <param name="shipVelocityMean">The mean of the ship velocity.</param>
        /// <param name="shipVelocityVariation">The variation of the ship velocity.</param>
        /// <param name="levellingCount">The levelling count.</param>
        /// <param name="probabilityCollisionSecondaryStructure">The probability of collision of the secondary structure.</param>
        /// <param name="flowVelocityStructureClosableMean">The mean of the flow velocity structure closable.</param>
        /// <param name="flowVelocityStructureClosableVariation">The variation of the flow velocity structure closable.</param>
        /// <param name="insideWaterLevelMean">The mean of the inside water level.</param>
        /// <param name="insideWaterLevelStandardDeviation">The standard deviation of the inside water level.</param>
        /// <param name="allowedLevelIncreaseStorageMean">The mean of the allowed level of increase for storage.</param>
        /// <param name="allowedLevelIncreaseStorageStandardDeviation">The standard deviation of the allowed level of increase for storage.</param>
        /// <param name="modelFactorStorageVolumeMean">The mean of the model factor storage volume.</param>
        /// <param name="modelFactorStorageVolumeStandardDeviation">The standard deviation of the model factor storage volume.</param>
        /// <param name="storageStructureAreaMean">The mean of the storage structure area.</param>
        /// <param name="storageStructureAreaVariation">The variation of the storage structure area.</param>
        /// <param name="modelFactorInflowVolume">The model factor inflow volume.</param>
        /// <param name="flowWidthAtBottomProtectionMean">The mean of the flow width at bottom protection.</param>
        /// <param name="flowWidthAtBottomProtectionStandardDeviation">The standard deviation of the flow width at bottom protection.</param>
        /// <param name="criticalOvertoppingDischargeMean">The mean of the critical overtopping discharge.</param>
        /// <param name="criticalOvertoppingDischargeVariation">The variation of the critical overtopping discharge.</param>
        /// <param name="failureProbabilityStructureWithErosion">The failure probability structure with erosion.</param>
        /// <param name="stormDurationMean">The mean of the storm duration.</param>
        /// <param name="stormDurationVariation">The variation of the storm duration.</param>
        /// <param name="modelFactorLongThresholdMean">The mean of the model factor long threshold.</param>
        /// <param name="modelFactorLongThresholdStandardDeviation">The standard deviation of the model factor long threshold.</param>
        /// <param name="bankWidthMean">The mean of the bank width.</param>
        /// <param name="bankWidthStandardDeviation">The standard deviation of the bank width.</param>
        /// <param name="evaluationLevel">The evaluation level.</param>
        /// <param name="modelFactorLoadEffectMean">The mean of the model factor load effect.</param>
        /// <param name="modelFactorLoadEffectStandardDeviation">The standard deviation of the model factor load effect.</param>
        /// <param name="waveRatioMaxHN">The N of the wave ratio max h.</param>
        /// <param name="waveRatioMaxHStandardDeviation">The standard deviation of the wave ratio max h.</param>
        /// <param name="verticalDistance">The vertical distance.</param>
        /// <param name="modificationFactorWavesSlowlyVaryingPressureComponent">The modification factor waves slowly-varying pressure component.</param>
        /// <param name="modificationFactorDynamicOrImpulsivePressureComponent">The modification factor dynamic or impulsive pressure component.</param>
        protected StructuresStabilityPointCalculationInput(long hydraulicBoundaryLocationId,
                                                           double sectionNormal,
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
            : base(hydraulicBoundaryLocationId)
        {
            Section = new HydraRingSection(1, double.NaN, sectionNormal);
            ForelandsPoints = forelandPoints;
            BreakWater = breakWater;
            this.volumicWeightWater = volumicWeightWater;
            this.gravitationalAcceleration = gravitationalAcceleration;
            this.levelCrestStructureMean = levelCrestStructureMean;
            this.levelCrestStructureStandardDeviation = levelCrestStructureStandardDeviation;
            this.structureNormalOrientation = structureNormalOrientation;
            this.factorStormDurationOpenStructure = factorStormDurationOpenStructure;
            this.thresholdHeightOpenWeirMean = thresholdHeightOpenWeirMean;
            this.thresholdHeightOpenWeirStandardDeviation = thresholdHeightOpenWeirStandardDeviation;
            this.insideWaterLevelFailureConstructionMean = insideWaterLevelFailureConstructionMean;
            this.insideWaterLevelFailureConstructionStandardDeviation = insideWaterLevelFailureConstructionStandardDeviation;
            this.failureProbabilityRepairClosure = failureProbabilityRepairClosure;
            this.failureCollisionEnergyMean = failureCollisionEnergyMean;
            this.failureCollisionEnergyVariation = failureCollisionEnergyVariation;
            this.modelFactorCollisionLoadMean = modelFactorCollisionLoadMean;
            this.modelFactorCollisionLoadVariation = modelFactorCollisionLoadVariation;
            this.shipMassMean = shipMassMean;
            this.shipMassVariation = shipMassVariation;
            this.shipVelocityMean = shipVelocityMean;
            this.shipVelocityVariation = shipVelocityVariation;
            this.levellingCount = levellingCount;
            this.probabilityCollisionSecondaryStructure = probabilityCollisionSecondaryStructure;
            this.flowVelocityStructureClosableMean = flowVelocityStructureClosableMean;
            this.flowVelocityStructureClosableVariation = flowVelocityStructureClosableVariation;
            this.insideWaterLevelMean = insideWaterLevelMean;
            this.insideWaterLevelStandardDeviation = insideWaterLevelStandardDeviation;
            this.allowedLevelIncreaseStorageMean = allowedLevelIncreaseStorageMean;
            this.allowedLevelIncreaseStorageStandardDeviation = allowedLevelIncreaseStorageStandardDeviation;
            this.modelFactorStorageVolumeMean = modelFactorStorageVolumeMean;
            this.modelFactorStorageVolumeStandardDeviation = modelFactorStorageVolumeStandardDeviation;
            this.storageStructureAreaMean = storageStructureAreaMean;
            this.storageStructureAreaVariation = storageStructureAreaVariation;
            this.modelFactorInflowVolume = modelFactorInflowVolume;
            this.flowWidthAtBottomProtectionMean = flowWidthAtBottomProtectionMean;
            this.flowWidthAtBottomProtectionStandardDeviation = flowWidthAtBottomProtectionStandardDeviation;
            this.criticalOvertoppingDischargeMean = criticalOvertoppingDischargeMean;
            this.criticalOvertoppingDischargeVariation = criticalOvertoppingDischargeVariation;
            this.failureProbabilityStructureWithErosion = failureProbabilityStructureWithErosion;
            this.stormDurationMean = stormDurationMean;
            this.stormDurationVariation = stormDurationVariation;
            this.modelFactorLongThresholdMean = modelFactorLongThresholdMean;
            this.modelFactorLongThresholdStandardDeviation = modelFactorLongThresholdStandardDeviation;
            this.bankWidthMean = bankWidthMean;
            this.bankWidthStandardDeviation = bankWidthStandardDeviation;
            this.evaluationLevel = evaluationLevel;
            this.modelFactorLoadEffectMean = modelFactorLoadEffectMean;
            this.modelFactorLoadEffectStandardDeviation = modelFactorLoadEffectStandardDeviation;
            this.waveRatioMaxHN = waveRatioMaxHN;
            this.waveRatioMaxHStandardDeviation = waveRatioMaxHStandardDeviation;
            this.verticalDistance = verticalDistance;
            this.modificationFactorWavesSlowlyVaryingPressureComponent = modificationFactorWavesSlowlyVaryingPressureComponent;
            this.modificationFactorDynamicOrImpulsivePressureComponent = modificationFactorDynamicOrImpulsivePressureComponent;
        }

        public override HydraRingFailureMechanismType FailureMechanismType { get; } = HydraRingFailureMechanismType.StructuresStructuralFailure;

        public override int VariableId { get; } = 58;

        public override HydraRingSection Section { get; }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield return new DeterministicHydraRingVariable(43, volumicWeightWater);
                yield return new DeterministicHydraRingVariable(58, gravitationalAcceleration);
                yield return new NormalHydraRingVariable(60, HydraRingDeviationType.Standard, levelCrestStructureMean, levelCrestStructureStandardDeviation);
                yield return new DeterministicHydraRingVariable(61, structureNormalOrientation);
                yield return new DeterministicHydraRingVariable(63, factorStormDurationOpenStructure);
                yield return new NormalHydraRingVariable(65, HydraRingDeviationType.Standard, thresholdHeightOpenWeirMean, thresholdHeightOpenWeirStandardDeviation);
                yield return new NormalHydraRingVariable(82, HydraRingDeviationType.Standard, insideWaterLevelFailureConstructionMean, insideWaterLevelFailureConstructionStandardDeviation);
                yield return new DeterministicHydraRingVariable(85, failureProbabilityRepairClosure);
                yield return new LogNormalHydraRingVariable(86, HydraRingDeviationType.Variation, failureCollisionEnergyMean, failureCollisionEnergyVariation);
                yield return new NormalHydraRingVariable(87, HydraRingDeviationType.Variation, modelFactorCollisionLoadMean, modelFactorCollisionLoadVariation);
                yield return new NormalHydraRingVariable(88, HydraRingDeviationType.Variation, shipMassMean, shipMassVariation);
                yield return new NormalHydraRingVariable(89, HydraRingDeviationType.Variation, shipVelocityMean, shipVelocityVariation);
                yield return new DeterministicHydraRingVariable(90, levellingCount);
                yield return new DeterministicHydraRingVariable(91, probabilityCollisionSecondaryStructure);
                yield return new NormalHydraRingVariable(92, HydraRingDeviationType.Variation, flowVelocityStructureClosableMean, flowVelocityStructureClosableVariation);
                yield return new NormalHydraRingVariable(93, HydraRingDeviationType.Standard, insideWaterLevelMean, insideWaterLevelStandardDeviation);
                yield return new LogNormalHydraRingVariable(94, HydraRingDeviationType.Standard, allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation);
                yield return new LogNormalHydraRingVariable(95, HydraRingDeviationType.Standard, modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation);
                yield return new LogNormalHydraRingVariable(96, HydraRingDeviationType.Variation, storageStructureAreaMean, storageStructureAreaVariation);
                yield return new DeterministicHydraRingVariable(97, modelFactorInflowVolume);
                yield return new LogNormalHydraRingVariable(103, HydraRingDeviationType.Standard, flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation);
                yield return new LogNormalHydraRingVariable(104, HydraRingDeviationType.Variation, criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation);
                yield return new DeterministicHydraRingVariable(105, failureProbabilityStructureWithErosion);
                yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, stormDurationMean, stormDurationVariation);
                yield return new NormalHydraRingVariable(125, HydraRingDeviationType.Standard, modelFactorLongThresholdMean, modelFactorLongThresholdStandardDeviation);
                yield return new NormalHydraRingVariable(130, HydraRingDeviationType.Standard, bankWidthMean, bankWidthStandardDeviation);
                yield return new DeterministicHydraRingVariable(131, evaluationLevel);
                yield return new NormalHydraRingVariable(132, HydraRingDeviationType.Standard, modelFactorLoadEffectMean, modelFactorLoadEffectStandardDeviation);
                yield return new RayleighNHydraRingVariable(133, HydraRingDeviationType.Standard, waveRatioMaxHN, waveRatioMaxHStandardDeviation);
                yield return new DeterministicHydraRingVariable(134, verticalDistance);
                yield return new DeterministicHydraRingVariable(135, modificationFactorWavesSlowlyVaryingPressureComponent);
                yield return new DeterministicHydraRingVariable(136, modificationFactorDynamicOrImpulsivePressureComponent);
            }
        }

        public override IEnumerable<HydraRingForelandPoint> ForelandsPoints { get; }

        public override HydraRingBreakWater BreakWater { get; }

        public override int IterationMethodId
        {
            get
            {
                return 6;
            }
        }

        public abstract override int? GetSubMechanismModelId(int subMechanismId);
    }
}