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
using System.Collections.Generic;

namespace Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints
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
        /// <param name="stochasts">A collection of <see cref="SubMechanismIllustrationPointStochast"/>
        /// that are associated with this illustration point.</param>
        /// <param name="illustrationPointResults">A collection of 
        /// <see cref="IllustrationPointResult"/> that are associated with this 
        /// illustration point.</param>
        /// <param name="beta">The beta value of the illustration point</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>, <paramref name="stochasts"/>
        /// , or <paramref name="illustrationPointResults"/> is <c>null</c>.</exception>
        public SubMechanismIllustrationPoint(string name,
                                             IEnumerable<SubMechanismIllustrationPointStochast> stochasts,
                                             IEnumerable<IllustrationPointResult> illustrationPointResults,
                                             double beta)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }

            if (illustrationPointResults == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointResults));
            }

            Name = name;
            Beta = beta;
            Stochasts = stochasts;
            Results = illustrationPointResults;
        }

        /// <summary>
        /// Gets the name of the illustration point.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the realized stochasts that belong to this sub mechanism illustration point.
        /// </summary>
        public IEnumerable<SubMechanismIllustrationPointStochast> Stochasts { get; }

        /// <summary>
        /// Gets the beta value that was realized.
        /// </summary>
        public double Beta { get; }

        /// <summary>
        /// Gets the output variables.
        /// </summary>
        public IEnumerable<IllustrationPointResult> Results { get; }
    }
}