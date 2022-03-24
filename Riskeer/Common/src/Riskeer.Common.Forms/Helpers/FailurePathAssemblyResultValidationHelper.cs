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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Class containing validation helper methods for a <see cref="FailureMechanismAssemblyResult"/> 
    /// </summary>
    public static class FailurePathAssemblyResultValidationHelper
    {
        /// <summary>
        /// Gets the validation error for a <see cref="FailureMechanismAssemblyResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="FailureMechanismAssemblyResult"/> to get the validation messages for.</param>
        /// <returns>An error message when the validation fails or <see cref="string.Empty"/> when there are no errors.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        public static string GetValidationError(FailureMechanismAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return result.IsManualProbability() && double.IsNaN(result.ManualFailureMechanismAssemblyProbability)
                       ? Resources.FailureProbability_must_not_be_NaN
                       : string.Empty;
        }
    }
}