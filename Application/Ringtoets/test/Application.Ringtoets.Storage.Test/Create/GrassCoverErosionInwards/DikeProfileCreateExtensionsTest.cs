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

using System;

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class DikeProfileCreateExtensionsTest
    {
        [Test]
        public void Create_RegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(1.1, 2.2),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(3.3, 4.4), 0.75),
                                                  new RoughnessPoint(new Point2D(5.5, 6.6), 0.75)
                                              },
                                              new[]
                                              {
                                                  new Point2D(7.7, 8.8),
                                                  new Point2D(9.9, 10.10)
                                              },
                                              null,
                                              new DikeProfile.ConstructionProperties());

            // Call
            TestDelegate call = () => dikeProfile.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_WithoutBreakWater_ReturnEntityWithNullBreakWaterProperties()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(1.1, 2.2),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(3.3, 4.4), 0.75),
                                                  new RoughnessPoint(new Point2D(5.5, 6.6), 0.75)
                                              },
                                              new[]
                                              {
                                                  new Point2D(7.7, 8.8),
                                                  new Point2D(9.9, 10.10)
                                              },
                                              null,
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Name = "Dike profile without break water.",
                                                  DikeHeight = 11.11,
                                                  Orientation = 12.12,
                                                  X0 = 13.13
                                              });
            var registry = new PersistenceRegistry();

            // Call
            DikeProfileEntity entity = dikeProfile.Create(registry);

            // Assert
            Assert.AreEqual(dikeProfile.WorldReferencePoint.X, entity.X);
            Assert.AreEqual(dikeProfile.WorldReferencePoint.Y, entity.Y);
            Assert.AreEqual(dikeProfile.X0, entity.X0);
            byte[] convertedDikeGeometry = new RoughnessPointBinaryConverter().ToBytes(dikeProfile.DikeGeometry);
            CollectionAssert.AreEqual(convertedDikeGeometry, entity.DikeGeometryData);
            byte[] convertedForeshoreGeometry = new Point2DBinaryConverter().ToBytes(dikeProfile.ForeshoreGeometry);
            CollectionAssert.AreEqual(convertedForeshoreGeometry, entity.ForeShoreData);
            Assert.AreEqual(dikeProfile.Orientation.Value, entity.Orientation);
            Assert.AreEqual(dikeProfile.DikeHeight.Value, entity.DikeHeight);
            Assert.AreEqual(dikeProfile.Name, entity.Name);

            Assert.IsNull(entity.BreakWaterHeight);
            Assert.IsNull(entity.BreakWaterType);
        }

        [Test]
        [TestCase(BreakWaterType.Caisson, 1.1)]
        [TestCase(BreakWaterType.Dam, 2.2)]
        [TestCase(BreakWaterType.Wall, -3.3)]
        public void Create_WithBreakWater_ReturnEntity(BreakWaterType type, double height)
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(1234.4567, 5678.432),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(-6.6, -3.3), 0.75),
                                                  new RoughnessPoint(new Point2D(4.4, 5.5), 0.75)
                                              },
                                              new[]
                                              {
                                                  new Point2D(-12.12, -13.13),
                                                  new Point2D(-6.6, -3.3)
                                              },
                                              new BreakWater(type, height),
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Name = "Dike profile without break water.",
                                                  DikeHeight = 98.76,
                                                  Orientation = 76.54,
                                                  X0 = -12.34
                                              });
            var registry = new PersistenceRegistry();

            // Call
            DikeProfileEntity entity = dikeProfile.Create(registry);

            // Assert
            Assert.AreEqual(dikeProfile.WorldReferencePoint.X, entity.X);
            Assert.AreEqual(dikeProfile.WorldReferencePoint.Y, entity.Y);
            Assert.AreEqual(dikeProfile.X0, entity.X0);
            byte[] convertedDikeGeometry = new RoughnessPointBinaryConverter().ToBytes(dikeProfile.DikeGeometry);
            CollectionAssert.AreEqual(convertedDikeGeometry, entity.DikeGeometryData);
            byte[] convertedForeshoreGeometry = new Point2DBinaryConverter().ToBytes(dikeProfile.ForeshoreGeometry);
            CollectionAssert.AreEqual(convertedForeshoreGeometry, entity.ForeShoreData);
            Assert.AreEqual(dikeProfile.Orientation.Value, entity.Orientation);
            Assert.AreEqual(dikeProfile.DikeHeight.Value, entity.DikeHeight);
            Assert.AreEqual(dikeProfile.Name, entity.Name);

            Assert.AreEqual((byte)type, entity.BreakWaterType);
            Assert.AreEqual(height, entity.BreakWaterHeight);
        }

        [Test]
        public void Create_Always_RegisterEntityToPersistenceRegistry()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(1.1, 2.2),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(3.3, 4.4), 0.75),
                                                  new RoughnessPoint(new Point2D(5.5, 6.6), 0.75)
                                              },
                                              new[]
                                              {
                                                  new Point2D(7.7, 8.8),
                                                  new Point2D(9.9, 10.10)
                                              }, null, new DikeProfile.ConstructionProperties());
            var registry = new PersistenceRegistry();

            // Call
            DikeProfileEntity entity = dikeProfile.Create(registry);

            // Assert
            entity.DikeProfileEntityId = 345678;
            registry.TransferIds();
            Assert.AreEqual(entity.DikeProfileEntityId, dikeProfile.StorageId);
        }
    }
}