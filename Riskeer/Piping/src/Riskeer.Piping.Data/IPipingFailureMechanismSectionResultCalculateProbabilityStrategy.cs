// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Interface describing a strategy to calculate probabilities.
    /// </summary>
    public interface IPipingFailureMechanismSectionResultCalculateProbabilityStrategy
    {
        /// <summary>
        /// Calculates the probability per profile.
        /// </summary>
        /// <returns>The calculated initial failure mechanism result probability; or <see cref="double.NaN"/> when there
        /// are no relevant calculations, when not all relevant calculations are performed or when the
        /// contribution of the relevant calculations don't add up to 1.</returns>
        double CalculateProfileProbability();

        /// <summary>
        /// Calculates the probability per section.
        /// </summary>
        /// <returns>The calculated initial failure mechanism result probability; or <see cref="double.NaN"/> when there
        /// are no relevant calculations, when not all relevant calculations are performed or when the
        /// contribution of the relevant calculations don't add up to 1.</returns>
        double CalculateSectionProbability();
    }
}