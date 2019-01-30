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
using System.Collections.Generic;
using System.Linq;

namespace Riskeer.Common.Data.IllustrationPoints
{
    /// <summary>
    /// Illustration point which represents a node in a fault tree.
    /// </summary>
    public class FaultTreeIllustrationPoint : IllustrationPointBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point node.</param>
        /// <param name="beta">The beta value of this illustration point.</param>
        /// <param name="stochasts">A collection of <see cref="Stochasts"/>
        /// that are associated with this illustration point node.</param>
        /// <param name="combinationType">The way to combine two nodes into a single
        /// tree node element in the fault tree.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or 
        /// <paramref name="stochasts"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <see cref="stochasts"/> aren't unique.</exception>
        public FaultTreeIllustrationPoint(string name,
                                          double beta,
                                          IEnumerable<Stochast> stochasts,
                                          CombinationType combinationType)
            : base(name, beta)
        {
            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }

            StochastValidator.ValidateStochasts(stochasts);

            CombinationType = combinationType;
            Stochasts = stochasts;
        }

        /// <summary>
        /// Gets the stochasts that belong to this illustration point.
        /// </summary>
        public IEnumerable<Stochast> Stochasts { get; private set; }

        /// <summary>
        /// Gets the combination type corresponding to this illustration point.
        /// </summary>
        public CombinationType CombinationType { get; }

        public override object Clone()
        {
            var clone = (FaultTreeIllustrationPoint) base.Clone();

            clone.Stochasts = Stochasts.Select(s => (Stochast) s.Clone()).ToArray();

            return clone;
        }
    }
}