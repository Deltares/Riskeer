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
using System.Linq;

namespace Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions
{
    /// <summary>
    /// Container of all data necessary for performing a trapezoid based wave conditions calculation (Q-variant) via Hydra-Ring.
    /// </summary>
    public class WaveConditionsTrapezoidCalculationInput : WaveConditionsCalculationInput
    {
        private readonly double beta1;
        private readonly double beta2;

        /// <summary>
        /// Creates a new instance of the <see cref="WaveConditionsTrapezoidCalculationInput"/> class.
        /// </summary>
        /// <param name="sectionId">The id of the section to use during the calculation.</param>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="forelandPoints">The foreland points to use during the calculation.</param>
        /// <param name="breakWater">The break water to use during the calculation.</param>
        /// <param name="waterLevel">The water level to calculate the wave conditions for.</param>
        /// <param name="a">The a-value to use during the calculation.</param>
        /// <param name="b">The b-value to use during the calculation.</param>
        /// <param name="beta1">The beta1-value to use during the calculation.</param>
        /// <param name="beta2">The beta2-value to use during the calculation.</param>
        public WaveConditionsTrapezoidCalculationInput(int sectionId, long hydraulicBoundaryLocationId, double norm,
                                                       IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                       HydraRingBreakWater breakWater,
                                                       double waterLevel,
                                                       double a,
                                                       double b,
                                                       double beta1,
                                                       double beta2)
            : base(sectionId,
                   hydraulicBoundaryLocationId,
                   norm,
                   forelandPoints,
                   breakWater,
                   waterLevel,
                   a,
                   b)
        {
            this.beta1 = beta1;
            this.beta2 = beta2;
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                var variables = base.Variables.ToList();

                // beta1-value
                variables.Add(new HydraRingVariable(117, HydraRingDistributionType.Deterministic, beta1,
                                                    HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN));

                // beta2-value
                variables.Add(new HydraRingVariable(118, HydraRingDistributionType.Deterministic, beta2,
                                                    HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN));

                return variables;
            }
        }

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 5:
                    return 70;
                default:
                    return null;
            }
        }
    }
}