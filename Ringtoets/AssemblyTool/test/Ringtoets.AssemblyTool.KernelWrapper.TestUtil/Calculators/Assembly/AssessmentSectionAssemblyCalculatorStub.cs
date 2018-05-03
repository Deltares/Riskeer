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
        /// Gets the lower norm input when assembling the assessment section with failure mechanisms
        /// with probability;
        /// </summary>
        public double LowerLimitNormInput { get; private set; }

        /// <summary>
        /// Gets the lower norm input when assembling the assessment section with failure mechanisms
        /// with probability;
        /// </summary>
        public double SignalingNormInput { get; private set; }

        /// <summary>
        /// Gets the collection of failure mechanism assembly input when assembling the
        /// assessment section with failure mechanisms with probability.
        /// </summary>
        public IEnumerable<FailureMechanismAssembly> FailureMechanismAssemblyInput { get; private set; }

        /// <summary>
        /// Gets the collection of failure mechanism assembly category group input when assembling the 
        /// assessment section with failure mechanisms without probability.
        /// </summary>
        public IEnumerable<FailureMechanismAssemblyCategoryGroup> FailureMechanismAssemblyCategoryGroupInput { get; private set; }

        /// <summary>
        /// Gets the failure mechanisms without probability input when assembling the assessment section.
        /// </summary>
        public AssessmentSectionAssemblyCategoryGroup FailureMechanismsWithoutProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the failure mechanisms with probability input when assembling the assessment section.
        /// </summary>
        public AssessmentSectionAssembly FailureMechanismsWithProbabilityInput { get; private set; }

        /// <summary>
        /// Gets or sets the output of the assessment section assembly for failure
        ///  mechanisms with probability.
        /// </summary>
        public AssessmentSectionAssembly AssessmentSectionAssemblyOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the assessment section assembly category group for failure 
        /// mechanisms without probability or for assembling the assessment section.
        /// </summary>
        public AssessmentSectionAssemblyCategoryGroup? AssessmentSectionAssemblyCategoryGroupOutput { get; set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown when performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public AssessmentSectionAssembly AssembleFailureMechanisms(IEnumerable<FailureMechanismAssembly> input,
                                                                   double signalingNorm,
                                                                   double lowerLimitNorm)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismAssemblyInput = input;
            LowerLimitNormInput = lowerLimitNorm;
            SignalingNormInput = signalingNorm;

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
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismsWithoutProbabilityInput = failureMechanismsWithoutProbability;
            FailureMechanismsWithProbabilityInput = failureMechanismsWithProbability;

            return AssessmentSectionAssemblyCategoryGroupOutput ?? AssessmentSectionAssemblyCategoryGroup.C;
        }
    }
}