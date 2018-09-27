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
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.TestUtil;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.Piping;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingStochasticSoilProfileCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.4, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            TestDelegate test = () => stochasticSoilProfile.Create(null, 0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsPipingStochasticSoilProfileEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            int order = random.Next();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(probability, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            var registry = new PersistenceRegistry();

            // Call
            PipingStochasticSoilProfileEntity entity = stochasticSoilProfile.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(probability, entity.Probability);
            Assert.IsNotNull(entity.PipingSoilProfileEntity);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_DifferentStochasticSoilProfilesWithSamePipingSoilProfile_ReturnsPipingStochasticSoilProfileEntityWithSameSoilProfileEntitySet()
        {
            // Setup
            PipingSoilProfile testPipingSoilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            var firstStochasticSoilProfile = new PipingStochasticSoilProfile(new Random(21).NextDouble(), testPipingSoilProfile);
            var secondStochasticSoilProfile = new PipingStochasticSoilProfile(new Random(21).NextDouble(), testPipingSoilProfile);
            var registry = new PersistenceRegistry();

            // Call
            PipingStochasticSoilProfileEntity firstEntity = firstStochasticSoilProfile.Create(registry, 0);
            PipingStochasticSoilProfileEntity secondEntity = secondStochasticSoilProfile.Create(registry, 0);

            // Assert
            Assert.AreSame(firstEntity.PipingSoilProfileEntity, secondEntity.PipingSoilProfileEntity);
        }

        [Test]
        public void Create_SamePipingStochasticSoilProfileMultipleTimes_ReturnSameEntity()
        {
            // Setup
            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0.4, soilProfile);
            var registry = new PersistenceRegistry();

            // Call
            PipingStochasticSoilProfileEntity entity1 = stochasticSoilProfile.Create(registry, 0);
            PipingStochasticSoilProfileEntity entity2 = stochasticSoilProfile.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}