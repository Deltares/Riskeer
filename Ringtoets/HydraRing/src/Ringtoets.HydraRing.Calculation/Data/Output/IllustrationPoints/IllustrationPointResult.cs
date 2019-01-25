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

namespace Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints
{
    /// <summary>
    /// An output variable for a sub mechanism illustration point.
    /// </summary>
    public class IllustrationPointResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointResult"/>.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="value">The output of the sub mechanism illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="description"/> 
        /// is <c>null</c>.</exception>
        public IllustrationPointResult(string description, double value)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            Description = description;
            Value = value;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value { get; }
    }
}