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

using System.Collections.Generic;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories
{
    /// <summary>
    /// Interface representing an assembly categories calculator.
    /// </summary>
    public interface IAssemblyCategoriesCalculator
    {
        /// <summary>
        /// Performs the calculation for getting the assessment section categories.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to calculate with.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to calculate with.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="AssessmentSectionAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyCategoriesCalculatorException">Thrown when an error occurs
        /// while performing the calculation.</exception>
        IEnumerable<AssessmentSectionAssemblyCategory> CalculateAssessmentSectionCategories(
            double signalingNorm, double lowerLimitNorm);

        /// <summary>
        /// Performs the calculation for getting the failure mechanism categories.
        /// </summary>
        /// <param name="assemblyCategoriesInput">The object containing the input parameters for
        /// determining the assembly categories.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyCategoriesCalculatorException">Thrown when an error occurs
        /// while performing the calculation.</exception>
        IEnumerable<FailureMechanismAssemblyCategory> CalculateFailureMechanismCategories(AssemblyCategoriesInput assemblyCategoriesInput);

        /// <summary>
        /// Performs the calculation for getting the failure mechanism section categories.
        /// </summary>
        /// <param name="assemblyCategoriesInput">The object containing the input parameters for
        /// determining the assembly categories.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismSectionAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyCategoriesCalculatorException">Thrown when an error occurs
        /// while performing the calculation.</exception>
        IEnumerable<FailureMechanismSectionAssemblyCategory> CalculateFailureMechanismSectionCategories(
            AssemblyCategoriesInput assemblyCategoriesInput);

        /// <summary>
        /// Performs the calculation for getting the geotechnical failure mechanism section categories.
        /// </summary>
        /// <param name="normativeNorm">The norm which has been defined on the assessment section.</param>
        /// <param name="failureMechanismN">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <param name="failureMechanismContribution">The contribution of a failure mechanism.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismSectionAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyCategoriesCalculatorException">Thrown when an error occurs
        /// while performing the calculation.</exception>
        IEnumerable<FailureMechanismSectionAssemblyCategory> CalculateGeotechnicalFailureMechanismSectionCategories(double normativeNorm,
                                                                                                                    double failureMechanismN,
                                                                                                                    double failureMechanismContribution);
    }
}