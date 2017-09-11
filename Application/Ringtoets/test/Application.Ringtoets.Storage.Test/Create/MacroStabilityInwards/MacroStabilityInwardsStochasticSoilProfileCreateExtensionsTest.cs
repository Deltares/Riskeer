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
using Core.Common.Base.Geometry;
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

            var soilProfile1D =  soilProfile as MacroStabilityInwardsSoilProfile1D;
            if (soilProfile1D == null)
            {
                Assert.IsNull(entity.MacroStabilityInwardsSoilProfile1DEntity);
            }
            else
            {
                Assert.AreEqual(soilProfile1D.Name, entity.MacroStabilityInwardsSoilProfile1DEntity.Name);
            }

            var soilProfile2D =  soilProfile as MacroStabilityInwardsSoilProfile2D;
            if (soilProfile2D == null)
            {
                Assert.IsNull(entity.MacroStabilityInwardsSoilProfile2DEntity);
            }
            else
            {
                Assert.AreEqual(soilProfile2D.Name, entity.MacroStabilityInwardsSoilProfile2DEntity.Name);
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
                if (firstEntity.MacroStabilityInwardsSoilProfile1DEntity != null)
                {
                    Assert.AreNotSame(firstEntity.MacroStabilityInwardsSoilProfile1DEntity, secondEntity.MacroStabilityInwardsSoilProfile1DEntity);
                }
                if (firstEntity.MacroStabilityInwardsSoilProfile2DEntity != null)
                {
                    Assert.AreNotSame(firstEntity.MacroStabilityInwardsSoilProfile2DEntity, secondEntity.MacroStabilityInwardsSoilProfile2DEntity);
                }
            }
            else
            {
                Assert.AreSame(firstEntity.MacroStabilityInwardsSoilProfile1DEntity, secondEntity.MacroStabilityInwardsSoilProfile1DEntity);
                Assert.AreSame(firstEntity.MacroStabilityInwardsSoilProfile2DEntity, secondEntity.MacroStabilityInwardsSoilProfile2DEntity);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetMacroStabilityInwardsSoilProfiles))]
        public void Create_SameStochasticSoilProfileMultipleTimes_ReturnSameEntity(IMacroStabilityInwardsSoilProfile soilProfile)
        {
            // Setup
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.4, soilProfile);
            var registry = new PersistenceRegistry();

            // Call
            MacroStabilityInwardsStochasticSoilProfileEntity entity1 = stochasticSoilProfile.Create(registry, 0);
            MacroStabilityInwardsStochasticSoilProfileEntity entity2 = stochasticSoilProfile.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
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
            yield return new MacroStabilityInwardsSoilProfile2D(nameof(MacroStabilityInwardsSoilProfile2D), new[]
            {
                new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }), Enumerable.Empty<Ring>())
            }, new MacroStabilityInwardsPreconsolidationStress[0]);
        }
    }
}