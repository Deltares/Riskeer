using System;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Base implementation to hold all information for exporting assembly results of a failure mechanism section.
    /// </summary>
    public abstract class ExportableAggregatedFailureMechanismSectionAssemblyResultBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResultBase"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The failure mechanism section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSection"/> is <c>null.</c></exception>
        protected ExportableAggregatedFailureMechanismSectionAssemblyResultBase(ExportableFailureMechanismSection failureMechanismSection)
        {
            if (failureMechanismSection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSection));
            }

            FailureMechanismSection = failureMechanismSection;
        }

        /// <summary>
        /// Gets the failure mechanism section.
        /// </summary>
        public ExportableFailureMechanismSection FailureMechanismSection { get; }
    }
}