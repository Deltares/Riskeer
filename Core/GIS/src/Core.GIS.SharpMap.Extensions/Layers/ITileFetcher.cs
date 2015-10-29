using System;
using BruTile;

namespace Core.GIS.SharpMap.Extensions.Layers
{
    public interface ITileFetcher
    {
        byte[] FetchImageBytes(TileIndex index, Uri url);
    }
}