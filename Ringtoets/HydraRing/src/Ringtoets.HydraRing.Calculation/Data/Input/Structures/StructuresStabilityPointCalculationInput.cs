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

namespace Ringtoets.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a structures stability point calculation via Hydra-Ring.
    /// </summary>
    public abstract class StructuresStabilityPointCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection section;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;
        private readonly HydraRingBreakWater breakWater;
        private readonly double volumicWeightWater;
        private readonly double gravitationalAcceleration;
        private readonly double levelCrestStructureMean;
        private readonly double levelCrestStructureStandardDeviation;
        private readonly double structureNormalOrientation;
        private readonly double factorStormDurationOpenStructure;
        private readonly double modelFactorSubCriticalFlowMean;
        private readonly double modelFactorSubCriticalFlowVariation;
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
        private readonly double flowVelocityStructureClosableStandardDeviation;
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
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station.</param>
        /// <param name="section">The section.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="volumicWeightWater">The volumic weight of water.</param>
        /// <param name="gravitationalAcceleration">The gravitational acceleration.</param>
        /// <param name="levelCrestStructureMean">The mean of the crest level of the structure.</param>
        /// <param name="levelCrestStructureStandardDeviation">The standard deviation of the crest level of the structure.</param>
        /// <param name="structureNormalOrientation">The orientation of the normal of the structure.</param>
        /// <param name="factorStormDurationOpenStructure">The factor of the storm duration for an open structure.</param>
        /// <param name="modelFactorSubCriticalFlowMean">The mean of the model factor sub critical flow.</param>
        /// <param name="modelFactorSubCriticalFlowVariation">The variation of the model factor sub critical flow.</param>
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
        /// <param name="flowVelocityStructureClosableStandardDeviation">The standard deviation of the flow velocity structure closable.</param>
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
        /// <param name="bankWidthMean">The mean of the berm width.</param>
        /// <param name="bankWidthStandardDeviation">The standard deviation of the berm width.</param>
        /// <param name="evaluationLevel">The evaluation level.</param>
        /// <param name="modelFactorLoadEffectMean">The mean of the model factor load effect.</param>
        /// <param name="modelFactorLoadEffectStandardDeviation">The standard deviation of the model factor load effect.</param>
        /// <param name="waveRatioMaxHN">The N of the wave ratio max h.</param>
        /// <param name="waveRatioMaxHStandardDeviation">The standard deviation of the wave ratio max h.</param>
        /// <param name="verticalDistance">The vertical distance.</param>
        /// <param name="modificationFactorWavesSlowlyVaryingPressureComponent">The modification factor waves slowly-varying pressure component.</param>
        /// <param name="modificationFactorDynamicOrImpulsivePressureComponent">The modification factor dynamic or impulsive pressure component.</param>
        protected StructuresStabilityPointCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection section,
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
            : base(hydraulicBoundaryLocationId)
        {
            this.section = section;
            this.forelandPoints = forelandPoints;
            this.breakWater = breakWater;
            this.volumicWeightWater = volumicWeightWater;
            this.gravitationalAcceleration = gravitationalAcceleration;
            this.levelCrestStructureMean = levelCrestStructureMean;
            this.levelCrestStructureStandardDeviation = levelCrestStructureStandardDeviation;
            this.structureNormalOrientation = structureNormalOrientation;
            this.factorStormDurationOpenStructure = factorStormDurationOpenStructure;
            this.modelFactorSubCriticalFlowMean = modelFactorSubCriticalFlowMean;
            this.modelFactorSubCriticalFlowVariation = modelFactorSubCriticalFlowVariation;
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
            this.flowVelocityStructureClosableStandardDeviation = flowVelocityStructureClosableStandardDeviation;
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

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.StructuresStructuralFailure;
            }
        }

        public override int VariableId
        {
            get
            {
                return 58;
            }
        }

        public override HydraRingSection Section
        {
            get
            {
                return section;
            }
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield return new HydraRingVariable(43, HydraRingDistributionType.Deterministic, volumicWeightWater,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, gravitationalAcceleration,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(60, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, levelCrestStructureMean,
                                                   levelCrestStructureStandardDeviation, double.NaN);
                yield return new HydraRingVariable(61, HydraRingDistributionType.Deterministic, structureNormalOrientation,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(63, HydraRingDistributionType.Deterministic, factorStormDurationOpenStructure,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(64, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Variation, modelFactorSubCriticalFlowMean,
                                                   modelFactorSubCriticalFlowVariation, double.NaN);
                yield return new HydraRingVariable(65, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, thresholdHeightOpenWeirMean,
                                                   thresholdHeightOpenWeirStandardDeviation, double.NaN);
                yield return new HydraRingVariable(82, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, insideWaterLevelFailureConstructionMean,
                                                   insideWaterLevelFailureConstructionStandardDeviation, double.NaN);
                yield return new HydraRingVariable(85, HydraRingDistributionType.Deterministic, failureProbabilityRepairClosure,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(86, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Variation, failureCollisionEnergyMean,
                                                   failureCollisionEnergyVariation, double.NaN);
                yield return new HydraRingVariable(87, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Variation, modelFactorCollisionLoadMean,
                                                   modelFactorCollisionLoadVariation, double.NaN);
                yield return new HydraRingVariable(88, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Variation, shipMassMean,
                                                   shipMassVariation, double.NaN);
                yield return new HydraRingVariable(89, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Variation, shipVelocityMean,
                                                   shipVelocityVariation, double.NaN);
                yield return new HydraRingVariable(90, HydraRingDistributionType.Deterministic, levellingCount,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(91, HydraRingDistributionType.Deterministic, probabilityCollisionSecondaryStructure,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(92, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, flowVelocityStructureClosableMean,
                                                   flowVelocityStructureClosableStandardDeviation, double.NaN);
                yield return new HydraRingVariable(93, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, insideWaterLevelMean,
                                                   insideWaterLevelStandardDeviation, double.NaN);
                yield return new HydraRingVariable(94, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Standard, allowedLevelIncreaseStorageMean,
                                                   allowedLevelIncreaseStorageStandardDeviation, double.NaN);
                yield return new HydraRingVariable(95, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Standard, modelFactorStorageVolumeMean,
                                                   modelFactorStorageVolumeStandardDeviation, double.NaN);
                yield return new HydraRingVariable(96, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Variation, storageStructureAreaMean,
                                                   storageStructureAreaVariation, double.NaN);
                yield return new HydraRingVariable(97, HydraRingDistributionType.Deterministic, modelFactorInflowVolume,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(103, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Standard, flowWidthAtBottomProtectionMean,
                                                   flowWidthAtBottomProtectionStandardDeviation, double.NaN);
                yield return new HydraRingVariable(104, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Variation, criticalOvertoppingDischargeMean,
                                                   criticalOvertoppingDischargeVariation, double.NaN);
                yield return new HydraRingVariable(105, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, failureProbabilityStructureWithErosion,
                                                   0, double.NaN); // Note: Pass the deterministic value as normal distribution (with standard deviation 0.0) as Hydra-Ring otherwise crashes
                yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Variation, stormDurationMean,
                                                   stormDurationVariation, double.NaN);
                yield return new HydraRingVariable(130, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, bankWidthMean,
                                                   bankWidthStandardDeviation, double.NaN);
                yield return new HydraRingVariable(131, HydraRingDistributionType.Deterministic, evaluationLevel,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(132, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, modelFactorLoadEffectMean,
                                                   modelFactorLoadEffectStandardDeviation, double.NaN);
                yield return new HydraRingVariable(133, HydraRingDistributionType.RayleighN, double.NaN,
                                                   HydraRingDeviationType.Standard, waveRatioMaxHStandardDeviation,
                                                   waveRatioMaxHN, double.NaN); // Note: Pass the N as "variability" and the standard deviation as "mean"
                yield return new HydraRingVariable(134, HydraRingDistributionType.Deterministic, verticalDistance,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(135, HydraRingDistributionType.Deterministic, modificationFactorWavesSlowlyVaryingPressureComponent,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(136, HydraRingDistributionType.Deterministic, modificationFactorDynamicOrImpulsivePressureComponent,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            }
        }

        public override IEnumerable<HydraRingForelandPoint> ForelandsPoints
        {
            get
            {
                return forelandPoints;
            }
        }

        public override HydraRingBreakWater BreakWater
        {
            get
            {
                return breakWater;
            }
        }

        public abstract override int? GetSubMechanismModelId(int subMechanismId);
    }
}