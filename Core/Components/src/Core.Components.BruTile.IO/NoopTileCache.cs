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

using BruTile;
using BruTile.Cache;

namespace Core.Components.BruTile.IO
{
    /// <summary>
    /// A <see cref="ITileCache{T}"/> that does nothing.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/TileFetcher.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    internal class NoopTileCache : ITileCache<byte[]>
    {
        /// <summary>
        /// The singleton instance of <see cref="NoopTileCache"/>.
        /// </summary>
        public static readonly NoopTileCache Instance = new NoopTileCache();

        public void Add(TileIndex index, byte[] tile)
        {
            // Nothing to add.
        }

        public void Remove(TileIndex index)
        {
            // Nothing to remove.
        }

        public byte[] Find(TileIndex index)
        {
            return null;
        }
    }
}