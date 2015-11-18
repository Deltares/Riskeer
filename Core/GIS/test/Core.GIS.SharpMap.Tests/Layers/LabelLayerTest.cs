using System.ComponentModel;
using System.Drawing;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Rendering.Thematics;
using NUnit.Framework;
using SharpTestsEx;
using Point = Core.GIS.NetTopologySuite.Geometries.Point;

namespace Core.GIS.SharpMap.Tests.Layers
{
    [TestFixture]
    public class LabelLayerTest
    {
        [Test]
        public void SetGetText()
        {
            var map = new Map.Map(new Size(100, 100));

            var feature1 = new SimpleFeature
            {
                Data = 1, Geometry = new Point(50, 50)
            };
            var feature2 = new SimpleFeature
            {
                Data = 5, Geometry = new Point(50, 55)
            };
            var featureCollection = new FeatureCollection
            {
                Features =
                {
                    feature1, feature2
                }
            };

            var callCount = 0;

            var vectorLayer = new VectorLayer
            {
                DataSource = featureCollection
            };

            var labelLayer = new LabelLayer
            {
                Visible = true,
                DataSource = featureCollection,
                Parent = vectorLayer,
                Map = map,
                LabelStringDelegate = delegate(IFeature feature)
                {
                    callCount++;

                    feature
                        .Should().Be.OfType<SimpleFeature>();

                    return ((SimpleFeature) feature).Data.ToString();
                }
            };

            labelLayer.Render();

            callCount
                .Should("labels of 2 simple feature rendered").Be.EqualTo(2);
        }

        [Test]
        [Ignore("WTI-81 | Will be activated (and will run correctly) when Entity is removed from Layer")]
        public void LabelLayersBubblesPropertyChangesOfTheme()
        {
            var counter = 0;
            var theme = new CategorialTheme();
            var labelLayer = new LabelLayer { Theme = theme };

            ((INotifyPropertyChanged) labelLayer).PropertyChanged += (sender, e) =>
            {
                if (sender is ITheme)
                {
                    counter++;
                }
            };

            theme.AttributeName = "Test";

            Assert.AreEqual(1, counter);
        }

        private class SimpleFeature : Feature
        {
            public int Data { get; set; }
        }
    }
}