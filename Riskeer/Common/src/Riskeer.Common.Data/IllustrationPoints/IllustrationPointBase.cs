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
using Core.Common.Base.Data;

namespace Riskeer.Common.Data.IllustrationPoints
{
    /// <summary>
    /// Base class for illustration point elements.
    /// </summary>
    public abstract class IllustrationPointBase : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointBase"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point.</param>
        /// <param name="beta">The beta value of the illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        protected IllustrationPointBase(string name, double beta)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Beta = new RoundedDouble(5, beta);
        }

        /// <summary>
        /// Gets the name of the illustration point.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the beta value that was realized.
        /// </summary>
        public RoundedDouble Beta { get; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}