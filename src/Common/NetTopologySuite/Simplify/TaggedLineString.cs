using System.Collections;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace GisSharpBlog.NetTopologySuite.Simplify
{
    /// <summary>
    /// 
    /// </summary>
    public class TaggedLineString
    {
        private readonly IList resultSegs = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentLine"></param>
        public TaggedLineString(ILineString parentLine) : this(parentLine, 2) {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentLine"></param>
        /// <param name="minimumSize"></param>
        public TaggedLineString(ILineString parentLine, int minimumSize)
        {
            Parent = parentLine;
            MinimumSize = minimumSize;
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinimumSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ILineString Parent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate[] ParentCoordinates
        {
            get
            {
                return Parent.Coordinates;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate[] ResultCoordinates
        {
            get
            {
                return ExtractCoordinates(resultSegs);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ResultSize
        {
            get
            {
                int resultSegsSize = resultSegs.Count;
                return resultSegsSize == 0 ? 0 : resultSegsSize + 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TaggedLineSegment[] Segments { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public TaggedLineSegment GetSegment(int i)
        {
            return Segments[i];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seg"></param>
        public void AddToResult(LineSegment seg)
        {
            resultSegs.Add(seg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ILineString AsLineString()
        {
            return Parent.Factory.CreateLineString(ExtractCoordinates(resultSegs));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ILinearRing AsLinearRing()
        {
            return Parent.Factory.CreateLinearRing(ExtractCoordinates(resultSegs));
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            ICoordinate[] pts = Parent.Coordinates;
            Segments = new TaggedLineSegment[pts.Length - 1];
            for (int i = 0; i < pts.Length - 1; i++)
            {
                TaggedLineSegment seg = new TaggedLineSegment(pts[i], pts[i + 1], Parent, i);
                Segments[i] = seg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segs"></param>
        /// <returns></returns>
        private static ICoordinate[] ExtractCoordinates(IList segs)
        {
            ICoordinate[] pts = new ICoordinate[segs.Count + 1];
            LineSegment seg = null;
            for (int i = 0; i < segs.Count; i++)
            {
                seg = (LineSegment) segs[i];
                pts[i] = seg.P0;
            }
            // add last point
            pts[pts.Length - 1] = seg.P1;
            return pts;
        }
    }
}