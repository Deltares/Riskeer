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
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingWindDirection = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;

namespace Riskeer.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraRingWindDirection"/> data into <see cref="WindDirection"/> data.
    /// </summary>
    public static class WindDirectionConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="WindDirection"/> based on the information of <paramref name="hydraRingWindDirection"/>.
        /// </summary>
        /// <param name="hydraRingWindDirection">The <see cref="HydraRingWindDirection"/> to base the 
        /// <see cref="WindDirection"/> to create on.</param>
        /// <returns>The newly created <see cref="WindDirection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRingWindDirection"/> 
        /// is <c>null</c>.</exception>
        public static WindDirection Convert(HydraRingWindDirection hydraRingWindDirection)
        {
            if (hydraRingWindDirection == null)
            {
                throw new ArgumentNullException(nameof(hydraRingWindDirection));
            }

            return new WindDirection(hydraRingWindDirection.Name,
                                     hydraRingWindDirection.Angle);
        }
    }
}