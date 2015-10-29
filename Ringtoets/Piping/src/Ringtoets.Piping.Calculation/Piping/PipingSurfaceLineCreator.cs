using System.Collections.Generic;
using System.Linq;

using Deltares.WTIPiping;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Piping
{
    /// <summary>
    /// Creates <see cref="Deltares.WTIPiping.PipingSurfaceLine"/> instances which are required by the <see cref="PipingCalculation"/>.
    /// </summary>
    internal static class PipingSurfaceLineCreator
    {
        /// <summary>
        /// Creates a <see cref="Deltares.WTIPiping.PipingSurfaceLine"/> for the kernel
        /// given different surface line.
        /// </summary>
        /// <param name="line">The surface line configured in the Ringtoets application.</param>
        /// <returns>The surface line to be consumed by the kernel.</returns>
        public static PipingSurfaceLine Create(RingtoetsPipingSurfaceLine line)
        {
            var surfaceLine = new PipingSurfaceLine
            {
                Name = line.Name
            };
            if (line.Points.Any())
            {
                surfaceLine.Points.AddRange(CreatePoints(line));
            }

            return surfaceLine;
        }

        private static IEnumerable<PipingPoint> CreatePoints(RingtoetsPipingSurfaceLine line)
        {
            return line.ProjectGeometryToLZ().Select(p => new PipingPoint(p.X, 0.0, p.Y));
        }
    }
}