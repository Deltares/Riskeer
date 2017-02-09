﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using BruTile;

namespace Core.Components.DotSpatial.Layer.BruTile.TileFetching
{
    /// <summary>
    /// Interface for object that can retrieve raw tile-image data based on a <see cref="TileInfo"/>.
    /// </summary>
    public interface ITileFetcher : IDisposable
    {
        /// <summary>
        /// Event raised when the tile fetcher has received a tile.
        /// </summary>
        /// <remarks>These events can be fired asynchronously.</remarks>
        event EventHandler<TileReceivedEventArgs> TileReceived;

        /// <summary>
        /// Event raised when the tile request queue is empty.
        /// </summary>
        /// <remarks>These events can be fired asynchronously.</remarks>
        event EventHandler QueueEmpty;

        /// <summary>
        /// Gets the tile from cache or queues a fetch request.
        /// </summary>
        /// <param name="tileInfo">The tile info.</param>
        /// <returns>The tile data if a match can be found in the cache, otherwise <c>null</c>
        /// is returned.</returns>
        /// <remarks>If no tile can be returned, you can use <see cref="TileReceived"/> and
        /// <see cref="QueueEmpty"/> events for handling tile retrieval once the queued
        /// request has been served.</remarks>
        byte[] GetTile(TileInfo tileInfo);

        /// <summary>
        /// Stops all pending tile requests.
        /// </summary>
        void DropAllPendingTileRequests();

        /// <summary>
        /// Determines if this instance is idle and has no tile requests unserved.
        /// </summary>
        bool IsReady();
    }
}