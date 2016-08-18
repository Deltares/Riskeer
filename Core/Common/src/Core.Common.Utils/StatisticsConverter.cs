// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using MathNet.Numerics.Distributions;

namespace Core.Common.Utils
{
    /// <summary>
    /// This class contains a converter related to the statistics domain.
    /// </summary>
    public static class StatisticsConverter
    {
        /// <summary>
        /// Calculates the probability from a norm.
        /// </summary>
        /// <param name="norm">The norm to convert.</param>
        /// <returns>The probability.</returns>
        public static double NormToBeta(double norm)
        {
            return -Normal.InvCDF(0.0, 1.0, 1.0 / norm);
        }
    }
}