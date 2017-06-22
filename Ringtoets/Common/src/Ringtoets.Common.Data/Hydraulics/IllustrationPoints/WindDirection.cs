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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Hydraulics.IllustrationPoints
{
    /// <summary>
    /// A wind direction.
    /// </summary>
    public class WindDirection
    {
        private const int windDirectionAngleNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble>
            windDirectionAngleValidityRange = new Range<RoundedDouble>(new RoundedDouble(windDirectionAngleNumberOfDecimals),
                                                                       new RoundedDouble(windDirectionAngleNumberOfDecimals, 360));

        /// <summary>
        /// Creates a new instance of <see cref="WindDirection"/>.
        /// </summary>
        /// <param name="name">The name of the wind direction.</param>
        /// <param name="angle">The angle of the wind direction in degrees.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="angle"/>
        /// is not in the interval of [0, 360].</exception>
        public WindDirection(string name, double angle)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var windDirectionAngle = new RoundedDouble(2, angle);
            if (!windDirectionAngle.Equals(double.NaN)
                && !windDirectionAngleValidityRange.InRange(windDirectionAngle))
            {
                string message = string.Format(Resources.WindDirection_WindDirectionAngle_Value_needs_to_be_in_Range_0_,
                                               windDirectionAngleValidityRange);
                throw new ArgumentOutOfRangeException(nameof(angle), message);
            }

            Name = name;
            Angle = windDirectionAngle;
        }

        /// <summary>
        /// Gets the descriptive name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the angle of the wind direction.
        /// </summary>
        public RoundedDouble Angle { get; }
    }
}