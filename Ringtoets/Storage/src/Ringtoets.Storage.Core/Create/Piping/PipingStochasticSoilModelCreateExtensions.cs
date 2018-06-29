// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util.Extensions;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingStochasticSoilModel"/> related to creating
    /// a <see cref="StochasticSoilModelEntity"/>.
    /// </summary>
    internal static class PipingStochasticSoilModelCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StochasticSoilModelEntity"/> based on the information of the <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <param name="model">The model to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="StochasticSoilModelEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static StochasticSoilModelEntity Create(this PipingStochasticSoilModel model,
                                                         PersistenceRegistry registry,
                                                         int order)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(model))
            {
                return registry.Get(model);
            }

            var entity = new StochasticSoilModelEntity
            {
                Name = model.Name.DeepClone(),
                StochasticSoilModelSegmentPointXml = new Point2DXmlSerializer().ToXml(model.Geometry),
                Order = order
            };

            AddEntitiesForStochasticSoilProfiles(model, registry, entity);

            registry.Register(entity, model);
            return entity;
        }

        private static void AddEntitiesForStochasticSoilProfiles(PipingStochasticSoilModel model,
                                                                 PersistenceRegistry registry,
                                                                 StochasticSoilModelEntity entity)
        {
            var i = 0;
            foreach (PipingStochasticSoilProfile stochasticSoilProfile in model.StochasticSoilProfiles)
            {
                entity.PipingStochasticSoilProfileEntities.Add(stochasticSoilProfile.Create(registry, i++));
            }
        }
    }
}