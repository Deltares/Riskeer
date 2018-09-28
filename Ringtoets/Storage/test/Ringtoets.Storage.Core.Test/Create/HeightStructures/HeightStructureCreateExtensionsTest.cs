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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.HeightStructures;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.HeightStructures
{
    [TestFixture]
    public class HeightStructureCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            HeightStructure structure = new TestHeightStructure();

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
            HeightStructure structure = new TestHeightStructure();
            var registry = new PersistenceRegistry();

            const int order = 4;

            // Call
            HeightStructureEntity entity = structure.Create(registry, order);

            // Assert
            Assert.AreEqual(structure.Name, entity.Name);
            Assert.AreNotSame(structure.Name, entity.Name);
            Assert.AreEqual(structure.Id, entity.Id);
            Assert.AreNotSame(structure.Id, entity.Id);
            Assert.AreEqual(structure.Location.X, entity.X);
            Assert.AreEqual(structure.Location.Y, entity.Y);
            Assert.AreEqual(structure.StructureNormalOrientation.Value, entity.StructureNormalOrientation);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.Mean.Value, entity.AllowedLevelIncreaseStorageMean);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.StandardDeviation.Value, entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge.Mean.Value, entity.CriticalOvertoppingDischargeMean);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value, entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.AreEqual(structure.FailureProbabilityStructureWithErosion, entity.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.Mean.Value, entity.FlowWidthAtBottomProtectionMean);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.StandardDeviation.Value, entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.AreEqual(structure.LevelCrestStructure.Mean.Value, entity.LevelCrestStructureMean);
            Assert.AreEqual(structure.LevelCrestStructure.StandardDeviation.Value, entity.LevelCrestStructureStandardDeviation);
            Assert.AreEqual(structure.StorageStructureArea.Mean.Value, entity.StorageStructureAreaMean);
            Assert.AreEqual(structure.StorageStructureArea.CoefficientOfVariation.Value, entity.StorageStructureAreaCoefficientOfVariation);
            Assert.AreEqual(structure.WidthFlowApertures.Mean.Value, entity.WidthFlowAperturesMean);
            Assert.AreEqual(structure.WidthFlowApertures.StandardDeviation.Value, entity.WidthFlowAperturesStandardDeviation);
            Assert.AreEqual(order, entity.Order);

            Assert.IsTrue(registry.Contains(structure));
        }

        [Test]
        public void Create_NaNValue_ReturnEntityWithNullValue()
        {
            // Setup
            var structure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = "A",
                Id = "B",
                Location = new Point2D(double.NaN, double.NaN),
                StructureNormalOrientation = RoundedDouble.NaN,
                AllowedLevelIncreaseStorage =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                FailureProbabilityStructureWithErosion = double.NaN,
                FlowWidthAtBottomProtection =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                LevelCrestStructure =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                StorageStructureArea =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                WidthFlowApertures =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                }
            });
            var registry = new PersistenceRegistry();

            // Call
            HeightStructureEntity entity = structure.Create(registry, 0);

            // Assert
            Assert.IsNull(entity.X);
            Assert.IsNull(entity.Y);
            Assert.IsNull(entity.StructureNormalOrientation);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.IsNull(entity.CriticalOvertoppingDischargeMean);
            Assert.IsNull(entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.IsNull(entity.FailureProbabilityStructureWithErosion);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionMean);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.IsNull(entity.LevelCrestStructureMean);
            Assert.IsNull(entity.LevelCrestStructureStandardDeviation);
            Assert.IsNull(entity.StorageStructureAreaMean);
            Assert.IsNull(entity.StorageStructureAreaCoefficientOfVariation);
            Assert.IsNull(entity.WidthFlowAperturesMean);
            Assert.IsNull(entity.WidthFlowAperturesStandardDeviation);
        }

        [Test]
        public void Create_StructureAlreadyRegistered_ReturnRegisteredEntity()
        {
            // Setup
            var structure = new TestHeightStructure();

            var registeredEntity = new HeightStructureEntity();
            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, structure);

            // Call
            HeightStructureEntity entity = structure.Create(registry, 0);

            // Assert
            Assert.AreSame(registeredEntity, entity);
        }
    }
}