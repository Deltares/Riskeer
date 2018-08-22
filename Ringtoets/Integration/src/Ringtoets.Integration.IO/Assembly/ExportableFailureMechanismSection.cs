using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class which holds all the information to export a failure mechanism section.
    /// </summary>
    public class ExportableFailureMechanismSection
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismSection"/>.
        /// </summary>
        /// <param name="geometry">The geometry of the failure mechanism section.</param>
        /// <param name="startDistance">The start distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <param name="endDistance">The end distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is <c>null</c>.</exception>
        public ExportableFailureMechanismSection(IEnumerable<Point2D> geometry, double startDistance, double endDistance)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            Geometry = geometry;
            StartDistance = startDistance;
            EndDistance = endDistance;
        }

        /// <summary>
        /// Gets the geometry of this failure mechanism section.
        /// </summary>
        public IEnumerable<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the start distance between this failure mechanism section and the start of the reference line.
        /// [m]
        /// </summary>
        public double StartDistance { get; }

        /// <summary>
        /// Gets the end distance between this failure mechanism section and the reference line.
        /// [m]
        /// </summary>
        public double EndDistance { get; }
    }
}