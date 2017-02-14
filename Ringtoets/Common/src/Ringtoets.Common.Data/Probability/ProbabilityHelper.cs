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

using System;
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probability
{
    /// <summary>
    /// Class that holds helper methods for probabilities.
    /// </summary>
    public static class ProbabilityHelper
    {
        private static readonly Range<double> probabilityValidityRange = new Range<double>(0, 1);

        /// <summary>
        /// Checks whether the given <paramref name="probability"/> is valid.
        /// </summary>
        /// <param name="probability">The probability to check.</param>
        /// <param name="nanIsValid">Optional: <c>false</c> is <see cref="double.NaN"/> should
        /// be considered a valid value. Default is <c>true</c>.</param>
        /// <returns><c>true</c> when <paramref name="probability"/> is valid; <c>false</c> otherwise.</returns>
        public static bool IsValidProbability(double probability, bool nanIsValid = false)
        {
            if (nanIsValid && double.IsNaN(probability))
            {
                return true;
            }
            return probabilityValidityRange.InRange(probability);
        }

        /// <summary>
        /// Checks if a argument is a valid probability value.
        /// </summary>
        /// <param name="probability">The value to be validated.</param>
        /// <param name="paramName">The name of the argument.</param>
        /// <param name="nanIsValid">Optional: <c>false</c> is <see cref="double.NaN"/> should
        /// be considered a valid value. Default is <c>true</c>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="probability"/>
        /// is not a valid probability value.</exception>
        public static void ValidateProbability(double probability, string paramName, bool nanIsValid = false)
        {
            if (!IsValidProbability(probability, nanIsValid))
            {
                string message = string.Format(Resources.Probability_Must_be_in_Range_0_,
                                               probabilityValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                throw new ArgumentOutOfRangeException(paramName, message);
            }
        }
    }
}