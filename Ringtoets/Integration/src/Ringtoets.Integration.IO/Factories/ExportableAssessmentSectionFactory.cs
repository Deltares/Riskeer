using System;
using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Assembly;

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
        public static ExportableAssessmentSection CreateExportableAssessmentSection(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new ExportableAssessmentSection(assessmentSection.Name,
                                                   assessmentSection.ReferenceLine.Points,
                                                   new ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod.WBI0A1, AssessmentSectionAssemblyCategoryGroup.A),
                                                   Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>>(),
                                                   Enumerable.Empty<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>>(),
                                                   new ExportableCombinedSectionAssemblyCollection(Enumerable.Empty<ExportableCombinedFailureMechanismSection>(),
                                                                                                   Enumerable.Empty<ExportableCombinedSectionAssembly>()));
        }
    }
}