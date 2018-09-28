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
using System.Reflection;
using BruTile;
using BruTile.Web;

namespace Core.Components.BruTile.Configurations
{
    /// <summary>
    /// Class with helper methods for retrieving data using reflection.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Configuration/ReflectionHelper.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    internal static class BruTileReflectionHelper
    {
        /// <summary>
        /// Gets the <see cref="ITileProvider"/> for the given <see cref="ITileSource"/>.
        /// </summary>
        /// <param name="source">The tile source.</param>
        /// <returns>The tile provider.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="source"/> does
        /// not have the expected field to get the <see cref="ITileProvider"/> from.</exception>
        /// <exception cref="FieldAccessException">Thrown when caller does not have permission
        /// to access the expected field that holds the <see cref="ITileProvider"/> instance.</exception>
        internal static ITileProvider GetProviderFromTileSource(ITileSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            FieldInfo fi = null;
            // Note: This implementation respects inheritance. Cannot use 'source.GetType()'
            // as that only grant access to fields declared in that type. Therefore the _provider
            // field would be inaccessible if 'source' would be an extended type of those below.
            if (source is HttpTileSource)
            {
                fi = typeof(HttpTileSource).GetField("_provider", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            else if (source is TileSource)
            {
                fi = typeof(TileSource).GetField("_provider", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            if (fi == null)
            {
                throw new NotSupportedException($"Unable to find a {typeof(ITileProvider).Name} field for type {source.GetType().Name}.");
            }

            return (ITileProvider) fi.GetValue(source);
        }
    }
}