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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.Piping
{
    [TestFixture]
    public class StochasticSoilModelCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () => stochasticSoilModel.Create(null, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsStochasticSoilModelEntityWithPropertiesSet()
        {
            // Setup
            var order = new Random(1).Next();
            string testName = "testName";
            string testSegmentName = "testSegmentName";
            var stochasticSoilModel = new StochasticSoilModel(-1, testName, testSegmentName);
            var registry = new PersistenceRegistry();

            // Call
            var entity = stochasticSoilModel.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(testSegmentName, entity.SegmentName);
            Assert.AreEqual(order, entity.Order);
            Assert.IsEmpty(entity.StochasticSoilProfileEntities);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            string testName = "testName";
            string testSegmentName = "testSegmentName";
            var stochasticSoilModel = new StochasticSoilModel(-1, testName, testSegmentName);
            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity = stochasticSoilModel.Create(registry, 0);

            // Assert
            Assert.AreNotSame(testName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(testName, entity.Name);

            Assert.AreNotSame(testSegmentName, entity.SegmentName,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(testSegmentName, entity.SegmentName);
        }

        [Test]
        public void Create_WithStochasticSoilProfiles_ReturnsStochasticSoilModelEntityWithPropertiesAndStochasticSoilProfileEntitiesSet()
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel(-1, "testName", "testSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(50, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            });
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(50, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            });
            var registry = new PersistenceRegistry();

            // Call
            var entity = stochasticSoilModel.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.StochasticSoilProfileEntities.Count);
        }

        [Test]
        public void Create_WithGeometryPoints_ReturnsStochasticSoilModelEntityWithPropertiesAndStochasticSoilModelSegmentPointEntitiesSet()
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel(-1, "testName", "testSegmentName");
            stochasticSoilModel.Geometry.AddRange(new[]
            {
                new Point2D(-12.34, 56.78),
                new Point2D(91.23, -34.56)
            });
            var registry = new PersistenceRegistry();

            // Call
            var entity = stochasticSoilModel.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            string expectedXml = new Point2DXmlSerializer().ToXml(stochasticSoilModel.Geometry);
            Assert.AreEqual(expectedXml, entity.StochasticSoilModelSegmentPointXml);
        }

        [Test]
        public void Create_SameModelCreatedMultipleTimes_ReturnSameEntity()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A", "B");

            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity1 = soilModel.Create(registry, 0);
            StochasticSoilModelEntity entity2 = soilModel.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}