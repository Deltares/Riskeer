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
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="SubmechanismIllustrationPoint"/> in the <see cref="IllustrationPointsTableControl"/>.
    /// </summary>
    internal class IllustrationPointRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointRow"/>.
        /// </summary>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="closingSituation">The closing situation.</param>
        /// <param name="probability">The calculated probability.</param>
        /// <param name="reliability">The calculated reliability.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="windDirection"/> or <paramref name="closingSituation"/>
        /// is <c>null</c>.</exception>
        public IllustrationPointRow(string windDirection, string closingSituation,
                                    double probability, double reliability)
        {
            if (windDirection == null)
            {
                throw new ArgumentNullException(nameof(windDirection));
            }
            if (closingSituation == null)
            {
                throw new ArgumentNullException(nameof(closingSituation));
            }
            WindDirection = windDirection;
            ClosingSituation = closingSituation;
            Probability = probability;
            Reliability = reliability;
        }

        /// <summary>
        /// Gets the wind direction.
        /// </summary>
        public string WindDirection { get; }

        /// <summary>
        /// Gets the closing situation.
        /// </summary>
        public string ClosingSituation { get; }

        /// <summary>
        /// Gets the calculated probability.
        /// </summary>
        public double Probability { get; }

        /// <summary>
        /// Gets the calculated reliability.
        /// </summary>
        public double Reliability { get; }
    }
}