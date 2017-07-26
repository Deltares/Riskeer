// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class couples a SoilProfile to a probability of occurrence.
    /// </summary>
    public class StochasticSoilProfile
    {
        private static readonly Range<double> probabilityValidityRange = new Range<double>(0, 1);
        private double probability;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilProfile"/>.
        /// </summary>
        /// <param name="soilProfileId">Database identifier of the stochastic soil profile.</param>
        /// <param name="probability">Probability of the stochastic soil profile.</param>
        /// <param name="soilProfileType">Type of the stochastic soil profile.</param>
        public StochasticSoilProfile(long soilProfileId, double probability, SoilProfileType soilProfileType)
        {
            Probability = probability;
            SoilProfileType = soilProfileType;
            SoilProfileId = soilProfileId;
        }

        /// <summary>
        /// Gets the type of the stochastic soil profile.
        /// </summary>
        public SoilProfileType SoilProfileType { get; private set; }

        /// <summary>
        /// Gets the database identifier of the stochastic soil profile.
        /// </summary>
        public long SoilProfileId { get; }

        /// <summary>
        /// Gets the probability of the stochastic soil profile.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/>
        /// is outside the range [0, 1].</exception>
        public double Probability
        {
            get
            {
                return probability;
            }
            private set
            {
                if (!probabilityValidityRange.InRange(value))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        string.Format(
                            Resources.StochasticSoilProfile_Probability_Should_be_in_range_0_,
                            probabilityValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture)));
                }
                probability = value;
            }
        }
    }
}