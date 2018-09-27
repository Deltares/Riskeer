﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Helpers
{
    /// <summary>
    /// Class containing helper methods for <see cref="ExportableFailureMechanismSection"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionHelper
    {
        /// <summary>
        /// Creates a lookup between failure mechanism section results and the corresponding
        /// <see cref="ExportableFailureMechanismSection"/>.
        /// </summary>
        /// <typeparam name="TSectionResult">The type of <see cref="FailureMechanismSectionResult"/>.</typeparam>
        /// <param name="failureMechanismSectionResults">The failure mechanism sections results to create a
        /// <see cref="ExportableFailureMechanismSection"/> for.</param>
        /// <returns>A <see cref="IDictionary{TKey,TValue}"/> between the failure mechanism section results
        /// and <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResults"/> is <c>null</c>.</exception>
        public static IDictionary<TSectionResult, ExportableFailureMechanismSection> CreateFailureMechanismSectionResultLookup<TSectionResult>(
            IEnumerable<TSectionResult> failureMechanismSectionResults)
            where TSectionResult : FailureMechanismSectionResult
        {
            if (failureMechanismSectionResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResults));
            }

            var failureMechanismSectionsLookup = new Dictionary<TSectionResult, ExportableFailureMechanismSection>();

            double startDistance = 0;
            foreach (TSectionResult sectionResult in failureMechanismSectionResults)
            {
                FailureMechanismSection failureMechanismSection = sectionResult.Section;
                double endDistance = startDistance + failureMechanismSection.Length;

                failureMechanismSectionsLookup[sectionResult] = new ExportableFailureMechanismSection(failureMechanismSection.Points,
                                                                                                      startDistance,
                                                                                                      endDistance);

                startDistance = endDistance;
            }

            return failureMechanismSectionsLookup;
        }

        /// <summary>
        /// Gets the geometry of a failure mechanism section based on the geometry
        /// of the <paramref name="referenceLine"/>, <paramref name="sectionStart"/> and <paramref name="sectionEnd"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line to get the geometry from.</param>
        /// <param name="sectionStart">The start of the section relative to the start of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section relative to the start of the reference line in meters.</param>
        /// <returns>The failure mechanism section geometry.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/> is <c>null</c>.</exception>
        public static IEnumerable<Point2D> GetFailureMechanismSectionGeometry(ReferenceLine referenceLine, double sectionStart, double sectionEnd)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            int startIndex;
            int endIndex;

            Point2D[] referenceLinePoints = referenceLine.Points.ToArray();
            Point2D startPoint = GetPointByOffset(referenceLinePoints, sectionStart, out startIndex);
            Point2D endPoint = GetPointByOffset(referenceLinePoints, sectionEnd, out endIndex);

            var sectionPoints = new List<Point2D>
            {
                startPoint
            };

            sectionPoints.AddRange(referenceLinePoints.Skip(startIndex + 1).Take(endIndex - startIndex));

            if (!sectionPoints.Contains(endPoint))
            {
                sectionPoints.Add(endPoint);
            }

            return sectionPoints;
        }

        private static Point2D GetPointByOffset(Point2D[] referenceLinePoints, double offset, out int index)
        {
            index = 0;
            Point2D point = null;

            if (Math.Abs(offset) < double.Epsilon)
            {
                return referenceLinePoints[0];
            }

            double totalLength = 0;

            for (var i = 1; i < referenceLinePoints.Length; i++)
            {
                index = i;
                double pointsLength = Math2D.Length(new[]
                {
                    referenceLinePoints[i - 1],
                    referenceLinePoints[i]
                });

                totalLength += pointsLength;

                if (Math.Abs(totalLength - offset) < double.Epsilon)
                {
                    point = referenceLinePoints[i];
                    break;
                }

                if (totalLength > offset)
                {
                    double distance = offset - (totalLength - pointsLength);
                    point = InterpolatePoint(referenceLinePoints[i - 1], referenceLinePoints[i], distance);
                    index = i - 1;
                    break;
                }
            }

            return point;
        }

        private static Point2D InterpolatePoint(Point2D startPoint, Point2D endPoint, double distance)
        {
            double magnitude = Math2D.Length(new[]
            {
                startPoint,
                endPoint
            });

            double factor = distance / magnitude;

            return new Point2D(startPoint.X + (factor * (endPoint.X - startPoint.X)),
                               startPoint.Y + (factor * (endPoint.Y - startPoint.Y)));
        }
    }
}