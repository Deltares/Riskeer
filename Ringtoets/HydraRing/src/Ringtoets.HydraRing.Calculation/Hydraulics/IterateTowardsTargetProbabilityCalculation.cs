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

namespace Ringtoets.HydraRing.Calculation.Hydraulics
{
    /// <summary>
    /// Container of all data necessary for performing a type-2 calculation via Hydra-Ring ("iterate towards a target probability, provided as reliability index").
    /// </summary>
    public abstract class IterateTowardsTargetProbabilityCalculation : HydraRingCalculation
    {
        private readonly double beta;

        /// <summary>
        /// Creates a new instance of the <see cref="IterateTowardsTargetProbabilityCalculation"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="beta">The reliability index to use during the calculation.</param>
        protected IterateTowardsTargetProbabilityCalculation(int hydraulicBoundaryLocationId, double beta) : base(hydraulicBoundaryLocationId)
        {
            this.beta = beta;
        }

        public override int CalculationTypeId
        {
            get
            {
                return 2;
            }
        }

        public override double Beta
        {
            get
            {
                return beta;
            }
        }
    }
}