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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// Extension methods for read operations for <see cref="BackgroundData"/>
    /// based on the <see cref="BackgroundDataEntity"/>.
    /// </summary>
    internal static class BackgroundDataEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="BackgroundDataEntity"/> and use the information
        /// to construct <see cref="BackgroundData"/>.
        /// </summary>
        /// <param name="entity">The <see cref="BackgroundDataEntity"/>
        /// to create <see cref="BackgroundData"/> for.</param>
        /// <returns>A new <see cref="BackgroundData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="entity"/> is <c>null</c>.</exception>
        internal static BackgroundData Read(this BackgroundDataEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            IBackgroundDataConfiguration configuration = null;

            if ((BackgroundDataType) entity.BackgroundDataType == BackgroundDataType.Wmts)
            {
                configuration = ReadWmtsConfiguration(entity.BackgroundDataMetaEntities);
            }
            else if ((BackgroundDataType) entity.BackgroundDataType == BackgroundDataType.WellKnown)
            {
                configuration = ReadWellKnownConfiguration(entity.BackgroundDataMetaEntities);
            }

            var backgroundData = new BackgroundData(configuration)
            {
                IsVisible = Convert.ToBoolean(entity.IsVisible),
                Transparency = (RoundedDouble) entity.Transparency,
                Name = entity.Name
            };

            return backgroundData;
        }

        private static IBackgroundDataConfiguration ReadWmtsConfiguration(IEnumerable<BackgroundDataMetaEntity> backgroundDataMetaEntities)
        {
            string sourceCapabilitiesUrl = null;
            string selectedCapabilityIdentifier = null;
            string preferredFormat = null;

            bool isConfigured = Convert.ToBoolean(Convert.ToInt16(backgroundDataMetaEntities
                                                                  .Single(entity => entity.Key.Equals(BackgroundDataIdentifiers.IsConfigured))
                                                                  .Read().Value));

            if (isConfigured)
            {
                foreach (BackgroundDataMetaEntity backgroundDataMetaEntity in backgroundDataMetaEntities)
                {
                    if (backgroundDataMetaEntity.Key.Equals(BackgroundDataIdentifiers.SourceCapabilitiesUrl))
                    {
                        sourceCapabilitiesUrl = backgroundDataMetaEntity.Read().Value;
                    }

                    if (backgroundDataMetaEntity.Key.Equals(BackgroundDataIdentifiers.SelectedCapabilityIdentifier))
                    {
                        selectedCapabilityIdentifier = backgroundDataMetaEntity.Read().Value;
                    }

                    if (backgroundDataMetaEntity.Key.Equals(BackgroundDataIdentifiers.PreferredFormat))
                    {
                        preferredFormat = backgroundDataMetaEntity.Read().Value;
                    }
                }
            }

            return new WmtsBackgroundDataConfiguration(isConfigured,
                                                       sourceCapabilitiesUrl,
                                                       selectedCapabilityIdentifier,
                                                       preferredFormat);
        }

        private static IBackgroundDataConfiguration ReadWellKnownConfiguration(IEnumerable<BackgroundDataMetaEntity> backgroundDataMetaEntities)
        {
            KeyValuePair<string, string> parameter =
                backgroundDataMetaEntities.Single(metaEntity => metaEntity.Key.Equals(BackgroundDataIdentifiers.WellKnownTileSource))
                                          .Read();

            var wellKnownTileSource = (RingtoetsWellKnownTileSource) Convert.ToInt32(parameter.Value);
            return new WellKnownBackgroundDataConfiguration(wellKnownTileSource);
        }
    }
}