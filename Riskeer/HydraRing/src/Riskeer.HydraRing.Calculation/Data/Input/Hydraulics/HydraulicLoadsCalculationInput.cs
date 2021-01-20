﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Data.Input.Hydraulics
{
    /// <summary>
    /// Container for all data necessary for performing a hydraulic loads calculation via Hydra-Ring.
    /// </summary>
    public abstract class HydraulicLoadsCalculationInput : ReliabilityIndexCalculationInput
    {
        private readonly double modelFactorCriticalOvertopping;
        private readonly double factorFbMean;
        private readonly double factorFbStandardDeviation;
        private readonly double factorFbLowerBoundary;
        private readonly double factorFbUpperBoundary;
        private readonly double factorFnMean;
        private readonly double factorFnStandardDeviation;
        private readonly double factorFnLowerBoundary;
        private readonly double factorFnUpperBoundary;
        private readonly double modelFactorOvertopping;
        private readonly double modelFactorFrunupMean;
        private readonly double modelFactorFrunupStandardDeviation;
        private readonly double modelFactorFrunupLowerBoundary;
        private readonly double modelFactorFrunupUpperBoundary;
        private readonly double exponentModelFactorShallowMean;
        private readonly double exponentModelFactorShallowStandardDeviation;
        private readonly double exponentModelFactorShallowLowerBoundary;
        private readonly double exponentModelFactorShallowUpperBoundary;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraulicLoadsCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="norm">The norm.</param>
        /// <param name="sectionNormal">The normal of the section.</param>
        /// <param name="profilePoints">The profile points.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="modelFactorCriticalOvertopping">The model factor critical overtopping.</param>
        /// <param name="factorFbMean">The mean of the factor Fb</param>
        /// <param name="factorFbStandardDeviation">The standard deviation of the factor Fb.</param>
        /// <param name="factorFbLowerBoundary">The lower boundary of the factor Fb.</param>
        /// <param name="factorFbUpperBoundary">The upper boundary of the factor Fb.</param>
        /// <param name="factorFnMean">The mean of the factor Fn.</param>
        /// <param name="factorFnStandardDeviation">The standard deviation of the factor Fn.</param>
        /// <param name="factorFnLowerBoundary">The lower boundary of the factor Fn.</param>
        /// <param name="factorFnUpperBoundary">The upper boundary of the factor Fn.</param>
        /// <param name="modelFactorOvertopping">The factor overtopping.</param>
        /// <param name="modelFactorFrunupMean">The mean of the factor frunup.</param>
        /// <param name="modelFactorFrunupStandardDeviation">The standard deviation of the factor frunup.</param>
        /// <param name="modelFactorFrunupLowerBoundary">The lower boundary of the factor frunup.</param>
        /// <param name="modelFactorFrunupUpperBoundary">The upper boundary of the factor frunup.</param>
        /// <param name="exponentModelFactorShallowMean">The mean of the exponent model factor shallow.</param>
        /// <param name="exponentModelFactorShallowStandardDeviation">The standard deviation of the exponent model factor shallow.</param>
        /// <param name="exponentModelFactorShallowLowerBoundary">The lower boundary of the exponent model factor shallow.</param>
        /// <param name="exponentModelFactorShallowUpperBoundary">The upper boundary of the exponent model factor shallow.</param>
        protected HydraulicLoadsCalculationInput(long hydraulicBoundaryLocationId, double norm,
                                                 double sectionNormal,
                                                 IEnumerable<HydraRingRoughnessProfilePoint> profilePoints,
                                                 IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                 HydraRingBreakWater breakWater,
                                                 double modelFactorCriticalOvertopping,
                                                 double factorFbMean, double factorFbStandardDeviation,
                                                 double factorFbLowerBoundary, double factorFbUpperBoundary,
                                                 double factorFnMean, double factorFnStandardDeviation,
                                                 double factorFnLowerBoundary, double factorFnUpperBoundary,
                                                 double modelFactorOvertopping,
                                                 double modelFactorFrunupMean, double modelFactorFrunupStandardDeviation,
                                                 double modelFactorFrunupLowerBoundary, double modelFactorFrunupUpperBoundary,
                                                 double exponentModelFactorShallowMean, double exponentModelFactorShallowStandardDeviation,
                                                 double exponentModelFactorShallowLowerBoundary, double exponentModelFactorShallowUpperBoundary)
            : base(hydraulicBoundaryLocationId, norm)
        {
            Section = new HydraRingSection(1, double.NaN, sectionNormal);
            ProfilePoints = profilePoints;
            ForelandPoints = forelandPoints;
            BreakWater = breakWater;

            this.modelFactorCriticalOvertopping = modelFactorCriticalOvertopping;
            this.factorFbMean = factorFbMean;
            this.factorFbStandardDeviation = factorFbStandardDeviation;
            this.factorFbLowerBoundary = factorFbLowerBoundary;
            this.factorFbUpperBoundary = factorFbUpperBoundary;
            this.factorFnMean = factorFnMean;
            this.factorFnStandardDeviation = factorFnStandardDeviation;
            this.factorFnLowerBoundary = factorFnLowerBoundary;
            this.factorFnUpperBoundary = factorFnUpperBoundary;
            this.modelFactorOvertopping = modelFactorOvertopping;
            this.modelFactorFrunupMean = modelFactorFrunupMean;
            this.modelFactorFrunupStandardDeviation = modelFactorFrunupStandardDeviation;
            this.modelFactorFrunupLowerBoundary = modelFactorFrunupLowerBoundary;
            this.modelFactorFrunupUpperBoundary = modelFactorFrunupUpperBoundary;
            this.exponentModelFactorShallowMean = exponentModelFactorShallowMean;
            this.exponentModelFactorShallowStandardDeviation = exponentModelFactorShallowStandardDeviation;
            this.exponentModelFactorShallowLowerBoundary = exponentModelFactorShallowLowerBoundary;
            this.exponentModelFactorShallowUpperBoundary = exponentModelFactorShallowUpperBoundary;
        }

        public override HydraRingSection Section { get; }

        public override int FaultTreeModelId => 1017;

        public override IEnumerable<HydraRingProfilePoint> ProfilePoints { get; }

        public override IEnumerable<HydraRingForelandPoint> ForelandPoints { get; }

        public override HydraRingBreakWater BreakWater { get; }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield return new DeterministicHydraRingVariable(8, modelFactorCriticalOvertopping);
                yield return new TruncatedNormalHydraRingVariable(10, HydraRingDeviationType.Standard, factorFbMean, factorFbStandardDeviation,
                                                                  factorFbLowerBoundary, factorFbUpperBoundary);
                yield return new TruncatedNormalHydraRingVariable(11, HydraRingDeviationType.Standard, factorFnMean, factorFnStandardDeviation,
                                                                  factorFnLowerBoundary, factorFnUpperBoundary);
                yield return new DeterministicHydraRingVariable(12, modelFactorOvertopping);
                yield return new TruncatedNormalHydraRingVariable(120, HydraRingDeviationType.Standard, modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                                                                  modelFactorFrunupLowerBoundary, modelFactorFrunupUpperBoundary);
                yield return new TruncatedNormalHydraRingVariable(123, HydraRingDeviationType.Standard, exponentModelFactorShallowMean,
                                                                  exponentModelFactorShallowStandardDeviation, exponentModelFactorShallowLowerBoundary,
                                                                  exponentModelFactorShallowUpperBoundary);
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
    }
}