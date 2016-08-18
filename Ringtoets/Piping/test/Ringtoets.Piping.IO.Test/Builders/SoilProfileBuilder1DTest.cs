﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Builders
{
    [TestFixture]
    public class SoilProfileBuilder1DTest
    {
        [Test]
        public void Build_WithOutLayers_ThrowsSoilProfileBuilderException()
        {
            // Setup
            var profileName = "SomeProfile";
            var builder = new SoilProfileBuilder1D(profileName, 0.0, 0);

            // Call
            TestDelegate test = () => builder.Build();

            // Assert
            Assert.Throws<SoilProfileBuilderException>(test);
        }

        [Test]
        public void Build_WithSingleLayer_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            var profileName = "SomeProfile";
            var random = new Random(22);
            var bottom = random.NextDouble();
            var top = random.NextDouble();
            const long pipingSoilProfileId = 1234L;
            var builder = new SoilProfileBuilder1D(profileName, bottom, pipingSoilProfileId);
            builder.Add(new PipingSoilLayer(top)
            {
                IsAquifer = true
            });

            // Call
            PipingSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(1, soilProfile.Layers.Count());
            Assert.AreEqual(SoilProfileType.SoilProfile1D, soilProfile.SoilProfileType);
            Assert.AreEqual(pipingSoilProfileId, soilProfile.PipingSoilProfileId);
            Assert.AreEqual(top, soilProfile.Layers.ToArray()[0].Top);
            Assert.AreEqual(bottom, soilProfile.Bottom);
        }

        [Test]
        public void Build_WithMultipleLayers_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            var profileName = "SomeProfile";
            var random = new Random(22);
            var bottom = random.NextDouble();
            var top = bottom + random.NextDouble();
            var top2 = bottom + random.NextDouble();
            const long pipingSoilProfileId = 1234L;

            var builder = new SoilProfileBuilder1D(profileName, bottom, pipingSoilProfileId);
            builder.Add(new PipingSoilLayer(top)
            {
                IsAquifer = true
            });
            builder.Add(new PipingSoilLayer(top2));

            // Call
            PipingSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(2, soilProfile.Layers.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                top,
                top2
            }, soilProfile.Layers.Select(l => l.Top));
            Assert.AreEqual(bottom, soilProfile.Bottom);
            Assert.AreEqual(pipingSoilProfileId, soilProfile.PipingSoilProfileId);
        }
    }
}