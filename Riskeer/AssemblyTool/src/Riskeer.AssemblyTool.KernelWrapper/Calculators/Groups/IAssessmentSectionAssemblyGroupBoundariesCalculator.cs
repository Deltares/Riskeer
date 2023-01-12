// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Groups
{
    /// <summary>
    /// Interface representing an assessment section assembly group boundaries calculator.
    /// </summary>
    public interface IAssessmentSectionAssemblyGroupBoundariesCalculator
    {
        /// <summary>
        /// Performs the calculation for getting the assessment section assembly group boundaries.
        /// </summary>
        /// <param name="signalFloodingProbability">The signal flooding probability to calculate with.</param>
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability to calculate with.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="AssessmentSectionAssemblyGroupBoundaries"/>.</returns>
        /// <exception cref="AssessmentSectionAssemblyGroupBoundariesCalculatorException">Thrown when an error occurs
        /// while performing the calculation.</exception>
        IEnumerable<AssessmentSectionAssemblyGroupBoundaries> CalculateAssessmentSectionAssemblyGroupBoundaries(
            double signalFloodingProbability, double maximumAllowableFloodingProbability);
    }
}