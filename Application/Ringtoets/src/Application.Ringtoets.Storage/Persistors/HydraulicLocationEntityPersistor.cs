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
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// The persistor for <see cref="HydraulicLocationEntity"/>.
    /// </summary>
    public class HydraulicLocationEntityPersistor
    {
        private readonly IRingtoetsEntities ringtoetsContext;
        private readonly HydraulicLocationConverter converter;

        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> insertedList = new Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>();
        private readonly ICollection<HydraulicLocationEntity> modifiedList = new List<HydraulicLocationEntity>();

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationEntityPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public HydraulicLocationEntityPersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }

            this.ringtoetsContext = ringtoetsContext;

            converter = new HydraulicLocationConverter();
        }

        /// <summary>
        /// Loads the <see cref="HydraulicLocationEntity"/> as <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="entities">The <see cref="HydraulicLocationEntity"/> to load.</param>
        /// <returns>A new instance of <see cref="HydraulicBoundaryLocation"/>, based on the properties of <paramref name="entities"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities"/> is <c>null</c>.</exception>
        public IEnumerable<HydraulicBoundaryLocation> LoadModel(ICollection<HydraulicLocationEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            return converter.ConvertEntityToModel(entities);
        }

        /// <summary>
        /// Ensures that the <paramref name="model"/> is set as <see cref="HydraulicLocationEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="HydraulicBoundaryLocation"/> objects can be searched and added. 
        /// Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> to be saved in the storage.</param>
        /// <exception cref="OverflowException">Thrown when <paramref name="model.Location"/> cannot be converted.</exception>
        public void UpdateModel(ICollection<HydraulicLocationEntity> parentNavigationProperty, HydraulicBoundaryDatabase model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            foreach (var location in model.Locations)
            {
                if (location == null)
                {
                    throw new ArgumentException("Null location cannot be added");
                }

                if (location.StorageId < 1)
                {
                    InsertLocation(parentNavigationProperty, location);
                    continue;
                }

                if (parentNavigationProperty == null)
                {
                    throw new ArgumentNullException("parentNavigationProperty");
                }

                HydraulicLocationEntity entity;
                try
                {
                    entity = parentNavigationProperty.SingleOrDefault(db => db.HydraulicLocationEntityId == location.StorageId);
                }
                catch (InvalidOperationException exception)
                {
                    throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "HydraulicLocationEntity", location.StorageId), exception);
                }

                if (entity == null)
                {
                    throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "HydraulicLocationEntity", location.StorageId));
                }

                modifiedList.Add(entity);

                converter.ConvertModelToEntity(location, entity);
            }
        }

        /// <summary>
        /// Ensures that the <paramref name="hydraulicBoundaryDatabase"/> is added as <see cref="HydraulicLocationEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="HydraulicLocationEntity"/> objects can be added.
        ///  Usually, this collection is a navigation property of a <see cref="IDbSet{HydraulicLocationEntity}"/>.</param>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryLocation"/> to be saved in the storage.</param>
        /// <exception cref="OverflowException">Thrown when <paramref name="hydraulicBoundaryDatabase.Location"/> cannot be converted.</exception>
        public void InsertModel(ICollection<HydraulicLocationEntity> parentNavigationProperty, HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            if (hydraulicBoundaryDatabase == null)
            {
                return;
            }

            foreach (var location in hydraulicBoundaryDatabase.Locations)
            {
                InsertLocation(parentNavigationProperty, location);
            }
        }

        /// <summary>
        /// Removes all entities from <see cref="IRingtoetsEntities.ProjectEntities"/> that are not marked as 'updated'.
        /// </summary>
        /// <param name="parentNavigationProperty">List where <see cref="HydraulicLocationEntity"/> objects can be searched. 
        /// Usually, this collection is a navigation property of a <see cref="IDbSet{HydraulicLocationEntity}"/>.</param>
        public void RemoveUnModifiedEntries(ICollection<HydraulicLocationEntity> parentNavigationProperty)
        {
            var untouchedModifiedList = parentNavigationProperty.Where(e => e.HydraulicLocationEntityId > 0 && !modifiedList.Contains(e));
            ringtoetsContext.Set<HydraulicLocationEntity>().RemoveRange(untouchedModifiedList);

            modifiedList.Clear();
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            foreach (var entry in insertedList)
            {
                entry.Value.StorageId = entry.Key.HydraulicLocationEntityId;
            }
            insertedList.Clear();
        }

        private void InsertLocation(ICollection<HydraulicLocationEntity> parentNavigationProperty, HydraulicBoundaryLocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            var entity = new HydraulicLocationEntity();
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, location);

            converter.ConvertModelToEntity(location, entity);

            if (location.StorageId > 0)
            {
                modifiedList.Add(entity);
            }
        }
    }
}