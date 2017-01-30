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
using Core.Common.Base.Data;
using Core.Components.Gis.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// Extension methods for read operations for <see cref="AssessmentSection.BackgroundMapData"/>
    /// based on the <see cref="WmtsMapDataEntity"/>.
    /// </summary>
    internal static class WmtsMapDataEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="WmtsMapDataEntity"/> and use the information
        /// to construct a <see cref="WmtsMapData"/>.
        /// </summary>
        /// <param name="entity">The <see cref="WmtsMapDataEntity"/>
        /// to create <see cref="WmtsMapData"/> for.</param>
        /// <returns>A new <see cref="WmtsMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="entity"/> is <c>null</c>.</exception>
        internal static WmtsMapData Read(this WmtsMapDataEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new WmtsMapData(entity.Name,
                                          entity.SourceCapabilitiesUrl,
                                          entity.SelectedCapabilityName,
                                          entity.PreferredFormat)
            {
                IsVisible = Convert.ToBoolean(entity.IsVisible),
                Transparency = (RoundedDouble) entity.Transparency.ToNaNAsNull()
            };
        }
    }
}