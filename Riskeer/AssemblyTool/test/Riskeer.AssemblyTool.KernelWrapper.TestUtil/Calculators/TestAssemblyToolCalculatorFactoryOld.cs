﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators
{
    /// <summary>
    /// Factory which creates assembly tool calculator stubs for testing purposes.
    /// </summary>
    public class TestAssemblyToolCalculatorFactoryOld : IAssemblyToolCalculatorFactoryOld
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestAssemblyToolCalculatorFactoryOld"/>.
        /// </summary>
        public TestAssemblyToolCalculatorFactoryOld()
        {
            LastCreatedAssessmentSectionAssemblyGroupBoundariesCalculator = new AssessmentSectionAssemblyGroupBoundariesCalculatorStub();
        }

        /// <summary>
        /// Gets the last created <see cref="AssessmentSectionAssemblyGroupBoundariesCalculatorStub"/>.
        /// </summary>
        public AssessmentSectionAssemblyGroupBoundariesCalculatorStub LastCreatedAssessmentSectionAssemblyGroupBoundariesCalculator { get; }

        public IAssessmentSectionAssemblyGroupBoundariesCalculator CreateAssemblyCategoriesCalculator(IAssemblyToolKernelFactoryOld factory)
        {
            return LastCreatedAssessmentSectionAssemblyGroupBoundariesCalculator;
        }
    }
}