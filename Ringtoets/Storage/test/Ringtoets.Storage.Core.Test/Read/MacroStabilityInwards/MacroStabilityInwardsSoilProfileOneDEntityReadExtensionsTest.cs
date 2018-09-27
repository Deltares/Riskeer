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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;
using Ringtoets.Storage.Core.Read.MacroStabilityInwards;

namespace Ringtoets.Storage.Core.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileOneDEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilProfileOneDEntity();

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
            TestDelegate test = () => ((MacroStabilityInwardsSoilProfileOneDEntity) null).Read(collector);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewSoilProfileWithPropertiesSet()
        {
            // Setup
            var random = new Random(31);
            double bottom = random.NextDouble();
            var entity = new MacroStabilityInwardsSoilProfileOneDEntity
            {
                Name = nameof(MacroStabilityInwardsSoilProfileOneDEntity),
                Bottom = bottom,
                MacroStabilityInwardsSoilLayerOneDEntities =
                {
                    new MacroStabilityInwardsSoilLayerOneDEntity
                    {
                        Top = bottom + 0.5,
                        MaterialName = "A",
                        Order = 1
                    },
                    new MacroStabilityInwardsSoilLayerOneDEntity
                    {
                        Top = bottom + 1.2,
                        MaterialName = "B",
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsSoilProfile1D profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entity.Name, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, profile.Layers.Select(l => l.Data.MaterialName));
        }

        [Test]
        public void Read_WithNullValues_ReturnsSoilProfileWithNaNValues()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilProfileOneDEntity
            {
                Name = nameof(MacroStabilityInwardsSoilProfileOneDEntity),
                Bottom = null,
                MacroStabilityInwardsSoilLayerOneDEntities =
                {
                    new MacroStabilityInwardsSoilLayerOneDEntity
                    {
                        MaterialName = nameof(MacroStabilityInwardsSoilLayerOneDEntity)
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsSoilProfile1D profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entity.Name, profile.Name);
            Assert.IsNaN(profile.Bottom);
            Assert.AreEqual(entity.MacroStabilityInwardsSoilLayerOneDEntities.Count, profile.Layers.Count());

            MacroStabilityInwardsSoilLayer1D layer = profile.Layers.ElementAt(0);
            Assert.AreEqual(entity.MacroStabilityInwardsSoilLayerOneDEntities.First().MaterialName, layer.Data.MaterialName);
        }

        [Test]
        public void GivenReadObject_WhenReadCalledOnSameEntity_ThenSameObjectInstanceReturned()
        {
            // Given
            const string testName = "testName";
            var random = new Random(31);
            double bottom = random.NextDouble();
            var entity = new MacroStabilityInwardsSoilProfileOneDEntity
            {
                Name = testName,
                Bottom = bottom,
                MacroStabilityInwardsSoilLayerOneDEntities =
                {
                    new MacroStabilityInwardsSoilLayerOneDEntity
                    {
                        Top = bottom + 0.5
                    }
                }
            };
            var collector = new ReadConversionCollector();

            MacroStabilityInwardsSoilProfile1D profile = entity.Read(collector);

            // When
            MacroStabilityInwardsSoilProfile1D secondProfile = entity.Read(collector);

            // Then
            Assert.AreSame(profile, secondProfile);
        }
    }
}