using System;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Web.Wms;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Tests
{
    [TestFixture]
    public class TestWmsCapabilityParser
    {
        [Test]
        [Ignore("Reguires internet")]
        public void Test130()
        {
            Client c = new Client("http://wms.iter.dk/example_capabilities_1_3_0.xml");
            Assert.AreEqual(3, c.ServiceDescription.Keywords.Length);
            Assert.AreEqual("1.3.0", c.WmsVersion);
            Assert.AreEqual("http://hostname/path?", c.GetMapRequests[0].OnlineResource);
            Assert.AreEqual("image/gif", c.GetMapOutputFormats[0]);
            Assert.AreEqual(4, c.Layer.ChildLayers.Length);
        }

        [Test]
        [Ignore("Reguires internet")]
        public void TestDemisv111()
        {
            Client c = new Client("http://www2.demis.nl/mapserver/request.asp");
            Assert.AreEqual("Demis World Map", c.ServiceDescription.Title);
            Assert.AreEqual("1.1.1", c.WmsVersion);
            Assert.AreEqual("http://www2.demis.nl/wms/wms.asp?wms=WorldMap&", c.GetMapRequests[0].OnlineResource);
            Assert.AreEqual("image/png", c.GetMapOutputFormats[0]);
            Assert.AreEqual(20, c.Layer.ChildLayers.Length);
        }

        [Test]
        [Ignore("Reguires internet")]
        public void AddLayerOK()
        {
            WmsLayer layer = new WmsLayer("wms", "http://wms.iter.dk/example_capabilities_1_3_0.xml");
            layer.AddLayer("ROADS_1M");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [Ignore("Reguires internet")]
        public void AddLayerFail()
        {
            WmsLayer layer = new WmsLayer("wms", "http://wms.iter.dk/example_capabilities_1_3_0.xml");
            layer.AddLayer("NonExistingLayer");
        }
    }
}