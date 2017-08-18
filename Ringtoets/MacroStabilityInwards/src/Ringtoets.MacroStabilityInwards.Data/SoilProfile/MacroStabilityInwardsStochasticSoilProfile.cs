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
using Ringtoets.MacroStabilityInwards.Data.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// This class couples a SoilProfile to a probability of occurrence.
    /// </summary>
    public class MacroStabilityInwardsStochasticSoilProfile : Observable
    {
        private static readonly Range<double> probabilityValidityRange = new Range<double>(0, 1);
        private double probability;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="probability">Probability of the stochastic soil profile.</param>
        /// <param name="soilProfileType">Type of the stochastic soil profile.</param>
        /// <param name="soilProfileId">Database identifier of the stochastic soil profile.</param>
        public MacroStabilityInwardsStochasticSoilProfile(double probability, SoilProfileType soilProfileType, long soilProfileId)
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
        /// Gets the <see cref="ISoilProfile"/>.
        /// </summary>
        public ISoilProfile SoilProfile { get; set; }

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

        /// <summary>
        /// Updates the probability of the <see cref="MacroStabilityInwardsStochasticSoilProfile"/> 
        /// by adding <paramref name="probabilityToAdd"/>.
        /// </summary>
        /// <param name="probabilityToAdd">The amount to increase the <see cref="Probability"/>
        /// with.</param>
        public void AddProbability(double probabilityToAdd)
        {
            Probability += probabilityToAdd;
        }

        /// <summary>
        /// Updates the <see cref="MacroStabilityInwardsStochasticSoilProfile"/> with the properties
        /// from <paramref name="fromProfile"/>.
        /// </summary>
        /// <param name="fromProfile">The <see cref="MacroStabilityInwardsStochasticSoilProfile"/> to
        /// obtain the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromProfile"/>
        /// is <c>null</c>.</exception>
        /// <returns><c>true</c> if the profile has been updated; <c>false</c> otherwise.</returns>
        public bool Update(MacroStabilityInwardsStochasticSoilProfile fromProfile)
        {
            if (fromProfile == null)
            {
                throw new ArgumentNullException(nameof(fromProfile));
            }
            if (!Equals(fromProfile))
            {
                SoilProfile = fromProfile.SoilProfile;
                SoilProfileType = fromProfile.SoilProfileType;
                Probability = fromProfile.Probability;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return SoilProfile?.ToString() ?? string.Empty;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MacroStabilityInwardsStochasticSoilProfile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Probability.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) SoilProfileType;
                hashCode = (hashCode * 397) ^ (SoilProfile?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsStochasticSoilProfile other)
        {
            return Probability.Equals(other.Probability)
                   && SoilProfileType == other.SoilProfileType
                   && Equals(SoilProfile, other.SoilProfile);
        }
    }
}