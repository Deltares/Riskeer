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
using Ringtoets.HydraRing.Calculation.Data.Variables;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a vertical wall based structures closure calculation via Hydra-Ring.
    /// </summary>
    public class StructuresClosureVerticalWallCalculationInput : StructuresClosureCalculationInput
    {
        private readonly double modelFactorOvertoppingFlowMean;
        private readonly double modelFactorOvertoppingFlowStandardDeviation;
        private readonly double structureNormalOrientation;
        private readonly double modelFactorSuperCriticalFlowMean;
        private readonly double modelFactorSuperCriticalFlowStandardDeviation;
        private readonly double levelCrestStructureNotClosingMean;
        private readonly double levelCrestStructureNotClosingStandardDeviation;
        private readonly double widthFlowAperturesMean;
        private readonly double widthFlowAperturesStandardDeviation;
        private readonly double deviationWaveDirection;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresClosureVerticalWallCalculationInput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="sectionNormal">The normal of the section.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="gravitationalAcceleration">The gravitational acceleration.</param>
        /// <param name="factorStormDurationOpenStructure">The factor of the storm duration for an open structure.</param>
        /// <param name="failureProbabilityOpenStructure">The failure probability for an open structure.</param>
        /// <param name="failureProbabilityReparation">The reparation failure probability.</param>
        /// <param name="identicalApertures">The number of identical apertures.</param>
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
        /// <param name="probabilityOpenStructureBeforeFlooding">The probability of an open structure before flooding.</param>
        /// <param name="modelFactorOvertoppingFlowMean">The mean of the model factor overtopping flow.</param>
        /// <param name="modelFactorOvertoppingFlowStandardDeviation">The standard deviation of the model factor overtopping flow.</param>
        /// <param name="structureNormalOrientation">The orientation of the normal of the structure.</param>
        /// <param name="modelFactorSuperCriticalFlowMean">The mean of the model factor super critical flow.</param>
        /// <param name="modelFactorSuperCriticalFlowStandardDeviation">The standard deviation of the model factor super critical flow.</param>
        /// <param name="levelCrestStructureNotClosingMean">The mean of the crest level of the structure when not closing.</param>
        /// <param name="levelCrestStructureNotClosingStandardDeviation">The standard deviation of the crest level of the structure when not closing.</param>
        /// <param name="widthFlowAperturesMean">The mean of the width flow apertures.</param>
        /// <param name="widthFlowAperturesStandardDeviation">The standard deviation of the width flow apertures.</param>
        /// <param name="deviationWaveDirection">The deviation of the wave direction.</param>
        public StructuresClosureVerticalWallCalculationInput(long hydraulicBoundaryLocationId,
                                                             double sectionNormal,
                                                             IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                             HydraRingBreakWater breakWater,
                                                             double gravitationalAcceleration,
                                                             double factorStormDurationOpenStructure,
                                                             double failureProbabilityOpenStructure,
                                                             double failureProbabilityReparation,
                                                             int identicalApertures,
                                                             double allowedLevelIncreaseStorageMean, double allowedLevelIncreaseStorageStandardDeviation,
                                                             double modelFactorStorageVolumeMean, double modelFactorStorageVolumeStandardDeviation,
                                                             double storageStructureAreaMean, double storageStructureAreaVariation,
                                                             double modelFactorInflowVolume,
                                                             double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                                                             double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeVariation,
                                                             double failureProbabilityStructureWithErosion,
                                                             double stormDurationMean, double stormDurationVariation,
                                                             double probabilityOpenStructureBeforeFlooding,
                                                             double modelFactorOvertoppingFlowMean, double modelFactorOvertoppingFlowStandardDeviation,
                                                             double structureNormalOrientation,
                                                             double modelFactorSuperCriticalFlowMean, double modelFactorSuperCriticalFlowStandardDeviation,
                                                             double levelCrestStructureNotClosingMean, double levelCrestStructureNotClosingStandardDeviation,
                                                             double widthFlowAperturesMean, double widthFlowAperturesStandardDeviation,
                                                             double deviationWaveDirection)
            : base(hydraulicBoundaryLocationId,
                   sectionNormal,
                   forelandPoints, breakWater,
                   gravitationalAcceleration,
                   factorStormDurationOpenStructure,
                   failureProbabilityOpenStructure,
                   failureProbabilityReparation,
                   identicalApertures,
                   allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                   modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                   storageStructureAreaMean, storageStructureAreaVariation,
                   modelFactorInflowVolume,
                   flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                   criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                   failureProbabilityStructureWithErosion,
                   stormDurationMean, stormDurationVariation,
                   probabilityOpenStructureBeforeFlooding)
        {
            this.modelFactorOvertoppingFlowMean = modelFactorOvertoppingFlowMean;
            this.modelFactorOvertoppingFlowStandardDeviation = modelFactorOvertoppingFlowStandardDeviation;
            this.structureNormalOrientation = structureNormalOrientation;
            this.modelFactorSuperCriticalFlowMean = modelFactorSuperCriticalFlowMean;
            this.modelFactorSuperCriticalFlowStandardDeviation = modelFactorSuperCriticalFlowStandardDeviation;
            this.levelCrestStructureNotClosingMean = levelCrestStructureNotClosingMean;
            this.levelCrestStructureNotClosingStandardDeviation = levelCrestStructureNotClosingStandardDeviation;
            this.widthFlowAperturesMean = widthFlowAperturesMean;
            this.widthFlowAperturesStandardDeviation = widthFlowAperturesStandardDeviation;
            this.deviationWaveDirection = deviationWaveDirection;
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
                    return 105;
                case 425:
                    return 109;
                default:
                    return null;
            }
        }

        private IEnumerable<HydraRingVariable> GetVariables()
        {
            yield return new LogNormalHydraRingVariable(59, HydraRingDeviationType.Standard, modelFactorOvertoppingFlowMean, modelFactorOvertoppingFlowStandardDeviation);
            yield return new DeterministicHydraRingVariable(61, structureNormalOrientation);
            yield return new NormalHydraRingVariable(62, HydraRingDeviationType.Standard, modelFactorSuperCriticalFlowMean, modelFactorSuperCriticalFlowStandardDeviation);
            yield return new NormalHydraRingVariable(72, HydraRingDeviationType.Standard, levelCrestStructureNotClosingMean, levelCrestStructureNotClosingStandardDeviation);
            yield return new NormalHydraRingVariable(106, HydraRingDeviationType.Standard, widthFlowAperturesMean, widthFlowAperturesStandardDeviation);
            yield return new DeterministicHydraRingVariable(107, deviationWaveDirection);
        }
    }
}