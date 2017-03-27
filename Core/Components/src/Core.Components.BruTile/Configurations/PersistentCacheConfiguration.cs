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
using System.IO;
using BruTile;
using BruTile.Cache;
using Core.Common.Utils;
using Core.Components.BruTile.Data;
using Core.Components.BruTile.IO;
using Core.Components.BruTile.Properties;
using Core.Components.Gis.Exceptions;

namespace Core.Components.BruTile.Configurations
{
    /// <summary>
    /// Base class for a persistent cache configuration.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Configuration/CacheConfiguration.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    public abstract class PersistentCacheConfiguration : IConfiguration
    {
        protected readonly string PersistentCacheDirectoryPath;

        private FileCache fileCache;
        private ITileSource tileSource;

        /// <summary>
        /// Initializes a new instance of <see cref="PersistentCacheConfiguration"/>.
        /// </summary>
        /// <param name="persistentCacheDirectoryPath">The path to the directory for this
        /// cache to keep its data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="persistentCacheDirectoryPath"/>
        /// is invalid.</exception>
        protected PersistentCacheConfiguration(string persistentCacheDirectoryPath)
        {
            if (!IOUtils.IsValidFolderPath(persistentCacheDirectoryPath))
            {
                throw new ArgumentException(string.Format(Resources.PersistentCacheConfiguration_Invalid_path_for_persistent_cache,
                                                          persistentCacheDirectoryPath),
                                            nameof(persistentCacheDirectoryPath));
            }
            PersistentCacheDirectoryPath = persistentCacheDirectoryPath;
        }

        public ITileSchema TileSchema
        {
            get
            {
                return tileSource?.Schema;
            }
        }

        public ITileFetcher TileFetcher { get; private set; }

        public bool Initialized { get; protected set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract IConfiguration Clone();

        public abstract void Initialize();

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                TileFetcher?.Dispose();
            }

            if (fileCache != null)
            {
                FileCacheManager.Instance.UnsubscribeFileCache(fileCache);
                fileCache = null;
            }

            IsDisposed = true;
        }

        /// <summary>
        /// Initializes the configuration based on the given <see cref="ITileSource"/>.
        /// </summary>
        /// <param name="tileSource">The tile source to initialize for.</param>
        /// <exception cref="CannotCreateTileCacheException">Thrown when a critical error
        /// occurs when creating the tile cache.</exception>
        /// <exception cref="CannotReceiveTilesException">Thrown when <paramref name="tileSource"/>
        /// does not allow for tiles to be retrieved.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when calling this method while
        /// this instance is disposed.</exception>
        protected void InitializeFromTileSource(ITileSource tileSource)
        {
            ThrowExceptionIfDisposed();

            this.tileSource = tileSource;
            IPersistentCache<byte[]> tileCache = CreateTileCache();
            try
            {
                ITileProvider provider = BruTileReflectionHelper.GetProviderFromTileSource(tileSource);
                TileFetcher = new AsyncTileFetcher(provider,
                                                   BruTileSettings.MemoryCacheMinimum,
                                                   BruTileSettings.MemoryCacheMaximum,
                                                   tileCache);
            }
            catch (Exception e) when (e is NotSupportedException || e is ArgumentException)
            {
                throw new CannotReceiveTilesException(Resources.Configuration_InitializeFromTileSource_TileSource_does_not_allow_access_to_provider, e);
            }
            Initialized = true;
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> when <see cref="IsDisposed"/>
        /// is <c>true</c>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when calling this method while
        /// this instance is disposed.</exception>
        protected void ThrowExceptionIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Creates the tile cache.
        /// </summary>
        /// <returns>The file cache.</returns>
        /// <exception cref="CannotCreateTileCacheException">Thrown when a critical error
        /// occurs when creating the tile cache.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when calling this method while
        /// this instance is disposed.</exception>
        protected IPersistentCache<byte[]> CreateTileCache()
        {
            ThrowExceptionIfDisposed();

            if (!Directory.Exists(PersistentCacheDirectoryPath))
            {
                try
                {
                    Directory.CreateDirectory(PersistentCacheDirectoryPath);
                }
                catch (Exception e) when (SupportedCreateDirectoryExceptions(e))
                {
                    string message = Resources.PersistentCacheConfiguration_CreateTileCache_Critical_error_while_creating_tile_cache;
                    throw new CannotCreateTileCacheException(message, e);
                }
            }

            fileCache = FileCacheManager.Instance.GetfileChache(PersistentCacheDirectoryPath);
            return fileCache;
        }

        private bool IsDisposed { get; set; }

        private static bool SupportedCreateDirectoryExceptions(Exception exception)
        {
            return exception is IOException
                   || exception is UnauthorizedAccessException
                   || exception is ArgumentException
                   || exception is NotSupportedException;
        }

        ~PersistentCacheConfiguration()
        {
            Dispose(false);
        }
    }
}