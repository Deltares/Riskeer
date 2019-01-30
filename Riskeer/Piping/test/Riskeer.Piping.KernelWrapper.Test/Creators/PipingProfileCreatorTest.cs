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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deltares.WTIPiping;
using NUnit.Framework;
using Riskeer.Piping.KernelWrapper.Creators;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class PipingProfileCreatorTest
    {
        [Test]
        public void Create_ProfileWithOneLayer_ReturnsProfileWithSingleLayer()
        {
            // Setup
            var random = new Random(22);
            double expectedTop = random.NextDouble();
            double expectedBottom = expectedTop - random.NextDouble();

            var layers = new[]
            {
                new PipingSoilLayer(expectedTop)
                {
                    IsAquifer = true
                }
            };
            var soilProfile = new PipingSoilProfile(string.Empty, expectedBottom, layers, SoilProfileType.SoilProfile1D);

            // Call
            PipingProfile actual = PipingProfileCreator.Create(soilProfile);

            // Assert
            Assert.IsNotNull(actual.Layers);
            Assert.IsNotNull(actual.BottomAquiferLayer);
            Assert.IsNotNull(actual.TopAquiferLayer);
            Assert.AreEqual(1, actual.Layers.Count);
            Assert.AreEqual(expectedTop, actual.Layers[0].TopLevel);
            Assert.AreEqual(expectedTop, actual.TopLevel);
            Assert.AreEqual(expectedBottom, actual.BottomLevel);

            PipingLayer pipingLayer = actual.Layers.First();
            Assert.IsTrue(pipingLayer.IsAquifer);
        }

        [Test]
        public void Create_ProfileWithDecreasingTops_ReturnsProfileWithMultipleLayers()
        {
            // Setup
            var random = new Random(22);
            double expectedTopA = random.NextDouble();
            double expectedTopB = expectedTopA - random.NextDouble();
            double expectedTopC = expectedTopB - random.NextDouble();
            double expectedBottom = expectedTopC - random.NextDouble();
            var layers = new[]
            {
                new PipingSoilLayer(expectedTopA)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(expectedTopB),
                new PipingSoilLayer(expectedTopC)
            };

            var soilProfile = new PipingSoilProfile(string.Empty, expectedBottom, layers, SoilProfileType.SoilProfile1D);

            // Call
            PipingProfile actual = PipingProfileCreator.Create(soilProfile);

            // Assert
            Assert.AreEqual(3, actual.Layers.Count);
            IEnumerable expectedAquifers = new[]
            {
                true,
                false,
                false
            };
            CollectionAssert.AreEqual(expectedAquifers, actual.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[]
            {
                expectedTopA,
                expectedTopB,
                expectedTopC
            }, actual.Layers.Select(l => l.TopLevel));
            Assert.AreEqual(expectedBottom, actual.BottomLevel);
        }

        [Test]
        public void Create_ProfileWithMultipleLayers_ReturnsProfileWithOrderedMultipleLayers()
        {
            // Setup
            var random = new Random(22);
            double expectedTopA = random.NextDouble();
            double expectedTopB = random.NextDouble() + expectedTopA;
            double expectedTopC = random.NextDouble() + expectedTopB;
            double expectedBottom = expectedTopA - random.NextDouble();
            var layers = new[]
            {
                new PipingSoilLayer(expectedTopA)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(expectedTopB),
                new PipingSoilLayer(expectedTopC)
            };
            var soilProfile = new PipingSoilProfile(string.Empty, expectedBottom, layers, SoilProfileType.SoilProfile1D);

            // Precondition
            CollectionAssert.AreNotEqual(layers, layers.OrderByDescending(l => l.Top), "Layer collection should not be in descending order by the Top property.");

            // Call
            PipingProfile actual = PipingProfileCreator.Create(soilProfile);

            // Assert
            IEnumerable<PipingLayer> ordered = actual.Layers.OrderByDescending(l => l.TopLevel);
            CollectionAssert.AreEqual(ordered, actual.Layers);

            Assert.AreEqual(3, actual.Layers.Count);
            IEnumerable expectedAquifers = new[]
            {
                false,
                false,
                true
            };
            CollectionAssert.AreEqual(expectedAquifers, actual.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[]
            {
                expectedTopC,
                expectedTopB,
                expectedTopA
            }, actual.Layers.Select(l => l.TopLevel));
            Assert.AreEqual(expectedBottom, actual.BottomLevel);
        }
    }
}