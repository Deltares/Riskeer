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

namespace Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints
{
    /// <summary>
    /// Combination of a wind direction and a closing situation.
    /// </summary>
    public class WindDirectionClosingSituation
    {
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

            WindDirection = windDirection;
            ClosingSituation = closingSituation;
        }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingSituation { get; }

        /// <summary>
        /// Gets the wind direction.
        /// </summary>
        public WindDirection WindDirection { get; }

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

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((WindDirectionClosingSituation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (WindDirection.GetHashCode() * 397) ^ ClosingSituation.GetHashCode();
            }
        }

        private bool Equals(WindDirectionClosingSituation other)
        {
            return Equals(WindDirection, other.WindDirection)
                   && string.Equals(ClosingSituation, other.ClosingSituation);
        }
    }
}