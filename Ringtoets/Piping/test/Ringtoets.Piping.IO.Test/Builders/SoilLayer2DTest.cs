using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.Test.Builders
{
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
        }

        [Test]
        [TestCase(1e-8)]
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
        [TestCase(1e-8)]
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
            Assert.AreEqual(new Segment2D(pointA,pointB), layer.OuterLoop[0]);
            Assert.AreEqual(new Segment2D(pointB,pointA), layer.OuterLoop[1]);
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
            Assert.AreEqual(new Segment2D(pointA,pointB), layer.OuterLoop[0]);
            Assert.AreEqual(new Segment2D(pointB,pointC), layer.OuterLoop[1]);
            Assert.AreEqual(new Segment2D(pointC,pointA), layer.OuterLoop[2]);
        }

        [Test]
        [TestCase(1e-8)]
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
        [TestCase(1e-8)]
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
                        new Point2D
                        {
                            X = 1.0, Y = y1
                        },
                        new Point2D
                        {
                            X = 1.2, Y = y2
                        }),
                        
                    new Segment2D(
                        new Point2D
                        {
                            X = 1.2, Y = y2
                        },
                        new Point2D
                        {
                            X = 1.0, Y = y1
                        })
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
                        new Point2D
                        {
                            X = -0.1, Y = expectedZ
                        },
                        new Point2D
                        {
                            X = 0.1, Y = expectedZ
                        }),
                        
                    new Segment2D(
                        new Point2D
                        {
                            X = -0.1, Y = expectedZ
                        },
                        new Point2D
                        {
                            X = 0.1, Y = expectedZ
                        })
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
            var exception = Assert.Throws<SoilLayer2DConversionException>(test);
            Assert.AreEqual(String.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_x, atX), exception.Message);
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
            var exception = Assert.Throws<SoilLayer2DConversionException>(test);
            Assert.AreEqual(String.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_x, atX), exception.Message);
        }
    }
}