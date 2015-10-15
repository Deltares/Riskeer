using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace GisSharpBlog.NetTopologySuite.Triangulate
{
    /// <summary>
    /// Models a constraint segment which can be split in two in various ways, 
    /// according to certain geometric constraints.
    /// </summary>
    /// <author>Martin Davis</author>
    public class SplitSegment
    {
        private readonly LineSegment _seg;
        private readonly double _segLen;

        public SplitSegment(LineSegment seg)
        {
            _seg = seg;
            _segLen = seg.Length;
        }

        public double MinimumLength { get; set; }

        public ICoordinate SplitPoint { get; private set; }

        public void SplitAt(double length, ICoordinate endPt)
        {
            double actualLen = GetConstrainedLength(length);
            double frac = actualLen/_segLen;
            SplitPoint = endPt.Equals2D(_seg.P0) ? _seg.PointAlong(frac) : PointAlongReverse(_seg, frac);
        }

        public void SplitAt(ICoordinate pt)
        {
            // check that given pt doesn't violate min length
            double minFrac = MinimumLength/_segLen;
            if (pt.Distance(_seg.P0) < MinimumLength)
            {
                SplitPoint = _seg.PointAlong(minFrac);
                return;
            }
            if (pt.Distance(_seg.P1) < MinimumLength)
            {
                SplitPoint = PointAlongReverse(_seg, minFrac);
                return;
            }
            // passes minimum distance check - use provided point as split pt
            SplitPoint = pt;
        }

        /// <summary>
        /// Computes the {@link Coordinate} that lies a given fraction along the line defined by the
        /// reverse of the given segment. A fraction of <code>0.0</code> returns the end point of the
        /// segment; a fraction of <code>1.0</code> returns the start point of the segment.
        /// </summary>
        /// <param name="seg">the LineSegment</param>
        /// <param name="segmentLengthFraction">the fraction of the segment length along the line</param>
        /// <returns>the point at that distance</returns>
        private static ICoordinate PointAlongReverse(LineSegment seg, double segmentLengthFraction)
        {
            var coord = new Coordinate();
            coord.X = seg.P1.X - segmentLengthFraction*(seg.P1.X - seg.P0.X);
            coord.Y = seg.P1.Y - segmentLengthFraction*(seg.P1.Y - seg.P0.Y);
            return coord;
        }

        private double GetConstrainedLength(double len)
        {
            if (len < MinimumLength)
            {
                return MinimumLength;
            }
            return len;
        }
    }
}