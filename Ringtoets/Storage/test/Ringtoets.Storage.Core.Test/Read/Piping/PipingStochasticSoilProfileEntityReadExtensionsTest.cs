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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.Piping;

namespace Ringtoets.Storage.Core.Test.Read.Piping
{
    [TestFixture]
    public class PipingStochasticSoilProfileEntityReadExtensionsTest
    {
        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingStochasticSoilProfileEntity();

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
            TestDelegate test = () => ((PipingStochasticSoilProfileEntity) null).Read(collector);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("entity", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewStochasticSoilProfileWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            var entity = new PipingStochasticSoilProfileEntity
            {
                Probability = random.NextDouble(),
                PipingSoilProfileEntity = new PipingSoilProfileEntity
                {
                    Name = "StochasticSoilProfile",
                    SourceType = Convert.ToByte(random.NextEnumValue<SoilProfileType>()),
                    PipingSoilLayerEntities =
                    {
                        new PipingSoilLayerEntity()
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            PipingStochasticSoilProfile profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entity.Probability, profile.Probability, 1e-6);
        }

        [Test]
        public void Read_DifferentStochasticSoilProfileEntitiesWithSameSoilProfileEntity_ReturnsStochasticSoilProfilesWithSamePipingSoilProfile()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var soilProfileEntity = new PipingSoilProfileEntity
            {
                Name = "StochasticSoilProfile",
                SourceType = Convert.ToByte(random.NextEnumValue<SoilProfileType>()),
                PipingSoilLayerEntities =
                {
                    new PipingSoilLayerEntity()
                }
            };
            var firstEntity = new PipingStochasticSoilProfileEntity
            {
                Probability = probability,
                PipingSoilProfileEntity = soilProfileEntity
            };
            var secondEntity = new PipingStochasticSoilProfileEntity
            {
                Probability = 1 - probability,
                PipingSoilProfileEntity = soilProfileEntity
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
        public void GivenReadObject_WhenReadCalledOnSameEntity_ThenSameObjectInstanceReturned()
        {
            // Given
            var random = new Random(9);
            var entity = new PipingStochasticSoilProfileEntity
            {
                PipingSoilProfileEntity = new PipingSoilProfileEntity
                {
                    Name = "StochasticSoilProfile",
                    SourceType = Convert.ToByte(random.NextEnumValue<SoilProfileType>()),
                    PipingSoilLayerEntities =
                    {
                        new PipingSoilLayerEntity()
                    }
                }
            };

            var collector = new ReadConversionCollector();

            PipingStochasticSoilProfile profile1 = entity.Read(collector);

            // When
            PipingStochasticSoilProfile profile2 = entity.Read(collector);

            // Then
            Assert.AreSame(profile1, profile2);
        }
    }
}