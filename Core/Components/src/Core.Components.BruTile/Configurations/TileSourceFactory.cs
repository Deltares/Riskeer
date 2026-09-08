// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using BruTile;
using BruTile.Predefined;
using BruTile.Wmts;
using Core.Components.BruTile.Properties;
using Core.Components.Gis.Exceptions;

namespace Core.Components.BruTile.Configurations
{
    /// <summary>
    /// Class responsible for creating <see cref="ITileSource"/> instances for a given
    /// source.
    /// </summary>
    public class TileSourceFactory : ITileSourceFactory
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static ITileSourceFactory instance;

        /// <summary>
        /// Gets the singleton instance of <see cref="ITileSourceFactory"/>.
        /// </summary>
        /// <remarks>
        /// Also performs some one time setup logic (adding support for all security protocols).
        /// </remarks>
        public static ITileSourceFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TileSourceFactory();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public IEnumerable<ITileSource> GetWmtsTileSources(string capabilitiesUrl)
        {
            ITileSource[] wmtsTileSources = ParseWmtsTileSources(capabilitiesUrl).ToArray();
            if (wmtsTileSources.Any(ts => !(ts.Schema is WmtsTileSchema)))
            {
                throw new CannotFindTileSourceException(Resources.TileSourceFactory_GetWmtsTileSources_TileSource_without_WmtsTileSchema_error);
            }

            return wmtsTileSources;
        }

        public ITileSource GetKnownTileSource(KnownTileSource knownTileSource)
        {
            return KnownTileSources.Create(knownTileSource,
                                           configureHttpRequestMessage: ConfigureHttpRequestMessage);
        }

        /// <summary>
        /// Parses the capabilities XML provided by the WMTS.
        /// </summary>
        /// <param name="capabilitiesUrl">The WMTS URL.</param>
        /// <returns>The tile sources offered by the service.</returns>
        /// <exception cref="CannotFindTileSourceException">Thrown when unable to connect
        /// to the WMTS and parse the response.</exception>
        private static IEnumerable<ITileSource> ParseWmtsTileSources(string capabilitiesUrl)
        {
            try
            {
                using (Stream s = httpClient.GetStreamAsync(capabilitiesUrl).GetAwaiter().GetResult())
                {
                    return WmtsCapabilitiesParser.Parse(s,
                                                        BoundingBoxAxisOrderInterpretation.CRS,
                                                        ConfigureHttpRequestMessage);
                }
            }
            catch (Exception e)
            {
                string message = string.Format(Resources.TileSourceFactory_ParseWmtsTileSources_Cannot_connect_to_WMTS_0_,
                                               capabilitiesUrl);
                throw new CannotFindTileSourceException(message, e);
            }
        }

        private static void ConfigureHttpRequestMessage(HttpRequestMessage message)
        {
            message.Headers.UserAgent.ParseAdd("Riskeer");
        }
    }
}