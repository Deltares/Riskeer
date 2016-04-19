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
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for classes which implement <see cref="IFailureMechanism"/>.
    /// </summary>
    public abstract class FailureMechanismPersistorBase<T> where T : IFailureMechanism
    {
        private readonly DbSet<FailureMechanismEntity> failureMechanismSet;
        private readonly Dictionary<FailureMechanismEntity, T> insertedList = new Dictionary<FailureMechanismEntity, T>();
        private readonly ICollection<FailureMechanismEntity> modifiedList = new List<FailureMechanismEntity>();

        private readonly IEntityConverter<T, FailureMechanismEntity> converter;

        /// <summary>
        /// New instance of <see cref="FailureMechanismPersistorBase{T}"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <param name="converter">An implementation of the <see cref="IEntityConverter{T,T}"/> to use in the persistor.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        protected FailureMechanismPersistorBase(IRingtoetsEntities ringtoetsContext, IEntityConverter<T, FailureMechanismEntity> converter)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            failureMechanismSet = ringtoetsContext.FailureMechanismEntities;
            this.converter = converter;
        }

        /// <summary>
        /// Loads the <see cref="FailureMechanismEntity"/> as <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="entity"><see cref="FailureMechanismEntity"/> to load from.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to load data in.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// <item><paramref name="failureMechanism"/> is <c>null</c>.</item>
        /// </list></exception>
        public void LoadModel(FailureMechanismEntity entity, T failureMechanism)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }

            var model = converter.ConvertEntityToModel(entity);

            failureMechanism.StorageId = model.StorageId;

            LoadChildren(failureMechanism, entity);
        }

        /// <summary>
        /// Implement to provide a way to load the children of the <paramref name="entity"/> as children of <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <typeparamref name="T"/> to load into.</param>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to load from.</param>
        protected abstract void LoadChildren(T model, FailureMechanismEntity entity);

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
        public void UpdateModel(ICollection<FailureMechanismEntity> parentNavigationProperty, T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            if (model.StorageId < 1)
            {
                InsertModel(parentNavigationProperty, model);
                return;
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

            converter.ConvertModelToEntity(model, entity);

            UpdateChildren(model, entity);
        }

        /// <summary>
        /// Implement to provide a way to update the children of the <paramref name="entity"/> with data from <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <typeparamref name="T"/> for which to use the data to update the <paramref name="entity"/>.</param>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to update.</param>
        protected abstract void UpdateChildren(T model, FailureMechanismEntity entity);

        /// <summary>
        /// Ensures that the model is added as <see cref="FailureMechanismEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="FailureMechanismEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="IFailureMechanism"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// </list></exception>
        public void InsertModel(ICollection<FailureMechanismEntity> parentNavigationProperty, T model)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            var entity = new FailureMechanismEntity();
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, model);

            converter.ConvertModelToEntity(model, entity);

            if (model.StorageId > 0)
            {
                modifiedList.Add(entity);
            }

            InsertChildren(model, entity);
        }


        /// <summary>
        /// Implement to provide a way to insert the children of the <paramref name="model"/> into the <paramref name="entity"/>.
        /// </summary>
        /// <param name="model">The <typeparamref name="T"/> for which to use the data to update the <paramref name="entity"/>.</param>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to update.</param>
        protected abstract void InsertChildren(T model, FailureMechanismEntity entity);

        /// <summary>
        /// All unmodified <see cref="FailureMechanismEntity"/> in <paramref name="parentNavigationProperty"/> will be removed.
        /// </summary>
        /// <param name="parentNavigationProperty">List where <see cref="FailureMechanismEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        public void RemoveUnModifiedEntries(ICollection<FailureMechanismEntity> parentNavigationProperty)
        {
            var untouchedModifiedList = parentNavigationProperty.Where(e => e.FailureMechanismEntityId > 0 && !modifiedList.Contains(e));
            failureMechanismSet.RemoveRange(untouchedModifiedList);

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
    }
}