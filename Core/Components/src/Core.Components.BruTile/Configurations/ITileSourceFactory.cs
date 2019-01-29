// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using BruTile;
using BruTile.Predefined;
using BruTile.Wmts;
using Core.Components.Gis.Exceptions;

namespace Core.Components.BruTile.Configurations
{
    /// <summary>
    /// Interface for objects responsible for creating <see cref="ITileSource"/> from a
    /// given URL.
    /// </summary>
    public interface ITileSourceFactory
    {
        /// <summary>
        /// Returns all tile sources based on the capabilities of a Web Map Tile Service.
        /// </summary>
        /// <param name="capabilitiesUrl">The URL to the 'GetCapabilities' part of the service.</param>
        /// <returns>The tile sources with <see cref="ITileSource.Schema"/> initialized
        /// with an instance of <see cref="WmtsTileSchema"/>.</returns>
        /// <exception cref="CannotFindTileSourceException">Thrown when unable to retrieve
        /// tile sources from the given service.</exception>
        IEnumerable<ITileSource> GetWmtsTileSources(string capabilitiesUrl);

        /// <summary>
        /// Returns the tile source for <paramref name="knownTileSource"/>.
        /// </summary>
        /// <param name="knownTileSource">The known tile service to get the tile source for.</param>
        /// <returns>The tile source for <paramref name="knownTileSource"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="knownTileSource"/> is not supported.</exception>
        ITileSource GetKnownTileSource(KnownTileSource knownTileSource);
    }
}