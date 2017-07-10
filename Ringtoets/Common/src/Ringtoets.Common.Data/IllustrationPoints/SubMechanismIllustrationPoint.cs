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
    /// Illustration point which contains the results.
    /// </summary>
    public class SubMechanismIllustrationPoint : IllustrationPointBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="SubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point result.</param>
        /// <param name="beta">The beta value that was realized.</param>
        /// <param name="stochasts">The stochasts for the sub mechanism illustration point.</param>
        /// <param name="illustrationPointResults">The output variables.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of: 
        /// <list type="bullet">
        /// <item><paramref name="name"/></item>
        /// <item><paramref name="stochasts"/></item>
        /// <item><paramref name="illustrationPointResults"/></item>
        /// </list>
        /// is <c>null</c>.</exception>
        public SubMechanismIllustrationPoint(string name,
                                             double beta,
                                             IEnumerable<SubMechanismIllustrationPointStochast> stochasts,
                                             IEnumerable<IllustrationPointResult> illustrationPointResults)
            : base(name, beta)
        {
            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }
            if (illustrationPointResults == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointResults));
            }

            Stochasts = stochasts;
            IllustrationPointResults = illustrationPointResults;
        }

        /// <summary>
        /// Gets the stochasts that belong to this submechanism illustration point.
        /// </summary>
        public IEnumerable<SubMechanismIllustrationPointStochast> Stochasts { get; }

        /// <summary>
        /// Gets the output variables.
        /// </summary>
        public IEnumerable<IllustrationPointResult> IllustrationPointResults { get; }
    }
}