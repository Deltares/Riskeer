// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Variables;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics
{
    /// <summary>
    /// Container for all data necessary for performing an overtopping rate calculation via Hydra-Ring.
    /// </summary>
    public class OvertoppingRateCalculationInput : HydraulicLoadsCalculationInput
    {
        private readonly double dikeHeight;

        /// <summary>
        /// Creates a new instance of the <see cref="OvertoppingRateCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="norm">The norm.</param>
        /// <param name="sectionNormal">The normal of the section.</param>
        /// <param name="profilePoints">The profile points.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="dikeHeight">The dike height.</param>
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
        public OvertoppingRateCalculationInput(long hydraulicBoundaryLocationId,
                                               double norm,
                                               double sectionNormal,
                                               IEnumerable<HydraRingRoughnessProfilePoint> profilePoints,
                                               IEnumerable<HydraRingForelandPoint> forelandPoints,
                                               HydraRingBreakWater breakWater,
                                               double dikeHeight,
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
            : base(hydraulicBoundaryLocationId,
                   norm,
                   sectionNormal,
                   profilePoints,
                   forelandPoints,
                   breakWater,
                   modelFactorCriticalOvertopping,
                   factorFbMean, factorFbStandardDeviation,
                   factorFbLowerBoundary, factorFbUpperBoundary,
                   factorFnMean, factorFnStandardDeviation,
                   factorFnLowerBoundary, factorFnUpperBoundary,
                   modelFactorOvertopping,
                   modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                   modelFactorFrunupLowerBoundary, modelFactorFrunupUpperBoundary,
                   exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation,
                   exponentModelFactorShallowLowerBoundary, exponentModelFactorShallowUpperBoundary)
        {
            this.dikeHeight = dikeHeight;
        }

        public override HydraRingFailureMechanismType FailureMechanismType { get; } = HydraRingFailureMechanismType.OvertoppingRate;

        public override int VariableId { get; } = 17;

        public override int CalculationTypeId
        {
            get
            {
                return 2;
            }
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                List<HydraRingVariable> variables = base.Variables.ToList();
                variables.AddRange(GetVariables());

                return variables.OrderBy(v => v.VariableId);
            }
        }

        private IEnumerable<HydraRingVariable> GetVariables()
        {
            yield return new DeterministicHydraRingVariable(1, dikeHeight);
            yield return new DeterministicHydraRingVariable(17, 0.0);
        }
    }
}