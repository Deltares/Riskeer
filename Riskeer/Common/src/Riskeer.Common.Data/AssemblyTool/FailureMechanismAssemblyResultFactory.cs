// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Factory for calculating assembly results of a failure mechanism.
    /// </summary>
    public static class FailureMechanismAssemblyResultFactory
    {
        /// <summary>
        /// Assembles the failure mechanism based on its input arguments.
        /// </summary>
        /// <param name="failureMechanismN">The length effect factor 'N' of the failure mechanism.</param>
        /// <param name="failureMechanismSectionAssemblyResults">A collection of <see cref="FailureMechanismSectionAssemblyResult"/>.</param>
        /// <param name="applyLengthEffect">Indicator whether the failure mechanism section length effect is applied.</param>
        /// <param name="failureMechanismAssemblyResult">The <see cref="FailureMechanismAssemblyResult"/>.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyResultWrapper"/> containing the assembly result of the failure mechanism.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionAssemblyResults"/>
        /// or <paramref name="failureMechanismAssemblyResult"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the failure mechanism could not be successfully assembled.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismAssemblyProbabilityResultType"/>
        /// of the failure mechanism is not supported.</exception>
        public static FailureMechanismAssemblyResultWrapper AssembleFailureMechanism(
            double failureMechanismN, IEnumerable<FailureMechanismSectionAssemblyResult> failureMechanismSectionAssemblyResults,
            bool applyLengthEffect, FailureMechanismAssemblyResult failureMechanismAssemblyResult)
        {
            if (failureMechanismSectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssemblyResults));
            }

            if (failureMechanismAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismAssemblyResult));
            }

            if (failureMechanismAssemblyResult.ProbabilityResultType == FailureMechanismAssemblyProbabilityResultType.None)
            {
                throw new AssemblyException(Resources.FailureMechanismAssemblyResultFactory_AssembleFailureMechanism_Missing_input_for_assembly);
            }

            if (failureMechanismAssemblyResult.ProbabilityResultType == FailureMechanismAssemblyProbabilityResultType.Manual)
            {
                return new FailureMechanismAssemblyResultWrapper(
                    failureMechanismAssemblyResult.ManualFailureMechanismAssemblyProbability,
                    AssemblyMethod.Manual);
            }

            try
            {
                IFailureMechanismAssemblyCalculator calculator =
                    AssemblyToolCalculatorFactory.Instance.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactory.Instance);

                switch (failureMechanismAssemblyResult.ProbabilityResultType)
                {
                    case FailureMechanismAssemblyProbabilityResultType.P1:
                        return calculator.AssembleWithIndependentSectionResults(failureMechanismSectionAssemblyResults);
                    case FailureMechanismAssemblyProbabilityResultType.P2:
                        return calculator.AssembleWithWorstSectionResult(failureMechanismSectionAssemblyResults);
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (FailureMechanismAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }
    }
}