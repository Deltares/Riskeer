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
using Ringtoets.Common.Data.AssessmentSection;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extensions methods for <see cref="BackgroundData"/> related to 
    /// creating a <see cref="BackgroundMapDataEntity"/>.
    /// </summary>
    internal static class BackgroundDataCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="BackgroundMapDataEntity"/> based on the information of the
        /// <see cref="BackgroundData"/>.
        /// </summary>
        /// <param name="backgroundData">The container to create a <see cref="BackgroundMapDataEntity"/> for.</param>
        /// <returns>The entity, or <c>null</c> if <see cref="BackgroundData.BackgroundMapDataType"/> is not supported.</returns>
        internal static BackgroundMapDataEntity Create(this BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }
            
            if (backgroundData.BackgroundMapDataType == BackgroundMapDataType.Wmts)
            {
                return CreateWithWmtsData(backgroundData);
            }
            return null; // TODO: WTI-1141
        }

        private static BackgroundMapDataEntity CreateWithWmtsData(BackgroundData backgroundData)
        {
            var entity = new BackgroundMapDataEntity
            {
                Name = backgroundData.Name.DeepClone(),
                IsVisible = Convert.ToByte(backgroundData.IsVisible),
                Transparency = backgroundData.Transparency
            };

            if (backgroundData.IsConfigured)
            {
                entity.SelectedCapabilityName = backgroundData.Parameters[BackgroundDataIdentifiers.SelectedCapabilityIdentifier].DeepClone();
                entity.SourceCapabilitiesUrl = backgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl].DeepClone();
                entity.PreferredFormat = backgroundData.Parameters[BackgroundDataIdentifiers.PreferredFormat].DeepClone();
            }

            return entity;
        }
    }
}