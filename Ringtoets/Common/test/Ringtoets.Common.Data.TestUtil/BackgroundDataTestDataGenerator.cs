﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.ComponentModel;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Class responsible for generating test data of <see cref="BackgroundData"/>.
    /// </summary>
    public static class BackgroundDataTestDataGenerator
    {
        /// <summary>
        /// Gets the <see cref="BackgroundData"/> with the given data of <see cref="WmtsMapData"/>.
        /// </summary>
        /// <param name="wmtsMapData">The <see cref="WmtsMapData"/> to create the background data for.</param>
        /// <returns>The created <see cref="BackgroundData"/>.</returns>
        public static BackgroundData GetWmtsBackgroundMapData(WmtsMapData wmtsMapData)
        {
            var backgroundMapData = new BackgroundData(new WmtsBackgroundDataConfiguration(wmtsMapData.IsConfigured,
                                                                                           wmtsMapData.SourceCapabilitiesUrl,
                                                                                           wmtsMapData.SelectedCapabilityIdentifier,
                                                                                           wmtsMapData.PreferredFormat))
            {
                Name = wmtsMapData.Name,
                IsVisible = wmtsMapData.IsVisible,
                Transparency = wmtsMapData.Transparency
            };

            return backgroundMapData;
        }

        /// <summary>
        /// Gets the <see cref="BackgroundData"/> with <see cref="WellKnownTileSource"/>.
        /// </summary>
        /// <param name="tileSource">The <see cref="WellKnownTileSource"/> to create the background data for.</param>
        /// <returns>The created <see cref="BackgroundData"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tileSource"/>
        /// is an invalid <see cref="WellKnownTileSource"/>.</exception>
        public static BackgroundData GetWellKnownBackgroundMapData(WellKnownTileSource tileSource)
        {
            return new BackgroundData(new WellKnownBackgroundDataConfiguration(tileSource))
            {
                IsVisible = true,
                Name = TypeUtils.GetDisplayName(tileSource)
            };
        }
    }
}