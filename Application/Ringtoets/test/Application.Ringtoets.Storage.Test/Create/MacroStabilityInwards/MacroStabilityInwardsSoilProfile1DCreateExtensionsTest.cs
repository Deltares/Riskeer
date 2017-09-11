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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile1DCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var soilProfile = new TestMacroStabilityInwardsSoilProfile1D();

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
            var soilProfile = new MacroStabilityInwardsSoilProfile1D(name, -random.NextDouble(), new[]
            {
                new MacroStabilityInwardsSoilLayer1D(random.NextDouble()),
                new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
            });
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsSoilProfile1DEntity entity = soilProfile.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(soilProfile.Bottom, entity.Bottom);
            Assert.AreEqual(2, entity.MacroStabilityInwardsSoilLayer1DEntity.Count);

            MacroStabilityInwardsSoilLayer1DEntity firstLayerEntity = entity.MacroStabilityInwardsSoilLayer1DEntity.ElementAt(0);
            Assert.AreEqual(soilProfile.Layers.ElementAt(0).Top, firstLayerEntity.Top);

            MacroStabilityInwardsSoilLayer1DEntity secondLayerEntity = entity.MacroStabilityInwardsSoilLayer1DEntity.ElementAt(1);
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
            MacroStabilityInwardsSoilProfile1DEntity entity = soilProfile.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.Bottom);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "some name";
            var soilProfile = new TestMacroStabilityInwardsSoilProfile1D(name);
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsSoilProfile1DEntity entity = soilProfile.Create(registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(name, entity.Name);
        }

        [Test]
        public void Create_ForTheSameObjectTwice_ReturnsSameEntityInstance()
        {
            // Setup
            var soilProfile = new TestMacroStabilityInwardsSoilProfile1D();
            var registry = new PersistenceRegistry();

            MacroStabilityInwardsSoilProfile1DEntity firstEntity = soilProfile.Create(registry);

            // Call
            MacroStabilityInwardsSoilProfile1DEntity secondEntity = soilProfile.Create(registry);

            // Assert
            Assert.AreSame(firstEntity, secondEntity);
        }
    }
}