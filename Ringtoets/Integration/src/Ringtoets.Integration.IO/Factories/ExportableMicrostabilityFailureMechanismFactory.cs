using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
    /// with assembly results for microstability.
    /// </summary>
    public static class ExportableMicrostabilityFailureMechanismFactory
    {
        private const ExportableFailureMechanismType failureMechanismCode = ExportableFailureMechanismType.STMI;
        private const ExportableFailureMechanismGroup failureMechanismGroup = ExportableFailureMechanismGroup.Group4;
        private const ExportableAssemblyMethod failureMechanismAssemblyMethod = ExportableAssemblyMethod.WBI1A1;

        /// <summary>
        /// Creates a <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="MicrostabilityFailureMechanism"/> to create a
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> for.</param>
        /// <returns>A <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> CreateExportableFailureMechanism(
            MicrostabilityFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (!failureMechanism.IsRelevant)
            {
                return ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithoutProbability(failureMechanismCode,
                                                                                                                   failureMechanismGroup,
                                                                                                                   failureMechanismAssemblyMethod);
            }

            FailureMechanismAssemblyCategoryGroup failureMechanismAssembly = MicrostabilityFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism);


            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSections(failureMechanism.Sections);
            Dictionary<MicrostabilityFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                failureMechanism.SectionResults
                                .Zip(exportableFailureMechanismSections, (sectionResult, exportableSection) => new
                                {
                                    key = sectionResult,
                                    value = exportableSection
                                })
                                .ToDictionary(pair => pair.key, pair => pair.value);

            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                new ExportableFailureMechanismAssemblyResult(failureMechanismAssemblyMethod,
                                                             failureMechanismAssembly),
                failureMechanismSectionsLookup.Values, 
                CreateFailureMechanismSectionResults(failureMechanismSectionsLookup),
                failureMechanismCode,
                failureMechanismGroup);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/>
        /// with assembly results based on the sections in <paramref name="failureMechanismSections"/>.
        /// </summary>
        /// <param name="failureMechanismSections">The mapping between the <see cref="MicrostabilityFailureMechanismSectionResult"/>
        /// and <see cref="ExportableFailureMechanismSection"/>.</param>
        /// <returns>A collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResult> CreateFailureMechanismSectionResults(
            Dictionary<MicrostabilityFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSections)
        {
            var exportableResults = new List<ExportableAggregatedFailureMechanismSectionAssemblyResult>();
            foreach (KeyValuePair<MicrostabilityFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSections)
            {
                MicrostabilityFailureMechanismSectionResult failureMechanismSectionResult = failureMechanismSectionPair.Key;
                FailureMechanismSectionAssemblyCategoryGroup simpleAssembly =
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleSimpleAssessment(failureMechanismSectionResult);

                FailureMechanismSectionAssemblyCategoryGroup detailedAssembly =
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleDetailedAssessment(failureMechanismSectionResult);
                FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly =
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(failureMechanismSectionResult);
                FailureMechanismSectionAssemblyCategoryGroup combinedAssembly =
                    MicrostabilityFailureMechanismAssemblyFactory.AssembleCombinedAssessment(failureMechanismSectionResult);

                exportableResults.Add(
                    new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                        failureMechanismSectionPair.Value,
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(simpleAssembly, ExportableAssemblyMethod.WBI0E1),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(detailedAssembly, ExportableAssemblyMethod.WBI0G1),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(tailorMadeAssembly, ExportableAssemblyMethod.WBI0T1),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(combinedAssembly, ExportableAssemblyMethod.WBI0A1)));
            }

            return exportableResults;
        }
    }
}