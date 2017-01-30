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
using BruTile.Cache;
using Core.Common.Utils;
using Core.Components.DotSpatial.Properties;

namespace Core.Components.DotSpatial.Layer.BruTile.Configurations
{
    /// <summary>
    /// Base class for a persistent cache configuration.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Configuration/CacheConfiguration.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    public abstract class PersistentCacheConfiguration
    {
        protected readonly string persistentCacheDirectoryPath;

        /// <summary>
        /// Initialized a new instance of <see cref="PersistentCacheConfiguration"/>.
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
            this.persistentCacheDirectoryPath = persistentCacheDirectoryPath;
        }

        /// <summary>
        /// Creates the tile cache.
        /// </summary>
        /// <returns>The file cache.</returns>
        /// <exception cref="CannotCreateTileCacheException">Thrown when a critical error
        /// occurs when creating the tile cache.</exception>
        protected IPersistentCache<byte[]> CreateTileCache()
        {
            if (!Directory.Exists(persistentCacheDirectoryPath))
            {
                try
                {
                    Directory.CreateDirectory(persistentCacheDirectoryPath);
                }
                catch (Exception e) when (SupportedCreateDirectoryExceptions(e))
                {
                    string message = Resources.PersistentCacheConfiguration_CreateTileCache_Critical_error_while_creating_tile_cache;
                    throw new CannotCreateTileCacheException(message, e);
                }
            }

            return new FileCache(persistentCacheDirectoryPath, BruTileSettings.PersistentCacheFormat,
                                 TimeSpan.FromDays(BruTileSettings.PersistentCacheExpireInDays));
        }

        private static bool SupportedCreateDirectoryExceptions(Exception exception)
        {
            return exception is IOException
                   || exception is UnauthorizedAccessException
                   || exception is ArgumentException
                   || exception is NotSupportedException;
        }
    }
}