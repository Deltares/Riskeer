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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// This class defines a percentile based design variable for a log-normal distribution
    /// defined with a variation coefficient.
    /// </summary>
    public class VariationCoefficientLogNormalDistributionDesignVariable : VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution>
    {
        private static readonly Range<double> percentileValidityRange = new Range<double>(0, 1);
        private double percentile;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogNormalDistributionDesignVariable"/> class
        /// with <see cref="Percentile"/> equal to 0.5.
        /// </summary>
        /// <param name="distribution">A log-normal distribution.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is 
        /// <c>null</c>.</exception>
        public VariationCoefficientLogNormalDistributionDesignVariable(VariationCoefficientLogNormalDistribution distribution) : base(distribution)
        {
            percentile = 0.5;
        }

        /// <summary>
        /// Gets or sets the percentile used to derive a design value based on <see cref="DesignVariable{TDistributionType}.Distribution"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> 
        /// is not in range [0,1].</exception>
        public double Percentile
        {
            get
            {
                return percentile;
            }
            set
            {
                if (!percentileValidityRange.InRange(value))
                {
                    string message = string.Format(Resources.DesignVariable_Percentile_must_be_in_Range_0_,
                                                   percentileValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                    throw new ArgumentOutOfRangeException(null, message);
                }

                percentile = value;
            }
        }

        public override RoundedDouble GetDesignValue()
        {
            return new RoundedDouble(
                Distribution.Mean.NumberOfDecimalPlaces,
                LogNormalDistributionDesignVariableCalculator.CreateWithCoefficientOfVariation(
                                                                 Distribution.Mean,
                                                                 Distribution.CoefficientOfVariation,
                                                                 Distribution.Shift)
                                                             .GetDesignValue(Percentile));
        }
    }
}