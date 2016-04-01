﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.Calculation.Data.Input.Overtopping
{
    /// <summary>
    /// Container of all data necessary for performing an overtopping calculation via Hydra-Ring.
    /// </summary>
    public class OvertoppingCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection section;
        private readonly IEnumerable<HydraRingProfilePoint> profilePoints;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;

        /// <summary>
        /// Creates a new instance of the <see cref="OvertoppingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="hydraRingSection">The section to use during the calculation.</param>
        /// <param name="hydraRingProfilePoints">The profile points to use during the calculation.</param>
        /// <param name="hydraRingForelandPoints">The foreland points to use during the calculation.</param>
        public OvertoppingCalculationInput(int hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                           IEnumerable<HydraRingProfilePoint> hydraRingProfilePoints,
                                           IEnumerable<HydraRingForelandPoint> hydraRingForelandPoints)
            : base(hydraulicBoundaryLocationId)
        {
            section = hydraRingSection;
            profilePoints = hydraRingProfilePoints;
            forelandPoints = hydraRingForelandPoints;
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.DikesOvertopping;
            }
        }

        public override int VariableId
        {
            get
            {
                return 1;
            }
        }

        public override HydraRingSection Section
        {
            get
            {
                return section;
            }
        }

        public override IEnumerable<HydraRingProfilePoint> ProfilePoints
        {
            get
            {
                return profilePoints;
            }
        }

        public override IEnumerable<HydraRingForelandPoint> ForelandsPoints
        {
            get
            {
                return forelandPoints;
            }
        }

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 102:
                    return 94;
                case 103:
                    return 95;
                default:
                    return null;
            }
        }
    }
}