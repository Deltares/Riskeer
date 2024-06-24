// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Forms.Exceptions;
using Riskeer.Common.Forms.Properties;
using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class to parse probabilities.
    /// </summary>
    public static class ProbabilityParsingHelper
    {
        private const string returnPeriodNotation = "1/";

        /// <summary>
        /// Parses a string value to a probability.
        /// </summary>
        /// <param name="value">The value to be parsed.</param>
        /// <returns>A <see cref="double"/> representing a probability.</returns>
        /// <exception cref="ProbabilityParsingException">Thrown when <paramref name="value"/> could not be successfully parsed as a probability.</exception>
        public static double Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim() == Resources.RoundedDouble_No_result_dash)
            {
                return double.NaN;
            }

            try
            {
                string trimmedString = value.Trim();
                if (!trimmedString.StartsWith(returnPeriodNotation))
                {
                    return Convert.ToDouble(value);
                }

                string returnPeriodValue = trimmedString.Substring(2).ToLower();
                return returnPeriodValue != CommonBaseResources.RoundedDouble_ToString_PositiveInfinity.ToLower()
                           ? 1 / Convert.ToDouble(returnPeriodValue)
                           : 0.0;
            }
            catch (FormatException exception)
            {
                throw new ProbabilityParsingException(Resources.Probability_Could_not_parse_string_to_probability,
                                                      exception);
            }
            catch (OverflowException exception)
            {
                throw new ProbabilityParsingException(Resources.Probability_Value_too_large_or_too_small,
                                                      exception);
            }
        }
    }
}