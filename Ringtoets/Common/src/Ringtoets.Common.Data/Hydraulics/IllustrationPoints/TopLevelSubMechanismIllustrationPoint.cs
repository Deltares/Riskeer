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

namespace Ringtoets.Common.Data.Hydraulics.IllustrationPoints
{
    /// <summary>
    /// Represents the top level combination of <see cref="WindDirection"/>, closing situation and 
    /// <see cref="SubMechanismIllustrationPoint"/>.
    /// </summary>
    public class TopLevelSubMechanismIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="TopLevelSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="closingSituation">The closing situation.</param>
        /// <param name="subMechanismIllustrationPoint">The illustrationPoint.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public TopLevelSubMechanismIllustrationPoint(WindDirection windDirection,
                                                     string closingSituation,
                                                     SubMechanismIllustrationPoint subMechanismIllustrationPoint)
        {
            if (windDirection == null)
            {
                throw new ArgumentNullException(nameof(windDirection));
            }
            if (closingSituation == null)
            {
                throw new ArgumentNullException(nameof(closingSituation));
            }
            if (subMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(subMechanismIllustrationPoint));
            }

            WindDirection = windDirection;
            ClosingSituation = closingSituation;
            SubMechanismIllustrationPoint = subMechanismIllustrationPoint;
        }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingSituation { get; }

        /// <summary>
        /// Gets the wind direction.
        /// </summary>
        public WindDirection WindDirection { get; }

        /// <summary>
        /// Gets the submechanism illustration point.
        /// </summary>
        public SubMechanismIllustrationPoint SubMechanismIllustrationPoint { get; }
    }
}