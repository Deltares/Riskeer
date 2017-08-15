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

using System.Linq;
using NUnit.Framework;
using Ringtoets.Piping.Data.SoilProfile;

namespace Ringtoets.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class PipingStochasticSoilModelTestFactoryTest
    {
        [Test]
        public void CreatePipingStochasticSoilModel_ExpectedPropertiesSet()
        {
            // Call
            PipingStochasticSoilModel model = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();

            // Assert
            Assert.IsInstanceOf<PipingStochasticSoilModel>(model);
            Assert.IsEmpty(model.Name);
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
            CollectionAssert.AreEquivalent(new[]
            {
                0.5,
                0.5
            }, model.StochasticSoilProfiles.Select(p => p.Probability));
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
        }

        [Test]
        public void CreatePipingStochasticSoilModelWithName_ExpectedPropertiesSet()
        {
            // Setup
            const string name = "some name";

            // Call
            PipingStochasticSoilModel model = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(name);

            // Assert
            Assert.IsInstanceOf<PipingStochasticSoilModel>(model);
            Assert.AreEqual(name, model.Name);
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
            CollectionAssert.AreEquivalent(new[]
            {
                0.5,
                0.5
            }, model.StochasticSoilProfiles.Select(p => p.Probability));
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
        }
    }
}