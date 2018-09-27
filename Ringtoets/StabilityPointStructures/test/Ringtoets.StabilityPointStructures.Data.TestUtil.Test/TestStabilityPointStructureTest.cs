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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestStabilityPointStructureTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var structure = new TestStabilityPointStructure();

            // Assert
            Assert.IsInstanceOf<StabilityPointStructure>(structure);
            Assert.AreEqual("id", structure.Id);
            Assert.AreEqual("name", structure.Name);
            Assert.AreEqual(new Point2D(1.234, 2.3456), structure.Location);
            AssertStabilityPointStructuresDefault(structure);
        }

        [Test]
        public void Constructor_WithId_ExpectedValues()
        {
            // Setup
            const string id = "cool id";

            // Call
            var structure = new TestStabilityPointStructure(id);

            // Assert
            Assert.IsInstanceOf<StabilityPointStructure>(structure);
            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual("name", structure.Name);
            Assert.AreEqual(new Point2D(1.234, 2.3456), structure.Location);
            AssertStabilityPointStructuresDefault(structure);
        }

        [Test]
        public void Constructor_WithNameAndId_ExpectedValues()
        {
            // Setup
            const string id = "cool Id!";
            const string name = "cool name too!";

            // Call
            var structure = new TestStabilityPointStructure(id, name);

            // Assert
            Assert.IsInstanceOf<StabilityPointStructure>(structure);
            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual(name, structure.Name);
            Assert.AreEqual(new Point2D(1.234, 2.3456), structure.Location);
            AssertStabilityPointStructuresDefault(structure);
        }

        [Test]
        public void Constructor_WithLocation_ExpectedValues()
        {
            // Setup
            var point = new Point2D(1, 2);

            // Call
            var structure = new TestStabilityPointStructure(point);

            // Assert
            Assert.IsInstanceOf<StabilityPointStructure>(structure);
            Assert.AreEqual("id", structure.Id);
            Assert.AreEqual("name", structure.Name);
            Assert.AreEqual(point, structure.Location);
            AssertStabilityPointStructuresDefault(structure);
        }

        [Test]
        public void Constructor_WithLocationAndId_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1234.5, 5678.9);
            const string id = "<some id>";

            // Call
            var structure = new TestStabilityPointStructure(location, id);

            // Assert
            Assert.IsInstanceOf<StabilityPointStructure>(structure);
            Assert.AreEqual("name", structure.Name);
            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual(location, structure.Location);
            AssertStabilityPointStructuresDefault(structure);
        }

        private static void AssertStabilityPointStructuresDefault(TestStabilityPointStructure structure)
        {
            AssertAreEqual(123.456, structure.StructureNormalOrientation);

            AssertAreEqual(234.567, structure.StorageStructureArea.Mean);
            AssertAreEqual(0.234, structure.StorageStructureArea.CoefficientOfVariation);

            AssertAreEqual(345.678, structure.AllowedLevelIncreaseStorage.Mean);
            AssertAreEqual(0.35, structure.AllowedLevelIncreaseStorage.StandardDeviation);

            AssertAreEqual(456.789, structure.WidthFlowApertures.Mean);
            AssertAreEqual(0.456, structure.WidthFlowApertures.StandardDeviation);

            AssertAreEqual(567.890, structure.InsideWaterLevel.Mean);
            AssertAreEqual(0.567, structure.InsideWaterLevel.StandardDeviation);

            AssertAreEqual(678.901, structure.ThresholdHeightOpenWeir.Mean);
            AssertAreEqual(0.678, structure.ThresholdHeightOpenWeir.StandardDeviation);

            AssertAreEqual(789.012, structure.CriticalOvertoppingDischarge.Mean);
            AssertAreEqual(0.789, structure.CriticalOvertoppingDischarge.CoefficientOfVariation);

            AssertAreEqual(890.123, structure.FlowWidthAtBottomProtection.Mean);
            AssertAreEqual(0.890, structure.FlowWidthAtBottomProtection.StandardDeviation);

            AssertAreEqual(901.234, structure.ConstructiveStrengthLinearLoadModel.Mean);
            AssertAreEqual(0.901, structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation);

            AssertAreEqual(123.456, structure.ConstructiveStrengthQuadraticLoadModel.Mean);
            AssertAreEqual(0.123, structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation);

            AssertAreEqual(234.567, structure.BankWidth.Mean);
            AssertAreEqual(0.234, structure.BankWidth.StandardDeviation);

            AssertAreEqual(345.678, structure.InsideWaterLevelFailureConstruction.Mean);
            AssertAreEqual(0.35, structure.InsideWaterLevelFailureConstruction.StandardDeviation);

            AssertAreEqual(555.555, structure.EvaluationLevel);

            AssertAreEqual(456.789, structure.LevelCrestStructure.Mean);
            AssertAreEqual(0.456, structure.LevelCrestStructure.StandardDeviation);

            AssertAreEqual(555.55, structure.VerticalDistance);
            Assert.AreEqual(0.55, structure.FailureProbabilityRepairClosure);

            AssertAreEqual(567.890, structure.FailureCollisionEnergy.Mean);
            AssertAreEqual(0.567, structure.FailureCollisionEnergy.CoefficientOfVariation);

            AssertAreEqual(7777777.777, structure.ShipMass.Mean);
            AssertAreEqual(0.777, structure.ShipMass.CoefficientOfVariation);

            AssertAreEqual(567.890, structure.ShipVelocity.Mean);
            AssertAreEqual(0.567, structure.ShipVelocity.CoefficientOfVariation);

            Assert.AreEqual(42, structure.LevellingCount);
            Assert.AreEqual(0.55, structure.ProbabilityCollisionSecondaryStructure);

            AssertAreEqual(678.901, structure.FlowVelocityStructureClosable.Mean);
            AssertAreEqual(0.2, structure.FlowVelocityStructureClosable.CoefficientOfVariation);

            AssertAreEqual(789.012, structure.StabilityLinearLoadModel.Mean);
            AssertAreEqual(0.789, structure.StabilityLinearLoadModel.CoefficientOfVariation);

            AssertAreEqual(890.123, structure.StabilityQuadraticLoadModel.Mean);
            AssertAreEqual(0.890, structure.StabilityQuadraticLoadModel.CoefficientOfVariation);

            AssertAreEqual(901.234, structure.AreaFlowApertures.Mean);
            AssertAreEqual(0.901, structure.AreaFlowApertures.StandardDeviation);

            Assert.AreEqual(StabilityPointStructureInflowModelType.FloodedCulvert, structure.InflowModelType);
        }

        private static void AssertAreEqual(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }
    }
}