using System.ComponentModel;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Rendering.Thematics;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.Layers
{
    [TestFixture]
    public class LayerTest
    {
        [Test]
        public void Theme_PropertyHasChanged_EventBubbledByLayer()
        {
            var counter = 0;
            var theme = new CategorialTheme();
            var testLayer = new TestLayer { Theme = theme };

            ((INotifyPropertyChanged) testLayer).PropertyChanged += (sender, e) =>
            {
                if (sender is ITheme)
                {
                    counter++;
                }
            };

            theme.AttributeName = "Test";

            Assert.AreEqual(1, counter);
        }

        [Test]
        public void LabelLayer_PropertyHasChanged_EventBubbledByLayer()
        {
            var counter = 0;
            var labelLayer = new LabelLayer();
            var testLayer = new TestLayer { LabelLayer = labelLayer };

            ((INotifyPropertyChanged) testLayer).PropertyChanged += (sender, e) =>
            {
                if (sender is ILabelLayer)
                {
                    counter++;
                }
            };

            labelLayer.Priority = 1;

            Assert.AreEqual(1, counter);
        }

        private class TestLayer : Layer
        {

        }
    }
}
