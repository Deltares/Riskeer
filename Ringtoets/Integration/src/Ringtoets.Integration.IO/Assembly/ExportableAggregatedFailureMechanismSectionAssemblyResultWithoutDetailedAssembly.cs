using System;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export an assembly result without a detailed assembly
    /// of a failure mechanism section .
    /// </summary>
    public class ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly :
        ExportableAggregatedFailureMechanismSectionAssemblyResultBase<ExportableSectionAssemblyResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The failure mechanism section.</param>
        /// <param name="simpleAssembly">The simple assembly result of the failure mechanism section.</param>
        /// <param name="tailorMadeAssembly">The tailor made assembly result of the failure mechanism section.</param>
        /// <param name="combinedAssembly">The combined assembly result of the failure mechanism section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null.</c></exception>
        public ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(ExportableFailureMechanismSection failureMechanismSection,
                                                                                                ExportableSectionAssemblyResult simpleAssembly,
                                                                                                ExportableSectionAssemblyResult tailorMadeAssembly,
                                                                                                ExportableSectionAssemblyResult combinedAssembly)
            : base(failureMechanismSection, simpleAssembly, tailorMadeAssembly, combinedAssembly) {}
    }
}