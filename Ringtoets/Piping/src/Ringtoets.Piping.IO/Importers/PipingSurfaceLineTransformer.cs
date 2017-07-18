// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Transforms generic <see cref="SurfaceLine"/> into piping specific <see cref="RingtoetsPipingSurfaceLine"/>.
    /// </summary>
    public class PipingSurfaceLineTransformer : ISurfaceLineTransformer<RingtoetsPipingSurfaceLine>
    {
        private enum ReferenceLineIntersectionsResult
        {
            NoIntersections,
            OneIntersection,
            MultipleIntersectionsOrOverlap
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(PipingSurfaceLineTransformer));
        private readonly ReferenceLine referenceLine;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLineTransformer"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line to determine locations for the surface
        /// lines for.</param>
        public PipingSurfaceLineTransformer(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }
            this.referenceLine = referenceLine;
        }

        public RingtoetsPipingSurfaceLine Transform(SurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            var pipingSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = surfaceLine.Name
            };
            pipingSurfaceLine.SetGeometry(surfaceLine.Points);

            ReferenceLineIntersectionResult result = CheckReferenceLineInterSections(pipingSurfaceLine, referenceLine);

            if (result.TypeOfIntersection != ReferenceLineIntersectionsResult.OneIntersection
                || !ValidateDikeToesInOrder(pipingSurfaceLine, characteristicPoints))
            {
                return null;
            }

            pipingSurfaceLine.ReferenceLineIntersectionWorldPoint = result.IntersectionPoint;

            SetCharacteristicPointsOnSurfaceLine(pipingSurfaceLine, characteristicPoints);

            return pipingSurfaceLine;
        }

        private static void SetCharacteristicPointsOnSurfaceLine(RingtoetsPipingSurfaceLine readSurfaceLine, CharacteristicPoints characteristicPointsLocation)
        {
            if (characteristicPointsLocation == null)
            {
                return;
            }
            readSurfaceLine.TrySetDitchPolderSide(characteristicPointsLocation.DitchPolderSide);
            readSurfaceLine.TrySetBottomDitchPolderSide(characteristicPointsLocation.BottomDitchPolderSide);
            readSurfaceLine.TrySetBottomDitchDikeSide(characteristicPointsLocation.BottomDitchDikeSide);
            readSurfaceLine.TrySetDitchDikeSide(characteristicPointsLocation.DitchDikeSide);
            readSurfaceLine.TrySetDikeToeAtRiver(characteristicPointsLocation.DikeToeAtRiver);
            readSurfaceLine.TrySetDikeToeAtPolder(characteristicPointsLocation.DikeToeAtPolder);
        }

        private static bool ValidateDikeToesInOrder(RingtoetsPipingSurfaceLine readSurfaceLine, CharacteristicPoints characteristicPoints)
        {
            if (characteristicPoints != null && characteristicPoints.DikeToeAtRiver != null && characteristicPoints.DikeToeAtPolder != null)
            {
                Point2D localDikeToeAtRiver = readSurfaceLine.GetLocalPointFromGeometry(characteristicPoints.DikeToeAtRiver);
                Point2D localDikeToeAtPolder = readSurfaceLine.GetLocalPointFromGeometry(characteristicPoints.DikeToeAtPolder);

                if (localDikeToeAtPolder.X <= localDikeToeAtRiver.X)
                {
                    log.WarnFormat(Resources.SurfaceLinesCsvImporter_CheckCharacteristicPoints_EntryPointL_greater_or_equal_to_ExitPointL_for_0_, characteristicPoints.Name);
                    return false;
                }
            }
            return true;
        }

        private static ReferenceLineIntersectionResult CheckReferenceLineInterSections(RingtoetsPipingSurfaceLine readSurfaceLine, ReferenceLine line)
        {
            ReferenceLineIntersectionResult result = GetReferenceLineIntersections(line, readSurfaceLine);

            if (result.TypeOfIntersection == ReferenceLineIntersectionsResult.NoIntersections)
            {
                log.ErrorFormat(Resources.SurfaceLinesCsvImporter_CheckReferenceLineInterSections_Surfaceline_0_does_not_correspond_to_current_referenceline_1_,
                                readSurfaceLine.Name,
                                Resources.SurfaceLinesCsvImporter_CheckReferenceLineInterSections_This_could_be_caused_coordinates_being_local_coordinate_system);
            }
            if (result.TypeOfIntersection == ReferenceLineIntersectionsResult.MultipleIntersectionsOrOverlap)
            {
                log.ErrorFormat(Resources.SurfaceLinesCsvImporter_CheckReferenceLineInterSections_Surfaceline_0_does_not_correspond_to_current_referenceline, readSurfaceLine.Name);
            }

            return result;
        }

        private static ReferenceLineIntersectionResult GetReferenceLineIntersections(ReferenceLine referenceLine, RingtoetsPipingSurfaceLine surfaceLine)
        {
            IEnumerable<Segment2D> surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)));
            Segment2D[] referenceLineSegments = Math2D.ConvertLinePointsToLineSegments(referenceLine.Points).ToArray();

            return GetReferenceLineIntersectionsResult(surfaceLineSegments, referenceLineSegments);
        }

        private static ReferenceLineIntersectionResult GetReferenceLineIntersectionsResult(IEnumerable<Segment2D> surfaceLineSegments, Segment2D[] referenceLineSegments)
        {
            Point2D intersectionPoint = null;
            foreach (Segment2D surfaceLineSegment in surfaceLineSegments)
            {
                foreach (Segment2D referenceLineSegment in referenceLineSegments)
                {
                    Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(surfaceLineSegment, referenceLineSegment);

                    if (result.IntersectionType == Intersection2DType.Intersects)
                    {
                        if (intersectionPoint != null)
                        {
                            // Early exit as multiple intersections is a return result:
                            return ReferenceLineIntersectionResult.CreateMultipleIntersectionsOrOverlapResult();
                        }
                        intersectionPoint = result.IntersectionPoints[0];
                    }

                    if (result.IntersectionType == Intersection2DType.Overlaps)
                    {
                        // Early exit as overlap is a return result:
                        return ReferenceLineIntersectionResult.CreateMultipleIntersectionsOrOverlapResult();
                    }
                }
            }
            return intersectionPoint != null
                       ? ReferenceLineIntersectionResult.CreateIntersectionResult(intersectionPoint)
                       : ReferenceLineIntersectionResult.CreateNoSingleIntersectionResult();
        }

        private class ReferenceLineIntersectionResult
        {
            private ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult typeOfIntersection, Point2D intersectionPoint)
            {
                TypeOfIntersection = typeOfIntersection;
                IntersectionPoint = intersectionPoint;
            }

            public ReferenceLineIntersectionsResult TypeOfIntersection { get; }
            public Point2D IntersectionPoint { get; }

            public static ReferenceLineIntersectionResult CreateNoSingleIntersectionResult()
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.NoIntersections, null);
            }

            public static ReferenceLineIntersectionResult CreateIntersectionResult(Point2D point)
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.OneIntersection, point);
            }

            public static ReferenceLineIntersectionResult CreateMultipleIntersectionsOrOverlapResult()
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.MultipleIntersectionsOrOverlap, null);
            }
        }
    }
}