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
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics
{
    /// <summary>
    /// Container for all data necessary for performing a hydraulic loads calculation via Hydra-Ring.
    /// </summary>
    public abstract class HydraulicLoadsCalculationInput : ReliabilityIndexCalculationInput
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
        private readonly double modelFactorFrunupStandardDeviation;
        private readonly double modelFactorFrunupMean;
        private readonly double exponentModelFactorShallowStandardDeviation;
        private readonly double exponentModelFactorShallowMean;

        /// <summary>
        /// Creates a new instance of the <see cref="HydraulicLoadsCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="norm">The norm.</param>
        /// <param name="section">The section.</param>
        /// <param name="profilePoints">The profile points.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="modelFactorCriticalOvertopping">The model factor critical overtopping.</param>
        /// <param name="factorFbMean">The mean of the factor Fb</param>
        /// <param name="factorFbStandardDeviation">The standard deviation of the factor Fb.</param>
        /// <param name="factorFnMean">The mean of the factor Fn.</param>
        /// <param name="factorFnStandardDeviation">The standard deviation of the factor Fn.</param>
        /// <param name="modelFactorOvertopping">The factor overtopping.</param>
        /// <param name="modelFactorFrunupMean">The mean of the factor frunup.</param>
        /// <param name="modelFactorFrunupStandardDeviation">The standard deviation of the factor frunup.</param>
        /// <param name="exponentModelFactorShallowMean">The mean of the exponent model factor shallow.</param>
        /// <param name="exponentModelFactorShallowStandardDeviation">The standard deviation of the exponent model factor shallow.</param>
        protected HydraulicLoadsCalculationInput(long hydraulicBoundaryLocationId, double norm,
                                                 HydraRingSection section,
                                                 IEnumerable<HydraRingRoughnessProfilePoint> profilePoints,
                                                 IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                 HydraRingBreakWater breakWater,
                                                 double modelFactorCriticalOvertopping,
                                                 double factorFbMean, double factorFbStandardDeviation,
                                                 double factorFnMean, double factorFnStandardDeviation,
                                                 double modelFactorOvertopping,
                                                 double modelFactorFrunupMean, double modelFactorFrunupStandardDeviation,
                                                 double exponentModelFactorShallowMean, double exponentModelFactorShallowStandardDeviation)
            : base(hydraulicBoundaryLocationId, norm)
        {
            this.section = section;
            this.modelFactorCriticalOvertopping = modelFactorCriticalOvertopping;
            this.factorFbMean = factorFbMean;
            this.factorFbStandardDeviation = factorFbStandardDeviation;
            this.factorFnMean = factorFnMean;
            this.factorFnStandardDeviation = factorFnStandardDeviation;
            this.modelFactorOvertopping = modelFactorOvertopping;
            this.modelFactorFrunupMean = modelFactorFrunupMean;
            this.modelFactorFrunupStandardDeviation = modelFactorFrunupStandardDeviation;
            this.exponentModelFactorShallowMean = exponentModelFactorShallowMean;
            this.exponentModelFactorShallowStandardDeviation = exponentModelFactorShallowStandardDeviation;
            this.profilePoints = profilePoints;
            this.forelandPoints = forelandPoints;
            this.breakWater = breakWater;
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.HydraulicLoads;
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
                yield return new HydraRingVariable(8, HydraRingDistributionType.Deterministic, modelFactorCriticalOvertopping,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(10, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, factorFbMean, factorFbStandardDeviation,
                                                   double.NaN);
                yield return new HydraRingVariable(11, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, factorFnMean, factorFnStandardDeviation,
                                                   double.NaN);
                yield return new HydraRingVariable(12, HydraRingDistributionType.Deterministic, modelFactorOvertopping,
                                                   HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
                yield return new HydraRingVariable(120, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, modelFactorFrunupMean,
                                                   modelFactorFrunupStandardDeviation, double.NaN);
                yield return new HydraRingVariable(123, HydraRingDistributionType.Normal, double.NaN,
                                                   HydraRingDeviationType.Standard, exponentModelFactorShallowMean,
                                                   exponentModelFactorShallowStandardDeviation, double.NaN);
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