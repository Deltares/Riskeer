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
    /// Combination of <see cref="WindDirection"/>, closing situation and 
    /// <see cref="IllustrationPoint"/>.
    /// </summary>
    public class WindDirectionClosingSituationIllustrationPoint
    {
        /// <summary>
        /// Creates an instance of <see cref="WindDirectionClosingSituationIllustrationPoint"/>.
        /// </summary>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="closingSituation">The closing situation.</param>
        /// <param name="illustrationPoint">The illustrationPoint.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public WindDirectionClosingSituationIllustrationPoint(WindDirection windDirection,
                                                              string closingSituation,
                                                              IllustrationPoint illustrationPoint)
        {
            if (windDirection == null)
            {
                throw new ArgumentNullException(nameof(windDirection));
            }
            if (closingSituation == null)
            {
                throw new ArgumentNullException(nameof(closingSituation));
            }
            if (illustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(illustrationPoint));
            }

            WindDirection = windDirection;
            ClosingSituation = closingSituation;
            IllustrationPoint = illustrationPoint;
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
        /// Gets the illustration point.
        /// </summary>
        public IllustrationPoint IllustrationPoint { get; }
    }
}