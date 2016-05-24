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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    public class StochasticSoilProfileCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilProfile = new StochasticSoilProfile(40, SoilProfileType.SoilProfile1D, -1);

            // Call
            TestDelegate test = () => stochasticSoilProfile.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsStochasticSoilProfileEntityWithPropertiesSet()
        {
            // Setup
            var probability = new Random(21).NextDouble();
            var stochasticSoilProfile = new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            var collector = new PersistenceRegistry();

            // Call
            var entity = stochasticSoilProfile.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(probability, entity.Probability);
        }   

        [Test]
        public void Create_DifferentStochasticSoilProfilesWithSamePipingSoilProfile_ReturnsStochasticSoilProfileEntityWithSameSoilProfileEntitySet()
        {
            // Setup
            var testPipingSoilProfile = new TestPipingSoilProfile();
            var firstStochasticSoilProfile = new StochasticSoilProfile(new Random(21).NextDouble(), SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = testPipingSoilProfile
            };
            var secondStochasticSoilProfile = new StochasticSoilProfile(new Random(21).NextDouble(), SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = testPipingSoilProfile
            };
            var collector = new PersistenceRegistry();

            // Call
            var firstEntity = firstStochasticSoilProfile.Create(collector);
            var secondEntity = secondStochasticSoilProfile.Create(collector);

            // Assert
            Assert.AreSame(firstEntity.SoilProfileEntity, secondEntity.SoilProfileEntity);
        }   
    }
}