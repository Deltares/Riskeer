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
using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Class representing a failure mechanism assembly calculator.
    /// </summary>
    public class FailureMechanismAssemblyCalculator : IFailureMechanismAssemblyCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public FailureMechanismAssemblyCategoryGroup AssembleFailureMechanism(IEnumerable<FailureMechanismSectionAssembly> sectionAssemblies)
        {
            try
            {
                IFailureMechanismResultAssembler kernel = factory.CreateFailureMechanismAssemblyKernel();
                EFailureMechanismCategory output = kernel.AssembleFailureMechanismWbi1A1(
                    sectionAssemblies.Select(FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult).ToArray(),
                    false);

                return FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(output);
            }
            catch (Exception e)
            {
                throw new FailureMechanismAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismAssembly AssembleFailureMechanism(IEnumerable<FailureMechanismSectionAssembly> sectionAssemblies,
                                                                 AssemblyCategoriesInput assemblyCategoriesInput)
        {
            try
            {
                IFailureMechanismResultAssembler kernel = factory.CreateFailureMechanismAssemblyKernel();
                FailureMechanismAssemblyResult output = kernel.AssembleFailureMechanismWbi1B1(
                    new AssessmentSection(1, assemblyCategoriesInput.SignalingNorm, assemblyCategoriesInput.LowerLimitNorm),
                    new FailureMechanism(assemblyCategoriesInput.N, assemblyCategoriesInput.FailureMechanismContribution),
                    sectionAssemblies.Select(FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult).ToArray(),
                    false);

                return FailureMechanismAssemblyCreator.Create(output);
            }
            catch (Exception e)
            {
                throw new FailureMechanismAssemblyCalculatorException(e.Message, e);
            }
        }
    }
}