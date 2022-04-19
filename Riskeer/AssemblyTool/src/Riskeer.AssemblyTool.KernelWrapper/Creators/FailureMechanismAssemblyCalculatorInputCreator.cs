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

using System;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FailureMechanismSections;
using FailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input that can be used in the failure mechanism assembly calculator.
    /// </summary>
    internal static class FailureMechanismAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates a <see cref="ResultWithProfileAndSectionProbabilities"/> based on <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The <see cref="FailureMechanismSectionAssemblyResult"/> to create the
        /// <see cref="ResultWithProfileAndSectionProbabilities"/> with.</param>
        /// <returns>A <see cref="ResultWithProfileAndSectionProbabilities"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        public static ResultWithProfileAndSectionProbabilities CreateResultWithProfileAndSectionProbabilities(FailureMechanismSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new ResultWithProfileAndSectionProbabilities(
                AssemblyCalculatorInputCreator.CreateProbability(result.ProfileProbability),
                AssemblyCalculatorInputCreator.CreateProbability(result.SectionProbability));
        }

        /// <summary>
        /// Creates a <see cref="Probability"/> based on <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The <see cref="FailureMechanismSectionAssemblyResult"/> to create the
        /// <see cref="Probability"/> with.</param>
        /// <returns>A <see cref="Probability"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        public static Probability CreateProbability(FailureMechanismSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return AssemblyCalculatorInputCreator.CreateProbability(result.SectionProbability);
        }
    }
}