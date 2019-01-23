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
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingSoilProfileCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();

            // Call
            TestDelegate test = () => soilProfile.Create(null);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithCollectorAndLayers_ReturnsPipingSoilProfileEntityWithPropertiesAndPipingSoilLayerEntitiesSet()
        {
            // Setup
            var random = new Random(21);

            const string testName = "testName";
            double bottom = random.NextDouble();
            var layers = new[]
            {
                new PipingSoilLayer(bottom + 1),
                new PipingSoilLayer(bottom + 2)
            };
            var soilProfileType = random.NextEnumValue<SoilProfileType>();
            var soilProfile = new PipingSoilProfile(testName, bottom, layers, soilProfileType);
            var registry = new PersistenceRegistry();

            // Call
            PipingSoilProfileEntity entity = soilProfile.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(bottom, entity.Bottom);
            Assert.AreEqual(Convert.ToByte(soilProfileType), entity.SourceType);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(2, entity.PipingSoilLayerEntities.Count);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var random = new Random(21);
            const string testName = "testName";
            var layers = new[]
            {
                new PipingSoilLayer(1),
                new PipingSoilLayer(2)
            };
            var soilProfile = new PipingSoilProfile(testName, 0, layers, random.NextEnumValue<SoilProfileType>());
            var registry = new PersistenceRegistry();

            // Call
            PipingSoilProfileEntity entity = soilProfile.Create(registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(testName, entity.Name);
        }

        [Test]
        public void Create_ForTheSameEntityTwice_ReturnsSamePipingSoilProfileEntityInstance()
        {
            // Setup
            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            var registry = new PersistenceRegistry();

            PipingSoilProfileEntity firstEntity = soilProfile.Create(registry);

            // Call
            PipingSoilProfileEntity secondEntity = soilProfile.Create(registry);

            // Assert
            Assert.AreSame(firstEntity, secondEntity);
        }
    }
}