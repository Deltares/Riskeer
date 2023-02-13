﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using Core.Common.Base.Data;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;

namespace Riskeer.StabilityPointStructures.Data.TestUtil
{
    /// <summary>
    /// Stability point structures calculation scenario used for testing purposes.
    /// </summary>
    public class TestStabilityPointStructuresCalculationScenario : StructuresCalculationScenario<StabilityPointStructuresInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestStabilityPointStructuresCalculationScenario"/>.
        /// </summary>
        public TestStabilityPointStructuresCalculationScenario()
        {
            InputParameters.Structure = new TestStabilityPointStructure();
            InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "location", 1, 1, new HrdFile());
            InputParameters.FactorStormDurationOpenStructure = (RoundedDouble) 1;
            InputParameters.FailureProbabilityStructureWithErosion = 1;
        }
    }
}