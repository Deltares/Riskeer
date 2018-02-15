﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Data.Exceptions;

namespace Ringtoets.Common.Data.AssemblyTool
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
        /// <exception cref="AssemblyFactoryException">Thrown when an error occurred while creating the categories.</exception>
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
                throw new AssemblyFactoryException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates the failure mechanism section assembly categories.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to use in the calculation.</param>
        /// <param name="probabilityDistributionFactor">The probability distribution factor to calculate with.</param>
        /// <param name="n">The n to calculate with.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismSectionAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyFactoryException">Thrown when an error occurred while creating the categories.</exception>
        public static IEnumerable<FailureMechanismSectionAssemblyCategory> CreateFailureMechanismSectionAssemblyCategories(
            double signalingNorm,
            double lowerLimitNorm,
            double probabilityDistributionFactor,
            double n)
        {
            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateFailureMechanismSectionCategories(signalingNorm,
                                                                             lowerLimitNorm,
                                                                             probabilityDistributionFactor,
                                                                             n);
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
                throw new AssemblyFactoryException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates the geotechnic failure mechanism section assembly categories.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to use in the calculation.</param>
        /// <param name="probabilityDistributionFactor">The probability distribution factor to calculate with.</param>
        /// <param name="n">The n to calculate with.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="FailureMechanismSectionAssemblyCategory"/>.</returns>
        /// <exception cref="AssemblyFactoryException">Thrown when an error occurred while creating the categories.</exception>
        public static IEnumerable<FailureMechanismSectionAssemblyCategory> CreateGeotechnicFailureMechanismSectionAssemblyCategories(
            double signalingNorm,
            double lowerLimitNorm,
            double probabilityDistributionFactor,
            double n)
        {
            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateGeotechnicFailureMechanismSectionCategories(signalingNorm,
                                                                                       lowerLimitNorm,
                                                                                       probabilityDistributionFactor,
                                                                                       n);
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
                throw new AssemblyFactoryException(e.Message, e);
            }
        }
    }
}