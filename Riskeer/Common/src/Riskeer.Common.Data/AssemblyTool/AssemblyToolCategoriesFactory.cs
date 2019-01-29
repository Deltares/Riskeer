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
using Ringtoets.Common.Data.Exceptions;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Factory for calculating the assembly tool categories.
    /// </summary>
    public static class AssemblyToolCategoriesFactory
    {
        /// <summary>
        /// Creates the assessment section assembly categories.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to use in the calculation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="AssessmentSectionAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when an error occurred while creating the categories.</exception>
        public static IEnumerable<AssessmentSectionAssemblyCategory> CreateAssessmentSectionAssemblyCategories(double signalingNorm, double lowerLimitNorm)
        {
            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates the failure mechanism assembly categories.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to use in the calculation.</param>
        /// <param name="failureMechanismContribution">The failure mechanism contribution to calculate with.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when an error occurred while creating the categories.</exception>
        public static IEnumerable<FailureMechanismAssemblyCategory> CreateFailureMechanismAssemblyCategories(
            double signalingNorm,
            double lowerLimitNorm,
            double failureMechanismContribution,
            double n)
        {
            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateFailureMechanismCategories(new AssemblyCategoriesInput(
                                                                          n, failureMechanismContribution / 100,
                                                                          signalingNorm, lowerLimitNorm));
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates the failure mechanism assembly categories.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to use in the calculation.</param>
        /// <param name="failureProbabilityMarginFactor">The failure probability margin factor to
        /// calculate with.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when an error occurred while creating the categories.</exception>
        public static IEnumerable<FailureMechanismAssemblyCategory> CreateFailureMechanismAssemblyCategories(
            double signalingNorm,
            double lowerLimitNorm,
            double failureProbabilityMarginFactor)
        {
            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateFailureMechanismCategories(new AssemblyCategoriesInput(
                                                                          1, failureProbabilityMarginFactor,
                                                                          signalingNorm, lowerLimitNorm));
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates the failure mechanism section assembly categories.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to use in the calculation.</param>
        /// <param name="failureMechanismContribution">The failure mechanism contribution to calculate with.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismSectionAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when an error occurred while creating the categories.</exception>
        public static IEnumerable<FailureMechanismSectionAssemblyCategory> CreateFailureMechanismSectionAssemblyCategories(
            double signalingNorm,
            double lowerLimitNorm,
            double failureMechanismContribution,
            double n)
        {
            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateFailureMechanismSectionCategories(new AssemblyCategoriesInput(
                                                                                 n, failureMechanismContribution / 100,
                                                                                 signalingNorm, lowerLimitNorm));
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates the geotechnical failure mechanism section assembly categories.
        /// </summary>
        /// <param name="normativeNorm">The norm to use in the calculation.</param>
        /// <param name="failureMechanismContribution">The failure mechanism contribution to calculate with.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismSectionAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when an error occurred while creating the categories.</exception>
        public static IEnumerable<FailureMechanismSectionAssemblyCategory> CreateGeotechnicalFailureMechanismSectionAssemblyCategories(
            double normativeNorm,
            double failureMechanismContribution,
            double n)
        {
            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateGeotechnicalFailureMechanismSectionCategories(normativeNorm,
                                                                                         n,
                                                                                         failureMechanismContribution / 100);
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }
    }
}