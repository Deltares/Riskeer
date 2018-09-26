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

namespace Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints
{
    /// <summary>
    /// An illustration point which uses the results of two sub illustration points
    /// to obtain a result.
    /// </summary>
    public class FaultTreeIllustrationPoint : IIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="FaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="name">The name of the fault tree illustration point</param>
        /// <param name="beta">The combined beta values of its children.</param>
        /// <param name="stochasts">The combined stochasts of its children.</param>
        /// <param name="combinationType">The way in which the sub illustration points are combined.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public FaultTreeIllustrationPoint(string name,
                                          double beta,
                                          IEnumerable<Stochast> stochasts,
                                          CombinationType combinationType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }

            Beta = beta;
            CombinationType = combinationType;
            Name = name;
            Stochasts = stochasts;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the combined stochasts of its children.
        /// </summary>
        public IEnumerable<Stochast> Stochasts { get; }

        /// <summary>
        /// Gets the combined beta values of its children.
        /// </summary>
        public double Beta { get; }

        /// <summary>
        /// Gets the way in which the sub illustration points are combined to
        /// obtain a result for the fault tree illustration point.
        /// </summary>
        public CombinationType CombinationType { get; }
    }
}