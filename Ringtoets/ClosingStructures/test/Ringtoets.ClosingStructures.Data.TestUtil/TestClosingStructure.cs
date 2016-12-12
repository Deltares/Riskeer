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

namespace Ringtoets.ClosingStructures.Data.TestUtil
{
    public class TestClosingStructure : ClosingStructure
    {
        public TestClosingStructure()
            : this("test") {}

        public TestClosingStructure(string name)
            : this(name, new Point2D(12345.56789, 9876.54321), ClosingStructureInflowModelType.VerticalWall) {}

        public TestClosingStructure(Point2D location) : this("test", location, ClosingStructureInflowModelType.VerticalWall) {}

        public TestClosingStructure(ClosingStructureInflowModelType type)
            : this("test", new Point2D(12345.56789, 9876.54321), type) {}

        private TestClosingStructure(string name, Point2D location, ClosingStructureInflowModelType type)
            : base(new ConstructionProperties
            {
                Name = name,
                Id = "id",
                Location = location,
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) 20000,
                    CoefficientOfVariation = (RoundedDouble) 0.1
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) 0.2,
                    StandardDeviation = (RoundedDouble) 0.1
                },
                StructureNormalOrientation = 10.0,
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) 21,
                    CoefficientOfVariation = (RoundedDouble) 0.05
                },
                LevelCrestStructureNotClosing =
                {
                    Mean = (RoundedDouble) 4.95,
                    StandardDeviation = (RoundedDouble) 0.05
                },
                InsideWaterLevel =
                {
                    Mean = (RoundedDouble) 0.5,
                    StandardDeviation = (RoundedDouble) 0.1
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = (RoundedDouble) 4.95,
                    StandardDeviation = (RoundedDouble) 0.1
                },
                AreaFlowApertures =
                {
                    Mean = (RoundedDouble) 31.5,
                    StandardDeviation = (RoundedDouble) 0.01
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) 1.0,
                    CoefficientOfVariation = (RoundedDouble) 0.15
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) 25.0,
                    StandardDeviation = (RoundedDouble) 0.05
                },
                ProbabilityOrFrequencyOpenStructureBeforeFlooding = 1.0,
                FailureProbabilityOpenStructure = 0.1,
                IdenticalApertures = 4,
                FailureProbabilityReparation = 1.0,
                InflowModelType = type
            }) {}
    }
}