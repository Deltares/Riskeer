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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;
using Ringtoets.Storage.Core.Read.Piping;

namespace Ringtoets.Storage.Core.Test.Read.Piping
{
    [TestFixture]
    public class PipingSoilProfileEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingSoilProfileEntity();

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
            TestDelegate test = () => ((PipingSoilProfileEntity) null).Read(collector);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewPipingSoilProfileWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            double bottom = random.NextDouble();
            var sourceType = random.NextEnumValue<SoilProfileType>();
            var entity = new PipingSoilProfileEntity
            {
                Name = "testName",
                Bottom = bottom,
                SourceType = Convert.ToByte(sourceType),
                PipingSoilLayerEntities =
                {
                    new PipingSoilLayerEntity
                    {
                        Top = bottom + 0.5,
                        MaterialName = "A",
                        Order = 1
                    },
                    new PipingSoilLayerEntity
                    {
                        Top = bottom + 1.2,
                        MaterialName = "B",
                        Order = 0
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingSoilProfile profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entity.Name, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom, 1e-6);
            Assert.AreEqual(sourceType, profile.SoilProfileSourceType);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, profile.Layers.Select(l => l.MaterialName));
        }

        [Test]
        public void Read_WithNullValues_ReturnsPipingSoilProfileWithNaNValues()
        {
            // Setup
            var entity = new PipingSoilProfileEntity
            {
                Name = nameof(PipingSoilProfileEntity),
                PipingSoilLayerEntities =
                {
                    new PipingSoilLayerEntity
                    {
                        MaterialName = nameof(PipingSoilLayerEntity)
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingSoilProfile profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entity.Name, profile.Name);
            Assert.IsNaN(profile.Bottom);
            Assert.AreEqual(entity.PipingSoilLayerEntities.Count, profile.Layers.Count());

            PipingSoilLayer layer = profile.Layers.ElementAt(0);
            Assert.AreEqual(entity.PipingSoilLayerEntities.First().MaterialName, layer.MaterialName);
        }

        [Test]
        public void GivenReadObject_WhenReadCalledOnSameEntity_ThenSameObjectInstanceReturned()
        {
            // Given
            var random = new Random(21);
            var entity = new PipingSoilProfileEntity
            {
                Name = "testName",
                Bottom = random.NextDouble(),
                SourceType = Convert.ToByte(random.NextEnumValue<SoilProfileType>()),
                PipingSoilLayerEntities =
                {
                    new PipingSoilLayerEntity
                    {
                        Top = random.NextDouble() + 1
                    }
                }
            };
            var collector = new ReadConversionCollector();

            PipingSoilProfile profile = entity.Read(collector);

            // When
            PipingSoilProfile secondProfile = entity.Read(collector);

            // Then
            Assert.AreSame(profile, secondProfile);
        }
    }
}