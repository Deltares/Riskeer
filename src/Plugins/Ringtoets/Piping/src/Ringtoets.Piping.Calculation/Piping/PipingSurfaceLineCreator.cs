using System;
using System.Collections.Generic;
using System.Linq;

using Deltares.WTIPiping;

using MathNet.Numerics.LinearAlgebra.Double;

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
            var surfaceLinePoints = line.Points.ToArray();
            var localCoordinatesX = new double[surfaceLinePoints.Length];
            localCoordinatesX[0] = 0.0;

            if (surfaceLinePoints.Length > 1)
            {
                ProjectPointsAfterFirstOntoSpanningLine(line, localCoordinatesX);
            }

            for (int i = 0; i < localCoordinatesX.Length; i++)
            {
                yield return new PipingPoint(localCoordinatesX[i], 0.0, surfaceLinePoints[i].Z);
            }
        }

        /// <summary>
        /// This method defines the 'spanning line' as the 2D vector going from start to end
        /// of the surface line points. Then all except the first point is projected onto
        /// this vector. Then the local coordinates are determined by taking the length of
        /// each vector along the 'spanning line'.
        /// </summary>
        /// <param name="line">The surface line to be projected, which has more than 1 geometry point.</param>
        /// <param name="localCoordinatesX">The array into which the projected X-coordinate 
        /// values should be stored. It's <see cref="Array.Length"/> should be the same as
        /// the collection-size of <paramref name="line"/>'s <see cref="RingtoetsPipingSurfaceLine.Points"/>
        /// collection.</param>
        private static void ProjectPointsAfterFirstOntoSpanningLine(RingtoetsPipingSurfaceLine line, double[] localCoordinatesX)
        {
            // Determine the vectors from the first coordinate to each other coordinate point 
            // in the XY world coordinate plane:
            var worldCoordinates = line.Points.Select(p => new Point2D { X = p.X, Y = p.Y }).ToArray();
            var worldCoordinateVectors = new Vector[worldCoordinates.Length - 1];
            for (int i = 1; i < worldCoordinates.Length; i++)
            {
                worldCoordinateVectors[i - 1] = worldCoordinates[i] - worldCoordinates[0];
            }

            // Determine the 'spanning line' vector:
            var spanningVector = worldCoordinateVectors[worldCoordinateVectors.Length - 1];
            var spanningVectorDotProduct = spanningVector.DotProduct(spanningVector);
            var length = Math.Sqrt(spanningVectorDotProduct);

            // Project each vector onto the 'spanning vector' to determine it's X coordinate in local coordinates:
            for (int i = 0; i < worldCoordinateVectors.Length - 1; i++)
            {
                var projectOnSpanningVectorFactor = (worldCoordinateVectors[i].DotProduct(spanningVector)) /
                                                    (spanningVectorDotProduct);
                localCoordinatesX[i + 1] = projectOnSpanningVectorFactor * length;
            }
            localCoordinatesX[localCoordinatesX.Length - 1] = length;
        }
    }
}