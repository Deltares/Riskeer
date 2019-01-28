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
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Data.Input.WaveConditions
{
    /// <summary>
    /// Container of all data necessary for performing a cosine based wave conditions calculation (Q-variant) via Hydra-Ring.
    /// </summary>
    public class WaveConditionsCosineCalculationInput : WaveConditionsCalculationInput
    {
        private readonly double c;

        /// <summary>
        /// Creates a new instance of the <see cref="WaveConditionsCosineCalculationInput"/> class.
        /// </summary>
        /// <param name="sectionId">The id of the section.</param>
        /// <param name="sectionNormal">The normal of the section.</param>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="norm">The norm.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="waterLevel">The water level to calculate the wave conditions for.</param>
        /// <param name="a">The a-value.</param>
        /// <param name="b">The b-value.</param>
        /// <param name="c">The c-value.</param>
        public WaveConditionsCosineCalculationInput(int sectionId,
                                                    double sectionNormal,
                                                    long hydraulicBoundaryLocationId,
                                                    double norm,
                                                    IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                    HydraRingBreakWater breakWater,
                                                    double waterLevel,
                                                    double a,
                                                    double b,
                                                    double c)
            : base(sectionId,
                   sectionNormal,
                   hydraulicBoundaryLocationId,
                   norm,
                   forelandPoints,
                   breakWater,
                   waterLevel,
                   a,
                   b)
        {
            this.c = c;
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                List<HydraRingVariable> variables = base.Variables.ToList();

                variables.Add(new DeterministicHydraRingVariable(119, c));

                return variables;
            }
        }

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 5:
                    return 71;
                default:
                    return null;
            }
        }
    }
}