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
using HydraStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraRealizedStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.RealizedStochast;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraStochast"/> data into <see cref="Stochast"/> data.
    /// </summary>
    public static class StochastConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="Stochast"/> based on the information of <paramref name="hydraStochast"/>.
        /// </summary>
        /// <param name="hydraStochast">The <see cref="HydraStochast"/> to base the 
        /// <see cref="Stochast"/> to create on.</param>
        /// <returns>The newly created <see cref="Stochast"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraStochast"/> 
        /// or <see cref="HydraStochast.Name"/> is <c>null</c>.</exception>
        public static Stochast CreateStochast(HydraStochast hydraStochast)
        {
            if (hydraStochast == null)
            {
                throw new ArgumentNullException(nameof(hydraStochast));
            }
            int stochastDuration = Convert.ToInt32(hydraStochast.Duration);
            return new Stochast(hydraStochast.Name, stochastDuration, hydraStochast.Alpha);
        }

        /// <summary>
        /// Creates a new instance of <see cref="RealizedStochast"/> based on the information of <paramref name="hydraRealizedStochast"/>.
        /// </summary>
        /// <param name="hydraRealizedStochast">The <see cref="HydraRealizedStochast"/> to base the 
        /// <see cref="RealizedStochast"/> to create on.</param>
        /// <returns>The newly created <see cref="Stochast"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraRealizedStochast"/> 
        /// or <see cref="HydraStochast.Name"/> is <c>null</c>.</exception>
        public static RealizedStochast CreateRealizedStochast(HydraRealizedStochast hydraRealizedStochast)
        {
            if (hydraRealizedStochast == null)
            {
                throw new ArgumentNullException(nameof(hydraRealizedStochast));
            }
            int stochastDuration = Convert.ToInt32(hydraRealizedStochast.Duration);
            return new RealizedStochast(hydraRealizedStochast.Name,
                                        stochastDuration,
                                        hydraRealizedStochast.Alpha,
                                        hydraRealizedStochast.Realization);
        }
    }
}