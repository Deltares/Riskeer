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

namespace Ringtoets.StabilityPointStructures.Data.TestUtil
{
    public class TestStabilityPointStructure : StabilityPointStructure
    {
        public TestStabilityPointStructure() : this("aName"){}

        public TestStabilityPointStructure(string name)
            : base(new ConstructionProperties
            {
                Name = name,
                Id = "anId",
                Location = new Point2D(1.234, 2.3456),
                StructureNormalOrientation = 123.456,
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) 234.567,
                    CoefficientOfVariation = (RoundedDouble) 0.234
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) 345.678,
                    StandardDeviation = (RoundedDouble) 0.345
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) 456.789,
                    CoefficientOfVariation = (RoundedDouble) 0.456
                },
                InsideWaterLevel =
                {
                    Mean = (RoundedDouble) 567.890,
                    StandardDeviation = (RoundedDouble) 0.567
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = (RoundedDouble) 678.901,
                    StandardDeviation = (RoundedDouble) 0.678
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) 789.012,
                    CoefficientOfVariation = (RoundedDouble) 0.789
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) 890.123,
                    StandardDeviation = (RoundedDouble) 0.890
                },
                ConstructiveStrengthLinearLoadModel =
                {
                    Mean = (RoundedDouble) 901.234,
                    CoefficientOfVariation = (RoundedDouble) 0.901
                },
                ConstructiveStrengthQuadraticLoadModel =
                {
                    Mean = (RoundedDouble) 123.456,
                    CoefficientOfVariation = (RoundedDouble) 0.123
                },
                BankWidth =
                {
                    Mean = (RoundedDouble) 234.567,
                    StandardDeviation = (RoundedDouble) 0.234
                },
                InsideWaterLevelFailureConstruction =
                {
                    Mean = (RoundedDouble) 345.678,
                    StandardDeviation = (RoundedDouble) 0.345
                },
                EvaluationLevel = 555.555,
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) 456.789,
                    StandardDeviation = (RoundedDouble) 0.456
                },
                VerticalDistance = 555.55,
                FailureProbabilityRepairClosure = 0.55,
                FailureCollisionEnergy =
                {
                    Mean = (RoundedDouble) 567.890,
                    CoefficientOfVariation = (RoundedDouble) 0.567
                },
                ShipMass =
                {
                    Mean = (RoundedDouble) 7777777.777,
                    CoefficientOfVariation = (RoundedDouble) 0.777
                },
                ShipVelocity =
                {
                    Mean = (RoundedDouble) 567.890,
                    CoefficientOfVariation = (RoundedDouble) 0.567
                },
                LevellingCount = 42,
                ProbabilityCollisionSecondaryStructure = 0.55,
                FlowVelocityStructureClosable =
                {
                    Mean = (RoundedDouble) 678.901,
                    StandardDeviation = (RoundedDouble) 0.678
                },
                StabilityLinearLoadModel =
                {
                    Mean = (RoundedDouble) 789.012,
                    CoefficientOfVariation = (RoundedDouble) 0.789
                },
                StabilityQuadraticLoadModel =
                {
                    Mean = (RoundedDouble) 890.123,
                    CoefficientOfVariation = (RoundedDouble) 0.890
                },
                AreaFlowApertures =
                {
                    Mean = (RoundedDouble) 901.234,
                    StandardDeviation = (RoundedDouble) 0.901
                },
                InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert
            }) {}
    }
}