using System;
using System.Globalization;
using System.IO;
using BruTile;
using BruTile.Cache;
using BruTile.Tms;
using BruTile.Web;
using Core.Common.Utils;

namespace Core.GIS.SharpMap.Extensions.Layers
{
    // TODO: remove this class, use TileSource.Create()
    public class OpenStreetMapLayer : AsyncTileLayer
    {
        private static ITileCache<byte[]> omsCache;

        //a list of resolutions in which the tiles are stored
        private readonly double[] _resolutions = new[]
        {
            156543.033900000,
            78271.516950000,
            39135.758475000,
            19567.879237500,
            9783.939618750,
            4891.969809375,
            2445.984904688,
            1222.992452344,
            611.496226172,
            305.748113086,
            152.874056543,
            76.437028271,
            38.218514136,
            19.109257068,
            9.554628534,
            4.777314267,
            2.388657133,
            1.194328567,
            0.597164283
        };

        public static string CacheLocation
        {
            get
            {
                var path = SettingsHelper.GetApplicationLocalUserSettingsDirectory();
                return Path.Combine(path, "cache_open_street_map");
            }
        }

        protected override ITileCache<byte[]> GetOrCreateCache()
        {
            if (omsCache == null)
            {
                if (CacheLocation == null)
                {
                    omsCache = new MemoryCache<byte[]>(1000, 100000);
                }
                else
                {
                    var cacheDirectoryPath = CacheLocation;
                    omsCache = new FileCache(cacheDirectoryPath, "png");
                }
            }
            return omsCache;
        }

        protected override ITileSchema CreateTileSchema()
        {
            var schema = new TileSchema
            {
                Name = "OpenStreetMap",
                OriginX = -20037508.342789,
                OriginY = 20037508.342789,
                YAxis = YAxis.OSM,
                Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789),
                Height = 256,
                Width = 256,
                Format = "png",
                Srs = "EPSG:900913"
            };

            var i = 0;
            foreach (var resolution in _resolutions)
            {
                var levelId = i++.ToString(CultureInfo.InvariantCulture);
                schema.Resolutions[levelId] = new Resolution
                {
                    UnitsPerPixel = resolution, Id = levelId
                };
            }
            return schema;
        }

        protected override IRequest CreateRequest()
        {
            return new TmsRequest(new Uri("http://a.tile.openstreetmap.org"), "png");
        }
    }
}