// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Components.Gis.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Utils.TypeConverters;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="ImageBasedMapData"/> for data used in the map.
    /// </summary>
    public static class RingtoetsBackgroundMapDataFactory
    {
        /// <summary>
        /// Creates <see cref="ImageBasedMapData"/> from a <see cref="BackgroundData"/>.
        /// </summary>
        /// <param name="backgroundData">The <see cref="BackgroundData"/> to create
        /// the <see cref="ImageBasedMapData"/> for.</param>
        /// <returns>The created <see cref="ImageBasedMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="backgroundData"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="BackgroundMapDataType"/>
        /// is not valid.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="BackgroundData.Configuration"/>
        /// contains an invalid value for <see cref="WellKnownTileSource"/></exception>.
        public static ImageBasedMapData CreateBackgroundMapData(BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            return BackgroundDataConverter.ConvertFrom(backgroundData);
        }
    }
}