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
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics
{
    /// <summary>
    /// Container for all data necessary for performing a dike height calculation via Hydra-Ring.
    /// </summary>
    public class DikeHeightCalculationInput : ReliabilityIndexCalculationInput
    {
        private readonly HydraRingSection section;
        private readonly IEnumerable<HydraRingProfilePoint> profilePoints;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;
        private readonly HydraRingBreakWater breakWater;

        private readonly double modelFactorCriticalOvertopping;
        private readonly double factorFbMean;
        private readonly double factorFbStandardDeviation;
        private readonly double factorFnMean;
        private readonly double factorFnStandardDeviation;
        private readonly double modelFactorOvertopping;
        private readonly double criticalOvertoppingMean;
        private readonly double criticalOvertoppingStandardDeviation;
        private readonly double modelFactorFrunupStandardDeviation;
        private readonly double modelFactorFrunupMean;
        private readonly double exponentModelFactorShallowStandardDeviation;
        private readonly double exponentModelFactorShallowMean;

        /// <summary>
        /// Creates a new instance of the <see cref="OvertoppingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="hydraRingSection">The section to use during the calculation.</param>
        /// <param name="hydraRingModelFactorCriticalOvertopping">The model factor critical overtopping to use during the calculation.</param>
        /// <param name="hydraRingFactorFbMean">The mean of the factor Fb to use during the calculation</param>
        /// <param name="hydraRingFactorFbStandardDeviation">The standard deviation of the factor Fb to use during the calculation.</param>
        /// <param name="hydraRingFactorFnMean">The mean of the factor Fn to use during the calculation.</param>
        /// <param name="hydraRingFactorFnStandardDeviation">The standard deviation of the factor Fn to use during the calculation.</param>
        /// <param name="hydraRingmodelFactorOvertopping">The factor overtopping to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingMean">The mean of the critical overtopping to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingStandardDeviation">The standard deviation of the critical overtopping to use during the calculation.</param>
        /// <param name="hydraRingModelFactorFrunupMean">The mean of the factor frunup to use during the calculation.</param>
        /// <param name="hydraRingModelFactorFrunupStandardDeviation">The standard deviation of the factor frunup to use during the calculation.</param>
        /// <param name="hydraRingExponentModelFactorShallowMean">The mean of the exponent model factor shallow to use during the calculation.</param>
        /// <param name="hydraRingExponentModelFactorShallowStandardDeviation">The standard deviation of the exponent model factor shallow to use during the calculation.</param>
        /// <param name="hydraRingProfilePoints">The profile points to use during the calculation.</param>
        /// <param name="hydraRingForelandPoints">The foreland points to use during the calculation.</param>
        /// <param name="hydraRingBreakWater">The break water to use during the calculation.</param>
        public DikeHeightCalculationInput(long hydraulicBoundaryLocationId, double norm,
                                          HydraRingSection hydraRingSection, double hydraRingModelFactorCriticalOvertopping,
                                          double hydraRingFactorFbMean, double hydraRingFactorFbStandardDeviation,
                                          double hydraRingFactorFnMean, double hydraRingFactorFnStandardDeviation,
                                          double hydraRingmodelFactorOvertopping, double hydraRingCriticalOvertoppingMean,
                                          double hydraRingCriticalOvertoppingStandardDeviation, double hydraRingModelFactorFrunupMean,
                                          double hydraRingModelFactorFrunupStandardDeviation, double hydraRingExponentModelFactorShallowMean,
                                          double hydraRingExponentModelFactorShallowStandardDeviation,
                                          IEnumerable<HydraRingRoughnessProfilePoint> hydraRingProfilePoints,
                                          IEnumerable<HydraRingForelandPoint> hydraRingForelandPoints,
                                          HydraRingBreakWater hydraRingBreakWater)
            : base(hydraulicBoundaryLocationId, norm)
        {
            section = hydraRingSection;
            modelFactorCriticalOvertopping = hydraRingModelFactorCriticalOvertopping;
            factorFbMean = hydraRingFactorFbMean;
            factorFbStandardDeviation = hydraRingFactorFbStandardDeviation;
            factorFnMean = hydraRingFactorFnMean;
            factorFnStandardDeviation = hydraRingFactorFnStandardDeviation;
            modelFactorOvertopping = hydraRingmodelFactorOvertopping;
            modelFactorFrunupMean = hydraRingModelFactorFrunupMean;
            modelFactorFrunupStandardDeviation = hydraRingModelFactorFrunupStandardDeviation;
            exponentModelFactorShallowMean = hydraRingExponentModelFactorShallowMean;
            exponentModelFactorShallowStandardDeviation = hydraRingExponentModelFactorShallowStandardDeviation;

            criticalOvertoppingMean = hydraRingCriticalOvertoppingMean;
            criticalOvertoppingStandardDeviation = hydraRingCriticalOvertoppingStandardDeviation;
            profilePoints = hydraRingProfilePoints;
            forelandPoints = hydraRingForelandPoints;
            breakWater = hydraRingBreakWater;
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.DikesHeight;
            }
        }

        public override int VariableId
        {
            get
            {
                return 1;
            }
        }

        public override HydraRingSection Section
        {
            get
            {
                return section;
            }
        }

        public override IEnumerable<HydraRingProfilePoint> ProfilePoints
        {
            get
            {
                return profilePoints;
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

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                return GetHydraRingVariables();
            }
        }

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 102:
                    return 94;
                case 103:
                    return 95;
                default:
                    return null;
            }
        }

        private IEnumerable<HydraRingVariable> GetHydraRingVariables()
        {
            // Dike height
            yield return new HydraRingVariable(1, HydraRingDistributionType.Deterministic, 0.0,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Model factor critical overtopping
            yield return new HydraRingVariable(8, HydraRingDistributionType.Deterministic, modelFactorCriticalOvertopping,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Factor Fb
            yield return new HydraRingVariable(10, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, factorFbMean, factorFbStandardDeviation,
                                               double.NaN);

            // Factor Fn
            yield return new HydraRingVariable(11, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, factorFnMean, factorFnStandardDeviation,
                                               double.NaN);

            // Model factor overtopping
            yield return new HydraRingVariable(12, HydraRingDistributionType.Deterministic, modelFactorOvertopping,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);

            // Critical overtopping
            yield return new HydraRingVariable(17, HydraRingDistributionType.LogNormal, double.NaN,
                                               HydraRingDeviationType.Standard, criticalOvertoppingMean,
                                               criticalOvertoppingStandardDeviation, double.NaN);

            // Model factor Frunup
            yield return new HydraRingVariable(120, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, modelFactorFrunupMean,
                                               modelFactorFrunupStandardDeviation, double.NaN);

            // Exponent model factor shallow
            yield return new HydraRingVariable(123, HydraRingDistributionType.Normal, double.NaN,
                                               HydraRingDeviationType.Standard, exponentModelFactorShallowMean,
                                               exponentModelFactorShallowStandardDeviation, double.NaN);
        }
    }
}