using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export a failure mechanism section
    /// which is the result of an combined section assembly.
    /// </summary>
    public class ExportableCombinedFailureMechanismSection : ExportableFailureMechanismSection
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableCombinedFailureMechanismSection"/>.
        /// </summary>
        /// <param name="geometry">The geometry of the failure mechanism section.</param>
        /// <param name="startDistance">The start distance of the failure mechanism section between the section
        /// and the start of the reference line.</param>
        /// <param name="endDistance">The end distance of the failure mechanism section between the section
        /// and the start of the reference line.</param>
        /// <param name="assemblyMethod">The assembly method which was used to get this section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/>
        /// is <c>null</c>.</exception>
        public ExportableCombinedFailureMechanismSection(IEnumerable<Point2D> geometry,
                                                         double startDistance,
                                                         double endDistance,
                                                         ExportableAssemblyMethod assemblyMethod)
            : base(geometry, startDistance, endDistance)
        {
            AssemblyMethod = assemblyMethod;
        }

        /// <summary>
        /// Gets the method that was used to get this section.
        /// </summary>
        public ExportableAssemblyMethod AssemblyMethod { get; }
    }
}