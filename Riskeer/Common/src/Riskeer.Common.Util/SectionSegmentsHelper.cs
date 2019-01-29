// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.Common.Util
{
    /// <summary>
    /// Class that holds helper methods for <see cref="SectionSegments"/>.
    /// </summary>
    public static class SectionSegmentsHelper
    {
        /// <summary>
        /// Creates <see cref="SectionSegments"/> from an <see cref="IEnumerable{T}"/> of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="sections">The <see cref="IEnumerable{T}"/> of <see cref="FailureMechanismSection"/>
        /// to create <see cref="SectionSegments"/> for.</param>
        /// <returns>An array of <see cref="SectionSegments"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sections"/> is <c>null</c> or when 
        /// an element in <paramref name="sections"/> is <c>null</c>.</exception>
        public static SectionSegments[] MakeSectionSegments(IEnumerable<FailureMechanismSection> sections)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            return sections.Select(s => new SectionSegments(s)).ToArray();
        }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSection"/> for the given <paramref name="point"/>.
        /// </summary>
        /// <param name="sectionSegmentsCollection">The segment sections to get the <see cref="FailureMechanismSection"/> from.</param>
        /// <param name="point">The <see cref="Point2D"/> to get the <see cref="FailureMechanismSection"/> for.</param>
        /// <returns>The <see cref="FailureMechanismSection"/> that corresponds to the given <paramref name="point"/> or <c>null</c> if none found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any paramater is <c>null</c>.</exception>
        public static FailureMechanismSection GetSectionForPoint(IEnumerable<SectionSegments> sectionSegmentsCollection, Point2D point)
        {
            if (sectionSegmentsCollection == null)
            {
                throw new ArgumentNullException(nameof(sectionSegmentsCollection));
            }

            double minimumDistance = double.PositiveInfinity;
            FailureMechanismSection section = null;

            foreach (SectionSegments sectionSegments in sectionSegmentsCollection)
            {
                double distance = sectionSegments.Distance(point);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    section = sectionSegments.Section;
                }
            }

            return section;
        }
    }
}