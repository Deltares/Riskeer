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

using Core.Common.Util.Attributes;
using Core.Components.Gis.Properties;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// All tile sources for which built-in support is provided.
    /// </summary>
    public enum WellKnownTileSource
    {
        /// <summary>
        /// OpenStreetMap.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.OpenStreetMap_DisplayName))]
        OpenStreetMap = 1,

        /// <summary>
        /// Microsoft Bing's satellite imagery. 
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BingAerial_DisplayName))]
        BingAerial = 2,

        /// <summary>
        /// Microsoft Bing's satellite imagery highlighting roads and major landmarks.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BingHybrid_DisplayName))]
        BingHybrid = 3,

        /// <summary>
        /// Microsoft Bing's map showing roads, buildings and geography.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BingRoads_DisplayName))]
        BingRoads = 4,

        /// <summary>
        /// Esri's topographic map showing boundaries, cities, water features, physiographic features, park, landmarks, transportation and buildings.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.EsriWorldTopo_DisplayName))]
        EsriWorldTopo = 5,

        /// <summary>
        /// Esri's surface elevation as shaded relief.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.EsriWorldShadedRelief_DisplayName))]
        EsriWorldShadedRelief = 6
    }
}