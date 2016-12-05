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

namespace Ringtoets.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a structures closure calculation via Hydra-Ring.
    /// </summary>
    public abstract class StructuresClosureCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection section;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;
        private readonly HydraRingBreakWater breakWater;
        private readonly double gravitationalAcceleration;
        private readonly double factorStormDurationOpenStructure;
        private readonly double failureProbabilityOpenStructure;
        private readonly double failureProbabilityReparation;
        private readonly int identicalApertures;
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
        private readonly double stormDurationMean;
        private readonly double stormDurationVariation;
        private readonly double probabilityOpenStructureBeforeFlooding;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresClosureCalculationInput"/>.
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
        /// <param name="probabilityOpenStructureBeforeFlooding">The probability of an open structure before flooding.</param>
        protected StructuresClosureCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection section,
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
                                                    double probabilityOpenStructureBeforeFlooding)
            : base(hydraulicBoundaryLocationId)
        {
            this.section = section;
            this.forelandPoints = forelandPoints;
            this.breakWater = breakWater;
            this.gravitationalAcceleration = gravitationalAcceleration;
            this.factorStormDurationOpenStructure = factorStormDurationOpenStructure;
            this.failureProbabilityOpenStructure = failureProbabilityOpenStructure;
            this.failureProbabilityReparation = failureProbabilityReparation;
            this.identicalApertures = identicalApertures;
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
            this.stormDurationMean = stormDurationMean;
            this.stormDurationVariation = stormDurationVariation;
            this.probabilityOpenStructureBeforeFlooding = probabilityOpenStructureBeforeFlooding;
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
                return 58;
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
                yield return new DeterministicHydraRingVariable(58, gravitationalAcceleration);
                yield return new DeterministicHydraRingVariable(63, factorStormDurationOpenStructure);
                yield return new DeterministicHydraRingVariable(68, failureProbabilityOpenStructure);
                yield return new DeterministicHydraRingVariable(69, failureProbabilityReparation);
                yield return new DeterministicHydraRingVariable(71, identicalApertures);
                yield return new LogNormalHydraRingVariable(94, HydraRingDeviationType.Standard, allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation);
                yield return new LogNormalHydraRingVariable(95, HydraRingDeviationType.Standard, modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation);
                yield return new LogNormalHydraRingVariable(96, HydraRingDeviationType.Variation, storageStructureAreaMean, storageStructureAreaVariation);
                yield return new DeterministicHydraRingVariable(97, modelFactorInflowVolume);
                yield return new LogNormalHydraRingVariable(103, HydraRingDeviationType.Standard, flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation);
                yield return new LogNormalHydraRingVariable(104, HydraRingDeviationType.Variation, criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation);
                yield return new DeterministicHydraRingVariable(105, failureProbabilityStructureWithErosion);
                yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, stormDurationMean, stormDurationVariation);
                yield return new DeterministicHydraRingVariable(129, probabilityOpenStructureBeforeFlooding);
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

        public abstract override int? GetSubMechanismModelId(int subMechanismId);
    }
}