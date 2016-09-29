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
        /// <param name="hydraRingLevelCrestStructureMean">The mean of the level crest structure to use during the calculation.</param>
        /// <param name="hydraRingLevelCrestStructureStandardDeviation">The standard deviation of the level crest structure to use during the calculation.</param>
        /// <param name="hydraRingStructureNormalOrientation">The orientation of the normal of the structure to use during the calculation.</param>
        /// <param name="hydraRingModelFactorSuperCriticalFlowMean">The mean of the model factor super critical flow to use during the calculation.</param>
        /// <param name="hydraRingModelFactorSuperCriticalFlowStandardDeviation">The standard deviation of the model factor super critical flow to use during the calculation.</param>
        /// <param name="hydraRingAllowedLevelIncreaseStorageMean">The mean of the allowed increase of the level for the storage to use during the calculation.</param>
        /// <param name="hydraRingAllowedLevelIncreaseStorageStandardDeviation">The standard deviation of the allowed increase of the level for the storage to use during the calculation.</param>
        /// <param name="hydraRingModelFactorStorageVolumeMean">The mean of the model factor for the storage volume to use during the calculation.</param>
        /// <param name="hydraRingModelFactorStorageVolumeStandardDeviation">The standard deviation of the model factor for the storage volume to use during the calculation.</param>
        /// <param name="hydraRingStorageStructureAreaMean">The mean of the storage structure area to use during the calculation.</param>
        /// <param name="hydraRingStorageStructureAreaVariation">The variation of the storage structure area to use during the calculation.</param>
        /// <param name="hydraRingModelFactorInflowVolume">The model factor inflow volume to use during the calculation.</param>
        /// <param name="hydraRingFlowWidthAtBottomProtectionMean">The mean of the flow width at bottom protection to use during the calculation.</param>
        /// <param name="hydraRingFlowWidthAtBottomProtectionStandardDeviation">The standard deviation of the flow width at bottom protection to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingDischargeMean">The mean of the critical overtopping discharge to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingDischargeVariation">The variation of the critical overtopping discharge to use during the calculation.</param>
        /// <param name="hydraRingFailureProbabilityStructureWithErosion">The failure probability structure with erosion to use during the calculation.</param>
        /// <param name="hydraRingWidthFlowAperturesMean">The mean of the width of flow apertures to use during the calculation.</param>
        /// <param name="hydraRingWidthFlowAperturesVariation">The variation of the width of flow apertures to use during the calculation.</param>
        /// <param name="hydraRingDeviationOfTheWaveDirection">The deviation of the wave direction to use during the calculation.</param>
        /// <param name="hydraRingStormDurationMean">The mean of the storm duration to use during the calculation.</param>
        /// <param name="hydraRingStormDurationVariation">The variation of the storm duration to use during the calculation.</param>
        public StructuresOvertoppingCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                                     double hydraRingGravitationalAcceleration,
                                                     double hydraRingModelFactorOvertoppingFlowMean, double hydraRingModelFactorOvertoppingFlowStandardDeviation,
                                                     double hydraRingLevelCrestStructureMean, double hydraRingLevelCrestStructureStandardDeviation,
                                                     double hydraRingStructureNormalOrientation,
                                                     double hydraRingModelFactorSuperCriticalFlowMean, double hydraRingModelFactorSuperCriticalFlowStandardDeviation,
                                                     double hydraRingAllowedLevelIncreaseStorageMean, double hydraRingAllowedLevelIncreaseStorageStandardDeviation,
                                                     double hydraRingModelFactorStorageVolumeMean, double hydraRingModelFactorStorageVolumeStandardDeviation,
                                                     double hydraRingStorageStructureAreaMean, double hydraRingStorageStructureAreaVariation,
                                                     double hydraRingModelFactorInflowVolume,
                                                     double hydraRingFlowWidthAtBottomProtectionMean, double hydraRingFlowWidthAtBottomProtectionStandardDeviation,
                                                     double hydraRingCriticalOvertoppingDischargeMean, double hydraRingCriticalOvertoppingDischargeVariation,
                                                     double hydraRingFailureProbabilityStructureWithErosion,
                                                     double hydraRingWidthFlowAperturesMean, double hydraRingWidthFlowAperturesVariation,
                                                     double hydraRingDeviationOfTheWaveDirection,
                                                     double hydraRingStormDurationMean, double hydraRingStormDurationVariation
            )
            : base(hydraulicBoundaryLocationId)
        {
            section = hydraRingSection;
            gravitationalAcceleration = hydraRingGravitationalAcceleration;
            modelFactorOvertoppingFlowMean = hydraRingModelFactorOvertoppingFlowMean;
            modelFactorOvertoppingFlowStandardDeviation = hydraRingModelFactorOvertoppingFlowStandardDeviation;
            levelCrestStructureMean = hydraRingLevelCrestStructureMean;
            levelCrestStructureStandardDeviation = hydraRingLevelCrestStructureStandardDeviation;
            structureNormalOrientation = hydraRingStructureNormalOrientation;
            modelFactorSuperCriticalFlowMean = hydraRingModelFactorSuperCriticalFlowMean;
            modelFactorSuperCriticalFlowStandardDeviation = hydraRingModelFactorSuperCriticalFlowStandardDeviation;
            allowedLevelIncreaseStorageMean = hydraRingAllowedLevelIncreaseStorageMean;
            allowedLevelIncreaseStorageStandardDeviation = hydraRingAllowedLevelIncreaseStorageStandardDeviation;
            modelFactorStorageVolumeMean = hydraRingModelFactorStorageVolumeMean;
            modelFactorStorageVolumeStandardDeviation = hydraRingModelFactorStorageVolumeStandardDeviation;
            storageStructureAreaMean = hydraRingStorageStructureAreaMean;
            storageStructureAreaVariation = hydraRingStorageStructureAreaVariation;
            modelFactorInflowVolume = hydraRingModelFactorInflowVolume;
            flowWidthAtBottomProtectionMean = hydraRingFlowWidthAtBottomProtectionMean;
            flowWidthAtBottomProtectionStandardDeviation = hydraRingFlowWidthAtBottomProtectionStandardDeviation;
            criticalOvertoppingDischargeMean = hydraRingCriticalOvertoppingDischargeMean;
            criticalOvertoppingDischargeVariation = hydraRingCriticalOvertoppingDischargeVariation;
            failureProbabilityStructureWithErosion = hydraRingFailureProbabilityStructureWithErosion;
            widthFlowAperturesMean = hydraRingWidthFlowAperturesMean;
            widthFlowAperturesVariation = hydraRingWidthFlowAperturesVariation;
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
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Model factor overtopping flow
            yield return new HydraRingVariable(59, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorOvertoppingFlowMean,
                                               modelFactorOvertoppingFlowStandardDeviation, double.NaN);

            // Level crest structure
            yield return new HydraRingVariable(60, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, levelCrestStructureMean,
                                               levelCrestStructureStandardDeviation, double.NaN);

            // Orientation of the normal of the structure
            yield return new HydraRingVariable(61, HydraRingDistributionType.Deterministic, structureNormalOrientation,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Model factor super critical flow
            yield return new HydraRingVariable(62, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorSuperCriticalFlowMean,
                                               modelFactorSuperCriticalFlowStandardDeviation, double.NaN);

            // Allowed level increase storage
            yield return new HydraRingVariable(94, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, allowedLevelIncreaseStorageMean,
                                               allowedLevelIncreaseStorageStandardDeviation, double.NaN);

            // Model factor storage volume
            yield return new HydraRingVariable(95, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorStorageVolumeMean,
                                               modelFactorStorageVolumeStandardDeviation, double.NaN);

            // Storage structure area
            yield return new HydraRingVariable(96, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, storageStructureAreaMean,
                                               storageStructureAreaVariation, double.NaN);

            // Model factor inflow volume
            yield return new HydraRingVariable(97, HydraRingDistributionType.Deterministic, modelFactorInflowVolume,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Flow width at bottom protection
            yield return new HydraRingVariable(103, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, flowWidthAtBottomProtectionMean,
                                               flowWidthAtBottomProtectionStandardDeviation, double.NaN);

            // Critical overtopping discharge
            yield return new HydraRingVariable(104, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, criticalOvertoppingDischargeMean,
                                               criticalOvertoppingDischargeVariation, double.NaN);

            // Failure probability structure with erosion
            yield return new HydraRingVariable(105, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, failureProbabilityStructureWithErosion,
                                               0.0, double.NaN); // HACK: Pass the deterministic value as normal distribution (with standard deviation 0.0) as Hydra-Ring otherwise crashes

            // Width flow apertures
            yield return new HydraRingVariable(106, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Variation, widthFlowAperturesMean,
                                               widthFlowAperturesVariation, double.NaN);

            // Deviation of the wave direction
            yield return new HydraRingVariable(107, HydraRingDistributionType.Deterministic, deviationOfTheWaveDirection,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Storm duration
            yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Variation, stormDurationMean,
                                               stormDurationVariation, double.NaN);
        }
    }
}