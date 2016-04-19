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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    public class StochasticSoilModelPersistor
    {
        private readonly StochasticSoilModelConverter converter;
        private readonly ICollection<StochasticSoilModelEntity> modifiedList = new List<StochasticSoilModelEntity>();
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> insertedList = new Dictionary<StochasticSoilModelEntity, StochasticSoilModel>();
        private readonly DbSet<StochasticSoilModelEntity> stochasticSoilModelSet;

        public StochasticSoilModelPersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            stochasticSoilModelSet = ringtoetsContext.StochasticSoilModelEntities;
            converter = new StochasticSoilModelConverter();
        }

        public IEnumerable<StochasticSoilModel> LoadModel(IEnumerable<StochasticSoilModelEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            return entities.Select(e => converter.ConvertEntityToModel(e));
        }

        public void InsertModel(ICollection<StochasticSoilModelEntity> parentNavigationProperty, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }
            if (stochasticSoilModels == null)
            {
                return;
            }

            foreach (var stochasticSoilModel in stochasticSoilModels)
            {
                InsertStochasticSoilModel(parentNavigationProperty, stochasticSoilModel);
            }
        }

        public void UpdateModel(ICollection<StochasticSoilModelEntity> parentNavigationProperty, IList<StochasticSoilModel> model)
        {
            if (model == null)
            {
                return;
            }

            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            foreach (var stochasticSoilModel in model)
            {
                if (stochasticSoilModel == null)
                {
                    throw new ArgumentException("A null StochasticSoilModel cannot be added");
                }

                if (stochasticSoilModel.StorageId < 1)
                {
                    InsertStochasticSoilModel(parentNavigationProperty, stochasticSoilModel);
                }
                else
                {
                    StochasticSoilModelEntity entity;
                    try
                    {
                        entity = parentNavigationProperty.SingleOrDefault(db => db.StochasticSoilModelEntityId == stochasticSoilModel.StorageId);
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "StochasticSoilModelEntity", stochasticSoilModel.StorageId), exception);
                    }

                    if (entity == null)
                    {
                        throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "StochasticSoilModelEntity", stochasticSoilModel.StorageId));
                    }

                    modifiedList.Add(entity);

                    converter.ConvertModelToEntity(stochasticSoilModel, entity);
                }
            }

            RemoveUnModifiedEntries(parentNavigationProperty);
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            foreach (var entry in insertedList)
            {
                entry.Value.StorageId = entry.Key.StochasticSoilModelEntityId;
            }
            insertedList.Clear();
        }

        private void InsertStochasticSoilModel(ICollection<StochasticSoilModelEntity> parentNavigationProperty, StochasticSoilModel stochasticSoilModel)
        {
            var entity = new StochasticSoilModelEntity();
            converter.ConvertModelToEntity(stochasticSoilModel, entity);
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, stochasticSoilModel);
        }

        private void RemoveUnModifiedEntries(IEnumerable<StochasticSoilModelEntity> parentNavigationProperty)
        {
            var untouchedModifiedList = parentNavigationProperty.Where(e => e.StochasticSoilModelEntityId > 0 && !modifiedList.Contains(e));
            stochasticSoilModelSet.RemoveRange(untouchedModifiedList);

            modifiedList.Clear();
        }
    }
}