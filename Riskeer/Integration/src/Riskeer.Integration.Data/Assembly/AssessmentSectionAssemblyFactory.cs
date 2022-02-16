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
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data.Properties;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Data.Assembly
{
    /// <summary>
    /// Factory for assembling the assembly results for an assessment section.
    /// </summary>
    public static class AssessmentSectionAssemblyFactory
    {
        /// <summary>
        /// Assembles the results of the failure mechanisms with probability within the assessment sections.
        /// </summary>
        /// <param name="assessmentSection">The assessment section which contains the failure mechanisms to assemble for.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A <see cref="FailureMechanismAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when <see cref="FailureMechanismAssembly"/> cannot be created.</exception>
        public static FailureMechanismAssembly AssembleFailureMechanismsWithProbability(AssessmentSection assessmentSection,
                                                                                        bool useManual)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            try
            {
                IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
                IAssessmentSectionAssemblyCalculatorOld calculator =
                    calculatorFactory.CreateAssessmentSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                return calculator.AssembleFailureMechanisms(GetFailureMechanismWithProbabilityAssemblyResults(assessmentSection, useManual),
                                                            failureMechanismContribution.SignalingNorm,
                                                            failureMechanismContribution.LowerLimitNorm,
                                                            assessmentSection.FailureProbabilityMarginFactor);
            }
            catch (AssessmentSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
            catch (AssemblyException e)
            {
                throw new AssemblyException(Resources.AssessmentSectionAssemblyFactory_Error_while_assembling_failureMechanims, e);
            }
        }

        /// <summary>
        /// Assembles the results of failure mechanisms without probability within the assessment section.
        /// </summary>
        /// <param name="assessmentSection">The assessment section which contains the failure mechanisms to assemble for.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when <see cref="AssessmentSectionAssemblyCategoryGroup"/> cannot be created.</exception>
        public static FailureMechanismAssemblyCategoryGroup AssembleFailureMechanismsWithoutProbability(AssessmentSection assessmentSection,
                                                                                                        bool useManual)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            try
            {
                IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
                IAssessmentSectionAssemblyCalculatorOld calculator =
                    calculatorFactory.CreateAssessmentSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

                return calculator.AssembleFailureMechanisms(GetFailureMechanismsWithoutProbabilityAssemblyResults(assessmentSection,
                                                                                                                  useManual));
            }
            catch (AssessmentSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
            catch (AssemblyException e)
            {
                throw new AssemblyException(Resources.AssessmentSectionAssemblyFactory_Error_while_assembling_failureMechanims, e);
            }
        }

        /// <summary>
        /// Assembles the assessment section.
        /// </summary>
        /// <param name="assessmentSection">The assessment section which contains the failure mechanisms to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when <see cref="AssessmentSectionAssemblyResult"/> cannot be created.</exception>
        public static AssessmentSectionAssemblyResult AssembleAssessmentSection(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            try
            {
                IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
                IAssessmentSectionAssemblyCalculator calculator =
                    calculatorFactory.CreateAssessmentSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

                IEnumerable<double> failureMechanismAssemblyResult = GetFailureMechanismAssemblyResults(assessmentSection);
                FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
                return calculator.AssembleAssessmentSection(failureMechanismAssemblyResult, contribution.LowerLimitNorm, contribution.SignalingNorm);
            }
            catch (AssessmentSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
            catch (AssemblyException e)
            {
                throw new AssemblyException(Resources.AssessmentSectionAssemblyFactory_Error_while_assembling_failureMechanims, e);
            }
        }
        
        /// <summary>
        /// Assembles the assessment section.
        /// </summary>
        /// <param name="assessmentSection">The assessment section which contains the failure mechanisms to assemble for.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A <see cref="AssessmentSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when <see cref="AssessmentSectionAssemblyCategoryGroup"/> cannot be created.</exception>
        public static AssessmentSectionAssemblyCategoryGroup AssembleAssessmentSection(AssessmentSection assessmentSection,
                                                                                       bool useManual)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            try
            {
                IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
                IAssessmentSectionAssemblyCalculatorOld calculator =
                    calculatorFactory.CreateAssessmentSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

                return calculator.AssembleAssessmentSection(AssembleFailureMechanismsWithoutProbability(assessmentSection, useManual),
                                                            AssembleFailureMechanismsWithProbability(assessmentSection, useManual));
            }
            catch (AssessmentSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the combined failure mechanism sections.
        /// </summary>
        /// <param name="assessmentSection">The assessment section that contains all
        /// the failure mechanism sections to assemble.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when a <see cref="CombinedFailureMechanismSectionAssemblyResult"/>
        /// cannot be created.</exception>
        public static IEnumerable<CombinedFailureMechanismSectionAssemblyResult> AssembleCombinedPerFailureMechanismSection(AssessmentSection assessmentSection,
                                                                                                                            bool useManual)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            try
            {
                IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
                IAssessmentSectionAssemblyCalculator calculator = calculatorFactory.CreateAssessmentSectionAssemblyCalculator(
                    AssemblyToolKernelFactory.Instance);

                Dictionary<IFailureMechanism, int> failureMechanismsToAssemble = assessmentSection.GetFailureMechanisms()
                                                                                                  .Where(fm => fm.InAssembly)
                                                                                                  .Select((fm, i) => new
                                                                                                  {
                                                                                                      FailureMechanism = fm,
                                                                                                      Index = i
                                                                                                  })
                                                                                                  .ToDictionary(x => x.FailureMechanism, x => x.Index);

                IEnumerable<CombinedFailureMechanismSectionAssembly> output = calculator.AssembleCombinedFailureMechanismSections(
                    CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, failureMechanismsToAssemble.Keys),
                    assessmentSection.ReferenceLine.Length);

                return CombinedFailureMechanismSectionAssemblyResultFactory.Create(output, failureMechanismsToAssemble, assessmentSection);
            }
            catch (AssessmentSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
            catch (AssemblyException e)
            {
                throw new AssemblyException(Resources.AssessmentSectionAssemblyFactory_Error_while_assembling_failureMechanims, e);
            }
        }

        /// <summary>
        /// Gets the failure mechanism assembly results based on the input arguments.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to retrieve the failure mechanism assembly results
        /// for.</param>
        /// <returns>A collection of failure mechanism assembly results.</returns>
        /// <exception cref="AssemblyException">Thrown when the results could not be assembled.</exception>
        private static IEnumerable<double> GetFailureMechanismAssemblyResults(AssessmentSection assessmentSection)
        {
            return new[]
            {
                PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.Piping, assessmentSection),
                MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.MacroStabilityInwards, assessmentSection),
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                ClosingStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.ClosingStructures, assessmentSection),
                HeightStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.HeightStructures, assessmentSection),
                StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.StabilityPointStructures, assessmentSection),
                GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverErosionOutwards, assessmentSection),
                StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.StabilityStoneCover, assessmentSection),
                WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.WaveImpactAsphaltCover, assessmentSection),
                DuneErosionFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.DuneErosion, assessmentSection),
                PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.PipingStructure, assessmentSection),
                StandAloneFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverSlipOffInwards, assessmentSection),
                StandAloneFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards, assessmentSection),
                StandAloneFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.Microstability, assessmentSection),
                StandAloneFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.WaterPressureAsphaltCover, assessmentSection)
            };
        }
        
        private static IEnumerable<FailureMechanismAssembly> GetFailureMechanismWithProbabilityAssemblyResults(AssessmentSection assessmentSection,
                                                                                                               bool useManual)
        {
            return new[]
            {
                GrassCoverErosionInwardsFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.GrassCoverErosionInwards, assessmentSection, useManual),
                HeightStructuresFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.HeightStructures, assessmentSection, useManual),
                ClosingStructuresFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.ClosingStructures, assessmentSection, useManual),
                StabilityPointStructuresFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.StabilityPointStructures, assessmentSection, useManual),
                PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.Piping, assessmentSection, useManual),
                MacroStabilityInwardsFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.MacroStabilityInwards, assessmentSection, useManual)
            };
        }

        private static IEnumerable<FailureMechanismAssemblyCategoryGroup> GetFailureMechanismsWithoutProbabilityAssemblyResults(AssessmentSection assessmentSection,
                                                                                                                                bool useManual)
        {
            return new[]
            {
                StabilityStoneCoverFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.StabilityStoneCover, useManual),
                WaveImpactAsphaltCoverFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.WaveImpactAsphaltCover, useManual),
                GrassCoverErosionOutwardsFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.GrassCoverErosionOutwards, useManual),
                DuneErosionFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.DuneErosion, useManual),
                MicrostabilityFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.Microstability, useManual),
                WaterPressureAsphaltCoverFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.WaterPressureAsphaltCover, useManual),
                GrassCoverSlipOffOutwardsFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards, useManual),
                GrassCoverSlipOffInwardsFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.GrassCoverSlipOffInwards, useManual),
                PipingStructureFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(assessmentSection.PipingStructure, useManual)
            };
        }
    }
}