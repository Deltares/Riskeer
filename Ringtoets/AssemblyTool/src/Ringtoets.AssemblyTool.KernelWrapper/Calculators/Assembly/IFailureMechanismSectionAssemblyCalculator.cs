// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Interface representing a failure mechanism section assembly calculator.
    /// </summary>
    public interface IFailureMechanismSectionAssemblyCalculator
    {
        /// <summary>
        /// Assembles the simple assessment for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultType"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultType input);

        /// <summary>
        /// Assembles the simple assessment for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultValidityOnlyType"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType input);

        /// <summary>
        /// Assembles the detailed assessment based on the input parameters.
        /// </summary>
        /// <param name="probability">The calculated probability.</param>
        /// <param name="categories">The collection of categories for this failure mechanism section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categories"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categories"/>
        /// contains items that are <c>null</c>.</exception>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleDetailedAssessment(double probability, IEnumerable<FailureMechanismSectionAssemblyCategory> categories);

        /// <summary>
        /// Assembles the detailed assessment based on the input parameters.
        /// </summary>
        /// <param name="probability">The calculated probability.</param>
        /// <param name="categories">The collection of categories for this failure mechanism section.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categories"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categories"/>
        /// contains items that are <c>null</c>.</exception>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleDetailedAssessment(double probability,
                                                                   IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
                                                                   double n);
    }
}