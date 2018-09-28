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

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// This object represents a probabilistic distribution.
    /// </summary>
    /// <seealso cref="IVariationCoefficientDistribution"/>
    public interface IDistribution : ICloneable
    {
        /// <summary>
        /// Gets or sets the mean (expected value, E(X)) of the distribution.
        /// </summary>
        RoundedDouble Mean { get; set; }

        /// <summary>
        /// Gets or sets the standard deviation (square root of the Var(X)) of the distribution.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Standard deviation is less than 0.</exception>
        RoundedDouble StandardDeviation { get; set; }
    }
}