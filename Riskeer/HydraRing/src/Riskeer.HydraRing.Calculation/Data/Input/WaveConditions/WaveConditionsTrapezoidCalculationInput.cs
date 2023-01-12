﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Data.Input.WaveConditions
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
        /// <param name="sectionId">The id of the section.</param>
        /// <param name="sectionNormal">The normal of the section.</param>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="targetProbability">The target probability.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="waterLevel">The water level to calculate the wave conditions for.</param>
        /// <param name="a">The a-value.</param>
        /// <param name="b">The b-value.</param>
        /// <param name="beta1">The beta1-value.</param>
        /// <param name="beta2">The beta2-value.</param>
        public WaveConditionsTrapezoidCalculationInput(int sectionId,
                                                       double sectionNormal,
                                                       long hydraulicBoundaryLocationId,
                                                       double targetProbability,
                                                       IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                       HydraRingBreakWater breakWater,
                                                       double waterLevel,
                                                       double a,
                                                       double b,
                                                       double beta1,
                                                       double beta2)
            : base(sectionId,
                   sectionNormal,
                   hydraulicBoundaryLocationId,
                   targetProbability,
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
                List<HydraRingVariable> variables = base.Variables.ToList();

                variables.Add(new DeterministicHydraRingVariable(117, beta1));
                variables.Add(new DeterministicHydraRingVariable(118, beta2));

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