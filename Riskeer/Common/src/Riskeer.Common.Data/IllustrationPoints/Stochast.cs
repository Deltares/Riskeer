// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.Common.Data.IllustrationPoints
{
    /// <summary>
    /// Container alpha value definitions.
    /// </summary>
    public class Stochast : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="Stochast"/>.
        /// </summary>
        /// <param name="name">The name of the stochast.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="alpha">The alpha value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public Stochast(string name, double duration, double alpha)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Duration = new RoundedDouble(1, duration);
            Alpha = new RoundedDouble(5, alpha);
        }

        /// <summary>
        /// Gets the name of the stochast.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the duration of the stochast.
        /// </summary>
        public RoundedDouble Duration { get; }

        /// <summary>
        /// Gets the alpha value of the stochast.
        /// </summary>
        public RoundedDouble Alpha { get; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}