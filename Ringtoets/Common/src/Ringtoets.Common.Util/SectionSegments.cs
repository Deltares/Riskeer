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
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Util
{
    /// <summary>
    /// This class represents the geometry of a <see cref="FailureMechanismSection"/> as a collection of <see cref="Segment2D"/> objects.
    /// </summary>
    public class SectionSegments
    {
        private readonly IEnumerable<Segment2D> segments;

        /// <summary>
        /// Creates a new instance of <see cref="SectionSegments"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> whose <see cref="FailureMechanismSection.Points"/> 
        /// this class represents as a collection of <see cref="Segment2D"/> objects.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public SectionSegments(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Section = section;
            segments = Math2D.ConvertPointsToLineSegments(section.Points);
        }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section { get; }

        /// <summary>
        /// Calculate the Euclidean distance between the <see cref="FailureMechanismSection"/> and a <see cref="Point2D"/>.
        /// </summary>
        /// <param name="point">The <see cref="Point2D"/>.</param>
        /// <returns>The Euclidean distance as a <see cref="double"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        public double Distance(Point2D point)
        {
            return segments.Min(segment => segment.GetEuclideanDistanceToPoint(point));
        }
    }
}