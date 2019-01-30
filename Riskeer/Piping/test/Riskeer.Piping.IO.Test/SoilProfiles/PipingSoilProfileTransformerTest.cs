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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.IO.SoilProfiles;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.IO.Test.SoilProfiles
{
    [TestFixture]
    public class PipingSoilProfileTransformerTest
    {
        [Test]
        public void Transform_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingSoilProfileTransformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Transform_InvalidSoilProfile_ThrowsImportedDataTransformException()
        {
            // Setup
            var invalidType = new TestSoilProfile();

            // Call
            TestDelegate test = () => PipingSoilProfileTransformer.Transform(invalidType);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            const string message = "De ondergrondschematisatie van het type 'TestSoilProfile' is niet ondersteund. " +
                                   "Alleen ondergrondschematisaties van het type 'SoilProfile1D' of 'SoilProfile2D' zijn ondersteund.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void Transform_SoilProfile2DWithoutIntersection_ThrowsImportedDataTransformException()
        {
            // Setup
            const string name = "name";
            var profile = new SoilProfile2D(0, name, new[]
            {
                new SoilLayer2D(new SoilLayer2DLoop(new Segment2D[0]), Enumerable.Empty<SoilLayer2D>())
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            TestDelegate test = () => PipingSoilProfileTransformer.Transform(profile);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Geen geldige X waarde gevonden om intersectie te maken uit 2D profiel '{name}'.",
                            exception.Message);
        }

        [Test]
        public void Transform_ValidSoilProfile2D_ReturnsExpectedPipingSoilProfile()
        {
            // Setup
            const string name = "name";
            const double bottom = 0.5;
            const double intersectionX = 1.0;

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new List<Segment2D[]>(),
                                                                         new List<Segment2D>
                                                                         {
                                                                             new Segment2D(new Point2D(1.0, bottom),
                                                                                           new Point2D(1.2, 1)),
                                                                             new Segment2D(new Point2D(1.2, 1),
                                                                                           new Point2D(1.0, bottom))
                                                                         });
            var profile = new SoilProfile2D(0, name, new[]
            {
                layer
            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = intersectionX
            };

            // Call
            PipingSoilProfile transformed = PipingSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(name, transformed.Name);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, transformed.SoilProfileSourceType);
            Assert.AreEqual(bottom, transformed.Bottom);

            double bottomOut;
            IEnumerable<PipingSoilLayer> actualPipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, intersectionX, out bottomOut);

            AssertPipingSoilLayers(actualPipingSoilLayers, transformed.Layers);
        }

        [Test]
        public void Transform_SoilProfile2DLayerWithVerticalLineOnXInXml_ThrowsImportedDataTransformException()
        {
            // Setup
            const string profileName = "SomeProfile";
            const double atX = 0.0;

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                new List<Segment2D[]>(),
                new List<Segment2D>
                {
                    new Segment2D(new Point2D(atX, 0.0),
                                  new Point2D(atX, 1.0)),
                    new Segment2D(new Point2D(atX, 1.0),
                                  new Point2D(0.5, 0.5)),
                    new Segment2D(new Point2D(0.5, 0.5),
                                  new Point2D(atX, 0.0))
                });

            var profile = new SoilProfile2D(0, profileName, new[]
            {
                layer
            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = atX
            };

            // Call
            TestDelegate test = () => PipingSoilProfileTransformer.Transform(profile);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            string message = $"Er is een fout opgetreden bij het inlezen van grondlaag '{layer.MaterialName}': " +
                             "Er kan geen 1D-profiel bepaald worden wanneer segmenten in een 2D " +
                             $"laag verticaal lopen op de gekozen positie: x = {atX}.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void Transform_SoilProfile2DWithoutIntersectionX_ThrowsImportedDataTransformException()
        {
            // Setup
            const string profileName = "SomeProfile";
            var profile = new SoilProfile2D(0, profileName, new[]
            {
                new SoilLayer2D(new SoilLayer2DLoop(new Segment2D[0]), Enumerable.Empty<SoilLayer2D>())
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            TestDelegate test = () => PipingSoilProfileTransformer.Transform(profile);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = $"Geen geldige X waarde gevonden om intersectie te maken uit 2D profiel '{profileName}'.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Transform_InvalidSoilProfile2D_ThrowsImportedDataTransformException()
        {
            // Setup
            var random = new Random(21);
            var profile = new SoilProfile2D(0, "A profile name", Enumerable.Empty<SoilLayer2D>(), Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = random.NextDouble()
            };

            // Call
            TestDelegate test = () => PipingSoilProfileTransformer.Transform(profile);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentException>(innerException);
            string expectedMessage = CreateExpectedErrorMessage(profile.Name, innerException.Message);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Transform_SoilProfile2DWithSingleLayerOnlyOuterLoop_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            const string profileName = "SomeProfile";
            var firstPoint = new Point2D(-0.5, 1.0);
            var secondPoint = new Point2D(0.5, 1.0);
            var thirdPoint = new Point2D(0.5, -1.0);
            var fourthPoint = new Point2D(-0.5, -1.0);

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                new List<Segment2D[]>(),
                new List<Segment2D>
                {
                    new Segment2D(firstPoint, secondPoint),
                    new Segment2D(secondPoint, thirdPoint),
                    new Segment2D(thirdPoint, fourthPoint),
                    new Segment2D(fourthPoint, firstPoint)
                });

            var profile = new SoilProfile2D(0, profileName, new[]
            {
                layer
            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = 0.0
            };

            // Call
            PipingSoilProfile transformed = PipingSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profileName, transformed.Name);
            Assert.AreEqual(1, transformed.Layers.Count());
            Assert.AreEqual(1.0, transformed.Layers.ToArray()[0].Top);
            Assert.AreEqual(-1.0, transformed.Bottom);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, transformed.SoilProfileSourceType);
        }

        [Test]
        public void Transform_SoilProfile2DWithMultipleLayersOnlyOuterLoop_ReturnsProfileWithBottomAndLayers()
        {
            // Setup
            const string profileName = "SomeProfile";
            const long pipingSoilProfileId = 1234L;

            var profile = new SoilProfile2D(pipingSoilProfileId, profileName,
                                            new List<SoilLayer2D>
                                            {
                                                SoilLayer2DTestFactory.CreateSoilLayer2D(
                                                    new List<Segment2D[]>(),
                                                    Segment2DLoopCollectionHelper.CreateFromString(
                                                        string.Join(Environment.NewLine,
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
                                                                    "..."))),
                                                SoilLayer2DTestFactory.CreateSoilLayer2D(
                                                    new List<Segment2D[]>(),
                                                    Segment2DLoopCollectionHelper.CreateFromString(
                                                        string.Join(Environment.NewLine,
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
                                                                    "..."))),
                                                SoilLayer2DTestFactory.CreateSoilLayer2D(
                                                    new List<Segment2D[]>(),
                                                    Segment2DLoopCollectionHelper.CreateFromString(
                                                        string.Join(Environment.NewLine,
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
                                                                    "...")))
                                            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = 1.0
            };

            // Call
            PipingSoilProfile transformed = PipingSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profileName, transformed.Name);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, transformed.SoilProfileSourceType);
            Assert.AreEqual(3, transformed.Layers.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                2.0,
                4.0,
                8.0
            }, transformed.Layers.Select(rl => rl.Top));
            Assert.AreEqual(1.0, transformed.Bottom);
        }

        [Test]
        public void Transform_SoilProfile2DWithLayerFilledWithOtherLayer_ReturnsProfileWithBottomAndLayers()
        {
            // Setup
            const string profileName = "SomeProfile";
            const long pipingSoilProfileId = 1234L;
            List<Segment2D> loopHole = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "5",
                            ".....",
                            ".4.1.",
                            ".3.2.",
                            ".....",
                            "....."));

            SoilLayer2D soilLayer2D = SoilLayer2DTestFactory.CreateSoilLayer2D(
                new[]
                {
                    loopHole
                },
                Segment2DLoopCollectionHelper.CreateFromString(
                    string.Join(Environment.NewLine,
                                "5",
                                "2...3",
                                ".....",
                                ".....",
                                ".....",
                                "1...4")));

            var profile = new SoilProfile2D(pipingSoilProfileId, profileName,
                                            new List<SoilLayer2D>
                                            {
                                                soilLayer2D,
                                                SoilLayer2DTestFactory.CreateSoilLayer2D(
                                                    new List<Segment2D[]>(),
                                                    loopHole)
                                            }, Enumerable.Empty<PreconsolidationStress>())
            {
                IntersectionX = 2.0
            };

            // Call
            PipingSoilProfile transformed = PipingSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profileName, transformed.Name);
            Assert.AreEqual(SoilProfileType.SoilProfile2D, transformed.SoilProfileSourceType);
            Assert.AreEqual(4, transformed.Layers.Count());
            CollectionAssert.AreEquivalent(new[]
            {
                4.0,
                3.0,
                3.0,
                2.0
            }, transformed.Layers.Select(rl => rl.Top));
            Assert.AreEqual(0, transformed.Bottom);
        }

        [Test]
        public void Transform_InvalidSoilProfile1D_ThrowsImportedDataTransformException()
        {
            // Setup
            const string profileName = "SomeProfile";
            var random = new Random(22);
            double bottom = random.NextDouble();
            const long pipingSoilProfileId = 1234L;

            var profile = new SoilProfile1D(pipingSoilProfileId,
                                            profileName,
                                            bottom,
                                            Enumerable.Empty<SoilLayer1D>());

            // Call
            TestDelegate call = () => PipingSoilProfileTransformer.Transform(profile);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentException>(innerException);
            string expectedMessage = CreateExpectedErrorMessage(profileName, innerException.Message);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Transform_SoilProfile1DWithSingleLayer_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            const string profileName = "SomeProfile";
            var random = new Random(22);
            double bottom = random.NextDouble();
            double top = random.NextDouble();
            const long pipingSoilProfileId = 1234L;

            var profile = new SoilProfile1D(pipingSoilProfileId,
                                            profileName,
                                            bottom,
                                            new[]
                                            {
                                                SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer(top)
                                            }
            );

            // Call
            PipingSoilProfile transformed = PipingSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profileName, transformed.Name);
            Assert.AreEqual(SoilProfileType.SoilProfile1D, transformed.SoilProfileSourceType);

            PipingSoilLayer[] layers = transformed.Layers.ToArray();
            Assert.AreEqual(1, layers.Length);
            Assert.AreEqual(top, layers[0].Top);
            Assert.AreEqual(bottom, transformed.Bottom);
        }

        [Test]
        public void Transform_SoilProfile1DWithMultipleLayers_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            const string profileName = "SomeProfile";
            var random = new Random(22);
            double bottom = random.NextDouble();
            double top = bottom + random.NextDouble();
            double top2 = bottom + random.NextDouble();
            const long pipingSoilProfileId = 1234L;

            var profile = new SoilProfile1D(pipingSoilProfileId,
                                            profileName,
                                            bottom,
                                            new[]
                                            {
                                                SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer(top),
                                                SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer(top2)
                                            }
            );

            // Call
            PipingSoilProfile transformed = PipingSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profileName, transformed.Name);
            Assert.AreEqual(2, transformed.Layers.Count());
            Assert.AreEqual(bottom, transformed.Bottom);
        }

        private static string CreateExpectedErrorMessage(string soilProfileName, string errorMessage)
        {
            return $"Er is een fout opgetreden bij het inlezen van ondergrondschematisatie '{soilProfileName}': " +
                   $"{errorMessage}";
        }

        private static void AssertPipingSoilLayers(IEnumerable<PipingSoilLayer> expectedSoilLayer2Ds,
                                                   IEnumerable<PipingSoilLayer> actualSoilLayer2Ds)
        {
            PipingSoilLayer[] expectedSoilLayer2DsArray = expectedSoilLayer2Ds.ToArray();
            PipingSoilLayer[] actualSoilLayers2DArray = actualSoilLayer2Ds.ToArray();
            Assert.AreEqual(expectedSoilLayer2DsArray.Length, actualSoilLayers2DArray.Length);

            for (var i = 0; i < expectedSoilLayer2DsArray.Length; i++)
            {
                AssertPipingSoilLayer(expectedSoilLayer2DsArray[i], actualSoilLayers2DArray[i]);
            }
        }

        private static void AssertPipingSoilLayer(PipingSoilLayer expected, PipingSoilLayer actual)
        {
            Assert.AreEqual(expected.Top, actual.Top);
            Assert.AreEqual(expected.IsAquifer, actual.IsAquifer);
            DistributionAssert.AreEqual(expected.BelowPhreaticLevel, actual.BelowPhreaticLevel);
            DistributionAssert.AreEqual(expected.DiameterD70, actual.DiameterD70);
            DistributionAssert.AreEqual(expected.Permeability, actual.Permeability);
            Assert.AreEqual(expected.MaterialName, actual.MaterialName);
            Assert.AreEqual(expected.Color, actual.Color);
        }

        private class TestSoilProfile : ISoilProfile
        {
            public string Name { get; }
        }
    }
}