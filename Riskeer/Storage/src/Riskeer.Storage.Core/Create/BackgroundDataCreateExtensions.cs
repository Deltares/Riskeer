// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extensions methods for <see cref="BackgroundData"/> related to 
    /// creating a <see cref="BackgroundDataEntity"/>.
    /// </summary>
    internal static class BackgroundDataCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="BackgroundDataEntity"/> based on the information of the
        /// <see cref="BackgroundData"/>.
        /// </summary>
        /// <param name="backgroundData">The container to create a <see cref="BackgroundDataEntity"/> for.</param>
        /// <returns>The created <see cref="BackgroundDataEntity"/>.</returns>
        internal static BackgroundDataEntity Create(this BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            var entity = new BackgroundDataEntity
            {
                Name = backgroundData.Name.DeepClone(),
                IsVisible = Convert.ToByte(backgroundData.IsVisible),
                Transparency = backgroundData.Transparency
            };

            AddEntitiesForBackgroundDataMeta(backgroundData, entity);

            return entity;
        }

        private static void AddEntitiesForBackgroundDataMeta(BackgroundData backgroundData, BackgroundDataEntity entity)
        {
            var wmtsBackgroundDataConfiguration = backgroundData.Configuration as WmtsBackgroundDataConfiguration;
            var wellKnownBackgroundDataConfiguration = backgroundData.Configuration as WellKnownBackgroundDataConfiguration;

            if (wmtsBackgroundDataConfiguration != null)
            {
                entity.BackgroundDataType = Convert.ToByte(BackgroundDataType.Wmts);
                AddWmtsMetaEntities(wmtsBackgroundDataConfiguration, entity);
            }
            else if (wellKnownBackgroundDataConfiguration != null)
            {
                entity.BackgroundDataType = Convert.ToByte(BackgroundDataType.WellKnown);
                AddWellKnownTileSourceMetaEntities(wellKnownBackgroundDataConfiguration, entity);
            }
        }

        private static void AddWellKnownTileSourceMetaEntities(WellKnownBackgroundDataConfiguration wellKnownConfiguration, BackgroundDataEntity entity)
        {
            entity.BackgroundDataMetaEntities.Add(new BackgroundDataMetaEntity
            {
                Key = BackgroundDataIdentifiers.WellKnownTileSource.DeepClone(),
                Value = ((int) wellKnownConfiguration.WellKnownTileSource).ToString()
            });
        }

        private static void AddWmtsMetaEntities(WmtsBackgroundDataConfiguration wmtsBackgroundDataConfiguration, BackgroundDataEntity entity)
        {
            entity.BackgroundDataMetaEntities.Add(new BackgroundDataMetaEntity
            {
                Key = BackgroundDataIdentifiers.IsConfigured,
                Value = Convert.ToByte(wmtsBackgroundDataConfiguration.IsConfigured).ToString()
            });

            if (wmtsBackgroundDataConfiguration.IsConfigured)
            {
                entity.BackgroundDataMetaEntities.Add(new BackgroundDataMetaEntity
                {
                    Key = BackgroundDataIdentifiers.SelectedCapabilityIdentifier.DeepClone(),
                    Value = wmtsBackgroundDataConfiguration.SelectedCapabilityIdentifier.DeepClone()
                });
                entity.BackgroundDataMetaEntities.Add(new BackgroundDataMetaEntity
                {
                    Key = BackgroundDataIdentifiers.SourceCapabilitiesUrl.DeepClone(),
                    Value = wmtsBackgroundDataConfiguration.SourceCapabilitiesUrl.DeepClone()
                });
                entity.BackgroundDataMetaEntities.Add(new BackgroundDataMetaEntity
                {
                    Key = BackgroundDataIdentifiers.PreferredFormat.DeepClone(),
                    Value = wmtsBackgroundDataConfiguration.PreferredFormat.DeepClone()
                });
            }
        }
    }
}