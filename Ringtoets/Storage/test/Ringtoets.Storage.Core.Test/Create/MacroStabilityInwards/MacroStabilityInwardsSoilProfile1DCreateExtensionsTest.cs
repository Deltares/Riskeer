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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.MacroStabilityInwards;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile1DCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSoilProfile1D soilProfile =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();

            // Call
            TestDelegate test = () => soilProfile.Create(null);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => ((MacroStabilityInwardsSoilProfile1D) null).Create(registry);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("soilProfile", parameterName);
        }

        [Test]
        public void Create_WithValidProperties_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            const string name = "some name";
            double bottom = -random.NextDouble();
            var soilProfile = new MacroStabilityInwardsSoilProfile1D(name, bottom, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(random.NextDouble()),
                new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
            });
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsSoilProfileOneDEntity entity = soilProfile.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(soilProfile.Bottom, entity.Bottom);
            Assert.AreEqual(soilProfile.Layers.Count(), entity.MacroStabilityInwardsSoilLayerOneDEntities.Count);

            MacroStabilityInwardsSoilLayerOneDEntity firstLayerEntity = entity.MacroStabilityInwardsSoilLayerOneDEntities.ElementAt(0);
            Assert.AreEqual(soilProfile.Layers.ElementAt(0).Top, firstLayerEntity.Top);

            MacroStabilityInwardsSoilLayerOneDEntity secondLayerEntity = entity.MacroStabilityInwardsSoilLayerOneDEntities.ElementAt(1);
            Assert.AreEqual(soilProfile.Layers.ElementAt(1).Top, secondLayerEntity.Top);
        }

        [Test]
        public void Create_WithNaNProperties_ReturnsEntityWithPropertiesSetToNull()
        {
            // Setup
            var random = new Random(31);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("some name", double.NaN, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
            });
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsSoilProfileOneDEntity entity = soilProfile.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.Bottom);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "some name";
            MacroStabilityInwardsSoilProfile1D soilProfile =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D(name);
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsSoilProfileOneDEntity entity = soilProfile.Create(registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(name, entity.Name);
        }

        [Test]
        public void GivenCreatedEntity_WhenCreateCalledOnSameObject_ThenSameEntityReturned()
        {
            // Given
            MacroStabilityInwardsSoilProfile1D soilProfile =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var registry = new PersistenceRegistry();

            MacroStabilityInwardsSoilProfileOneDEntity firstEntity = soilProfile.Create(registry);

            // When
            MacroStabilityInwardsSoilProfileOneDEntity secondEntity = soilProfile.Create(registry);

            // Then
            Assert.AreSame(firstEntity, secondEntity);
        }
    }
}