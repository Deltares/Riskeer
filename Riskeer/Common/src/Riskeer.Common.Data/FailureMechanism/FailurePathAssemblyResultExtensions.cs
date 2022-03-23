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

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Extension methods for <see cref="FailurePathAssemblyResult"/>.
    /// </summary>
    public static class FailurePathAssemblyResultExtensions
    {
        /// <summary>
        /// Gets whether the <see cref="FailurePathAssemblyResult"/> is a manual probability.
        /// </summary>
        /// <param name="result">The <see cref="FailurePathAssemblyResult"/> to determine.</param>
        /// <returns>An indicator whether the <see cref="FailurePathAssemblyResult"/> is a manual probability.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        public static bool IsManualProbability(this FailurePathAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return result.ProbabilityResultType == FailurePathAssemblyProbabilityResultType.Manual;
        }
    }
}