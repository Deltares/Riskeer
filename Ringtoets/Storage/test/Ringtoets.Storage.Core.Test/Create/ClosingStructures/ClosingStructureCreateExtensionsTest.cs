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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.ClosingStructures;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.ClosingStructures
{
    [TestFixture]
    public class ClosingStructureCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            ClosingStructure structure = new TestClosingStructure();

            // Call
            TestDelegate call = () => structure.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_ValidStructure_ReturnEntity()
        {
            // Setup
            ClosingStructure structure = new TestClosingStructure();
            var registry = new PersistenceRegistry();

            const int order = 4;

            // Call
            ClosingStructureEntity entity = structure.Create(registry, order);

            // Assert
            Assert.AreEqual(structure.Name, entity.Name);
            Assert.AreNotSame(structure.Name, entity.Name);
            Assert.AreEqual(structure.Id, entity.Id);
            Assert.AreNotSame(structure.Id, entity.Id);
            Assert.AreEqual(structure.Location.X, entity.X);
            Assert.AreEqual(structure.Location.Y, entity.Y);
            Assert.AreEqual(structure.StructureNormalOrientation.Value, entity.StructureNormalOrientation);

            Assert.AreEqual(structure.StorageStructureArea.Mean.Value, entity.StorageStructureAreaMean);
            Assert.AreEqual(structure.StorageStructureArea.CoefficientOfVariation.Value, entity.StorageStructureAreaCoefficientOfVariation);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.Mean.Value, entity.AllowedLevelIncreaseStorageMean);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.StandardDeviation.Value, entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.AreEqual(structure.WidthFlowApertures.Mean.Value, entity.WidthFlowAperturesMean);
            Assert.AreEqual(structure.WidthFlowApertures.StandardDeviation.Value, entity.WidthFlowAperturesStandardDeviation);
            Assert.AreEqual(structure.LevelCrestStructureNotClosing.Mean.Value, entity.LevelCrestStructureNotClosingMean);
            Assert.AreEqual(structure.LevelCrestStructureNotClosing.StandardDeviation.Value, entity.LevelCrestStructureNotClosingStandardDeviation);
            Assert.AreEqual(structure.InsideWaterLevel.Mean.Value, entity.InsideWaterLevelMean);
            Assert.AreEqual(structure.InsideWaterLevel.StandardDeviation.Value, entity.InsideWaterLevelStandardDeviation);
            Assert.AreEqual(structure.ThresholdHeightOpenWeir.Mean.Value, entity.ThresholdHeightOpenWeirMean);
            Assert.AreEqual(structure.ThresholdHeightOpenWeir.StandardDeviation.Value, entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.AreEqual(structure.AreaFlowApertures.Mean.Value, entity.AreaFlowAperturesMean);
            Assert.AreEqual(structure.AreaFlowApertures.StandardDeviation.Value, entity.AreaFlowAperturesStandardDeviation);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge.Mean.Value, entity.CriticalOvertoppingDischargeMean);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value, entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.Mean.Value, entity.FlowWidthAtBottomProtectionMean);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.StandardDeviation.Value, entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.AreEqual(structure.ProbabilityOrFrequencyOpenStructureBeforeFlooding, entity.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(structure.FailureProbabilityOpenStructure, entity.FailureProbabilityOpenStructure);
            Assert.AreEqual(structure.IdenticalApertures, entity.IdenticalApertures);
            Assert.AreEqual(structure.FailureProbabilityReparation, entity.FailureProbabilityReparation);
            Assert.AreEqual(Convert.ToByte(structure.InflowModelType), entity.InflowModelType);
            Assert.AreEqual(order, entity.Order);

            Assert.IsTrue(registry.Contains(structure));
        }

        [Test]
        public void Create_NaNValue_ReturnEntityWithNullValue()
        {
            // Setup
            var structure = new ClosingStructure(new ClosingStructure.ConstructionProperties
            {
                Name = "A",
                Id = "B",
                Location = new Point2D(double.NaN, double.NaN),
                StructureNormalOrientation = RoundedDouble.NaN,
                StorageStructureArea =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                WidthFlowApertures =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                LevelCrestStructureNotClosing =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                InsideWaterLevel =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                AreaFlowApertures =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                ProbabilityOrFrequencyOpenStructureBeforeFlooding = double.NaN,
                FailureProbabilityOpenStructure = double.NaN,
                FailureProbabilityReparation = double.NaN
            });
            var registry = new PersistenceRegistry();

            // Call
            ClosingStructureEntity entity = structure.Create(registry, 0);

            // Assert
            Assert.IsNull(entity.X);
            Assert.IsNull(entity.Y);
            Assert.IsNull(entity.StructureNormalOrientation);
            Assert.IsNull(entity.StorageStructureAreaMean);
            Assert.IsNull(entity.StorageStructureAreaCoefficientOfVariation);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.IsNull(entity.WidthFlowAperturesMean);
            Assert.IsNull(entity.WidthFlowAperturesStandardDeviation);
            Assert.IsNull(entity.LevelCrestStructureNotClosingMean);
            Assert.IsNull(entity.LevelCrestStructureNotClosingStandardDeviation);
            Assert.IsNull(entity.InsideWaterLevelMean);
            Assert.IsNull(entity.InsideWaterLevelStandardDeviation);
            Assert.IsNull(entity.ThresholdHeightOpenWeirMean);
            Assert.IsNull(entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.IsNull(entity.AreaFlowAperturesMean);
            Assert.IsNull(entity.AreaFlowAperturesStandardDeviation);
            Assert.IsNull(entity.CriticalOvertoppingDischargeMean);
            Assert.IsNull(entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionMean);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.IsNull(entity.ProbabilityOpenStructureBeforeFlooding);
            Assert.IsNull(entity.FailureProbabilityOpenStructure);
            Assert.IsNull(entity.FailureProbabilityReparation);
        }

        [Test]
        public void Create_StructureAlreadyRegistered_ReturnRegisteredEntity()
        {
            // Setup
            ClosingStructure structure = new TestClosingStructure();

            var registeredEntity = new ClosingStructureEntity();
            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, structure);

            // Call
            ClosingStructureEntity entity = structure.Create(registry, 0);

            // Assert
            Assert.AreSame(registeredEntity, entity);
        }
    }
}