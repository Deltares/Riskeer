using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using BruTile;
using BruTile.Cache;
using BruTile.Extensions;
using BruTile.Predefined;
using BruTile.Web;
using BruTile.Wms;
using BruTile.Wmsc;
using DelftTools.Utils;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using SharpMap.Api.Layers;
using SharpMap.Layers;
using Layer = BruTile.Wms.Layer;

namespace SharpMap.Extensions.Layers
{
    /// <summary>
    /// TODO: incomplete
    /// </summary>
    public class WmscLayer : AsyncTileLayer
    {
        private ITileCache<byte[]> cache;

        private ITileSchema tileSchema;

        private static bool makeLayerVisible;

        private static ICoordinateTransformation geo2webTransformation;
        private IEnvelope schemaEnvelope;

        public WmscLayer()
        {
            TransparentColor = Color.White;
        }

        public virtual string Url { get; set; }

        public virtual string CacheLocation
        {
            get
            {
                var path = SettingsHelper.GetApplicationLocalUserSettingsDirectory();
                return Path.Combine(path,
                    "cache_wms_" + Url.Replace(':', '_').Replace('/', '_').Replace('&', '_').Replace('?', '_') + WmsLayer);
            }
        }

        public virtual string WmsLayer { get; set; }

        private static ICoordinateTransformation Geo2webTransformation
        {
            get
            {
                if (geo2webTransformation == null)
                {
                    var srcSrs = SharpMap.Map.CoordinateSystemFactory.CreateFromEPSG(4326);
                    var dstSrs = SharpMap.Map.CoordinateSystemFactory.CreateFromEPSG(3857);

                    geo2webTransformation = SharpMap.Map.CoordinateSystemFactory.CreateTransformation(srcSrs, dstSrs);
                }
                return geo2webTransformation;
            }
        }

        protected override ITileCache<byte[]> GetOrCreateCache()
        {
            if (cache == null)
            {
                //no cache so mem
                if (CacheLocation == null)
                    cache = new MemoryCache<byte[]>(1000, 100000);
                else
                    cache = new FileCache(CacheLocation, "jpg");
            }
            return cache;
        }

        protected override ITileSchema CreateTileSchema()
        {
            if (tileSchema == null)
            {
                tileSchema = new GlobalSphericalMercator
                {
                    Extent = new Extent(schemaEnvelope.MinX, schemaEnvelope.MinY, schemaEnvelope.MaxX, schemaEnvelope.MaxY)
                };
            }

            return tileSchema;
        }

        protected override IRequest CreateRequest()
        {
            return new WmscRequest(new Uri(Url), tileSchema, new List<string>(new[] { WmsLayer }), null, null, "1.3.0");
        }

        public override object Clone()
        {
            var clone = (WmscLayer)base.Clone();
            clone.schemaEnvelope = (IEnvelope) schemaEnvelope.Clone();
            clone.WmsLayer = WmsLayer;
            clone.Url = Url;
            return clone;
        }

        public static ILayer CreateWmsLayersFromUrl(string url)
        {
            url += "REQUEST=GetCapabilities&SERVICE=WMS";

            var webRequest = (HttpWebRequest) WebRequest.Create(url);

            using (var webResponse = webRequest.GetSyncResponse(10000))
            {
                if (webResponse == null)
                {
                    throw (new WebException("An error occurred while fetching tile", null));
                }

                using (var responseStream = webResponse.GetResponseStream())
                {
                    var capabilities = new WmsCapabilities(responseStream);

                    makeLayerVisible = true;

                    return capabilities.Capability.Layer.ChildLayers.Count > 0 
                        ? CreateChildLayers(url, capabilities.Capability.Layer.ChildLayers, new GroupLayer { Name = GetWmsLayerName(capabilities.Capability.Layer), ReadOnly = true }) 
                        : CreateWmsLayer(url, capabilities.Capability.Layer);
                }
            }
        }

        private static ILayer CreateChildLayers(string url, IEnumerable<Layer> childLayers, IGroupLayer parent)
        {
            foreach (var childWmsLayer in childLayers)
            {
                parent.Layers.Add(childWmsLayer.ChildLayers.Count > 0
                    ? CreateChildLayers(url, childWmsLayer.ChildLayers, new GroupLayer { Name = GetWmsLayerName(childWmsLayer), ReadOnly = true })
                    : CreateWmsLayer(url, childWmsLayer));
            }

            return parent;
        }

        private static WmscLayer CreateWmsLayer(string url, Layer wmsLayer)
        {
            var layer = new WmscLayer
            {
                Name = GetWmsLayerName(wmsLayer),
                Url = url,
                WmsLayer = wmsLayer.Name,
                Visible = makeLayerVisible,
                ReadOnly = true,
                schemaEnvelope = CreateEnvelope(wmsLayer) 
            };

            if (makeLayerVisible)
            {
                makeLayerVisible = false;
            }

            return layer;
        }

        private static IEnvelope CreateEnvelope(Layer wmsLayer)
        {
            var minX = wmsLayer.ExGeographicBoundingBox.WestBoundLongitude;
            var maxX = wmsLayer.ExGeographicBoundingBox.EastBoundLongitude;
            var minY = wmsLayer.ExGeographicBoundingBox.SouthBoundLatitude;
            var maxY = wmsLayer.ExGeographicBoundingBox.NorthBoundLatitude;

            var pointsGeo = new List<double[]> {new[] {minX, minY}, new[] {maxX, minY}, new[] {maxX, maxY}, new[] {minX, maxY}};
            var pointsWeb = Geo2webTransformation.MathTransform.TransformList(pointsGeo);

            var envelope = new Envelope();
            envelope.ExpandToInclude(pointsWeb[0][0], pointsWeb[0][1]);
            envelope.ExpandToInclude(pointsWeb[1][0], pointsWeb[1][1]);
            envelope.ExpandToInclude(pointsWeb[2][0], pointsWeb[2][1]);
            envelope.ExpandToInclude(pointsWeb[3][0], pointsWeb[3][1]);

            return envelope;
        }

        private static string GetWmsLayerName(Layer layer)
        {
            return string.IsNullOrEmpty(layer.Name) ? layer.Title : layer.Name;
        }
    }
}