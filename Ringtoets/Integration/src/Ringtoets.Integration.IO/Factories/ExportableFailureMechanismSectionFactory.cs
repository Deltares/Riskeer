using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
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
        /// Creates a collection of <see cref="ExportableFailureMechanismSection"/> based on <paramref name="failureMechanismSections"/>.
        /// </summary>
        /// <param name="failureMechanismSections">The collection of <see cref="FailureMechanismSection"/>
        /// to create a collection of <see cref="ExportableFailureMechanismSection"/> for.</param>
        /// <returns>A collection of <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSections"/> is <c>null</c>.</exception>
        public static IEnumerable<ExportableFailureMechanismSection> CreateExportableFailureMechanismSections(IEnumerable<FailureMechanismSection> failureMechanismSections)
        {
            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            var exportableFailureMechanismSections = new List<ExportableFailureMechanismSection>();

            double startDistance = 0;
            foreach (FailureMechanismSection section in failureMechanismSections)
            {
                double endDistance = startDistance + Math2D.Length(section.Points);

                exportableFailureMechanismSections.Add(new ExportableFailureMechanismSection(section.Points,
                                                                                             startDistance,
                                                                                             endDistance));
                startDistance = endDistance;
            }

            return exportableFailureMechanismSections;
        }
    }
}