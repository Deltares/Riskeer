using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Geometry;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms
{
    /// <summary>
    /// Class holds methods to help views when dealing with <see cref="PipingCalculation"/>
    /// </summary>
    public static class PipingCalculationHelper
    {
        /// <summary>
        /// Gets the piping soil profiles matching the input of a calculation.
        /// </summary>
        /// <param name="calculation">The calculation.</param>
        /// <param name="availableSoilModels">The available soil models.</param>
        /// <returns>The (sub)set of soil profiles from <paramref name="availableSoilModels"/>
        /// or empty if no matching <see cref="StochasticSoilModel"/> instances can be found
        /// or when there is not enough information to associate soil profiles to the calculation.</returns>
        public static IEnumerable<PipingSoilProfile> GetPipingSoilProfilesForCalculation(PipingCalculation calculation, IEnumerable<StochasticSoilModel> availableSoilModels)
        {
            RingtoetsPipingSurfaceLine calculationSurfaceLine = calculation.InputParameters.SurfaceLine;
            if (calculationSurfaceLine == null)
            {
                return Enumerable.Empty<PipingSoilProfile>();
            }

            Segment2D[] surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(calculationSurfaceLine.Points.Select(p => new Point2D(p.X, p.Y))).ToArray();

            var soilProfileObjectsForCalculation = new List<PipingSoilProfile>();
            foreach (StochasticSoilModel stochasticSoilModel in availableSoilModels.Where(sm => sm.StochasticSoilProfiles.Any()))
            {
                if (DoesSoilModelGeometryIntersectWithSurfaceLineGeometry(stochasticSoilModel, surfaceLineSegments))
                {
                    soilProfileObjectsForCalculation.AddRange(stochasticSoilModel.StochasticSoilProfiles.Select(ssp => ssp.SoilProfile));
                }
            }
            return soilProfileObjectsForCalculation;
        }

        private static bool DoesSoilModelGeometryIntersectWithSurfaceLineGeometry(StochasticSoilModel stochasticSoilModel, Segment2D[] surfaceLineSegments)
        {
            IEnumerable<Segment2D> soilProfileGeometrySegments = Math2D.ConvertLinePointsToLineSegments(stochasticSoilModel.Geometry);
            return soilProfileGeometrySegments.Any(s => DoesSegmentIntersectWithSegmentArray(s, surfaceLineSegments));
        }

        private static bool DoesSegmentIntersectWithSegmentArray(Segment2D segment, Segment2D[] segmentArray)
        {
            // Consider intersections and overlaps similarly
            return segmentArray.Any(sls => Math2D.GetIntersectionBetweenSegments(segment, sls).IntersectionType != Intersection2DType.DoesNotIntersect);
        }
    }
}