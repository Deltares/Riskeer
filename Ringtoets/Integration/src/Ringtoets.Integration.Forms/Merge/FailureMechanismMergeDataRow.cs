using System;
using System.Linq;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.Forms.Merge
{
    /// <summary>
    /// Row representing the information of a <see cref="IFailureMechanism"/> to be
    /// used for merging.
    /// </summary>
    internal class FailureMechanismMergeDataRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismMergeDataRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> it needs to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismMergeDataRow(IFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the wrapped failure mechanism of the row.
        /// </summary>
        public IFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Indicates whether the failure mechanism is selected to be merged.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets the name of the failure mechanism.
        /// </summary>
        public string Name
        {
            get
            {
                return FailureMechanism.Name;
            }
        }

        /// <summary>
        /// Gets if the failure mechanism is relevant.
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return FailureMechanism.IsRelevant;
            }
        }

        /// <summary>
        /// Gets if the failure mechanism has sections.
        /// </summary>
        public bool HasSections
        {
            get
            {
                return FailureMechanism.Sections.Any();
            }
        }

        /// <summary>
        /// Gets the amount of calculations that are contained by the failure mechanism.
        /// </summary>
        public int NumberOfCalculations
        {
            get
            {
                return FailureMechanism.Calculations.Count();
            }
        }
    }
}