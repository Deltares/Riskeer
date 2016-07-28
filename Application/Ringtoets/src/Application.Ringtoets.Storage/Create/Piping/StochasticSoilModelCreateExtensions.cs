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

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.DbContext;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="StochasticSoilModel"/> related to creating database entities.
    /// </summary>
    internal static class StochasticSoilModelCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StochasticSoilModelEntity"/> based on the information of the <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="model">The model to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="StochasticSoilModelEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static StochasticSoilModelEntity Create(this StochasticSoilModel model, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (registry.Contains(model))
            {
                return registry.Get(model);
            }

            var entity = new StochasticSoilModelEntity
            {
                Name = model.Name,
                SegmentName = model.SegmentName,
                SegmentPoints = new Point2DBinaryConverter().ToBytes(model.Geometry)
            };

            AddEntitiesForStochasticSoilProfiles(model, registry, entity);

            registry.Register(entity, model);
            return entity;
        }

        private static void AddEntitiesForStochasticSoilProfiles(StochasticSoilModel model, PersistenceRegistry registry, StochasticSoilModelEntity entity)
        {
            foreach (var stochasticSoilProfile in model.StochasticSoilProfiles)
            {
                entity.StochasticSoilProfileEntities.Add(stochasticSoilProfile.Create(registry));
            }
        }
    }
}