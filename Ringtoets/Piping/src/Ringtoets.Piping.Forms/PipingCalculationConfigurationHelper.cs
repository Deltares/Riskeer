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
    public static class PipingCalculationConfigurationHelper
    {
        /// <summary>
        /// Creates a structure of <see cref="PipingCalculationGroup"/> and <see cref="PipingCalculation"/> based on combination of the
        /// <paramref name="surfaceLines"/> and the <paramref name="soilModels"/>.
        /// </summary>
        /// <param name="surfaceLines">Surface lines to generate the structure for and to use to configure <see cref="PipingCalculation"/>
        /// with.</param>
        /// <param name="soilModels">The soil models from which profiles are taken to configure <see cref="PipingCalculation"/> with.</param>
        /// <returns></returns>
        public static IEnumerable<IPipingCalculationItem> Generate(IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> soilModels)
        {
            if (surfaceLines == null)
            {
                return Enumerable.Empty<IPipingCalculationItem>();
            }
            var pipingCalculationGroups = surfaceLines.Select(sl => CreateCalculationGroup(sl, soilModels));
            return pipingCalculationGroups;
        }

        /// <summary>
        /// Gets the piping soil profiles matching the input of a calculation.
        /// </summary>
        /// <param name="surfaceLine">The surface line used to match a <see cref="StochasticSoilModel"/>.</param>
        /// <param name="availableSoilModels">The available soil models.</param>
        /// <returns>The (sub)set of soil profiles from <paramref name="availableSoilModels"/>
        /// or empty if no matching <see cref="StochasticSoilModel"/> instances can be found
        /// or when there is not enough information to associate soil profiles to the calculation.</returns>
        public static IEnumerable<PipingSoilProfile> GetPipingSoilProfilesForSurfaceLine(RingtoetsPipingSurfaceLine surfaceLine, IEnumerable<StochasticSoilModel> availableSoilModels)
        {
            if (surfaceLine == null)
            {
                return Enumerable.Empty<PipingSoilProfile>();
            }

            Segment2D[] surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y))).ToArray();

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

        private static IPipingCalculationItem CreateCalculationGroup(RingtoetsPipingSurfaceLine surfaceLine, IEnumerable<StochasticSoilModel> soilModels)
        {
            var pipingCalculationGroup = new PipingCalculationGroup(surfaceLine.Name, true);
            if (soilModels != null)
            {
                foreach (var profile in GetPipingSoilProfilesForSurfaceLine(surfaceLine, soilModels))
                {
                    pipingCalculationGroup.Children.Add(CreatePipingCalculation(surfaceLine, profile));
                }
            }

            return pipingCalculationGroup;
        }

        private static IPipingCalculationItem CreatePipingCalculation(RingtoetsPipingSurfaceLine surfaceLine, PipingSoilProfile profile)
        {
            return new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    SoilProfile = profile
                }
            };
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