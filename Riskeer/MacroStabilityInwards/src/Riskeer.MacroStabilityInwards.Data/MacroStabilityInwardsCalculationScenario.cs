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
using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// This class holds the information for a calculation scenario.
    /// </summary>
    public class MacroStabilityInwardsCalculationScenario : MacroStabilityInwardsCalculation, ICalculationScenario
    {
        private RoundedDouble contribution;
        private const int contributionNumberOfDecimalPlaces = 4;
        private static readonly Range<RoundedDouble> contributionValidityRange = new Range<RoundedDouble>(new RoundedDouble(contributionNumberOfDecimalPlaces),
                                                                                                          new RoundedDouble(contributionNumberOfDecimalPlaces, 1.0));

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        public MacroStabilityInwardsCalculationScenario()
        {
            IsRelevant = true;
            contribution = new RoundedDouble(4, 1.0);
        }

        public bool IsRelevant { get; set; }

        public RoundedDouble Contribution
        {
            get => contribution;
            set
            {
                RoundedDouble newValue = value.ToPrecision(contributionNumberOfDecimalPlaces);

                if (!contributionValidityRange.InRange(newValue))
                {
                    throw new ArgumentOutOfRangeException(null, string.Format(RiskeerCommonDataResources.Contribution_must_be_within_Range_0_and_100));
                }

                contribution = newValue;
            }
        }
    }
}