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
    /// Container of all data necessary for performing a quadratic flooded culvert based structures stability point calculation via Hydra-Ring.
    /// </summary>
    public class StructuresStabilityPointFloodedCulvertQuadraticCalculationInput : StructuresStabilityPointCalculationInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="StructuresStabilityPointFloodedCulvertQuadraticCalculationInput"/>.
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
        /// <param name="hydarRingInsideWaterLevelFailureConstructionMean">The mean of the inside water level at failure of construction to use during the calculation.</param>
        /// <param name="hydarRingInsideWaterLevelFailureConstructionStandardDeviation">The standard deviation of the inside water level at failure of construction to use during the calculation.</param>
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
        public StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                                                               IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                                               double hydraRingVolumicWeightWater, double hydraRingGravitationalAcceleration,
                                                                               double hydraRingLevelCrestStructureMean, double hydraRingLevelCrestStructureStandardDeviation,
                                                                               double hydraRingStructureNormalOrientation, double hydraRingFactorStormDurationOpenStructure,
                                                                               double hydraRingModelFactorSubCriticalFlowMean, double hydraRingModelFactorSubCriticalFlowVariation,
                                                                               double hydraRingThresholdHeightOpenWeirMean, double hydraRingThresholdHeightOpenWeirStandardDeviation,
                                                                               double hydarRingInsideWaterLevelFailureConstructionMean, double hydarRingInsideWaterLevelFailureConstructionStandardDeviation,
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
            : base(hydraulicBoundaryLocationId, hydraRingSection, forelandPoints,
                   hydraRingVolumicWeightWater, hydraRingGravitationalAcceleration,
                   hydraRingLevelCrestStructureMean, hydraRingLevelCrestStructureStandardDeviation,
                   hydraRingStructureNormalOrientation, hydraRingFactorStormDurationOpenStructure,
                   hydraRingModelFactorSubCriticalFlowMean, hydraRingModelFactorSubCriticalFlowVariation,
                   hydraRingThresholdHeightOpenWeirMean, hydraRingThresholdHeightOpenWeirStandardDeviation,
                   hydarRingInsideWaterLevelFailureConstructionMean, hydarRingInsideWaterLevelFailureConstructionStandardDeviation,
                   hydraRingFailureProbabilityRepairClosure,
                   hydraRingFailureCollisionEnergyMean, hydraRingFailureCollisionEnergyVariation,
                   hydraRingModelFactorCollisionLoadMean, hydraRingModelFactorCollisionLoadVariation,
                   hydraRingShipMassMean, hydraRingShipMassVariation,
                   hydraRingShipVelocityMean, hydraRingShipVelocityVariation,
                   hydraRingLevelingCount, hydraRingProbabilityCollisionSecondaryStructure,
                   hydraRingFlowVelocityStructureClosableMean, hydraRingFlowVelocityStructureClosableStandardDeviation,
                   hydraRingInsideWaterLevelMean, hydraRingInsideWaterLevelStandardDeviation,
                   hydraRingAllowedLevelIncreaseStorageMean, hydraRingAllowedLevelIncreaseStorageStandardDeviation,
                   hydraRingModelFactorStorageVolumeMean, hydraRingModelFactorStorageVolumeStandardDeviation,
                   hydraRingStorageStructureAreaMean, hydraRingStorageStructureAreaVariation,
                   hydraRingModelFactorInflowVolume,
                   hydraRingFlowWidthAtBottomProtectionMean, hydraRingFlowWidthAtBottomProtectionStandardDeviation,
                   hydraRingCriticalOvertoppingDischargeMean, hydraRingCriticalOvertoppingDischargeVariation,
                   hydraRingFailureProbabilityStructureWithErosion,
                   hydraRingStormDurationMean, hydraRingStormDurationVariation,
                   hydraRingBermWidthMean, hydraRingBermWidthStandardDeviation,
                   hydraRingEvaluationLevel,
                   hydraRingModelFactorLoadEffectMean, hydraRingModelFactorLoadEffectStandardDeviation,
                   hydraRingWaveRatioMaxHMean, hydraRingWaveRatioMaxHStandardDeviation,
                   hydraRingVerticalDistance,
                   hydraRingModificationFactorWavesSlowlyVaryingPressureComponent,
                   hydraRingModificationFactorDynamicOrImpulsivePressureComponent) {}

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 424:
                    return 107;
                case 425:
                    return 113;
                case 430:
                    return 115;
                case 435:
                    return 117;
                default:
                    return null;
            }
        }
    }
}