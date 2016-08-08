﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Read.Piping;

using NUnit.Framework;

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class SoilProfileEntityReadExtensionsTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new SoilProfileEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithCollector_ReturnsNewPipingSoilProfileWithPropertiesSet(bool isRelevant)
        {
            // Setup
            string testName = "testName";
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            double bottom = random.NextDouble();
            var entity = new SoilProfileEntity
            {
                SoilProfileEntityId = entityId,
                Name = testName,
                Bottom = bottom,
                SoilLayerEntities =
                {
                    new SoilLayerEntity
                    {
                        Top = bottom + 0.5,
                        MaterialName = "A",
                        Order = 1
                    },
                    new SoilLayerEntity
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
            Assert.AreEqual(entityId, profile.StorageId);
            Assert.AreEqual(testName, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom, 1e-6);
            CollectionAssert.AreEqual(new[]{"B", "A"}, profile.Layers.Select(l => l.MaterialName));
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithCollectorWithoutLayers_ThrowsArgumentException(bool isRelevant)
        {
            // Setup
            var entity = new SoilProfileEntity();
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => entity.Read(collector);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Read_WithCollectorReadTwice_ReturnsSamePipingSoilProfile()
        {
            // Setup
            string testName = "testName";
            double bottom = new Random(21).NextDouble();
            var entity = new SoilProfileEntity
            {
                Name = testName,
                Bottom = bottom,
                SoilLayerEntities =
                {
                    new SoilLayerEntity{ Top = bottom + 0.5 },
                    new SoilLayerEntity{ Top = bottom + 1.2 }
                }
            };
            var collector = new ReadConversionCollector();

            PipingSoilProfile profile = entity.Read(collector);

            // Call
            var secondFailureMechanism = entity.Read(collector);

            // Assert
            Assert.AreSame(profile, secondFailureMechanism);
        }
    }
}