using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableAssessmentSection"/>
    /// with assembly results.
    /// </summary>
    public static class ExportableAssessmentSectionFactory
    {
        /// <summary>
        /// Creates an <see cref="ExportableAssessmentSection"/> with assembly results
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create
        /// a <see cref="ExportableAssessmentSection"/> for.</param>
        /// <returns>A <see cref="ExportableAssessmentSection"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created
        /// for <paramref name="assessmentSection"/>.</exception>
        public static ExportableAssessmentSection CreateExportableAssessmentSection(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new ExportableAssessmentSection(assessmentSection.Name,
                                                   assessmentSection.ReferenceLine.Points,
                                                   CreateExportableAssessmentSectionAssemblyResult(assessmentSection),
                                                   CreateExportableFailureMechanismsWithProbability(assessmentSection),
                                                   Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                   new ExportableCombinedSectionAssemblyCollection(Enumerable.Empty<ExportableCombinedFailureMechanismSection>(),
                                                                                                   Enumerable.Empty<ExportableCombinedSectionAssembly>()));
        }

        /// <summary>
        /// Creates a <see cref="ExportableAssessmentSectionAssemblyResult"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a
        /// <see cref="ExportableAssessmentSectionAssemblyResult"/> for.</param>
        /// <returns>A <see cref="ExportableAssessmentSectionAssemblyResult"/> with assembly results.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created
        /// for <paramref name="assessmentSection"/>.</exception>
        private static ExportableAssessmentSectionAssemblyResult CreateExportableAssessmentSectionAssemblyResult(AssessmentSection assessmentSection)
        {
            return new ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod.WBI2C1,
                                                                 AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection));
        }

        private static IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> CreateExportableFailureMechanismsWithProbability(AssessmentSection assessmentSection)
        {
            var exportableFailureMechanisms = new List<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>();
            exportableFailureMechanisms.Add(CreateExportablePipingFailureMechanism(assessmentSection));

            return exportableFailureMechanisms;
        }

        private static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> CreateExportablePipingFailureMechanism(AssessmentSection assessmentSection)
        {
            FailureMechanismAssembly failureMechanismAssembly = PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism(assessmentSection.Piping, assessmentSection);

            Dictionary<PipingFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                assessmentSection.Piping
                                 .SectionResults
                                 .ToDictionary(s => s, sectionResult => CreateExportableFailureMechanismSection(sectionResult.Section));

            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                new ExportableFailureMechanismAssemblyResultWithProbability(ExportableAssemblyMethod.WBI1B1,
                                                                            failureMechanismAssembly.Group,
                                                                            failureMechanismAssembly.Probability),
                failureMechanismSectionsLookup.Values,
                CreateExportablePipingFailureMechanismSectionResults(failureMechanismSectionsLookup, assessmentSection),
                ExportableFailureMechanismType.STPH,
                ExportableFailureMechanismGroup.Group2);
        }

        private static IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability> CreateExportablePipingFailureMechanismSectionResults(
            Dictionary<PipingFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSections,
            AssessmentSection assessmentSection)
        {
            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
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