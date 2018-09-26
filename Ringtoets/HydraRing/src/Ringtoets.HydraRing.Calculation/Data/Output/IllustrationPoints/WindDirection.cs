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
    /// A wind direction for which illustration points are determined.
    /// </summary>
    public class WindDirection
    {
        /// <summary>
        /// Creates a <see cref="WindDirection"/>.
        /// </summary>
        /// <param name="name">The descriptive name.</param>
        /// <param name="angle">The angle.</param>
        public WindDirection(string name, double angle)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Angle = angle;
        }

        /// <summary>
        /// Gets the descriptive name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the angle.
        /// </summary>
        public double Angle { get; }

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

            return Equals((WindDirection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0) * 397) ^ Angle.GetHashCode();
            }
        }

        private bool Equals(WindDirection other)
        {
            return string.Equals(Name, other.Name) && Angle.Equals(other.Angle);
        }
    }
}