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

using System;
using System.ComponentModel;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.AssessmentSection
{
    /// <summary>
    /// Extension class for <see cref="RingtoetsWellKnownTileSource"/>.
    /// </summary>
    public static class RingtoetsWellKnownTileSourceExtension
    {
        /// <summary>
        /// Gets the display name corresponding to <paramref name="ringtoetsWellKnownTileSource"/>.
        /// </summary>
        /// <param name="ringtoetsWellKnownTileSource">The <see cref="RingtoetsWellKnownTileSource"/> 
        /// to get the display name of.</param>
        /// <returns>The display name.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="ringtoetsWellKnownTileSource"/> 
        /// is not a valid value of <see cref="RingtoetsWellKnownTileSource"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="ringtoetsWellKnownTileSource"/>
        /// is not supported.</exception>
        public static string GetDisplayName(this RingtoetsWellKnownTileSource ringtoetsWellKnownTileSource)
        {
            if (!Enum.IsDefined(typeof(RingtoetsWellKnownTileSource), ringtoetsWellKnownTileSource))
            {
                throw new InvalidEnumArgumentException(nameof(ringtoetsWellKnownTileSource),
                                                       (int) ringtoetsWellKnownTileSource,
                                                       typeof(RingtoetsWellKnownTileSource));
            }

            switch (ringtoetsWellKnownTileSource)
            {
                case RingtoetsWellKnownTileSource.OpenStreetMap:
                    return Resources.RingtoetsWellKnownTileSource_OpenStreetMap_DisplayName;
                case RingtoetsWellKnownTileSource.BingAerial:
                    return Resources.RingtoetsWellKnownTileSource_BingAerial_DisplayName;
                case RingtoetsWellKnownTileSource.BingHybrid:
                    return Resources.RingtoetsWellKnownTileSource_BingHybrid_DisplayName;
                case RingtoetsWellKnownTileSource.BingRoads:
                    return Resources.RingtoetsWellKnownTileSource_BingRoads_DisplayName;
                case RingtoetsWellKnownTileSource.EsriWorldTopo:
                    return Resources.RingtoetsWellKnownTileSource_EsriWorldTopo_DisplayName;
                case RingtoetsWellKnownTileSource.EsriWorldShadedRelief:
                    return Resources.RingtoetsWellKnownTileSource_EsriWorldShadedRelief_DisplayName;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}