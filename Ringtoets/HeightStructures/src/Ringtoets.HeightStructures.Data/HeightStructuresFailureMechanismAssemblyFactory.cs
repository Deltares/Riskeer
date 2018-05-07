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
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Factory for assembling assembly results for a height structures failure mechanism.
    /// </summary>
    public static class HeightStructuresFailureMechanismAssemblyFactory
    {
        /// <summary>
        /// Assembles the simple assessment results.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// simple assembly results for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleSimpleAssessment(
            HeightStructuresFailureMechanismSectionResult failureMechanismSectionResult)
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
                return calculator.AssembleSimpleAssessment(failureMechanismSectionResult.SimpleAssessmentResult);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the detailed assessment result.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// assemble the detailed assembly for.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleDetailedAssessment(
            HeightStructuresFailureMechanismSectionResult failureMechanismSectionResult,
            HeightStructuresFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleDetailedAssessment(
                    failureMechanismSectionResult.DetailedAssessmentResult,
                    failureMechanismSectionResult.GetDetailedAssessmentProbability(failureMechanism, assessmentSection),
                    CreateAssemblyCategoriesInput(failureMechanism, assessmentSection));
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the tailor made assessment result.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// assemble the tailor made assembly for.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleTailorMadeAssessment(
            HeightStructuresFailureMechanismSectionResult failureMechanismSectionResult,
            HeightStructuresFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleTailorMadeAssessment(
                    failureMechanismSectionResult.TailorMadeAssessmentResult,
                    failureMechanismSectionResult.TailorMadeAssessmentProbability,
                    CreateAssemblyCategoriesInput(failureMechanism, assessmentSection));
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the combined assembly.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// combine the assemblies for.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleCombinedAssessment(
            HeightStructuresFailureMechanismSectionResult failureMechanismSectionResult,
            HeightStructuresFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            FailureMechanismSectionAssembly simpleAssembly = AssembleSimpleAssessment(failureMechanismSectionResult);
            FailureMechanismSectionAssembly detailedAssembly = AssembleDetailedAssessment(
                failureMechanismSectionResult, failureMechanism, assessmentSection);
            FailureMechanismSectionAssembly tailorMadeAssembly = AssembleTailorMadeAssessment(
                failureMechanismSectionResult, failureMechanism, assessmentSection);

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
        /// <param name="failureMechanism">The failure mechanism to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the failure mechanism belongs to.</param>
        /// <returns>A <see cref="FailureMechanismAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismAssembly AssembleFailureMechanism(
            HeightStructuresFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!failureMechanism.IsRelevant)
            {
                return FailureMechanismAssemblyFactory.CreateNotApplicableAssembly();
            }

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput(failureMechanism, assessmentSection);
            IEnumerable<FailureMechanismSectionAssembly> sectionAssemblies = AssembleSections(failureMechanism,
                                                                                              assessmentSection,
                                                                                              assemblyCategoriesInput);

            try
            {
                IFailureMechanismAssemblyCalculator calculator =
                    calculatorFactory.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactory.Instance);

                return calculator.Assemble(sectionAssemblies, assemblyCategoriesInput);
            }
            catch (FailureMechanismAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the combined assembly for all sections in the <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the failure mechanism belongs to.</param>
        /// <param name="assemblyCategoriesInput">The input parameters used to determine the assembly categories.</param>
        /// <exception cref="AssemblyException">Thrown when a <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        /// <returns>A collection of all section assembly results.</returns>
        private static IEnumerable<FailureMechanismSectionAssembly> AssembleSections(HeightStructuresFailureMechanism failureMechanism,
                                                                                     IAssessmentSection assessmentSection,
                                                                                     AssemblyCategoriesInput assemblyCategoriesInput)
        {
            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator sectionCalculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            var sectionAssemblies = new List<FailureMechanismSectionAssembly>();

            foreach (HeightStructuresFailureMechanismSectionResult sectionResult in failureMechanism.SectionResults)
            {
                FailureMechanismSectionAssembly sectionAssembly;
                if (sectionResult.UseManualAssemblyProbability)
                {
                    try
                    {
                        sectionAssembly = sectionCalculator.AssembleManual(sectionResult.ManualAssemblyProbability, assemblyCategoriesInput);
                    }
                    catch (FailureMechanismSectionAssemblyCalculatorException e)
                    {
                        throw new AssemblyException(e.Message, e);
                    }
                }
                else
                {
                    sectionAssembly = AssembleCombinedAssessment(sectionResult,
                                                                 failureMechanism,
                                                                 assessmentSection);
                }

                sectionAssemblies.Add(sectionAssembly);
            }

            return sectionAssemblies;
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput(HeightStructuresFailureMechanism failureMechanism,
                                                                             IAssessmentSection assessmentSection)
        {
            return AssemblyCategoriesInputFactory.CreateAssemblyCategoriesInput(failureMechanism.GeneralInput.N,
                                                                                failureMechanism,
                                                                                assessmentSection);
        }
    }
}