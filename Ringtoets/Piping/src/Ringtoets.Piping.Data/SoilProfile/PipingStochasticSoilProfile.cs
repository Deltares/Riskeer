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
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.SoilProfile
{
    /// <summary>
    /// This class couples a <see cref="PipingSoilProfile"/> to a probability of occurrence.
    /// </summary>
    public class PipingStochasticSoilProfile : Observable
    {
        private static readonly Range<double> probabilityValidityRange = new Range<double>(0, 1);

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

            ValidateProbability(probability);

            Probability = probability;
            SoilProfile = soilProfile;
        }

        /// <summary>
        /// Gets the probability of the stochastic soil profile.
        /// </summary>
        public double Probability { get; }

        /// <summary>
        /// Gets the <see cref="PipingSoilProfile"/>.
        /// </summary>
        public PipingSoilProfile SoilProfile { get; }

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
            var other = obj as PipingStochasticSoilProfile;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Probability.GetHashCode();
                hashCode = (hashCode * 397) ^ (SoilProfile?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Checks that <paramref name="probability"/> is valid.
        /// </summary>
        /// <param name="probability">Probability of the stochastic soil profile.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="probability"/> 
        /// is outside the range [0, 1].</exception>
        private static void ValidateProbability(double probability)
        {
            if (!probabilityValidityRange.InRange(probability))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(probability),
                    string.Format(
                        Resources.StochasticSoilProfile_Probability_Should_be_in_range_0_,
                        probabilityValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture)));
            }
        }

        private bool Equals(PipingStochasticSoilProfile other)
        {
            return Probability.Equals(other.Probability)
                   && Equals(SoilProfile, other.SoilProfile);
        }
    }
}