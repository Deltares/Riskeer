using System.Collections.Generic;
using BruTile;
using BruTile.Wmts;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.Gis.Data;

namespace Core.Components.DotSpatial.TestUtil
{
    public class TestTileSourceFactory : ITileSourceFactory
    {
        private readonly TestWmtsTileSource wmtsTileSource;

        public TestTileSourceFactory(WmtsMapData backgroundMapData)
        {
            if (backgroundMapData.IsConfigured)
            {
                wmtsTileSource = new TestWmtsTileSource(backgroundMapData);
            }
        }

        public IEnumerable<ITileSource> GetWmtsTileSources(string capabilitiesUrl)
        {
            if (wmtsTileSource != null)
            {
                yield return wmtsTileSource;
            }
        }
    }
}