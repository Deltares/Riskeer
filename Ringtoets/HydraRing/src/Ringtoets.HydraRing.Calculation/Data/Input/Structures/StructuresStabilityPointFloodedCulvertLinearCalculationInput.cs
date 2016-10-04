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
using System.Linq;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a linear flooded culvert based structures stability point calculation via Hydra-Ring.
    /// </summary>
    public class StructuresStabilityPointFloodedCulvertLinearCalculationInput : StructuresStabilityPointCalculationInput
    {
        private readonly double drainCoefficientMean;
        private readonly double drainCoefficientStandardDeviation;
        private readonly double areaFlowAperturesMean;
        private readonly double areaFlowAperturesStandardDeviation;
        private readonly double stabilityLinearLoadModelMean;
        private readonly double stabilityLinearLoadModelVariation;
        private readonly double constructiveStrengthLinearLoadModelMean;
        private readonly double constructiveStrengthLinearLoadModelVariation;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresStabilityPointFloodedCulvertLinearCalculationInput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station.</param>
        /// <param name="section">The section.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="volumicWeightWater">The volumic weight of water.</param>
        /// <param name="gravitationalAcceleration">The gravitational acceleration.</param>
        /// <param name="levelCrestStructureMean">The mean of the level crest of the structure.</param>
        /// <param name="levelCrestStructureStandardDeviation">The standard deviation of the level crest of the structure.</param>
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
        /// <param name="levelingCount">The leveling count.</param>
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
        /// <param name="bermWidthMean">The mean of the berm width.</param>
        /// <param name="bermWidthStandardDeviation">The standard deviation of the berm width.</param>
        /// <param name="evaluationLevel">The evaluation level.</param>
        /// <param name="modelFactorLoadEffectMean">The mean of the model factor load effect.</param>
        /// <param name="modelFactorLoadEffectStandardDeviation">The standard deviation of the model factor load effect.</param>
        /// <param name="waveRatioMaxHMean">The mean of the wave ratio max h.</param>
        /// <param name="waveRatioMaxHStandardDeviation">The standard deviation of the wave ratio max h.</param>
        /// <param name="verticalDistance">The vertical distance.</param>
        /// <param name="modificationFactorWavesSlowlyVaryingPressureComponent">The modification factor waves slowly-varying pressure component.</param>
        /// <param name="modificationFactorDynamicOrImpulsivePressureComponent">The modification factor dynamic or impulsive pressure component.</param>
        /// <param name="drainCoefficientMean">The mean of the drain coefficient.</param>
        /// <param name="drainCoefficientStandardDeviation">The standard deviation of the drain coefficient.</param>
        /// <param name="areaFlowAperturesMean">The mean of the area of flow apertures.</param>
        /// <param name="areaFlowAperturesStandardDeviation">The standard deviation of the area of flow apertures.</param>
        /// <param name="stabilityLinearLoadModelMean">The mean of the stability linear load model.</param>
        /// <param name="stabilityLinearLoadModelVariation">The variation of the stability linear load model.</param>
        /// <param name="constructiveStrengthLinearLoadModelMean">The mean of the constructive strength linear load model.</param>
        /// <param name="constructiveStrengthLinearLoadModelVariation">The variation of the constructive strength linear load model.</param>
        public StructuresStabilityPointFloodedCulvertLinearCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection section,
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
                                                                            int levelingCount,
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
                                                                            double modificationFactorDynamicOrImpulsivePressureComponent,
                                                                            double drainCoefficientMean, double drainCoefficientStandardDeviation,
                                                                            double areaFlowAperturesMean, double areaFlowAperturesStandardDeviation,
                                                                            double stabilityLinearLoadModelMean, double stabilityLinearLoadModelVariation,
                                                                            double constructiveStrengthLinearLoadModelMean, double constructiveStrengthLinearLoadModelVariation)
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
                   modificationFactorDynamicOrImpulsivePressureComponent)
        {
            this.drainCoefficientMean = drainCoefficientMean;
            this.drainCoefficientStandardDeviation = drainCoefficientStandardDeviation;
            this.areaFlowAperturesMean = areaFlowAperturesMean;
            this.areaFlowAperturesStandardDeviation = areaFlowAperturesStandardDeviation;
            this.stabilityLinearLoadModelMean = stabilityLinearLoadModelMean;
            this.stabilityLinearLoadModelVariation = stabilityLinearLoadModelVariation;
            this.constructiveStrengthLinearLoadModelMean = constructiveStrengthLinearLoadModelMean;
            this.constructiveStrengthLinearLoadModelVariation = constructiveStrengthLinearLoadModelVariation;
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                var variables = base.Variables.ToList();
                variables.AddRange(GetVariables());

                return variables.OrderBy(v => v.VariableId);
            }
        }

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 424:
                    return 107;
                case 425:
                    return 113;
                case 430:
                    return 114;
                case 435:
                    return 116;
                default:
                    return null;
            }
        }

        private IEnumerable<HydraRingVariable> GetVariables()
        {
            yield return new HydraRingVariable(66, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, drainCoefficientMean,
                                               drainCoefficientStandardDeviation, double.NaN);
            yield return new HydraRingVariable(67, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, areaFlowAperturesMean,
                                               areaFlowAperturesStandardDeviation, double.NaN);
            yield return new HydraRingVariable(80, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, stabilityLinearLoadModelMean,
                                               stabilityLinearLoadModelVariation, double.NaN);
            yield return new HydraRingVariable(83, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, constructiveStrengthLinearLoadModelMean,
                                               constructiveStrengthLinearLoadModelVariation, double.NaN);
        }
    }
}