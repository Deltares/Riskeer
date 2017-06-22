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
    public class WindDirectionClosingScenarioIllustrationPoint
    {
        /// <summary>
        /// Creates an instance of <see cref="WindDirectionClosingScenarioIllustrationPoint"/>.
        /// </summary>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="closingScenario">The closing situation.</param>
        /// <param name="illustrationPoint">The illustrationPoint.</param>
        public WindDirectionClosingScenarioIllustrationPoint(WindDirection windDirection,
                                                             string closingScenario,
                                                             IllustrationPoint illustrationPoint)
        {
            if (windDirection == null)
            {
                throw new ArgumentNullException(nameof(windDirection));
            }
            if (closingScenario == null)
            {
                throw new ArgumentNullException(nameof(closingScenario));
            }
            if (illustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(illustrationPoint));
            }

            WindDirection = windDirection;
            ClosingScenario = closingScenario;
            IllustrationPoint = illustrationPoint;
        }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingScenario { get; }

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