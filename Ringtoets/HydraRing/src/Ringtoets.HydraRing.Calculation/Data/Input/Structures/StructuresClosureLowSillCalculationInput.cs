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
using System.Linq;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a low sill based structures closure calculation via Hydra-Ring.
    /// </summary>
    public class StructuresClosureLowSillCalculationInput : StructuresClosureCalculationInput
    {
        private readonly double modelFactorOvertoppingSuperCriticalFlowMean;
        private readonly double modelFactorOvertoppingSuperCriticalFlowStandardDeviation;
        private readonly double modelFactorSubCriticalFlowMean;
        private readonly double modelFactorSubCriticalFlowVariation;
        private readonly double thresholdHeightOpenWeirMean;
        private readonly double thresholdHeightOpenWeirStandardDeviation;
        private readonly double insideWaterLevelMean;
        private readonly double insideWaterLevelStandardDeviation;
        private readonly double widthOfFlowAperturesMean;
        private readonly double widthOfFlowAperturesVariation;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresClosureLowSillCalculationInput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="hydraRingSection">The section to use during the calculation.</param>
        /// <param name="forelandPoints">The foreland points to use during the calculation.</param>
        /// <param name="hydraRingGravitationalAcceleration">The gravitational acceleration to use during the calculation.</param>
        /// <param name="hydraRingFactorStormDurationOpenStructure">The factor of the storm duration for an open structure to use during the calculation.</param>
        /// <param name="hydraRingFailureProbabilityOpenStructure">The failure probability for an open structure to use during the calculation.</param>
        /// <param name="hydraRingFailureProbabilityReparation">The reparation failure probabilty to use during the calculation.</param>
        /// <param name="hydraRingIdenticalAperture">The identical aperture to use during the calculation.</param>
        /// <param name="hydraRingAllowableIncreaseOfLevelForStorageMean">The mean of the allowable increase of the level for the storage to use during the calculation.</param>
        /// <param name="hydraRingAllowableIncreaseOfLevelForStorageStandardDeviation">The standard deviation of the allowable increase of the level for the storage to use during the calculation.</param>
        /// <param name="hydraRingModelFactorForStorageVolumeMean">The mean of the model factor for the storage volume to use during the calculation.</param>
        /// <param name="hydraRingModelFactorForStorageVolumeStandardDeviation">The standard deviation of the model factor for the storage volume to use during the calculation.</param>
        /// <param name="hydraRingStorageStructureAreaMean">The mean of the storage structure area to use during the calculation.</param>
        /// <param name="hydraRingStorageStructureAreaVariation">The variation of the storage structure area to use during the calculation.</param>
        /// <param name="hydraRingModelFactorForIncomingFlowVolume">The model factor for incoming flow volume to use during the calculation.</param>
        /// <param name="hydraRingFlowWidthAtBottomProtectionMean">The mean of the flow width at bottom protection to use during the calculation.</param>
        /// <param name="hydraRingFlowWidthAtBottomProtectionStandardDeviation">The standard deviation of the flow width at bottom protection to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingDischargeMean">The mean of the critical overtopping discharge to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingDischargeVariation">The variation of the critical overtopping discharge to use during the calculation.</param>
        /// <param name="hydraRingFailureProbabilityOfStructureGivenErosion">The failure probability of structure given erosion to use during the calculation.</param>
        /// <param name="hydraRingStormDurationMean">The mean of the storm duration to use during the calculation.</param>
        /// <param name="hydraRingStormDurationVariation">The variation of the storm duration to use during the calculation.</param>
        /// <param name="hydraRingProbabilityOpenStructureBeforeFlooding">The propability of an open structure before flooding to use during the calculation.</param>
        /// <param name="hydraRingModelFactorSubCriticalFlowMean">The mean of the model factor sub citrical flow to use during the calculation.</param>
        /// <param name="hydraRingModelFactorSubCriticalFlowVariation">The variation of the model factor sub citrical flow to use during the calculation.</param>
        /// <param name="hydraRingThresholdHeightOpenWeirMean">The mean of the threshold height open weir to use during the calculation.</param>
        /// <param name="hydraRingThresholdHeightOpenWeirStandardDeviation">The standard deviation of the threshold height open weir to use during the calculation.</param>
        /// <param name="hydraRingInsideWaterLevelMean">The mean of the inside water level to use during the calculation.</param>
        /// <param name="hydraRingInsideWaterLevelStandardDeviation">The standard deviation of the inside water level to use during the calculation.</param>
        /// <param name="hydraRingModelFactorOvertoppingSuperCriticalFlowMean">The mean of the model factor overtopping super critical flow to use during the calculation.</param>
        /// <param name="hydraRingModelFactorOvertoppingSuperCriticalFlowStandardDeviation">The standard deviation of the model factor overtopping super critical flow to use during the calculation.</param>
        /// <param name="hydraRingWidthOfFlowAperturesMean">The mean of the width of flow apertures to use during the calculation.</param>
        /// <param name="hydraRingWidthOfFlowAperturesVariation">The variation of the width of flow apertures to use during the calculation.</param>
        public StructuresClosureLowSillCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                                        IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                        double hydraRingGravitationalAcceleration, double hydraRingFactorStormDurationOpenStructure,
                                                        double hydraRingFailureProbabilityOpenStructure, double hydraRingFailureProbabilityReparation,
                                                        double hydraRingIdenticalAperture, double hydraRingAllowableIncreaseOfLevelForStorageMean,
                                                        double hydraRingAllowableIncreaseOfLevelForStorageStandardDeviation, double hydraRingModelFactorForStorageVolumeMean,
                                                        double hydraRingModelFactorForStorageVolumeStandardDeviation, double hydraRingStorageStructureAreaMean,
                                                        double hydraRingStorageStructureAreaVariation, double hydraRingModelFactorForIncomingFlowVolume,
                                                        double hydraRingFlowWidthAtBottomProtectionMean, double hydraRingFlowWidthAtBottomProtectionStandardDeviation,
                                                        double hydraRingCriticalOvertoppingDischargeMean, double hydraRingCriticalOvertoppingDischargeVariation,
                                                        double hydraRingFailureProbabilityOfStructureGivenErosion, double hydraRingStormDurationMean,
                                                        double hydraRingStormDurationVariation, double hydraRingProbabilityOpenStructureBeforeFlooding,
                                                        double hydraRingModelFactorSubCriticalFlowMean, double hydraRingModelFactorSubCriticalFlowVariation,
                                                        double hydraRingThresholdHeightOpenWeirMean, double hydraRingThresholdHeightOpenWeirStandardDeviation,
                                                        double hydraRingInsideWaterLevelMean, double hydraRingInsideWaterLevelStandardDeviation,
                                                        double hydraRingModelFactorOvertoppingSuperCriticalFlowMean,
                                                        double hydraRingModelFactorOvertoppingSuperCriticalFlowStandardDeviation,
                                                        double hydraRingWidthOfFlowAperturesMean, double hydraRingWidthOfFlowAperturesVariation)
            : base(hydraulicBoundaryLocationId, hydraRingSection, forelandPoints,
                   hydraRingGravitationalAcceleration, hydraRingFactorStormDurationOpenStructure,
                   hydraRingFailureProbabilityOpenStructure, hydraRingFailureProbabilityReparation,
                   hydraRingIdenticalAperture, hydraRingAllowableIncreaseOfLevelForStorageMean,
                   hydraRingAllowableIncreaseOfLevelForStorageStandardDeviation, hydraRingModelFactorForStorageVolumeMean,
                   hydraRingModelFactorForStorageVolumeStandardDeviation, hydraRingStorageStructureAreaMean,
                   hydraRingStorageStructureAreaVariation, hydraRingModelFactorForIncomingFlowVolume,
                   hydraRingFlowWidthAtBottomProtectionMean, hydraRingFlowWidthAtBottomProtectionStandardDeviation,
                   hydraRingCriticalOvertoppingDischargeMean, hydraRingCriticalOvertoppingDischargeVariation,
                   hydraRingFailureProbabilityOfStructureGivenErosion, hydraRingStormDurationMean,
                   hydraRingStormDurationVariation, hydraRingProbabilityOpenStructureBeforeFlooding)
        {
            modelFactorOvertoppingSuperCriticalFlowMean = hydraRingModelFactorOvertoppingSuperCriticalFlowMean;
            modelFactorOvertoppingSuperCriticalFlowStandardDeviation = hydraRingModelFactorOvertoppingSuperCriticalFlowStandardDeviation;
            modelFactorSubCriticalFlowMean = hydraRingModelFactorSubCriticalFlowMean;
            modelFactorSubCriticalFlowVariation = hydraRingModelFactorSubCriticalFlowVariation;
            thresholdHeightOpenWeirMean = hydraRingThresholdHeightOpenWeirMean;
            thresholdHeightOpenWeirStandardDeviation = hydraRingThresholdHeightOpenWeirStandardDeviation;
            insideWaterLevelMean = hydraRingInsideWaterLevelMean;
            insideWaterLevelStandardDeviation = hydraRingInsideWaterLevelStandardDeviation;
            widthOfFlowAperturesMean = hydraRingWidthOfFlowAperturesMean;
            widthOfFlowAperturesVariation = hydraRingWidthOfFlowAperturesVariation;
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
            // Model factor overtopping super critical flow
            yield return new HydraRingVariable(62, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorOvertoppingSuperCriticalFlowMean,
                                               modelFactorOvertoppingSuperCriticalFlowStandardDeviation, double.NaN);

            // Model factor sub critical flow
            yield return new HydraRingVariable(64, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Variation, modelFactorSubCriticalFlowMean,
                                               modelFactorSubCriticalFlowVariation, double.NaN);

            // Threshold height open weir
            yield return new HydraRingVariable(65, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, thresholdHeightOpenWeirMean,
                                               thresholdHeightOpenWeirStandardDeviation, double.NaN);

            // Inside water level
            yield return new HydraRingVariable(93, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, insideWaterLevelMean,
                                               insideWaterLevelStandardDeviation, double.NaN);

            // Width of flow apertures
            yield return new HydraRingVariable(106, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Variation, widthOfFlowAperturesMean,
                                               widthOfFlowAperturesVariation, double.NaN);
        }
    }
}