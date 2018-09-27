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
using Core.Components.BruTile.Configurations;
using Core.Components.Gis.Data;

namespace Core.Components.BruTile.TestUtil
{
    /// <summary>
    /// Configures <see cref="TileSourceFactory.Instance"/> to temporarily use a different
    /// instance.
    /// </summary>
    public sealed class UseCustomTileSourceFactoryConfig : IDisposable
    {
        private readonly ITileSourceFactory originalFactory;

        /// <summary>
        /// Creates a new instance of <see cref="UseCustomTileSourceFactoryConfig"/>.
        /// </summary>
        /// <param name="factory">The temporary factory to be used.</param>
        public UseCustomTileSourceFactoryConfig(ITileSourceFactory factory)
        {
            originalFactory = TileSourceFactory.Instance;
            TileSourceFactory.Instance = factory;
        }

        /// <summary>
        /// Creates a new instance of <see cref="UseCustomTileSourceFactoryConfig"/> that
        /// initializes test stubs to work for a <see cref="ImageBasedMapData"/> instance.
        /// </summary>
        /// <param name="backgroundMapData">The map data to work with.</param>
        public UseCustomTileSourceFactoryConfig(ImageBasedMapData backgroundMapData)
            : this(new TestTileSourceFactory(backgroundMapData)) {}

        /// <summary>
        /// Reverts the <see cref="TileSourceFactory.Instance"/> to the value
        /// it had at time of construction of the <see cref="TileSourceFactory"/>.
        /// </summary>
        public void Dispose()
        {
            TileSourceFactory.Instance = originalFactory;
        }
    }
}