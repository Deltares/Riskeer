// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Ringtoets.Common.Data.AssessmentSection
{
    /// <summary>
    /// All well known tile sources for which built-in support is provided.
    /// </summary>
    public enum RingtoetsWellKnownTileSource
    {
        /// <summary>
        /// OpenStreetMap.
        /// </summary>
        OpenStreetMap = 1,

        /// <summary>
        /// Microsoft Bing's satellite imagery. 
        /// </summary>
        BingAerial = 2,

        /// <summary>
        /// Microsoft Bing's satellite imagery highlighting roads and major landmarks.
        /// </summary>
        BingHybrid = 3,

        /// <summary>
        /// Microsoft Bing's map showing roads, buildings and geography.
        /// </summary>
        BingRoads = 4,

        /// <summary>
        /// Esri's topographic map showing boundaries, cities, water features, physiographic features, park, landmarks, transportation and buildings.
        /// </summary>
        EsriWorldTopo = 5,

        /// <summary>
        /// Esri's surface elevation as shaded relief.
        /// </summary>
        EsriWorldShadedRelief = 6
    }
}