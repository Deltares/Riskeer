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

using Assembly.Kernel.Interfaces;

namespace Ringtoets.AssemblyTool.KernelWrapper.Kernels
{
    /// <summary>
    /// Factory responsible for creating kernels that can be used for performing an assembly tool calculation.
    /// </summary>
    public interface IAssemblyToolKernelFactory
    {
        /// <summary>
        /// Creates an assembly categories kernel.
        /// </summary>
        /// <returns>A new <see cref="ICategoryLimitsCalculator"/>.</returns>
        ICategoryLimitsCalculator CreateAssemblyCategoriesKernel();

        /// <summary>
        /// Creates a failure mechanism section assembly kernel.
        /// </summary>
        /// <returns>A new <see cref="IAssessmentResultsTranslator"/>.</returns>
        IAssessmentResultsTranslator CreateFailureMechanismSectionAssemblyKernel();

        /// <summary>
        /// Creates a failure mechanism assembly kernel.
        /// </summary>
        /// <returns>A new <see cref="IFailureMechanismResultAssembler"/>.</returns>
        IFailureMechanismResultAssembler CreateFailureMechanismAssemblyKernel();

        /// <summary>
        /// Creates an assessment section assembly kernel.
        /// </summary>
        /// <returns>A new <see cref="IAssessmentGradeAssembler"/>.</returns>
        IAssessmentGradeAssembler CreateAssessmentSectionAssemblyKernel();

        /// <summary>
        /// Creates a combined failure mechanism section assembly kernel.
        /// </summary>
        /// <returns>A new <see cref="ICommonFailureMechanismSectionAssembler"/>.</returns>
        ICommonFailureMechanismSectionAssembler CreateCombinedFailureMechanismSectionAssemblyKernel();
    }
}