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
using Ringtoets.HydraRing.Calculation.Data.Variables;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a low sill based structures closure calculation via Hydra-Ring.
    /// </summary>
    public class StructuresClosureLowSillCalculationInput : StructuresClosureCalculationInput
    {
        private readonly double modelFactorSuperCriticalFlowMean;
        private readonly double modelFactorSuperCriticalFlowStandardDeviation;
        private readonly double modelFactorSubCriticalFlowMean;
        private readonly double modelFactorSubCriticalFlowVariation;
        private readonly double thresholdHeightOpenWeirMean;
        private readonly double thresholdHeightOpenWeirStandardDeviation;
        private readonly double insideWaterLevelMean;
        private readonly double insideWaterLevelStandardDeviation;
        private readonly double widthFlowAperturesMean;
        private readonly double widthFlowAperturesVariation;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresClosureLowSillCalculationInput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="section">The section.</param>
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
        /// <param name="probabilityOrFrequencyOpenStructureBeforeFlooding">The probability or frequency of an open structure before flooding.</param>
        /// <param name="modelFactorSuperCriticalFlowMean">The mean of the model factor super critical flow.</param>
        /// <param name="modelFactorSuperCriticalFlowStandardDeviation">The standard deviation of the model factor super critical flow.</param>
        /// <param name="modelFactorSubCriticalFlowMean">The mean of the model factor sub critical flow.</param>
        /// <param name="modelFactorSubCriticalFlowVariation">The variation of the model factor sub critical flow.</param>
        /// <param name="thresholdHeightOpenWeirMean">The mean of the threshold height open weir.</param>
        /// <param name="thresholdHeightOpenWeirStandardDeviation">The standard deviation of the threshold height open weir.</param>
        /// <param name="insideWaterLevelMean">The mean of the inside water level.</param>
        /// <param name="insideWaterLevelStandardDeviation">The standard deviation of the inside water level.</param>
        /// <param name="widthFlowAperturesMean">The mean of the width flow apertures.</param>
        /// <param name="widthFlowAperturesVariation">The variation of the width flow apertures.</param>
        public StructuresClosureLowSillCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection section,
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
                                                        double probabilityOrFrequencyOpenStructureBeforeFlooding,
                                                        double modelFactorSuperCriticalFlowMean, double modelFactorSuperCriticalFlowStandardDeviation,
                                                        double modelFactorSubCriticalFlowMean, double modelFactorSubCriticalFlowVariation,
                                                        double thresholdHeightOpenWeirMean, double thresholdHeightOpenWeirStandardDeviation,
                                                        double insideWaterLevelMean, double insideWaterLevelStandardDeviation,
                                                        double widthFlowAperturesMean, double widthFlowAperturesVariation)
            : base(hydraulicBoundaryLocationId, section,
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
                   probabilityOrFrequencyOpenStructureBeforeFlooding)
        {
            this.modelFactorSuperCriticalFlowMean = modelFactorSuperCriticalFlowMean;
            this.modelFactorSuperCriticalFlowStandardDeviation = modelFactorSuperCriticalFlowStandardDeviation;
            this.modelFactorSubCriticalFlowMean = modelFactorSubCriticalFlowMean;
            this.modelFactorSubCriticalFlowVariation = modelFactorSubCriticalFlowVariation;
            this.thresholdHeightOpenWeirMean = thresholdHeightOpenWeirMean;
            this.thresholdHeightOpenWeirStandardDeviation = thresholdHeightOpenWeirStandardDeviation;
            this.insideWaterLevelMean = insideWaterLevelMean;
            this.insideWaterLevelStandardDeviation = insideWaterLevelStandardDeviation;
            this.widthFlowAperturesMean = widthFlowAperturesMean;
            this.widthFlowAperturesVariation = widthFlowAperturesVariation;
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
                    return 106;
                case 425:
                    return 111;
                default:
                    return null;
            }
        }

        private IEnumerable<HydraRingVariable> GetVariables()
        {
            yield return new NormalHydraRingVariable(62, HydraRingDeviationType.Standard, modelFactorSuperCriticalFlowMean, modelFactorSuperCriticalFlowStandardDeviation);
            yield return new NormalHydraRingVariable(64, HydraRingDeviationType.Variation, modelFactorSubCriticalFlowMean, modelFactorSubCriticalFlowVariation);
            yield return new NormalHydraRingVariable(65, HydraRingDeviationType.Standard, thresholdHeightOpenWeirMean, thresholdHeightOpenWeirStandardDeviation);
            yield return new NormalHydraRingVariable(93, HydraRingDeviationType.Standard, insideWaterLevelMean, insideWaterLevelStandardDeviation);
            yield return new NormalHydraRingVariable(106, HydraRingDeviationType.Variation, widthFlowAperturesMean, widthFlowAperturesVariation);
        }
    }
}