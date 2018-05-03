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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly
{
    /// <summary>
    /// Assessment section assembly calculator stub for testing purposes.
    /// </summary>
    public class AssessmentSectionAssemblyCalculatorStub : IAssessmentSectionAssemblyCalculator
    {
        /// <summary>
        /// Gets the assembly categories input when assembling the assessment section with 
        /// failure mechanisms with probability.
        /// </summary>
        public AssemblyCategoriesInput AssemblyCategoriesInput { get; private set; }

        /// <summary>
        /// Gets the collection of failure mechanism assembly input when assembling the
        /// assessment section with failure mechanisms with probability.
        /// </summary>
        public IEnumerable<FailureMechanismAssembly> FailureMechanismAssemblyInput { get; private set; }

        /// <summary>
        /// Gets the collection of failure mechanism assembly input when assembling the 
        /// assessment section with failure mechanisms without probability
        /// </summary>
        public IEnumerable<FailureMechanismAssemblyCategoryGroup> FailureMechanismAssemblyCategoryGroupInput { get; private set; }

        /// <summary>
        /// Gets or sets the output of the assessment section assembly for failure
        ///  mechanisms with probability.
        /// </summary>
        public AssessmentSectionAssembly AssessmentSectionAssemblyOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the assessment section assembly for failure 
        /// mechanisms without probability.
        /// </summary>
        public AssessmentSectionAssemblyCategoryGroup? AssessmentSectionAssemblyCategoryGroupOutput { get; set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown when performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public AssessmentSectionAssembly AssembleFailureMechanisms(IEnumerable<FailureMechanismAssembly> input,
                                                                   AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismAssemblyInput = input;
            AssemblyCategoriesInput = assemblyCategoriesInput;

            return AssessmentSectionAssemblyOutput ??
                   new AssessmentSectionAssembly(0.75, AssessmentSectionAssemblyCategoryGroup.D);
        }

        public AssessmentSectionAssemblyCategoryGroup AssembleFailureMechanisms(IEnumerable<FailureMechanismAssemblyCategoryGroup> input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismAssemblyCategoryGroupInput = input;

            return AssessmentSectionAssemblyCategoryGroupOutput ?? AssessmentSectionAssemblyCategoryGroup.D;
        }

        public AssessmentSectionAssemblyCategoryGroup AssembleAssessmentSection(AssessmentSectionAssemblyCategoryGroup failureMechanismsWithoutProbability,
                                                                                AssessmentSectionAssembly failureMechanismsWithProbability)
        {
            throw new NotImplementedException();
        }
    }
}