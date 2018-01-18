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
using Ringtoets.AssemblyTool.Data.Input;
using Ringtoets.AssemblyTool.Data.Output;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels.Categories;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories
{
    /// <summary>
    /// Assembly categories calculator stub for testing purposes.
    /// </summary>
    public class AssemblyCategoriesCalculatorStub : IAssemblyCategoriesCalculator
    {
        /// <summary>
        /// Gets the assembly categories calculator input.
        /// </summary>
        public AssemblyCategoriesCalculatorInput Input { get; private set; }

        /// <summary>
        /// Gets or sets the output of the <see cref="CalculateAssessmentSectionCategories"/> calculation.
        /// </summary>
        public IEnumerable<AssessmentSectionAssemblyCategoryResult> AssessmentSectionCategoriesOutput { get; set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public IEnumerable<AssessmentSectionAssemblyCategoryResult> CalculateAssessmentSectionCategories(
            AssemblyCategoriesCalculatorInput input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssemblyCategoriesCalculatorException("Message", new AssemblyCategoriesKernelWrapperException());
            }

            Input = input;

            return AssessmentSectionCategoriesOutput
                   ?? (AssessmentSectionCategoriesOutput = new[]
                   {
                       new AssessmentSectionAssemblyCategoryResult(1, 2, AssessmentSectionAssemblyCategoryResultType.A),
                       new AssessmentSectionAssemblyCategoryResult(2.01, 3, AssessmentSectionAssemblyCategoryResultType.B),
                       new AssessmentSectionAssemblyCategoryResult(3.01, 4, AssessmentSectionAssemblyCategoryResultType.C)
                   });
        }
    }
}