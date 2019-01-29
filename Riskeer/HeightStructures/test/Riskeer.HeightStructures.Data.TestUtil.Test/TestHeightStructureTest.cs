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
using Ringtoets.Common.Data.Probabilistics;

namespace Riskeer.HeightStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestHeightStructureTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var heightStructure = new TestHeightStructure();

            // Assert
            AssertTestHeightStructure(heightStructure, "name", "id", new Point2D(0, 0));
        }

        [Test]
        public void Constructor_CustomLocation_ExpectedValues()
        {
            // Setup
            var customLocation = new Point2D(10, 20);

            // Call
            var heightStructure = new TestHeightStructure(customLocation);

            // Assert
            AssertTestHeightStructure(heightStructure, "name", "id", customLocation);
        }

        [Test]
        public void Constructor_CustomLocationAndId_ExpectedValues()
        {
            // Setup
            var customLocation = new Point2D(10, 20);
            const string customId = "A different id for structure";

            // Call
            var heightStructure = new TestHeightStructure(customLocation, customId);

            // Assert
            AssertTestHeightStructure(heightStructure, "name", customId, customLocation);
        }

        [Test]
        public void Constructor_CustomId_ExpectedValues()
        {
            // Setup
            const string customId = "A different id";

            // Call
            var heightStructure = new TestHeightStructure(customId);

            // Assert
            var expectedLocation = new Point2D(0, 0);
            AssertTestHeightStructure(heightStructure, "name", customId, expectedLocation);
        }

        [Test]
        public void Constructor_CustomIdAndName_ExpectedValues()
        {
            // Setup
            const string customName = "A different name for structure";
            const string customId = "A different id for structure";

            // Call
            var heightStructure = new TestHeightStructure(customId, customName);

            // Assert
            var expectedLocation = new Point2D(0, 0);
            AssertTestHeightStructure(heightStructure, customName, customId, expectedLocation);
        }

        [Test]
        public void Constructor_CustomNameAndLocationAndId_ExpectedValues()
        {
            // Setup
            const string customName = "A different name for structure";
            const string customId = "A different id for structure";
            var customLocation = new Point2D(10, 20);

            // Call
            var heightStructure = new TestHeightStructure(customName, customId, customLocation);

            // Assert
            AssertTestHeightStructure(heightStructure, customName, customId, customLocation);
        }

        private static void AssertTestHeightStructure(HeightStructure heightStructure, string expectedName,
                                                      string expectedId, Point2D expectedLocation)
        {
            Assert.IsInstanceOf<HeightStructure>(heightStructure);
            Assert.AreEqual(expectedName, heightStructure.Name);
            Assert.AreEqual(expectedId, heightStructure.Id);
            Assert.AreEqual(expectedLocation.X, heightStructure.Location.X);
            Assert.AreEqual(expectedLocation.Y, heightStructure.Location.Y);

            Assert.IsInstanceOf<RoundedDouble>(heightStructure.StructureNormalOrientation);
            Assert.AreEqual(2, heightStructure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.12, heightStructure.StructureNormalOrientation.Value);

            NormalDistribution levelCrestStructure = heightStructure.LevelCrestStructure;
            Assert.AreEqual(2, levelCrestStructure.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(234.57, levelCrestStructure.Mean.Value);
            Assert.AreEqual(2, levelCrestStructure.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, levelCrestStructure.StandardDeviation.Value);

            LogNormalDistribution flowWidthAtBottomProtection = heightStructure.FlowWidthAtBottomProtection;
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(345.68, flowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.35, flowWidthAtBottomProtection.StandardDeviation.Value);

            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = heightStructure.CriticalOvertoppingDischarge;
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(456.79, criticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(2, criticalOvertoppingDischarge.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.46, criticalOvertoppingDischarge.CoefficientOfVariation.Value);

            NormalDistribution widthFlowApertures = heightStructure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, widthFlowApertures.Mean.Value);
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, widthFlowApertures.StandardDeviation.Value);

            Assert.AreEqual(0.67890, heightStructure.FailureProbabilityStructureWithErosion);

            VariationCoefficientLogNormalDistribution storageStructureArea = heightStructure.StorageStructureArea;
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(112.22, storageStructureArea.Mean.Value);
            Assert.AreEqual(2, storageStructureArea.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.11, storageStructureArea.CoefficientOfVariation.Value);

            LogNormalDistribution allowedLevelIncreaseStorage = heightStructure.AllowedLevelIncreaseStorage;
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(225.34, allowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.23, allowedLevelIncreaseStorage.StandardDeviation.Value);
        }
    }
}