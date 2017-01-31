// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class couples a SoilProfile to a probability of occurrence.
    /// </summary>
    public class StochasticSoilProfile
    {
        private double probability;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilProfile"/>.
        /// </summary>
        /// <param name="probability">Probability of the stochastic soil profile.</param>
        /// <param name="soilProfileType">Type of the stochastic soil profile.</param>
        /// <param name="soilProfileId">Database identifier of the stochastic soil profile.</param>
        public StochasticSoilProfile(double probability, SoilProfileType soilProfileType, long soilProfileId)
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
        public long SoilProfileId { get; private set; }

        /// <summary>
        /// Gets the <see cref="PipingSoilProfile"/>.
        /// </summary>
        public PipingSoilProfile SoilProfile { get; set; }

        /// <summary>
        /// Gets the probability of the stochastic soil profile.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/> is outside the range
        /// [0, 1].</exception>
        public double Probability
        {
            get
            {
                return probability;
            }
            private set
            {
                if (!double.IsNaN(value) && (value < 0 || value > 1))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.StochasticSoilProfile_Probability_Should_be_in_range_0_1);
                }
                probability = value;
            }
        }

        /// <summary>
        /// Updates the probability of the <see cref="StochasticSoilProfile"/> 
        /// by adding <paramref name="probabilityToAdd"/>.
        /// </summary>
        /// <param name="probabilityToAdd">The amount to increase the <see cref="Probability"/>
        /// with.</param>
        public void AddProbability(double probabilityToAdd)
        {
            Probability += probabilityToAdd;
        }

        /// <summary>
        /// Updates the <see cref="StochasticSoilProfile"/> with the properties
        /// from <paramref name="fromProfile"/>.
        /// </summary>
        /// <param name="fromProfile">The <see cref="StochasticSoilProfile"/> to
        /// obtain the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromProfile"/>
        /// is <c>null</c>.</exception>
        public void Update(StochasticSoilProfile fromProfile)
        {
            if (fromProfile == null)
            {
                throw new ArgumentNullException(nameof(fromProfile));
            }
            SoilProfile = fromProfile.SoilProfile;
            Probability = fromProfile.Probability;
        }

        public override string ToString()
        {
            return SoilProfile == null ? string.Empty : SoilProfile.ToString();
        }
    }
}