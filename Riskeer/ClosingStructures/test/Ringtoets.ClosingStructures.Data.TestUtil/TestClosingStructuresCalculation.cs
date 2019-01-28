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

using Core.Common.Base.Data;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.ClosingStructures.Data.TestUtil
{
    /// <summary>
    /// Closing structures calculation used for testing purposes.
    /// </summary>
    public class TestClosingStructuresCalculation : StructuresCalculation<ClosingStructuresInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestClosingStructuresCalculation"/>.
        /// </summary>
        public TestClosingStructuresCalculation()
        {
            InputParameters.Structure = new TestClosingStructure();
            InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "location", 1, 1);
            InputParameters.FactorStormDurationOpenStructure = (RoundedDouble) 1;
            InputParameters.FailureProbabilityStructureWithErosion = 1;
            InputParameters.DeviationWaveDirection = (RoundedDouble) 0;
        }
    }
}