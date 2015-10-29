using System;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    public class PipingSoilProfileTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void Constructor_WithNameBottomLayersAndAquifer_ReturnsInstanceWithPropsAndEquivalentLayerCollection(int layerCount)
        {
            // Setup
            var name = "Profile";
            var bottom = new Random(22).NextDouble();
            var equivalentLayers = new Collection<PipingSoilLayer>();
            for (var i = 0; i < layerCount; i++)
            {
                equivalentLayers.Add(new PipingSoilLayer(0.0));
                if (i == 0)
                {
                    equivalentLayers[0].IsAquifer = true;
                }
            }

            // Call
            var profile = new PipingSoilProfile(name, bottom, equivalentLayers);

            // Assert
            Assert.AreNotSame(equivalentLayers, profile.Layers);
            CollectionAssert.AreEquivalent(equivalentLayers, profile.Layers);
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void Constructor_WithNameBottomLayersNoAquifer_ThrowsArgumentException(int layerCount)
        {
            // Setup
            var name = "Profile";
            var bottom = new Random(22).NextDouble();
            var equivalentLayers = new Collection<PipingSoilLayer>();
            for (var i = 0; i < layerCount; i++)
            {
                equivalentLayers.Add(new PipingSoilLayer(0.0));
            }

            // Call
            TestDelegate test = () => new PipingSoilProfile(name, bottom, equivalentLayers);

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(Properties.Resources.Error_CannotConstructPipingSoilProfileWithoutAquiferLayer, message);
        }

        [Test]
        public void Constructor_WithNameBottomLayersEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new PipingSoilProfile(String.Empty, Double.NaN, new Collection<PipingSoilLayer>());

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(Properties.Resources.Error_CannotConstructPipingSoilProfileWithoutLayers, message);
        }

        [Test]
        public void Constructor_WithNameBottomLayersNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new PipingSoilProfile(String.Empty, Double.NaN, null);

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(Properties.Resources.Error_CannotConstructPipingSoilProfileWithoutLayers, message);
        }
        
    }
}