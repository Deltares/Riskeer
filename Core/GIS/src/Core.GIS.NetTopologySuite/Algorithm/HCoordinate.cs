using System;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.NetTopologySuite.Algorithm
{
    /// <summary> 
    /// Represents a homogeneous coordinate for 2-D coordinates.
    /// </summary>
    public class HCoordinate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        public HCoordinate(double x, double y, double w)
        {
            X = x;
            Y = y;
            W = w;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public HCoordinate(ICoordinate p)
        {
            X = p.X;
            Y = p.Y;
            W = 1.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public HCoordinate(HCoordinate p1, HCoordinate p2)
        {
            X = p1.Y*p2.W - p2.Y*p1.W;
            Y = p2.X*p1.W - p1.X*p2.W;
            W = p1.X*p2.Y - p2.X*p1.Y;
        }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate Coordinate
        {
            get
            {
                return new Coordinate(GetX(), GetY());
            }
        }

        /// <summary> 
        /// Computes the (approximate) intersection point between two line segments
        /// using homogeneous coordinates.
        /// Note that this algorithm is
        /// not numerically stable; i.e. it can produce intersection points which
        /// lie outside the envelope of the line segments themselves.  In order
        /// to increase the precision of the calculation input points should be normalized
        /// before passing them to this routine.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static ICoordinate Intersection(ICoordinate p1, ICoordinate p2, ICoordinate q1, ICoordinate q2)
        {
            HCoordinate l1 = new HCoordinate(new HCoordinate(p1), new HCoordinate(p2));
            HCoordinate l2 = new HCoordinate(new HCoordinate(q1), new HCoordinate(q2));
            HCoordinate intHCoord = new HCoordinate(l1, l2);
            ICoordinate intPt = intHCoord.Coordinate;
            return intPt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetX()
        {
            double a = X/W;
            if ((Double.IsNaN(a)) || (Double.IsInfinity(a)))
            {
                throw new NotRepresentableException();
            }
            return a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetY()
        {
            double a = Y/W;
            if ((Double.IsNaN(a)) || (Double.IsInfinity(a)))
            {
                throw new NotRepresentableException();
            }
            return a;
        }

        /// <summary>
        /// Direct access to x private field
        /// </summary>
        [Obsolete("This is a simple access to x private field: use GetX() instead.")]
        protected double X { get; set; }

        /// <summary>
        /// Direct access to y private field
        /// </summary>
        [Obsolete("This is a simple access to y private field: use GetY() instead.")]
        protected double Y { get; set; }

        /// <summary>
        /// Direct access to w private field
        /// </summary>
        [Obsolete("This is a simple access to w private field: how do you use this field for?...")]
        protected double W { get; set; }
    }
}