using System;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export a failure mechanism section assembly result.
    /// </summary>
    public class ExportableAggregatedFailureMechanismSectionAssemblyResult :
        ExportableAggregatedFailureMechanismSectionAssemblyResultBase<ExportableSectionAssemblyResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The failure mechanism section.</param>
        /// <param name="simpleAssembly">The simple assembly result of the failure mechanism section.</param>
        /// <param name="detailedAssembly">The detailed assembly result of the failure mechanism section.</param>
        /// <param name="tailorMadeAssembly">The tailor made assembly result of the failure mechanism section.</param>
        /// <param name="combinedAssembly">The combined assembly result of the failure mechanism section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null.</c></exception>
        public ExportableAggregatedFailureMechanismSectionAssemblyResult(ExportableFailureMechanismSection failureMechanismSection,
                                                                         ExportableSectionAssemblyResult simpleAssembly,
                                                                         ExportableSectionAssemblyResult detailedAssembly,
                                                                         ExportableSectionAssemblyResult tailorMadeAssembly,
                                                                         ExportableSectionAssemblyResult combinedAssembly)
            : base(failureMechanismSection, simpleAssembly, tailorMadeAssembly, combinedAssembly)
        {
            if (detailedAssembly == null)
            {
                throw new ArgumentNullException(nameof(detailedAssembly));
            }

            DetailedAssembly = detailedAssembly;
        }

        /// <summary>
        /// Gets the detailed assembly result.
        /// </summary>
        public ExportableSectionAssemblyResult DetailedAssembly { get; }
    }
}