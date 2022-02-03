﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Primitives;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Integration.Data.StandAlone.AssemblyFactories
{
    /// <summary>
    /// Factory for creating assembly results for a grass cover slip off inwards failure mechanism.
    /// </summary>
    public static class GrassCoverSlipOffInwardsFailureMechanismAssemblyFactory
    {
        /// <summary>
        /// Assembles the simple assessment results.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// simple assembly results for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup AssembleSimpleAssessment(
            GrassCoverSlipOffInwardsFailureMechanismSectionResultOld failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

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
        /// Assembles the detailed assessment result.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// assemble the detailed assembly for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(
            GrassCoverSlipOffInwardsFailureMechanismSectionResultOld failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

            try
            {
                return calculator.AssembleDetailedAssessment(
                    failureMechanismSectionResult.DetailedAssessmentResult);
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
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(
            GrassCoverSlipOffInwardsFailureMechanismSectionResultOld failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

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
        /// Assembles the combined assembly.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// combine the assemblies for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup AssembleCombinedAssessment(
            GrassCoverSlipOffInwardsFailureMechanismSectionResultOld failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

            try
            {
                FailureMechanismSectionAssemblyCategoryGroup simpleAssembly = AssembleSimpleAssessment(failureMechanismSectionResult);

                if (failureMechanismSectionResult.SimpleAssessmentResult == SimpleAssessmentResultType.NotApplicable
                    || failureMechanismSectionResult.SimpleAssessmentResult == SimpleAssessmentResultType.ProbabilityNegligible)
                {
                    return calculator.AssembleCombined(simpleAssembly);
                }

                return calculator.AssembleCombined(
                    simpleAssembly,
                    AssembleDetailedAssessment(failureMechanismSectionResult),
                    AssembleTailorMadeAssessment(failureMechanismSectionResult));
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the assembly category group of the given <paramref name="failureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to get the assembly category group for.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup GetSectionAssemblyCategoryGroup(
            GrassCoverSlipOffInwardsFailureMechanismSectionResultOld failureMechanismSectionResult,
            bool useManual)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            try
            {
                return failureMechanismSectionResult.UseManualAssembly && useManual
                           ? ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(failureMechanismSectionResult.ManualAssemblyCategoryGroup)
                           : AssembleCombinedAssessment(failureMechanismSectionResult);
            }
            catch (Exception e) when (e is NotSupportedException || e is InvalidEnumArgumentException)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the failure mechanism assembly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assemble for.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismAssemblyCategoryGroup AssembleFailureMechanism(GrassCoverSlipOffInwardsFailureMechanism failureMechanism,
                                                                                     bool useManual)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (!failureMechanism.InAssembly)
            {
                return FailureMechanismAssemblyResultFactory.CreateNotApplicableCategory();
            }

            try
            {
                IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> sectionAssemblies =
                    failureMechanism.SectionResultsOld.Select(result => GetSectionAssemblyCategoryGroup(result, useManual)).ToArray();

                IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
                IFailureMechanismAssemblyCalculatorOld calculator =
                    calculatorFactory.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

                return calculator.Assemble(sectionAssemblies);
            }
            catch (FailureMechanismAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
            catch (AssemblyException e)
            {
                throw new AssemblyException(RiskeerCommonDataResources.FailureMechanismAssemblyFactory_Error_while_assembling_failureMechanism, e);
            }
        }
    }
}