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
using System.ComponentModel;
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Assembly;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using AssemblyTool.Kernel.Data.CalculationResults;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assessments
{
    /// <summary>
    /// Class representing a failure mechanism section assembly assessment calculator.
    /// </summary>
    public class FailureMechanismSectionAssessmentAssemblyCalculator : IFailureMechanismSectionAssessmentAssemblyCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssessmentAssemblyCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionAssessmentAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public FailureMechanismSectionAssessment AssembleSimpleAssessment(SimpleAssessmentResultType input)
        {
            IFailureMechanismSectionAssemblyCalculator kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
            CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> output = kernel.SimpleAssessmentDirectFailureMechanisms(
                FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResult(input));

            return FailureMechanismSectionAssessmentCreator.Create(output.Result);
        }

        public FailureMechanismSectionAssessment AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType input)
        {
            throw new NotImplementedException();
        }
    }
}