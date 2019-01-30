// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base;
using Riskeer.Common.Data.Probability;
using Riskeer.Piping.Primitives;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Piping.Data.SoilProfile
{
    /// <summary>
    /// This class couples a <see cref="PipingSoilProfile"/> to a probability of occurrence.
    /// </summary>
    public class PipingStochasticSoilProfile : Observable
    {
        private double probability;

        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="probability">Probability of the stochastic soil profile.</param>
        /// <param name="soilProfile">The soil profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="probability"/>
        /// is outside the range [0, 1].</exception>
        public PipingStochasticSoilProfile(double probability, PipingSoilProfile soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            Probability = probability;
            SoilProfile = soilProfile;
        }

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
                ProbabilityHelper.ValidateProbability(
                    value,
                    nameof(value),
                    RiskeerCommonDataResources.StochasticSoilProfile_Probability_Should_be_in_range_0_);
                probability = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="PipingSoilProfile"/>.
        /// </summary>
        public PipingSoilProfile SoilProfile { get; private set; }

        /// <summary>
        /// Updates the <see cref="PipingStochasticSoilProfile"/> with the properties
        /// from <paramref name="fromProfile"/>.
        /// </summary>
        /// <param name="fromProfile">The <see cref="PipingStochasticSoilProfile"/> to
        /// obtain the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromProfile"/>
        /// is <c>null</c>.</exception>
        public void Update(PipingStochasticSoilProfile fromProfile)
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
            return SoilProfile.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((PipingStochasticSoilProfile) obj);
        }

        public override int GetHashCode()
        {
            return Probability.GetHashCode();
        }

        private bool Equals(PipingStochasticSoilProfile other)
        {
            return Probability.Equals(other.Probability)
                   && Equals(SoilProfile, other.SoilProfile);
        }
    }
}