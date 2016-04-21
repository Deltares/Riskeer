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
    /// <summary>
    /// Class responsible for loading and saving <see cref="StochasticSoilModel"/> objects from and into the database.
    /// </summary>
    public class StochasticSoilModelPersistor
    {
        private readonly StochasticSoilProfilePersistor soilProfilePersistor;
        private readonly StochasticSoilModelConverter soilModelConverter = new StochasticSoilModelConverter();

        private readonly ICollection<StochasticSoilModelEntity> modifiedList = new List<StochasticSoilModelEntity>();
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> insertedList = new Dictionary<StochasticSoilModelEntity, StochasticSoilModel>();
        private readonly DbSet<StochasticSoilModelEntity> stochasticSoilModelSet;

        /// <summary>
        /// New instance of <see cref="StochasticSoilModelPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public StochasticSoilModelPersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            stochasticSoilModelSet = ringtoetsContext.StochasticSoilModelEntities;
            soilProfilePersistor = new StochasticSoilProfilePersistor(ringtoetsContext);
        }

        /// <summary>
        /// Loads the <see cref="StochasticSoilModelEntity"/> as <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to load.</param>
        /// <returns>A new instance of <see cref="StochasticSoilModel"/>, based on the properties of <paramref name="entity"/>.</returns>
        public StochasticSoilModel LoadModel(StochasticSoilModelEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var stochasticSoilModel = soilModelConverter.ConvertEntityToModel(entity);

            LoadChildren(stochasticSoilModel, entity);

            return stochasticSoilModel;
        }

        /// <summary>
        /// Ensures that the model is added as <see cref="StochasticSoilModelEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="StochasticSoilModelEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="stochasticSoilModel"><see cref="StochasticSoilModel"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// </list></exception>
        public void InsertModel(ICollection<StochasticSoilModelEntity> parentNavigationProperty, StochasticSoilModel stochasticSoilModel)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }
            if (stochasticSoilModel == null)
            {
                return;
            }

            var entity = InsertStochasticSoilModel(parentNavigationProperty, stochasticSoilModel);

            InsertChildren(entity, stochasticSoilModel);
        }

        /// <summary>
        /// Ensures that the <paramref name="stochasticSoilModel"/> is set as <see cref="StochasticSoilModel"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="StochasticSoilModelEntity"/> objects can be searched and added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="stochasticSoilModel"><see cref="StochasticSoilModel"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// </list></exception>
        public void UpdateModel(ICollection<StochasticSoilModelEntity> parentNavigationProperty, StochasticSoilModel stochasticSoilModel)
        {
            if (stochasticSoilModel == null)
            {
                return;
            }

            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
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

                soilModelConverter.ConvertModelToEntity(stochasticSoilModel, entity);

                foreach (var soilProfile in stochasticSoilModel.StochasticSoilProfiles)
                {
                    soilProfilePersistor.UpdateModel(entity.StochasticSoilProfileEntities, soilProfile);
                }

                soilProfilePersistor.RemoveUnModifiedEntries(entity.StochasticSoilProfileEntities);
            }
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

            soilProfilePersistor.PerformPostSaveActions();
        }

        /// <summary>
        /// Remove the entries which were not modified by this <see cref="StochasticSoilModelPersistor"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">The collection to remove unmodified entries for.</param>
        public void RemoveUnModifiedEntries(IEnumerable<StochasticSoilModelEntity> parentNavigationProperty)
        {
            var untouchedModifiedList = parentNavigationProperty.Where(e => e.StochasticSoilModelEntityId > 0 && !modifiedList.Contains(e));
            stochasticSoilModelSet.RemoveRange(untouchedModifiedList);

            modifiedList.Clear();
        }

        private void LoadChildren(StochasticSoilModel stochasticSoilModel, StochasticSoilModelEntity entity)
        {
            foreach (var profileEntity in entity.StochasticSoilProfileEntities)
            {
                stochasticSoilModel.StochasticSoilProfiles.Add(soilProfilePersistor.LoadModel(profileEntity));
            }
        }

        private void InsertChildren(StochasticSoilModelEntity entity, StochasticSoilModel stochasticSoilModel)
        {
            foreach (var stochasticSoilProfile in stochasticSoilModel.StochasticSoilProfiles)
            {
                soilProfilePersistor.InsertModel(entity.StochasticSoilProfileEntities, stochasticSoilProfile);
            }
        }

        private StochasticSoilModelEntity InsertStochasticSoilModel(ICollection<StochasticSoilModelEntity> parentNavigationProperty, StochasticSoilModel stochasticSoilModel)
        {
            var entity = new StochasticSoilModelEntity();
            soilModelConverter.ConvertModelToEntity(stochasticSoilModel, entity);
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, stochasticSoilModel);
            return entity;
        }
    }
}