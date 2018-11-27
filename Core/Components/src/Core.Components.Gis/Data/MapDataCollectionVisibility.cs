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

using Core.Common.Util.Attributes;
using Core.Components.Gis.Properties;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Enumeration that defines the visibility of a <see cref="MapDataCollection"/>.
    /// </summary>
    public enum MapDataCollectionVisibility
    {
        /// <summary>
        /// Map data collection visible.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapDataCollectionVisibility_Visible_DisplayName))]
        Visible = 1,

        /// <summary>
        /// Map data collection not visible.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapDataCollectionVisibility_NotVisible_DisplayName))]
        NotVisible = 2,

        /// <summary>
        /// Map data collection partially visible.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapDataCollectionVisibility_Mixed_DisplayName))]
        Mixed = 3
    }
}