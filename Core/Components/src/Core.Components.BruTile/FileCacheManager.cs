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
using System.Collections.Generic;
using System.Linq;
using BruTile.Cache;
using Core.Components.BruTile.Data;

namespace Core.Components.BruTile
{
    /// <summary>
    /// Class that manages the file caches.
    /// </summary>
    public class FileCacheManager
    {
        private static FileCacheManager instance;
        private readonly List<RegisteredFileCache> registeredFileCaches;

        private FileCacheManager()
        {
            registeredFileCaches = new List<RegisteredFileCache>();
        }

        /// <summary>
        /// The singleton instance of <see cref="FileCacheManager"/>.
        /// </summary>
        public static FileCacheManager Instance
        {
            get
            {
                return instance ?? (instance = new FileCacheManager());
            }
        }

        /// <summary>
        /// Gets the <see cref="FileCache"/> for a given <paramref name="cacheDirectoryPath"/>.
        /// </summary>
        /// <param name="cacheDirectoryPath">The path to get the file cache for.</param>        
        /// <returns>A <see cref="FileCache"/> for the given <paramref name="cacheDirectoryPath"/>.</returns>        
        /// <remarks>When a <see cref="FileCache"/> for the given <paramref name="cacheDirectoryPath"/>
        /// already exists, this one is returned. A new <see cref="FileCache"/> is created otherwise.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cacheDirectoryPath"/> is <c>null</c>.</exception>
        public FileCache GetfileChache(string cacheDirectoryPath)
        {
            if (cacheDirectoryPath == null)
            {
                throw new ArgumentNullException(nameof(cacheDirectoryPath));
            }

            foreach (RegisteredFileCache registeredFileCache in registeredFileCaches)
            {
                if (registeredFileCache.CacheDirectoryPath == cacheDirectoryPath)
                {
                    registeredFileCache.CallCount++;
                    return registeredFileCache.FileChache;
                }
            }

            var fileCache = new FileCache(cacheDirectoryPath, BruTileSettings.PersistentCacheFormat,
                                          TimeSpan.FromDays(BruTileSettings.PersistentCacheExpireInDays));
            registeredFileCaches.Add(new RegisteredFileCache(fileCache, cacheDirectoryPath));
            return fileCache;
        }

        /// <summary>
        /// Unsubscribes the file cache for the given <paramref name="fileCache"/>.
        /// </summary>
        /// <param name="fileCache">The <see cref="FileCache"/> to unsubscribe from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileCache"/>
        /// is <c>null</c>.</exception>
        public void UnsubscribeFileCache(FileCache fileCache)
        {
            if (fileCache == null)
            {
                throw new ArgumentNullException(nameof(fileCache));
            }

            var fileCachesToRemove = new List<RegisteredFileCache>();

            foreach (RegisteredFileCache registeredFileCache in registeredFileCaches)
            {
                if (registeredFileCache.FileChache.Equals(fileCache))
                {
                    registeredFileCache.CallCount--;

                    if (registeredFileCache.CallCount == 0)
                    {
                        fileCachesToRemove.Add(registeredFileCache);
                    }
                }
            }

            if (fileCachesToRemove.Any())
            {
                foreach (RegisteredFileCache fileCacheToRemove in fileCachesToRemove)
                {
                    registeredFileCaches.Remove(fileCacheToRemove);
                    fileCacheToRemove.Dispose();
                }

                fileCachesToRemove.Clear();
            }
        }

        private class RegisteredFileCache : IDisposable
        {
            public RegisteredFileCache(FileCache fileChache, string cacheDirectoryPath)
            {
                FileChache = fileChache;
                CacheDirectoryPath = cacheDirectoryPath;
                CallCount = 1;
            }

            public int CallCount { get; set; }

            public FileCache FileChache { get; private set; }

            public string CacheDirectoryPath { get; private set; }

            public void Dispose()
            {
                FileChache = null;
                CacheDirectoryPath = null;
            }
        }
    }
}