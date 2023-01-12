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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Groups
{
    /// <summary>
    /// Assembly category limits kernel stub for testing purposes.
    /// </summary>
    public class AssemblyCategoryLimitsKernelStub : ICategoryLimitsCalculator
    {
        /// <summary>
        /// Gets a value indicating whether a calculation was called or not. 
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public AssessmentSection AssessmentSection { get; private set; }

        /// <summary>
        /// Sets the interpretation categories.
        /// </summary>
        public CategoriesList<InterpretationCategory> InterpretationCategoryLimits { private get; set; }

        /// <summary>
        /// Sets the assessment section categories.
        /// </summary>
        public CategoriesList<AssessmentSectionCategory> AssessmentSectionCategoryLimits { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="Exception"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="AssemblyException"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowAssemblyExceptionOnCalculate { private get; set; }

        public CategoriesList<AssessmentSectionCategory> CalculateAssessmentSectionCategoryLimitsBoi21(AssessmentSection section)
        {
            ThrowException();

            Calculated = true;
            AssessmentSection = section;

            return AssessmentSectionCategoryLimits;
        }

        public CategoriesList<InterpretationCategory> CalculateInterpretationCategoryLimitsBoi01(AssessmentSection section)
        {
            ThrowException();

            Calculated = true;
            AssessmentSection = section;

            return InterpretationCategoryLimits;
        }

        private void ThrowException()
        {
            AssemblyKernelStubHelper.ThrowException(ThrowExceptionOnCalculate, ThrowAssemblyExceptionOnCalculate, EAssemblyErrors.EmptyResultsList);
        }
    }
}