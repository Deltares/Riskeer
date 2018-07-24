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
using System.Linq;
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
        public FailureMechanismAssemblyCategoryGroup FailureMechanismsWithoutProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the failure mechanisms with probability input when assembling the assessment section.
        /// </summary>
        public FailureMechanismAssembly FailureMechanismsWithProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the combined failure mechanism sections input.
        /// </summary>
        public IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> CombinedFailureMechanismSectionsInput { get; private set; }

        /// <summary>
        /// Gets the assessment section length input.
        /// </summary>
        public double AssessmentSectionLength { get; private set; }

        /// <summary>
        /// Gets or sets the output of the assessment section assembly for failure
        ///  mechanisms with probability.
        /// </summary>
        public FailureMechanismAssembly AssembleFailureMechanismsAssemblyOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the assessment section assembly category group when assembling failure 
        /// mechanisms without probability.
        /// </summary>
        public FailureMechanismAssemblyCategoryGroup? AssembleFailureMechanismsAssemblyCategoryGroupOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the assessment section assembly category group 
        /// when assembling an assessment section.
        /// </summary>
        public AssessmentSectionAssemblyCategoryGroup? AssembleAssessmentSectionCategoryGroupOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the combined failure mechanism section assembly.
        /// </summary>
        public IEnumerable<CombinedFailureMechanismSectionAssembly> CombinedFailureMechanismSectionAssemblyOutput { get; set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public FailureMechanismAssembly AssembleFailureMechanisms(IEnumerable<FailureMechanismAssembly> input,
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

            return AssembleFailureMechanismsAssemblyOutput ??
                   new FailureMechanismAssembly(0.75, FailureMechanismAssemblyCategoryGroup.IIIt);
        }

        public FailureMechanismAssemblyCategoryGroup AssembleFailureMechanisms(IEnumerable<FailureMechanismAssemblyCategoryGroup> input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismAssemblyCategoryGroupInput = input;

            if (AssembleFailureMechanismsAssemblyCategoryGroupOutput == null)
            {
                AssembleFailureMechanismsAssemblyCategoryGroupOutput = FailureMechanismAssemblyCategoryGroup.IIIt;
            }

            return AssembleFailureMechanismsAssemblyCategoryGroupOutput.Value;
        }

        public AssessmentSectionAssemblyCategoryGroup AssembleAssessmentSection(FailureMechanismAssemblyCategoryGroup failureMechanismsWithoutProbability,
                                                                                FailureMechanismAssembly failureMechanismsWithProbability)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismsWithoutProbabilityInput = failureMechanismsWithoutProbability;
            FailureMechanismsWithProbabilityInput = failureMechanismsWithProbability;

            if (AssembleAssessmentSectionCategoryGroupOutput == null)
            {
                AssembleAssessmentSectionCategoryGroupOutput = AssessmentSectionAssemblyCategoryGroup.C;
            }

            return AssembleAssessmentSectionCategoryGroupOutput.Value;
        }

        public IEnumerable<CombinedFailureMechanismSectionAssembly> AssembleCombinedFailureMechanismSections(IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input,
                                                                                                             double assessmentSectionLength)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            CombinedFailureMechanismSectionsInput = input;
            AssessmentSectionLength = assessmentSectionLength;

            return CombinedFailureMechanismSectionAssemblyOutput ?? (CombinedFailureMechanismSectionAssemblyOutput = new[]
                                                                        {
                                                                            new CombinedFailureMechanismSectionAssembly(
                                                                                new CombinedAssemblyFailureMechanismSection(0, 1, FailureMechanismSectionAssemblyCategoryGroup.IIIv),
                                                                                input.Select(failureMechanism => FailureMechanismSectionAssemblyCategoryGroup.VIv).ToArray())
                                                                        });
        }
    }
}