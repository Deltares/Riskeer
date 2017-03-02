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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class ForeshoreProfileCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();

            // Call
            TestDelegate test = () => foreshoreProfile.Create(null, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithSimpleProperties_ReturnsForeshoreProfileWithSimplePropertiesSet()
        {
            // Setup
            var name = "testName";
            var random = new Random(21);
            int order = random.Next();
            double orientation = random.NextDouble();
            double x0 = random.NextDouble();

            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), Enumerable.Empty<Point2D>(), null, new ForeshoreProfile.ConstructionProperties
            {
                Id = "fpid",
                Name = name,
                Orientation = orientation,
                X0 = x0
            });
            var registry = new PersistenceRegistry();

            // Call
            ForeshoreProfileEntity entity = foreshoreProfile.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(orientation, entity.Orientation, foreshoreProfile.Orientation.GetAccuracy());
            Assert.AreEqual(x0, entity.X0);
            Assert.IsNull(entity.BreakWaterType);
            Assert.IsNull(entity.BreakWaterHeight);
        }

        [Test]
        public void Create_WithNaNProperties_ReturnsForeshoreProfileWithPropertiesSetToNaN()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(
                new Point2D(0, 0),
                Enumerable.Empty<Point2D>(),
                new BreakWater(BreakWaterType.Caisson, double.NaN),
                new ForeshoreProfile.ConstructionProperties
                {
                    Id = "id",
                    Orientation = double.NaN,
                    X0 = double.NaN
                });
            var registry = new PersistenceRegistry();

            // Call
            ForeshoreProfileEntity entity = foreshoreProfile.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNaN(entity.Orientation);
            Assert.IsNaN(entity.X0);
            Assert.IsNaN(entity.BreakWaterHeight);
        }

        [Test]
        public void Create_WithGeometry_ReturnsForeshoreProfileWithGeometryStringSet()
        {
            // Setup
            int order = new Random(21).Next();
            var geometryPoints = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 0)
            };
            var foreshoreProfile = new TestForeshoreProfile(geometryPoints);
            var registry = new PersistenceRegistry();

            // Call
            ForeshoreProfileEntity entity = foreshoreProfile.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            string expectedXml = new Point2DXmlSerializer().ToXml(geometryPoints);
            Assert.AreEqual(expectedXml, entity.GeometryXml);
        }

        [Test]
        public void Create_WithBreakWater_ReturnsForeshoreProfileWithBreakWaterPropertiesSet()
        {
            // Setup
            double height = new Random(21).NextDouble();
            var breakWaterType = BreakWaterType.Caisson;
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(breakWaterType, height));
            var registry = new PersistenceRegistry();

            // Call
            ForeshoreProfileEntity entity = foreshoreProfile.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((int) breakWaterType, entity.BreakWaterType);
            Assert.AreEqual(height, entity.BreakWaterHeight, foreshoreProfile.BreakWater.Height.GetAccuracy());
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            string testName = "original name";
            var foreshoreProfile = new TestForeshoreProfile(testName);
            var registry = new PersistenceRegistry();

            // Call
            ForeshoreProfileEntity entity = foreshoreProfile.Create(registry, 0);

            // Assert
            Assert.AreNotSame(testName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(testName, entity.Name);
        }

        [Test]
        public void Create_ForeshoreProfileAlreadyRegistered_ReturnRegisteredEntity()
        {
            // Setup
            var forshoreProfile = new TestForeshoreProfile();
            var registry = new PersistenceRegistry();

            ForeshoreProfileEntity entity1 = forshoreProfile.Create(registry, 0);

            // Precondition:
            Assert.IsTrue(registry.Contains(forshoreProfile));

            // Call
            ForeshoreProfileEntity entity2 = forshoreProfile.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}