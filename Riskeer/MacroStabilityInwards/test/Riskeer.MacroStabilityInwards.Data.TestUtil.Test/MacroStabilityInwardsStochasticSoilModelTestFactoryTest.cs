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

using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelTestFactoryTest
    {
        [Test]
        public void CreateValidStochasticSoilModel_NoParameters_ReturnsStochasticSoilModel()
        {
            // Call
            MacroStabilityInwardsStochasticSoilModel model = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();

            // Assert
            Assert.IsNotNull(model);
            Assert.IsEmpty(model.Name);
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                0.5,
                0.5
            }, model.StochasticSoilProfiles.Select(p => p.Probability));
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, model.Geometry);
        }

        [Test]
        public void CreateValidStochasticSoilModel_WithNameParameter_ReturnsStochasticSoilModel()
        {
            // Setup
            const string soilModelName = "soil model name";

            // Call
            MacroStabilityInwardsStochasticSoilModel model =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(soilModelName);

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(soilModelName, model.Name);
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                0.5,
                0.5
            }, model.StochasticSoilProfiles.Select(p => p.Probability));
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, model.Geometry);
        }

        [Test]
        public void CreateValidStochasticSoilModel_WithNameAndGeometryParameters_ReturnsStochasticSoilModel()
        {
            // Setup
            const string soilModelName = "soil model name";
            var geometry = new[]
            {
                new Point2D(2, 2)
            };

            // Call
            MacroStabilityInwardsStochasticSoilModel model =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(soilModelName, geometry);

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(soilModelName, model.Name);
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                0.5,
                0.5
            }, model.StochasticSoilProfiles.Select(p => p.Probability));
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
            Assert.AreSame(geometry, model.Geometry);
        }

        [Test]
        public void CreateValidStochasticSoilModel_WithNameAndSoilProfiles_ReturnsStochasticSoilModel()
        {
            // Setup
            const string soilModelName = "soil model name";
            var stochasticSoilProfiles = new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.5, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D())
            };

            // Call
            MacroStabilityInwardsStochasticSoilModel model =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(soilModelName, stochasticSoilProfiles);

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(soilModelName, model.Name);
            CollectionAssert.AreEqual(stochasticSoilProfiles, model.StochasticSoilProfiles);
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, model.Geometry);
        }

        [Test]
        public void CreateValidStochasticSoilModel_WithSoilProfiles_ReturnsStochasticSoilModel()
        {
            // Setup
            var stochasticSoilProfiles = new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.5, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D())
            };

            // Call
            MacroStabilityInwardsStochasticSoilModel model =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(stochasticSoilProfiles);

            // Assert
            Assert.IsNotNull(model);
            Assert.IsEmpty(model.Name);
            CollectionAssert.AreEqual(stochasticSoilProfiles, model.StochasticSoilProfiles);
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, model.Geometry);
        }
    }
}