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
    /// This class defines a design variable for a normal distribution.
    /// </summary>
    public class NormalDistributionDesignVariable : PercentileBasedDesignVariable<NormalDistribution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalDistributionDesignVariable"/> class.
        /// </summary>
        /// <param name="distribution">A normal distribution.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is 
        /// <c>null</c>.</exception>
        public NormalDistributionDesignVariable(NormalDistribution distribution) : base(distribution) {}

        public override RoundedDouble GetDesignValue()
        {
            return new RoundedDouble(
                Distribution.Mean.NumberOfDecimalPlaces,
                NormalDistributionDesignVariableCalculator.CreateWithStandardDeviation(
                                                              Distribution.Mean,
                                                              Distribution.StandardDeviation)
                                                          .GetDesignValue(Percentile));
        }
    }
}