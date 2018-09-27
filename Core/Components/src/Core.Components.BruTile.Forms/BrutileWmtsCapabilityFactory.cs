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

using System.Collections.Generic;
using BruTile;
using BruTile.Wmts;
using Core.Components.BruTile.Configurations;
using Core.Components.Gis.Exceptions;
using Core.Components.Gis.Forms;

namespace Core.Components.BruTile.Forms
{
    /// <summary>
    /// Class responsible for creating <see cref="WmtsCapability"/> instances for a given
    /// source.
    /// </summary>
    public class BruTileWmtsCapabilityFactory : IWmtsCapabilityFactory
    {
        /// <summary>
        /// Returns all <see cref="WmtsCapability"/> based on the capabilities of a Web Map Tile Service.
        /// </summary>
        /// <param name="capabilitiesUrl">The URL to the 'GetCapabilities' part of the service.</param>
        /// <returns>The WMTS capabilities.</returns>
        /// <exception cref="CannotFindTileSourceException">Thrown when unable to retrieve
        /// tile sources from the given service.</exception>
        public IEnumerable<WmtsCapability> GetWmtsCapabilities(string capabilitiesUrl)
        {
            IEnumerable<ITileSource> tileSources = TileSourceFactory.Instance.GetWmtsTileSources(capabilitiesUrl);

            foreach (ITileSource tileSource in tileSources)
            {
                var wmtsTileSchema = (WmtsTileSchema) tileSource.Schema;
                yield return new WmtsCapability(wmtsTileSchema.Identifier, wmtsTileSchema.Format,
                                                tileSource.Name, wmtsTileSchema.Srs);
            }
        }
    }
}