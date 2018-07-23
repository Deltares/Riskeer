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
using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data.Assembly
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
        /// <returns>A <see cref="FailureMechanismAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when <see cref="AssessmentSectionAssembly"/> cannot be created.</exception>
        public static FailureMechanismAssembly AssembleFailureMechanismsWithProbability(AssessmentSection assessmentSection)
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

                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                return calculator.AssembleFailureMechanisms(GetFailureMechanismWithProbabilityAssemblyResults(assessmentSection),
                                                            failureMechanismContribution.SignalingNorm,
                                                            failureMechanismContribution.LowerLimitNorm);
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
        /// <returns>A <see cref="FailureMechanismAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when <see cref="AssessmentSectionAssemblyCategoryGroup"/> cannot be created.</exception>
        public static FailureMechanismAssemblyCategoryGroup AssembleFailureMechanismsWithoutProbability(AssessmentSection assessmentSection)
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

                return calculator.AssembleFailureMechanisms(GetFailureMechanismsWithoutProbabilityAssemblyResults(assessmentSection));
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
        /// <returns>A <see cref="AssessmentSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when <see cref="AssessmentSectionAssemblyCategoryGroup"/> cannot be created.</exception>
        public static AssessmentSectionAssemblyCategoryGroup AssembleAssessmentSection(AssessmentSection assessmentSection)
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

                return calculator.AssembleAssessmentSection(AssembleFailureMechanismsWithoutProbability(assessmentSection),
                                                            AssembleFailureMechanismsWithProbability(assessmentSection));
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
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when a <see cref="CombinedFailureMechanismSectionAssemblyResult"/>
        /// cannot be created.</exception>
        public static IEnumerable<CombinedFailureMechanismSectionAssemblyResult> AssembleCombinedPerFailureMechanismSection(AssessmentSection assessmentSection)
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

                Dictionary<IFailureMechanism, int> relevantFailureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                                .Where(fm => fm.IsRelevant)
                                                                                                .Select((fm, i) => new
                                                                                                {
                                                                                                    FailureMechanism = fm,
                                                                                                    Index = i
                                                                                                })
                                                                                                .ToDictionary(x => x.FailureMechanism, x => x.Index);

                IEnumerable<CombinedFailureMechanismSectionAssembly> output = calculator.AssembleCombinedFailureMechanismSections(
                    CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, relevantFailureMechanisms.Keys),
                    assessmentSection.ReferenceLine.Length);

                return CombinedFailureMechanismSectionAssemblyResultFactory.Create(output, relevantFailureMechanisms, assessmentSection);
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

        private static IEnumerable<FailureMechanismAssembly> GetFailureMechanismWithProbabilityAssemblyResults(AssessmentSection assessmentSection)
        {
            return new[]
            {
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                HeightStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.HeightStructures, assessmentSection),
                ClosingStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.ClosingStructures, assessmentSection),
                StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.StabilityPointStructures, assessmentSection),
                PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.Piping, assessmentSection),
                MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.MacroStabilityInwards, assessmentSection)
            };
        }

        private static IEnumerable<FailureMechanismAssemblyCategoryGroup> GetFailureMechanismsWithoutProbabilityAssemblyResults(AssessmentSection assessmentSection)
        {
            return new[]
            {
                StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.StabilityStoneCover),
                WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.WaveImpactAsphaltCover),
                GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverErosionOutwards),
                DuneErosionFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.DuneErosion),
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.MacroStabilityOutwards, assessmentSection),
                MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.Microstability),
                WaterPressureAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.WaterPressureAsphaltCover),
                GrassCoverSlipOffOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards),
                GrassCoverSlipOffInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverSlipOffInwards),
                PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.PipingStructure),
                StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.StrengthStabilityLengthwiseConstruction),
                TechnicalInnovationFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.TechnicalInnovation)
            };
        }
    }
}