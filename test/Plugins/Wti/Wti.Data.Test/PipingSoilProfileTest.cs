using System;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Wti.Data.Test
{
    public class PipingSoilProfileTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void Constructor_WithNameBottomLayers_ReturnsInstanceWithPropsAndEquivalentLayerCollection(int layerCount)
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
            var profile = new PipingSoilProfile(name, bottom, equivalentLayers);

            // Assert
            Assert.AreNotSame(equivalentLayers, profile.Layers);
            CollectionAssert.AreEquivalent(equivalentLayers, profile.Layers);
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom);
        }
        
    }
}