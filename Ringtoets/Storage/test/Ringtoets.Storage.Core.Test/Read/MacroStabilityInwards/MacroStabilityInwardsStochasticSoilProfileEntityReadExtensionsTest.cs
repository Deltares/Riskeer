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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;
using Ringtoets.Storage.Core.Read.MacroStabilityInwards;
using Ringtoets.Storage.Core.TestUtil.MacroStabilityInwards;

namespace Ringtoets.Storage.Core.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilProfileEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new MacroStabilityInwardsStochasticSoilProfileEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => ((MacroStabilityInwardsStochasticSoilProfileEntity) null).Read(collector);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void Read_WithCollectorAnd1dProfile_ReturnsStochasticSoilProfileWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            var entity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                Probability = random.NextDouble(),
                MacroStabilityInwardsSoilProfileOneDEntity = new MacroStabilityInwardsSoilProfileOneDEntity
                {
                    Name = "SoilProfile",
                    MacroStabilityInwardsSoilLayerOneDEntities =
                    {
                        new MacroStabilityInwardsSoilLayerOneDEntity()
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(stochasticSoilProfile);
            Assert.AreEqual(entity.Probability, stochasticSoilProfile.Probability, 1e-6);

            IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> profile = stochasticSoilProfile.SoilProfile;
            Assert.IsInstanceOf<MacroStabilityInwardsSoilProfile1D>(profile);
            Assert.AreEqual(entity.MacroStabilityInwardsSoilProfileOneDEntity.Name, profile.Name);
        }

        [Test]
        public void Read_WithCollectorAnd2dProfile_ReturnsStochasticSoilProfileWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            var entity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                Probability = random.NextDouble(),
                MacroStabilityInwardsSoilProfileTwoDEntity = new MacroStabilityInwardsSoilProfileTwoDEntity
                {
                    Name = "SoilProfile",
                    MacroStabilityInwardsSoilLayerTwoDEntities =
                    {
                        MacroStabilityInwardsSoilLayerTwoDEntityTestFactory.CreateMacroStabilityInwardsSoilLayerTwoDEntity()
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(stochasticSoilProfile);
            Assert.AreEqual(entity.Probability, stochasticSoilProfile.Probability, 1e-6);

            IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> profile = stochasticSoilProfile.SoilProfile;
            Assert.IsInstanceOf<MacroStabilityInwardsSoilProfile2D>(profile);
            Assert.AreEqual(entity.MacroStabilityInwardsSoilProfileTwoDEntity.Name, profile.Name);
        }

        [Test]
        public void Read_DifferentStochasticSoilProfileEntitiesWithSame1dProfile_ReturnsStochasticSoilProfilesWithSameSoilProfile()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var soilProfileOneDEntity = new MacroStabilityInwardsSoilProfileOneDEntity
            {
                Name = "SoilProfile",
                MacroStabilityInwardsSoilLayerOneDEntities =
                {
                    new MacroStabilityInwardsSoilLayerOneDEntity()
                }
            };

            var firstEntity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                Probability = probability,
                MacroStabilityInwardsSoilProfileOneDEntity = soilProfileOneDEntity
            };
            var secondEntity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                Probability = 1 - probability,
                MacroStabilityInwardsSoilProfileOneDEntity = soilProfileOneDEntity
            };
            var collector = new ReadConversionCollector();

            MacroStabilityInwardsStochasticSoilProfile firstStochasticSoilProfile = firstEntity.Read(collector);

            // Call
            MacroStabilityInwardsStochasticSoilProfile secondStochasticSoilProfile = secondEntity.Read(collector);

            // Assert
            Assert.AreNotSame(firstStochasticSoilProfile, secondStochasticSoilProfile);
            Assert.AreSame(firstStochasticSoilProfile.SoilProfile, secondStochasticSoilProfile.SoilProfile);
        }

        [Test]
        public void Read_DifferentStochasticSoilProfileEntitiesWithSame2dProfile_ReturnsStochasticSoilProfilesWithSameSoilProfile()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var soilProfileTwoDEntity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = "SoilProfile",
                MacroStabilityInwardsSoilLayerTwoDEntities =
                {
                    MacroStabilityInwardsSoilLayerTwoDEntityTestFactory.CreateMacroStabilityInwardsSoilLayerTwoDEntity()
                }
            };

            var firstEntity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                Probability = probability,
                MacroStabilityInwardsSoilProfileTwoDEntity = soilProfileTwoDEntity
            };
            var secondEntity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                Probability = 1 - probability,
                MacroStabilityInwardsSoilProfileTwoDEntity = soilProfileTwoDEntity
            };
            var collector = new ReadConversionCollector();

            MacroStabilityInwardsStochasticSoilProfile firstStochasticSoilProfile = firstEntity.Read(collector);

            // Call
            MacroStabilityInwardsStochasticSoilProfile secondStochasticSoilProfile = secondEntity.Read(collector);

            // Assert
            Assert.AreNotSame(firstStochasticSoilProfile, secondStochasticSoilProfile);
            Assert.AreSame(firstStochasticSoilProfile.SoilProfile, secondStochasticSoilProfile.SoilProfile);
        }

        [Test]
        public void GivenReadObject_WhenReadCalledOnSameEntity_ThenSameObjectInstanceReturned()
        {
            // Given
            var entity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                MacroStabilityInwardsSoilProfileOneDEntity = new MacroStabilityInwardsSoilProfileOneDEntity
                {
                    Name = "StochasticSoilProfile",
                    MacroStabilityInwardsSoilLayerOneDEntities =
                    {
                        new MacroStabilityInwardsSoilLayerOneDEntity()
                    }
                }
            };

            var collector = new ReadConversionCollector();

            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile1 = entity.Read(collector);

            // When
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile2 = entity.Read(collector);

            // Then
            Assert.AreSame(stochasticSoilProfile1, stochasticSoilProfile2);
        }
    }
}