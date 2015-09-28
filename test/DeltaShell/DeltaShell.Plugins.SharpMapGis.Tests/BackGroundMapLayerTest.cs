using NUnit.Framework;
using SharpMap;
using SharpMap.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Tests
{
    [TestFixture]
    public class BackGroundMapLayerTest
    {
        [Test]
        public void ChangingDefaultMapUpdatesBackGroundLayer()
        {
            var map = new Map();
            var backgroundLayer = new BackGroundMapLayer(map);

            //change the default map
            map.Layers.Add(new VectorLayer("Bla"));

            //assert the background got updated
            Assert.AreEqual(1,backgroundLayer.Layers.Count);
        }

        [Test]
        public void UpdateDoesNotChangeDisableLayers()
        {
            var map = new Map();
            
            var backgroundLayer = new BackGroundMapLayer(map);

            //change the default map
            map.Layers.Add(new VectorLayer("Bla"));

            backgroundLayer.Layers[0].Visible = false;

            backgroundLayer.UpdateLayers();

            Assert.IsFalse(backgroundLayer.Layers[0].Visible);
        }

        [Test]
        public void BackgroundMapLayersInOtherMapAreNotSelectable()
        {
            var defaultMap = new Map();

            var vectorLayer = new VectorLayer("Bla");
            defaultMap.Layers.Add(vectorLayer);
            
            vectorLayer.Selectable = true; //force selectable

            var backgroundLayer = new BackGroundMapLayer(defaultMap);
            
            var map = new Map();

            map.Layers.Add(backgroundLayer);

            vectorLayer.Selectable = true; //force selectable again, just to be sure

            Assert.IsFalse(((GroupLayer) map.Layers[0]).Layers[0].Selectable);
        }

        [Test]
        public void CloneTest()
        {
            var defaultMap = new Map();
            var vectorLayer = new VectorLayer("Bla");
            var backgroundLayer = new BackGroundMapLayer(defaultMap){Name = "ABC"};

            defaultMap.Layers.Add(vectorLayer);

            var clonedBackgroundLayer = backgroundLayer.Clone() as BackGroundMapLayer;

            Assert.NotNull(clonedBackgroundLayer);
            Assert.AreEqual(backgroundLayer.Name, clonedBackgroundLayer.Name);
            Assert.AreEqual(backgroundLayer.Layers.Count, clonedBackgroundLayer.Layers.Count);
            Assert.AreEqual(backgroundLayer.Layers[0].Name, clonedBackgroundLayer.Layers[0].Name);
        }
    }
}
