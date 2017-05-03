// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
            var heightStructure = new HeightStructure(
                new HeightStructure.ConstructionProperties
                {
                    Name = "aName", Id = "anId", Location = location,
                    StructureNormalOrientation = (RoundedDouble) 0.12345,
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
                        StandardDeviation = (RoundedDouble) 0.56789
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
                });

            // Assert
            Assert.IsInstanceOf<StructureBase>(heightStructure);
            Assert.AreEqual("aName", heightStructure.Name);
            Assert.AreEqual("anId", heightStructure.Id);
            Assert.IsInstanceOf<Point2D>(heightStructure.Location);
            Assert.AreEqual(location.X, heightStructure.Location.X);
            Assert.AreEqual(location.Y, heightStructure.Location.Y);

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

        [Test]
        public void Constructor_DefaultConstructorProperties_ExpectedValues()
        {
            // Setup
            var location = new Point2D(1.22, 2.333);

            // Call
            var heightStructure = new HeightStructure(
                new HeightStructure.ConstructionProperties
                {
                    Name = "aName",
                    Id = "anId",
                    Location = location
                });

            // Assert
            Assert.IsInstanceOf<StructureBase>(heightStructure);
            Assert.AreEqual("aName", heightStructure.Name);
            Assert.AreEqual("anId", heightStructure.Id);
            Assert.IsInstanceOf<Point2D>(heightStructure.Location);
            Assert.AreEqual(location.X, heightStructure.Location.X);
            Assert.AreEqual(location.Y, heightStructure.Location.Y);

            Assert.AreEqual(2, heightStructure.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.IsNaN(heightStructure.StructureNormalOrientation);

            NormalDistribution levelCrestStructure = heightStructure.LevelCrestStructure;
            Assert.AreEqual(2, levelCrestStructure.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, levelCrestStructure.Mean.Value);
            Assert.AreEqual(2, levelCrestStructure.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.05, levelCrestStructure.StandardDeviation.Value);

            LogNormalDistribution flowWidthAtBottomProtection = heightStructure.FlowWidthAtBottomProtection;
            Assert.AreEqual(2, flowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, flowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(2, flowWidthAtBottomProtection.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.05, flowWidthAtBottomProtection.StandardDeviation.Value);

            VariationCoefficientLogNormalDistribution criticalOvertoppingDischarge = heightStructure.CriticalOvertoppingDischarge;
            Assert.AreEqual(2, criticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, criticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(2, criticalOvertoppingDischarge.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.15, criticalOvertoppingDischarge.CoefficientOfVariation.Value);

            NormalDistribution widthFlowApertures = heightStructure.WidthFlowApertures;
            Assert.AreEqual(2, widthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, widthFlowApertures.Mean.Value);
            Assert.AreEqual(2, widthFlowApertures.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.2, widthFlowApertures.StandardDeviation.Value);

            Assert.AreEqual(1, heightStructure.FailureProbabilityStructureWithErosion);

            VariationCoefficientLogNormalDistribution storageStructureArea = heightStructure.StorageStructureArea;
            Assert.AreEqual(2, storageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, storageStructureArea.Mean.Value);
            Assert.AreEqual(2, storageStructureArea.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, storageStructureArea.CoefficientOfVariation.Value);

            LogNormalDistribution allowedLevelIncreaseStorage = heightStructure.AllowedLevelIncreaseStorage;
            Assert.AreEqual(2, allowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, allowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(2, allowedLevelIncreaseStorage.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.1, allowedLevelIncreaseStorage.StandardDeviation.Value);
        }

        [Test]
        public void CopyProperties_FromStructureNull_ThrowsArgumentNullException()
        {
            // Setup
            var structure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0)
            });

            // Call
            TestDelegate call = () => structure.CopyProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("fromStructure", paramName);
        }

        [Test]
        public void CopyProperties_FromStructure_UpdatesProperties()
        {
            // Setup
            var structure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "aName",
                Id = "anId",
                Location = new Point2D(0, 0)
            });

            var otherStructure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "otherName",
                Id = "otherId",
                Location = new Point2D(1, 1)
            });

            // Call
            structure.CopyProperties(otherStructure);

            // Assert
            Assert.AreNotEqual(structure.Id, otherStructure.Id);
            Assert.AreEqual(structure.Name, otherStructure.Name);
            Assert.AreEqual(structure.Location, otherStructure.Location);
            Assert.AreEqual(structure.StructureNormalOrientation, otherStructure.StructureNormalOrientation);

            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.Mean, structure.AllowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.StandardDeviation, structure.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.Shift, structure.AllowedLevelIncreaseStorage.Shift);
                            
            Assert.AreEqual(structure.CriticalOvertoppingDischarge.Mean, structure.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge.CoefficientOfVariation, structure.CriticalOvertoppingDischarge.CoefficientOfVariation);
                            
            Assert.AreEqual(structure.FailureProbabilityStructureWithErosion, structure.FailureProbabilityStructureWithErosion);
                            
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.Mean, structure.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.StandardDeviation, structure.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.Shift, structure.FlowWidthAtBottomProtection.Shift);
                            
            Assert.AreEqual(structure.LevelCrestStructure.Mean, structure.LevelCrestStructure.Mean);
            Assert.AreEqual(structure.LevelCrestStructure.StandardDeviation, structure.LevelCrestStructure.StandardDeviation);
                            
            Assert.AreEqual(structure.StorageStructureArea.Mean, structure.StorageStructureArea.Mean);
            Assert.AreEqual(structure.StorageStructureArea.CoefficientOfVariation, structure.StorageStructureArea.CoefficientOfVariation);
                            
            Assert.AreEqual(structure.WidthFlowApertures.Mean, structure.WidthFlowApertures.Mean);
            Assert.AreEqual(structure.WidthFlowApertures.StandardDeviation, structure.WidthFlowApertures.StandardDeviation);
        }
    }
}