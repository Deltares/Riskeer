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

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// Describes how the <see cref="StructuresParameterRow.VarianceValue"/> should be interpreted.
    /// </summary>
    public enum VarianceType
    {
        /// <summary>
        /// It hasn't been specified how the property should be interpreted. This could mean
        /// the origin was lacking information or because the value is considered deterministic.
        /// </summary>
        NotSpecified,

        /// <summary>
        /// The property should be interpreted as the standard deviation of a random variable.
        /// </summary>
        StandardDeviation,

        /// <summary>
        /// The property should be interpreted as the coefficient of variation
        /// (standard deviation / mean).
        /// </summary>
        CoefficientOfVariation
    }
}