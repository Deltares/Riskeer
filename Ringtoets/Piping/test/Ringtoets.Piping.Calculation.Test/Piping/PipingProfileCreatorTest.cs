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
            IEnumerable<PipingSoilLayer> layers = new []
            {
                new PipingSoilLayer(expectedTop), 
            };
            var soilProfile = new PipingSoilProfile(String.Empty, expectedBottom, layers);

            // Call
            PipingProfile actual = PipingProfileCreator.Create(soilProfile);

            // Assert
            Assert.IsNotNull(actual.Layers);
            Assert.IsNotNull(actual.BottomAquiferLayer);
            Assert.IsNotNull(actual.TopAquiferLayer);
            Assert.AreEqual(1, actual.Layers.Count);
            Assert.IsTrue(actual.Layers.First(l => Math.Abs(l.TopLevel - actual.Layers.Max(ll => ll.TopLevel)) < 1e-6).IsAquifer);
            Assert.AreEqual(expectedTop, actual.Layers[0].TopLevel);
            Assert.AreEqual(expectedTop, actual.TopLevel);
            Assert.AreEqual(expectedBottom, actual.BottomLevel);
        }

        [Test]
        public void Create_ProfileWithMultipleLayers_ReturnsProfileWithMultipleLayers()
        {
            // Setup
            var random = new Random(22);
            var expectedTopA = random.NextDouble();
            var expectedTopB = random.NextDouble() + expectedTopA;
            var expectedTopC = random.NextDouble() + expectedTopB;
            var expectedBottom = random.NextDouble();
            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(expectedTopA), 
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
            CollectionAssert.AreEqual(new []{ expectedTopC, expectedTopB, expectedTopA }, actual.Layers.Select(l => l.TopLevel));
            Assert.AreEqual(expectedBottom, actual.BottomLevel);
        }

        [Test]
        public void Create_ProfileWithDecreasingTops_ReturnsProfileWithSingleLayer()
        {
            // Setup
            var random = new Random(22);
            var expectedTopA = random.NextDouble();
            var expectedTopB = expectedTopA - random.NextDouble();
            var expectedTopC = expectedTopB - random.NextDouble();
            var expectedBottom = random.NextDouble();
            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(expectedTopA), 
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
    }
}