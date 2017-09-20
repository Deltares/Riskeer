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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
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
            var soilProfile = mockRepository.Stub<IMacroStabilityInwardsSoilProfile>();
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
        [TestCaseSource(nameof(GetMacroStabilityInwardsSoilProfiles))]
        public void Create_WithValidProperties_ReturnsStochasticSoilProfileEntityWithPropertiesSet(IMacroStabilityInwardsSoilProfile soilProfile)
        {
            // Setup
            var random = new Random(31);
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfile);

            int order = random.Next();

            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsStochasticSoilProfileEntity entity = stochasticSoilProfile.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(stochasticSoilProfile.Probability, entity.Probability);

            var soilProfile1D = soilProfile as MacroStabilityInwardsSoilProfile1D;
            if (soilProfile1D == null)
            {
                Assert.IsNull(entity.MacroStabilityInwardsSoilProfileOneDEntity);
            }
            else
            {
                Assert.AreEqual(soilProfile1D.Name, entity.MacroStabilityInwardsSoilProfileOneDEntity.Name);
            }

            var soilProfile2D = soilProfile as MacroStabilityInwardsSoilProfile2D;
            if (soilProfile2D == null)
            {
                Assert.IsNull(entity.MacroStabilityInwardsSoilProfileTwoDEntity);
            }
            else
            {
                Assert.AreEqual(soilProfile2D.Name, entity.MacroStabilityInwardsSoilProfileTwoDEntity.Name);
            }

            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        [TestCaseSource(nameof(GetSameExpectedMacroStabilityInwardsSoilProfiles))]
        public void Create_DifferentStochasticSoilProfilesWithSameSoilProfile_ReturnsEntityWithSameSoilProfileEntitySet(
            IMacroStabilityInwardsSoilProfile soilProfileOne,
            IMacroStabilityInwardsSoilProfile soilProfileTwo,
            bool expectedToBeSame)
        {
            // Setup
            var random = new Random(31);
            var firstStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfileOne);
            var secondStochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), soilProfileTwo);
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsStochasticSoilProfileEntity firstEntity = firstStochasticSoilProfile.Create(registry, 0);
            MacroStabilityInwardsStochasticSoilProfileEntity secondEntity = secondStochasticSoilProfile.Create(registry, 0);

            // Assert
            if (!expectedToBeSame)
            {
                if (firstEntity.MacroStabilityInwardsSoilProfileOneDEntity != null)
                {
                    Assert.AreNotSame(firstEntity.MacroStabilityInwardsSoilProfileOneDEntity, secondEntity.MacroStabilityInwardsSoilProfileOneDEntity);
                }
                if (firstEntity.MacroStabilityInwardsSoilProfileTwoDEntity != null)
                {
                    Assert.AreNotSame(firstEntity.MacroStabilityInwardsSoilProfileTwoDEntity, secondEntity.MacroStabilityInwardsSoilProfileTwoDEntity);
                }
            }
            else
            {
                Assert.AreSame(firstEntity.MacroStabilityInwardsSoilProfileOneDEntity, secondEntity.MacroStabilityInwardsSoilProfileOneDEntity);
                Assert.AreSame(firstEntity.MacroStabilityInwardsSoilProfileTwoDEntity, secondEntity.MacroStabilityInwardsSoilProfileTwoDEntity);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetMacroStabilityInwardsSoilProfiles))]
        public void GivenCreatedEntity_WhenCreateCalledOnSameObject_ThenSameEntityInstanceReturned(IMacroStabilityInwardsSoilProfile soilProfile)
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

        private class UnsupportedMacroStabilityInwardsSoilProfile : IMacroStabilityInwardsSoilProfile
        {
            public string Name { get; }
        }

        private static IEnumerable<TestCaseData> GetSameExpectedMacroStabilityInwardsSoilProfiles()
        {
            IMacroStabilityInwardsSoilProfile[] soilProfiles = GetMacroStabilityInwardsSoilProfiles().ToArray();
            int count = soilProfiles.Length;
            for (var i = 0; i < count; i++)
            {
                for (var j = 0; j < count; j++)
                {
                    yield return new TestCaseData(soilProfiles[i], soilProfiles[j], i == j);
                }
            }
        }

        private static IEnumerable<IMacroStabilityInwardsSoilProfile> GetMacroStabilityInwardsSoilProfiles()
        {
            yield return new TestMacroStabilityInwardsSoilProfile1D(nameof(MacroStabilityInwardsSoilProfile1D));
            yield return MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D();
        }
    }
}