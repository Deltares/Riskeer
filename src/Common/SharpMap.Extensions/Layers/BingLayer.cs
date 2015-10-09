using System;
using System.Collections.Generic;
using System.IO;
using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using DelftTools.Utils;

namespace SharpMap.Extensions.Layers
{
    // TODO: remove this class, use TileSource.Create()
    public class BingLayer : AsyncTileLayer
    {
        private static readonly IDictionary<string, ITileCache<byte[]>> bingCache = new Dictionary<string, ITileCache<byte[]>>();

        private ITileCache<byte[]> cache;

        private BingMapType? mapType;

        public virtual string CacheLocation
        {
            get
            {
                var path = SettingsHelper.GetApplicationLocalUserSettingsDirectory();
                return Path.Combine(path, "cache_bing_" + MapType.ToLower());
            }
        }

        public virtual string MapType { get; set; }

        public override object Clone()
        {
            var clone = (BingLayer) base.Clone();
            clone.MapType = MapType;
            return clone;
        }

        protected override ITileCache<byte[]> GetOrCreateCache()
        {
            if (cache == null)
            {
                if (string.IsNullOrEmpty(MapType))
                {
                    return null;
                }

                if (!bingCache.ContainsKey(MapType))
                {
                    //no cache so mem
                    if (CacheLocation == null)
                    {
                        bingCache[MapType] = new MemoryCache<byte[]>(1000, 100000);
                    }
                    else
                    {
                        bingCache[MapType] = new FileCache(CacheLocation, "jpg");
                    }
                }

                cache = bingCache[MapType];
            }

            return cache;
        }

        protected override ITileSchema CreateTileSchema()
        {
            return new BingSchema();
        }

        protected override IRequest CreateRequest()
        {
            if (mapType == null)
            {
                mapType = (BingMapType) Enum.Parse(typeof(BingMapType), MapType);
            }

            switch (mapType)
            {
                case BingMapType.Aerial:
                    return new BingRequest("http://t{s}.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g={apiversion}&token={userkey}", string.Empty, mapType.Value);
                case BingMapType.Hybrid:
                    return new BingRequest("http://t{s}.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g={apiversion}&token={userkey}", string.Empty, mapType.Value);
                case BingMapType.Roads:
                    return new BingRequest("http://t{s}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g={apiversion}&token={userkey}", string.Empty, mapType.Value);
            }

            return null;
        }
    }
}