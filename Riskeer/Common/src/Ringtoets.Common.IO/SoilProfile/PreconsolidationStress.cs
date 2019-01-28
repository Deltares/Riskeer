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

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Class containing parameters for preconsolidation stress definitions.
    /// </summary>
    public class PreconsolidationStress
    {
        /// <summary>
        /// Gets or sets the X coordinate of the preconsolidation stress location.
        /// [m]
        /// </summary>
        public double XCoordinate { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the Z coordinate of the preconsolidation stress location.
        /// [m+NAP]
        /// </summary>
        public double ZCoordinate { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets distribution type of the preconsolidation stress.
        /// </summary>
        public long? StressDistributionType { get; set; }

        /// <summary>
        /// Gets or sets the mean for the distribution of the preconsolidation stress.
        /// [kN/m²]
        /// </summary>
        public double StressMean { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the coefficient of variation for the distribution of the preconsolidation stress.
        /// [kN/m²]
        /// </summary>
        public double StressCoefficientOfVariation { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the shift for the distribution of the  preconsolidation stress.
        /// [kN/m²]
        /// </summary>
        public double StressShift { get; set; } = double.NaN;
    }
}