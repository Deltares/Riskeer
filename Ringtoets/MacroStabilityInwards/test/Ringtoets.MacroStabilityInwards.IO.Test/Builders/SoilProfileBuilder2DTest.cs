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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.Builders;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Builders
{
    [TestFixture]
    public class SoilProfileBuilder2DTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("name")]
        public void Constructor_WithNameInvalidX_ThrowsArgumentException(string name)
        {
            // Call
            TestDelegate test = () => new SoilProfileBuilder2D(name, double.NaN, 0);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            string message = $"Geen geldige X waarde gevonden om intersectie te maken uit 2D profiel '{name}'.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("name")]
        public void Constructor_WithNameValidX_DoesNotThrow(string name)
        {
            // Call
            TestDelegate test = () => new SoilProfileBuilder2D(name, 0.0, 0);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Add_LayerWithVerticalLineOnXInXml_ThrowsSoilProfileBuilderException()
        {
            // Setup
            const string profileName = "SomeProfile";
            const double atX = 0.0;
            var builder = new SoilProfileBuilder2D(profileName, atX, 0);

            var soilLayer = new SoilLayer2D
            {
                OuterLoop = new List<Segment2D>
                {
                    new Segment2D(new Point2D(atX, 0.0),
                                  new Point2D(atX, 1.0)),
                    new Segment2D(new Point2D(atX, 1.0),
                                  new Point2D(0.5, 0.5)),
                    new Segment2D(new Point2D(0.5, 0.5),
                                  new Point2D(atX, 0.0))
                }
            };

            // Call
            TestDelegate test = () => builder.Add(soilLayer);

            // Assert
            var exception = Assert.Throws<SoilProfileBuilderException>(test);
            Assert.IsInstanceOf<SoilLayerConversionException>(exception.InnerException);
            string message = $"Er kan geen 1D-profiel bepaald worden wanneer segmenten in een 2D laag verticaal lopen op de gekozen positie: x = {atX}.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void Build_WithOutLayers_ThrowsSoilProfileBuilderException()
        {
            // Setup
            const string profileName = "SomeProfile";
            var builder = new SoilProfileBuilder2D(profileName, 0.0, 0);

            // Call
            TestDelegate test = () => builder.Build();

            // Assert
            Assert.Throws<SoilProfileBuilderException>(test);
        }

        [Test]
        public void Build_WithSingleLayerOnlyOuterLoop_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            const string profileName = "SomeProfile";
            const long soilProfileId = 1234L;
            var builder = new SoilProfileBuilder2D(profileName, 0.0, soilProfileId);
            var firstPoint = new Point2D(-0.5, 1.0);
            var secondPoint = new Point2D(0.5, 1.0);
            var thirdPoint = new Point2D(0.5, -1.0);
            var fourthPoint = new Point2D(-0.5, -1.0);
            builder.Add(new SoilLayer2D
            {
                OuterLoop = new List<Segment2D>
                {
                    new Segment2D(firstPoint, secondPoint),
                    new Segment2D(secondPoint, thirdPoint),
                    new Segment2D(thirdPoint, fourthPoint),
                    new Segment2D(fourthPoint, firstPoint)
                },
                IsAquifer = 1.0
            });

            // Call
            MacroStabilityInwardsSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(1, soilProfile.Layers.Count());
            Assert.AreEqual(1.0, soilProfile.Layers.ToArray()[0].Top);
            Assert.AreEqual(-1.0, soilProfile.Bottom);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, soilProfile.SoilProfileType);
            Assert.AreEqual(soilProfileId, soilProfile.MacroStabilityInwardsSoilProfileId);
        }

        [Test]
        public void Build_WithMultipleLayersOnlyOuterLoop_ReturnsProfileWithBottomAndALayers()
        {
            // Setup
            const string profileName = "SomeProfile";
            const long soilProfileId = 1234L;
            var builder = new SoilProfileBuilder2D(profileName, 1.0, soilProfileId);
            builder.Add(new SoilLayer2D
            {
                OuterLoop = Segment2DLoopCollectionHelper.CreateFromString(string.Join(Environment.NewLine,
                                                                                       "10",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "1.2",
                                                                                       "4.3",
                                                                                       "..."))
            }).Add(new SoilLayer2D
            {
                OuterLoop = Segment2DLoopCollectionHelper.CreateFromString(string.Join(Environment.NewLine,
                                                                                       "10",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "4.3",
                                                                                       "...",
                                                                                       "1.2",
                                                                                       "...",
                                                                                       "..."))
            }).Add(new SoilLayer2D
            {
                OuterLoop = Segment2DLoopCollectionHelper.CreateFromString(string.Join(Environment.NewLine,
                                                                                       "10",
                                                                                       "...",
                                                                                       "1.2",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "4.3",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...",
                                                                                       "...")),
                IsAquifer = 1.0
            });

            // Call
            MacroStabilityInwardsSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(3, soilProfile.Layers.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                2.0,
                4.0,
                8.0
            }, soilProfile.Layers.Select(rl => rl.Top));
            Assert.AreEqual(1.0, soilProfile.Bottom);
            Assert.AreEqual(soilProfileId, soilProfile.MacroStabilityInwardsSoilProfileId);
        }

        [Test]
        public void Build_WithLayerFilledWithOtherLayer_ReturnsProfileWithBottomAndALayers()
        {
            // Setup
            const string profileName = "SomeProfile";
            const long soilProfileId = 1234L;
            var builder = new SoilProfileBuilder2D(profileName, 2.0, soilProfileId);
            List<Segment2D> loopHole = Segment2DLoopCollectionHelper.CreateFromString(string.Join(Environment.NewLine,
                                                                                                  "5",
                                                                                                  ".....",
                                                                                                  ".4.1.",
                                                                                                  ".3.2.",
                                                                                                  ".....",
                                                                                                  "....."));
            var soilLayer2D = new SoilLayer2D
            {
                OuterLoop = Segment2DLoopCollectionHelper.CreateFromString(string.Join(Environment.NewLine,
                                                                                       "5",
                                                                                       "2...3",
                                                                                       ".....",
                                                                                       ".....",
                                                                                       ".....",
                                                                                       "1...4")),
                IsAquifer = 1.0
            };
            soilLayer2D.AddInnerLoop(loopHole);
            builder.Add(soilLayer2D).Add(new SoilLayer2D
            {
                OuterLoop = loopHole
            });

            // Call
            MacroStabilityInwardsSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(3, soilProfile.Layers.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                4.0,
                3.0,
                2.0
            }, soilProfile.Layers.Select(rl => rl.Top));
            Assert.AreEqual(0.0, soilProfile.Bottom);
            Assert.AreEqual(soilProfileId, soilProfile.MacroStabilityInwardsSoilProfileId);
        }
    }
}