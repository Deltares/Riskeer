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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.SoilProfiles;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.SoilProfiles
{
    [TestFixture]
    public class PipingSoilLayerTransformerTest
    {
        private readonly string outerLoopSimple = string.Join(Environment.NewLine,
                                                              "6",
                                                              "..1..2..",
                                                              "........",
                                                              "........",
                                                              "........",
                                                              "........",
                                                              "..4..3..");

        private readonly string outerLoopWide = string.Join(Environment.NewLine,
                                                            "6",
                                                            "..1..2..",
                                                            "........",
                                                            "........",
                                                            "........",
                                                            ".4....3.",
                                                            "........");

        private readonly string innerLoopWide = string.Join(Environment.NewLine,
                                                            "6",
                                                            "...43...",
                                                            "........",
                                                            "...12...",
                                                            "........",
                                                            "........",
                                                            "........");

        [Test]
        [TestCase(1.0, true)]
        [TestCase(0.0, false)]
        public void SoilLayer1DTransform_ValidIsAquifer_ReturnsPipingSoilLayer1D(double isAquifer, bool transformedIsAquifer)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.IsAquifer = isAquifer;

            // Call
            PipingSoilLayer soilLayer = PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedIsAquifer, soilLayer.IsAquifer);
        }

        [Test]
        [TestCase(null)]
        [TestCase(1.01)]
        [TestCase(0.01)]
        public void SoilLayer1DTransform_InvalidIsAquifer_ThrowsImportedDataException(double? isAquifer)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.IsAquifer = isAquifer;

            // Call
            TestDelegate call = () => PipingSoilLayerTransformer.Transform(layer);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Ongeldige waarde voor parameter 'Is aquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        }

        [Test]
        [TestCaseSource(nameof(GetColorCases))]
        public void SoilLayer1DTransform_ValidColors_ReturnsMacroStabilityInwardsSoilLayer1D(double? color, Color transformedColor)
        {
            // Setup
            SoilLayer1D layer = SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer();
            layer.Color = color;

            // Call
            PipingSoilLayer soilLayer = PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(transformedColor, soilLayer.Color);
        }

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

            double isAquifer = random.Next(0, 2);
            double top = random.NextDouble();
            const string materialName = "materialX";
            double color = random.NextDouble();

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
                BelowPhreaticLevelDistributionType = belowPhreaticLevelDistribution,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                DiameterD70DistributionType = diameterD70Distribution,
                DiameterD70Shift = diameterD70Shift,
                DiameterD70Mean = diameterD70Mean,
                DiameterD70CoefficientOfVariation = diameterD70CoefficientOfVariation,
                PermeabilityDistributionType = permeabilityDistribution,
                PermeabilityShift = permeabilityShift,
                PermeabilityMean = permeabilityMean,
                PermeabilityCoefficientOfVariation = permeabilityCoefficientOfVariation
            };

            // Call
            PipingSoilLayer pipingSoilLayer = PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Assert.AreEqual(materialName, pipingSoilLayer.MaterialName);
            bool expectedIsAquifer = isAquifer.Equals(1.0);
            Assert.AreEqual(expectedIsAquifer, pipingSoilLayer.IsAquifer);
            Color expectedColor = Color.FromArgb(Convert.ToInt32(color));
            Assert.AreEqual(expectedColor, pipingSoilLayer.Color);

            Assert.AreEqual(top, pipingSoilLayer.Top);

            Assert.AreEqual(layer.BelowPhreaticLevelMean, pipingSoilLayer.BelowPhreaticLevel.Mean,
                            pipingSoilLayer.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(layer.BelowPhreaticLevelDeviation, pipingSoilLayer.BelowPhreaticLevel.StandardDeviation,
                            pipingSoilLayer.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(layer.BelowPhreaticLevelShift, pipingSoilLayer.BelowPhreaticLevel.Shift,
                            pipingSoilLayer.BelowPhreaticLevel.GetAccuracy());

            Assert.AreEqual(layer.DiameterD70Mean, pipingSoilLayer.DiameterD70.Mean,
                            pipingSoilLayer.DiameterD70.GetAccuracy());
            Assert.AreEqual(layer.DiameterD70CoefficientOfVariation, pipingSoilLayer.DiameterD70.CoefficientOfVariation,
                            pipingSoilLayer.DiameterD70.GetAccuracy());

            Assert.AreEqual(layer.PermeabilityMean, pipingSoilLayer.Permeability.Mean,
                            pipingSoilLayer.Permeability.GetAccuracy());
            Assert.AreEqual(layer.PermeabilityCoefficientOfVariation, pipingSoilLayer.Permeability.CoefficientOfVariation,
                            pipingSoilLayer.Permeability.GetAccuracy());
        }

        [Test]
        public void SoilLayer1DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException()
        {
            // Setup
            var layer = new SoilLayer1D(0.0)
            {
                BelowPhreaticLevelDistributionType = -1,
                MaterialName = nameof(SoilLayer1D)
            };

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName,
                                                                                    "Verzadigd gewicht",
                                                                                    "Parameter moet verschoven lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(InvalidStochasticDistributionValuesSoilLayer1D))]
        public void SoilLayer1DTransform_InvalidStochasticDistributionValues_ThrowsImportedDataTransformException(SoilLayer1D layer, string parameter)
        {
            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(innerException);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName,
                                                                                    parameter,
                                                                                    innerException.Message);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectLogNormalDistributionsTypeSoilLayer1D))]
        public void SoilLayer1DTransform_IncorrectLogNormalDistributionType_ThrowsImportedDataTransformException(SoilLayer1D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName,
                                                                                    parameterName,
                                                                                    "Parameter moet lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectLogNormalDistributionsShiftSoilLayer1D))]
        public void SoilLayer1DTransform_IncorrectLogNormalDistributionShift_ThrowsImportedDataTransformException(SoilLayer1D layer, string parameterName)
        {
            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName,
                                                                                    parameterName,
                                                                                    "Parameter moet lognormaal verdeeld zijn met een verschuiving gelijk aan 0.");
            Assert.AreEqual(expectedMessage, exception.Message);
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
            var layer = new SoilLayer2D(new SoilLayer2DLoop(new Segment2D[0]), Enumerable.Empty<SoilLayer2D>());
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
            double color = random.NextDouble();
            double bottom;

            double isAquifer = random.Next(0, 2);
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
                new Segment2D(new Point2D(x1, y2),
                              new Point2D(x1, y1))
            };

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                Enumerable.Empty<IEnumerable<Segment2D>>(),
                outerLoop);
            layer.MaterialName = materialName;
            layer.IsAquifer = isAquifer;
            layer.Color = color;
            layer.BelowPhreaticLevelDistributionType = logNormalDistribution;
            layer.BelowPhreaticLevelShift = logNormalShift;
            layer.BelowPhreaticLevelMean = belowPhreaticLevelMean;
            layer.BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation;
            layer.DiameterD70DistributionType = logNormalDistribution;
            layer.DiameterD70Shift = logNormalShift;
            layer.DiameterD70Mean = diameterD70Mean;
            layer.DiameterD70CoefficientOfVariation = diameterD70CoefficientOfVariation;
            layer.PermeabilityDistributionType = logNormalDistribution;
            layer.PermeabilityShift = logNormalShift;
            layer.PermeabilityMean = permeabilityMean;
            layer.PermeabilityCoefficientOfVariation = permeabilityCoefficientOfVariation;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, x2, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(y1, bottom, 1e-6);
            PipingSoilLayer resultLayer = pipingSoilLayers[0];
            Assert.AreEqual(y2, resultLayer.Top, 1e-6);

            Assert.AreEqual(materialName, resultLayer.MaterialName);
            bool expectedIsAquifer = isAquifer.Equals(1.0);
            Assert.AreEqual(expectedIsAquifer, resultLayer.IsAquifer);
            Color expectedColor = Color.FromArgb(Convert.ToInt32(color));
            Assert.AreEqual(expectedColor, resultLayer.Color);

            Assert.AreEqual(layer.BelowPhreaticLevelMean, resultLayer.BelowPhreaticLevel.Mean,
                            resultLayer.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(layer.BelowPhreaticLevelDeviation, resultLayer.BelowPhreaticLevel.StandardDeviation,
                            resultLayer.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(layer.BelowPhreaticLevelShift, resultLayer.BelowPhreaticLevel.Shift,
                            resultLayer.BelowPhreaticLevel.GetAccuracy());

            Assert.AreEqual(layer.DiameterD70Mean, resultLayer.DiameterD70.Mean,
                            resultLayer.DiameterD70.GetAccuracy());
            Assert.AreEqual(layer.DiameterD70CoefficientOfVariation, resultLayer.DiameterD70.CoefficientOfVariation,
                            resultLayer.DiameterD70.GetAccuracy());

            Assert.AreEqual(layer.PermeabilityMean, resultLayer.Permeability.Mean,
                            resultLayer.Permeability.GetAccuracy());
            Assert.AreEqual(layer.PermeabilityCoefficientOfVariation, resultLayer.Permeability.CoefficientOfVariation,
                            resultLayer.Permeability.GetAccuracy());
        }

        [Test]
        [TestCase(1.0, true)]
        [TestCase(0.0, false)]
        public void SoilLayer2DTransform_ValidIsAquifer_ReturnsPipingSoilLayers(double isAquifer, bool transformedIsAquifer)
        {
            // Setup
            const double x1 = 1.0;
            const double x2 = 1.1;
            const double x3 = 1.2;
            SoilLayer2D layer = CreateValidConfiguredSoilLayer2D(x1, x3);
            layer.IsAquifer = isAquifer;

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(layer, x2, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(transformedIsAquifer, pipingSoilLayers[0].IsAquifer);
        }

        [Test]
        [TestCase(null)]
        [TestCase(1.01)]
        [TestCase(0.01)]
        public void SoilLayer2DTransform_InvalidIsAquifer_ThrowsImportedDataException(double? isAquifer)
        {
            // Setup
            const double x1 = 1.0;
            const double x2 = 1.1;
            const double x3 = 1.2;
            SoilLayer2D layer = CreateValidConfiguredSoilLayer2D(x1, x3);
            layer.IsAquifer = isAquifer;

            double bottom;

            PipingSoilLayer[] pipingSoilLayers = null;

            // Call
            TestDelegate call = () => pipingSoilLayers = PipingSoilLayerTransformer.Transform(layer, x2, out bottom).ToArray();

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                "Ongeldige waarde voor parameter 'Is aquifer'.");
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
            Assert.IsNull(pipingSoilLayers);
        }

        [Test]
        [TestCaseSource(nameof(GetColorCases))]
        public void SoilLayer2DTransform_ValidColors_ReturnsMacroStabilityInwardsSoilLayer1D(double? color, Color transformedColor)
        {
            // Setup
            const double x1 = 1.0;
            const double x2 = 1.1;
            const double x3 = 1.2;
            SoilLayer2D layer = CreateValidConfiguredSoilLayer2D(x1, x3);
            layer.Color = color;

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(layer, x2, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(transformedColor, pipingSoilLayers[0].Color);
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
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new List<Segment2D[]>(), new List<Segment2D>
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

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new List<Segment2D[]>(), outerLoop);

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
        public void SoilLayer2DTransform_OuterLoopNoNestedLayers_ReturnsOneLayer()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopSimple);

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(new List<Segment2D[]>(), outerLoop);

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopNestedLayerSimple_ReturnsThreeLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopSimple);

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
            Assert.AreEqual(3, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                4.0,
                1.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopNestedLayerComplex_ReturnsFiveLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopSimple);

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
            Assert.AreEqual(5, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                4.0,
                3.0,
                2.0,
                1.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopMultipleNestedLayers_ReturnsFiveLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopSimple);

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
            Assert.AreEqual(5, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                4.0,
                3.0,
                1.0,
                0.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopNestedLayerInNestedLayer_ReturnsFiveLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopSimple);

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "..1..2..",
                            "........",
                            "........",
                            "..4..3..",
                            "........"));

            List<Segment2D> innerLoop2 = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "........",
                            "...12...",
                            "...43...",
                            "........",
                            "........"));

            var layer = new SoilLayer2D(new SoilLayer2DLoop(outerLoop.ToArray()), new[]
            {
                new SoilLayer2D(new SoilLayer2DLoop(innerLoop.ToArray()), new[]
                {
                    new SoilLayer2D(new SoilLayer2DLoop(innerLoop2.ToArray()), Enumerable.Empty<SoilLayer2D>())
                    {
                        IsAquifer = 0.0
                    }
                })
                {
                    IsAquifer = 0.0
                }
            })
            {
                IsAquifer = 0.0
            };

            double bottom;

            // Call
            PipingSoilLayer[] pipingSoilLayers = PipingSoilLayerTransformer.Transform(
                layer, 3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(5, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                4.0,
                3.0,
                2.0,
                1.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopOverlappingNestedLayer_ReturnsTwoLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopWide);

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
            Assert.AreEqual(2, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                2.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopOverlappingNestedLayersFirstNestedLayerNotOverBottom_ReturnsThreeLayer()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopWide);

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
            Assert.AreEqual(3, pipingSoilLayers.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                4.0,
                3.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopNestedLayerOnBorderBottom_ReturnsThreeLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopWide);

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
            Assert.AreEqual(3, pipingSoilLayers.Length);
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                5.0,
                3.0,
                1.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopNestedLayerOverlapTop_ReturnsTwoLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                string.Join(Environment.NewLine,
                            "6",
                            "........",
                            "..1..2..",
                            "........",
                            "........",
                            ".4....3.",
                            "........"));

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                innerLoopWide);

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
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                3.0,
                5.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopNestedLayerOnBorderTop_ReturnsTwoLayers()
        {
            // Setup
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopWide);

            List<Segment2D> innerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                innerLoopWide);

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
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[]
            {
                3.0,
                5.0
            }, pipingSoilLayers.Select(rl => rl.Top));
        }

        [Test]
        public void SoilLayer2DTransform_OuterLoopVerticalAtX_ThrowsImportedDataTransformException()
        {
            // Setup
            const double atX = 2.0;
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopSimple);

            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D(
                Enumerable.Empty<Segment2D[]>(), outerLoop);

            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, atX, out bottom);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                $"Er kan geen 1D-profiel bepaald worden wanneer segmenten in een 2D laag verticaal lopen op de gekozen positie: x = {atX}.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void SoilLayer2DTransform_SingleNestedLayerVerticalAtX_ThrowsImportedDataTransformException()
        {
            // Setup
            const double atX = 3.0;
            List<Segment2D> outerLoop = Segment2DLoopCollectionHelper.CreateFromString(
                outerLoopSimple);

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
            string expectedMessage = CreateExpectedErrorMessage(layer.MaterialName,
                                                                $"Er kan geen 1D-profiel bepaald worden wanneer segmenten in een 2D laag verticaal lopen op de gekozen positie: x = {atX}.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void SoilLayer2DTransform_IncorrectShiftedLogNormalDistribution_ThrowsImportedDataTransformException()
        {
            // Setup
            SoilLayer2D layer = SoilLayer2DTestFactory.CreateSoilLayer2D();

            layer.BelowPhreaticLevelDistributionType = -1;

            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, 0, out bottom);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName,
                                                                                    "Verzadigd gewicht",
                                                                                    "Parameter moet verschoven lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectLogNormalDistributionsTypeSoilLayer2D))]
        public void SoilLayer2DTransform_IncorrectLogNormalDistributionType_ThrowsImportedDataTransformException(SoilLayer2D layer, string parameterName)
        {
            // Setup
            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, 0, out bottom);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, "Parameter moet lognormaal verdeeld zijn.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(IncorrectLogNormalDistributionsShiftSoilLayer2D))]
        public void SoilLayer2DTransform_IncorrectLogNormalDistributionShift_ThrowsImportedDataTransformException(SoilLayer2D layer, string parameterName)
        {
            // Setup
            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, 0, out bottom);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);

            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName,
                                                                                    parameterName,
                                                                                    "Parameter moet lognormaal verdeeld zijn met een verschuiving gelijk aan 0.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(InvalidStochasticDistributionValuesSoilLayer2D))]
        public void SoilLayer2DTransform_InvalidStochasticDistributionValues_ThrowsImportedDataTransformException(SoilLayer2D layer, string parameterName)
        {
            // Setup
            double bottom;

            // Call
            TestDelegate test = () => PipingSoilLayerTransformer.Transform(layer, 1, out bottom);

            // Assert
            Exception exception = Assert.Throws<ImportedDataTransformException>(test);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(innerException);
            string expectedMessage = CreateExpectedErrorMessageForParameterVariable(layer.MaterialName, parameterName, innerException.Message);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        private static SoilLayer2D CreateValidConfiguredSoilLayer2D(double x1, double x3)
        {
            var random = new Random(21);
            double y1 = random.NextDouble();
            double y2 = y1 + random.NextDouble();
            var outerLoop = new List<Segment2D>
            {
                new Segment2D(new Point2D(x1, y1),
                              new Point2D(x3, y1)),
                new Segment2D(new Point2D(x3, y1),
                              new Point2D(x3, y2)),
                new Segment2D(new Point2D(x3, y2),
                              new Point2D(x1, y2)),
                new Segment2D(new Point2D(x1, y2),
                              new Point2D(x1, y1))
            };

            return SoilLayer2DTestFactory.CreateSoilLayer2D(Enumerable.Empty<IEnumerable<Segment2D>>(), outerLoop);
        }

        private static IEnumerable<TestCaseData> GetColorCases()
        {
            yield return new TestCaseData(null, Color.Empty)
                .SetName("Color result Empty");
            yield return new TestCaseData((double) -12156236, Color.FromArgb(70, 130, 180))
                .SetName("Color result Purple");
            yield return new TestCaseData((double) -65281, Color.FromArgb(255, 0, 255))
                .SetName("Color result Pink");
        }

        private static string CreateExpectedErrorMessageForParameterVariable(string materialName, string parameterName, string errorMessage)
        {
            return $"Er is een fout opgetreden bij het inlezen van grondlaag '{materialName}' voor parameter '{parameterName}': {errorMessage}";
        }

        private static string CreateExpectedErrorMessage(string materialName, string errorMessage)
        {
            return $"Er is een fout opgetreden bij het inlezen van grondlaag '{materialName}': {errorMessage}";
        }

        #region Test Data: Invalid DistributionType

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsTypeSoilLayer1D()
        {
            return IncorrectLogNormalDistributionsType(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsTypeSoilLayer2D()
        {
            return IncorrectLogNormalDistributionsType(SoilLayer2DTestFactory.CreateSoilLayer2D, nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsType(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_IncorrectDistribution{{1}}_ThrowsImportedDataTransformException";
            const long validDistributionType = SoilLayerConstants.LogNormalDistributionValue;
            const double validShift = 0.0;

            SoilLayerBase invalidDiameterD70Distribution = soilLayer();
            invalidDiameterD70Distribution.BelowPhreaticLevelDistributionType = validDistributionType;
            invalidDiameterD70Distribution.DiameterD70DistributionType = -1;
            invalidDiameterD70Distribution.DiameterD70Shift = validShift;
            invalidDiameterD70Distribution.PermeabilityDistributionType = validDistributionType;
            invalidDiameterD70Distribution.PermeabilityShift = validShift;

            yield return new TestCaseData(invalidDiameterD70Distribution, "d70"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidPermeabilityDistribution = soilLayer();
            invalidPermeabilityDistribution.BelowPhreaticLevelDistributionType = validDistributionType;
            invalidPermeabilityDistribution.DiameterD70DistributionType = validDistributionType;
            invalidPermeabilityDistribution.DiameterD70Shift = validShift;
            invalidPermeabilityDistribution.PermeabilityDistributionType = -1;
            invalidPermeabilityDistribution.PermeabilityShift = validShift;

            yield return new TestCaseData(invalidPermeabilityDistribution, "Doorlatendheid"
            ).SetName(string.Format(testNameFormat, typeName));
        }

        #endregion

        #region Test Data: Invalid Shift

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsShiftSoilLayer1D()
        {
            return IncorrectLogNormalDistributionsShift(() => new SoilLayer1D(0.0), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsShiftSoilLayer2D()
        {
            return IncorrectLogNormalDistributionsShift(SoilLayer2DTestFactory.CreateSoilLayer2D, nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> IncorrectLogNormalDistributionsShift(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_IncorrectShift{{1}}_ThrowsImportedDataTransformException";
            const long validDistributionType = SoilLayerConstants.LogNormalDistributionValue;
            const double validShift = 0.0;

            SoilLayerBase invalidDiameterD70Shift = soilLayer();
            invalidDiameterD70Shift.BelowPhreaticLevelDistributionType = validDistributionType;
            invalidDiameterD70Shift.DiameterD70DistributionType = validDistributionType;
            invalidDiameterD70Shift.DiameterD70Shift = -1;
            invalidDiameterD70Shift.PermeabilityDistributionType = validDistributionType;
            invalidDiameterD70Shift.PermeabilityShift = validShift;

            yield return new TestCaseData(invalidDiameterD70Shift, "d70"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidPermeabilityShift = soilLayer();
            invalidPermeabilityShift.BelowPhreaticLevelDistributionType = validDistributionType;
            invalidPermeabilityShift.DiameterD70DistributionType = validDistributionType;
            invalidPermeabilityShift.DiameterD70Shift = validShift;
            invalidPermeabilityShift.PermeabilityDistributionType = validDistributionType;
            invalidPermeabilityShift.PermeabilityShift = -1;

            yield return new TestCaseData(invalidPermeabilityShift, "Doorlatendheid"
            ).SetName(string.Format(testNameFormat, typeName));
        }

        #endregion

        #region Test Data: Invalid Distribution Properties

        private static IEnumerable<TestCaseData> InvalidStochasticDistributionValuesSoilLayer1D()
        {
            return InvalidStochasticDistributionValues(() => SoilLayer1DTestFactory.CreateSoilLayer1DWithValidAquifer(), nameof(SoilLayer1D));
        }

        private static IEnumerable<TestCaseData> InvalidStochasticDistributionValuesSoilLayer2D()
        {
            return InvalidStochasticDistributionValues(SoilLayer2DTestFactory.CreateSoilLayer2D, nameof(SoilLayer2D));
        }

        private static IEnumerable<TestCaseData> InvalidStochasticDistributionValues(Func<SoilLayerBase> soilLayer, string typeName)
        {
            const string testNameFormat = "{0}Transform_InvalidStochasticDistributionValues{{1}}_ThrowsImportedDataTransformException";
            const double invalidMean = 0;

            SoilLayerBase invalidBelowPhreaticLevel = soilLayer();
            invalidBelowPhreaticLevel.BelowPhreaticLevelMean = 1;
            invalidBelowPhreaticLevel.BelowPhreaticLevelShift = 2;
            yield return new TestCaseData(invalidBelowPhreaticLevel, "Verzadigd gewicht"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidPermeability = soilLayer();
            invalidPermeability.PermeabilityMean = invalidMean;
            yield return new TestCaseData(invalidPermeability, "Doorlatendheid"
            ).SetName(string.Format(testNameFormat, typeName));

            SoilLayerBase invalidDiameterD70 = soilLayer();
            invalidDiameterD70.DiameterD70Mean = invalidMean;
            yield return new TestCaseData(invalidDiameterD70, "d70"
            ).SetName(string.Format(testNameFormat, typeName));
        }

        #endregion
    }
}