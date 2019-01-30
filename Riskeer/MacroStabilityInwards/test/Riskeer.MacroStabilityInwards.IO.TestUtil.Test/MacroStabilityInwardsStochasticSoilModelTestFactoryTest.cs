// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.MacroStabilityInwards.IO.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelTestFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsStochasticSoilModelWithGeometry_WithNameAndStochasticSoilProfiles_ReturnsStochasticSoilModel()
        {
            // Setup
            const string soilModelName = "some name";

            var random = new Random(21);

            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            var stochasticSoilProfiles = new[]
            {
                new StochasticSoilProfile(random.NextDouble(), soilProfile)
            };

            // Call
            StochasticSoilModel model =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry(soilModelName,
                                                                                                                               stochasticSoilProfiles);

            // Assert
            Assert.AreEqual(soilModelName, model.Name);
            Assert.AreEqual(FailureMechanismType.Stability, model.FailureMechanismType);
            CollectionAssert.AreEqual(stochasticSoilProfiles, model.StochasticSoilProfiles);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            }, model.Geometry);
        }

        [Test]
        public void CreateMacroStabilityInwardsStochasticSoilModelWithGeometry_WithStochasticSoilProfiles_ReturnsStochasticSoilModel()
        {
            // Setup 
            var random = new Random(21);

            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            var stochasticSoilProfiles = new[]
            {
                new StochasticSoilProfile(random.NextDouble(), soilProfile)
            };

            // Call
            StochasticSoilModel model =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateMacroStabilityInwardsStochasticSoilModelWithGeometry(stochasticSoilProfiles);

            // Assert
            Assert.AreEqual("Macro Stability Inwards Stochastic Soil Model", model.Name);
            Assert.AreEqual(FailureMechanismType.Stability, model.FailureMechanismType);
            CollectionAssert.AreEqual(stochasticSoilProfiles, model.StochasticSoilProfiles);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            }, model.Geometry);
        }
    }
}