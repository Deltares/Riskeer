// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Core.Components.Gis.Data;

namespace Core.Components.Gis.TestUtil
{
    /// <summary>
    /// Helper class to create <see cref="WmtsMapData"/> that can be used in testing.
    /// </summary>
    public static class WmtsMapDataTestHelper
    {
        /// <summary>
        /// Creates an instance of <see cref="WmtsMapData"/> configured to the "standaard(EPSG:28992)" map layer of the "BRT-A"
        /// tile service of PDOK.
        /// </summary>
        public static WmtsMapData CreateDefaultPdokMapData()
        {
            return new WmtsMapData("PDOK BRT-A",
                                   "https://service.pdok.nl/brt/achtergrondkaart/wmts/v2_0?request=getcapabilities&service=wmts",
                                   "standaard(EPSG:28992)",
                                   "image/png");
        }

        /// <summary>
        /// Creates an instance of <see cref="WmtsMapData"/> configured to the "grijs(EPSG:25831)" map layer of the "BRT-A"
        /// tile service of PDOK.
        /// </summary>
        public static WmtsMapData CreateAlternativePdokMapData()
        {
            return new WmtsMapData("PDOK BRT-A",
                                   "https://service.pdok.nl/brt/achtergrondkaart/wmts/v2_0?request=getcapabilities&service=wmts",
                                   "grijs(EPSG:25831)",
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