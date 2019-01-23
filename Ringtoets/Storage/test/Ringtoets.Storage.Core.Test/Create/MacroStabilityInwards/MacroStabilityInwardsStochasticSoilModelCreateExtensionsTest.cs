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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();

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
            TestDelegate test = () => ((MacroStabilityInwardsStochasticSoilModel) null).Create(registry, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("model", parameterName);
        }

        [Test]
        public void Create_WithValidProperties_ReturnsStochasticSoilModelEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(1);
            int order = random.Next();
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(nameof(MacroStabilityInwardsStochasticSoilModel));
            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity = stochasticSoilModel.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(stochasticSoilModel.Name, entity.Name);
            Assert.AreEqual(order, entity.Order);
            CollectionAssert.IsEmpty(entity.PipingStochasticSoilProfileEntities);
        }

        [Test]
        public void Create_WithStochasticSoilProfiles_ReturnsStochasticSoilModelEntityWithPropertiesSet()
        {
            // Setup
            var stochasticSoilProfiles = new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.1, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D()),
                new MacroStabilityInwardsStochasticSoilProfile(0.9, MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D())
            };
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("Model", stochasticSoilProfiles);

            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity = stochasticSoilModel.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity);
            CollectionAssert.IsEmpty(entity.PipingStochasticSoilProfileEntities);
            Assert.AreEqual(stochasticSoilProfiles.Length, entity.MacroStabilityInwardsStochasticSoilProfileEntities.Count);

            MacroStabilityInwardsStochasticSoilProfile firstStochasticSoilProfile = stochasticSoilProfiles[0];
            MacroStabilityInwardsStochasticSoilProfileEntity firstStochasticSoilProfileEntity = entity.MacroStabilityInwardsStochasticSoilProfileEntities.First();
            Assert.AreEqual(firstStochasticSoilProfile.Probability, firstStochasticSoilProfileEntity.Probability);
            Assert.IsNotNull(firstStochasticSoilProfileEntity.MacroStabilityInwardsSoilProfileOneDEntity);
            Assert.IsNull(firstStochasticSoilProfileEntity.MacroStabilityInwardsSoilProfileTwoDEntity);

            MacroStabilityInwardsStochasticSoilProfile secondStochasticSoilProfile = stochasticSoilProfiles[1];
            MacroStabilityInwardsStochasticSoilProfileEntity secondStochasticSoilProfileEntity = entity.MacroStabilityInwardsStochasticSoilProfileEntities.ElementAt(1);
            Assert.AreEqual(secondStochasticSoilProfile.Probability, secondStochasticSoilProfileEntity.Probability);
            Assert.IsNull(secondStochasticSoilProfileEntity.MacroStabilityInwardsSoilProfileOneDEntity);
            Assert.IsNotNull(secondStochasticSoilProfileEntity.MacroStabilityInwardsSoilProfileTwoDEntity);
        }

        [Test]
        public void Create_WithGeometryPoints_ReturnsStochasticSoilModelEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("testName", new[]
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
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "testName";
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(name);
            var registry = new PersistenceRegistry();

            // Call
            StochasticSoilModelEntity entity = stochasticSoilModel.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(name, entity.Name);
        }

        [Test]
        public void GivenCreatedEntity_WhenCreateCalledOnSameObject_ThenSameEntityInstanceReturned()
        {
            // Given
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            var registry = new PersistenceRegistry();

            StochasticSoilModelEntity firstEntity = stochasticSoilModel.Create(registry, 0);

            // When
            StochasticSoilModelEntity secondEntity = stochasticSoilModel.Create(registry, 0);

            // Then
            Assert.AreSame(firstEntity, secondEntity);
        }
    }
}