// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly
{
    /// <summary>
    /// Failure mechanism assembly calculator stub for testing purposes.
    /// </summary>
    public class FailureMechanismAssemblyCalculatorStub : IFailureMechanismAssemblyCalculator
    {
        /// <summary>
        /// Gets the collection of <see cref="FailureMechanismSectionAssembly"/>.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssembly> FailureMechanismSectionAssemblies { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> FailureMechanismSectionCategories { get; private set; }

        /// <summary>
        /// Gets or sets the output of the failure mechanism assembly without probabilities.
        /// </summary>
        public FailureMechanismAssemblyCategoryGroup? FailureMechanismAssemblyCategoryGroupOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the failure mechanism assembly with probabilities.
        /// </summary>
        public FailureMechanismAssembly FailureMechanismAssemblyOutput { get; set; }

        /// <summary>
        /// Gets the assembly categories input used in the assembly calculation methods.
        /// </summary>
        public AssemblyCategoriesInput AssemblyCategoriesInput { get; private set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public FailureMechanismAssemblyCategoryGroup Assemble(IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> sectionCategories)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismSectionCategories = sectionCategories;

            if (FailureMechanismAssemblyCategoryGroupOutput == null)
            {
                FailureMechanismAssemblyCategoryGroupOutput = FailureMechanismAssemblyCategoryGroup.IIt;
            }

            return FailureMechanismAssemblyCategoryGroupOutput.Value;
        }

        public FailureMechanismAssembly Assemble(IEnumerable<FailureMechanismSectionAssembly> sectionAssemblies,
                                                 AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismSectionAssemblies = sectionAssemblies;
            AssemblyCategoriesInput = assemblyCategoriesInput;

            return FailureMechanismAssemblyOutput ??
                   (FailureMechanismAssemblyOutput = new FailureMechanismAssembly(1.0, FailureMechanismAssemblyCategoryGroup.IIIt));
        }
    }
}