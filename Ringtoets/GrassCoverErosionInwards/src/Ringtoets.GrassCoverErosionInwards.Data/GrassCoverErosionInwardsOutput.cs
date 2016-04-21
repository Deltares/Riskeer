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

using Core.Common.Base.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// This class contains the results of a GrassCoverErosioninwards calculation.
    /// </summary>
    public class GrassCoverErosionInwardsOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsOutput"/>.
        /// </summary>
        /// <param name="requiredProbability">The required (maximum allowed) probability of failure.</param>
        /// <param name="requiredReliability">The required (maximum allowed) reliability of the failure mechanism.</param>
        /// <param name="probability">The calculated probability of failing.</param>
        /// <param name="reliability">The calculated reliability of the failure mechanism.</param>
        /// <param name="factorOfSafety">The factor of safety for the failure mechanisms.</param>
        public GrassCoverErosionInwardsOutput(double requiredProbability, double requiredReliability, double probability, double reliability, double factorOfSafety)
        {
            RequiredProbability = new RoundedDouble(2, requiredProbability);
            RequiredReliability = new RoundedDouble(3, requiredReliability);
            Probability = new RoundedDouble(2, probability);
            Reliability = new RoundedDouble(3, reliability);
            FactorOfSafety = new RoundedDouble(3, factorOfSafety);
        }

        /// <summary>
        /// Gets the required probability of the failure mechanism.
        /// </summary>
        public RoundedDouble RequiredProbability { get; private set; }

        /// <summary>
        /// Get the required reliability of the failure mechanism.
        /// </summary>
        public RoundedDouble RequiredReliability { get; private set; }

        /// <summary>
        /// Gets the factor of safety of the failure mechanism.
        /// </summary>
        public RoundedDouble FactorOfSafety { get; private set; }

        /// <summary>
        /// Gets the reliability of the failure mechanism.
        /// </summary>
        public RoundedDouble Reliability { get; private set; }

        /// <summary>
        /// Gets the probability of failing..
        /// </summary>
        public RoundedDouble Probability { get; private set; }
    }
}