// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Concurrent;
using System.Threading;
using Amib.Threading;
using BruTile;
using BruTile.Cache;

namespace Core.Components.DotSpatial.Layer.BruTile.TileFetching
{
    /// <summary>
    /// Class responsible for fetching map tiles from a <see cref="ITileProvider"/>.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/TileFetcher.cs
    /// </remarks>
    public class TileFetcher : IDisposable
    {
        private readonly ConcurrentDictionary<TileIndex, int> activeTileRequests = new ConcurrentDictionary<TileIndex, int>();
        private readonly ConcurrentDictionary<TileIndex, int> openTileRequests = new ConcurrentDictionary<TileIndex, int>();
        private ITileProvider provider;
        private MemoryCache<byte[]> volatileCache;
        private ITileCache<byte[]> persistentCache;
        private SmartThreadPool threadPool;

        /// <summary>
        /// Event raised when <see cref="AsyncMode"/> is <c>true</c> and the tile fetcher
        /// has received a tile.
        /// </summary>
        public event EventHandler<TileReceivedEventArgs> TileReceived;

        /// <summary>
        /// Event raised when <see cref="AsyncMode"/> is <c>true</c> and the tile request queue is empty
        /// </summary>
        public event EventHandler QueueEmpty;

        /// <summary>
        /// Creates an instance of <see cref="TileFetcher"/>.
        /// </summary>
        /// <param name="provider">The tile provider.</param>
        /// <param name="minTiles">Minimum number of tiles in memory cache.</param>
        /// <param name="maxTiles">Maximum number of tiles in memory cache.</param>
        /// <param name="permaCache">Optional: the persistent cache.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="provider"/>
        /// is <c>null</c>.</exception>
        public TileFetcher(ITileProvider provider, int minTiles, int maxTiles, ITileCache<byte[]> permaCache)
            : this(provider, minTiles, maxTiles, BruTileSettings.MaximumNumberOfThreads, permaCache) {}

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="provider">The tile provider.</param>
        /// <param name="minTiles">Minimum number of tiles in memory cache.</param>
        /// <param name="maxTiles">Maximum number of tiles in memory cache.</param>
        /// <param name="maxNumberOfThreads">The maximum number of threads used to get tiles.</param>
        /// <param name="permaCache">Optional: the persistent cache.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="provider"/>
        /// is <c>null</c>.</exception>
        public TileFetcher(ITileProvider provider, int minTiles, int maxTiles, int maxNumberOfThreads, ITileCache<byte[]> permaCache = null)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            this.provider = provider;
            volatileCache = new MemoryCache<byte[]>(minTiles, maxTiles);
            persistentCache = permaCache ?? NoopTileCache.Instance;
            threadPool = new SmartThreadPool(10000, maxNumberOfThreads);
            AsyncMode = BruTileSettings.FetchTilesAsyncByDefault;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tile fetcher should work asynchronously
        /// (tile receive notifications will be broadcast using <see cref="TileReceived"/>
        /// and <see cref="QueueEmpty"/>) or not (An <see cref="AutoResetEvent"/> argument
        /// can be used to be notified on a successful tile-fetch).
        /// </summary>
        public bool AsyncMode { get; set; }

        /// <summary>
        /// Gets the tile from cache or queues a fetch request.
        /// </summary>
        /// <param name="tileInfo">The tile info.</param>
        /// <param name="tileRetrievedEvent">Optional: The event object that notifies when
        /// the tile request has been served. Only used when <see cref="AsyncMode"/>
        /// is <c>true</c>.</param>
        /// <returns>The tile data if a match can be found in the cache, otherwise <c>null</c>
        /// is returned.</returns>
        /// <remarks>If no tile can be returned, you can use <see cref="TileReceived"/> and
        /// <see cref="QueueEmpty"/> events for handling tile retrieval once the queued
        /// request has been served.</remarks>
        public byte[] GetTile(TileInfo tileInfo, AutoResetEvent tileRetrievedEvent = null)
        {
            byte[] res = GetTileFromCache(tileInfo);
            if (res != null)
            {
                return res;
            }

            ScheduleTileRequest(tileInfo, tileRetrievedEvent);
            return null;
        }

        /// <summary>
        /// Determines if this instance is idle and has no tile requests unserved.
        /// </summary>
        public bool IsReady()
        {
            return activeTileRequests.Count == 0 && openTileRequests.Count == 0;
        }

        /// <summary>
        /// Stops all pending tile requests.
        /// </summary>
        public void DropAllPendingTileRequests()
        {
            // Notes: http://dotspatial.codeplex.com/discussions/473428
            threadPool.Cancel(false);
            int dummy;
            foreach (var request in activeTileRequests.ToArray())
            {
                if (!openTileRequests.ContainsKey(request.Key))
                {
                    if (!activeTileRequests.TryRemove(request.Key, out dummy))
                    {
                        activeTileRequests.TryRemove(request.Key, out dummy);
                    }
                }
            }
            openTileRequests.Clear();
        }

        public void Dispose()
        {
            if (volatileCache == null)
            {
                return;
            }

            volatileCache.Clear();
            volatileCache = null;
            provider = null;
            persistentCache = null;

            threadPool.Dispose();
            threadPool = null;
        }

        private byte[] GetTileFromCache(TileInfo tileInfo)
        {
            TileIndex index = tileInfo.Index;
            return volatileCache.Find(index) ?? persistentCache.Find(index);
        }

        private void ScheduleTileRequest(TileInfo tileInfo, AutoResetEvent tileRetrievedEvent)
        {
            if (!HasTileAlreadyBeenRequested(tileInfo.Index))
            {
                activeTileRequests.TryAdd(tileInfo.Index, 1);
                object[] threadArguments = AsyncMode ?
                                               new object[]
                                               {
                                                   tileInfo
                                               } :
                                               new object[]
                                               {
                                                   tileInfo,
                                                   tileRetrievedEvent ?? new AutoResetEvent(false)
                                               };
                threadPool.QueueWorkItem(GetTileOnThread, threadArguments);
            }
        }

        private bool HasTileAlreadyBeenRequested(TileIndex tileIndex)
        {
            return activeTileRequests.ContainsKey(tileIndex) || openTileRequests.ContainsKey(tileIndex);
        }

        /// <summary>
        /// Method to actually get the tile from the <see cref="provider"/>.
        /// </summary>
        /// <param name="parameters">The thread parameters. The first argument is the <see cref="TileInfo"/>
        /// for the file to be fetched. A second argument should be provided when <see cref="AsyncMode"/>
        /// is false, which should be an <see cref="AutoResetEvent"/>.</param>
        private void GetTileOnThread(object[] parameters)
        {
            var tileInfo = (TileInfo) parameters[0];
            var tileRetrievedEvent = !AsyncMode ? (AutoResetEvent) parameters[1] : null;

            GetTileOnThreadCore(tileInfo, tileRetrievedEvent);
        }

        private void GetTileOnThreadCore(TileInfo tileInfo, AutoResetEvent tileRetrievedEvent)
        {
            if (!Thread.CurrentThread.IsAlive)
            {
                return;
            }
            byte[] result = TryRequestTileData(tileInfo, tileRetrievedEvent);

            MarkTileRequestHandled(tileInfo);

            if (result != null)
            {
                volatileCache.Add(tileInfo.Index, result);
                persistentCache.Add(tileInfo.Index, result);

                NotifyReceivedTile(tileInfo, tileRetrievedEvent, result);
            }
        }

        private byte[] TryRequestTileData(TileInfo tileInfo, AutoResetEvent autoResetEvent)
        {
            byte[] result = null;
            try
            {
                openTileRequests.TryAdd(tileInfo.Index, 1);
                result = provider.GetTile(tileInfo);
            }
            catch {}

            //Try at least once again
            if (result == null)
            {
                try
                {
                    result = provider.GetTile(tileInfo);
                }
                catch
                {
                    if (!AsyncMode)
                    {
                        autoResetEvent.Set();
                    }
                }
            }
            return result;
        }

        private void MarkTileRequestHandled(TileInfo tileInfo)
        {
            int dummy;
            if (!activeTileRequests.TryRemove(tileInfo.Index, out dummy))
            {
                //try again
                activeTileRequests.TryRemove(tileInfo.Index, out dummy);
            }
            if (!openTileRequests.TryRemove(tileInfo.Index, out dummy))
            {
                //try again
                openTileRequests.TryRemove(tileInfo.Index, out dummy);
            }
        }

        private void NotifyReceivedTile(TileInfo tileInfo, AutoResetEvent tileRetrievedEvent, byte[] result)
        {
            if (AsyncMode)
            {
                OnTileReceived(new TileReceivedEventArgs(tileInfo, result));
            }
            else
            {
                tileRetrievedEvent.Set();
            }
        }

        private void OnTileReceived(TileReceivedEventArgs tileReceivedEventArgs)
        {
            // Only raise events if we are in async mode!
            if (!AsyncMode)
            {
                return;
            }

            TileReceived?.Invoke(this, tileReceivedEventArgs);

            if (IsReady())
            {
                OnQueueEmpty(EventArgs.Empty);
            }
        }

        private void OnQueueEmpty(EventArgs eventArgs)
        {
            // Only raise events if we are in async mode!
            if (!AsyncMode)
            {
                return;
            }

            QueueEmpty?.Invoke(this, eventArgs);
        }
    }
}