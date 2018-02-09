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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Assembly service for assembling the assembly tool results for grass cover erosion outwards.
    /// </summary>
    public static class GrassCoverErosionOutwardsAssemblyService
    {
        /// <summary>
        /// Assembles the simple assessment results.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// simple assembly results for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/> 
        /// is <c>null</c>.</exception>
        public static void AssembleSimpleAssessment(GrassCoverErosionOutwardsFailureMechanismSectionResult failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                failureMechanismSectionResult.SimpleAssemblyResult =
                    calculator.AssembleSimpleAssessment(failureMechanismSectionResult.SimpleAssessmentInput);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException) {}
        }
    }
}