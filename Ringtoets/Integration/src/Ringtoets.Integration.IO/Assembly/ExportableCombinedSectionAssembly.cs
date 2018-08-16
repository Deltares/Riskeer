﻿using System;
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
        /// <param name="combinedAssemblyResult">The combined assembly result of this section.</param>
        /// <param name="failureMechanismResults">The assembly results per failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ExportableCombinedSectionAssembly(ExportableCombinedFailureMechanismSection section,
                                                 ExportableFailureMechanismAssemblyResult combinedAssemblyResult,
                                                 IEnumerable<ExportableCombinedSectionAssemblyResult> failureMechanismResults)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (combinedAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(combinedAssemblyResult));
            }

            if (failureMechanismResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismResults));
            }

            Section = section;
            CombinedAssemblyResult = combinedAssemblyResult;
            FailureMechanismResults = failureMechanismResults;
        }

        /// <summary>
        /// Gets the section of the assembly.
        /// </summary>
        public ExportableCombinedFailureMechanismSection Section { get; }

        /// <summary>
        /// Gets the assembly result of this section.
        /// </summary>
        public ExportableFailureMechanismAssemblyResult CombinedAssemblyResult { get; }

        /// <summary>
        /// Gets the assembly results per failure mechanism.
        /// </summary>
        public IEnumerable<ExportableCombinedSectionAssemblyResult> FailureMechanismResults { get; }
    }
}