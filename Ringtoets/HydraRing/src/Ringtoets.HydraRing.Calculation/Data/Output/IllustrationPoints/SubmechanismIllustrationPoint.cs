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
    /// Illustration point which contains the result of applying the sub mechanism.
    /// </summary>
    public class SubMechanismIllustrationPoint : IIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="SubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point.</param>
        /// <param name="beta">The beta value of the illustration point</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        public SubMechanismIllustrationPoint(string name, double beta)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Beta = beta;
            Stochasts = new List<RealizedStochast>();
            Results = new List<IllustrationPointResult>();
        }


        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the stochasts that were realized.
        /// </summary>
        public ICollection<RealizedStochast> Stochasts { get; }

        /// <summary>
        /// Gets the beta values that were realized.
        /// </summary>
        public double Beta { get; set; }

        /// <summary>
        /// Gets the output variables.
        /// </summary>
        public ICollection<IllustrationPointResult> Results { get; }
    }
}