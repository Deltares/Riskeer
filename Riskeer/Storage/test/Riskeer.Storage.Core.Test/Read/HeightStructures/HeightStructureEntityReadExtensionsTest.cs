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
using NUnit.Framework;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.HeightStructures;

namespace Riskeer.Storage.Core.Test.Read.HeightStructures
{
    [TestFixture]
    public class HeightStructureEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new HeightStructureEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_EntityNotReadBefore_RegisterEntity()
        {
            // Setup
            var entity = new HeightStructureEntity
            {
                Name = "name",
                Id = "id"
            };

            var collector = new ReadConversionCollector();

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            HeightStructure calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
            Assert.AreSame(calculation, collector.Get(entity));
        }

        [Test]
        public void Read_ValidEntity_ReturnHeightStructure()
        {
            // Setup
            var entity = new HeightStructureEntity
            {
                Name = "A",
                Id = "B",
                X = 1.1,
                Y = 2.2,
                StructureNormalOrientation = 3.3,
                LevelCrestStructureMean = 4.4,
                LevelCrestStructureStandardDeviation = 5.5,
                FlowWidthAtBottomProtectionMean = 6.6,
                FlowWidthAtBottomProtectionStandardDeviation = 7.7,
                CriticalOvertoppingDischargeMean = 8.8,
                CriticalOvertoppingDischargeCoefficientOfVariation = 9.9,
                WidthFlowAperturesMean = 10.10,
                WidthFlowAperturesStandardDeviation = 11.11,
                FailureProbabilityStructureWithErosion = 12.12,
                StorageStructureAreaMean = 13.13,
                StorageStructureAreaCoefficientOfVariation = 14.14,
                AllowedLevelIncreaseStorageMean = 15.15,
                AllowedLevelIncreaseStorageStandardDeviation = 16.16
            };

            var collector = new ReadConversionCollector();

            // Call
            HeightStructure structure = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Name, structure.Name);
            Assert.AreEqual(entity.Id, structure.Id);
            Assert.AreEqual(entity.X, structure.Location.X);
            Assert.AreEqual(entity.Y, structure.Location.Y);
            Assert.AreEqual(entity.StructureNormalOrientation, structure.StructureNormalOrientation.Value);

            Assert.AreEqual(entity.LevelCrestStructureMean, structure.LevelCrestStructure.Mean.Value);
            Assert.AreEqual(entity.LevelCrestStructureStandardDeviation, structure.LevelCrestStructure.StandardDeviation.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionMean, structure.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionStandardDeviation, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeMean, structure.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeCoefficientOfVariation, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.WidthFlowAperturesMean, structure.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(entity.WidthFlowAperturesStandardDeviation, structure.WidthFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(entity.FailureProbabilityStructureWithErosion, structure.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(entity.StorageStructureAreaMean, structure.StorageStructureArea.Mean.Value);
            Assert.AreEqual(entity.StorageStructureAreaCoefficientOfVariation, structure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageMean, structure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageStandardDeviation, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
        }

        [Test]
        public void Read_NullValues_ReturnHeightStructureWithNaN()
        {
            // Setup
            var entity = new HeightStructureEntity
            {
                Name = "A",
                Id = "B",
                X = null,
                Y = null,
                StructureNormalOrientation = null,
                LevelCrestStructureMean = null,
                LevelCrestStructureStandardDeviation = null,
                FlowWidthAtBottomProtectionMean = null,
                FlowWidthAtBottomProtectionStandardDeviation = null,
                CriticalOvertoppingDischargeMean = null,
                CriticalOvertoppingDischargeCoefficientOfVariation = null,
                WidthFlowAperturesMean = null,
                WidthFlowAperturesStandardDeviation = null,
                FailureProbabilityStructureWithErosion = null,
                StorageStructureAreaMean = null,
                StorageStructureAreaCoefficientOfVariation = null,
                AllowedLevelIncreaseStorageMean = null,
                AllowedLevelIncreaseStorageStandardDeviation = null
            };

            var collector = new ReadConversionCollector();

            // Call
            HeightStructure structure = entity.Read(collector);

            // Assert
            Assert.IsNaN(structure.Location.X);
            Assert.IsNaN(structure.Location.Y);
            Assert.IsNaN(structure.StructureNormalOrientation);

            Assert.IsNaN(structure.LevelCrestStructure.Mean.Value);
            Assert.IsNaN(structure.LevelCrestStructure.StandardDeviation.Value);
            Assert.IsNaN(structure.FlowWidthAtBottomProtection.Mean.Value);
            Assert.IsNaN(structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.IsNaN(structure.CriticalOvertoppingDischarge.Mean.Value);
            Assert.IsNaN(structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.IsNaN(structure.WidthFlowApertures.Mean.Value);
            Assert.IsNaN(structure.WidthFlowApertures.StandardDeviation.Value);
            Assert.IsNaN(structure.FailureProbabilityStructureWithErosion);
            Assert.IsNaN(structure.StorageStructureArea.Mean.Value);
            Assert.IsNaN(structure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.IsNaN(structure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.IsNaN(structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
        }

        [Test]
        public void Read_EntityRegistered_ReturnRegisteredStructure()
        {
            // Setup
            var entity = new HeightStructureEntity();
            HeightStructure registeredStructure = new TestHeightStructure();
            var collector = new ReadConversionCollector();
            collector.Read(entity, registeredStructure);

            // Call
            HeightStructure readStructure = entity.Read(collector);

            // Assert
            Assert.AreSame(registeredStructure, readStructure);
        }
    }
}