using System.Collections;

namespace GisSharpBlog.NetTopologySuite.Noding
{
    /// <summary>
    /// Base class for <see cref="INoder" />s which make a single pass to find intersections.
    /// This allows using a custom <see cref="ISegmentIntersector" />
    /// (which for instance may simply identify intersections, rather than insert them).
    /// </summary>
    public abstract class SinglePassNoder : INoder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SinglePassNoder"/> class.
        /// </summary>
        public SinglePassNoder()
        {
            SegmentIntersector = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SinglePassNoder"/> class.
        /// </summary>
        /// <param name="segInt">The <see cref="ISegmentIntersector" /> to use.</param>
        public SinglePassNoder(ISegmentIntersector segInt)
        {
            SegmentIntersector = segInt;
        }

        /// <summary>
        /// Gets/sets the <see cref="ISegmentIntersector" /> to use with this noder.
        /// A <see cref="ISegmentIntersector" />  will normally add intersection nodes
        /// to the input segment strings, but it may not - it may
        /// simply record the presence of intersections.
        /// However, some <see cref="INoder" />s may require that intersections be added.
        /// </summary>
        public ISegmentIntersector SegmentIntersector { get; set; }

        /// <summary>
        /// Computes the noding for a collection of <see cref="SegmentString"/>s.
        /// Some Noders may add all these nodes to the input <see cref="SegmentString"/>s;
        /// others may only add some or none at all.
        /// </summary>
        /// <param name="segStrings"></param>
        public abstract void ComputeNodes(IList segStrings);

        /// <summary>
        /// Returns a <see cref="IList"/> of fully noded <see cref="SegmentString"/>s.
        /// The <see cref="SegmentString"/>s have the same context as their parent.
        /// </summary>
        /// <returns></returns>
        public abstract IList GetNodedSubstrings();
    }
}