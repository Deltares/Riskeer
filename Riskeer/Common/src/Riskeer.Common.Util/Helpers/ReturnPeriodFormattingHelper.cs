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
using Riskeer.Common.Util.Properties;

namespace Riskeer.Common.Util.Helpers
{
    /// <summary>
    /// This class helps in consistently writing the return period.
    /// </summary>
    public static class ReturnPeriodFormattingHelper
    {
        /// <summary>
        /// Formats the specified probability to a return period.
        /// </summary>
        /// <param name="probability">The probability.</param>
        /// <returns>The formatted text.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// <paramref name="probability"/> is smaller or equal to 0.0.</exception>
        /// <exception cref="FormatException">Thrown when the probability
        /// cannot be formatted as a string.</exception>
        public static string FormatFromProbability(double probability)
        {
            if (probability <= 0.0)
            {
                throw new ArgumentOutOfRangeException(null, @"Probability must be larger than 0.0.");
            }

            return string.Format(Resources.ReturnPeriodFormat, 1.0 / probability);
        }
    }
}