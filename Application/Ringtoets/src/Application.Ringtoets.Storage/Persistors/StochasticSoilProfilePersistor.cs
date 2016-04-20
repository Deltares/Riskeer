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
    public class StochasticSoilProfilePersistor
    {
        private readonly DbSet<StochasticSoilProfileEntity> stochasticSoilProfileSet;

        private readonly Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile> insertedList = new Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile>();
        private readonly ICollection<StochasticSoilProfileEntity> modifiedList = new List<StochasticSoilProfileEntity>();
        private readonly StochasticSoilProfileConverter stochasticSoilProfileConverter = new StochasticSoilProfileConverter();

        /// <summary>
        /// New instance of <see cref="StochasticSoilProfilePersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public StochasticSoilProfilePersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            stochasticSoilProfileSet = ringtoetsContext.StochasticSoilProfileEntities;
        }

        /// <summary>
        /// Loads the <see cref="StochasticSoilProfileEntity"/> as <see cref="StochasticSoilProfile"/>.
        /// </summary>
        /// <param name="entity">The <see cref="AssessmentSectionEntity"/> to load.</param>
        /// <returns>A new instance of <see cref="StochasticSoilProfileEntity"/>, based on the properties of <paramref name="entity"/>.</returns>
        public StochasticSoilProfile LoadModel(StochasticSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return stochasticSoilProfileConverter.ConvertEntityToModel(entity);
        }

        /// <summary>
        /// Ensures that the model is added as <see cref="StochasticSoilProfileEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="StochasticSoilProfileEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="stochasticSoilProfile"><see cref="StochasticSoilProfile"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// </list></exception>
        public void InsertModel(ICollection<StochasticSoilProfileEntity> parentNavigationProperty, StochasticSoilProfile stochasticSoilProfile)
        {
            if (stochasticSoilProfile == null)
            {
                return;
            }
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            InsertStochasticSoilProfile(parentNavigationProperty, stochasticSoilProfile);
        }

        /// <summary>
        /// Ensures that the <paramref name="stochasticSoilProfile"/> is set as <see cref="StochasticSoilProfile"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="StochasticSoilProfileEntity"/> objects can be searched and added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="stochasticSoilProfile"><see cref="StochasticSoilProfile"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// </list></exception>
        public void UpdateModel(ICollection<StochasticSoilProfileEntity> parentNavigationProperty, StochasticSoilProfile stochasticSoilProfile)
        {
            if (stochasticSoilProfile == null)
            {
                return;
            }
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            if (stochasticSoilProfile.StorageId < 1)
            {
                InsertStochasticSoilProfile(parentNavigationProperty, stochasticSoilProfile);
            }
            else
            {
                StochasticSoilProfileEntity entity;
                try
                {
                    entity = parentNavigationProperty.SingleOrDefault(db => db.StochasticSoilProfileEntityId == stochasticSoilProfile.StorageId);
                }
                catch (InvalidOperationException exception)
                {
                    throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "StochasticSoilProfileEntity", stochasticSoilProfile.StorageId), exception);
                }

                if (entity == null)
                {
                    throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "StochasticSoilProfileEntity", stochasticSoilProfile.StorageId));
                }

                modifiedList.Add(entity);

                stochasticSoilProfileConverter.ConvertModelToEntity(stochasticSoilProfile, entity);
            }
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            foreach (var entry in insertedList)
            {
                entry.Value.StorageId = entry.Key.StochasticSoilProfileEntityId;
            }
            insertedList.Clear();
        }

        private void InsertStochasticSoilProfile(ICollection<StochasticSoilProfileEntity> parentNavigationProperty, StochasticSoilProfile stochasticSoilProfile)
        {
            var entity = new StochasticSoilProfileEntity();
            stochasticSoilProfileConverter.ConvertModelToEntity(stochasticSoilProfile, entity);
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, stochasticSoilProfile);
        }

        public void RemoveUnModifiedEntries(IEnumerable<StochasticSoilProfileEntity> parentNavigationProperty)
        {
            var untouchedModifiedList = parentNavigationProperty.Where(e => e.StochasticSoilProfileEntityId > 0 && !modifiedList.Contains(e));
            stochasticSoilProfileSet.RemoveRange(untouchedModifiedList);

            modifiedList.Clear();
        }
    }
}