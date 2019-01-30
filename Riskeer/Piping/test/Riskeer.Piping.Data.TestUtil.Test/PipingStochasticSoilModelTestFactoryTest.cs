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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class PipingStochasticSoilModelTestFactoryTest
    {
        [Test]
        public void CreatePipingStochasticSoilModel_ReturnsPipingStochasticSoilModel()
        {
            // Call
            PipingStochasticSoilModel model = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();

            // Assert
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
        public void CreatePipingStochasticSoilModel_WithName_ReturnsPipingStochasticSoilModel()
        {
            // Setup
            const string name = "some name";

            // Call
            PipingStochasticSoilModel model = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(name);

            // Assert
            Assert.AreEqual(name, model.Name);
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
        public void CreatePipingStochasticSoilModel_WithNameAndGeometry_ReturnsPipingStochasticSoilModel()
        {
            // Setup
            var random = new Random(21);
            const string name = "some name";
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            PipingStochasticSoilModel model = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(name, geometry);

            // Assert
            Assert.AreEqual(name, model.Name);
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
        public void CreatePipingStochasticSoilModel_WithNameAndStochasticSoilProfiles_ReturnsPipingStochasticSoilModel()
        {
            // Setup
            var random = new Random(21);
            const string name = "some name";
            var stochasticProfiles = new[]
            {
                new PipingStochasticSoilProfile(random.NextDouble(), PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            };

            // Call
            PipingStochasticSoilModel model = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(name, stochasticProfiles);

            // Assert
            Assert.AreEqual(name, model.Name);
            CollectionAssert.AreEqual(stochasticProfiles, model.StochasticSoilProfiles);
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, model.Geometry);
        }

        [Test]
        public void CreatePipingStochasticSoilModel_WithStochasticSoilProfiles_ReturnsPipingStochasticSoilModel()
        {
            // Setup
            var random = new Random(21);
            var stochasticProfiles = new[]
            {
                new PipingStochasticSoilProfile(random.NextDouble(), PipingSoilProfileTestFactory.CreatePipingSoilProfile())
            };

            // Call
            PipingStochasticSoilModel model = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(stochasticProfiles);

            // Assert
            Assert.IsEmpty(model.Name);
            CollectionAssert.AreEqual(stochasticProfiles, model.StochasticSoilProfiles);
            CollectionAssert.AllItemsAreNotNull(model.StochasticSoilProfiles.Select(p => p.SoilProfile));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, model.Geometry);
        }
    }
}