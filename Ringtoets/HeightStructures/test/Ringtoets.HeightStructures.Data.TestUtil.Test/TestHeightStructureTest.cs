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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.HeightStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestHeightStructureTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var heightStructure = new TestHeightStructure();

            // Assert
            Assert.AreEqual("Test", heightStructure.Name);
            Assert.AreEqual("Id", heightStructure.Id);
            var location = new Point2D(0, 0);
            Assert.AreEqual(location.X, heightStructure.Location.X);
            Assert.AreEqual(location.Y, heightStructure.Location.Y);

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

            VariationCoefficientNormalDistribution widthFlowApertures = heightStructure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, widthFlowApertures.Mean.Value);
            Assert.AreEqual(2, widthFlowApertures.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, widthFlowApertures.CoefficientOfVariation.Value);

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

        [Test]
        public void Constructor_CustomName_ExpectedValues()
        {
            // Setup
            var customName = "A different name for structure";

            // Call
            var heightStructure = new TestHeightStructure(customName);

            // Assert
            Assert.AreEqual(customName, heightStructure.Name);
            Assert.AreEqual("Id", heightStructure.Id);
            var location = new Point2D(0, 0);
            Assert.AreEqual(location.X, heightStructure.Location.X);
            Assert.AreEqual(location.Y, heightStructure.Location.Y);

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

            VariationCoefficientNormalDistribution widthFlowApertures = heightStructure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(567.89, widthFlowApertures.Mean.Value);
            Assert.AreEqual(2, widthFlowApertures.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, widthFlowApertures.CoefficientOfVariation.Value);

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