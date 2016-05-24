// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Create;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PipingSoilProfileCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () => soilProfile.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollectorAndLayers_ReturnsSoilProfileEntityWithPropertiesAndSoilLayerEntitiesSet()
        {
            // Setup
            string testName = "testName";
            double bottom = new Random(21).NextDouble();
            var layers = new []
            {
                new PipingSoilLayer(bottom + 1), 
                new PipingSoilLayer(bottom + 2) 
            };
            var soilProfile = new PipingSoilProfile(testName, bottom, layers, SoilProfileType.SoilProfile1D, -1);
            var collector = new PersistenceRegistry();

            // Call
            var entity = soilProfile.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToDecimal(bottom), entity.Bottom);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(2, entity.SoilLayerEntities.Count);
        }  

        [Test]
        public void Create_ForTheSameEntityTwice_ReturnsSameSoilProfileEntityInstance()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();
            var collector = new PersistenceRegistry();

            var firstEntity = soilProfile.Create(collector);

            // Call
            var secondEntity = soilProfile.Create(collector);

            // Assert
            Assert.AreSame(firstEntity, secondEntity);
        }  
    }
}