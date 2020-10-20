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

namespace Riskeer.HydraRing.Calculation.Data.Input
{
    /// <summary>
    /// Container of all data necessary for performing a piping calculation via Hydra-Ring.
    /// </summary>
    public class PipingCalculationInput : ExceedanceProbabilityCalculationInput
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PipingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="sectionLength">The length of the section.</param>
        public PipingCalculationInput(long hydraulicBoundaryLocationId,
                                      double sectionLength)
            : base(hydraulicBoundaryLocationId)
        {
            Section = new HydraRingSection(1, sectionLength, double.NaN);
        }

        public override HydraRingFailureMechanismType FailureMechanismType { get; } = HydraRingFailureMechanismType.Piping;

        public override int VariableId { get; } = 58;

        public override HydraRingSection Section { get; }
    }
}