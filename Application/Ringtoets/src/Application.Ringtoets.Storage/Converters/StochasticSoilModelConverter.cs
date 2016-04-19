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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Converters
{
    public class StochasticSoilModelConverter : IEntityConverter<StochasticSoilModel, StochasticSoilModelEntity> {
        public StochasticSoilModel ConvertEntityToModel(StochasticSoilModelEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var convertedModel = new StochasticSoilModel(-1, entity.Name, entity.SegmentName)
            {
                StorageId = entity.StochasticSoilModelEntityId
            };
            foreach (var profileEntity in entity.StochasticSoilProfileEntities)
            {
                var profile = new StochasticSoilProfile((double)profileEntity.Probability.Value, SoilProfileType.SoilProfile1D, -1);
                convertedModel.StochasticSoilProfiles.Add(profile);
            }
            return convertedModel;
        }

        public void ConvertModelToEntity(StochasticSoilModel modelObject, StochasticSoilModelEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.Name = modelObject.Name;
            entity.SegmentName = modelObject.SegmentName;
            entity.StochasticSoilModelEntityId = modelObject.StorageId;

            foreach (var stochasticProfile in modelObject.StochasticSoilProfiles)
            {
                var profile = new StochasticSoilProfileEntity
                {
                    Probability = Convert.ToDecimal(stochasticProfile.Probability)
                };
                entity.StochasticSoilProfileEntities.Add(profile);
            }
        }
    }
}