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

using System;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Riskeer.StabilityPointStructures.Data.TestUtil
{
    /// <summary>
    /// <see cref="StabilityPointStructure"/> used for testing purposes.
    /// </summary>
    public class TestStabilityPointStructure : StabilityPointStructure
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestStabilityPointStructure"/>.
        /// </summary>
        public TestStabilityPointStructure() : this("name", "id", new Point2D(1.234, 2.3456)) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStabilityPointStructure"/>.
        /// </summary>
        /// <param name="id">The name of the structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/>
        /// is <c>null</c>, empty or consists of only whitespaces.</exception>
        public TestStabilityPointStructure(string id) : this("name", id, new Point2D(1.234, 2.3456)) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStabilityPointStructure"/>.
        /// </summary>
        /// <param name="id">The id of the structure.</param>
        /// <param name="name">The name of the structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/>
        /// or <paramref name="id"/> is <c>null</c>, empty or consists of only whitespaces.</exception>
        public TestStabilityPointStructure(string id, string name) : this(name, id, new Point2D(1.234, 2.3456)) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStabilityPointStructure"/>.
        /// </summary>
        /// <param name="location">The location of the structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> 
        /// is <c>null</c>.</exception>
        public TestStabilityPointStructure(Point2D location) : this("name", "id", location) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStabilityPointStructure"/>.
        /// </summary>
        /// <param name="location">The location of the structure.</param>
        /// <param name="id">The id of the structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> 
        /// is <c>null</c>, empty or consists of only whitespaces.</exception>
        public TestStabilityPointStructure(Point2D location, string id)
            : this("name", id, location) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStabilityPointStructure"/>.
        /// </summary>
        /// <param name="name">The name of the structure.</param>
        /// <param name="id"></param>
        /// <param name="location">The location of the structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> 
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/>
        /// or <paramref name="id"/> is <c>null</c>, empty or consists of only whitespaces.</exception>
        private TestStabilityPointStructure(string name, string id, Point2D location)
            : base(new ConstructionProperties
            {
                Name = name,
                Id = id,
                Location = location,
                StructureNormalOrientation = (RoundedDouble) 123.456,
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
                    StandardDeviation = (RoundedDouble) 0.456
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
                    CoefficientOfVariation = (RoundedDouble) 0.2
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