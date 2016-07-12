// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Utils
{
    /// <summary>
    /// Class holds helper methods to match <see cref="GrassCoverErosionInwardsCalculation"/> objects 
    /// with <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects. 
    /// </summary>
    public static class GrassCoverErosionInwardsHelper
    {
        /// <summary>
        /// Determine which <see cref="GrassCoverErosionInwardsCalculation"/> objects are available for a <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="GrassCoverErosionInwardsCalculation"/> objects.</param>
        /// <param name="calculations">The <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects.</param>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing a <see cref="IList{T}"/> 
        /// of <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects 
        /// for each section name which has calculations.</returns>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> CollectCalculationsPerSegment(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException("sectionResults");
            }

            if (calculations == null)
            {
                throw new ArgumentNullException("calculations");
            }

            SectionSegments[] sectionSegments = MakeSectionSegments(sectionResults);

            var calculationsPerSegment = new Dictionary<string, IList<GrassCoverErosionInwardsCalculation>>();

            foreach (var calculation in calculations)
            {
                FailureMechanismSection section = FindSectionForCalculation(sectionSegments, calculation);
                if (section == null)
                {
                    continue;
                }

                UpdateCalculationsOfSegment(calculationsPerSegment, section.Name, calculation);
            }
            return calculationsPerSegment;
        }

        /// <summary>
        /// Determine which <see cref="FailureMechanismSection"/> geometrically contains the <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects 
        /// whose <see cref="FailureMechanismSection"/> are considered.</param>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/>.</param>
        /// <returns>The containing <see cref="FailureMechanismSection"/>, or <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static FailureMechanismSection FailureMechanismSectionForCalculation(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
            GrassCoverErosionInwardsCalculation calculation)
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException("sectionResults");
            }

            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            SectionSegments[] sectionSegments = MakeSectionSegments(sectionResults);

            return FindSectionForCalculation(sectionSegments, calculation);
        }

        private static SectionSegments[] MakeSectionSegments(IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults)
        {
            return sectionResults.Select(sr => new SectionSegments(sr.Section)).ToArray();
        }

        private static FailureMechanismSection FindSectionForCalculation(SectionSegments[] sectionSegmentsCollection, GrassCoverErosionInwardsCalculation calculation)
        {
            var dikeProfile = calculation.InputParameters.DikeProfile;
            if (dikeProfile == null)
            {
                return null;
            }

            var minimumDistance = double.PositiveInfinity;
            FailureMechanismSection section = null;

            foreach (var sectionSegments in sectionSegmentsCollection)
            {
                var distance = sectionSegments.Distance(dikeProfile.WorldReferencePoint);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    section = sectionSegments.Section;
                }
            }
            return section;
        }

        private static void UpdateCalculationsOfSegment(Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> calculationsPerSegment,
                                                        string sectionName, GrassCoverErosionInwardsCalculation calculation)
        {
            if (!calculationsPerSegment.ContainsKey(sectionName))
            {
                calculationsPerSegment.Add(sectionName, new List<GrassCoverErosionInwardsCalculation>());
            }
            calculationsPerSegment[sectionName].Add(calculation);
        }

        /// <summary>
        /// This class represents the geometry of a <see cref="FailureMechanismSection"/> as a collection of <see cref="Segment2D"/> objects.
        /// </summary>
        private class SectionSegments
        {
            private readonly IEnumerable<Segment2D> segments;

            /// <summary>
            /// Creates a new instance of <see cref="SectionSegments"/>.
            /// </summary>
            /// <param name="section">The <see cref="FailureMechanismSection"/> whose <see cref="FailureMechanismSection.Points"/> 
            /// this class represents as a collection of <see cref="Segment2D"/> objects.</param>
            public SectionSegments(FailureMechanismSection section)
            {
                Section = section;
                segments = Math2D.ConvertLinePointsToLineSegments(section.Points);
            }

            /// <summary>
            /// Gets the <see cref="FailureMechanismSection"/>.
            /// </summary>
            public FailureMechanismSection Section { get; private set; }

            /// <summary>
            /// Calculate the Euclidean distance between the <see cref="FailureMechanismSection"/> and a <see cref="Point2D"/>.
            /// </summary>
            /// <param name="point">The <see cref="Point2D"/>.</param>
            /// <returns>The Euclidean distance as a <see cref="double"/>.</returns>
            public double Distance(Point2D point)
            {
                return segments.Min(segment => segment.GetEuclideanDistanceToPoint(point));
            }
        }
    }
}