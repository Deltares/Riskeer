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

using System;
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Data.FailureMechanism;
using IFailureMechanismSectionAssemblyCalculatorKernel = AssemblyTool.Kernel.Assembly.IFailureMechanismSectionAssemblyCalculator;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Class representing a failure mechanism section assembly calculator.
    /// </summary>
    public class FailureMechanismSectionAssemblyCalculator : IFailureMechanismSectionAssemblyCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultType input)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> output = kernel.SimpleAssessmentDirectFailureMechanisms(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResult(input));

                return FailureMechanismSectionAssemblyCreator.Create(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType input)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> output = kernel.SimpleAssessmentDirectFailureMechanisms(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimplecalclCalculationResultValidityOnly(input));

                return FailureMechanismSectionAssemblyCreator.Create(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }
    }
}