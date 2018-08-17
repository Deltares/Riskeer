using System;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instance of <see cref="ExportableAssessmentSection"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ExportableFailureMechanismSection"/> based on
        /// <paramref name="failureMechanismSection"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The <see cref="FailureMechanismSection"/>
        /// to create a <see cref="ExportableFailureMechanismSection"/> for.</param>
        /// <returns>A <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSection"/>
        /// is <c>null</c>.</exception>
        public static ExportableFailureMechanismSection CreateExportableFailureMechanismSection(FailureMechanismSection failureMechanismSection)
        {
            if (failureMechanismSection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSection));
            }

            return new ExportableFailureMechanismSection(failureMechanismSection.Points, double.NaN, double.NaN);
        }
    }
}