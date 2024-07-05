// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

namespace Riskeer.Common.Forms.Providers
{
    /// <summary>
    /// Interface for providing error messages about the failure mechanism section result rows
    /// that contain calculated probabilities.
    /// </summary>
    public interface IFailureMechanismSectionResultRowWithCalculatedProbabilityErrorProvider : IFailureMechanismSectionResultRowErrorProvider
    {
        /// <summary>
        /// Gets the calculated probability validation error.
        /// </summary>
        /// <param name="getProbabilityFunc">The function to get the probability to validate.</param>
        /// <returns>An error message when the validation fails;
        /// or <see cref="string.Empty"/> when there are no errors.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="getProbabilityFunc"/> is <c>null</c>.</exception>
        string GetCalculatedProbabilityValidationError(Func<double> getProbabilityFunc);
    }
}