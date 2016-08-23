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

namespace Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions
{
    /// <summary>
    /// Container of all data necessary for performing a cosine based wave conditions calculation (Q-variant) via Hydra-Ring.
    /// </summary>
    public class WaveConditionsCosineInput : WaveConditionsInput
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WaveConditionsCosineInput"/> class.
        /// </summary>
        /// <param name="sectionId">The id of the section to use during the calculation.</param>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="hydraRingForelandPoints">The foreland points to use during the calculation.</param>
        /// <param name="hydraRingBreakWater">The break water to use during the calculation.</param>
        public WaveConditionsCosineInput(int sectionId, long hydraulicBoundaryLocationId, double norm,
                                         IEnumerable<HydraRingForelandPoint> hydraRingForelandPoints,
                                         HydraRingBreakWater hydraRingBreakWater)
            : base(sectionId, hydraulicBoundaryLocationId, norm, hydraRingForelandPoints, hydraRingBreakWater) {}

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 4:
                    return 71;
                case 5:
                    return 71;
                default:
                    return null;
            }
        }
    }
}