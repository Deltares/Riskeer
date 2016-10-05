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

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructureTest
    {
        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var heightStructure = new HeightStructure("aName", "anId", location, 
                                                      0.12345, 
                                                      234.567, 0.23456, 
                                                      345.678, 0.34567, 
                                                      456.789, 0.45678, 
                                                      567.890, 0.56789, 
                                                      0.67890, 
                                                      112.223, 0.11222, 
                                                      225.336, 0.22533);

            // Assert
            Assert.IsInstanceOf<StructureBase>(heightStructure);
            Assert.AreEqual("aName", heightStructure.Name);
            Assert.AreEqual("anId", heightStructure.Id);
            Assert.IsInstanceOf<Point2D>(heightStructure.Location);
            Assert.AreEqual(location.X, heightStructure.Location.X);
            Assert.AreEqual(location.Y, heightStructure.Location.Y);

            Assert.IsInstanceOf<RoundedDouble>(heightStructure.StructureNormalOrientation);
            Assert.AreEqual(2, heightStructure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.12, heightStructure.StructureNormalOrientation.Value);

            var levelCrestStructure = heightStructure.LevelCrestStructure;
            Assert.IsInstanceOf<NormalDistribution>(levelCrestStructure);
            Assert.AreEqual(2, levelCrestStructure.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(234.57, levelCrestStructure.Mean.Value);
            Assert.AreEqual(2, levelCrestStructure.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, levelCrestStructure.StandardDeviation.Value);

            var flowWidthAtBottomProtection = heightStructure.FlowWidthAtBottomProtection;
            Assert.IsInstanceOf<LogNormalDistribution>(flowWidthAtBottomProtection);
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(345.68, flowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.35, flowWidthAtBottomProtection.StandardDeviation.Value);

            var criticalOvertoppingDischarge = heightStructure.CriticalOvertoppingDischarge;
            Assert.IsInstanceOf<LogNormalDistribution>(criticalOvertoppingDischarge);
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(456.79, criticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(2, criticalOvertoppingDischarge.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.46, criticalOvertoppingDischarge.StandardDeviation.Value);

            var widthOfFlowApertures = heightStructure.WidthOfFlowApertures;
            Assert.IsInstanceOf<NormalDistribution>(widthOfFlowApertures);
            Assert.AreEqual(2, widthOfFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, widthOfFlowApertures.Mean.Value);
            Assert.AreEqual(2, widthOfFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, widthOfFlowApertures.StandardDeviation.Value);

            Assert.AreEqual(0.67890, heightStructure.FailureProbabilityStructureWithErosion);

            var storageStructureArea = heightStructure.StorageStructureArea;
            Assert.IsInstanceOf<LogNormalDistribution>(storageStructureArea);
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(112.22, storageStructureArea.Mean.Value);
            Assert.AreEqual(2, storageStructureArea.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.11, storageStructureArea.StandardDeviation.Value);

            var allowedLevelIncreaseStorage = heightStructure.AllowedLevelIncreaseStorage;
            Assert.IsInstanceOf<LogNormalDistribution>(allowedLevelIncreaseStorage);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(225.34, allowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, allowedLevelIncreaseStorage.StandardDeviation.Value);
        }
    }
}
