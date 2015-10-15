using System;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Algorithm;

namespace GisSharpBlog.NetTopologySuite.Geometries
{
    /// <summary> 
    /// Represents a planar triangle, and provides methods for calculating various
    /// properties of triangles.
    /// </summary>
    public class Triangle
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Triangle(ICoordinate p0, ICoordinate p1, ICoordinate p2)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
        }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate P0 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate P1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate P2 { get; set; }

        /// <summary>
        /// The inCentre of a triangle is the point which is equidistant
        /// from the sides of the triangle.  This is also the point at which the bisectors
        /// of the angles meet.
        /// </summary>
        /// <returns>
        /// The point which is the InCentre of the triangle.
        /// </returns>
        public ICoordinate InCentre
        {
            get
            {
                // the lengths of the sides, labelled by their opposite vertex
                double len0 = P1.Distance(P2);
                double len1 = P0.Distance(P2);
                double len2 = P0.Distance(P1);
                double circum = len0 + len1 + len2;

                double inCentreX = (len0*P0.X + len1*P1.X + len2*P2.X)/circum;
                double inCentreY = (len0*P0.Y + len1*P1.Y + len2*P2.Y)/circum;
                return new Coordinate(inCentreX, inCentreY);
            }
        }

        ///<summary>
        /// Computes the line which is the perpendicular bisector of the
        ///</summary>
        /// <param name="a">A point</param>
        /// <param name="b">Another point</param>
        /// <returns>The perpendicular bisector, as an HCoordinate line segment a-b.</returns>
        public static HCoordinate PerpendicularBisector(ICoordinate a, ICoordinate b)
        {
            // returns the perpendicular bisector of the line segment ab
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            HCoordinate l1 = new HCoordinate(a.X + dx/2.0, a.Y + dy/2.0, 1.0);
            HCoordinate l2 = new HCoordinate(a.X - dy + dx/2.0, a.Y + dx + dy/2.0, 1.0);
            return new HCoordinate(l1, l2);
        }

        ///<summary>Computes the circumcentre of a triangle.</summary>
        /// <remarks>
        /// The circumcentre is the centre of the circumcircle, 
        /// the smallest circle which encloses the triangle.
        /// It is also the common intersection point of the
        /// perpendicular bisectors of the sides of the triangle,
        /// and is the only point which has equal distance to all three
        /// vertices of the triangle.
        /// </remarks>
        /// <param name="a">A vertex of the triangle</param>
        /// <param name="b">A vertex of the triangle</param>
        /// <param name="c">A vertex of the triangle</param>
        /// <returns>The circumcentre of the triangle</returns>
        public static Coordinate Circumcentre(ICoordinate a, ICoordinate b, ICoordinate c)
        {
            // compute the perpendicular bisector of chord ab
            HCoordinate cab = PerpendicularBisector(a, b);
            // compute the perpendicular bisector of chord bc
            HCoordinate cbc = PerpendicularBisector(b, c);
            // compute the intersection of the bisectors (circle radii)
            HCoordinate hcc = new HCoordinate(cab, cbc);
            Coordinate cc;
            try
            {
                cc = new Coordinate(hcc.GetX(), hcc.GetY());
            }
            catch (NotRepresentableException ex)
            {
                // MD - not sure what we can do to prevent this (robustness problem)
                // Idea - can we condition which edges we choose?
                throw new InvalidOperationException(ex.Message);
            }
            return cc;
        }
    }
}