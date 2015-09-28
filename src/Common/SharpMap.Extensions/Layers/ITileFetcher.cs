﻿using System;
using BruTile;

namespace SharpMap.Extensions.Layers
{
    public interface ITileFetcher
    {
        byte[] FetchImageBytes(TileIndex index, Uri url);
    }
}