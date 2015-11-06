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
    public class SoilProfileBuilder2DTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("name")]
        public void Constructor_WithNameInvalidX_ThrowsArgumentException(string name)
        {
            // Call
            TestDelegate test = () => new SoilProfileBuilder2D(name, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(Resources.Error_SoilProfileBuilder_cant_determine_intersect_at_double_NaN, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("name")]
        public void Constructor_WithNameValidX_ReturnsNewInstance(string name)
        {
            // Call
            TestDelegate test = () => new SoilProfileBuilder2D(name, 0.0);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Build_WithOutLayers_ThrowsSoilProfileBuilderException()
        {
            // Setup
            var profileName = "SomeProfile";
            var builder = new SoilProfileBuilder2D(profileName, 0.0);

            // Call
            TestDelegate test = () => builder.Build();

            // Assert
            Assert.Throws<SoilProfileBuilderException>(test);
        }

        [Test]
        public void Build_WithSingleLayerOnlyOuterLoop_ReturnsProfileWithBottomAndALayer()
        {
            // Setup
            var profileName = "SomeProfile";
            var builder = new SoilProfileBuilder2D(profileName, 0.0);
            var firstPoint = new Point2D
            {
                X = -0.5, Y = 1.0
            };
            var secondPoint = new Point2D
            {
                X = 0.5, Y = 1.0
            };
            var thirdPoint = new Point2D
            {
                X = 0.5, Y = -1.0
            };
            var fourthPoint = new Point2D
            {
                X = -0.5, Y = -1.0
            };
            builder.Add(new SoilLayer2D
            {
                OuterLoop = new List<Segment2D>
                {
                    new Segment2D(firstPoint,secondPoint),
                    new Segment2D(secondPoint,thirdPoint),
                    new Segment2D(thirdPoint,fourthPoint),
                    new Segment2D(fourthPoint,firstPoint)
                },
                IsAquifer = 1.0
            });

            // Call
            PipingSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(1, soilProfile.Layers.Count());
            Assert.AreEqual(1.0, soilProfile.Layers.ToArray()[0].Top);
            Assert.AreEqual(-1.0, soilProfile.Bottom);
        }

        [Test]
        public void Build_WithMultipleLayersOnlyOuterLoop_ReturnsProfileWithBottomAndALayers()
        {
            // Setup
            var profileName = "SomeProfile";
            var builder = new SoilProfileBuilder2D(profileName, 1.0);
            builder.Add(new SoilLayer2D
            {
                OuterLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                    "..."
                ))
            }).Add(new SoilLayer2D
            {
                OuterLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                    "..."
                ))
            }).Add(new SoilLayer2D
            {
                OuterLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
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
                    "..."
                )),
                IsAquifer = 1.0
            });

            // Call
            PipingSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(3, soilProfile.Layers.Count());
            CollectionAssert.AreEquivalent(new[] { 2.0, 4.0, 8.0 }, soilProfile.Layers.Select(rl => rl.Top));
            Assert.AreEqual(1.0, soilProfile.Bottom);
        }


        [Test]
        public void Build_WithLayerFilledWithOtherLayer_ReturnsProfileWithBottomAndALayers()
        {
            // Setup
            var profileName = "SomeProfile";
            var builder = new SoilProfileBuilder2D(profileName, 2.0);
            var loopHole = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                    "5",
                    ".....",
                    ".4.1.",
                    ".3.2.",
                    ".....",
                    "....."
                ));
            var soilLayer2D = new SoilLayer2D
            {
                OuterLoop = PointCollectionHelper.CreateFromString(String.Join(Environment.NewLine,
                                                                               "5",
                                                                               "2...3",
                                                                               ".....",
                                                                               ".....",
                                                                               ".....",
                                                                               "1...4"
                                                                       )),
                IsAquifer = 1.0
            };
            soilLayer2D.AddInnerLoop(loopHole);
            builder.Add(soilLayer2D).Add(new SoilLayer2D
            {
                OuterLoop = loopHole
            });

            // Call
            PipingSoilProfile soilProfile = builder.Build();

            // Assert
            Assert.AreEqual(profileName, soilProfile.Name);
            Assert.AreEqual(3, soilProfile.Layers.Count());
            CollectionAssert.AreEquivalent(new[] { 4.0, 3.0, 2.0 }, soilProfile.Layers.Select(rl => rl.Top));
            Assert.AreEqual(0.0, soilProfile.Bottom);
        }
    }
}