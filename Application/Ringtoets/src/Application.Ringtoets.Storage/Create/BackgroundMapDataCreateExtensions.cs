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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils.Extensions;
using Core.Components.Gis.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extensions methods for <see cref="AssessmentSection.BackgroundMapData"/> related to 
    /// creating a <see cref="BackgroundMapDataEntity"/>.
    /// </summary>
    internal static class BackgroundMapDataCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="BackgroundMapDataEntity"/> based on the information of the <see cref="WmtsMapData"/>
        /// </summary>
        /// <param name="mapData">The <see cref="WmtsMapData"/> to create a 
        /// <see cref="BackgroundMapDataEntity"/> for.</param>
        /// <returns>A configured <see cref="BackgroundMapDataEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapData"/>
        /// is <c>null</c>.</exception>
        internal static BackgroundMapDataEntity Create(this WmtsMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            var entity = new BackgroundMapDataEntity
            {
                Name = mapData.Name.DeepClone(),
                IsVisible = Convert.ToByte(mapData.IsVisible),
                Transparency = mapData.Transparency
            };

            if (mapData.IsConfigured)
            {
                entity.SelectedCapabilityName = mapData.SelectedCapabilityIdentifier.DeepClone();
                entity.SourceCapabilitiesUrl = mapData.SourceCapabilitiesUrl.DeepClone();
                entity.PreferredFormat = mapData.PreferredFormat.DeepClone();
            }

            return entity;
        }
    }
}