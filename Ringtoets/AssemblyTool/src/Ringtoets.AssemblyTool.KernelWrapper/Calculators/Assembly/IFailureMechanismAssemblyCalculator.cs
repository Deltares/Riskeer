// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Interface representing a failure mechanism assembly calculator.
    /// </summary>
    public interface IFailureMechanismAssemblyCalculator
    {
        /// <summary>
        /// Assembles the failure mechanism using the given <paramref name="sectionCategories"/>.
        /// </summary>
        /// <param name="sectionCategories">The collection of <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="FailureMechanismAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        FailureMechanismAssemblyCategoryGroup Assemble(IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> sectionCategories);

        /// <summary>
        /// Assembles the failure mechanism with failure probabilities using the given parameters.
        /// </summary>
        /// <param name="sectionAssemblies">The collection of <see cref="FailureMechanismSectionAssembly"/> to assemble for.</param>
        /// <param name="assemblyCategoriesInput">The object containing the input parameters for
        /// determining the assembly categories.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        FailureMechanismAssembly Assemble(IEnumerable<FailureMechanismSectionAssembly> sectionAssemblies,
                                          AssemblyCategoriesInput assemblyCategoriesInput);
    }
}