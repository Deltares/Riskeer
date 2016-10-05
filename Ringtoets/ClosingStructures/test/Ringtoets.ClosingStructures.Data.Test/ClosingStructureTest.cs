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
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.ClosingStructures.Data.Test
{
    [TestFixture]
    public class ClosingStructureTest
    {
        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var structure = new ClosingStructure("aName", "anId", location,
                                                 123.456, 0.123,
                                                 234.567, 0.234,
                                                 345.678,
                                                 456.789, 0.456,
                                                 567.890, 0.567,
                                                 678.901, 0.678,
                                                 789.012, 0.789,
                                                 890.123, 0.890,
                                                 901.234, 0.901,
                                                 111.222, 0.111,
                                                 321.987,
                                                 654.321,
                                                 42,
                                                 987.654,
                                                 1);

            // Assert
            Assert.IsInstanceOf<StructureBase>(structure);
            Assert.AreEqual("aName", structure.Name);
            Assert.AreEqual("anId", structure.Id);
            Assert.IsInstanceOf<Point2D>(structure.Location);
            Assert.AreEqual(location.X, structure.Location.X);
            Assert.AreEqual(location.Y, structure.Location.Y);

            var storageStructureArea = structure.StorageStructureArea;
            Assert.IsInstanceOf<LogNormalDistribution>(storageStructureArea);
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(123.46, storageStructureArea.Mean.Value);
            Assert.AreEqual(2, storageStructureArea.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.12, storageStructureArea.StandardDeviation.Value);

            var allowedLevelIncreaseStorage = structure.AllowedLevelIncreaseStorage;
            Assert.IsInstanceOf<LogNormalDistribution>(allowedLevelIncreaseStorage);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(234.57, allowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, allowedLevelIncreaseStorage.StandardDeviation.Value);

            Assert.IsInstanceOf<RoundedDouble>(structure.StructureNormalOrientation);
            Assert.AreEqual(2, structure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.AreEqual(345.68, structure.StructureNormalOrientation.Value);

            var widthFlowApertures = structure.WidthFlowApertures;
            Assert.IsInstanceOf<NormalDistribution>(widthFlowApertures);
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(456.79, widthFlowApertures.Mean.Value);
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.46, widthFlowApertures.StandardDeviation.Value);

            var levelCrestStructureNotClosing = structure.LevelCrestStructureNotClosing;
            Assert.IsInstanceOf<NormalDistribution>(levelCrestStructureNotClosing);
            Assert.AreEqual(2, levelCrestStructureNotClosing.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, levelCrestStructureNotClosing.Mean.Value);
            Assert.AreEqual(2, levelCrestStructureNotClosing.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, levelCrestStructureNotClosing.StandardDeviation.Value);

            var insideWaterLevel = structure.InsideWaterLevel;
            Assert.IsInstanceOf<NormalDistribution>(insideWaterLevel);
            Assert.AreEqual(2, insideWaterLevel.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(678.90, insideWaterLevel.Mean.Value);
            Assert.AreEqual(2, insideWaterLevel.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.68, insideWaterLevel.StandardDeviation.Value);

            var thresholdHeightOpenWeir = structure.ThresholdHeightOpenWeir;
            Assert.IsInstanceOf<NormalDistribution>(thresholdHeightOpenWeir);
            Assert.AreEqual(2, thresholdHeightOpenWeir.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(789.01, thresholdHeightOpenWeir.Mean.Value);
            Assert.AreEqual(2, thresholdHeightOpenWeir.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.79, thresholdHeightOpenWeir.StandardDeviation.Value);

            var areaFlowApertures = structure.AreaFlowApertures;
            Assert.IsInstanceOf<LogNormalDistribution>(areaFlowApertures);
            Assert.AreEqual(2, areaFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(890.12, areaFlowApertures.Mean.Value);
            Assert.AreEqual(2, areaFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.89, areaFlowApertures.StandardDeviation.Value);

            var criticalOvertoppingDischarge = structure.CriticalOvertoppingDischarge;
            Assert.IsInstanceOf<LogNormalDistribution>(criticalOvertoppingDischarge);
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(901.23, criticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(2, criticalOvertoppingDischarge.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.90, criticalOvertoppingDischarge.StandardDeviation.Value);

            var flowWidthAtBottomProtection = structure.FlowWidthAtBottomProtection;
            Assert.IsInstanceOf<LogNormalDistribution>(flowWidthAtBottomProtection);
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(111.22, flowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.11, flowWidthAtBottomProtection.StandardDeviation.Value);

            Assert.IsInstanceOf<RoundedDouble>(structure.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(2, structure.ProbabilityOpenStructureBeforeFlooding.NumberOfDecimalPlaces);
            Assert.AreEqual(321.99, structure.ProbabilityOpenStructureBeforeFlooding.Value);

            Assert.IsInstanceOf<RoundedDouble>(structure.FailureProbablityOpenStructure);
            Assert.AreEqual(2, structure.FailureProbablityOpenStructure.NumberOfDecimalPlaces);
            Assert.AreEqual(654.32, structure.FailureProbablityOpenStructure.Value);

            Assert.AreEqual(42, structure.NumberOfIdenticalApertures);

            Assert.IsInstanceOf<RoundedDouble>(structure.FailureProbabilityReparation);
            Assert.AreEqual(2, structure.FailureProbabilityReparation.NumberOfDecimalPlaces);
            Assert.AreEqual(987.65, structure.FailureProbabilityReparation.Value);

            Assert.AreEqual(1, structure.InflowModel);
        }
    }
}