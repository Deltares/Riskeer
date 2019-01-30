// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Piping.Primitives;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingSoilProfile"/> related to creating a <see cref="PipingSoilProfileEntity"/>.
    /// </summary>
    internal static class PipingSoilProfileCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingSoilProfileEntity"/> based on the information of the <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="profile">The profile to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="PipingSoilProfileEntity"/> or one from the <paramref name="registry"/>
        /// if it was created for the <see cref="profile"/> earlier.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static PipingSoilProfileEntity Create(this PipingSoilProfile profile,
                                                       PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(profile))
            {
                return registry.Get(profile);
            }

            var entity = new PipingSoilProfileEntity
            {
                Name = profile.Name.DeepClone(),
                Bottom = profile.Bottom.ToNaNAsNull(),
                SourceType = Convert.ToByte(profile.SoilProfileSourceType)
            };

            AddEntitiesForPipingSoilLayers(profile, entity);

            registry.Register(entity, profile);
            return entity;
        }

        private static void AddEntitiesForPipingSoilLayers(PipingSoilProfile profile,
                                                           PipingSoilProfileEntity entity)
        {
            var index = 0;
            foreach (PipingSoilLayer pipingSoilLayer in profile.Layers)
            {
                entity.PipingSoilLayerEntities.Add(pipingSoilLayer.Create(index++));
            }
        }
    }
}