// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using System.ComponentModel;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.AssessmentSection
{
    /// <summary>
    /// Extension class for <see cref="RiskeerWellKnownTileSource"/>.
    /// </summary>
    public static class RiskeerWellKnownTileSourceExtension
    {
        /// <summary>
        /// Gets the display name corresponding to <paramref name="riskeerWellKnownTileSource"/>.
        /// </summary>
        /// <param name="riskeerWellKnownTileSource">The <see cref="RiskeerWellKnownTileSource"/> 
        /// to get the display name of.</param>
        /// <returns>The display name.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="riskeerWellKnownTileSource"/> 
        /// is not a valid value of <see cref="RiskeerWellKnownTileSource"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="riskeerWellKnownTileSource"/>
        /// is not supported.</exception>
        public static string GetDisplayName(this RiskeerWellKnownTileSource riskeerWellKnownTileSource)
        {
            if (!Enum.IsDefined(typeof(RiskeerWellKnownTileSource), riskeerWellKnownTileSource))
            {
                throw new InvalidEnumArgumentException(nameof(riskeerWellKnownTileSource),
                                                       (int) riskeerWellKnownTileSource,
                                                       typeof(RiskeerWellKnownTileSource));
            }

            switch (riskeerWellKnownTileSource)
            {
                case RiskeerWellKnownTileSource.OpenStreetMap:
                    return Resources.RiskeerWellKnownTileSource_OpenStreetMap_DisplayName;
                case RiskeerWellKnownTileSource.BingAerial:
                    return Resources.RiskeerWellKnownTileSource_BingAerial_DisplayName;
                case RiskeerWellKnownTileSource.BingHybrid:
                    return Resources.RiskeerWellKnownTileSource_BingHybrid_DisplayName;
                case RiskeerWellKnownTileSource.BingRoads:
                    return Resources.RiskeerWellKnownTileSource_BingRoads_DisplayName;
                case RiskeerWellKnownTileSource.EsriWorldTopo:
                    return Resources.RiskeerWellKnownTileSource_EsriWorldTopo_DisplayName;
                case RiskeerWellKnownTileSource.EsriWorldShadedRelief:
                    return Resources.RiskeerWellKnownTileSource_EsriWorldShadedRelief_DisplayName;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}