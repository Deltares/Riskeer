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

namespace Ringtoets.HydraRing.Calculation.Data.Input.Overtopping
{
    /// <summary>
    /// Container of all data necessary for performing a structures overtopping calculation via Hydra-Ring.
    /// </summary>
    public class StructuresOvertoppingCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection section;
        private readonly double gravitationalAcceleration;
        private readonly double modelFactorOvertoppingMean;
        private readonly double modelFactorOvertoppingStandardDeviation;
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
        private readonly double storageStructureAreaStandardDeviation;
        private readonly double modelFactorForIncomingFlowVolume;
        private readonly double flowWidthAtBottomProtectionMean;
        private readonly double flowWidthAtBottomProtectionStandardDeviation;
        private readonly double criticalOvertoppingDischargeMean;
        private readonly double criticalOvertoppingDischargeStandardDeviation;
        private readonly double failureProbabilityOfStructureGivenErosion;
        private readonly double widthOfFlowAperturesMean;
        private readonly double widthOfFlowAperturesStandardDeviation;
        private readonly double deviationOfTheWaveDirection;
        private readonly double stormDurationMean;
        private readonly double stormDurationStandardDeviation;

        /// <summary>
        /// Creates a new instance of the <see cref="StructuresOvertoppingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="hydraRingSection">The section to use during the calculation.</param>
        /// <param name="hydraRingGravitationalAcceleration">The gravitational acceleration to use during the calculation.</param>
        /// <param name="hydraRingModelFactorOvertoppingMean">The mean of the model factor overtopping to use during the calculation.</param>
        /// <param name="hydraRingModelFactorOvertoppingStandardDeviation">The model factor overtopping to use during the calculation.</param>
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
        /// <param name="hydraRingStorageStructureAreaStandardDeviation">The standard deviation of the storage structure area to use during the calculation.</param>
        /// <param name="hydraRingModelFactorForIncomingFlowVolume">The model factor for incoming flow volume to use during the calculation.</param>
        /// <param name="hydraRingFlowWidthAtBottomProtectionMean">The mean of the flow width at bottom protection to use during the calculation.</param>
        /// <param name="hydraRingFlowWidthAtBottomProtectionStandardDeviation">The standard deviation of the flow width at bottom protection to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingDischargeMean">The mean of the critical overtopping discharge to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingDischargeStandardDeviation">The standard deviation of the  critical overtopping discharge to use during the calculation.</param>
        /// <param name="hydraRingFailureProbabilityOfStructureGivenErosion">The failure probability of structure given erosion to use during the calculation.</param>
        /// <param name="hydraRingWidthOfFlowAperturesMean">The mean of the width of flow apertures to use during the calculation.</param>
        /// <param name="hydraRingWidthOfFlowAperturesStandardDeviation">The standard deviation of the width of flow apertures to use during the calculation.</param>
        /// <param name="hydraRingDeviationOfTheWaveDirection">The deviation of the wave direction to use during the calculation.</param>
        /// <param name="hydraRingStormDurationMean">The mean of the storm duration to use during the calculation.</param>
        /// <param name="hydraRingStormDurationStandardDeviation">The standard deviation of the storm duration to use during the calculation.</param>
        public StructuresOvertoppingCalculationInput(int hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                                     double hydraRingGravitationalAcceleration,
                                                     double hydraRingModelFactorOvertoppingMean, double hydraRingModelFactorOvertoppingStandardDeviation,
                                                     double hydraRingLevelOfCrestOfStructureMean, double hydraRingLevelOfCrestOfStructureStandardDeviation,
                                                     double hydraRingStructureNormalOrientation,
                                                     double hydraRingModelFactorOvertoppingSupercriticalFlowMean, double hydraRingModelFactorOvertoppingSupercriticalFlowStandardDeviation,
                                                     double hydraRingAllowableIncreaseOfLevelForStorageMean, double hydraRingAllowableIncreaseOfLevelForStorageStandardDeviation,
                                                     double hydraRingModelFactorForStorageVolumeMean, double hydraRingModelFactorForStorageVolumeStandardDeviation,
                                                     double hydraRingStorageStructureAreaMean, double hydraRingStorageStructureAreaStandardDeviation,
                                                     double hydraRingModelFactorForIncomingFlowVolume,
                                                     double hydraRingFlowWidthAtBottomProtectionMean, double hydraRingFlowWidthAtBottomProtectionStandardDeviation,
                                                     double hydraRingCriticalOvertoppingDischargeMean, double hydraRingCriticalOvertoppingDischargeStandardDeviation,
                                                     double hydraRingFailureProbabilityOfStructureGivenErosion,
                                                     double hydraRingWidthOfFlowAperturesMean, double hydraRingWidthOfFlowAperturesStandardDeviation,
                                                     double hydraRingDeviationOfTheWaveDirection,
                                                     double hydraRingStormDurationMean, double hydraRingStormDurationStandardDeviation
            )
            : base(hydraulicBoundaryLocationId)
        {
            section = hydraRingSection;
            gravitationalAcceleration = hydraRingGravitationalAcceleration;
            modelFactorOvertoppingMean = hydraRingModelFactorOvertoppingMean;
            modelFactorOvertoppingStandardDeviation = hydraRingModelFactorOvertoppingStandardDeviation;
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
            storageStructureAreaStandardDeviation = hydraRingStorageStructureAreaStandardDeviation;
            modelFactorForIncomingFlowVolume = hydraRingModelFactorForIncomingFlowVolume;
            flowWidthAtBottomProtectionMean = hydraRingFlowWidthAtBottomProtectionMean;
            flowWidthAtBottomProtectionStandardDeviation = hydraRingFlowWidthAtBottomProtectionStandardDeviation;
            criticalOvertoppingDischargeMean = hydraRingCriticalOvertoppingDischargeMean;
            criticalOvertoppingDischargeStandardDeviation = hydraRingCriticalOvertoppingDischargeStandardDeviation;
            failureProbabilityOfStructureGivenErosion = hydraRingFailureProbabilityOfStructureGivenErosion;
            widthOfFlowAperturesMean = hydraRingWidthOfFlowAperturesMean;
            widthOfFlowAperturesStandardDeviation = hydraRingWidthOfFlowAperturesStandardDeviation;
            deviationOfTheWaveDirection = hydraRingDeviationOfTheWaveDirection;
            stormDurationMean = hydraRingStormDurationMean;
            stormDurationStandardDeviation = hydraRingStormDurationStandardDeviation;
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
                yield return new OvertoppingGravitationalAcceleration(gravitationalAcceleration);
                yield return new OvertoppingModelfactorOvertopping(modelFactorOvertoppingMean, modelFactorOvertoppingStandardDeviation);
                yield return new OvertoppingLevelOfCrestOfStructure(levelOfCrestOfStructureMean, levelOfCrestOfStructureStandardDeviation);
                yield return new OvertoppingOrientationOfTheNormalOfTheStructure(structureNormalOrientation);
                yield return new OvertoppingModelfactorOvertoppingSupercriticalFlow(modelFactorOvertoppingSupercriticalFlowMean, modelFactorOvertoppingSupercriticalFlowStandardDeviation);
                yield return new OvertoppingAllowableIncreaseOfLevelForStorage(allowableIncreaseOfLevelForStorageMean, allowableIncreaseOfLevelForStorageStandardDeviation);
                yield return new OvertoppingModelFactorForStorageVolume(modelFactorForStorageVolumeMean, modelFactorForStorageVolumeStandardDeviation);
                yield return new OvertoppingStorageStructureArea(storageStructureAreaMean, storageStructureAreaStandardDeviation);
                yield return new OvertoppingModelFactorForIncomingFlowVolume(modelFactorForIncomingFlowVolume);
                yield return new OvertoppingFlowWidthAtBottomProtection(flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation);
                yield return new OvertoppingCriticalOvertoppingDischarge(criticalOvertoppingDischargeMean, criticalOvertoppingDischargeStandardDeviation);
                yield return new OvertoppingFailureProbabilityOfStructureGivenErosion(failureProbabilityOfStructureGivenErosion);
                yield return new OvertoppingWidthOfFlowApertures(widthOfFlowAperturesMean, widthOfFlowAperturesStandardDeviation);
                yield return new OvertoppingDeviationOfTheWaveDirection(deviationOfTheWaveDirection);
                yield return new OvertoppingStormDuration(stormDurationMean, stormDurationStandardDeviation);
            }
        }

        #region Overtopping Variables

        private class OvertoppingGravitationalAcceleration : HydraRingVariable
        {
            public OvertoppingGravitationalAcceleration(double acceleration) :
                base(58, HydraRingDistributionType.Deterministic, acceleration, HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN) {}
        }

        private class OvertoppingModelfactorOvertopping : HydraRingVariable
        {
            public OvertoppingModelfactorOvertopping(double mean, double standardDeviation) :
                base(59, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingLevelOfCrestOfStructure : HydraRingVariable
        {
            public OvertoppingLevelOfCrestOfStructure(double mean, double standardDeviation) :
                base(60, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingOrientationOfTheNormalOfTheStructure : HydraRingVariable
        {
            public OvertoppingOrientationOfTheNormalOfTheStructure(double orientation) :
                base(61, HydraRingDistributionType.Deterministic, orientation, HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN) {}
        }

        private class OvertoppingModelfactorOvertoppingSupercriticalFlow : HydraRingVariable
        {
            public OvertoppingModelfactorOvertoppingSupercriticalFlow(double mean, double standardDeviation) :
                base(62, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingAllowableIncreaseOfLevelForStorage : HydraRingVariable
        {
            public OvertoppingAllowableIncreaseOfLevelForStorage(double mean, double standardDeviation) :
                base(94, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingModelFactorForStorageVolume : HydraRingVariable
        {
            public OvertoppingModelFactorForStorageVolume(double mean, double standardDeviation) :
                base(95, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingStorageStructureArea : HydraRingVariable
        {
            public OvertoppingStorageStructureArea(double mean, double standardDeviation) :
                base(96, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingModelFactorForIncomingFlowVolume : HydraRingVariable
        {
            public OvertoppingModelFactorForIncomingFlowVolume(double modelFactor) :
                base(97, HydraRingDistributionType.Deterministic, modelFactor, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN) {}
        }

        private class OvertoppingFlowWidthAtBottomProtection : HydraRingVariable
        {
            public OvertoppingFlowWidthAtBottomProtection(double mean, double standardDeviation) :
                base(103, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingCriticalOvertoppingDischarge : HydraRingVariable
        {
            public OvertoppingCriticalOvertoppingDischarge(double mean, double standardDeviation) :
                base(104, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingFailureProbabilityOfStructureGivenErosion : HydraRingVariable
        {
            public OvertoppingFailureProbabilityOfStructureGivenErosion(double failureProbability) :
                base(105, HydraRingDistributionType.Deterministic, failureProbability, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN) {}
        }

        private class OvertoppingWidthOfFlowApertures : HydraRingVariable
        {
            public OvertoppingWidthOfFlowApertures(double mean, double standardDeviation) :
                base(106, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, mean, standardDeviation, double.NaN) {}
        }

        private class OvertoppingDeviationOfTheWaveDirection : HydraRingVariable
        {
            public OvertoppingDeviationOfTheWaveDirection(double deviation) :
                base(107, HydraRingDistributionType.Deterministic, deviation, HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN) {}
        }

        private class OvertoppingStormDuration : HydraRingVariable
        {
            public OvertoppingStormDuration(double mean, double standardDeviation) :
                base(108, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, mean, standardDeviation, double.NaN) {}
        }

        #endregion
    }
}