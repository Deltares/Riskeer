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
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Geometry;
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
            var random = new Random(31);

            var outerRingA = new Ring(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            });
            var outerRingB = new Ring(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            });

            var point2DXmlSerializer = new Point2DXmlSerializer();
            var ringXmlSerializer = new RingXmlSerializer();
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = nameof(MacroStabilityInwardsSoilProfileTwoDEntity),
                MacroStabilityInwardsSoilLayerTwoDEntities =
                {
                    new MacroStabilityInwardsSoilLayerTwoDEntity
                    {
                        MaterialName = "A",
                        OuterRingXml = point2DXmlSerializer.ToXml(outerRingA.Points),
                        HolesXml = ringXmlSerializer.ToXml(new Ring[0]),
                        Order = 1
                    },
                    new MacroStabilityInwardsSoilLayerTwoDEntity
                    {
                        MaterialName = "B",
                        OuterRingXml = point2DXmlSerializer.ToXml(outerRingB.Points),
                        HolesXml = ringXmlSerializer.ToXml(new Ring[0]),
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

            CollectionAssert.AreEqual(new[]
            {
                outerRingB,
                outerRingA
            }, profile.Layers.Select(l => l.OuterRing));

            CollectionAssert.AreEqual(new[]
            {
                new Ring[0],
                new Ring[0]
            }, profile.Layers.Select(l => l.Holes));
        }

        [Test]
        public void GivenReadObject_WhenReadCalledOnSameEntity_ThenSameObjectInstanceReturned()
        {
            // Given
            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = "testName",
                MacroStabilityInwardsSoilLayerTwoDEntities =
                {
                    MacroStabilityInwardsSoilLayerTwoDEntityTestFactory.CreateMacroStabilityInwardsSoilLayerTwoDEntity()
                }
            };
            var collector = new ReadConversionCollector();

            MacroStabilityInwardsSoilProfile2D profile = entity.Read(collector);

            // When
            MacroStabilityInwardsSoilProfile2D secondProfile = entity.Read(collector);

            // Then
            Assert.AreSame(profile, secondProfile);
        }
    }
}