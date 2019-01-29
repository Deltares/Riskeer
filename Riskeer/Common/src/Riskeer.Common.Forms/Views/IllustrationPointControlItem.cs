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
using Core.Common.Base.Data;
using Riskeer.Common.Data.IllustrationPoints;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Represents a single illustration point to be used in 
    /// <see cref="IllustrationPointsControl"/>.
    /// </summary>
    public class IllustrationPointControlItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointControlItem"/>.
        /// </summary>
        /// <param name="source">The wrapped source <see cref="TopLevelIllustrationPointBase"/>.</param>
        /// <param name="windDirectionName">The name of the wind direction.</param>
        /// <param name="closingSituation">The closing situation of the illustration
        ///     point.</param>
        /// <param name="stochasts">The associated stochasts.</param>
        /// <param name="beta">The beta of the illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when parameter 
        /// <paramref name="source"/>, <paramref name="windDirectionName"/>,
        /// <paramref name="closingSituation"/> or <paramref name="stochasts"/> is 
        /// <c>null</c>.</exception>
        public IllustrationPointControlItem(TopLevelIllustrationPointBase source,
                                            string windDirectionName,
                                            string closingSituation,
                                            IEnumerable<Stochast> stochasts,
                                            RoundedDouble beta)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (windDirectionName == null)
            {
                throw new ArgumentNullException(nameof(windDirectionName));
            }

            if (closingSituation == null)
            {
                throw new ArgumentNullException(nameof(closingSituation));
            }

            if (stochasts == null)
            {
                throw new ArgumentNullException(nameof(stochasts));
            }

            Source = source;
            WindDirectionName = windDirectionName;
            ClosingSituation = closingSituation;
            Beta = beta;
            Stochasts = stochasts;
        }

        /// <summary>
        /// Gets the wrapped source object.
        /// </summary>
        public TopLevelIllustrationPointBase Source { get; }

        /// <summary>
        /// Gets the wind direction name.
        /// </summary>
        public string WindDirectionName { get; }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingSituation { get; }

        /// <summary>
        /// Gets the beta.
        /// </summary>
        public RoundedDouble Beta { get; }

        /// <summary>
        /// Gets the associated stochasts.
        /// </summary>
        public IEnumerable<Stochast> Stochasts { get; }
    }
}