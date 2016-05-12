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
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class StochasticSoilModelCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () => stochasticSoilModel.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsStochasticSoilModelEntityWithPropertiesSet()
        {
            // Setup
            string testName = "testName";
            string testSegmentName = "testSegmentName";
            var stochasticSoilModel = new StochasticSoilModel(-1, testName, testSegmentName);
            var collector = new CreateConversionCollector();

            // Call
            var entity = stochasticSoilModel.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(testSegmentName, entity.SegmentName);
            Assert.IsEmpty(entity.StochasticSoilProfileEntities);
        }   

        [Test]
        public void Create_WithStochasticSoilProfiles_ReturnsStochasticSoilModelEntityWithPropertiesAndStochasticSoilProfileEntitiesSet()
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel(-1, "testName", "testSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(50, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            });
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(50, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            });
            var collector = new CreateConversionCollector();

            // Call
            var entity = stochasticSoilModel.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.StochasticSoilProfileEntities.Count);
        }   
    }
}