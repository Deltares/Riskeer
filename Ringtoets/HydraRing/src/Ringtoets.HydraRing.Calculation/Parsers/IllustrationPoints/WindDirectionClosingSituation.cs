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

namespace Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints
{
    /// <summary>
    /// Combination of a wind direction and a closing situation.
    /// </summary>
    public class WindDirectionClosingSituation
    {
        private readonly WindDirection windDirection;
        private readonly string closingSituation;

        /// <summary>
        /// Creates a new instance of <see cref="WindDirectionClosingSituation"/>.
        /// </summary>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="closingSituation">The closing situation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.
        /// </exception>
        public WindDirectionClosingSituation(WindDirection windDirection, string closingSituation)
        {
            if (windDirection == null)
            {
                throw new ArgumentNullException(nameof(windDirection));
            }
            if (closingSituation == null)
            {
                throw new ArgumentNullException(nameof(closingSituation));
            }
            this.windDirection = windDirection;
            this.closingSituation = closingSituation;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == GetType() && Equals((WindDirectionClosingSituation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (windDirection.GetHashCode() * 397) ^ closingSituation.GetHashCode();
            }
        }

        private bool Equals(WindDirectionClosingSituation other)
        {
            return Equals(windDirection, other.windDirection)
                   && string.Equals(closingSituation, other.closingSituation);
        }
    }
}