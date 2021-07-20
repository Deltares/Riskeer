﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Helpers;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// This class holds the information for a calculation scenario.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationScenario : GrassCoverErosionInwardsCalculation, ICalculationScenario
    {
        private RoundedDouble contribution;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        /// <param name="norm">The norm to use in the calculation input.</param>
        public GrassCoverErosionInwardsCalculationScenario(double norm) 
            : base(norm)
        {
            IsRelevant = true;
            contribution = new RoundedDouble(CalculationScenarioHelper.ContributionNumberOfDecimalPlaces, 1);
        }

        public bool IsRelevant { get; set; }

        public RoundedDouble Contribution
        {
            get => contribution;
            set
            {
                RoundedDouble newValue = value.ToPrecision(CalculationScenarioHelper.ContributionNumberOfDecimalPlaces);

                CalculationScenarioHelper.ValidateScenarioContribution(newValue);

                contribution = newValue;
            }
        }
    }
}