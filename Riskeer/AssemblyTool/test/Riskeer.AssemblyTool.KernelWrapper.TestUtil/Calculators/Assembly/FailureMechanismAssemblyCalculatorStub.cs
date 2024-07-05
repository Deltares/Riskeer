// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
        /// Gets the collection of <see cref="FailureMechanismSectionAssemblyResult"/> that is used in the calculation.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyResult> SectionAssemblyResultsInput { get; private set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Gets an indicator whether the assembly is based on the worst section result.
        /// </summary>
        public bool AssembleBasedOnWorstSectionResultCalled { get; private set; }

        /// <summary>
        /// Gets an indicator whether the assembly is based on independent section results.
        /// </summary>
        public bool AssembleBasedOnIndependentSectionResultsCalled { get; private set; }

        /// <summary>
        /// Gets or sets the result output of the assembly calculation.
        /// </summary>
        public FailureMechanismAssemblyResultWrapper AssemblyResultOutput { get; set; }

        public FailureMechanismAssemblyResultWrapper AssembleBasedOnIndependentSectionResults(IEnumerable<FailureMechanismSectionAssemblyResult> sectionAssemblyResults)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismAssemblyCalculatorException("Message", new Exception());
            }

            AssembleBasedOnIndependentSectionResultsCalled = true;

            SectionAssemblyResultsInput = sectionAssemblyResults;
            return AssemblyResultOutput ?? (AssemblyResultOutput = new FailureMechanismAssemblyResultWrapper(0.1, AssemblyMethod.BOI1A1));
        }

        public FailureMechanismAssemblyResultWrapper AssembleBasedOnWorstSectionResult(IEnumerable<FailureMechanismSectionAssemblyResult> sectionAssemblyResults)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismAssemblyCalculatorException("Message", new Exception());
            }

            AssembleBasedOnWorstSectionResultCalled = true;

            SectionAssemblyResultsInput = sectionAssemblyResults;
            return AssemblyResultOutput ?? (AssemblyResultOutput = new FailureMechanismAssemblyResultWrapper(0.1, AssemblyMethod.BOI1A2));
        }
    }
}