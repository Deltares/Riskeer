﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
    /// with assembly results for stability stone cover.
    /// </summary>
    public static class ExportableStabilityStoneCoverFailureMechanismFactory
    {
        private const ExportableFailureMechanismType failureMechanismCode = ExportableFailureMechanismType.ZST;
        private const ExportableFailureMechanismGroup failureMechanismGroup = ExportableFailureMechanismGroup.Group3;
        private const ExportableAssemblyMethod failureMechanismAssemblyMethod = ExportableAssemblyMethod.WBI1A1;

        /// <summary>
        /// Creates a <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with assmebly results based on the input parameters.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="StabilityStoneCoverFailureMechanism"/> to create a
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> for.</param>
        /// <returns>A <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> CreateExportableStabilityStoneCoverFailureMechanism(
            StabilityStoneCoverFailureMechanism failureMechanism)
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

            FailureMechanismAssemblyCategoryGroup failureMechanismAssembly = StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism);

            Dictionary<StabilityStoneCoverFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                failureMechanism.SectionResults
                                .ToDictionary(sectionResult => sectionResult,
                                              sectionResult => ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(sectionResult.Section));

            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                new ExportableFailureMechanismAssemblyResult(failureMechanismAssemblyMethod,
                                                             failureMechanismAssembly),
                failureMechanismSectionsLookup.Values, CreateFailureMechanismSectionResults(failureMechanismSectionsLookup),
                failureMechanismCode,
                failureMechanismGroup);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>
        /// with assembly results based on the sections in <paramref name="failureMechanismSections"/>.
        /// </summary>
        /// <param name="failureMechanismSections">The mapping between the <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>
        /// and <see cref="ExportableFailureMechanismSection"/>.</param>
        /// <returns>A collection of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResult> CreateFailureMechanismSectionResults(
            Dictionary<StabilityStoneCoverFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSections)
        {
            var exportableResults = new List<ExportableAggregatedFailureMechanismSectionAssemblyResult>();
            foreach (KeyValuePair<StabilityStoneCoverFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSections)
            {
                StabilityStoneCoverFailureMechanismSectionResult failureMechanismSectionResult = failureMechanismSectionPair.Key;
                FailureMechanismSectionAssemblyCategoryGroup simpleAssembly =
                    StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSimpleAssessment(failureMechanismSectionResult);

                FailureMechanismSectionAssemblyCategoryGroup detailedAssembly =
                    StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleDetailedAssessment(failureMechanismSectionResult);
                FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly =
                    StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(failureMechanismSectionResult);
                FailureMechanismSectionAssemblyCategoryGroup combinedAssembly =
                    StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleCombinedAssessment(failureMechanismSectionResult);

                exportableResults.Add(
                    new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                        failureMechanismSectionPair.Value,
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(simpleAssembly, ExportableAssemblyMethod.WBI0E3),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(detailedAssembly, ExportableAssemblyMethod.WBI0G6),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(tailorMadeAssembly, ExportableAssemblyMethod.WBI0T4),
                        ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(combinedAssembly, ExportableAssemblyMethod.WBI0A1)));
            }

            return exportableResults;
        }
    }
}