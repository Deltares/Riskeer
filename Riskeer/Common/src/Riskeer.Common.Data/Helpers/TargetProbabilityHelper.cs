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
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.Helpers
{
    /// <summary>
    /// Helper class for getting valid target probabilities.
    /// </summary>
    public class TargetProbabilityHelper
    {
        /// <summary>
        /// Validates the provided target probability.
        /// </summary>
        /// <param name="targetProbability">The target probability to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the target probabilty value
        /// is not in the interval {0.0, 0.1] or is <see cref="double.NaN"/>.</exception>
        public static void ValidateTargetProbability(double targetProbability)
        {
            if (double.IsNaN(targetProbability) || targetProbability <= 0 || targetProbability > 0.1)
            {
                throw new ArgumentOutOfRangeException(null, Resources.TargetProbability_Value_must_be_in_range);
            }
        }
    }
}