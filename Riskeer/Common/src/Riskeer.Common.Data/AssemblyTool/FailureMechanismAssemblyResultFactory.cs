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
using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.Common.Data.Exceptions;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Factory for creating assembly results of a failure mechanism.
    /// </summary>
    public static class FailureMechanismAssemblyResultFactory
    {
        /// <summary>
        /// Assembles the failure mechanism based on its input arguments.
        /// </summary>
        /// <param name="failureMechanismN">The length effect factor 'N' of the failure mechanism.</param>
        /// <param name="failureMechanismSectionAssemblyResults">A collection of <see cref="FailureMechanismSectionAssemblyResult"/>.</param>
        /// <returns>A failure probability of the failure mechanism.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionAssemblyResults"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the failure mechanism could not be successfully assembled.</exception>
        public static double AssembleFailureMechanism(double failureMechanismN,
                                                      IEnumerable<FailureMechanismSectionAssemblyResult> failureMechanismSectionAssemblyResults)
        {
            if (failureMechanismSectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssemblyResults));
            }

            try
            {
                IFailureMechanismAssemblyCalculator calculator =
                    AssemblyToolCalculatorFactory.Instance.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactory.Instance);

                return calculator.Assemble(failureMechanismN, failureMechanismSectionAssemblyResults);
            }
            catch (FailureMechanismAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }
    }
}