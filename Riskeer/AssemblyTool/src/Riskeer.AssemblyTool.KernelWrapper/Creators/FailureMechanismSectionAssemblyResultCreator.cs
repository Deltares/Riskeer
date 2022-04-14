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
using System.ComponentModel;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailureMechanismSections;
using FailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="FailureMechanismSectionAssemblyResult"/> instances.
    /// </summary>
    internal static class FailureMechanismSectionAssemblyResultCreator
    {
        /// <summary>
        /// Converts a <see cref="Probability"/> combined with a <see cref="EInterpretationCategory"/>
        /// into a <see cref="FailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="sectionProbability">The <see cref="Probability"/> to convert.</param>
        /// <param name="category">The <see cref="EInterpretationCategory"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/> based on <paramref name="sectionProbability"/>
        /// and <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssemblyResult Create(Probability sectionProbability, EInterpretationCategory category)
        {
            return new FailureMechanismSectionAssemblyResult(
                sectionProbability, sectionProbability, 1.0,
                FailureMechanismSectionAssemblyGroupConverter.ConvertTo(category));
        }

        /// <summary>
        /// Converts a <see cref="ResultWithProfileAndSectionProbabilities"/> combined with a <see cref="EInterpretationCategory"/>
        /// into a <see cref="FailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="ResultWithProfileAndSectionProbabilities"/> to convert.</param>
        /// <param name="category">The <see cref="EInterpretationCategory"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/> based on <paramref name="result"/>
        /// and <paramref name="category"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssemblyResult Create(ResultWithProfileAndSectionProbabilities result,
                                                                   EInterpretationCategory category)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new FailureMechanismSectionAssemblyResult(
                result.ProbabilityProfile, result.ProbabilitySection, result.LengthEffectFactor,
                FailureMechanismSectionAssemblyGroupConverter.ConvertTo(category));
        }
    }
}