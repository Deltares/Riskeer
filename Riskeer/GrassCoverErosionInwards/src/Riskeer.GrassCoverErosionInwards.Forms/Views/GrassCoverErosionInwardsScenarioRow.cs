// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Container of a <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>,
    /// which takes care of the representation of properties in a grid.
    /// </summary>
    public class GrassCoverErosionInwardsScenarioRow : ScenarioRow<GrassCoverErosionInwardsCalculationScenario>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsScenarioRow"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="GrassCoverErosionInwardsCalculationScenario"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationScenario"/> is <c>null</c>.</exception>
        internal GrassCoverErosionInwardsScenarioRow(GrassCoverErosionInwardsCalculationScenario calculationScenario)
            : base(calculationScenario) {}

        public override double FailureProbability
        {
            get
            {
                if (CalculationScenario.HasOutput)
                {
                    return CalculationScenario.Output.OvertoppingOutput.Reliability;
                }

                return double.NaN;
            }
        }

        public override void Update()
        {
            
        }
    }
}