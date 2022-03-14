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
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
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
        /// Assembles the assessment section.
        /// </summary>
        /// <param name="assessmentSection">The assessment section which contains the failure mechanisms to assemble for.</param>
        /// <returns>A <see cref="AssessmentSectionAssemblyResult"/>.</returns>
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

                IEnumerable<double> assemblyResults = GetFailureMechanismAssemblyResults(assessmentSection).ToArray();
                FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
                return calculator.AssembleAssessmentSection(assemblyResults, contribution.LowerLimitNorm, contribution.SignalingNorm);
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
        /// Assembles the combined failure mechanism sections.
        /// </summary>
        /// <param name="assessmentSection">The assessment section that contains all
        /// the failure mechanism sections to assemble.</param>
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when a <see cref="CombinedFailureMechanismSectionAssemblyResult"/>
        /// cannot be created.</exception>
        public static IEnumerable<CombinedFailureMechanismSectionAssemblyResult> AssembleCombinedPerFailureMechanismSection(
            AssessmentSection assessmentSection)
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
            if (assessmentSection.Piping.InAssembly)
            {
                yield return PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.Piping, assessmentSection);
            }

            if (assessmentSection.MacroStabilityInwards.InAssembly)
            {
                yield return MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.MacroStabilityInwards, assessmentSection);
            }

            if (assessmentSection.GrassCoverErosionInwards.InAssembly)
            {
                yield return GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverErosionInwards, assessmentSection);
            }

            if (assessmentSection.ClosingStructures.InAssembly)
            {
                yield return ClosingStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.ClosingStructures, assessmentSection);
            }

            if (assessmentSection.HeightStructures.InAssembly)
            {
                yield return HeightStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.HeightStructures, assessmentSection);
            }

            if (assessmentSection.StabilityPointStructures.InAssembly)
            {
                yield return StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.StabilityPointStructures, assessmentSection);
            }

            if (assessmentSection.GrassCoverErosionOutwards.InAssembly)
            {
                yield return GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverErosionOutwards, assessmentSection);
            }

            if (assessmentSection.StabilityStoneCover.InAssembly)
            {
                yield return StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.StabilityStoneCover, assessmentSection);
            }

            if (assessmentSection.WaveImpactAsphaltCover.InAssembly)
            {
                yield return WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.WaveImpactAsphaltCover, assessmentSection);
            }

            if (assessmentSection.DuneErosion.InAssembly)
            {
                yield return DuneErosionFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.DuneErosion, assessmentSection);
            }

            if (assessmentSection.PipingStructure.InAssembly)
            {
                yield return PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.PipingStructure, assessmentSection);
            }

            if (assessmentSection.GrassCoverSlipOffInwards.InAssembly)
            {
                yield return FailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverSlipOffInwards, assessmentSection);
            }

            if (assessmentSection.GrassCoverSlipOffOutwards.InAssembly)
            {
                yield return FailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards, assessmentSection);
            }

            if (assessmentSection.Microstability.InAssembly)
            {
                yield return FailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.Microstability, assessmentSection);
            }

            if (assessmentSection.WaterPressureAsphaltCover.InAssembly)
            {
                yield return FailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.WaterPressureAsphaltCover, assessmentSection);
            }

            foreach (SpecificFailurePath failurePath in assessmentSection.SpecificFailurePaths.Where(fp => fp.InAssembly))
            {
                yield return FailureMechanismAssemblyFactory.AssembleFailureMechanism(failurePath, assessmentSection);
            }
        }
    }
}