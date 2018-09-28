// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Amib.Threading;
using BruTile;
using BruTile.Cache;
using Core.Components.BruTile.Data;
using Core.Components.BruTile.IO.Properties;

namespace Core.Components.BruTile.IO
{
    /// <summary>
    /// Class responsible for fetching map tiles asynchronously from a <see cref="ITileProvider"/>.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/TileFetcher.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    public class AsyncTileFetcher : ITileFetcher
    {
        private readonly ConcurrentDictionary<TileIndex, int> activeTileRequests = new ConcurrentDictionary<TileIndex, int>();
        private readonly ConcurrentDictionary<TileIndex, int> openTileRequests = new ConcurrentDictionary<TileIndex, int>();
        private ITileProvider provider;
        private MemoryCache<byte[]> volatileCache;
        private ITileCache<byte[]> persistentCache;
        private SmartThreadPool threadPool;

        public event EventHandler<TileReceivedEventArgs> TileReceived;

        public event EventHandler QueueEmpty;

        /// <summary>
        /// Creates an instance of <see cref="AsyncTileFetcher"/>.
        /// </summary>
        /// <param name="provider">The tile provider.</param>
        /// <param name="minTiles">Minimum number of tiles in memory cache.</param>
        /// <param name="maxTiles">Maximum number of tiles in memory cache.</param>
        /// <param name="permaCache">Optional: the persistent cache. When null, no tiles
        /// will be cached outside of the volatile memory cache.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="provider"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when either <paramref name="minTiles"/>
        /// or <paramref name="maxTiles"/> is negative.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="minTiles"/>
        /// is not smaller than <paramref name="maxTiles"/>.</exception>
        public AsyncTileFetcher(ITileProvider provider, int minTiles, int maxTiles, ITileCache<byte[]> permaCache = null)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (minTiles < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minTiles), Resources.AsyncTileFetcher_Number_of_tiles_for_memory_cache_cannot_be_negative);
            }

            if (maxTiles < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTiles), Resources.AsyncTileFetcher_Number_of_tiles_for_memory_cache_cannot_be_negative);
            }

            if (minTiles >= maxTiles)
            {
                throw new ArgumentException(Resources.AsyncTileFetcher_Minimum_number_of_tiles_in_memory_cache_must_be_less_than_maximum);
            }

            this.provider = provider;
            volatileCache = new MemoryCache<byte[]>(minTiles, maxTiles);
            persistentCache = permaCache ?? NoopTileCache.Instance;
            threadPool = new SmartThreadPool(10000, BruTileSettings.MaximumNumberOfThreads);
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            ThrowExceptionIfDisposed();

            try
            {
                byte[] res = GetTileFromCache(tileInfo);
                if (res != null)
                {
                    return res;
                }
            }
            catch (IOException)
            {
                return null;
            }

            ScheduleTileRequest(tileInfo);
            return null;
        }

        public bool IsReady()
        {
            ThrowExceptionIfDisposed();

            return activeTileRequests.Count == 0 && openTileRequests.Count == 0;
        }

        public void DropAllPendingTileRequests()
        {
            ThrowExceptionIfDisposed();

            // Notes: http://dotspatial.codeplex.com/discussions/473428
            threadPool.Cancel(false);
            foreach (KeyValuePair<TileIndex, int> request in activeTileRequests.ToArray())
            {
                if (!openTileRequests.ContainsKey(request.Key))
                {
                    int dummy;
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                volatileCache.Clear();

                threadPool.Dispose();
                threadPool = null;

                volatileCache = null;
                provider = null;
                persistentCache = null;
            }

            IsDisposed = true;
        }

        private bool IsDisposed { get; set; }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> when <see cref="IsDisposed"/>
        /// is <c>true</c>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when calling this method while
        /// this instance is disposed.</exception>
        private void ThrowExceptionIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Gets the tile from the cache.
        /// </summary>
        /// <param name="tileInfo">The <see cref="TileInfo"/> to get the tile for.</param>
        /// <returns>An <see cref="Array"/> of <see cref="byte"/> which represent the tile.</returns>
        /// <exception cref="IOException">Thrown when an I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when this operation is not 
        /// supported on the current platform or the caller does not have the required permission.</exception>
        private byte[] GetTileFromCache(TileInfo tileInfo)
        {
            TileIndex index = tileInfo.Index;
            return volatileCache.Find(index) ?? persistentCache.Find(index);
        }

        private void ScheduleTileRequest(TileInfo tileInfo)
        {
            if (!HasTileAlreadyBeenRequested(tileInfo.Index))
            {
                activeTileRequests.TryAdd(tileInfo.Index, 1);
                var threadArguments = new object[]
                {
                    tileInfo
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
        /// for the file to be fetched.</param>
        private void GetTileOnThread(object[] parameters)
        {
            var tileInfo = (TileInfo) parameters[0];
            GetTileOnThreadCore(tileInfo);
        }

        private void GetTileOnThreadCore(TileInfo tileInfo)
        {
            if (!Thread.CurrentThread.IsAlive)
            {
                return;
            }

            byte[] result = TryRequestTileData(tileInfo);

            MarkTileRequestHandled(tileInfo);

            if (result != null)
            {
                volatileCache.Add(tileInfo.Index, result);
                persistentCache.Add(tileInfo.Index, result);

                OnTileReceived(new TileReceivedEventArgs(tileInfo, result));
            }
        }

        private byte[] TryRequestTileData(TileInfo tileInfo)
        {
            byte[] result = null;
            try
            {
                openTileRequests.TryAdd(tileInfo.Index, 1);
                result = provider.GetTile(tileInfo);
            }
            catch
            {
                // Nothing has to be done with the exception.
                // Result should stay null
            }

            //Try at least once again
            if (result == null)
            {
                try
                {
                    result = provider.GetTile(tileInfo);
                }
                catch
                {
                    // Nothing has to be done with the exception.
                    // Result should stay null
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

        private void OnTileReceived(TileReceivedEventArgs tileReceivedEventArgs)
        {
            TileReceived?.Invoke(this, tileReceivedEventArgs);

            if (IsReady())
            {
                OnQueueEmpty(EventArgs.Empty);
            }
        }

        private void OnQueueEmpty(EventArgs eventArgs)
        {
            QueueEmpty?.Invoke(this, eventArgs);
        }
    }
}