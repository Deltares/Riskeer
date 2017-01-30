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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BruTile;
using BruTile.Wmts;
using Core.Components.DotSpatial.Properties;

namespace Core.Components.DotSpatial.Layer.BruTile.Configurations
{
    /// <summary>
    /// Class responsible for creating <see cref="ITileSource"/> instances for a given
    /// source.
    /// </summary>
    public class TileSourceFactory : ITileSourceFactory
    {
        private static ITileSourceFactory instance;

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static ITileSourceFactory Instance
        {
            get
            {
                return instance ?? (instance = new TileSourceFactory());
            }
            set
            {
                instance = value;
            }
        }

        public IEnumerable<ITileSource> GetWmtsTileSources(string capabilitiesUrl)
        {
            ITileSource[] wmtsTileSources = ParseWmtsTileSources(capabilitiesUrl).ToArray();
            if(wmtsTileSources.Any(ts => !(ts.Schema is WmtsTileSchema)))
            {
                throw new CannotFindTileSourceException(Resources.TileSourceFactory_GetWmtsTileSources_TileSource_without_WmtsTileSchema_error);
            }
            return wmtsTileSources;
        }

        private static IEnumerable<ITileSource> ParseWmtsTileSources(string capabilitiesUrl)
        {
            try
            {
                WebRequest req = WebRequest.Create(capabilitiesUrl);
                using (WebResponse resp = req.GetResponse())
                {
                    using (Stream s = resp.GetResponseStream())
                    {
                        return WmtsParser.Parse(s);
                    }
                }
            }
            catch (Exception e)
            {
                string message = string.Format(Resources.TileSourceFactory_ParseWmtsTileSources_Cannot_connect_to_WMTS_0_,
                                               capabilitiesUrl);
                throw new CannotFindTileSourceException(message, e);
            }
        }
    }
}