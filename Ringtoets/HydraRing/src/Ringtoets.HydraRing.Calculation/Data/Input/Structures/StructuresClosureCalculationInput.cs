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
    /// Container of all data necessary for performing a structures closure calculation via Hydra-Ring.
    /// </summary>
    public abstract class StructuresClosureCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection hydraRingSection;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;
        private readonly double gravitationalAcceleration;
        private readonly double factorStormDurationOpenStructure;
        private readonly double failureProbabilityOpenStructure;
        private readonly double failureProbabilityReparation;
        private readonly double identicalAperture;
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
        private readonly double stormDurationMean;
        private readonly double stormDurationVariation;
        private readonly double probabilityOpenStructureBeforeFlooding;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresClosureCalculationInput"/>.
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
        protected StructuresClosureCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
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
                                                    double hydraRingStormDurationVariation, double hydraRingProbabilityOpenStructureBeforeFlooding)
            : base(hydraulicBoundaryLocationId)
        {
            this.hydraRingSection = hydraRingSection;
            this.forelandPoints = forelandPoints;
            gravitationalAcceleration = hydraRingGravitationalAcceleration;
            factorStormDurationOpenStructure = hydraRingFactorStormDurationOpenStructure;
            failureProbabilityOpenStructure = hydraRingFailureProbabilityOpenStructure;
            failureProbabilityReparation = hydraRingFailureProbabilityReparation;
            identicalAperture = hydraRingIdenticalAperture;
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
            stormDurationMean = hydraRingStormDurationMean;
            stormDurationVariation = hydraRingStormDurationVariation;
            probabilityOpenStructureBeforeFlooding = hydraRingProbabilityOpenStructureBeforeFlooding;
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.StructuresClosure;
            }
        }

        public override int VariableId
        {
            get
            {
                return 65;
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
            // Gravitational acceleration
            yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, gravitationalAcceleration,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Factor storm duration open structure
            yield return new HydraRingVariable(63, HydraRingDistributionType.Deterministic, double.NaN,
                                               HydraRingDeviationType.Standard, factorStormDurationOpenStructure, double.NaN, double.NaN);

            // Failure propability of the open structure
            yield return new HydraRingVariable(68, HydraRingDistributionType.Deterministic, failureProbabilityOpenStructure,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Fialure propability of reparation
            yield return new HydraRingVariable(69, HydraRingDistributionType.Deterministic, failureProbabilityReparation,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Number of identical aperture per system
            yield return new HydraRingVariable(71, HydraRingDistributionType.Deterministic, identicalAperture,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

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

            // Storm duration
            yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, stormDurationMean,
                                               stormDurationVariation, double.NaN);

            // Probability open structure just before flooding occurs
            yield return new HydraRingVariable(129, HydraRingDistributionType.Deterministic, probabilityOpenStructureBeforeFlooding,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
        }
    }
}