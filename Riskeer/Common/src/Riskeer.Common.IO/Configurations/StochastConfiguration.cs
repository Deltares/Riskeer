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

namespace Riskeer.Common.IO.Configurations
{
    /// <summary>
    /// Configuration of a stochast with possible values for mean, standard deviation
    /// and variation coefficient.
    /// </summary>
    public class StochastConfiguration
    {
        /// <summary>
        /// Gets or sets the mean of the stochast.
        /// </summary>
        public double? Mean { get; set; }

        /// <summary>
        /// Gets or sets the standard deviation of the stochast.
        /// </summary>
        public double? StandardDeviation { get; set; }

        /// <summary>
        /// Gets or sets the variation coefficient of the stochast.
        /// </summary>
        public double? VariationCoefficient { get; set; }
    }
}