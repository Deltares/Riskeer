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

namespace Riskeer.Common.Data.FailurePath
{
    /// <summary>
    /// Class that contains helper methods for assembling a failure path.
    /// </summary>
    public static class FailurePathAssemblyHelper
    {
        /// <summary>
        /// Assembles the failure path based on the input arguments.
        /// </summary>
        /// <param name="failurePath">The <see cref="IFailurePath"/> to assemble.</param>
        /// <param name="performFailurePathAssemblyFunc">The function to perform the failure path assembly with.</param>
        /// <returns>A <see cref="double"/> representing the failure path probability.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static double AssembleFailurePath(IFailurePath failurePath,
                                                 Func<double> performFailurePathAssemblyFunc)
        {
            if (failurePath == null)
            {
                throw new ArgumentNullException(nameof(failurePath));
            }

            if (performFailurePathAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performFailurePathAssemblyFunc));
            }

            FailurePathAssemblyResult assemblyResult = failurePath.AssemblyResult;
            return assemblyResult.ProbabilityResultType == FailurePathAssemblyProbabilityResultType.Manual
                       ? assemblyResult.ManualFailurePathAssemblyProbability
                       : performFailurePathAssemblyFunc();
        }
    }
}