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

using Assembly.Kernel.Implementations;
using Assembly.Kernel.Interfaces;

namespace Riskeer.AssemblyTool.KernelWrapper.Kernels
{
    /// <summary>
    /// Factory that creates the assembly tool kernels.
    /// </summary>
    public class AssemblyToolKernelFactory : IAssemblyToolKernelFactory
    {
        private static IAssemblyToolKernelFactory instance;

        private AssemblyToolKernelFactory() {}

        /// <summary>
        /// Gets or sets an instance of <see cref="IAssemblyToolKernelFactory"/>.
        /// </summary>
        public static IAssemblyToolKernelFactory Instance
        {
            get => instance ?? (instance = new AssemblyToolKernelFactory());
            set => instance = value;
        }

        public ICategoryLimitsCalculator CreateAssemblyGroupsKernel()
        {
            return new CategoryLimitsCalculator();
        }

        public IAssessmentResultsTranslator CreateFailureMechanismSectionAssemblyKernel()
        {
            return new AssessmentResultsTranslator();
        }

        public IFailureMechanismResultAssembler CreateFailureMechanismAssemblyKernel()
        {
            return new FailureMechanismResultAssembler();
        }

        public ICommonFailureMechanismSectionAssembler CreateCombinedFailureMechanismSectionAssemblyKernel()
        {
            return new CommonFailureMechanismSectionAssembler();
        }

        public IAssessmentGradeAssembler CreateAssessmentSectionAssemblyKernel()
        {
            return new AssessmentGradeAssembler();
        }
    }
}