using System;
using System.Collections.Generic;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export a collection
    /// of <see cref="ExportableCombinedSectionAssembly"/>.
    /// </summary>
    public class ExportableCombinedSectionAssemblyCollection
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        /// <param name="sections">The sections belonging to this collection of <see cref="ExportableCombinedSectionAssembly"/>.</param>
        /// <param name="combinedSectionAssemblyResults">The collection of <see cref="ExportableCombinedSectionAssembly"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ExportableCombinedSectionAssemblyCollection(IEnumerable<ExportableCombinedFailureMechanismSection> sections,
                                                  IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            if (combinedSectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssemblyResults));
            }

            Sections = sections;
            CombinedSectionAssemblyResults = combinedSectionAssemblyResults;
        }

        /// <summary>
        /// Gets the sections belonging to this collection of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        public IEnumerable<ExportableCombinedFailureMechanismSection> Sections { get; }

        /// <summary>
        /// Gets the collection of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        public IEnumerable<ExportableCombinedSectionAssembly> CombinedSectionAssemblyResults { get; }
    }
}