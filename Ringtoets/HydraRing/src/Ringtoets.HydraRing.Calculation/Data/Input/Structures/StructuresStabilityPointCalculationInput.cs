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

using System.Collections.Generic;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Structures
{
    /// <summary>
    /// Container of all data necessary for performing a structures stability point calculation via Hydra-Ring.
    /// </summary>
    public abstract class StructuresStabilityPointCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection hydraRingSection;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;

        /// <summary>
        /// Creates a new instance of <see cref="StructuresStabilityPointCalculationInput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="hydraRingSection">The section to use during the calculation.</param>
        /// <param name="forelandPoints">The foreland points to use during the calculation.</param>
        protected StructuresStabilityPointCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                                           IEnumerable<HydraRingForelandPoint> forelandPoints)
            : base(hydraulicBoundaryLocationId)
        {
            this.hydraRingSection = hydraRingSection;
            this.forelandPoints = forelandPoints;
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.StructuresStructuralFailure;
            }
        }

        public override int VariableId
        {
            get
            {
                return 58;
            }
        }

        public override HydraRingSection Section
        {
            get
            {
                return hydraRingSection;
            }
        }

        public override IEnumerable<HydraRingForelandPoint> ForelandsPoints
        {
            get
            {
                return forelandPoints;
            }
        }

        public abstract override int? GetSubMechanismModelId(int subMechanismId);
    }
}