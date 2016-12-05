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
    /// Container of all data necessary for performing an overtopping calculation via Hydra-Ring.
    /// </summary>
    public class OvertoppingCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection section;
        private readonly IEnumerable<HydraRingProfilePoint> profilePoints;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;
        private readonly HydraRingBreakWater breakWater;
        private readonly double dikeHeight;
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
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="section">The section.</param>
        /// <param name="profilePoints">The profile points.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="dikeHeight">The dike height.</param>
        /// <param name="modelFactorCriticalOvertopping">The model factor critical overtopping.</param>
        /// <param name="factorFbMean">The mean of the factor Fb</param>
        /// <param name="factorFbStandardDeviation">The standard deviation of the factor Fb.</param>
        /// <param name="factorFnMean">The mean of the factor Fn.</param>
        /// <param name="factorFnStandardDeviation">The standard deviation of the factor Fn.</param>
        /// <param name="modelFactorOvertopping">The factor overtopping.</param>
        /// <param name="criticalOvertoppingMean">The mean of the critical overtopping.</param>
        /// <param name="criticalOvertoppingStandardDeviation">The standard deviation of the critical overtopping.</param>
        /// <param name="modelFactorFrunupMean">The mean of the factor frunup.</param>
        /// <param name="modelFactorFrunupStandardDeviation">The standard deviation of the factor frunup.</param>
        /// <param name="exponentModelFactorShallowMean">The mean of the exponent model factor shallow.</param>
        /// <param name="exponentModelFactorShallowStandardDeviation">The standard deviation of the exponent model factor shallow.</param>
        public OvertoppingCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection section,
                                           IEnumerable<HydraRingRoughnessProfilePoint> profilePoints,
                                           IEnumerable<HydraRingForelandPoint> forelandPoints,
                                           HydraRingBreakWater breakWater,
                                           double dikeHeight,
                                           double modelFactorCriticalOvertopping,
                                           double factorFbMean, double factorFbStandardDeviation,
                                           double factorFnMean, double factorFnStandardDeviation,
                                           double modelFactorOvertopping,
                                           double criticalOvertoppingMean, double criticalOvertoppingStandardDeviation,
                                           double modelFactorFrunupMean, double modelFactorFrunupStandardDeviation,
                                           double exponentModelFactorShallowMean, double exponentModelFactorShallowStandardDeviation)
            : base(hydraulicBoundaryLocationId)
        {
            this.section = section;
            this.profilePoints = profilePoints;
            this.forelandPoints = forelandPoints;
            this.breakWater = breakWater;
            this.dikeHeight = dikeHeight;
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
            this.criticalOvertoppingMean = criticalOvertoppingMean;
            this.criticalOvertoppingStandardDeviation = criticalOvertoppingStandardDeviation;
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.DikesOvertopping;
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

        public override IEnumerable<HydraRingVariable2> NewVariables
        {
            get
            {
                yield return new DeterministicHydraRingVariable(1, dikeHeight);
                yield return new DeterministicHydraRingVariable(8, modelFactorCriticalOvertopping);
                yield return new NormalHydraRingVariable(10, HydraRingDeviationType.Standard, factorFbMean, factorFbStandardDeviation);
                yield return new NormalHydraRingVariable(11, HydraRingDeviationType.Standard, factorFnMean, factorFnStandardDeviation);
                yield return new DeterministicHydraRingVariable(12, modelFactorOvertopping);
                yield return new LogNormalHydraRingVariable(17, HydraRingDeviationType.Standard, criticalOvertoppingMean, criticalOvertoppingStandardDeviation);
                yield return new NormalHydraRingVariable(120, HydraRingDeviationType.Standard, modelFactorFrunupMean, modelFactorFrunupStandardDeviation);
                yield return new NormalHydraRingVariable(123, HydraRingDeviationType.Standard, exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation);
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