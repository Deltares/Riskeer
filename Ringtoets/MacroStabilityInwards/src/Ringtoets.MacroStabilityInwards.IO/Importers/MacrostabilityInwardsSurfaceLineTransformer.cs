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
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Importers
{
    /// <summary>
    /// Transforms generic <see cref="SurfaceLine"/> into piping specific <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>.
    /// </summary>
    public class MacroStabilityInwardsSurfaceLineTransformer : ISurfaceLineTransformer<RingtoetsMacroStabilityInwardsSurfaceLine>
    {
        private enum ReferenceLineIntersectionsResult
        {
            NoIntersections,
            OneIntersection,
            MultipleIntersectionsOrOverlap
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(MacroStabilityInwardsSurfaceLineTransformer));
        private readonly ReferenceLine referenceLine;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSurfaceLineTransformer"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line to determine locations for the surface
        /// lines for.</param>
        public MacroStabilityInwardsSurfaceLineTransformer(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }
            this.referenceLine = referenceLine;
        }

        public RingtoetsMacroStabilityInwardsSurfaceLine Transform(SurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            var macroStabilityInwardsSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = surfaceLine.Name
            };
            macroStabilityInwardsSurfaceLine.SetGeometry(surfaceLine.Points);

            ReferenceLineIntersectionResult result = CheckReferenceLineInterSections(macroStabilityInwardsSurfaceLine, referenceLine);

            if (result.TypeOfIntersection != ReferenceLineIntersectionsResult.OneIntersection)
            {
                return null;
            }

            macroStabilityInwardsSurfaceLine.ReferenceLineIntersectionWorldPoint = result.IntersectionPoint;

            SetCharacteristicPointsOnSurfaceLine(macroStabilityInwardsSurfaceLine, characteristicPoints);

            return macroStabilityInwardsSurfaceLine;
        }

        private static void SetCharacteristicPointsOnSurfaceLine(RingtoetsMacroStabilityInwardsSurfaceLine readSurfaceLine, CharacteristicPoints characteristicPointsLocation)
        {
            // TODO add characteristic points to surface line
        }

        private static ReferenceLineIntersectionResult CheckReferenceLineInterSections(RingtoetsMacroStabilityInwardsSurfaceLine readSurfaceLine, ReferenceLine line)
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

        private static ReferenceLineIntersectionResult GetReferenceLineIntersections(ReferenceLine referenceLine, RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
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