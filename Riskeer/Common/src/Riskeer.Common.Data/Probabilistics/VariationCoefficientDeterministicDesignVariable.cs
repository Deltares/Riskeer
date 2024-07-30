﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
    /// This class defines a design variable for a deterministic variation coefficient distribution.
    /// </summary>
    /// <typeparam name="T">The type of the variation coefficient distribution.</typeparam>
    public class VariationCoefficientDeterministicDesignVariable<T> : VariationCoefficientDesignVariable<T>
        where T : class, IVariationCoefficientDistribution
    {
        private readonly double deterministicValue;

        /// <summary>
        /// Creates a new instance of <see cref="VariationCoefficientDeterministicDesignVariable{T}"/>.
        /// </summary>
        /// <param name="distribution">A distribution.</param>
        /// <param name="deterministicValue">The value to be returned for the distribution.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is 
        /// <c>null</c>.</exception>
        public VariationCoefficientDeterministicDesignVariable(T distribution, double deterministicValue = 0.0)
            : base(distribution)
        {
            this.deterministicValue = deterministicValue;
        }

        public override RoundedDouble GetDesignValue()
        {
            return new RoundedDouble(Distribution.Mean.NumberOfDecimalPlaces, deterministicValue);
        }
    }
}