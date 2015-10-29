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
            var layer = new SoilLayer2D
            {
                OuterLoop = new HashSet<Point3D>
                {
                    new Point3D
                    {
                        X = 0.1, Z = new Random(22).NextDouble()
                    }
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
                OuterLoop = new HashSet<Point3D>
                {
                    new Point3D
                    {
                        X = -0.1, Z = expectedZ
                    },
                    new Point3D
                    {
                        X = 0.1, Z = expectedZ
                    }
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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine, 
                "6",
                "..1..2..",
                "........",
                "........",
                "........",
                "........",
                "..4..3.."));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop
                }
            };

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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "..1..2..",
                "........",
                "........",
                "........",
                "........",
                "..4..3.."));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop
                }
            };

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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "..1..2..",
                "........",
                "........",
                "........",
                "........",
                "..4..3.."));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "........",
                "...12...",
                "...43...",
                "........",
                "........",
                "........"));

            var innerLoop2 = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop,
                    innerLoop2
                }
            };

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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "..1..2..",
                "........",
                "........",
                "........",
                ".4....3.",
                "........"));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop
                }
            };

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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "..1..2..",
                "........",
                "........",
                "........",
                ".4....3.",
                "........"));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "........",
                "...12...",
                "........",
                "...43...",
                "........",
                "........"));

            var innerLoop2 = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop,
                    innerLoop2
                }
            };

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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "..1..2..",
                "........",
                "........",
                "........",
                ".4....3.",
                "........"));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop
                }
            };

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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "........",
                "..1..2..",
                "........",
                "........",
                ".4....3.",
                "........"));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop
                }
            };

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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "..1..2..",
                "........",
                "........",
                "........",
                ".4....3.",
                "........"));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop
                }
            };

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
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
            Assert.AreEqual(String.Format(Resources.Error_CanNotDetermine1DProfileWithVerticalSegmentsAtX, atX), exception.Message);
        }

        [Test]
        public void AsPipingSoilLayers_InnerLoopVerticalAtX_ThrowsException()
        {
            // Setup
            var atX = 3.0;
            var outerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                "6",
                "..1..2..",
                "........",
                "........",
                "........",
                "........",
                "..4..3.."));

            var innerLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                InnerLoops =
                {
                    innerLoop
                }
            };

            // Call
            double bottom;
            TestDelegate test = () => layer.AsPipingSoilLayers(atX, out bottom);

            // Assert
            var exception = Assert.Throws<SoilLayer2DConversionException>(test);
            Assert.AreEqual(String.Format(Resources.Error_CanNotDetermine1DProfileWithVerticalSegmentsAtX, atX), exception.Message);
        }
    }
}