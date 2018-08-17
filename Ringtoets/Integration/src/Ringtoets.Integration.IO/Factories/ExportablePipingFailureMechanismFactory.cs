﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
    /// with assembly results for piping.
    /// </summary>
    public static class ExportablePipingFailureMechanismFactory
    {
        /// <summary>
        /// Creates a <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with assmebly results based on the input parameters.
        /// </summary>
        /// <param name="pipingFailureMechanism">The piping failure mechanism to create a
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> for.</param>
        /// <param name="assessmentSection">The assessment section this failure mechanism belongs to.</param>
        /// <returns>A <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> CreateExportablePipingFailureMechanism(
            PipingFailureMechanism pipingFailureMechanism,
            IAssessmentSection assessmentSection)
        {
            FailureMechanismAssembly failureMechanismAssembly = PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism(pipingFailureMechanism, assessmentSection);

            Dictionary<PipingFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                pipingFailureMechanism.SectionResults
                                      .ToDictionary(s => s, sectionResult => CreateExportableFailureMechanismSection(sectionResult.Section));

            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                new ExportableFailureMechanismAssemblyResultWithProbability(ExportableAssemblyMethod.WBI1B1,
                                                                            failureMechanismAssembly.Group,
                                                                            failureMechanismAssembly.Probability),
                failureMechanismSectionsLookup.Values, CreateExportablePipingFailureMechanismSectionResults(failureMechanismSectionsLookup,
                                                                                                            pipingFailureMechanism, assessmentSection),
                ExportableFailureMechanismType.STPH,
                ExportableFailureMechanismGroup.Group2);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>
        /// with assembly results based on the sections in <paramref name="failureMechanismSections"/>.
        /// </summary>
        /// <param name="failureMechanismSections">The mapping between the <see cref="PipingFailureMechanismSectionResult"/>
        /// and <see cref="ExportableFailureMechanismSection"/></param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism the sections belong to.</param>
        /// <param name="assessmentSection">The assessment section the sections belong to.</param>
        /// <returns>A collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability> CreateExportablePipingFailureMechanismSectionResults(
            Dictionary<PipingFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSections,
            PipingFailureMechanism pipingFailureMechanism,
            IAssessmentSection assessmentSection)
        {
            IEnumerable<PipingCalculationScenario> pipingCalculationScenarios = pipingFailureMechanism.Calculations.Cast<PipingCalculationScenario>();

            var exportableResults = new List<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>();
            foreach (KeyValuePair<PipingFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSections)
            {
                PipingFailureMechanismSectionResult failureMechanismSectionResult = failureMechanismSectionPair.Key;
                FailureMechanismSectionAssembly simpleAssembly =
                    PipingFailureMechanismAssemblyFactory.AssembleSimpleAssessment(failureMechanismSectionResult);

                FailureMechanismSectionAssembly detailedAssembly =
                    PipingFailureMechanismAssemblyFactory.AssembleDetailedAssessment(failureMechanismSectionResult,
                                                                                     pipingCalculationScenarios,
                                                                                     pipingFailureMechanism,
                                                                                     assessmentSection);
                FailureMechanismSectionAssembly tailorMadeAssembly =
                    PipingFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(failureMechanismSectionResult,
                                                                                       pipingFailureMechanism,
                                                                                       assessmentSection);
                FailureMechanismSectionAssembly combinedAssembly =
                    PipingFailureMechanismAssemblyFactory.AssembleCombinedAssessment(failureMechanismSectionResult,
                                                                                     pipingCalculationScenarios,
                                                                                     pipingFailureMechanism,
                                                                                     assessmentSection);

                exportableResults.Add(
                    new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                        failureMechanismSectionPair.Value,
                        CreateExportableSectionAssemblyResultWithProbability(simpleAssembly, ExportableAssemblyMethod.WBI0E1),
                        CreateExportableSectionAssemblyResultWithProbability(detailedAssembly, ExportableAssemblyMethod.WBI0G5),
                        CreateExportableSectionAssemblyResultWithProbability(tailorMadeAssembly, ExportableAssemblyMethod.WBI0T5),
                        CreateExportableSectionAssemblyResultWithProbability(combinedAssembly, ExportableAssemblyMethod.WBI1B1)));
            }

            return exportableResults;
        }

        private static ExportableSectionAssemblyResultWithProbability CreateExportableSectionAssemblyResultWithProbability(FailureMechanismSectionAssembly assembly,
                                                                                                                           ExportableAssemblyMethod exportableAssemblyMethod)
        {
            return new ExportableSectionAssemblyResultWithProbability(exportableAssemblyMethod, assembly.Group, assembly.Probability);
        }

        private static ExportableFailureMechanismSection CreateExportableFailureMechanismSection(FailureMechanismSection failureMechanismSection)
        {
            return new ExportableFailureMechanismSection(failureMechanismSection.Points, double.NaN, double.NaN);
        }
    }
}