using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Utils;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Extensions.Features;
using NUnit.Framework;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.UI.Forms;
using SharpMap.UI.Tools;

namespace SharpMap.UI.Tests.Tools
{
    [TestFixture]
    public class NewPointFeatureToolTest
    {
        [Test]
        public void NewPointToolShouldCreateNewNameWithNewNameFormatSet()
        {
            // Create map and map control
            Map map = new Map();
            var testFeatures = new List<TestFeature>();

            var vectorLayer = new VectorLayer
            {
                DataSource = new FeatureCollection(testFeatures, typeof(TestFeature)),
                Visible = true,
                Style = new VectorStyle
                {
                    Fill = new SolidBrush(Color.Tomato),
                    Symbol = null,
                    Line = new Pen(Color.Turquoise, 3)
                }
            };
            map.Layers.Add(vectorLayer);

            var mapControl = new MapControl { Map = map };
            mapControl.Resize += delegate { mapControl.Refresh(); };
            mapControl.Dock = DockStyle.Fill;

            var newPointFeatureTool = new NewPointFeatureTool(l => true, "new test feature") { NewNameFormat = "New test feature {0}" };
            mapControl.Tools.Add(newPointFeatureTool);

            var args = new MouseEventArgs(MouseButtons.Left, 1, -1, -1, -1);

            newPointFeatureTool.OnMouseDown(new Coordinate(0, 20), args);
            newPointFeatureTool.OnMouseMove(new Coordinate(0, 20), args);
            newPointFeatureTool.OnMouseUp(new Coordinate(0, 20), args);

            Assert.IsFalse(newPointFeatureTool.IsBusy);
            Assert.AreEqual(1, testFeatures.Count);
            Assert.AreEqual("New test feature 1", testFeatures[0].Name);

            newPointFeatureTool.OnMouseDown(new Coordinate(0, 20), args);
            newPointFeatureTool.OnMouseMove(new Coordinate(0, 20), args);
            newPointFeatureTool.OnMouseUp(new Coordinate(0, 20), args);

            Assert.AreEqual(2, testFeatures.Count);
            Assert.AreEqual("New test feature 2", testFeatures[1].Name);
        }

        [Test]
        public void NewPointToolShouldWorkWithoutSnapRules()
        {
            // Create map and map control
            Map map = new Map();
            var testFeatures = new List<TestFeature>();

            var vectorLayer = new VectorLayer
                {
                    DataSource = new FeatureCollection(testFeatures, typeof (TestFeature)),
                    Visible = true,
                    Style = new VectorStyle
                        {
                            Fill = new SolidBrush(Color.Tomato),
                            Symbol = null,
                            Line = new Pen(Color.Turquoise, 3)
                        }
                };
            map.Layers.Add(vectorLayer);

            var mapControl = new MapControl {Map = map};
            mapControl.Resize += delegate { mapControl.Refresh(); };
            mapControl.Dock = DockStyle.Fill;

            var newPointFeatureTool = new NewPointFeatureTool(l => true, "new test feature");
            mapControl.Tools.Add(newPointFeatureTool);

            var args = new MouseEventArgs(MouseButtons.Left, 1, -1, -1, -1);

            newPointFeatureTool.OnMouseDown(new Coordinate(0, 20), args);
            newPointFeatureTool.OnMouseMove(new Coordinate(0, 20), args);
            newPointFeatureTool.OnMouseUp(new Coordinate(0, 20), args);

            Assert.IsFalse(newPointFeatureTool.IsBusy);
            Assert.AreEqual(1, testFeatures.Count);
        }

        private class TestFeature : Feature, INameable
        {
            public string Name { get; set; }
        }
    }
}