using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Ringtoets.Piping.IO.SoilProfile
{
    public class StochasticSoilModelSegment
    {
        public StochasticSoilModelSegment(long segmentSoilModelId, string segmentSoilModelName, string segmentName)
        {
            SegmentSoilModelId = segmentSoilModelId;
            SegmentSoilModelName = segmentSoilModelName;
            SegmentName = segmentName;
            SegmentPoints = new List<Point2D>();
            StochasticSoilProfileProbabilities = new List<StochasticSoilProfileProbability>();
        }

        public long SegmentSoilModelId { get; private set; }
        public string SegmentSoilModelName { get; private set; }

        public string SegmentName { get; private set; }

        public List<Point2D> SegmentPoints { get; private set; }
        public List<StochasticSoilProfileProbability> StochasticSoilProfileProbabilities { get; private set; }
    }
}