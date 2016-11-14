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
    /// Container of all data necessary for performing a structures overtopping calculation via Hydra-Ring.
    /// </summary>
    public class StructuresOvertoppingCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection section;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;
        private readonly HydraRingBreakWater breakWater;
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
        private readonly double widthFlowAperturesVariation;
        private readonly double deviationWaveDirection;
        private readonly double stormDurationMean;
        private readonly double stormDurationVariation;

        /// <summary>
        /// Creates a new instance of the <see cref="StructuresOvertoppingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="section">The section.</param>
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
        /// <param name="widthFlowAperturesVariation">The variation of the width flow apertures.</param>
        /// <param name="deviationWaveDirection">The deviation of the wave direction.</param>
        /// <param name="stormDurationMean">The mean of the storm duration.</param>
        /// <param name="stormDurationVariation">The variation of the storm duration.</param>
        public StructuresOvertoppingCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection section,
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
                                                     double widthFlowAperturesMean, double widthFlowAperturesVariation,
                                                     double deviationWaveDirection,
                                                     double stormDurationMean, double stormDurationVariation)
            : base(hydraulicBoundaryLocationId)
        {
            this.section = section;
            this.forelandPoints = forelandPoints;
            this.breakWater = breakWater;
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
            this.widthFlowAperturesVariation = widthFlowAperturesVariation;
            this.deviationWaveDirection = deviationWaveDirection;
            this.stormDurationMean = stormDurationMean;
            this.stormDurationVariation = stormDurationVariation;
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.StructuresOvertopping;
            }
        }

        public override int VariableId
        {
            get
            {
                return 60;
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
                yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, gravitationalAcceleration,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(59, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Standard, modelFactorOvertoppingFlowMean,
                                                   modelFactorOvertoppingFlowStandardDeviation, double.NaN);
                yield return new HydraRingVariable(60, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, levelCrestStructureMean,
                                                   levelCrestStructureStandardDeviation, double.NaN);
                yield return new HydraRingVariable(61, HydraRingDistributionType.Deterministic, structureNormalOrientation,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(62, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, modelFactorSuperCriticalFlowMean,
                                                   modelFactorSuperCriticalFlowStandardDeviation, double.NaN);
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
                yield return new HydraRingVariable(105, HydraRingDistributionType.Deterministic, failureProbabilityStructureWithErosion,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(106, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Variation, widthFlowAperturesMean,
                                                   widthFlowAperturesVariation, double.NaN);
                yield return new HydraRingVariable(107, HydraRingDistributionType.Deterministic, deviationWaveDirection,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN,
                                                   HydraRingDeviationType.Variation, stormDurationMean,
                                                   stormDurationVariation, double.NaN);
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
    }
}