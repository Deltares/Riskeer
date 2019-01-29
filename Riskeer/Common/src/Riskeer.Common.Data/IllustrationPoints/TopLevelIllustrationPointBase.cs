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

namespace Riskeer.Common.Data.IllustrationPoints
{
    /// <summary>
    /// A base class for combinations of wind direction, closing situations 
    /// and illustration points.
    /// </summary>
    public abstract class TopLevelIllustrationPointBase : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="TopLevelIllustrationPointBase"/>.
        /// </summary>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="closingSituation">The closing situation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters
        /// is <c>null</c>.</exception>
        protected TopLevelIllustrationPointBase(WindDirection windDirection, string closingSituation)
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
        /// Gets the wind direction.
        /// </summary>
        public WindDirection WindDirection { get; private set; }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingSituation { get; }

        public virtual object Clone()
        {
            var clone = (TopLevelIllustrationPointBase) MemberwiseClone();

            clone.WindDirection = (WindDirection) WindDirection.Clone();

            return clone;
        }
    }
}