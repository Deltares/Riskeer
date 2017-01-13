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
using System.Reflection;
using BruTile;
using BruTile.Web;

namespace Core.Components.DotSpatial.Layer.BruTile.Configurations
{
    /// <summary>
    /// Class with helper methods for retrieving data using reflection.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Configuration/ReflectionHelper.cs
    /// </remarks>
    internal static class BruTileReflectionHelper
    {
        /// <summary>
        /// Gets the <see cref="ITileProvider"/> for the given <see cref="ITileSource"/>.
        /// </summary>
        /// <param name="source">The tile source.</param>
        /// <returns>The tile provider.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> does
        /// not have the expected field to get the <see cref="ITileProvider"/> from.</exception>
        internal static ITileProvider GetProviderFromTileSource(ITileSource source)
        {
            FieldInfo fi = null;
            Type sourceType = source.GetType();
            if (sourceType == typeof(HttpTileSource))
            {
                fi = typeof(HttpTileSource).GetField("_provider", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            else if (sourceType == typeof(TileSource))
            {
                fi = typeof(TileSource).GetField("_provider", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if (fi == null)
            {
                throw new ArgumentException("Tile source does not have a private field '_provider'", nameof(source));
            }

            return (ITileProvider) fi.GetValue(source);
        }
    }
}