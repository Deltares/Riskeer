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

using System;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// Extension methods for read operations for <see cref="BackgroundData"/>
    /// based on the <see cref="BackgroundMapDataEntity"/>.
    /// </summary>
    internal static class BackgroundMapDataEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="BackgroundMapDataEntity"/> and use the information
        /// to construct <see cref="BackgroundData"/>.
        /// </summary>
        /// <param name="entity">The <see cref="BackgroundMapDataEntity"/>
        /// to create <see cref="BackgroundData"/> for.</param>
        /// <returns>A new <see cref="BackgroundData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="entity"/> is <c>null</c>.</exception>
        internal static BackgroundData Read(this BackgroundMapDataEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var backgroundData = new BackgroundData
            {
                IsVisible = Convert.ToBoolean(entity.IsVisible),
                Transparency = (RoundedDouble) entity.Transparency,
                Name = entity.Name
            };

            if (entity.SourceCapabilitiesUrl != null && entity.SelectedCapabilityName != null && entity.PreferredFormat != null)
            {
                backgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl] = entity.SourceCapabilitiesUrl;
                backgroundData.Parameters[BackgroundDataIdentifiers.SelectedCapabilityIdentifier] = entity.SelectedCapabilityName;
                backgroundData.Parameters[BackgroundDataIdentifiers.PreferredFormat] = entity.PreferredFormat;

                backgroundData.IsConfigured = true;
            }

            return backgroundData;
        }
    }
}