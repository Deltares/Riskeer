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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Utils;
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
        /// Determine which <see cref="GrassCoverErosionInwardsCalculation"/> objects are available for a
        /// <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="sections">The <see cref="GrassCoverErosionInwardsCalculation"/> objects.</param>
        /// <param name="calculations">The <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects.</param>
        /// <returns>A <see cref="Dictionary{K, V}"/> containing a <see cref="IList{T}"/> 
        /// of <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> objects 
        /// for each section name which has calculations.</returns>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static Dictionary<string, IList<GrassCoverErosionInwardsCalculation>> CollectCalculationsPerSegment(
            IEnumerable<FailureMechanismSection> sections,
            IEnumerable<GrassCoverErosionInwardsCalculation> calculations)
        {
            if (sections == null)
            {
                throw new ArgumentNullException("sections");
            }

            if (calculations == null)
            {
                throw new ArgumentNullException("calculations");
            }

            SectionSegments[] sectionSegments = SectionSegmentsHelper.MakeSectionSegments(sections);

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
        /// <param name="sections">The <see cref="FailureMechanismSection"/> objects 
        /// whose <see cref="FailureMechanismSection"/> are considered.</param>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/>.</param>
        /// <returns>The containing <see cref="FailureMechanismSection"/>, or <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">When any input parameter is <c>null</c>.</exception>
        public static FailureMechanismSection FailureMechanismSectionForCalculation(
            IEnumerable<FailureMechanismSection> sections,
            GrassCoverErosionInwardsCalculation calculation)
        {
            if (sections == null)
            {
                throw new ArgumentNullException("sections");
            }

            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }

            SectionSegments[] sectionSegments = SectionSegmentsHelper.MakeSectionSegments(sections);

            return FindSectionForCalculation(sectionSegments, calculation);
        }

        private static FailureMechanismSection FindSectionForCalculation(SectionSegments[] sectionSegmentsCollection,
                                                                         GrassCoverErosionInwardsCalculation calculation)
        {
            var dikeProfile = calculation.InputParameters.DikeProfile;
            return dikeProfile != null
                       ? SectionSegmentsHelper.GetSectionForPoint(sectionSegmentsCollection, dikeProfile.WorldReferencePoint)
                       : null;
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
    }
}