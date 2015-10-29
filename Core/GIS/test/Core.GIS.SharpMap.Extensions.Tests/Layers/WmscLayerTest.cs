using System;
using System.Net;
using System.Text;
using BruTile.Extensions;
using BruTile.Wms;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Extensions.Tests.Layers
{
    [TestFixture]
    public class WmscLayerTest
    {
        [Test]
        public void GetCapabilities()
        {
            var uri = "http://geoservices.rijkswaterstaat.nl/actueel_hoogtebestand_nl?";
            uri += "REQUEST=GetCapabilities&SERVICE=WMS";

            var webRequest = (HttpWebRequest) WebRequest.Create(uri);

            using (var webResponse = webRequest.GetSyncResponse(5000))
            {
                if (webResponse == null)
                {
                    throw (new WebException("An error occurred while fetching tile", null));
                }

                using (var responseStream = webResponse.GetResponseStream())
                {
                    var capabilities = new WmsCapabilities(responseStream);

                    DumpLayers(capabilities.Capability.Layer, 0);

                    // dump XML
                    var bytes = BruTile.Utilities.ReadFully(responseStream);
                    var str = Encoding.UTF8.GetString(bytes);

                    Console.WriteLine(str);
                }
            }
        }

        private void DumpLayers(Layer layer, int depth)
        {
            var str = "";

            for (var i = 0; i < depth; i++)
            {
                str += " ";
            }

            str += layer.Name;

            Console.WriteLine(str);

            foreach (var childLayer in layer.ChildLayers)
            {
                DumpLayers(childLayer, depth + 1);
            }
        }
    }
}