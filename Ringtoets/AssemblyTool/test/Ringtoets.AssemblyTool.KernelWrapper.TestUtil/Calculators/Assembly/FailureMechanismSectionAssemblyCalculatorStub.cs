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
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly
{
    /// <summary>
    /// Failure mechanism section assembly calculator stub for testing purposes.
    /// </summary>
    public class FailureMechanismSectionAssemblyCalculatorStub : IFailureMechanismSectionAssemblyCalculator
    {
        /// <summary>
        /// Gets or sets the output of the calculation.
        /// </summary>
        public FailureMechanismSectionAssembly SimpleAssessmentAssemblyOutput { get; private set; }

        /// <summary>
        /// Gets the input of the calculation.
        /// </summary>
        public SimpleAssessmentResultType SimpleAssessmentInput { get; private set; }

        /// <summary>
        /// Gets the input of the validity only calculation.
        /// </summary>
        public SimpleAssessmentResultValidityOnlyType SimpleAssessmentValidityOnlyInput { get; private set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultType input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            SimpleAssessmentInput = input;

            return SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.Iv);
        }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            SimpleAssessmentValidityOnlyInput = input;

            return SimpleAssessmentAssemblyOutput ??
                   (SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIIv));
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(double probability, IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            throw new NotImplementedException();
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(double probability,
                                                                          IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
                                                                          double n)
        {
            throw new NotImplementedException();
        }
    }
}