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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Primitives.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingStochasticSoilModelCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();

            // Call
            TestDelegate test = () => stochasticSoilModel.Create(null, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_StochasticSoilModelNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => ((PipingStochasticSoilModel) null).Create(registry, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsPipingStochasticSoilModelEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(1);
            int order = random.Next();
            const string testName = "testName";
            PipingStochasticSoilModel stochasticSoilModel =
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(testName);
            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity = stochasticSoilModel.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(order, entity.Order);
            CollectionAssert.IsEmpty(entity.MacroStabilityInwardsStochasticSoilProfileEntities);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "testName";
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(name);
            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity = stochasticSoilModel.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(name, entity.Name);
        }

        [Test]
        public void Create_WithStochasticSoilProfiles_ReturnsStochasticSoilModelEntityWithPropertiesAndPipingStochasticSoilProfileEntitiesSet()
        {
            // Setup
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("testName", new[]
            {
                new PipingStochasticSoilProfile(1, PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            });
            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity = stochasticSoilModel.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(stochasticSoilModel.StochasticSoilProfiles.Count(), entity.PipingStochasticSoilProfileEntities.Count);

            PipingStochasticSoilProfileEntity stochastEntity = entity.PipingStochasticSoilProfileEntities.Single();
            Assert.AreEqual(stochasticSoilModel.StochasticSoilProfiles.Single().Probability, stochastEntity.Probability);
            Assert.IsNotNull(stochastEntity.PipingSoilProfileEntity);

            CollectionAssert.IsEmpty(entity.MacroStabilityInwardsStochasticSoilProfileEntities);
        }

        [Test]
        public void Create_WithGeometryPoints_ReturnsStochasticSoilModelEntityWithPropertiesAndStochasticSoilModelSegmentPointEntitiesSet()
        {
            // Setup
            var random = new Random(31);
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("testName", new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            });

            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity = stochasticSoilModel.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            string expectedXml = new Point2DCollectionXmlSerializer().ToXml(stochasticSoilModel.Geometry);
            Assert.AreEqual(expectedXml, entity.StochasticSoilModelSegmentPointXml);
        }

        [Test]
        public void Create_SameModelCreatedMultipleTimes_ReturnSameEntity()
        {
            // Setup
            PipingStochasticSoilModel soilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("A");

            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity1 = soilModel.Create(registry, 0);
            StochasticSoilModelEntity entity2 = soilModel.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}