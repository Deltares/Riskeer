using System;

using MathNet.Numerics.LinearAlgebra.Double;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Defines a mathematical point in 2D Euclidean space.
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// Gets or sets the x coordinate.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y coordinate.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Determines the 2D vector defined by the difference of two <see cref="Point2D"/>.
        /// </summary>
        /// <param name="p1">Head of the vector.</param>
        /// <param name="p2">Tail of the vector.</param>
        /// <returns>A 2D vector.</returns>
        public static Vector operator -(Point2D p1, Point2D p2)
        {
            var result = new DenseVector(2);
            result[0] = p1.X - p2.X;
            result[1] = p1.Y - p2.Y;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Point2D)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", X, Y);
        }

        /// <summary>
        /// Compares the <see cref="Point2D"/> with <paramref name="other"/> based on <see cref="X"/> and <see cref="Y"/>.
        /// </summary>
        /// <param name="other">A <see cref="Point2D"/> to compare with.</param>
        /// <returns>True if the coordinates of the <see cref="Point3D"/> matches the coordinate of <paramref name="other"/>. False otherwise.</returns>
        protected bool Equals(Point2D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }
    }
}