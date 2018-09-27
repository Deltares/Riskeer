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

using BruTile.Cache;
using Core.Common.Util.Settings;

namespace Core.Components.BruTile.Data
{
    /// <summary>
    /// Class holding BruTile related settings.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/BruTileLayerSettings.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    public static class BruTileSettings
    {
        /// <summary>
        /// Gets a value indicating the maximum number of threads used for retrieving
        /// map tiles.
        /// </summary>
        public static int MaximumNumberOfThreads { get; } = 4;

        /// <summary>
        /// Gets a value indicating which format should be used to store the tiles.
        /// </summary>
        public static string PersistentCacheFormat { get; } = "png";

        /// <summary>
        /// Gets a value indicating how long tiles remain valid.
        /// </summary>
        public static int PersistentCacheExpireInDays { get; } = 14;

        /// <summary>
        /// Gets the <see cref="MemoryCache{T}.MinTiles"/> value
        /// </summary>
        public static int MemoryCacheMinimum { get; } = 100;

        /// <summary>
        /// Gets the <see cref="MemoryCache{T}.MaxTiles"/> value
        /// </summary>
        public static int MemoryCacheMaximum { get; } = 200;

        /// <summary>
        /// Gets the root directory path for the persistent cache.
        /// </summary>
        public static string PersistentCacheDirectoryRoot
        {
            get
            {
                return SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory("tilecaches");
            }
        }
    }
}