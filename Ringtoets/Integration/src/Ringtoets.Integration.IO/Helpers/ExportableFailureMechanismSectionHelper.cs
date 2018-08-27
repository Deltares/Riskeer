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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Helpers
{
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
                double endDistance = startDistance + Math2D.Length(failureMechanismSection.Points);

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
        /// <param name="sectionStart">The start of the section from the beginning of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of the reference line in meters.</param>
        /// <returns>A geometry based on the reference line and the section start and end.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/> is <c>null</c>.</exception>
        public static IEnumerable<Point2D> GetFailureMechanismSectionGeometry(ReferenceLine referenceLine, int sectionStart, int sectionEnd)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            int index;
            Point2D[] referenceLinePoints = referenceLine.Points.ToArray();
            Point2D startPoint = GetStartPoint(referenceLinePoints, sectionStart, out index);
            var sectionPoints = new List<Point2D>
            {
                startPoint
            };

            int sectionLength = sectionEnd - sectionStart;
            double sectionLengthOnReferenceLine = 0;
            Point2D lastPoint = startPoint;

            foreach (Point2D point in referenceLinePoints.Skip(index + 1))
            {
                double pointsLength = Math2D.Length(new[]
                {
                    lastPoint,
                    point
                });

                sectionLengthOnReferenceLine = sectionLengthOnReferenceLine + pointsLength;

                if (sectionLength > sectionLengthOnReferenceLine)
                {
                    sectionPoints.Add(point);
                    lastPoint = point;
                }
                else if (Math.Abs(sectionLength - sectionLengthOnReferenceLine) < 1e-6)
                {
                    sectionPoints.Add(point);
                    break;
                }
                else if (sectionLength < sectionLengthOnReferenceLine)
                {
                    sectionPoints.Add(InterpolatePoint(lastPoint, point, sectionLengthOnReferenceLine - sectionLength));
                    break;
                }
            }

            return sectionPoints;
        }

        private static Point2D GetStartPoint(Point2D[] referenceLinePoints, int sectionStart, out int index)
        {
            index = 0;
            Point2D startPoint = null;

            if (sectionStart == 0)
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

                totalLength = totalLength + pointsLength;

                if (Math.Abs(totalLength - sectionStart) < 1e-6)
                {
                    startPoint = referenceLinePoints[i];
                    break;
                }

                if (totalLength > sectionStart)
                {
                    startPoint = InterpolatePoint(referenceLinePoints[i - 1], referenceLinePoints[i], sectionStart - (totalLength - pointsLength));
                    index = i - 1;
                    break;
                }
            }

            return startPoint;
        }

        private static Point2D InterpolatePoint(Point2D startPoint, Point2D endPoint, double distance)
        {
            double magnitude = Math.Sqrt(Math.Pow(endPoint.Y - startPoint.Y, 2) + Math.Pow(endPoint.X - startPoint.X, 2));
            return new Point2D(startPoint.X + (distance * ((endPoint.X - startPoint.X) / magnitude)),
                               startPoint.Y + (distance * ((endPoint.Y - startPoint.Y) / magnitude)));
        }
    }
}