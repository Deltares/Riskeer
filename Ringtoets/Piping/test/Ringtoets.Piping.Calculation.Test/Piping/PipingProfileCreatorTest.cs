using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deltares.WTIPiping;
using NUnit.Framework;

using Ringtoets.Piping.Calculation.Piping;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Test.Piping
{
    public class PipingProfileCreatorTest
    {
        [Test]
        public void Create_ProfileWithOneLayer_ReturnsProfileWithSingleLayer()
        {
            // Setup
            var random = new Random(22);
            var expectedTop = random.NextDouble();
            var expectedBottom = random.NextDouble();
            var belowPhreaticLevel = random.NextDouble();
            var abovePhreaticLevel = random.NextDouble();
            var dryUnitWeight = random.NextDouble();
            IEnumerable<PipingSoilLayer> layers = new []
            {
                new PipingSoilLayer(expectedTop)
                {
                    IsAquifer = true,
                    BelowPhreaticLevel = belowPhreaticLevel,
                    AbovePhreaticLevel = abovePhreaticLevel,
                    DryUnitWeight = dryUnitWeight
                }, 
            };
            var soilProfile = new PipingSoilProfile(String.Empty, expectedBottom, layers);

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

            var pipingLayer = actual.Layers.First();
            Assert.IsTrue(pipingLayer.IsAquifer);
            Assert.AreEqual(belowPhreaticLevel, pipingLayer.BelowPhreaticLevel);
            Assert.AreEqual(abovePhreaticLevel, pipingLayer.AbovePhreaticLevel);
            Assert.AreEqual(dryUnitWeight, pipingLayer.DryUnitWeight);
        }

        [Test]
        public void Create_ProfileWithDecreasingTops_ReturnsProfileWithMultipleLayers()
        {
            // Setup
            var random = new Random(22);
            var expectedTopA = random.NextDouble();
            var expectedTopB = expectedTopA - random.NextDouble();
            var expectedTopC = expectedTopB - random.NextDouble();
            var expectedBottom = random.NextDouble();
            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(expectedTopA)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(expectedTopB),
                new PipingSoilLayer(expectedTopC)
            };

            var soilProfile = new PipingSoilProfile(String.Empty, expectedBottom, layers);

            // Call
            PipingProfile actual = PipingProfileCreator.Create(soilProfile);

            // Assert
            Assert.AreEqual(3, actual.Layers.Count);
            IEnumerable expectedAquifers = new[]
            {
                true,
                false,
                false,
            };
            CollectionAssert.AreEqual(expectedAquifers, actual.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new[] { expectedTopA, expectedTopB, expectedTopC }, actual.Layers.Select(l => l.TopLevel));
            Assert.AreEqual(expectedBottom, actual.BottomLevel);
        }

        [Test]
        public void Create_ProfileWithMultipleLayers_ReturnsProfileWithOrderedMultipleLayers()
        {
            // Setup
            var random = new Random(22);
            var expectedTopA = random.NextDouble();
            var expectedTopB = random.NextDouble() + expectedTopA;
            var expectedTopC = random.NextDouble() + expectedTopB;
            var expectedBottom = random.NextDouble();
            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(expectedTopA)
                {
                    IsAquifer = true
                }, 
                new PipingSoilLayer(expectedTopB), 
                new PipingSoilLayer(expectedTopC) 
            };
            var soilProfile = new PipingSoilProfile(string.Empty, expectedBottom, layers);

            // Precondition
            CollectionAssert.AreNotEqual(layers, layers.OrderByDescending(l => l.Top), "Layer collection should not be in descending order by the Top property.");

            // Call
            PipingProfile actual = PipingProfileCreator.Create(soilProfile);

            // Assert
            var ordered = actual.Layers.OrderByDescending(l => l.TopLevel);
            CollectionAssert.AreEqual(ordered, actual.Layers);

            Assert.AreEqual(3, actual.Layers.Count);
            IEnumerable expectedAquifers = new[]
            {
                false,
                false,
                true,
                
            };
            CollectionAssert.AreEqual(expectedAquifers, actual.Layers.Select(l => l.IsAquifer));
            CollectionAssert.AreEqual(new []{ expectedTopC, expectedTopB, expectedTopA }, actual.Layers.Select(l => l.TopLevel));
            Assert.AreEqual(expectedBottom, actual.BottomLevel);
        }

        [Test]
        public void Create_ProfileWithLayerBelowPhreaticLevelSet_ReturnsLayersWithBelowPhreaticLevel()
        {
            // Setup
            var random = new Random(22);
            var levelA = random.NextDouble();
            var levelB = random.NextDouble();
            var levelC = random.NextDouble();

            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(1)
                {
                    BelowPhreaticLevel = levelA
                }, 
                new PipingSoilLayer(0)
                {
                    BelowPhreaticLevel = levelB
                }, 
                new PipingSoilLayer(-1) 
                {
                    BelowPhreaticLevel = levelC
                }
            };
            var soilProfile = new PipingSoilProfile(string.Empty, -2, layers);

            // Call
            var actual = PipingProfileCreator.Create(soilProfile);

            CollectionAssert.AreEqual(new[]
            {
                levelA,
                levelB,
                levelC
            }, actual.Layers.Select(l => l.BelowPhreaticLevel));
        }

        [Test]
        public void Create_ProfileWithLayerAbovePhreaticLevelSet_ReturnsLayersWithBelowPhreaticLevel()
        {
            // Setup
            var random = new Random(22);
            var levelA = random.NextDouble();
            var levelB = random.NextDouble();
            var levelC = random.NextDouble();

            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(1)
                {
                    AbovePhreaticLevel = levelA
                }, 
                new PipingSoilLayer(0)
                {
                    AbovePhreaticLevel = levelB
                }, 
                new PipingSoilLayer(-1) 
                {
                    AbovePhreaticLevel = levelC
                }
            };
            var soilProfile = new PipingSoilProfile(string.Empty, -2, layers);

            // Call
            var actual = PipingProfileCreator.Create(soilProfile);

            CollectionAssert.AreEqual(new[]
            {
                levelA,
                levelB,
                levelC
            }, actual.Layers.Select(l => l.AbovePhreaticLevel));
        }

        [Test]
        public void Create_ProfileWithLayerDryUnitWeightSet_ReturnsLayersWithBelowPhreaticLevel()
        {
            // Setup
            var random = new Random(22);
            var weightA = random.NextDouble();
            var weightB = random.NextDouble();
            var weightC = random.NextDouble();

            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(1)
                {
                    DryUnitWeight = weightA
                },
                new PipingSoilLayer(0)
                {
                    DryUnitWeight = weightB
                }, 
                new PipingSoilLayer(-1)
                {
                    DryUnitWeight = weightC
                } 
            };
            var soilProfile = new PipingSoilProfile(string.Empty, -2, layers);

            // Call
            var actual = PipingProfileCreator.Create(soilProfile);

            CollectionAssert.AreEqual(new[]
            {
                weightA,
                weightB,
                weightC
            }, actual.Layers.Select(l => l.DryUnitWeight));
        }
    }
}