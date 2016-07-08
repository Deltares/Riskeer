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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Class holds a method to help <see cref="GrassCoverErosionInwardsScenariosView"/>. 
    /// </summary>
    public static class GrassCoverErosionInwardsHelper
    {
        /// <summary>
        /// Determine which <see cref="GrassCoverErosionInwardsCalculation"/> objects are available for a <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="GrassCoverErosionInwardsCalculation"/> objects.</param>
        /// <param name="calculations">The <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects.</param>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing a <see cref="IList{T}"/> of <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects for each section name which has calculations.</returns>
        public static Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> CollectCalculationsPerSegment(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            var calculationsPerSegment = new Dictionary<string, IList<GrassCoverErosionInwardsCalculation>>();

            if (sectionResults == null || calculations == null)
            {
                return calculationsPerSegment;
            }

            SectionSegments[] sectionSegments =
                (from sectionResult in sectionResults
                 let section = sectionResult.Section
                 let lineSegments = Math2D.ConvertLinePointsToLineSegments(section.Points)
                 select new SectionSegments(section.Name, lineSegments)
                ).ToArray();

            foreach (var calculation in calculations)
            {
                string sectionName = FindSectionForCalculation(sectionSegments, calculation);
                if (sectionName == null)
                {
                    continue;
                }

                UpdateCalculationsOfSegment(calculationsPerSegment, sectionName, calculation);
            }
            return calculationsPerSegment;
        }

        private static string FindSectionForCalculation(SectionSegments[] sectionSegmentsCollection, GrassCoverErosionInwardsCalculation calculation)
        {
            var minimumDistance = double.PositiveInfinity;
            string sectionName = null;

            foreach (var sectionSegments in sectionSegmentsCollection)
            {
                var dikeProfile = calculation.InputParameters.DikeProfile;
                if (dikeProfile == null)
                {
                    continue;
                }

                var distance = sectionSegments.Segments.Min(segment => segment.GetEuclideanDistanceToPoint(dikeProfile.WorldReferencePoint));
                if (!(distance < minimumDistance))
                {
                    continue;
                }

                minimumDistance = distance;
                sectionName = sectionSegments.Name;
            }
            return sectionName;
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
        /// This class represents a named collection of <see cref="Segment2D"/> objects.
        /// </summary>
        private class SectionSegments
        {
            /// <summary>
            /// Creates a new instance of <see cref="SectionSegments"/>.
            /// </summary>
            /// <param name="name">The name of a dike section.</param>
            /// <param name="segments">The <see cref="IEnumerable{Segment2D}"/> representing the geometry of a dike section.</param>
            public SectionSegments(string name, IEnumerable<Segment2D> segments)
            {
                Name = name;
                Segments = segments;
            }

            /// <summary>
            /// Gets the name this <see cref="SectionSegments"/> contains.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the <see cref="IEnumerable{Segment2D}"/> this <see cref="SectionSegments"/> contains.
            /// </summary>
            public IEnumerable<Segment2D> Segments { get; private set; }
        }
    }
}