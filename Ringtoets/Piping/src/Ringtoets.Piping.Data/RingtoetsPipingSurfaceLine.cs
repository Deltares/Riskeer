using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra.Double;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Definition of a surfaceline for piping.
    /// </summary>
    public class RingtoetsPipingSurfaceLine
    {
        private Point3D[] geometryPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsPipingSurfaceLine"/> class.
        /// </summary>
        public RingtoetsPipingSurfaceLine()
        {
            Name = string.Empty;
            geometryPoints = new Point3D[0];
        }

        /// <summary>
        /// Gets or sets the name of the surfaceline.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the 3D points describing its geometry.
        /// </summary>
        public IEnumerable<Point3D> Points
        {
            get
            {
                return geometryPoints;
            }
        }

        /// <summary>
        /// Gets or sets the first 3D geometry point defining the surfaceline in world coordinates.
        /// </summary>
        public Point3D StartingWorldPoint { get; private set; }

        /// <summary>
        /// Gets or sets the last 3D geometry point defining the surfaceline in world coordinates.
        /// </summary>
        public Point3D EndingWorldPoint { get; private set; }

        /// <summary>
        /// Sets the geometry of the surfaceline.
        /// </summary>
        /// <param name="points">The collection of points defining the surfaceline geometry.</param>
        public void SetGeometry(IEnumerable<Point3D> points)
        {
            geometryPoints = points.ToArray();

            if (geometryPoints.Length > 0)
            {
                StartingWorldPoint = geometryPoints[0];
                EndingWorldPoint = geometryPoints[geometryPoints.Length - 1];
            }
        }

        /// <summary>
        /// Projects the points in <see cref="Points"/> to localized coordinate (LZ-plane) system.
        /// Z-values are retained, and the first point is put a L=0.
        /// </summary>
        /// <returns>Collection of 2D points in the LZ-plane.</returns>
        public IEnumerable<Point2D> ProjectGeometryToLZ()
        {
            var count = geometryPoints.Length;
            if (count == 0)
            {
                return Enumerable.Empty<Point2D>();
            }

            var localCoordinatesX = new double[count];
            localCoordinatesX[0] = 0.0;
            if (count > 1)
            {
                ProjectPointsAfterFirstOntoSpanningLine(localCoordinatesX);
            }

            var result = new Point2D[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = new Point2D
                {
                    X = localCoordinatesX[i], Y = geometryPoints[i].Z
                };
            }
            return result;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// This method defines the 'spanning line' as the 2D vector going from start to end
        /// of the surface line points. Then all except the first point is projected onto
        /// this vector. Then the local coordinates are determined by taking the length of
        /// each vector along the 'spanning line'.
        /// </summary>
        /// <param name="localCoordinatesX">The array into which the projected X-coordinate 
        /// values should be stored. Its <see cref="Array.Length"/> should be the same as
        /// the collection-size of <see cref="geometryPoints"/>.</param>
        private void ProjectPointsAfterFirstOntoSpanningLine(double[] localCoordinatesX)
        {
            // Determine the vectors from the first coordinate to each other coordinate point 
            // in the XY world coordinate plane:
            Point2D[] worldCoordinates = Points.Select(p => new Point2D
            {
                X = p.X, Y = p.Y
            }).ToArray();
            var worldCoordinateVectors = new Vector[worldCoordinates.Length - 1];
            for (int i = 1; i < worldCoordinates.Length; i++)
            {
                worldCoordinateVectors[i - 1] = worldCoordinates[i] - worldCoordinates[0];
            }

            // Determine the 'spanning line' vector:
            Vector spanningVector = worldCoordinateVectors[worldCoordinateVectors.Length - 1];
            double spanningVectorDotProduct = spanningVector.DotProduct(spanningVector);
            double length = Math.Sqrt(spanningVectorDotProduct);

            // Project each vector onto the 'spanning vector' to determine it's X coordinate in local coordinates:
            for (int i = 0; i < worldCoordinateVectors.Length - 1; i++)
            {
                double projectOnSpanningVectorFactor = (worldCoordinateVectors[i].DotProduct(spanningVector)) /
                                                       (spanningVectorDotProduct);
                localCoordinatesX[i + 1] = projectOnSpanningVectorFactor * length;
            }
            localCoordinatesX[localCoordinatesX.Length - 1] = length;
        }
    }
}