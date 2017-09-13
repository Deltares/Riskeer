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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.MacroStabilityInwards;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileTwoDEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity();

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
            TestDelegate test = () => ((MacroStabilityInwardsSoilProfileTwoDEntity) null).Read(collector);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsSoilProfileWithPropertiesSet()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = nameof(MacroStabilityInwardsSoilProfileTwoDEntity),
                MacroStabilityInwardsSoilLayerTwoDEntities =
                {
                    new MacroStabilityInwardsSoilLayerTwoDEntity
                    {
                        MaterialName = "A",
                        Order = 1
                    },
                    new MacroStabilityInwardsSoilLayerTwoDEntity
                    {
                        MaterialName = "B",
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsSoilProfile2D profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entity.Name, profile.Name);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, profile.Layers.Select(l => l.Properties.MaterialName));
        }

        [Test]
        public void Read_WithNullValues_ReturnsSoilProfileWithNaNValues()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = nameof(MacroStabilityInwardsSoilProfileTwoDEntity),
                MacroStabilityInwardsSoilLayerTwoDEntities =
                {
                    new MacroStabilityInwardsSoilLayerTwoDEntity
                    {
                        MaterialName = nameof(MacroStabilityInwardsSoilLayerTwoDEntity)
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            MacroStabilityInwardsSoilProfile2D profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entity.Name, profile.Name);
            Assert.AreEqual(1, profile.Layers.Count());

            MacroStabilityInwardsSoilLayer2D layer = profile.Layers.ElementAt(0);
            Assert.AreEqual(entity.MacroStabilityInwardsSoilLayerTwoDEntities.First().MaterialName, layer.Properties.MaterialName);
        }

        [Test]
        public void Read_WithCollectorWithoutLayers_ThrowsArgumentException()
        {
            // Setup
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = "Name"
            };
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => entity.Read(collector);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Read_WithCollectorReadTwice_ReturnsSameSoilProfile()
        {
            // Setup
            const string testName = "testName";
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = testName,
                MacroStabilityInwardsSoilLayerTwoDEntities =
                {
                    new MacroStabilityInwardsSoilLayerTwoDEntity(),
                    new MacroStabilityInwardsSoilLayerTwoDEntity()
                }
            };
            var collector = new ReadConversionCollector();

            MacroStabilityInwardsSoilProfile2D profile = entity.Read(collector);

            // Call
            MacroStabilityInwardsSoilProfile2D secondProfile = entity.Read(collector);

            // Assert
            Assert.AreSame(profile, secondProfile);
        }
    }
}