using System;
using System.Collections.Generic;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export an combined section assembly result.
    /// </summary>
    public class ExportableCombinedSectionAssembly
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        /// <param name="section">The section that belongs to the assembly result.</param>
        /// <param name="combinedSectionAssemblyResult">The combined assembly result of this section.</param>
        /// <param name="failureMechanismResults">The assembly results per failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ExportableCombinedSectionAssembly(ExportableCombinedFailureMechanismSection section,
                                                 ExportableSectionAssemblyResult combinedSectionAssemblyResult,
                                                 IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (combinedSectionAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssemblyResult));
            }

            if (failureMechanismResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismResults));
            }

            Section = section;
            CombinedSectionAssemblyResult = combinedSectionAssemblyResult;
            FailureMechanismResults = failureMechanismResults;
        }

        /// <summary>
        /// Gets the section of the assembly.
        /// </summary>
        public ExportableCombinedFailureMechanismSection Section { get; }

        /// <summary>
        /// Gets the assembly result of this section.
        /// </summary>
        public ExportableSectionAssemblyResult CombinedSectionAssemblyResult { get; }

        /// <summary>
        /// Gets the assembly results per failure mechanism.
        /// </summary>
        public IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> FailureMechanismResults { get; }
    }
}