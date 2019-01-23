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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.ClosingStructures;

namespace Ringtoets.Storage.Core.Test.Read.ClosingStructures
{
    [TestFixture]
    public class ClosingStructureEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new ClosingStructureEntity();

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
            var entity = new ClosingStructureEntity
            {
                Name = "name",
                Id = "id"
            };

            var collector = new ReadConversionCollector();

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            ClosingStructure calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
            Assert.AreSame(calculation, collector.Get(entity));
        }

        [Test]
        public void Read_ValidEntity_ReturnClosingStructure()
        {
            // Setup
            var entity = new ClosingStructureEntity
            {
                Name = "A",
                Id = "B",
                X = 1.1,
                Y = 2.2,
                StructureNormalOrientation = 3.3,
                StorageStructureAreaMean = 4.4,
                StorageStructureAreaCoefficientOfVariation = 5.5,
                AllowedLevelIncreaseStorageMean = 6.6,
                AllowedLevelIncreaseStorageStandardDeviation = 7.7,
                WidthFlowAperturesMean = 8.8,
                WidthFlowAperturesStandardDeviation = 9.9,
                LevelCrestStructureNotClosingMean = 10.10,
                LevelCrestStructureNotClosingStandardDeviation = 11.11,
                InsideWaterLevelMean = 12.12,
                InsideWaterLevelStandardDeviation = 13.13,
                ThresholdHeightOpenWeirMean = 14.14,
                ThresholdHeightOpenWeirStandardDeviation = 15.15,
                AreaFlowAperturesMean = 16.16,
                AreaFlowAperturesStandardDeviation = 17.17,
                CriticalOvertoppingDischargeMean = 18.18,
                CriticalOvertoppingDischargeCoefficientOfVariation = 19.19,
                FlowWidthAtBottomProtectionMean = 20.20,
                FlowWidthAtBottomProtectionStandardDeviation = 21.21,
                ProbabilityOpenStructureBeforeFlooding = 22.22,
                FailureProbabilityOpenStructure = 23.23,
                IdenticalApertures = 24,
                FailureProbabilityReparation = 25.25,
                InflowModelType = Convert.ToByte(ClosingStructureInflowModelType.FloodedCulvert)
            };

            var collector = new ReadConversionCollector();

            // Call
            ClosingStructure structure = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Name, structure.Name);
            Assert.AreEqual(entity.Id, structure.Id);
            Assert.AreEqual(entity.X, structure.Location.X);
            Assert.AreEqual(entity.Y, structure.Location.Y);
            Assert.AreEqual(entity.StructureNormalOrientation, structure.StructureNormalOrientation.Value);
            Assert.AreEqual(entity.StorageStructureAreaMean, structure.StorageStructureArea.Mean.Value);
            Assert.AreEqual(entity.StorageStructureAreaCoefficientOfVariation, structure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageMean, structure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageStandardDeviation, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
            Assert.AreEqual(entity.WidthFlowAperturesMean, structure.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(entity.WidthFlowAperturesStandardDeviation, structure.WidthFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(entity.LevelCrestStructureNotClosingMean, structure.LevelCrestStructureNotClosing.Mean.Value);
            Assert.AreEqual(entity.LevelCrestStructureNotClosingStandardDeviation, structure.LevelCrestStructureNotClosing.StandardDeviation.Value);
            Assert.AreEqual(entity.InsideWaterLevelMean, structure.InsideWaterLevel.Mean.Value);
            Assert.AreEqual(entity.InsideWaterLevelStandardDeviation, structure.InsideWaterLevel.StandardDeviation.Value);
            Assert.AreEqual(entity.ThresholdHeightOpenWeirMean, structure.ThresholdHeightOpenWeir.Mean.Value);
            Assert.AreEqual(entity.ThresholdHeightOpenWeirStandardDeviation, structure.ThresholdHeightOpenWeir.StandardDeviation.Value);
            Assert.AreEqual(entity.AreaFlowAperturesMean, structure.AreaFlowApertures.Mean.Value);
            Assert.AreEqual(entity.AreaFlowAperturesStandardDeviation, structure.AreaFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeMean, structure.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeCoefficientOfVariation, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionMean, structure.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionStandardDeviation, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(entity.ProbabilityOpenStructureBeforeFlooding, structure.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(entity.FailureProbabilityOpenStructure, structure.FailureProbabilityOpenStructure);
            Assert.AreEqual(entity.IdenticalApertures, structure.IdenticalApertures);
            Assert.AreEqual(entity.FailureProbabilityReparation, structure.FailureProbabilityReparation);
            Assert.AreEqual((ClosingStructureInflowModelType) entity.InflowModelType, structure.InflowModelType);

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_NullValues_ReturnClosingStructureWithNaN()
        {
            // Setup
            var entity = new ClosingStructureEntity
            {
                Name = "A",
                Id = "B",
                X = null,
                Y = null,
                StructureNormalOrientation = null,
                StorageStructureAreaMean = null,
                StorageStructureAreaCoefficientOfVariation = null,
                AllowedLevelIncreaseStorageMean = null,
                AllowedLevelIncreaseStorageStandardDeviation = null,
                WidthFlowAperturesMean = null,
                WidthFlowAperturesStandardDeviation = null,
                LevelCrestStructureNotClosingMean = null,
                LevelCrestStructureNotClosingStandardDeviation = null,
                InsideWaterLevelMean = null,
                InsideWaterLevelStandardDeviation = null,
                ThresholdHeightOpenWeirMean = null,
                ThresholdHeightOpenWeirStandardDeviation = null,
                AreaFlowAperturesMean = null,
                AreaFlowAperturesStandardDeviation = null,
                CriticalOvertoppingDischargeMean = null,
                CriticalOvertoppingDischargeCoefficientOfVariation = null,
                FlowWidthAtBottomProtectionMean = null,
                FlowWidthAtBottomProtectionStandardDeviation = null,
                ProbabilityOpenStructureBeforeFlooding = null,
                FailureProbabilityOpenStructure = null
            };

            var collector = new ReadConversionCollector();

            // Call
            ClosingStructure structure = entity.Read(collector);

            // Assert
            Assert.IsNaN(structure.Location.X);
            Assert.IsNaN(structure.Location.Y);
            Assert.IsNaN(structure.StructureNormalOrientation);

            Assert.IsNaN(structure.StorageStructureArea.Mean);
            Assert.IsNaN(structure.StorageStructureArea.CoefficientOfVariation);
            Assert.IsNaN(structure.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(structure.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(structure.WidthFlowApertures.Mean);
            Assert.IsNaN(structure.WidthFlowApertures.StandardDeviation);
            Assert.IsNaN(structure.LevelCrestStructureNotClosing.Mean);
            Assert.IsNaN(structure.LevelCrestStructureNotClosing.StandardDeviation);
            Assert.IsNaN(structure.InsideWaterLevel.Mean);
            Assert.IsNaN(structure.InsideWaterLevel.StandardDeviation);
            Assert.IsNaN(structure.ThresholdHeightOpenWeir.Mean);
            Assert.IsNaN(structure.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.IsNaN(structure.AreaFlowApertures.Mean);
            Assert.IsNaN(structure.AreaFlowApertures.StandardDeviation);
            Assert.IsNaN(structure.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(structure.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.IsNaN(structure.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(structure.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(structure.ProbabilityOpenStructureBeforeFlooding);
            Assert.IsNaN(structure.FailureProbabilityOpenStructure);
            Assert.IsNaN(structure.FailureProbabilityReparation);
        }

        [Test]
        public void Read_EntityRegistered_ReturnRegisteredStructure()
        {
            // Setup
            var entity = new ClosingStructureEntity();
            ClosingStructure registeredStructure = new TestClosingStructure();
            var collector = new ReadConversionCollector();
            collector.Read(entity, registeredStructure);

            // Call
            ClosingStructure readStructure = entity.Read(collector);

            // Assert
            Assert.AreSame(registeredStructure, readStructure);
        }
    }
}