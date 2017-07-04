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

namespace Ringtoets.Common.Data.Hydraulics.IllustrationPoints
{
    /// <summary>
    /// The general submechanism illustration points result.
    /// </summary>
    public class GeneralResultSubMechanismIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralResultSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="governingWindDirection">The governing wind direction.</param>
        /// <param name="stochasts">The general alpha values.</param>
        /// <param name="topLevelSubMechanismIllustrationPoints">A collection of illustration 
        /// points for every combination of a wind direction and a closing situation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="governingWindDirection"/>,
        /// <paramref name="stochasts"/> or <paramref name="topLevelSubMechanismIllustrationPoints"/> is <c>null</c>.</exception>
        public GeneralResultSubMechanismIllustrationPoint(WindDirection governingWindDirection,
                                                          IEnumerable<Stochast> stochasts,
                                                          IEnumerable<TopLevelSubMechanismIllustrationPoint> topLevelSubMechanismIllustrationPoints)
        {
            if (governingWindDirection == null)
            {
                throw new ArgumentNullException(nameof(governingWindDirection));
            }
            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }
            if (topLevelSubMechanismIllustrationPoints == null)
            {
                throw new ArgumentNullException(nameof(topLevelSubMechanismIllustrationPoints));
            }

            GoverningWindDirection = governingWindDirection;
            Stochasts = stochasts;
            TopLevelSubMechanismIllustrationPoints = topLevelSubMechanismIllustrationPoints;
        }

        /// <summary>
        /// Gets the governing wind direction.
        /// </summary>
        public WindDirection GoverningWindDirection { get; }

        /// <summary>
        /// Gets the general alpha values.
        /// </summary>
        public IEnumerable<Stochast> Stochasts { get; }

        /// <summary>
        /// Gets the collection of illustration points for every combination of a wind direction and a closing situation.
        /// </summary>
        public IEnumerable<TopLevelSubMechanismIllustrationPoint> TopLevelSubMechanismIllustrationPoints { get; }
    }
}