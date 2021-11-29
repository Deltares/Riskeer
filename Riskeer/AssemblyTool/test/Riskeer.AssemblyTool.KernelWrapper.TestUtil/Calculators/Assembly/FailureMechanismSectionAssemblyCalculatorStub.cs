// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly
{
    /// <summary>
    /// Failure mechanism section assembly calculator stub for testing purposes.
    /// </summary>
    public class FailureMechanismSectionAssemblyCalculatorStub : IFailureMechanismSectionAssemblyCalculator
    {
        /// <summary>
        /// Gets the signaling norm that is used in the calculation.
        /// </summary>
        public double SignalingNorm { get; private set; }

        /// <summary>
        /// Gets the lower limit norm that is used in the calculation.
        /// </summary>
        public double LowerLimitNorm { get; private set; }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSectionAssemblyInput"/> that is used in the calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyInput FailureMechanismSectionAssemblyInput { get; private set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FailureMechanismSectionAssemblyResult"/> of the calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyResult FailureMechanismSectionAssemblyResultOutput { get; set; }

        public FailureMechanismSectionAssemblyResult AssembleFailureMechanismSection(double signalingNorm, double lowerLimitNorm,
                                                                                     FailureMechanismSectionAssemblyInput input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            SignalingNorm = signalingNorm;
            LowerLimitNorm = lowerLimitNorm;
            FailureMechanismSectionAssemblyInput = input;

            return FailureMechanismSectionAssemblyResultOutput ??
                   (FailureMechanismSectionAssemblyResultOutput =
                        new FailureMechanismSectionAssemblyResult(0.01, 0.1, 10, FailureMechanismSectionAssemblyGroup.I));
        }
    }
}