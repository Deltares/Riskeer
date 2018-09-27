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
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.HeightStructures.Data.TestUtil
{
    /// <summary>
    /// Height structures calculation for testing purposes.
    /// </summary>
    public class TestHeightStructuresCalculation : StructuresCalculation<HeightStructuresInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHeightStructuresCalculation"/>.
        /// </summary>
        public TestHeightStructuresCalculation()
        {
            InputParameters.Structure = new TestHeightStructure();
            InputParameters.LevelCrestStructure.Mean = (RoundedDouble) 5.74;
            InputParameters.LevelCrestStructure.StandardDeviation = (RoundedDouble) 0.94;
            InputParameters.StorageStructureArea.Mean = (RoundedDouble) 1.0;
            InputParameters.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) 1.10;
            InputParameters.StructureNormalOrientation = (RoundedDouble) 115;
            InputParameters.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) 1.0;
            InputParameters.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) 0.12;
            InputParameters.FlowWidthAtBottomProtection.Mean = (RoundedDouble) 18;
            InputParameters.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) 8.2;
            InputParameters.CriticalOvertoppingDischarge.Mean = (RoundedDouble) 1.0;
            InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) 0.88;
            InputParameters.WidthFlowApertures.Mean = (RoundedDouble) 18;
            InputParameters.WidthFlowApertures.StandardDeviation = (RoundedDouble) 2;
            InputParameters.FailureProbabilityStructureWithErosion = 1.0;
            InputParameters.DeviationWaveDirection = (RoundedDouble) 0;
        }
    }
}