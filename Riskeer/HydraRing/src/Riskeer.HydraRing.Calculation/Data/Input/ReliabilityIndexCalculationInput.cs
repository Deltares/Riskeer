// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Core.Common.Util;

namespace Riskeer.HydraRing.Calculation.Data.Input
{
    /// <summary>
    /// Container of all data necessary for performing Hydra-Ring calculations that
    /// iterate towards a reliability index.
    /// </summary>
    public abstract class ReliabilityIndexCalculationInput : HydraRingCalculationInput
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ReliabilityIndexCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location to use during the calculation.</param>
        /// <param name="targetProbability">The target probability to use during the calculation.</param>
        /// <remarks>As a part of the constructor, the <paramref name="targetProbability"/> is automatically converted into a reliability index.</remarks>
        protected ReliabilityIndexCalculationInput(long hydraulicBoundaryLocationId, double targetProbability) : base(hydraulicBoundaryLocationId)
        {
            Beta = StatisticsConverter.ProbabilityToReliability(targetProbability);
        }

        public override int CalculationTypeId => 9;

        public override double Beta { get; }
    }
}