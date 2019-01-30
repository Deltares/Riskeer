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

namespace Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints
{
    /// <summary>
    /// The general illustration points result.
    /// </summary>
    public class GeneralResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/>.
        /// </summary>
        /// <param name="beta">The general beta value.</param>
        /// <param name="governingWindDirection">The governing wind direction.</param>
        /// <param name="stochasts">The general alpha stochast values.</param>
        /// <param name="illustrationPoints">The trees of illustration points for each wind direction.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="governingWindDirection"/>, 
        /// <paramref name="stochasts"/>, or <paramref name="illustrationPoints"/> is <c>null</c>.</exception>
        public GeneralResult(double beta, WindDirection governingWindDirection,
                             IEnumerable<Stochast> stochasts,
                             Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode> illustrationPoints)
        {
            if (governingWindDirection == null)
            {
                throw new ArgumentNullException(nameof(governingWindDirection));
            }

            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }

            if (illustrationPoints == null)
            {
                throw new ArgumentNullException(nameof(illustrationPoints));
            }

            Beta = beta;
            GoverningWindDirection = governingWindDirection;
            Stochasts = stochasts;
            IllustrationPoints = illustrationPoints;
        }

        /// <summary>
        /// Gets the general beta value.
        /// </summary>
        public double Beta { get; }

        /// <summary>
        /// Gets the governing wind direction.
        /// </summary>
        public WindDirection GoverningWindDirection { get; }

        /// <summary>
        /// Gets the general alpha values.
        /// </summary>
        public IEnumerable<Stochast> Stochasts { get; }

        /// <summary>
        /// Gets the tree of illustration points for each wind direction and closing situation.
        /// </summary>
        public Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode> IllustrationPoints { get; }
    }
}