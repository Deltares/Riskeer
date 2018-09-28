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

using Core.Components.Gis.Data;

namespace Core.Components.Gis.TestUtil
{
    /// <summary>
    /// Helper class to create <see cref="WmtsMapData"/> that can be used in testing.
    /// </summary>
    public static class WmtsMapDataTestHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/> configured to the 'brtachtergrondkaart'
        /// of PDOK in RD-new coordinate system.
        /// </summary>
        public static WmtsMapData CreateDefaultPdokMapData()
        {
            return new WmtsMapData("PDOK achtergrondkaart",
                                   "https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities",
                                   "brtachtergrondkaart(EPSG:28992)",
                                   "image/png");
        }

        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/> configured to the 'brtachtergrondkaart'
        /// of PDOK in a ETRS89 / UTM zone 31 N coordinate system.
        /// </summary>
        public static WmtsMapData CreateAlternativePdokMapData()
        {
            return new WmtsMapData("PDOK achtergrondkaart",
                                   "https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities",
                                   "brtachtergrondkaart(EPSG:25831:RWS)",
                                   "image/png");
        }

        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/> that hasn't been configured.
        /// </summary>
        public static WmtsMapData CreateUnconnectedMapData()
        {
            return new WmtsMapData("<niet bepaald>");
        }
    }
}