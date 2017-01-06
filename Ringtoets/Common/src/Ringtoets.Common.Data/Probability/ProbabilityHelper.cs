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

namespace Ringtoets.Common.Data.Probability
{
    /// <summary>
    /// Class that holds helper methods for probabilities.
    /// </summary>
    public static class ProbabilityHelper
    {
        /// <summary>
        /// Checks whether the given <paramref name="probability"/> is valid.
        /// </summary>
        /// <param name="probability">The probability to check.</param>
        /// <returns><c>true</c> when <see cref="double.NaN"/> or within the [0.0, 1.0] range.
        /// <c>false</c> otherwise.</returns>
        public static bool IsValidProbability(double probability)
        {
            return double.IsNaN(probability) || (0.0 <= probability && probability <= 1.0);
        }
    }
}