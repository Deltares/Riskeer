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
using System.Collections.Generic;

namespace Ringtoets.Common.Data.IllustrationPoints
{
    /// <summary>
    /// Illustration point which represents a node in a fault tree.
    /// </summary>
    public class FaultTreeIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point node.</param>
        /// <param name="stochasts">A collection of <see cref="Stochasts"/>
        /// that are associated with this illustration point node.</param>
        /// <param name="beta">The beta value of this illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or 
        /// <paramref name="stochasts"/> is <c>null</c>.</exception>
        public FaultTreeIllustrationPoint(string name, IEnumerable<Stochast> stochasts, double beta)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }
            Name = name;
            Stochasts = stochasts;
            Beta = beta;
        }

        /// <summary>
        /// Gets the name of the illustration point.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the stochasts that belong to this illustration point.
        /// </summary>
        public IEnumerable<Stochast> Stochasts { get; }

        /// <summary>
        /// Gets the beta value that was realized.
        /// </summary>
        public double Beta { get; }
    }
}