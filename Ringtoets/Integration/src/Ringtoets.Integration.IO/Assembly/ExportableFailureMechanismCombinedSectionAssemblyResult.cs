using System;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export a combined section assembly result of a failure mechanism.
    /// </summary>
    public class ExportableFailureMechanismCombinedSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismCombinedSectionAssemblyResult"/>
        /// </summary>
        /// <param name="sectionAssemblyResult">The assembly result of the combined section.</param>
        /// <param name="code">The code of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionAssemblyResult"/> is <c>null</c>.</exception>
        public ExportableFailureMechanismCombinedSectionAssemblyResult(ExportableSectionAssemblyResult sectionAssemblyResult,
                                                                       ExportableFailureMechanismType code)
        {
            if (sectionAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(sectionAssemblyResult));
            }

            SectionAssemblyResult = sectionAssemblyResult;
            Code = code;
        }

        /// <summary>
        /// Gets the assembly result of this combined section.
        /// </summary>
        public ExportableSectionAssemblyResult SectionAssemblyResult { get; }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public ExportableFailureMechanismType Code { get; }
    }
}