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
using BruTile;

namespace Core.Components.DotSpatial.Layer.BruTile.TileFetching
{
    /// <summary>
    /// Event arguments for denoting the event that a new tile has been received.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/TileReceivedEventArgs.cs
    /// </remarks>
    public class TileReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance of <see cref="TileReceivedEventArgs"/>.
        /// </summary>
        /// <param name="tileInfo">The received tile info object.</param>
        /// <param name="tile">The tile data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public TileReceivedEventArgs(TileInfo tileInfo, byte[] tile)
        {
            if (tileInfo == null)
            {
                throw new ArgumentNullException(nameof(tileInfo));
            }
            if (tile == null)
            {
                throw new ArgumentNullException(nameof(tile));
            }

            TileInfo = tileInfo;
            Tile = tile;
        }

        /// <summary>
        /// Gets the tile information of the received tile.
        /// </summary>
        public TileInfo TileInfo { get; }

        /// <summary>
        /// Gets the actual tile data.
        /// </summary>
        public byte[] Tile { get; }
    }
}