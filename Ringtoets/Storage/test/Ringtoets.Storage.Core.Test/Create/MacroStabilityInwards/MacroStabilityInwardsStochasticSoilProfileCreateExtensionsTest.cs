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
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.MacroStabilityInwards;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilProfileCreateExtensionsTest
    {
        [Test]
        public void Create_StochasticSoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => ((MacroStabilityInwardsStochasticSoilProfile) null).Create(registry, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("stochasticSoilProfile", parameterName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var soilProfile = mockRepository.Stub<IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>>();
            mockRepository.ReplayAll();

            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, soilProfile);

            // Call
            TestDelegate test = () => stochasticSoilProfile.Create(null, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Create_WithUnsupportedSoilProfile_ThrowsNotSupportedException()
        {
            // Setup
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.5, new UnsupportedMacroStabilityInwardsSoilProfile());
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate test = () => stochasticSoilProfile.Create(registry, 1);

            // Assert
            var exception = Assert.Throws<NotSupportedException>(test);
            Assert.AreEqual($"{nameof(UnsupportedMacroStabilityInwardsSoilProfile)} is not supported. " +
                            $"Supported types are: {nameof(MacroStabilityInwardsSoilProfile1D)} and {nameof(MacroStabilityInwardsSoilProfile2D)}.", exception.Message);
        }

        [Test]
        public void Create_WithMacroStabilityInwardsSoilProfile1D_ReturnsStochasticSoilProfileEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            MacroStabilityInwardsSoilProfile1D soilProfile =
                MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D(nameof(MacroStabilityInwardsSoilProfile1D));
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfile);

            int order = random.Next();

            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsStochasticSoilProfileEntity entity = stochasticSoilProfile.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(stochasticSoilProfile.Probability, entity.Probability);
            Assert.AreEqual(soilProfile.Name, entity.MacroStabilityInwardsSoilProfileOneDEntity.Name);
            Assert.IsNull(entity.MacroStabilityInwardsSoilProfileTwoDEntity);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_WithMacroStabilityInwardsSoilProfile2D_ReturnsStochasticSoilProfileEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            MacroStabilityInwardsSoilProfile2D soilProfile =
                MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D();
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfile);

            int order = random.Next();

            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsStochasticSoilProfileEntity entity = stochasticSoilProfile.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(stochasticSoilProfile.Probability, entity.Probability);
            Assert.IsNull(entity.MacroStabilityInwardsSoilProfileOneDEntity);
            Assert.AreEqual(soilProfile.Name, entity.MacroStabilityInwardsSoilProfileTwoDEntity.Name);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_DifferentStochasticSoilProfilesWithSameMacroStabilityInwardsSoilProfile1D_ReturnsEntityWithSameSoilProfileEntitySet()
        {
            // Setup
            var random = new Random(31);

            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var firstStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfile);
            var secondStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfile);
            var registry = new PersistenceRegistry();

            MacroStabilityInwardsStochasticSoilProfileEntity firstEntity = firstStochasticSoilProfile.Create(registry, 0);

            // Call
            MacroStabilityInwardsStochasticSoilProfileEntity secondEntity = secondStochasticSoilProfile.Create(registry, 0);

            // Assert
            Assert.AreSame(firstEntity.MacroStabilityInwardsSoilProfileOneDEntity, secondEntity.MacroStabilityInwardsSoilProfileOneDEntity);
            Assert.IsNull(firstEntity.MacroStabilityInwardsSoilProfileTwoDEntity);
            Assert.IsNull(secondEntity.MacroStabilityInwardsSoilProfileTwoDEntity);
        }

        [Test]
        public void Create_DifferentStochasticSoilProfilesWithSameMacroStabilityInwardsSoilProfile2D_ReturnsEntityWithSameSoilProfileEntitySet()
        {
            // Setup
            var random = new Random(31);

            MacroStabilityInwardsSoilProfile2D soilProfile = MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D();
            var firstStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfile);
            var secondStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfile);
            var registry = new PersistenceRegistry();

            MacroStabilityInwardsStochasticSoilProfileEntity firstEntity = firstStochasticSoilProfile.Create(registry, 0);

            // Call
            MacroStabilityInwardsStochasticSoilProfileEntity secondEntity = secondStochasticSoilProfile.Create(registry, 0);

            // Assert
            Assert.IsNull(firstEntity.MacroStabilityInwardsSoilProfileOneDEntity);
            Assert.IsNull(secondEntity.MacroStabilityInwardsSoilProfileOneDEntity);
            Assert.AreSame(firstEntity.MacroStabilityInwardsSoilProfileTwoDEntity, secondEntity.MacroStabilityInwardsSoilProfileTwoDEntity);
        }

        [Test]
        [TestCaseSource(nameof(GetMacroStabilityInwardsSoilProfiles))]
        public void GivenCreatedEntity_WhenCreateCalledOnSameObject_ThenSameEntityInstanceReturned(IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile)
        {
            // Given
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.4, soilProfile);
            var registry = new PersistenceRegistry();

            MacroStabilityInwardsStochasticSoilProfileEntity entity1 = stochasticSoilProfile.Create(registry, 0);

            // When
            MacroStabilityInwardsStochasticSoilProfileEntity entity2 = stochasticSoilProfile.Create(registry, 0);

            // Then
            Assert.AreSame(entity1, entity2);
        }

        private class UnsupportedMacroStabilityInwardsSoilProfile : IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>
        {
            public string Name { get; }
            public IEnumerable<IMacroStabilityInwardsSoilLayer> Layers { get; }
        }

        private static IEnumerable<IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>> GetMacroStabilityInwardsSoilProfiles()
        {
            yield return MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            yield return MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D();
        }
    }
}