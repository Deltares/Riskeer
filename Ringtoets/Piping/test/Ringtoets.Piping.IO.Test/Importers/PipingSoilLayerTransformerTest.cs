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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Primitives;
using SoilLayer1D = Ringtoets.Common.IO.SoilProfile.SoilLayer1D;
using SoilLayer2D = Ringtoets.Common.IO.SoilProfile.SoilLayer2D;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class PipingSoilLayerTransformerTest
    {
        [Test]
        public void SoilLayer1DTransform_SoilLayer1DNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void SoilLayer1DTransform_PropertiesSetWithCorrectDistributionsAndDifferentLayerParameters_ExpectedProperties()
        {
            // Setup
            var random = new Random(22);

            bool isAquifer = random.NextBoolean();
            double top = random.NextDouble();
            const string materialName = "materialX";
            Color color = Color.AliceBlue;

            const int belowPhreaticLevelDistribution = 3;
            const int belowPhreaticLevelShift = 0;
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelDeviation = random.NextDouble();

            const int diameterD70Distribution = 3;
            const int diameterD70Shift = 0;
            double diameterD70Mean = random.NextDouble();
            double diameterD70CoefficientOfVariation = random.NextDouble();

            const int permeabilityDistribution = 3;
            const int permeabilityShift = 0;
            double permeabilityMean = random.NextDouble();
            double permeabilityCoefficientOfVariation = random.NextDouble();

            var layer = new SoilLayer1D(top)
            {
                MaterialName = materialName,
                IsAquifer = isAquifer,
                Color = color,
                BelowPhreaticLevelDistribution = belowPhreaticLevelDistribution,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                DiameterD70Distribution = diameterD70Distribution,
                DiameterD70Shift = diameterD70Shift,
                DiameterD70Mean = diameterD70Mean,
                DiameterD70CoefficientOfVariation = diameterD70CoefficientOfVariation,
                PermeabilityDistribution = permeabilityDistribution,
                PermeabilityShift = permeabilityShift,
                PermeabilityMean = permeabilityMean,
                PermeabilityCoefficientOfVariation = permeabilityCoefficientOfVariation
            };

            // Call
            PipingSoilLayer pipingSoilLayer = PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(top, pipingSoilLayer.Top);
            Assert.AreEqual(isAquifer, pipingSoilLayer.IsAquifer);
            Assert.AreEqual(belowPhreaticLevelMean, pipingSoilLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, pipingSoilLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(belowPhreaticLevelShift, pipingSoilLayer.BelowPhreaticLevelShift);
            Assert.AreEqual(diameterD70Mean, pipingSoilLayer.DiameterD70Mean);
            Assert.AreEqual(diameterD70CoefficientOfVariation, pipingSoilLayer.DiameterD70CoefficientOfVariation);
            Assert.AreEqual(permeabilityMean, pipingSoilLayer.PermeabilityMean);
            Assert.AreEqual(permeabilityCoefficientOfVariation, pipingSoilLayer.PermeabilityCoefficientOfVariation);
            Assert.AreEqual(materialName, pipingSoilLayer.MaterialName);
            Assert.AreEqual(color, pipingSoilLayer.Color);
        }

        [Test]
        public void SoilLayer1DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException()
        {
            // Setup
            var layer = new SoilLayer1D(0.0)
            {
                BelowPhreaticLevelDistribution = -1
            };

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Parameter 'Verzadigd gewicht' is niet verschoven lognormaal verdeeld.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectLogNormalDistributionsSoilLayer1D))]
        public void SoilLayer1DTransform_IncorrectLogNormalDistribution_ThrowsImportedDataTransformException(SoilLayer1D layer, string parameter)
        {
            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Parameter '{parameter}' is niet lognormaal verdeeld.", exception.Message);
        }

        [Test]
        public void SoilLayer2DTransform_SoilLayer2DNull_ThrowsArgumentNullException()
        {
            // Setup
            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(null, 0.0, out bottom);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void SoilLayer2DTransform_EmptySoilLayer2D_ReturnsEmptyCollectionWithMaxValueBottom()
        {
            // Setup
            var layer = new SoilLayer2D();
            double bottom;

            // Call
            IEnumerable<PipingSoilLayer> pipingSoilLayers = PipingSoilLayerTransformer.Transform(layer, 0.0, out bottom);

            // Assert
            CollectionAssert.IsEmpty(pipingSoilLayers);
            Assert.AreEqual(double.MaxValue, bottom);
        }

        [Test]
        public void SoilLayer2DTransform_PropertiesSetWithDifferentLayerParameters_ExpectedProperties()
        {
            // Setup
            var random = new Random(22);
            double y1 = random.NextDouble();
            double y2 = y1 + random.NextDouble();
            const double x1 = 1.0;
            const double x2 = 1.1;
            const double x3 = 1.2;
            const string materialName = "materialX";
            Color color = Color.DarkSeaGreen;
            double bottom;

            bool isAquifer = random.NextBoolean();
            const int logNormalDistribution = 3;
            const int logNormalShift = 0;

            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelDeviation = random.NextDouble();

            double diameterD70Mean = random.NextDouble();
            double diameterD70CoefficientOfVariation = random.NextDouble();

            double permeabilityMean = random.NextDouble();
            double permeabilityCoefficientOfVariation = random.NextDouble();

            var outerLoop = new List<Segment2D>
            {
                new Segment2D(new Point2D(x1, y1),
                              new Point2D(x3, y1)),
                new Segment2D(new Point2D(x3, y1),
                              new Point2D(x3, y2)),
                new Segment2D(new Point2D(x3, y2),
                              new Point2D(x1, y2)),
                new Segment2D(new Point2D(x1, y1),
                              new Point2D(x1, y2))
            };

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                Enumerable.Empty<IEnumerable<Segment2D>>(),
                outerLoop);
            layer.MaterialName = materialName;
            layer.IsAquifer = isAquifer;
            layer.Color = color;
            layer.BelowPhreaticLevelDistribution = logNormalDistribution;
            layer.BelowPhreaticLevelShift = logNormalShift;
            layer.BelowPhreaticLevelMean = belowPhreaticLevelMean;
            layer.BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation;
            layer.DiameterD70Distribution = logNormalDistribution;
            layer.DiameterD70Shift = logNormalShift;
            layer.DiameterD70Mean = diameterD70Mean;
            layer.DiameterD70CoefficientOfVariation = diameterD70CoefficientOfVariation;
            layer.PermeabilityDistribution = logNormalDistribution;
            layer.PermeabilityShift = logNormalShift;
            layer.PermeabilityMean = permeabilityMean;
            layer.PermeabilityCoefficientOfVariation = permeabilityCoefficientOfVariation;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, x2, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(y1, bottom, 1e-6);
            PipingSoilLayer resultLayer = pipingSoilLayers.First();
            Assert.AreEqual(y2, resultLayer.Top, 1e-6);
            Assert.AreEqual(isAquifer, resultLayer.IsAquifer);
            Assert.AreEqual(materialName, resultLayer.MaterialName);
            Assert.AreEqual(color, resultLayer.Color);

            Assert.AreEqual(belowPhreaticLevelMean, resultLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, resultLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(diameterD70Mean, resultLayer.DiameterD70Mean);
            Assert.AreEqual(diameterD70CoefficientOfVariation, resultLayer.DiameterD70CoefficientOfVariation);
            Assert.AreEqual(permeabilityMean, resultLayer.PermeabilityMean);
            Assert.AreEqual(permeabilityCoefficientOfVariation, resultLayer.PermeabilityCoefficientOfVariation);
        }

        [Test]
        public void SoilLayer2DTransform_WithOuterLoopNotIntersectingX_ReturnsEmptyCollectionWithMaxValueBottom()
        {
            // Setup
            var random = new Random(22);
            double y1 = random.NextDouble();
            double y2 = random.NextDouble();

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                new List<Segment2D[]>(),
                new List<Segment2D>
                {
                    new Segment2D(new Point2D(1.0, y1),
                                  new Point2D(1.2, y2)),
                    new Segment2D(new Point2D(1.2, y2),
                                  new Point2D(1.0, y1))
                });

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 0.0, out bottom).ToArray();

            // Assert
            CollectionAssert.IsEmpty(pipingSoilLayers);
            Assert.AreEqual(double.MaxValue, bottom);
        }

        [Test]
        public void SoilLayer2DTransform_WithOuterLoopIntersectingX_ReturnsBottomAndLayerWithTop()
        {
            // Setup
            double expectedZ = new Random(22).NextDouble();
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                new List<Segment2D[]>(),
                new List<Segment2D>
                {
                    new Segment2D(new Point2D(-0.1, expectedZ),
                                  new Point2D(0.1, expectedZ)),
                    new Segment2D(new Point2D(-0.1, expectedZ),
                                  new Point2D(0.1, expectedZ))
                });
            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 0.0, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(expectedZ, bottom);
            Assert.AreEqual(expectedZ, pipingSoilLayers[0].Top);
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopComplex_ReturnsTwoLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "..8.7...",
                            "..5.6...",
                            "..4..3..",
                            "........"));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                new List<Segment2D[]>(), outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(2, pipingSoilLayers.Length);
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                2.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopInnerLoopSimple_ReturnsTwoLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            "........",
                            "..4..3.."));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "...12...",
                            "........",
                            "........",
                            "...43...",
                            "........"));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop
            }, outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(2, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                1.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopInnerLoopComplex_ReturnsThreeLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            "........",
                            "..4..3.."));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "...1.2..",
                            "...87...",
                            "...56...",
                            "...4.3..",
                            "........"));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop
            }, outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(3, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                3.0,
                1.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopMultipleInnerLoops_ReturnsThreeLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            "........",
                            "..4..3.."));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "...12...",
                            "...43...",
                            "........",
                            "........",
                            "........"));

            List<Segment2D> innerLoop2 = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "........",
                            "........",
                            "........",
                            "...12...",
                            "........"));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop,
                innerLoop2
            }, outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(3, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                3.0,
                1.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopOverlappingInnerLoop_ReturnsOneLayer()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            ".4....3.",
                            "........"));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "........",
                            "........",
                            "...12...",
                            "........",
                            "...43..."));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop
            }, outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(2.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_Outer_LoopOverlappingInnerLoopsFirstInnerLoopNotOverBottom_ReturnsOneLayer()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            ".4....3.",
                            "........"));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "...12...",
                            "........",
                            "...43...",
                            "........",
                            "........"));

            List<Segment2D> innerLoop2 = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "........",
                            "...12...",
                            "........",
                            "........",
                            "...43..."));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop,
                innerLoop2
            }, outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(4.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopInnerLoopOnBorderBottom_ReturnsTwoLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            ".4....3.",
                            "........"));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "........",
                            "...12...",
                            "........",
                            "...43...",
                            "........"));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop
            }, outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(2, pipingSoilLayers.Length);
            Assert.AreEqual(3.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                1.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopInnerLoopOverlapTop_ReturnsOneLayer()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString
            (string.Join(Environment.NewLine,
                         "6",
                         "........",
                         "..1..2..",
                         "........",
                         "........",
                         ".4....3.",
                         "........"));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "...43...",
                            "........",
                            "...12...",
                            "........",
                            "........",
                            "........"));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop
            }, outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                3.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopInnerLoopOnBorderTop_ReturnsOneLayer()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            ".4....3.",
                            "........"));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "...43...",
                            "........",
                            "...12...",
                            "........",
                            "........",
                            "........"));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop
            }, outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                3.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopVerticalAtX_ThrowsImportedDataTransformException()
        {
            // Setup
            const double atX = 2.0;
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            "........",
                            "..4..3.."));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                Enumerable.Empty<Segment2D[]>(), outerLoop);

            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, atX, out bottom);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Er kan geen 1D-profiel bepaald worden wanneer segmenten in een 2D laag verticaal lopen op de gekozen positie: x = {atX}.", exception.Message);
        }

        [Test]
        public void SoilLayer2DTransform_InnerLoopVerticalAtX_ThrowsImportedDataTransformException()
        {
            // Setup
            const double atX = 3.0;
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "..1..2..",
                            "........",
                            "........",
                            "........",
                            "........",
                            "..4..3.."));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "...1.2..",
                            "........",
                            "........",
                            "...4.3..",
                            "........"));

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new[]
            {
                innerLoop
            }, outerLoop);

            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, atX, out bottom);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Er kan geen 1D-profiel bepaald worden wanneer segmenten in een 2D laag verticaal lopen op de gekozen positie: x = {atX}.", exception.Message);
        }

        [Test]
        public void SoilLayer2DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException()
        {
            // Setup
            var layer = new SoilLayer2D
            {
                BelowPhreaticLevelDistribution = -1
            };

            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, 0, out bottom);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual("Parameter 'Verzadigd gewicht' is niet verschoven lognormaal verdeeld.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectLogNormalDistributionsSoilLayer2D))]
        public void SoilLayer2DTransform_IncorrectLogNormalDistribution_ThrowsImportedDataTransformException(SoilLayer2D layer, string parameter)
        {
            // Setup
            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, 0, out bottom);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual($"Parameter '{parameter}' is niet lognormaal verdeeld.", exception.Message);
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsSoilLayer1D()
        {
            return IncorrectLogNormalDistributions(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsSoilLayer2D()
        {
            return IncorrectLogNormalDistributions(() => new SoilLayer2D(), nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributions(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_Incorrect{1}{{1}}_ThrowsImportedDataTransformException";
            const long validDistribution = SoilLayerConstants.LogNormalDistributionValue;
            const double validShift = 0.0;

            SoilLayerBase invalidDiameterD70Distribution = soilLayer();
            invalidDiameterD70Distribution.BelowPhreaticLevelDistribution = validDistribution;
            invalidDiameterD70Distribution.DiameterD70Distribution = -1;
            invalidDiameterD70Distribution.DiameterD70Shift = validShift;
            invalidDiameterD70Distribution.PermeabilityDistribution = validDistribution;
            invalidDiameterD70Distribution.PermeabilityShift = validShift;

            yield return new TestCaseData(invalidDiameterD70Distribution, "Korrelgrootte"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidDiameterD70Shift = soilLayer();
            invalidDiameterD70Shift.BelowPhreaticLevelDistribution = validDistribution;
            invalidDiameterD70Shift.DiameterD70Distribution = validDistribution;
            invalidDiameterD70Shift.DiameterD70Shift = -1;
            invalidDiameterD70Shift.PermeabilityDistribution = validDistribution;
            invalidDiameterD70Shift.PermeabilityShift = validShift;

            yield return new TestCaseData(invalidDiameterD70Shift, "Korrelgrootte"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));

            SoilLayerBase invalidPermeabilityDistribution = soilLayer();
            invalidPermeabilityDistribution.BelowPhreaticLevelDistribution = validDistribution;
            invalidPermeabilityDistribution.DiameterD70Distribution = validDistribution;
            invalidPermeabilityDistribution.DiameterD70Shift = validShift;
            invalidPermeabilityDistribution.PermeabilityDistribution = -1;
            invalidPermeabilityDistribution.PermeabilityShift = validShift;

            yield return new TestCaseData(invalidPermeabilityDistribution, "Doorlatendheid"
            ).SetName(string.Format(testNameFormat, typeName, "Distribution"));

            SoilLayerBase invalidPermeabilityShift = soilLayer();
            invalidPermeabilityShift.BelowPhreaticLevelDistribution = validDistribution;
            invalidPermeabilityShift.DiameterD70Distribution = validDistribution;
            invalidPermeabilityShift.DiameterD70Shift = validShift;
            invalidPermeabilityShift.PermeabilityDistribution = validDistribution;
            invalidPermeabilityShift.PermeabilityShift = -1;

            yield return new TestCaseData(invalidPermeabilityShift, "Doorlatendheid"
            ).SetName(string.Format(testNameFormat, typeName, "Shift"));
        }
    }
}