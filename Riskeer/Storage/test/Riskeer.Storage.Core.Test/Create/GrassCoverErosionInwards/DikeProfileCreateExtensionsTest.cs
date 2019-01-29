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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.GrassCoverErosionInwards;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class DikeProfileCreateExtensionsTest
    {
        [Test]
        public void Create_RegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();

            // Call
            TestDelegate call = () => dikeProfile.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_WithoutBreakWater_ReturnEntityWithNullBreakWaterProperties()
        {
            // Setup
            int order = new Random(22).Next();
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
                                                  Id = "no_breakwater",
                                                  Name = "Dike profile without break water.",
                                                  DikeHeight = 11.11,
                                                  Orientation = 12.12,
                                                  X0 = 13.13
                                              });
            var registry = new PersistenceRegistry();

            // Call
            DikeProfileEntity entity = dikeProfile.Create(registry, order);

            // Assert
            Assert.AreEqual(dikeProfile.WorldReferencePoint.X, entity.X);
            Assert.AreEqual(dikeProfile.WorldReferencePoint.Y, entity.Y);
            Assert.AreEqual(dikeProfile.X0, entity.X0);
            Assert.AreEqual(order, entity.Order);
            string convertedDikeGeometry = new RoughnessPointCollectionXmlSerializer().ToXml(dikeProfile.DikeGeometry);
            Assert.AreEqual(convertedDikeGeometry, entity.DikeGeometryXml);
            string convertedForeshoreGeometry = new Point2DCollectionXmlSerializer().ToXml(dikeProfile.ForeshoreGeometry);
            Assert.AreEqual(convertedForeshoreGeometry, entity.ForeshoreXml);
            Assert.AreEqual(dikeProfile.Orientation.Value, entity.Orientation);
            Assert.AreEqual(dikeProfile.DikeHeight.Value, entity.DikeHeight);
            Assert.AreEqual(dikeProfile.Id, entity.Id);
            Assert.AreEqual(dikeProfile.Name, entity.Name);

            Assert.IsNull(entity.BreakWaterHeight);
            Assert.IsNull(entity.BreakWaterType);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalId = "no_breakwater";
            const string originalName = "Dike profile without break water.";
            var dikeProfile = new DikeProfile(new Point2D(1.1, 2.2),
                                              new RoughnessPoint[0],
                                              new Point2D[0],
                                              null,
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Id = originalId,
                                                  Name = originalName
                                              });
            var registry = new PersistenceRegistry();

            // Call
            DikeProfileEntity entity = dikeProfile.Create(registry, 0);

            // Assert
            Assert.AreNotSame(originalName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(originalName, entity.Name);
            Assert.AreNotSame(originalId, entity.Id,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(originalId, entity.Id);
        }

        [Test]
        [TestCase(BreakWaterType.Caisson, 1.1)]
        [TestCase(BreakWaterType.Dam, 2.2)]
        [TestCase(BreakWaterType.Wall, -3.3)]
        public void Create_WithBreakWater_ReturnEntity(BreakWaterType type, double height)
        {
            // Setup
            int order = new Random(42).Next();
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
                                                  Id = "no_breakwater",
                                                  Name = "Dike profile without break water.",
                                                  DikeHeight = 98.76,
                                                  Orientation = 76.54,
                                                  X0 = -12.34
                                              });
            var registry = new PersistenceRegistry();

            // Call
            DikeProfileEntity entity = dikeProfile.Create(registry, order);

            // Assert
            Assert.AreEqual(dikeProfile.WorldReferencePoint.X, entity.X);
            Assert.AreEqual(dikeProfile.WorldReferencePoint.Y, entity.Y);
            Assert.AreEqual(dikeProfile.X0, entity.X0);
            Assert.AreEqual(order, entity.Order);
            string convertedDikeGeometry = new RoughnessPointCollectionXmlSerializer().ToXml(dikeProfile.DikeGeometry);
            Assert.AreEqual(convertedDikeGeometry, entity.DikeGeometryXml);
            string convertedForeshoreGeometry = new Point2DCollectionXmlSerializer().ToXml(dikeProfile.ForeshoreGeometry);
            Assert.AreEqual(convertedForeshoreGeometry, entity.ForeshoreXml);
            Assert.AreEqual(dikeProfile.Orientation.Value, entity.Orientation);
            Assert.AreEqual(dikeProfile.DikeHeight.Value, entity.DikeHeight);
            Assert.AreEqual(dikeProfile.Id, entity.Id);
            Assert.AreEqual(dikeProfile.Name, entity.Name);

            Assert.AreEqual((byte) type, entity.BreakWaterType);
            Assert.AreEqual(height, entity.BreakWaterHeight);
        }

        [Test]
        public void Create_DikeProfileAlreadyRegistered_ReturnRegisteredEntity()
        {
            // Setup
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            var registry = new PersistenceRegistry();
            DikeProfileEntity entity1 = dikeProfile.Create(registry, 0);

            // Precondition:
            Assert.IsTrue(registry.Contains(dikeProfile));

            // Call
            DikeProfileEntity entity2 = dikeProfile.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}