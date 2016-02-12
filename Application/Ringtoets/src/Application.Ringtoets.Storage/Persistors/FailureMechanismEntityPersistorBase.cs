﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for classes derived from <see cref="BaseFailureMechanism"/>.
    /// </summary>
    public abstract class FailureMechanismEntityPersistorBase
    {
        protected readonly IRingtoetsEntities dbContext;
        private readonly Dictionary<FailureMechanismEntity, IFailureMechanism> insertedList = new Dictionary<FailureMechanismEntity, IFailureMechanism>();
        private readonly ICollection<FailureMechanismEntity> modifiedList = new List<FailureMechanismEntity>();

        /// <summary>
        /// New instance of <see cref="FailureMechanismEntityPersistorBase"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        protected FailureMechanismEntityPersistorBase(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            dbContext = ringtoetsContext;
        }

        /// <summary>
        /// All unmodified <see cref="FailureMechanismEntity"/> in <paramref name="parentNavigationProperty"/> will be removed.
        /// </summary>
        /// <param name="parentNavigationProperty">List where <see cref="FailureMechanismEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        public void RemoveUnModifiedEntries(ICollection<FailureMechanismEntity> parentNavigationProperty)
        {
            var originalList = parentNavigationProperty.ToList();
            foreach (var u in modifiedList)
            {
                originalList.Remove(u);
            }

            foreach (var toDelete in originalList)
            {
                // If id = 0, the entity is marked as inserted
                if (toDelete.FailureMechanismEntityId > 0)
                {
                    dbContext.FailureMechanismEntities.Remove(toDelete);
                }
            }

            modifiedList.Clear();
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            foreach (var entry in insertedList)
            {
                entry.Value.StorageId = entry.Key.FailureMechanismEntityId;
            }
            insertedList.Clear();
        }

        /// <summary>
        /// Ensures that the model is added as <see cref="FailureMechanismEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="FailureMechanismEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="IFailureMechanism"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        protected void InsertModel(ICollection<FailureMechanismEntity> parentNavigationProperty, IFailureMechanism model)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            var entity = new FailureMechanismEntity();
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, model);

            ConvertModelToEntity(model, entity);

            if (model.StorageId > 0)
            {
                modifiedList.Add(entity);
            }
        }

        /// <summary>
        /// Ensures that the <paramref name="model"/> is set as <see cref="FailureMechanismEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="FailureMechanismEntity"/> objects can be searched and added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="IFailureMechanism"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        /// <exception cref="EntityNotFoundException">Thrown when the storageId of <paramref name="model"/> &gt; 0 and: <list type="bullet">
        /// <item>More than one element found in <paramref name="parentNavigationProperty"/> that should have been unique.</item>
        /// <item>No such element exists in <paramref name="parentNavigationProperty"/>.</item>
        /// </list></exception>
        protected void UpdateModel(ICollection<FailureMechanismEntity> parentNavigationProperty, IFailureMechanism model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (model.StorageId < 1)
            {
                InsertModel(parentNavigationProperty, model);
                return;
            }

            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            FailureMechanismEntity entity;
            try
            {
                entity = parentNavigationProperty.SingleOrDefault(db => db.FailureMechanismEntityId == model.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "FailureMechanismEntity", model.StorageId), exception);
            }
            if (entity == null)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "FailureMechanismEntity", model.StorageId));
            }

            modifiedList.Add(entity);

            ConvertModelToEntity(model, entity);
        }

        /// <summary>
        /// Updates the <paramref name="model"/> from <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to update from.</param>
        /// <param name="model">The <see cref="IFailureMechanism"/> to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        protected static void ConvertEntityToModel(FailureMechanismEntity entity, IFailureMechanism model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.StorageId = entity.FailureMechanismEntityId;
        }

        /// <summary>
        /// Updates the <paramref name="entity"/> from <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="IFailureMechanism"/> to update from.</param>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        private static void ConvertModelToEntity(IFailureMechanism model, FailureMechanismEntity entity)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            entity.FailureMechanismEntityId = model.StorageId;
            if (model is PipingFailureMechanism)
            {
                entity.FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism;
            }
        }
    }
}