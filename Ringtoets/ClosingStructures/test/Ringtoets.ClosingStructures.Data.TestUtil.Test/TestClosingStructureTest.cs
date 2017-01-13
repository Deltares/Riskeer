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

using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.ClosingStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestClosingStructureTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            ClosingStructure structure = new TestClosingStructure();

            // Assert
            Assert.AreEqual("test", structure.Name);
            Assert.AreEqual(new Point2D(12345.56789, 9876.54321), structure.Location);
            Assert.AreEqual(ClosingStructureInflowModelType.VerticalWall, structure.InflowModelType);
            AssertTestClosingStructureDefaults(structure);
        }

        [Test]
        public void Constructor_WithStructureName_ExpectedValues()
        {
            // Setup
            const string name = "<some name>";

            // Call
            ClosingStructure structure = new TestClosingStructure(name);

            // Assert
            Assert.AreEqual(name, structure.Name);
            Assert.AreEqual(new Point2D(12345.56789, 9876.54321), structure.Location);
            Assert.AreEqual(ClosingStructureInflowModelType.VerticalWall, structure.InflowModelType);
            AssertTestClosingStructureDefaults(structure);
        }

        [Test]
        public void Constructor_WithLocation_ExpectedValues()
        {
            // Setup
            Point2D location = new Point2D(1234.5, 5678.9);

            // Call
            ClosingStructure structure = new TestClosingStructure(location);

            // Assert
            Assert.AreEqual("test", structure.Name);
            Assert.AreEqual(location, structure.Location);
            Assert.AreEqual(ClosingStructureInflowModelType.VerticalWall, structure.InflowModelType);
            AssertTestClosingStructureDefaults(structure);
        }

        [Test]
        [TestCase(ClosingStructureInflowModelType.VerticalWall)]
        [TestCase(ClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert)]
        public void Constructor_WithLocationName_ExpectedValues(ClosingStructureInflowModelType type)
        {
            // Call
            ClosingStructure structure = new TestClosingStructure(type);

            // Assert
            Assert.AreEqual("test", structure.Name);
            Assert.AreEqual(new Point2D(12345.56789, 9876.54321), structure.Location);
            Assert.AreEqual(type, structure.InflowModelType);
            AssertTestClosingStructureDefaults(structure);
        }

        private static void AssertTestClosingStructureDefaults(ClosingStructure structure)
        {
            Assert.AreEqual("id", structure.Id);

            Assert.AreEqual(10.0, structure.StructureNormalOrientation.Value);

            Assert.AreEqual(20000, structure.StorageStructureArea.Mean.Value);
            Assert.AreEqual(0.1, structure.StorageStructureArea.CoefficientOfVariation.Value);

            Assert.AreEqual(0.2, structure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(0.1, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);

            Assert.AreEqual(21, structure.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(0.05, structure.WidthFlowApertures.StandardDeviation.Value);

            Assert.AreEqual(4.95, structure.LevelCrestStructureNotClosing.Mean.Value);
            Assert.AreEqual(0.05, structure.LevelCrestStructureNotClosing.StandardDeviation.Value);

            Assert.AreEqual(0.5, structure.InsideWaterLevel.Mean.Value);
            Assert.AreEqual(0.1, structure.InsideWaterLevel.StandardDeviation.Value);

            Assert.AreEqual(4.95, structure.ThresholdHeightOpenWeir.Mean.Value);
            Assert.AreEqual(0.1, structure.ThresholdHeightOpenWeir.StandardDeviation.Value);

            Assert.AreEqual(31.5, structure.AreaFlowApertures.Mean.Value);
            Assert.AreEqual(0.01, structure.AreaFlowApertures.StandardDeviation.Value);

            Assert.AreEqual(1.0, structure.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(0.15, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);

            Assert.AreEqual(25.0, structure.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(0.05, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);

            Assert.AreEqual(1.0, structure.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
            Assert.AreEqual(0.1, structure.FailureProbabilityOpenStructure);
            Assert.AreEqual(4, structure.IdenticalApertures);
            Assert.AreEqual(1.0, structure.FailureProbabilityReparation);
        }
    }
}