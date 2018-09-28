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
using BruTile;
using Core.Components.BruTile.IO;
using Core.Components.Gis.Exceptions;

namespace Core.Components.BruTile.Configurations
{
    /// <summary>
    /// Interface for all classes that can configure a BruTile layer.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Configuration/IConfiguration.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    public interface IConfiguration : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="ITileSchema"/>.
        /// </summary>
        ITileSchema TileSchema { get; }

        /// <summary>
        /// Gets the <see cref="ITileFetcher"/>.
        /// </summary>
        ITileFetcher TileFetcher { get; }

        /// <summary>
        /// Gets a value indicating whether the configuration has been fully initialized
        /// or not.
        /// </summary>
        /// <remarks><see cref="Initialize"/> can be used to initialize the configuration
        /// if needed.</remarks>
        bool Initialized { get; }

        /// <summary>
        /// Gets a deep copy of the configuration.
        /// </summary>
        /// <returns>The cloned configuration.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when calling this method while
        /// this instance is disposed.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when creating the file
        /// cache failed.</exception>
        IConfiguration Clone();

        /// <summary>
        /// Properly initialize the configuration, making it ready for tile fetching.
        /// </summary>
        /// <exception cref="CannotFindTileSourceException">Thrown when the configured
        /// <see cref="ITileSource"/> cannot be found.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when the configured
        /// tile cache cannot be created.</exception>
        /// <exception cref="CannotReceiveTilesException">Thrown when <see cref="TileSource"/>
        /// doesn't allow for tiles to be received.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when calling this method while
        /// this instance is disposed.</exception>
        void Initialize();
    }
}