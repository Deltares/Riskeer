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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.Test.Builders
{
    [TestFixture]
    public class SoilLayer2DTest
    {
        [Test]
        public void DefaultConstructor_Always_NotInstantiatedOuterLoopAndEmptyInnerLoops()
        {
            // Call
            var result = new SoilLayer2D();

            // Assert
            Assert.IsNull(result.OuterLoop);
            CollectionAssert.IsEmpty(result.InnerLoops);
            Assert.IsNull(result.IsAquifer);
            Assert.IsNull(result.MaterialName);
            Assert.IsNull(result.Color);

            Assert.IsNull(result.BelowPhreaticLevelDistribution);
            Assert.IsNull(result.BelowPhreaticLevelShift);
            Assert.IsNull(result.BelowPhreaticLevelMean);
            Assert.IsNull(result.BelowPhreaticLevelDeviation);

            Assert.IsNull(result.DiameterD70Distribution);
            Assert.IsNull(result.DiameterD70Shift);
            Assert.IsNull(result.DiameterD70Mean);
            Assert.IsNull(result.DiameterD70Deviation);

            Assert.IsNull(result.PermeabilityDistribution);
            Assert.IsNull(result.PermeabilityShift);
            Assert.IsNull(result.PermeabilityMean);
            Assert.IsNull(result.PermeabilityDeviation);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void OuterLoop_TwoDisconnectedSegment_ThrowsArgumentException(double diff)
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(0.0, diff);

            // Call
            TestDelegate test = () => layer.OuterLoop = new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC)
            };

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void OuterLoop_ThreeDisconnectedSegment_ThrowsArgumentException(double diff)
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0,0.0);
            var pointB = new Point2D(1.0,0.0);
            var pointC = new Point2D(1.0,1.0);
            var pointD = new Point2D(0.0,diff);

            // Call
            TestDelegate test = () => layer.OuterLoop = new List<Segment2D>
            {
                new Segment2D(pointA,pointB),
                new Segment2D(pointB,pointC),
                new Segment2D(pointC,pointD),
            };

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void OuterLoop_TwoConnectedSegment_SetsNewLoop()
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);

            // Call
            layer.OuterLoop = new List<Segment2D>
            {
                new Segment2D(pointA,pointB),
                new Segment2D(pointB,pointA)
            };

            // Assert
            Assert.NotNull(layer.OuterLoop);
            Assert.AreEqual(new Segment2D(pointA,pointB), layer.OuterLoop.ElementAt(0));
            Assert.AreEqual(new Segment2D(pointB,pointA), layer.OuterLoop.ElementAt(1));
        }

        [Test]
        public void OuterLoop_ThreeConnectedSegment_SetsNewLoop()
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);

            // Call
            layer.OuterLoop = new List<Segment2D>
            {
                new Segment2D(pointA,pointB),
                new Segment2D(pointB,pointC),
                new Segment2D(pointC,pointA)
            };

            // Assert
            Assert.NotNull(layer.OuterLoop);
            Assert.AreEqual(new Segment2D(pointA, pointB), layer.OuterLoop.ElementAt(0));
            Assert.AreEqual(new Segment2D(pointB, pointC), layer.OuterLoop.ElementAt(1));
            Assert.AreEqual(new Segment2D(pointC, pointA), layer.OuterLoop.ElementAt(2));
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void AddInnerLoop_TwoDisconnectedSegment_ThrowsArgumentException(double diff)
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(0.0, diff);

            // Call
            TestDelegate test = () => layer.AddInnerLoop(new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointC)
            });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void AddInnerLoop_ThreeDisconnectedSegment_ThrowsArgumentException(double diff)
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0,0.0);
            var pointB = new Point2D(1.0,0.0);
            var pointC = new Point2D(1.0,1.0);
            var pointD = new Point2D(0.0,diff);

            // Call
            TestDelegate test = () => layer.AddInnerLoop(new List<Segment2D>
            {
                new Segment2D(pointA,pointB),
                new Segment2D(pointB,pointC),
                new Segment2D(pointC,pointD),
            });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("De segmenten van de geometrie van de laag vormen geen lus.", exception.Message);
        }

        [Test]
        public void AddInnerLoop_TwoConnectedSegment_SetsNewLoop()
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);

            // Call
            layer.AddInnerLoop(new List<Segment2D>
            {
                new Segment2D(pointA,pointB),
                new Segment2D(pointB,pointA)
            });

            // Assert
            Assert.AreEqual(1, layer.InnerLoops.Count());
            Assert.AreEqual(new Segment2D(pointA, pointB), layer.InnerLoops.ElementAt(0)[0]);
            Assert.AreEqual(new Segment2D(pointB, pointA), layer.InnerLoops.ElementAt(0)[1]);
        }

        [Test]
        public void AddInnerLoop_ThreeConnectedSegment_SetsNewLoop()
        {
            // Setup
            var layer = new SoilLayer2D();
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);

            // Call
            layer.AddInnerLoop(new List<Segment2D>
            {
                new Segment2D(pointA,pointB),
                new Segment2D(pointB,pointC),
                new Segment2D(pointC,pointA)
            });

            // Assert
            Assert.AreEqual(1, layer.InnerLoops.Count());
            Assert.AreEqual(new Segment2D(pointA,pointB), layer.InnerLoops.ElementAt(0)[0]);
            Assert.AreEqual(new Segment2D(pointB, pointC), layer.InnerLoops.ElementAt(0)[1]);
            Assert.AreEqual(new Segment2D(pointC, pointA), layer.InnerLoops.ElementAt(0)[2]);
        }

        [Test]
        public void AsPipingSoilLayers_DefaultConstructed_ReturnsEmptyCollectionWithMaxValueBottom()
        {
            // Setup
            var layer = new SoilLayer2D();
            double bottom;

            // Call
            var result = layer.AsPipingSoilLayers(0.0, out bottom);

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.AreEqual(double.MaxValue, bottom);
        }

        [Test]
        public void AsPipingSoilLayer_PropertiesSetWithDifferentLayerParameters_PropertiesAreSetInPipingSoilLayer()
        {
            // Setup
            var random = new Random(22);
            var y1 = random.NextDouble();
            var y2 = y1 + random.NextDouble();
            var x1 = 1.0;
            var x2 = 1.1;
            var x3 = 1.2;
            var materialName = "materialX";
            var color = Color.DarkSeaGreen;
            double bottom;

            var logNormalDistribution = 3;
            var logNormalShift = 0;

            var belowPhreaticLevelMean = random.NextDouble();
            var belowPhreaticLevelDeviation = random.NextDouble();

            var diameterD70Mean = random.NextDouble();
            var diameterD70Deviation = random.NextDouble();

            var permeabilityMean = random.NextDouble();
            var permeabilityDeviation = random.NextDouble();

            var layer = new SoilLayer2D
            {
                MaterialName = materialName,
                IsAquifer = 1.0,
                Color = color.ToArgb(),

                BelowPhreaticLevelDistribution = logNormalDistribution,
                BelowPhreaticLevelShift = logNormalShift,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,

                DiameterD70Distribution = logNormalDistribution,
                DiameterD70Shift = logNormalShift,
                DiameterD70Mean = diameterD70Mean,
                DiameterD70Deviation = diameterD70Deviation,

                PermeabilityDistribution = logNormalDistribution,
                PermeabilityShift = logNormalShift,
                PermeabilityMean = permeabilityMean,
                PermeabilityDeviation = permeabilityDeviation,

                OuterLoop = new List<Segment2D>
                {
                    new Segment2D(
                        new Point2D(x1, y1),
                        new Point2D(x3, y1)
                        ),
                    new Segment2D(
                        new Point2D(x3, y1),
                        new Point2D(x3, y2)
                        ),
                    new Segment2D(
                        new Point2D(x3, y2),
                        new Point2D(x1, y2)
                        ),
                    new Segment2D(
                        new Point2D(x1, y1),
                        new Point2D(x1, y2)
                        )
                }
            };

            // Call
            var result = layer.AsPipingSoilLayers(x2, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(y1, bottom, 1e-6);
            var resultLayer = result.First();
            Assert.AreEqual(y2, resultLayer.Top, 1e-6);
            Assert.IsTrue(resultLayer.IsAquifer);
            Assert.AreEqual(materialName, resultLayer.MaterialName);
            Assert.AreEqual(Color.FromArgb(color.ToArgb()), resultLayer.Color);

            Assert.AreEqual(belowPhreaticLevelMean, resultLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, resultLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(diameterD70Mean, resultLayer.DiameterD70Mean);
            Assert.AreEqual(diameterD70Deviation, resultLayer.DiameterD70Deviation);
            Assert.AreEqual(permeabilityMean, resultLayer.PermeabilityMean);
            Assert.AreEqual(permeabilityDeviation, resultLayer.PermeabilityDeviation);
        }

        [Test]
        public void AsPipingSoilLayer_PropertiesSetWithNullMaterialName_MaterialNameEmptyInPipingSoilLayer()
        {
            // Setup
            var layer = new SoilLayer2D
            {
                OuterLoop = new List<Segment2D>
                {
                    new Segment2D(
                        new Point2D(0, 1),
                        new Point2D(1, 1)
                        ),
                    new Segment2D(
                        new Point2D(1, 1),
                        new Point2D(0, 1)
                        )
                }
            };
            double bottom;

            // Call
            var result = layer.AsPipingSoilLayers(0, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.IsEmpty(result[0].MaterialName);
        }

        [Test]
        public void AsPipingSoilLayers_WithOuterLoopNotIntersectingX_ReturnsEmptyCollectionWithMaxValueBottom()
        {
            // Setup
            var random = new Random(22);
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();

            var layer = new SoilLayer2D
            {
                OuterLoop = new List<Segment2D>
                {
                    new Segment2D(
                        new Point2D(1.0, y1),
                        new Point2D(1.2, y2)
                        ),
                    new Segment2D(
                        new Point2D(1.2, y2),
                        new Point2D(1.0, y1)
                        )
                }
            };
            double bottom;

            // Call
            var result = layer.AsPipingSoilLayers(0.0, out bottom);

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.AreEqual(double.MaxValue, bottom);
        }

        [Test]
        public void AsPipingSoilLayers_WithOuterLoopIntersectingX_ReturnsBottomAndLayerWithTop()
        {
            // Setup
            var expectedZ = new Random(22).NextDouble();
            var layer = new SoilLayer2D
            {
                OuterLoop = new List<Segment2D>
                {
                    new Segment2D(
                        new Point2D(-0.1, expectedZ),
                        new Point2D(0.1, expectedZ)
                        ),
                    new Segment2D(
                        new Point2D(-0.1, expectedZ),
                        new Point2D(0.1, expectedZ)
                        )
                }
            };
            double bottom;

            // Call
            var result = layer.AsPipingSoilLayers(0.0, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(expectedZ, bottom);
            Assert.AreEqual(expectedZ, result[0].Top);
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopComplex_ReturnsTwoLayers()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "..8.7...",
                                                                               "..5.6...",
                                                                               "..4..3..",
                                                                               "........"));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop
            };

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[] { 5.0, 2.0 }, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopInnerLoopSimple_ReturnsTwoLayers()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine, 
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "..4..3.."));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "........",
                                                                               "...12...",
                                                                               "........",
                                                                               "........",
                                                                               "...43...",
                                                                               "........"));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new [] {5.0, 1.0}, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopInnerLoopComplex_ReturnsThreeLayers()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "..4..3.."));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "........",
                                                                               "...1.2..",
                                                                               "...87...",
                                                                               "...56...",
                                                                               "...4.3..",
                                                                               "........"));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[] { 5.0, 3.0, 1.0 }, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopMultipleInnerLoops_ReturnsThreeLayers()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "..4..3.."));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "........",
                                                                               "...12...",
                                                                               "...43...",
                                                                               "........",
                                                                               "........",
                                                                               "........"));

            var innerLoop2 = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                                "6",
                                                                                "........",
                                                                                "........",
                                                                                "........",
                                                                                "........",
                                                                                "...12...",
                                                                                "........"));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);
            layer.AddInnerLoop(innerLoop2);

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(0, bottom);
            CollectionAssert.AreEquivalent(new[] { 5.0, 3.0, 1.0 }, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopOverlappingInnerLoop_ReturnsOneLayer()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               ".4....3.",
                                                                               "........"));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "...12...",
                                                                               "........",
                                                                               "...43..."));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(2.0, bottom);
            CollectionAssert.AreEquivalent(new[] { 5.0 }, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopOverlappingInnerLoopsFirstInnerLoopNotOverBottom_ReturnsOneLayer()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               ".4....3.",
                                                                               "........"));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "........",
                                                                               "...12...",
                                                                               "........",
                                                                               "...43...",
                                                                               "........",
                                                                               "........"));

            var innerLoop2 = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                                "6",
                                                                                "........",
                                                                                "........",
                                                                                "...12...",
                                                                                "........",
                                                                                "........",
                                                                                "...43..."));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);
            layer.AddInnerLoop(innerLoop2);

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(4.0, bottom);
            CollectionAssert.AreEquivalent(new[] { 5.0 }, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopInnerLoopOnBorderBottom_ReturnsTwoLayers()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               ".4....3.",
                                                                               "........"));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "........",
                                                                               "........",
                                                                               "...12...",
                                                                               "........",
                                                                               "...43...",
                                                                               "........"));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(3.0, bottom);
            CollectionAssert.AreEquivalent(new[] { 5.0, 1.0 }, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopInnerLoopOverlapTop_ReturnsOneLayer()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "........",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               ".4....3.",
                                                                               "........"));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "...43...",
                                                                               "........",
                                                                               "...12...",
                                                                               "........",
                                                                               "........",
                                                                               "........"));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[] { 3.0 }, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopInnerLoopOnBorderTop_ReturnsOneLayer()
        {
            // Setup
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               ".4....3.",
                                                                               "........"));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "...43...",
                                                                               "........",
                                                                               "...12...",
                                                                               "........",
                                                                               "........",
                                                                               "........"));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);

            // Call
            double bottom;
            var result = layer.AsPipingSoilLayers(3.5, out bottom).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1.0, bottom);
            CollectionAssert.AreEquivalent(new[] { 3.0 }, result.Select(rl => rl.Top));
        }

        [Test]
        public void AsPipingSoilLayers_OuterLoopVerticalAtX_ThrowsException()
        {
            // Setup
            var atX = 2.0;
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "..4..3.."));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop
            };

            // Call
            double bottom;
            TestDelegate test = () => layer.AsPipingSoilLayers(atX, out bottom);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual(String.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_X_0_, atX), exception.Message);
        }

        [Test]
        public void AsPipingSoilLayers_InnerLoopVerticalAtX_ThrowsException()
        {
            // Setup
            var atX = 3.0;
            var outerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "..1..2..",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "........",
                                                                               "..4..3.."));

            var innerLoop = Segment2DLoopCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "6",
                                                                               "........",
                                                                               "...1.2..",
                                                                               "........",
                                                                               "........",
                                                                               "...4.3..",
                                                                               "........"));

            var layer = new SoilLayer2D
            {
                OuterLoop = outerLoop,
            };
            layer.AddInnerLoop(innerLoop);

            // Call
            double bottom;
            TestDelegate test = () => layer.AsPipingSoilLayers(atX, out bottom);

            // Assert
            var exception = Assert.Throws<SoilLayerConversionException>(test);
            Assert.AreEqual(String.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_X_0_, atX), exception.Message);
        }

        [Test]
        [TestCase(false, true, true, "Verzadigd gewicht")]
        [TestCase(true, false, true, "Korrelgrootte")]
        [TestCase(true, true, false, "Doorlatendheid")]
        public void AsPipingSoilLayer_IncorrectDistributionType_ThrowsSoilLayerConversionException(
            bool isBelowPhreaticLevelDistributionValid,
            bool isDiameterD70DistributionValid,
            bool isPermeabilityDistributionValid,
            string expectedParameter)
        {
            // Setup
            var validShift = 0.0;
            var layer = new SoilLayer2D
            {
                BelowPhreaticLevelDistribution = isBelowPhreaticLevelDistributionValid ? SoilLayerConstants.LogNormalDistributionValue : -1,
                BelowPhreaticLevelShift = validShift,
                DiameterD70Distribution = isDiameterD70DistributionValid ? SoilLayerConstants.LogNormalDistributionValue : -1,
                DiameterD70Shift = validShift,
                PermeabilityDistribution = isPermeabilityDistributionValid ? SoilLayerConstants.LogNormalDistributionValue : -1,
                PermeabilityShift = validShift
            };

            double bottom;

            // Call
            TestDelegate test = () => layer.AsPipingSoilLayers(0, out bottom);

            // Assert
            var message = Assert.Throws<SoilLayerConversionException>(test).Message;
            Assert.AreEqual(string.Format("De parameter '{0}' is niet lognormaal verdeeld.", expectedParameter), message);
        }

        [Test]
        [TestCase(1e-6, 0.0, 0.0, "Verzadigd gewicht")]
        [TestCase(0.0, -1e-6, 0.0, "Korrelgrootte")]
        [TestCase(0.0, 0.0, 9, "Doorlatendheid")]
        public void AsPipingSoilLayer_ShiftNotZero_ThrowsSoilLayerConversionException(
            double belowPhreaticLevelShift,
            double diameterD70Shift,
            double permeabilityShift,
            string expectedParameter)
        {
            // Setup
            var validDistribution = SoilLayerConstants.LogNormalDistributionValue;
            var layer = new SoilLayer2D
            {
                BelowPhreaticLevelDistribution = validDistribution,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                DiameterD70Distribution = validDistribution,
                DiameterD70Shift = diameterD70Shift,
                PermeabilityDistribution = validDistribution,
                PermeabilityShift = permeabilityShift
            };

            double bottom;

            // Call
            TestDelegate test = () => layer.AsPipingSoilLayers(0, out bottom);

            // Assert
            var message = Assert.Throws<SoilLayerConversionException>(test).Message;
            Assert.AreEqual(string.Format("De parameter '{0}' is niet lognormaal verdeeld.", expectedParameter), message);
        }
    }
}