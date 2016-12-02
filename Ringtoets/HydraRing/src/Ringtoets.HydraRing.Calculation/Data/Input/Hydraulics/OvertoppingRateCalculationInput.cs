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
using System.Linq;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;

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
        /// <param name="modelFactorFrunupMean">The mean of the factor frunup.</param>
        /// <param name="modelFactorFrunupStandardDeviation">The standard deviation of the factor frunup.</param>
        /// <param name="exponentModelFactorShallowMean">The mean of the exponent model factor shallow.</param>
        /// <param name="exponentModelFactorShallowStandardDeviation">The standard deviation of the exponent model factor shallow.</param>
        public OvertoppingRateCalculationInput(long hydraulicBoundaryLocationId, double norm,
                                               HydraRingSection section,
                                               IEnumerable<HydraRingRoughnessProfilePoint> profilePoints,
                                               IEnumerable<HydraRingForelandPoint> forelandPoints,
                                               HydraRingBreakWater breakWater,
                                               double dikeHeight,
                                               double modelFactorCriticalOvertopping,
                                               double factorFbMean, double factorFbStandardDeviation,
                                               double factorFnMean, double factorFnStandardDeviation,
                                               double modelFactorOvertopping,
                                               double modelFactorFrunupMean, double modelFactorFrunupStandardDeviation,
                                               double exponentModelFactorShallowMean, double exponentModelFactorShallowStandardDeviation)
            : base(hydraulicBoundaryLocationId, norm,
                   section,
                   profilePoints,
                   forelandPoints,
                   breakWater,
                   modelFactorCriticalOvertopping,
                   factorFbMean, factorFbStandardDeviation,
                   factorFnMean, factorFnStandardDeviation,
                   modelFactorOvertopping,
                   modelFactorFrunupMean, modelFactorFrunupStandardDeviation,
                   exponentModelFactorShallowMean, exponentModelFactorShallowStandardDeviation)
        {
            this.dikeHeight = dikeHeight;
        }

        public override int VariableId
        {
            get
            {
                return 17;
            }
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                var variables = base.Variables.ToList();
                variables.AddRange(GetVariables());

                return variables.OrderBy(v => v.VariableId);
            }
        }

        private IEnumerable<HydraRingVariable> GetVariables()
        {
            yield return new HydraRingVariable(1, HydraRingDistributionType.Deterministic, dikeHeight,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(17, HydraRingDistributionType.Deterministic, 0.0,
                                               HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
        }
    }
}