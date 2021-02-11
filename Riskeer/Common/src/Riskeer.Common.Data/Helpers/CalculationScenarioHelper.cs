// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.Helpers
{
    /// <summary>
    /// Helper class for dealing with <see cref="ICalculationScenario"/>.
    /// </summary>
    public static class CalculationScenarioHelper
    {
        private static readonly Range<RoundedDouble> contributionValidityRange = new Range<RoundedDouble>(
            new RoundedDouble(ContributionNumberOfDecimalPlaces),
            new RoundedDouble(ContributionNumberOfDecimalPlaces, 1.0));

        /// <summary>
        /// Gets the scenario contribution number of decimal places.
        /// </summary>
        public static int ContributionNumberOfDecimalPlaces => 4;

        /// <summary>
        /// Validates whether the <paramref name="value"/> is valid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not 
        /// between [0, 1].</exception>
        public static void ValidateScenarioContribution(RoundedDouble value)
        {
            if (!contributionValidityRange.InRange(value))
            {
                throw new ArgumentOutOfRangeException(null, Resources.Contribution_must_be_in_range);
            }
        }
    }
}