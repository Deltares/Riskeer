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

namespace Ringtoets.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a structures stability point calculation via Hydra-Ring.
    /// </summary>
    public abstract class StructuresStabilityPointCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection hydraRingSection;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;
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
        private readonly double levelingCount;
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
        private readonly double bermWidthMean;
        private readonly double bermWidthStandardDeviation;
        private readonly double evaluationLevel;
        private readonly double modelFactorLoadEffectMean;
        private readonly double modelFactorLoadEffectStandardDeviation;
        private readonly double waveRatioMaxHMean;
        private readonly double waveRatioMaxHStandardDeviation;
        private readonly double verticalDistance;
        private readonly double modificationFactorWavesSlowlyVaryingPressureComponent;
        private readonly double modificationFactorDynamicOrImpulsivePressureComponent;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresStabilityPointCalculationInput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="hydraRingSection">The section to use during the calculation.</param>
        /// <param name="forelandPoints">The foreland points to use during the calculation.</param>
        /// <param name="hydraRingVolumicWeightWater">The volumic weight of water to use during the calculation.</param>
        /// <param name="hydraRingGravitationalAcceleration">The gravitational acceleration to use during the calculation.</param>
        /// <param name="hydraRingLevelCrestStructureMean">The mean of the level crest structure to use during the calculation.</param>
        /// <param name="hydraRingLevelCrestStructureStandardDeviation">The standard deviation of the level crest structure to use during the calculation.</param>
        /// <param name="hydraRingStructureNormalOrientation">The orientation of the normal of the structure to use during the calculation.</param>
        /// <param name="hydraRingFactorStormDurationOpenStructure">The factor of the storm duration for an open structure to use during the calculation.</param>
        /// <param name="hydraRingModelFactorSubCriticalFlowMean">The mean of the model factor sub critical flow to use during the calculation.</param>
        /// <param name="hydraRingModelFactorSubCriticalFlowVariation">The variation of the model factor sub critical flow to use during the calculation.</param>
        /// <param name="hydraRingThresholdHeightOpenWeirMean">The mean of the threshold height open weir to use during the calculation.</param>
        /// <param name="hydraRingThresholdHeightOpenWeirStandardDeviation">The standard deviation of the threshold height open weir to use during the calculation.</param>
        /// <param name="hydraRingInsideWaterLevelFailureConstructionMean">The mean of the inside water level at failure of construction to use during the calculation.</param>
        /// <param name="hydraRingInsideWaterLevelFailureConstructionStandardDeviation">The standard deviation of the inside water level at failure of construction to use during the calculation.</param>
        /// <param name="hydraRingFailureProbabilityRepairClosure">The failure probability repair closure to use during the calculation.</param>
        /// <param name="hydraRingFailureCollisionEnergyMean">The mean of the failure collision energy to use during the calculation.</param>
        /// <param name="hydraRingFailureCollisionEnergyVariation">The variation of the failure collision energy to use during the calculation.</param>
        /// <param name="hydraRingModelFactorCollisionLoadMean">The mean of the model factor collision load to use during the calculation.</param>
        /// <param name="hydraRingModelFactorCollisionLoadVariation">The variation of the model factor collision load to use during the calculation.</param>
        /// <param name="hydraRingShipMassMean">The mean of the ship mass to use during the calculation.</param>
        /// <param name="hydraRingShipMassVariation">The variation of the ship mass to use during the calculation.</param>
        /// <param name="hydraRingShipVelocityMean">The mean of the ship velocity to use during the calculation.</param>
        /// <param name="hydraRingShipVelocityVariation">The variation of the ship velocity to use during the calculation.</param>
        /// <param name="hydraRingLevelingCount">The leveling count to use during the calculation.</param>
        /// <param name="hydraRingProbabilityCollisionSecondaryStructure">The probability collision secondary structure to use during the calculation.</param>
        /// <param name="hydraRingFlowVelocityStructureClosableMean">The mean of the flow velocity structure closable to use during the calculation.</param>
        /// <param name="hydraRingFlowVelocityStructureClosableStandardDeviation">The standard deviation of the flow velocity structure closable to use during the calculation.</param>
        /// <param name="hydraRingInsideWaterLevelMean">The mean of the inside water level to use during the calculation.</param>
        /// <param name="hydraRingInsideWaterLevelStandardDeviation">The standard deviation of the inside water level to use during the calculation.</param>
        /// <param name="hydraRingAllowedLevelIncreaseStorageMean">The mean of the allowed level increase for storage to use during the calculation.</param>
        /// <param name="hydraRingAllowedLevelIncreaseStorageStandardDeviation">The standard deviation of the allowed level increase for storage to use during the calculation.</param>
        /// <param name="hydraRingModelFactorStorageVolumeMean">The mean of the model factor storage volume to use during the calculation.</param>
        /// <param name="hydraRingModelFactorStorageVolumeStandardDeviation">The standard deviation of the model factor storage volume to use during the calculation.</param>
        /// <param name="hydraRingStorageStructureAreaMean">The mean of the storage structure area to use during the calculation.</param>
        /// <param name="hydraRingStorageStructureAreaVariation">The variation of the storage structure area to use during the calculation.</param>
        /// <param name="hydraRingModelFactorInflowVolume">The model factor inflow volume to use during the calculation.</param>
        /// <param name="hydraRingFlowWidthAtBottomProtectionMean">The mean of the flow width at bottom protection to use during the calculation.</param>
        /// <param name="hydraRingFlowWidthAtBottomProtectionStandardDeviation">The standard deviation of the flow width at bottom protection to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingDischargeMean">The mean of the critical overtopping discharge to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingDischargeVariation">The variation of the critical overtopping discharge to use during the calculation.</param>
        /// <param name="hydraRingFailureProbabilityStructureWithErosion">The failure probability structure with erosion to use during the calculation.</param>
        /// <param name="hydraRingStormDurationMean">The mean of the storm duration to use during the calculation.</param>
        /// <param name="hydraRingStormDurationVariation">The variation of the storm duration to use during the calculation.</param>
        /// <param name="hydraRingBermWidthMean">The mean of the berm width to use during the calculation.</param>
        /// <param name="hydraRingBermWidthStandardDeviation">The standard deviation of the berm width to use during the calculation.</param>
        /// <param name="hydraRingEvaluationLevel">The evaluation level to use during the calculation.</param>
        /// <param name="hydraRingModelFactorLoadEffectMean">The mean of the model factor load effect to use during the calculation.</param>
        /// <param name="hydraRingModelFactorLoadEffectStandardDeviation">The standard deviation of the model factor load effect to use during the calculation.</param>
        /// <param name="hydraRingWaveRatioMaxHMean">The mean of the wave ratio max h to use during the calculation.</param>
        /// <param name="hydraRingWaveRatioMaxHStandardDeviation">The standard deviation of the wave ratio max h to use during the calculation.</param>
        /// <param name="hydraRingVerticalDistance">The vertical distance to use during the calculation.</param>
        /// <param name="hydraRingModificationFactorWavesSlowlyVaryingPressureComponent">The modification factor waves slowly-varying pressure component to use during the calculation.</param>
        /// <param name="hydraRingModificationFactorDynamicOrImpulsivePressureComponent">The modification factor dynamic or impulsive pressure component to use during the calculation.</param>
        protected StructuresStabilityPointCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                                           IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                           double hydraRingVolumicWeightWater, double hydraRingGravitationalAcceleration,
                                                           double hydraRingLevelCrestStructureMean, double hydraRingLevelCrestStructureStandardDeviation,
                                                           double hydraRingStructureNormalOrientation, double hydraRingFactorStormDurationOpenStructure,
                                                           double hydraRingModelFactorSubCriticalFlowMean, double hydraRingModelFactorSubCriticalFlowVariation,
                                                           double hydraRingThresholdHeightOpenWeirMean, double hydraRingThresholdHeightOpenWeirStandardDeviation,
                                                           double hydraRingInsideWaterLevelFailureConstructionMean, double hydraRingInsideWaterLevelFailureConstructionStandardDeviation,
                                                           double hydraRingFailureProbabilityRepairClosure,
                                                           double hydraRingFailureCollisionEnergyMean, double hydraRingFailureCollisionEnergyVariation,
                                                           double hydraRingModelFactorCollisionLoadMean, double hydraRingModelFactorCollisionLoadVariation,
                                                           double hydraRingShipMassMean, double hydraRingShipMassVariation,
                                                           double hydraRingShipVelocityMean, double hydraRingShipVelocityVariation,
                                                           double hydraRingLevelingCount, double hydraRingProbabilityCollisionSecondaryStructure,
                                                           double hydraRingFlowVelocityStructureClosableMean, double hydraRingFlowVelocityStructureClosableStandardDeviation,
                                                           double hydraRingInsideWaterLevelMean, double hydraRingInsideWaterLevelStandardDeviation,
                                                           double hydraRingAllowedLevelIncreaseStorageMean, double hydraRingAllowedLevelIncreaseStorageStandardDeviation,
                                                           double hydraRingModelFactorStorageVolumeMean, double hydraRingModelFactorStorageVolumeStandardDeviation,
                                                           double hydraRingStorageStructureAreaMean, double hydraRingStorageStructureAreaVariation,
                                                           double hydraRingModelFactorInflowVolume,
                                                           double hydraRingFlowWidthAtBottomProtectionMean, double hydraRingFlowWidthAtBottomProtectionStandardDeviation,
                                                           double hydraRingCriticalOvertoppingDischargeMean, double hydraRingCriticalOvertoppingDischargeVariation,
                                                           double hydraRingFailureProbabilityStructureWithErosion,
                                                           double hydraRingStormDurationMean, double hydraRingStormDurationVariation,
                                                           double hydraRingBermWidthMean, double hydraRingBermWidthStandardDeviation,
                                                           double hydraRingEvaluationLevel,
                                                           double hydraRingModelFactorLoadEffectMean, double hydraRingModelFactorLoadEffectStandardDeviation,
                                                           double hydraRingWaveRatioMaxHMean, double hydraRingWaveRatioMaxHStandardDeviation,
                                                           double hydraRingVerticalDistance,
                                                           double hydraRingModificationFactorWavesSlowlyVaryingPressureComponent,
                                                           double hydraRingModificationFactorDynamicOrImpulsivePressureComponent)
            : base(hydraulicBoundaryLocationId)
        {
            this.hydraRingSection = hydraRingSection;
            this.forelandPoints = forelandPoints;
            volumicWeightWater = hydraRingVolumicWeightWater;
            gravitationalAcceleration = hydraRingGravitationalAcceleration;
            levelCrestStructureMean = hydraRingLevelCrestStructureMean;
            levelCrestStructureStandardDeviation = hydraRingLevelCrestStructureStandardDeviation;
            structureNormalOrientation = hydraRingStructureNormalOrientation;
            factorStormDurationOpenStructure = hydraRingFactorStormDurationOpenStructure;
            modelFactorSubCriticalFlowMean = hydraRingModelFactorSubCriticalFlowMean;
            modelFactorSubCriticalFlowVariation = hydraRingModelFactorSubCriticalFlowVariation;
            thresholdHeightOpenWeirMean = hydraRingThresholdHeightOpenWeirMean;
            thresholdHeightOpenWeirStandardDeviation = hydraRingThresholdHeightOpenWeirStandardDeviation;
            insideWaterLevelFailureConstructionMean = hydraRingInsideWaterLevelFailureConstructionMean;
            insideWaterLevelFailureConstructionStandardDeviation = hydraRingInsideWaterLevelFailureConstructionStandardDeviation;
            failureProbabilityRepairClosure = hydraRingFailureProbabilityRepairClosure;
            failureCollisionEnergyMean = hydraRingFailureCollisionEnergyMean;
            failureCollisionEnergyVariation = hydraRingFailureCollisionEnergyVariation;
            modelFactorCollisionLoadMean = hydraRingModelFactorCollisionLoadMean;
            modelFactorCollisionLoadVariation = hydraRingModelFactorCollisionLoadVariation;
            shipMassMean = hydraRingShipMassMean;
            shipMassVariation = hydraRingShipMassVariation;
            shipVelocityMean = hydraRingShipVelocityMean;
            shipVelocityVariation = hydraRingShipVelocityVariation;
            levelingCount = hydraRingLevelingCount;
            probabilityCollisionSecondaryStructure = hydraRingProbabilityCollisionSecondaryStructure;
            flowVelocityStructureClosableMean = hydraRingFlowVelocityStructureClosableMean;
            flowVelocityStructureClosableStandardDeviation = hydraRingFlowVelocityStructureClosableStandardDeviation;
            insideWaterLevelMean = hydraRingInsideWaterLevelMean;
            insideWaterLevelStandardDeviation = hydraRingInsideWaterLevelStandardDeviation;
            allowedLevelIncreaseStorageMean = hydraRingAllowedLevelIncreaseStorageMean;
            allowedLevelIncreaseStorageStandardDeviation = hydraRingAllowedLevelIncreaseStorageStandardDeviation;
            modelFactorStorageVolumeMean = hydraRingModelFactorStorageVolumeMean;
            modelFactorStorageVolumeStandardDeviation = hydraRingModelFactorStorageVolumeStandardDeviation;
            storageStructureAreaMean = hydraRingStorageStructureAreaMean;
            storageStructureAreaVariation = hydraRingStorageStructureAreaVariation;
            modelFactorInflowVolume = hydraRingModelFactorInflowVolume;
            flowWidthAtBottomProtectionMean = hydraRingFlowWidthAtBottomProtectionMean;
            flowWidthAtBottomProtectionStandardDeviation = hydraRingFlowWidthAtBottomProtectionStandardDeviation;
            criticalOvertoppingDischargeMean = hydraRingCriticalOvertoppingDischargeMean;
            criticalOvertoppingDischargeVariation = hydraRingCriticalOvertoppingDischargeVariation;
            failureProbabilityStructureWithErosion = hydraRingFailureProbabilityStructureWithErosion;
            stormDurationMean = hydraRingStormDurationMean;
            stormDurationVariation = hydraRingStormDurationVariation;
            bermWidthMean = hydraRingBermWidthMean;
            bermWidthStandardDeviation = hydraRingBermWidthStandardDeviation;
            evaluationLevel = hydraRingEvaluationLevel;
            modelFactorLoadEffectMean = hydraRingModelFactorLoadEffectMean;
            modelFactorLoadEffectStandardDeviation = hydraRingModelFactorLoadEffectStandardDeviation;
            waveRatioMaxHMean = hydraRingWaveRatioMaxHMean;
            waveRatioMaxHStandardDeviation = hydraRingWaveRatioMaxHStandardDeviation;
            verticalDistance = hydraRingVerticalDistance;
            modificationFactorWavesSlowlyVaryingPressureComponent = hydraRingModificationFactorWavesSlowlyVaryingPressureComponent;
            modificationFactorDynamicOrImpulsivePressureComponent = hydraRingModificationFactorDynamicOrImpulsivePressureComponent;
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
                return hydraRingSection;
            }
        }

        public override IEnumerable<HydraRingForelandPoint> ForelandsPoints
        {
            get
            {
                return forelandPoints;
            }
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                return GetHydraRingVariables();
            }
        }

        public abstract override int? GetSubMechanismModelId(int subMechanismId);

        private IEnumerable<HydraRingVariable> GetHydraRingVariables()
        {
            // Volumic weight water
            yield return new HydraRingVariable(43, HydraRingDistributionType.Deterministic, volumicWeightWater,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Gravitational acceleration
            yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, gravitationalAcceleration,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Level crest structure
            yield return new HydraRingVariable(60, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, levelCrestStructureMean,
                                               levelCrestStructureStandardDeviation, double.NaN);

            // Orientation of the normal of the structure
            yield return new HydraRingVariable(61, HydraRingDistributionType.Deterministic, structureNormalOrientation,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Factor storm duration open structure
            yield return new HydraRingVariable(63, HydraRingDistributionType.Deterministic, factorStormDurationOpenStructure,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Model factor sub critical flow
            yield return new HydraRingVariable(64, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Variation, modelFactorSubCriticalFlowMean,
                                               modelFactorSubCriticalFlowVariation, double.NaN);

            // Threshold height open weir
            yield return new HydraRingVariable(65, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, thresholdHeightOpenWeirMean,
                                               thresholdHeightOpenWeirStandardDeviation, double.NaN);

            // Inside water level at failure of construction
            yield return new HydraRingVariable(82, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, insideWaterLevelFailureConstructionMean,
                                               insideWaterLevelFailureConstructionStandardDeviation, double.NaN);

            // Failure probability repair closure
            yield return new HydraRingVariable(85, HydraRingDistributionType.Deterministic, failureProbabilityRepairClosure,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Failure collision energy
            yield return new HydraRingVariable(86, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, failureCollisionEnergyMean,
                                               failureCollisionEnergyVariation, double.NaN);

            // Model factor collision load
            yield return new HydraRingVariable(87, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Variation, modelFactorCollisionLoadMean,
                                               modelFactorCollisionLoadVariation, double.NaN);

            // Ship mass
            yield return new HydraRingVariable(88, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Variation, shipMassMean,
                                               shipMassVariation, double.NaN);

            // Ship velocity
            yield return new HydraRingVariable(89, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Variation, shipVelocityMean,
                                               shipVelocityVariation, double.NaN);

            // Leveling count
            yield return new HydraRingVariable(90, HydraRingDistributionType.Deterministic, levelingCount,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Probability collision secondary structure
            yield return new HydraRingVariable(91, HydraRingDistributionType.Deterministic, probabilityCollisionSecondaryStructure,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Flow velocity structure closable
            yield return new HydraRingVariable(92, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, flowVelocityStructureClosableMean,
                                               flowVelocityStructureClosableStandardDeviation, double.NaN);

            // Inside water level
            yield return new HydraRingVariable(93, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, insideWaterLevelMean,
                                               insideWaterLevelStandardDeviation, double.NaN);

            // Allowed level increase storage
            yield return new HydraRingVariable(94, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, allowedLevelIncreaseStorageMean,
                                               allowedLevelIncreaseStorageStandardDeviation, double.NaN);

            // Model factor storage volume
            yield return new HydraRingVariable(95, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorStorageVolumeMean,
                                               modelFactorStorageVolumeStandardDeviation, double.NaN);

            // Storage structure area
            yield return new HydraRingVariable(96, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, storageStructureAreaMean,
                                               storageStructureAreaVariation, double.NaN);

            // Model factor inflow volume
            yield return new HydraRingVariable(97, HydraRingDistributionType.Deterministic, modelFactorInflowVolume,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Flow width at bottom protection
            yield return new HydraRingVariable(103, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, flowWidthAtBottomProtectionMean,
                                               flowWidthAtBottomProtectionStandardDeviation, double.NaN);

            // Critical overtopping discharge
            yield return new HydraRingVariable(104, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, criticalOvertoppingDischargeMean,
                                               criticalOvertoppingDischargeVariation, double.NaN);

            // Failure probability structure with erosion
            yield return new HydraRingVariable(105, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, failureProbabilityStructureWithErosion,
                                               0, double.NaN); // HACK: Pass the deterministic value as normal distribution (with standard deviation 0.0) as Hydra-Ring otherwise crashes

            // Storm duration
            yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, stormDurationMean,
                                               stormDurationVariation, double.NaN);

            // Berm width
            yield return new HydraRingVariable(130, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, bermWidthMean,
                                               bermWidthStandardDeviation, double.NaN);

            // Evaluation level
            yield return new HydraRingVariable(131, HydraRingDistributionType.Deterministic, evaluationLevel,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Model factor load effect
            yield return new HydraRingVariable(132, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorLoadEffectMean,
                                               modelFactorLoadEffectStandardDeviation, double.NaN);

            // Wave ratio max h
            yield return new HydraRingVariable(133, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, waveRatioMaxHMean,
                                               waveRatioMaxHStandardDeviation, double.NaN);

            // Vertical distance
            yield return new HydraRingVariable(134, HydraRingDistributionType.Deterministic, verticalDistance,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Modification factor waves slowly-varying pressure component
            yield return new HydraRingVariable(135, HydraRingDistributionType.Deterministic, modificationFactorWavesSlowlyVaryingPressureComponent,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Modification factor dynamic or impulsive pressure component
            yield return new HydraRingVariable(136, HydraRingDistributionType.Deterministic, modificationFactorDynamicOrImpulsivePressureComponent,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
        }
    }
}