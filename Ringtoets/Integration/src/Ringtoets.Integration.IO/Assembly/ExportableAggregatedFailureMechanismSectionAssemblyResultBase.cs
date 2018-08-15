using System;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Base implementation to hold all information for exporting assembly results for a failure mechanism section.
    /// </summary>
    /// <typeparam name="TSectionAssemblyResult">The type of <see cref="ExportableSectionAssemblyResult"/> the results hold.</typeparam>
    public abstract class ExportableAggregatedFailureMechanismSectionAssemblyResultBase<TSectionAssemblyResult>
        where TSectionAssemblyResult : ExportableSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultBase{T}"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The failure mechanism section.</param>
        /// <param name="simpleAssembly">The simple assembly result of the failure mechanism section.</param>
        /// <param name="tailorMadeAssembly">The tailor made assembly result of the failure mechanism section.</param>
        /// <param name="combinedAssembly">The combined assembly result of the failure mechanism section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null.</c></exception>
        protected ExportableAggregatedFailureMechanismSectionAssemblyResultBase(FailureMechanismSection failureMechanismSection,
                                                                                TSectionAssemblyResult simpleAssembly,
                                                                                TSectionAssemblyResult tailorMadeAssembly,
                                                                                TSectionAssemblyResult combinedAssembly)
        {
            if (failureMechanismSection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSection));
            }

            if (simpleAssembly == null)
            {
                throw new ArgumentNullException(nameof(simpleAssembly));
            }

            if (tailorMadeAssembly == null)
            {
                throw new ArgumentNullException(nameof(tailorMadeAssembly));
            }

            if (combinedAssembly == null)
            {
                throw new ArgumentNullException(nameof(combinedAssembly));
            }

            FailureMechanismSection = failureMechanismSection;
            SimpleAssembly = simpleAssembly;
            TailorMadeAssembly = tailorMadeAssembly;
            CombinedAssembly = combinedAssembly;
        }

        /// <summary>
        /// Gets the failure mechanism section.
        /// </summary>
        public FailureMechanismSection FailureMechanismSection { get; }

        /// <summary>
        /// Gets the simple assembly result.
        /// </summary>
        public TSectionAssemblyResult SimpleAssembly { get; }

        /// <summary>
        /// Gets the tailor made assembly result.
        /// </summary>
        public TSectionAssemblyResult TailorMadeAssembly { get; }

        /// <summary>
        /// Gets the combined assembly result.
        /// </summary>
        public TSectionAssemblyResult CombinedAssembly { get; }
    }
}