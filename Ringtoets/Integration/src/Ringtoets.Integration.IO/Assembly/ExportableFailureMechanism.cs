using System;
using System.Collections.Generic;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export the assembly results of a failure mechanism.
    /// </summary>
    /// <typeparam name="TFailureMechanismAssemblyResult">The type of <see cref="ExportableFailureMechanismAssemblyResult"/>.</typeparam>
    /// <typeparam name="TSectionAssemblyResult">The type of <see cref="ExportableSectionAssemblyResult"/>.</typeparam>
    public class ExportableFailureMechanism<TFailureMechanismAssemblyResult, TSectionAssemblyResult>
        where TFailureMechanismAssemblyResult : ExportableFailureMechanismAssemblyResult
        where TSectionAssemblyResult : ExportableSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult, TSectionAssemblyResult}"/>
        /// </summary>
        /// <param name="failureMechanismAssembly">The assembly result of the failure mechanism.</param>
        /// <param name="sections">The failure mechanism sections belonging to this failure mechanism.</param>
        /// <param name="sectionAssemblyResults">The assembly results for the failure mechanism sections.</param>
        /// <param name="code">The code of the failure mechanism.</param>
        /// <param name="group">The group of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismAssembly"/>,
        /// <paramref name="sections"/> or <paramref name="sectionAssemblyResults"/> is <c>null</c>.</exception>
        public ExportableFailureMechanism(TFailureMechanismAssemblyResult failureMechanismAssembly,
                                          IEnumerable<FailureMechanismSection> sections,
                                          IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase<TSectionAssemblyResult>> sectionAssemblyResults,
                                          ExportableFailureMechanismType code,
                                          ExportableFailureMechanismGroup group)
        {
            if (failureMechanismAssembly == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismAssembly));
            }

            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            if (sectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(sectionAssemblyResults));
            }

            FailureMechanismAssembly = failureMechanismAssembly;
            Sections = sections;
            SectionAssemblyResults = sectionAssemblyResults;
            Code = code;
            Group = group;
        }

        /// <summary>
        /// Gets the assembly result of the failure mechanism.
        /// </summary>
        public TFailureMechanismAssemblyResult FailureMechanismAssembly { get; }

        /// <summary>
        /// Gets the collection of sections.
        /// </summary>
        public IEnumerable<FailureMechanismSection> Sections { get; }

        /// <summary>
        /// Gets the collection of assembly results.
        /// </summary>
        public IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase<TSectionAssemblyResult>> SectionAssemblyResults { get; }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public ExportableFailureMechanismType Code { get; }

        /// <summary>
        /// Gets the group of the failure mechanism.
        /// </summary>
        public ExportableFailureMechanismGroup Group { get; }
    }
}