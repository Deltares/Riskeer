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
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <remarks>As a part of the constructor, the <paramref name="norm"/> is automatically converted into a reliability index.</remarks>
        protected ReliabilityIndexCalculationInput(long hydraulicBoundaryLocationId, double norm) : base(hydraulicBoundaryLocationId)
        {
            Beta = StatisticsConverter.ProbabilityToReliability(norm);
        }

        public override int CalculationTypeId
        {
            get
            {
                return 9;
            }
        }

        public override double Beta { get; }
    }
}