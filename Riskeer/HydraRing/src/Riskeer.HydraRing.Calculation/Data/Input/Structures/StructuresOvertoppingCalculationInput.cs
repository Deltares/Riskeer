// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
    /// Container of all data necessary for performing a structures overtopping calculation via Hydra-Ring.
    /// </summary>
    public class StructuresOvertoppingCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly double gravitationalAcceleration;
        private readonly double modelFactorOvertoppingFlowMean;
        private readonly double modelFactorOvertoppingFlowStandardDeviation;
        private readonly double levelCrestStructureMean;
        private readonly double levelCrestStructureStandardDeviation;
        private readonly double structureNormalOrientation;
        private readonly double modelFactorSuperCriticalFlowMean;
        private readonly double modelFactorSuperCriticalFlowStandardDeviation;
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
        private readonly double widthFlowAperturesMean;
        private readonly double widthFlowAperturesStandardDeviation;
        private readonly double deviationWaveDirection;
        private readonly double stormDurationMean;
        private readonly double stormDurationVariation;

        /// <summary>
        /// Creates a new instance of the <see cref="StructuresOvertoppingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="sectionNormal">The normal of the section.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="gravitationalAcceleration">The gravitational acceleration.</param>
        /// <param name="modelFactorOvertoppingFlowMean">The mean of the model factor overtopping flow.</param>
        /// <param name="modelFactorOvertoppingFlowStandardDeviation">The standard deviation of the model factor overtopping flow.</param>
        /// <param name="levelCrestStructureMean">The mean of the crest level of the structure.</param>
        /// <param name="levelCrestStructureStandardDeviation">The standard deviation of the crest level of the structure.</param>
        /// <param name="structureNormalOrientation">The orientation of the normal of the structure.</param>
        /// <param name="modelFactorSuperCriticalFlowMean">The mean of the model factor super critical flow.</param>
        /// <param name="modelFactorSuperCriticalFlowStandardDeviation">The standard deviation of the model factor super critical flow.</param>
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
        /// <param name="widthFlowAperturesMean">The mean of the width flow apertures.</param>
        /// <param name="widthFlowAperturesStandardDeviation">The standard deviation of the width flow apertures.</param>
        /// <param name="deviationWaveDirection">The deviation of the wave direction.</param>
        /// <param name="stormDurationMean">The mean of the storm duration.</param>
        /// <param name="stormDurationVariation">The variation of the storm duration.</param>
        public StructuresOvertoppingCalculationInput(long hydraulicBoundaryLocationId,
                                                     double sectionNormal,
                                                     IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                     HydraRingBreakWater breakWater,
                                                     double gravitationalAcceleration,
                                                     double modelFactorOvertoppingFlowMean, double modelFactorOvertoppingFlowStandardDeviation,
                                                     double levelCrestStructureMean, double levelCrestStructureStandardDeviation,
                                                     double structureNormalOrientation,
                                                     double modelFactorSuperCriticalFlowMean, double modelFactorSuperCriticalFlowStandardDeviation,
                                                     double allowedLevelIncreaseStorageMean, double allowedLevelIncreaseStorageStandardDeviation,
                                                     double modelFactorStorageVolumeMean, double modelFactorStorageVolumeStandardDeviation,
                                                     double storageStructureAreaMean, double storageStructureAreaVariation,
                                                     double modelFactorInflowVolume,
                                                     double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                                                     double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeVariation,
                                                     double failureProbabilityStructureWithErosion,
                                                     double widthFlowAperturesMean, double widthFlowAperturesStandardDeviation,
                                                     double deviationWaveDirection,
                                                     double stormDurationMean, double stormDurationVariation)
            : base(hydraulicBoundaryLocationId)
        {
            Section = new HydraRingSection(1, double.NaN, sectionNormal);
            ForelandsPoints = forelandPoints;
            BreakWater = breakWater;
            this.gravitationalAcceleration = gravitationalAcceleration;
            this.modelFactorOvertoppingFlowMean = modelFactorOvertoppingFlowMean;
            this.modelFactorOvertoppingFlowStandardDeviation = modelFactorOvertoppingFlowStandardDeviation;
            this.levelCrestStructureMean = levelCrestStructureMean;
            this.levelCrestStructureStandardDeviation = levelCrestStructureStandardDeviation;
            this.structureNormalOrientation = structureNormalOrientation;
            this.modelFactorSuperCriticalFlowMean = modelFactorSuperCriticalFlowMean;
            this.modelFactorSuperCriticalFlowStandardDeviation = modelFactorSuperCriticalFlowStandardDeviation;
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
            this.widthFlowAperturesMean = widthFlowAperturesMean;
            this.widthFlowAperturesStandardDeviation = widthFlowAperturesStandardDeviation;
            this.deviationWaveDirection = deviationWaveDirection;
            this.stormDurationMean = stormDurationMean;
            this.stormDurationVariation = stormDurationVariation;
        }

        public override HydraRingFailureMechanismType FailureMechanismType { get; } = HydraRingFailureMechanismType.StructuresOvertopping;

        public override int VariableId { get; } = 60;

        public override HydraRingSection Section { get; }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield return new DeterministicHydraRingVariable(58, gravitationalAcceleration);
                yield return new LogNormalHydraRingVariable(59, HydraRingDeviationType.Standard, modelFactorOvertoppingFlowMean, modelFactorOvertoppingFlowStandardDeviation);
                yield return new NormalHydraRingVariable(60, HydraRingDeviationType.Standard, levelCrestStructureMean, levelCrestStructureStandardDeviation);
                yield return new DeterministicHydraRingVariable(61, structureNormalOrientation);
                yield return new NormalHydraRingVariable(62, HydraRingDeviationType.Standard, modelFactorSuperCriticalFlowMean, modelFactorSuperCriticalFlowStandardDeviation);
                yield return new LogNormalHydraRingVariable(94, HydraRingDeviationType.Standard, allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation);
                yield return new LogNormalHydraRingVariable(95, HydraRingDeviationType.Standard, modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation);
                yield return new LogNormalHydraRingVariable(96, HydraRingDeviationType.Variation, storageStructureAreaMean, storageStructureAreaVariation);
                yield return new DeterministicHydraRingVariable(97, modelFactorInflowVolume);
                yield return new LogNormalHydraRingVariable(103, HydraRingDeviationType.Standard, flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation);
                yield return new LogNormalHydraRingVariable(104, HydraRingDeviationType.Variation, criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation);
                yield return new DeterministicHydraRingVariable(105, failureProbabilityStructureWithErosion);
                yield return new NormalHydraRingVariable(106, HydraRingDeviationType.Standard, widthFlowAperturesMean, widthFlowAperturesStandardDeviation);
                yield return new DeterministicHydraRingVariable(107, deviationWaveDirection);
                yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, stormDurationMean, stormDurationVariation);
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
    }
}