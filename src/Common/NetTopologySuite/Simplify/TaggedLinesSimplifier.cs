using System.Collections;

namespace GisSharpBlog.NetTopologySuite.Simplify
{
    /// <summary>
    /// Simplifies a collection of TaggedLineStrings, preserving topology
    /// (in the sense that no new intersections are introduced).
    /// </summary>
    public class TaggedLinesSimplifier
    {
        private readonly LineSegmentIndex inputIndex = new LineSegmentIndex();
        private readonly LineSegmentIndex outputIndex = new LineSegmentIndex();

        /// <summary>
        /// 
        /// </summary>
        public TaggedLinesSimplifier()
        {
            DistanceTolerance = 0.0;
        }

        /// <summary>
        /// Gets/Sets the distance tolerance for the simplification.
        /// Points closer than this tolerance to a simplified segment may
        /// be removed.
        /// </summary>        
        public double DistanceTolerance { get; set; }

        /// <summary>
        /// Simplify a collection of <c>TaggedLineString</c>s.
        /// </summary>
        /// <param name="taggedLines">The collection of lines to simplify.</param>
        public void Simplify(IList taggedLines)
        {
            for (IEnumerator i = taggedLines.GetEnumerator(); i.MoveNext();)
            {
                inputIndex.Add((TaggedLineString) i.Current);
            }
            for (IEnumerator i = taggedLines.GetEnumerator(); i.MoveNext();)
            {
                TaggedLineStringSimplifier tlss
                    = new TaggedLineStringSimplifier(inputIndex, outputIndex);
                tlss.DistanceTolerance = DistanceTolerance;
                tlss.Simplify((TaggedLineString) i.Current);
            }
        }
    }
}