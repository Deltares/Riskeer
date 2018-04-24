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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;

namespace Ringtoets.DuneErosion.Data
{
    /// <summary>
    /// Factory for creating assembly results for dune erosion failure mechanism section results.
    /// </summary>
    public static class DuneErosionFailureMechanismSectionResultAssemblyFactory
    {
        /// <summary>
        /// Assembles the simple assessment results based on <paramref name="failureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// simple assembly results for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/> 
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup AssembleSimpleAssessment(DuneErosionFailureMechanismSectionResult failureMechanismSectionResult)
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
                return calculator.AssembleSimpleAssessment(failureMechanismSectionResult.SimpleAssessmentResult).Group;
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the detailed assessment results.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// detailed assembly results for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(
            DuneErosionFailureMechanismSectionResult failureMechanismSectionResult)
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
                return calculator.AssembleDetailedAssessment(failureMechanismSectionResult.DetailedAssessmentResultForFactorizedSignalingNorm,
                                                             failureMechanismSectionResult.DetailedAssessmentResultForSignalingNorm,
                                                             failureMechanismSectionResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                                                             failureMechanismSectionResult.DetailedAssessmentResultForLowerLimitNorm,
                                                             failureMechanismSectionResult.DetailedAssessmentResultForFactorizedLowerLimitNorm);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the tailor made assessment results.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// tailor made assembly results for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(
            DuneErosionFailureMechanismSectionResult failureMechanismSectionResult)
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
                return calculator.AssembleTailorMadeAssessment(failureMechanismSectionResult.TailorMadeAssessmentResult);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the combined assessment results.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// combined assembly results for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup AssembleCombinedAssessment(
            DuneErosionFailureMechanismSectionResult failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            FailureMechanismSectionAssemblyCategoryGroup simpleAssembly = AssembleSimpleAssessment(failureMechanismSectionResult);
            FailureMechanismSectionAssemblyCategoryGroup detailedAssembly = AssembleDetailedAssessment(failureMechanismSectionResult);
            FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly = AssembleTailorMadeAssessment(failureMechanismSectionResult);

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the failure mechanism assembly.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The failure mechanism section results to
        /// get the assembly for.</param>
        /// <param name="considerManualAssembly">Indicator whether the manual assembly should be used in the assembly.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResults"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismAssemblyCategoryGroup AssembleFailureMechanism(
            IEnumerable<DuneErosionFailureMechanismSectionResult> failureMechanismSectionResults,
            bool considerManualAssembly = true)
        {
            if (failureMechanismSectionResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResults));
            }

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator sectionCalculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            var sectionAssemblies = new List<FailureMechanismSectionAssemblyCategoryGroup>();

            try
            {
                foreach (DuneErosionFailureMechanismSectionResult sectionResult in failureMechanismSectionResults)
                {
                    if (sectionResult.UseManualAssemblyCategoryGroup && considerManualAssembly)
                    {
                        sectionAssemblies.Add(sectionCalculator.AssembleTailorMadeAssessment(sectionResult.ManualAssemblyCategoryGroup));
                    }
                    else
                    {
                        sectionAssemblies.Add(AssembleCombinedAssessment(sectionResult));
                    }
                }

                IFailureMechanismAssemblyCalculator calculator =
                    calculatorFactory.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactory.Instance);

                return calculator.AssembleFailureMechanism(sectionAssemblies);
            }
            catch (Exception e) when (e is FailureMechanismAssemblyCalculatorException || e is FailureMechanismSectionAssemblyCalculatorException)
            {
                throw new AssemblyException(e.Message, e);
            }
        }
    }
}