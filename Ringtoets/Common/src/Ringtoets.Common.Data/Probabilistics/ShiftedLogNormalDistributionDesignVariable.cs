﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Base.Data;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// This class defines a design variable for a shifted log-normal distribution.
    /// </summary>
    public class ShiftedLogNormalDistributionDesignVariable : DesignVariable<ShiftedLogNormalDistribution>
    {
        private readonly ShiftedLogNormalDistribution distribution;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftedLogNormalDistributionDesignVariable"/> class.
        /// </summary>
        /// <param name="distribution">A shifted log-normal distribution.</param>
        public ShiftedLogNormalDistributionDesignVariable(ShiftedLogNormalDistribution distribution) : base(distribution)
        {
            this.distribution = distribution;
        }

        public override RoundedDouble GetDesignValue()
        {
            return new LogNormalDistributionDesignVariable(
                       new LogNormalDistribution(distribution.Mean.NumberOfDecimalPlaces)
                       {
                           Mean = Distribution.Mean - Distribution.Shift,
                           StandardDeviation = Distribution.StandardDeviation
                       })
            {
                Percentile = Percentile
            }.GetDesignValue() + distribution.Shift;
        }
    }
}