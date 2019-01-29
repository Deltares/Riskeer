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

using System.ComponentModel;
using Core.Common.Util.Reflection;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Class for representing an map tile source that for which that is built-in support.
    /// </summary>
    public class WellKnownTileSourceMapData : ImageBasedMapData
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WellKnownTileSourceMapData"/>.
        /// </summary>
        /// <param name="tileSource">The tile source.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tileSource"/>
        /// isn't a member of <see cref="WellKnownTileSource"/>.</exception>
        public WellKnownTileSourceMapData(WellKnownTileSource tileSource) : base(GetTileSourceName(tileSource))
        {
            TileSource = tileSource;
            IsConfigured = true;
        }

        /// <summary>
        /// Gets or sets the tile source of the map data.
        /// </summary>
        public WellKnownTileSource TileSource { get; private set; }

        /// <summary>
        /// Sets the tile source of the map data.
        /// </summary>
        /// <param name="tileSource">The <see cref="WellKnownTileSource"/> to set.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tileSource"/>
        /// isn't a member of <see cref="WellKnownTileSource"/>.</exception>
        public void SetTileSource(WellKnownTileSource tileSource)
        {
            Name = GetTileSourceName(tileSource);
            TileSource = tileSource;
        }

        /// <summary>
        /// Gets the name of the given tile source.
        /// </summary>
        /// <param name="tileSource">The tile source to get the name from.</param>
        /// <returns>The display name of the enum value.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tileSource"/>
        /// isn't a member of <see cref="WellKnownTileSource"/>.</exception> 
        private static string GetTileSourceName(WellKnownTileSource tileSource)
        {
            return TypeUtils.GetDisplayName(tileSource);
        }
    }
}