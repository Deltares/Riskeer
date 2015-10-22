using System.Linq;

using Deltares.WTIPiping;

using Wti.Data;

namespace Wti.Calculation.Piping
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
            surfaceLine.Points.AddRange(line.Points.Select(p => new PipingPoint(p.X, p.Y, p.Z)));

            return surfaceLine;
        }
    }
}