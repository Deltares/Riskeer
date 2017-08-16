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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.Piping;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class StochasticSoilProfileEntityReadExtensionsTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StochasticSoilProfileEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewStochasticSoilProfileWithPropertiesSetAndEntityRegistered()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var entity = new StochasticSoilProfileEntity
            {
                Probability = probability,
                SoilProfileEntity = new SoilProfileEntity
                {
                    SourceType = (byte) random.NextEnumValue<SoilProfileType>(),
                    SoilLayerEntities =
                    {
                        new SoilLayerEntity()
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilProfile profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(probability, profile.Probability, 1e-6);
            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_WithCollectorDifferentStochasticSoilProfileEntitiesWithSameSoilProfileEntity_ReturnsStochasticSoilProfilesWithSamePipingSoilProfile()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var soilProfileEntity = new SoilProfileEntity
            {
                SourceType = (byte) random.NextEnumValue<SoilProfileType>(),
                SoilLayerEntities =
                {
                    new SoilLayerEntity()
                }
            };
            var firstEntity = new StochasticSoilProfileEntity
            {
                Probability = probability,
                SoilProfileEntity = soilProfileEntity
            };
            var secondEntity = new StochasticSoilProfileEntity
            {
                Probability = 1 - probability,
                SoilProfileEntity = soilProfileEntity
            };
            var collector = new ReadConversionCollector();

            PipingStochasticSoilProfile firstProfile = firstEntity.Read(collector);

            // Call
            PipingStochasticSoilProfile secondProfile = secondEntity.Read(collector);

            // Assert
            Assert.AreNotSame(firstProfile, secondProfile);
            Assert.AreSame(firstProfile.SoilProfile, secondProfile.SoilProfile);
        }

        [Test]
        public void Read_SameStochasticSoilProfileEntityMultipleTimes_ReturnSameStochasticSoilProfile()
        {
            // Setup
            var random = new Random(9);
            var entity = new StochasticSoilProfileEntity
            {
                SoilProfileEntity = new SoilProfileEntity
                {
                    SourceType = (byte) random.NextEnumValue<SoilProfileType>(),
                    SoilLayerEntities =
                    {
                        new SoilLayerEntity()
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilProfile profile1 = entity.Read(collector);
            PipingStochasticSoilProfile profile2 = entity.Read(collector);

            // Assert
            Assert.AreSame(profile1, profile2);
        }
    }
}