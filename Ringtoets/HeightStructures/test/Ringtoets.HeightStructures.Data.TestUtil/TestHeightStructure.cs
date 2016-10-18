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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Ringtoets.HeightStructures.Data.TestUtil
{
    public class TestHeightStructure : HeightStructure
    {
        public TestHeightStructure()
            : this("Test") {}

        public TestHeightStructure(Point2D location)
            : this("Test", location) {}

        public TestHeightStructure(string name)
            : this(name, new Point2D(0.0, 0.0)) {}

        public TestHeightStructure(string name, Point2D location)
            : base(new ConstructionProperties
            {
                Name = name,
                Id = "Id",
                Location = location,
                StructureNormalOrientation = 0.12345,
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) 234.567,
                    StandardDeviation = (RoundedDouble) 0.23456
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) 345.678,
                    StandardDeviation = (RoundedDouble) 0.34567
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) 456.789,
                    CoefficientOfVariation = (RoundedDouble) 0.45678
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) 567.890,
                    CoefficientOfVariation = (RoundedDouble) 0.56789
                },
                FailureProbabilityStructureWithErosion = 0.67890,
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) 112.223,
                    CoefficientOfVariation = (RoundedDouble) 0.11222
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) 225.336,
                    StandardDeviation = (RoundedDouble) 0.22533
                }
            }) {}
    }
}