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
        private readonly double gravitationalAcceleration;
        private readonly double modelFactorOvertoppingFlowMean;
        private readonly double modelFactorOvertoppingFlowStandardDeviation;
        private readonly double levelOfCrestOfStructureMean;
        private readonly double levelOfCrestOfStructureStandardDeviation;
        private readonly double structureNormalOrientation;
        private readonly double modelFactorOvertoppingSupercriticalFlowMean;
        private readonly double modelFactorOvertoppingSupercriticalFlowStandardDeviation;
        private readonly double allowableIncreaseOfLevelForStorageMean;
        private readonly double allowableIncreaseOfLevelForStorageStandardDeviation;
        private readonly double modelFactorForStorageVolumeMean;
        private readonly double modelFactorForStorageVolumeStandardDeviation;
        private readonly double storageStructureAreaMean;
        private readonly double storageStructureAreaVariation;
        private readonly double modelFactorForIncomingFlowVolume;
        private readonly double flowWidthAtBottomProtectionMean;
        private readonly double flowWidthAtBottomProtectionStandardDeviation;
        private readonly double criticalOvertoppingDischargeMean;
        private readonly double criticalOvertoppingDischargeVariation;
        private readonly double failureProbabilityOfStructureGivenErosion;
        private readonly double widthOfFlowAperturesMean;
        private readonly double widthOfFlowAperturesVariation;
        private readonly double deviationOfTheWaveDirection;
        private readonly double stormDurationMean;
        private readonly double stormDurationVariation;

        /// <summary>
        /// Creates a new instance of the <see cref="StructuresOvertoppingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="hydraRingSection">The section to use during the calculation.</param>
        /// <param name="hydraRingGravitationalAcceleration">The gravitational acceleration to use during the calculation.</param>
        /// <param name="hydraRingModelFactorOvertoppingFlowMean">The mean of the model factor overtopping flow to use during the calculation.</param>
        /// <param name="hydraRingModelFactorOvertoppingFlowStandardDeviation">The standard deviation of the model factor overtopping flow to use during the calculation.</param>
        /// <param name="hydraRingLevelOfCrestOfStructureMean">The mean of the level of the crest of the structure to use during the calculation.</param>
        /// <param name="hydraRingLevelOfCrestOfStructureStandardDeviation">The standard deviation of the level of the crest of the structure to use during the calculation.</param>
        /// <param name="hydraRingStructureNormalOrientation">The orientation of the normal of the structure to use during the calculation.</param>
        /// <param name="hydraRingModelFactorOvertoppingSupercriticalFlowMean">The mean of the model factor overtopping supercritical flow to use during the calculation.</param>
        /// <param name="hydraRingModelFactorOvertoppingSupercriticalFlowStandardDeviation">The standard deviation of the model factor overtopping supercritical flow to use during the calculation.</param>
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
        /// <param name="hydraRingWidthOfFlowAperturesMean">The mean of the width of flow apertures to use during the calculation.</param>
        /// <param name="hydraRingWidthOfFlowAperturesVariation">The variation of the width of flow apertures to use during the calculation.</param>
        /// <param name="hydraRingDeviationOfTheWaveDirection">The deviation of the wave direction to use during the calculation.</param>
        /// <param name="hydraRingStormDurationMean">The mean of the storm duration to use during the calculation.</param>
        /// <param name="hydraRingStormDurationVariation">The variation of the storm duration to use during the calculation.</param>
        public StructuresOvertoppingCalculationInput(int hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                                     double hydraRingGravitationalAcceleration,
                                                     double hydraRingModelFactorOvertoppingFlowMean, double hydraRingModelFactorOvertoppingFlowStandardDeviation,
                                                     double hydraRingLevelOfCrestOfStructureMean, double hydraRingLevelOfCrestOfStructureStandardDeviation,
                                                     double hydraRingStructureNormalOrientation,
                                                     double hydraRingModelFactorOvertoppingSupercriticalFlowMean, double hydraRingModelFactorOvertoppingSupercriticalFlowStandardDeviation,
                                                     double hydraRingAllowableIncreaseOfLevelForStorageMean, double hydraRingAllowableIncreaseOfLevelForStorageStandardDeviation,
                                                     double hydraRingModelFactorForStorageVolumeMean, double hydraRingModelFactorForStorageVolumeStandardDeviation,
                                                     double hydraRingStorageStructureAreaMean, double hydraRingStorageStructureAreaVariation,
                                                     double hydraRingModelFactorForIncomingFlowVolume,
                                                     double hydraRingFlowWidthAtBottomProtectionMean, double hydraRingFlowWidthAtBottomProtectionStandardDeviation,
                                                     double hydraRingCriticalOvertoppingDischargeMean, double hydraRingCriticalOvertoppingDischargeVariation,
                                                     double hydraRingFailureProbabilityOfStructureGivenErosion,
                                                     double hydraRingWidthOfFlowAperturesMean, double hydraRingWidthOfFlowAperturesVariation,
                                                     double hydraRingDeviationOfTheWaveDirection,
                                                     double hydraRingStormDurationMean, double hydraRingStormDurationVariation
            )
            : base(hydraulicBoundaryLocationId)
        {
            section = hydraRingSection;
            gravitationalAcceleration = hydraRingGravitationalAcceleration;
            modelFactorOvertoppingFlowMean = hydraRingModelFactorOvertoppingFlowMean;
            modelFactorOvertoppingFlowStandardDeviation = hydraRingModelFactorOvertoppingFlowStandardDeviation;
            levelOfCrestOfStructureMean = hydraRingLevelOfCrestOfStructureMean;
            levelOfCrestOfStructureStandardDeviation = hydraRingLevelOfCrestOfStructureStandardDeviation;
            structureNormalOrientation = hydraRingStructureNormalOrientation;
            modelFactorOvertoppingSupercriticalFlowMean = hydraRingModelFactorOvertoppingSupercriticalFlowMean;
            modelFactorOvertoppingSupercriticalFlowStandardDeviation = hydraRingModelFactorOvertoppingSupercriticalFlowStandardDeviation;
            allowableIncreaseOfLevelForStorageMean = hydraRingAllowableIncreaseOfLevelForStorageMean;
            allowableIncreaseOfLevelForStorageStandardDeviation = hydraRingAllowableIncreaseOfLevelForStorageStandardDeviation;
            modelFactorForStorageVolumeMean = hydraRingModelFactorForStorageVolumeMean;
            modelFactorForStorageVolumeStandardDeviation = hydraRingModelFactorForStorageVolumeStandardDeviation;
            storageStructureAreaMean = hydraRingStorageStructureAreaMean;
            storageStructureAreaVariation = hydraRingStorageStructureAreaVariation;
            modelFactorForIncomingFlowVolume = hydraRingModelFactorForIncomingFlowVolume;
            flowWidthAtBottomProtectionMean = hydraRingFlowWidthAtBottomProtectionMean;
            flowWidthAtBottomProtectionStandardDeviation = hydraRingFlowWidthAtBottomProtectionStandardDeviation;
            criticalOvertoppingDischargeMean = hydraRingCriticalOvertoppingDischargeMean;
            criticalOvertoppingDischargeVariation = hydraRingCriticalOvertoppingDischargeVariation;
            failureProbabilityOfStructureGivenErosion = hydraRingFailureProbabilityOfStructureGivenErosion;
            widthOfFlowAperturesMean = hydraRingWidthOfFlowAperturesMean;
            widthOfFlowAperturesVariation = hydraRingWidthOfFlowAperturesVariation;
            deviationOfTheWaveDirection = hydraRingDeviationOfTheWaveDirection;
            stormDurationMean = hydraRingStormDurationMean;
            stormDurationVariation = hydraRingStormDurationVariation;
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
                return GetHydraRingVariables();
            }
        }

        private IEnumerable<HydraRingVariable> GetHydraRingVariables()
        {
            // Gravitational acceleration
            yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, gravitationalAcceleration,
                                               HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN);

            // Model factor overtopping flow
            yield return new HydraRingVariable(59, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorOvertoppingFlowMean,
                                               modelFactorOvertoppingFlowStandardDeviation, double.NaN);

            // Level of crest of structure
            yield return new HydraRingVariable(60, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, levelOfCrestOfStructureMean,
                                               levelOfCrestOfStructureStandardDeviation, double.NaN);

            // Orientation of the normal of the structure
            yield return new HydraRingVariable(61, HydraRingDistributionType.Deterministic, structureNormalOrientation,
                                               HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN);

            // Model factor overtopping supercritical flow
            yield return new HydraRingVariable(62, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorOvertoppingSupercriticalFlowMean,
                                               modelFactorOvertoppingSupercriticalFlowStandardDeviation, double.NaN);

            // Allowable increase of level for storage
            yield return new HydraRingVariable(94, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, allowableIncreaseOfLevelForStorageMean,
                                               allowableIncreaseOfLevelForStorageStandardDeviation, double.NaN);

            // Model factor for storage volume
            yield return new HydraRingVariable(95, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorForStorageVolumeMean,
                                               modelFactorForStorageVolumeStandardDeviation, double.NaN);

            // Storage structure area
            yield return new HydraRingVariable(96, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, storageStructureAreaMean,
                                               storageStructureAreaVariation, double.NaN);

            // Model factor for incoming flow volume
            yield return new HydraRingVariable(97, HydraRingDistributionType.Deterministic, modelFactorForIncomingFlowVolume,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Flow width at bottom protection
            yield return new HydraRingVariable(103, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, flowWidthAtBottomProtectionMean,
                                               flowWidthAtBottomProtectionStandardDeviation, double.NaN);

            // Critical overtopping discharge
            yield return new HydraRingVariable(104, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, criticalOvertoppingDischargeMean,
                                               criticalOvertoppingDischargeVariation, double.NaN);

            // Failure probability of structure given erosion
            yield return new HydraRingVariable(105, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, failureProbabilityOfStructureGivenErosion, 0, double.NaN);

            // Width of flow apertures
            yield return new HydraRingVariable(106, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Variation, widthOfFlowAperturesMean,
                                               widthOfFlowAperturesVariation, double.NaN);

            // Deviation of the wave direction
            yield return new HydraRingVariable(107, HydraRingDistributionType.Deterministic, deviationOfTheWaveDirection,
                                               HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN);

            // Storm duration
            yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, stormDurationMean,
                                               stormDurationVariation, double.NaN);
        }
    }
}