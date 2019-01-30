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
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsStochasticSoilModel"/> related to creating
    /// a <see cref="StochasticSoilModelEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsStochasticSoilModelCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StochasticSoilModelEntity"/> based on the information of the 
        /// <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="model">The model to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="StochasticSoilModelEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static StochasticSoilModelEntity Create(this MacroStabilityInwardsStochasticSoilModel model,
                                                       PersistenceRegistry registry,
                                                       int order)
        {
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
                StochasticSoilModelSegmentPointXml = new Point2DCollectionXmlSerializer().ToXml(model.Geometry),
                Order = order
            };

            AddEntitiesForStochasticSoilProfiles(model, registry, entity);

            registry.Register(entity, model);
            return entity;
        }

        private static void AddEntitiesForStochasticSoilProfiles(MacroStabilityInwardsStochasticSoilModel model,
                                                                 PersistenceRegistry registry,
                                                                 StochasticSoilModelEntity entity)
        {
            var i = 0;
            foreach (MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile in model.StochasticSoilProfiles)
            {
                entity.MacroStabilityInwardsStochasticSoilProfileEntities.Add(stochasticSoilProfile.Create(registry, i++));
            }
        }
    }
}