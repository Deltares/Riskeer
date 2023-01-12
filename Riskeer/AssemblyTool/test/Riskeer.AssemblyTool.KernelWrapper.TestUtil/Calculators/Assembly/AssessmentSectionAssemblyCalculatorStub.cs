// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly
{
    /// <summary>
    /// Assessment section assembly calculator stub for testing purposes.
    /// </summary>
    public class AssessmentSectionAssemblyCalculatorStub : IAssessmentSectionAssemblyCalculator
    {
        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Gets the failure mechanism probabilities input.
        /// </summary>
        public IEnumerable<double> FailureMechanismProbabilitiesInput { get; private set; }

        /// <summary>
        /// Gets the maximum allowable flooding probability input.
        /// </summary>
        public double MaximumAllowableFloodingProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the signal flooding probability input.
        /// </summary>
        public double SignalFloodingProbability { get; private set; }

        /// <summary>
        /// Gets or sets the output of an assessment section assembly.
        /// </summary>
        public AssessmentSectionAssemblyResultWrapper AssessmentSectionAssemblyResult { get; set; }

        /// <summary>
        /// Gets the combined failure mechanism sections input.
        /// </summary>
        public IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> CombinedFailureMechanismSectionsInput { get; private set; }

        /// <summary>
        /// Gets the assessment section length input.
        /// </summary>
        public double AssessmentSectionLength { get; private set; }

        /// <summary>
        /// Gets or sets the output of the combined failure mechanism section assembly.
        /// </summary>
        public CombinedFailureMechanismSectionAssemblyResultWrapper CombinedFailureMechanismSectionAssemblyOutput { get; set; }

        public AssessmentSectionAssemblyResultWrapper AssembleAssessmentSection(IEnumerable<double> failureMechanismProbabilities, double maximumAllowableFloodingProbability, double signalFloodingProbability)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            FailureMechanismProbabilitiesInput = failureMechanismProbabilities;
            MaximumAllowableFloodingProbabilityInput = maximumAllowableFloodingProbability;
            SignalFloodingProbability = signalFloodingProbability;

            return AssessmentSectionAssemblyResult ?? (AssessmentSectionAssemblyResult =
                                                           new AssessmentSectionAssemblyResultWrapper(
                                                               new AssessmentSectionAssemblyResult(0.14, AssessmentSectionAssemblyGroup.APlus),
                                                               AssemblyMethod.BOI2A1, AssemblyMethod.BOI2B1));
        }

        public CombinedFailureMechanismSectionAssemblyResultWrapper AssembleCombinedFailureMechanismSections(
            IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input, double assessmentSectionLength)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssessmentSectionAssemblyCalculatorException("Message", new Exception());
            }

            CombinedFailureMechanismSectionsInput = input;
            AssessmentSectionLength = assessmentSectionLength;

            return CombinedFailureMechanismSectionAssemblyOutput ?? (CombinedFailureMechanismSectionAssemblyOutput = new CombinedFailureMechanismSectionAssemblyResultWrapper(
                                                                         new[]
                                                                         {
                                                                             new CombinedFailureMechanismSectionAssembly(
                                                                                 new CombinedAssemblyFailureMechanismSection(
                                                                                     0, 1, FailureMechanismSectionAssemblyGroup.Zero),
                                                                                 input.Select(failureMechanism => FailureMechanismSectionAssemblyGroup.Dominant).ToArray())
                                                                         }, AssemblyMethod.BOI3A1, AssemblyMethod.BOI3B1, AssemblyMethod.BOI3C1));
        }
    }
}