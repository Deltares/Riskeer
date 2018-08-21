﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
    /// with assembly results for macro stability inwards.
    /// </summary>
    public static class ExportableMacroStabilityInwardsFailureMechanismFactory
    {
        private const ExportableFailureMechanismGroup failureMechanismGroup = ExportableFailureMechanismGroup.Group2;
        private const ExportableFailureMechanismType failureMechanismCode = ExportableFailureMechanismType.STBI;
        private const ExportableAssemblyMethod failureMechanismAssemblyMethod = ExportableAssemblyMethod.WBI1B1;

        /// <summary>
        /// Creates a <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/> to create a
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> for.</param>
        /// <param name="assessmentSection">The assessment section this failure mechanism belongs to.</param>
        /// <returns>A <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> CreateExportableFailureMechanism(
            MacroStabilityInwardsFailureMechanism failureMechanism,
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
                return ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithProbability(failureMechanismCode,
                                                                                                                failureMechanismGroup,
                                                                                                                failureMechanismAssemblyMethod);
            }

            FailureMechanismAssembly failureMechanismAssembly = MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection);

            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSections(failureMechanism.Sections);
            Dictionary<MacroStabilityInwardsFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                failureMechanism.SectionResults
                                .Zip(exportableFailureMechanismSections, (sectionResult, exportableSection) => new
                                {
                                    key = sectionResult,
                                    value = exportableSection
                                })
                                .ToDictionary(pair => pair.key, pair => pair.value);

            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                new ExportableFailureMechanismAssemblyResultWithProbability(failureMechanismAssemblyMethod,
                                                                            failureMechanismAssembly.Group,
                                                                            failureMechanismAssembly.Probability),
                failureMechanismSectionsLookup.Values,
                CreateExportableFailureMechanismSectionResults(failureMechanismSectionsLookup,
                                                               failureMechanism,
                                                               assessmentSection),
                failureMechanismCode,
                failureMechanismGroup);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>
        /// with assembly results based on the sections in <paramref name="failureMechanismSections"/>.
        /// </summary>
        /// <param name="failureMechanismSections">The mapping between the <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/>
        /// and <see cref="ExportableFailureMechanismSection"/>.</param>
        /// <param name="macroStabilityInwardsFailureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/> the sections belong to.</param>
        /// <param name="assessmentSection">The assessment section the sections belong to.</param>
        /// <returns>A collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability> CreateExportableFailureMechanismSectionResults(
            Dictionary<MacroStabilityInwardsFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSections,
            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism,
            IAssessmentSection assessmentSection)
        {
            IEnumerable<MacroStabilityInwardsCalculationScenario> macroStabilityInwardsCalculationScenarios =
                macroStabilityInwardsFailureMechanism.Calculations.Cast<MacroStabilityInwardsCalculationScenario>();

            var exportableResults = new List<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>();
            foreach (KeyValuePair<MacroStabilityInwardsFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSections)
            {
                MacroStabilityInwardsFailureMechanismSectionResult failureMechanismSectionResult = failureMechanismSectionPair.Key;
                FailureMechanismSectionAssembly simpleAssembly =
                    MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(failureMechanismSectionResult);

                FailureMechanismSectionAssembly detailedAssembly =
                    MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(failureMechanismSectionResult,
                                                                                                    macroStabilityInwardsCalculationScenarios,
                                                                                                    macroStabilityInwardsFailureMechanism,
                                                                                                    assessmentSection);
                FailureMechanismSectionAssembly tailorMadeAssembly =
                    MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(failureMechanismSectionResult,
                                                                                                      macroStabilityInwardsFailureMechanism,
                                                                                                      assessmentSection);
                FailureMechanismSectionAssembly combinedAssembly =
                    MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(failureMechanismSectionResult,
                                                                                                    macroStabilityInwardsCalculationScenarios,
                                                                                                    macroStabilityInwardsFailureMechanism,
                                                                                                    assessmentSection);

                exportableResults.Add(
                    new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                        failureMechanismSectionPair.Value,
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(simpleAssembly, ExportableAssemblyMethod.WBI0E1),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(detailedAssembly, ExportableAssemblyMethod.WBI0G5),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(tailorMadeAssembly, ExportableAssemblyMethod.WBI0T5),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(combinedAssembly, ExportableAssemblyMethod.WBI0A1)));
            }

            return exportableResults;
        }
    }
}