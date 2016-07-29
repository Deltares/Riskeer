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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Update.Piping
{
    /// <summary>
    /// Extension methods for <see cref="StochasticSoilModel"/> related to updating a <see cref="StochasticSoilModelEntity"/>.
    /// </summary>
    internal static class StochasticSoilModelUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="StochasticSoilModelEntity"/> in the database based on the information of the 
        /// <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="model">The model to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="model"/>
        /// does not have a corresponding entity in the database.</exception>
        internal static void Update(this StochasticSoilModel model, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            StochasticSoilModelEntity entity = model.GetCorrespondingEntity(
                context.StochasticSoilModelEntities,
                o => o.StochasticSoilModelEntityId);

            entity.Name = model.Name;
            entity.SegmentName = model.SegmentName;

            UpdateStochasticSoilProfiles(model, entity, registry, context);
            UpdateSoilModelSegment(model, entity);

            registry.Register(entity, model);
        }

        private static void UpdateStochasticSoilProfiles(StochasticSoilModel model, StochasticSoilModelEntity entity, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            foreach (var stochasticSoilProfile in model.StochasticSoilProfiles)
            {
                if (stochasticSoilProfile.IsNew())
                {
                    entity.StochasticSoilProfileEntities.Add(stochasticSoilProfile.Create(registry));
                }
                else
                {
                    stochasticSoilProfile.Update(registry, context);
                }
            }
        }

        private static void UpdateSoilModelSegment(StochasticSoilModel model, StochasticSoilModelEntity entity)
        {
            byte[] currentSegmentPointData = new Point2DBinaryConverter().ToBytes(model.Geometry);
            if (!BinaryDataEqualityHelper.AreEqual(entity.SegmentPoints, currentSegmentPointData))
            {
                entity.SegmentPoints = currentSegmentPointData;
            }
        }
    }
}