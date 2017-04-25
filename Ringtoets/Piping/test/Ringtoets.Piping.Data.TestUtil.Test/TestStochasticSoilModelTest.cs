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

using System.Linq;
using NUnit.Framework;

namespace Ringtoets.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class TestStochasticSoilModelTest
    {
        [Test]
        public void DefaultConstructor_ExpectedPropertiesSet()
        {
            // Call
            var model = new TestStochasticSoilModel();

            // Assert
            Assert.IsInstanceOf<StochasticSoilModel>(model);
            Assert.IsEmpty(model.Name);
            Assert.IsEmpty(model.SegmentName);
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
            CollectionAssert.AreEquivalent(new[]
            {
                0.5,
                0.5
            }, model.StochasticSoilProfiles.Select(p => p.Probability));
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
        }

        [Test]
        public void Constructor_WitValidName_ExpectedPropertiesSet()
        {
            // Setup
            const string name = "some name";

            // Call
            var model = new TestStochasticSoilModel(name);

            // Assert
            Assert.IsInstanceOf<StochasticSoilModel>(model);
            Assert.AreEqual(name, model.Name);
            Assert.IsEmpty(model.SegmentName);
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