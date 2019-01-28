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
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a quadratic low sill based structures stability point calculation via Hydra-Ring.
    /// </summary>
    public class StructuresStabilityPointLowSillQuadraticCalculationInput : StructuresStabilityPointCalculationInput
    {
        private readonly double constructiveStrengthQuadraticLoadModelMean;
        private readonly double constructiveStrengthQuadraticLoadModelVariation;
        private readonly double stabilityQuadraticLoadModelMean;
        private readonly double stabilityQuadraticLoadModelVariation;
        private readonly double widthFlowAperturesMean;
        private readonly double widthFlowAperturesStandardDeviation;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresStabilityPointLowSillQuadraticCalculationInput"/>.
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
        /// <param name="constructiveStrengthQuadraticLoadModelMean">The mean of the constructive strength quadratic load model.</param>
        /// <param name="constructiveStrengthQuadraticLoadModelVariation">The variation of the constructive strength quadratic load model.</param>
        /// <param name="stabilityQuadraticLoadModelMean">The mean of the stability quadratic load model.</param>
        /// <param name="stabilityQuadraticLoadModelVariation">The variation of the stability quadratic load model.</param>
        /// <param name="widthFlowAperturesMean">The mean of the width flow apertures.</param>
        /// <param name="widthFlowAperturesStandardDeviation">The standard deviation of the width flow apertures.</param>
        public StructuresStabilityPointLowSillQuadraticCalculationInput(long hydraulicBoundaryLocationId,
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
                                                                        double modificationFactorDynamicOrImpulsivePressureComponent,
                                                                        double constructiveStrengthQuadraticLoadModelMean, double constructiveStrengthQuadraticLoadModelVariation,
                                                                        double stabilityQuadraticLoadModelMean, double stabilityQuadraticLoadModelVariation,
                                                                        double widthFlowAperturesMean, double widthFlowAperturesStandardDeviation)
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
                   modificationFactorDynamicOrImpulsivePressureComponent)
        {
            this.constructiveStrengthQuadraticLoadModelMean = constructiveStrengthQuadraticLoadModelMean;
            this.constructiveStrengthQuadraticLoadModelVariation = constructiveStrengthQuadraticLoadModelVariation;
            this.stabilityQuadraticLoadModelMean = stabilityQuadraticLoadModelMean;
            this.stabilityQuadraticLoadModelVariation = stabilityQuadraticLoadModelVariation;
            this.widthFlowAperturesMean = widthFlowAperturesMean;
            this.widthFlowAperturesStandardDeviation = widthFlowAperturesStandardDeviation;
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                List<HydraRingVariable> variables = base.Variables.ToList();
                variables.AddRange(GetVariables());

                return variables.OrderBy(v => v.VariableId);
            }
        }

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 424:
                    return 106;
                case 425:
                    return 111;
                case 430:
                    return 115;
                case 435:
                    return 117;
                default:
                    return null;
            }
        }

        private IEnumerable<HydraRingVariable> GetVariables()
        {
            yield return new LogNormalHydraRingVariable(81, HydraRingDeviationType.Variation, constructiveStrengthQuadraticLoadModelMean, constructiveStrengthQuadraticLoadModelVariation);
            yield return new LogNormalHydraRingVariable(84, HydraRingDeviationType.Variation, stabilityQuadraticLoadModelMean, stabilityQuadraticLoadModelVariation);
            yield return new NormalHydraRingVariable(106, HydraRingDeviationType.Standard, widthFlowAperturesMean, widthFlowAperturesStandardDeviation);
        }
    }
}