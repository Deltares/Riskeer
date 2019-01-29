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
using Core.Common.Base.Data;

namespace Riskeer.Common.Data.Probabilistics
{
    /// <summary>
    /// This object represents a probabilistic distribution.
    /// </summary>
    /// <seealso cref="IDistribution"/>
    public interface IVariationCoefficientDistribution : ICloneable
    {
        /// <summary>
        /// Gets or sets the mean (expected value, E(X)) of the distribution.
        /// </summary>
        /// <remarks>As <see cref="CoefficientOfVariation"/> cannot be negative, the absolute
        /// value of the mean is used when the standard deviation needs to be calculated.</remarks>
        RoundedDouble Mean { get; set; }

        /// <summary>
        /// Gets or sets the coefficient of variation (CV, also known as relative standard
        /// deviation (SRD). Defined as standard deviation / |E(X)|) of the distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coefficient of variation
        /// is less than 0.</exception>
        RoundedDouble CoefficientOfVariation { get; set; }
    }
}