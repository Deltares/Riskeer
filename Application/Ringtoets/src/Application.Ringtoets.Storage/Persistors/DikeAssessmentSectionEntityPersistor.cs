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
using System.Diagnostics;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="DikeAssessmentSectionEntity"/>.
    /// </summary>
    public class DikeAssessmentSectionEntityPersistor : IPersistor<DikeAssessmentSectionEntity, DikeAssessmentSection>
    {
        private readonly IRingtoetsEntities dbContext;
        private readonly DikeAssessmentSectionEntityConverter converter;
        private readonly Dictionary<DikeAssessmentSectionEntity, DikeAssessmentSection> insertedList = new Dictionary<DikeAssessmentSectionEntity, DikeAssessmentSection>();
        private readonly ICollection<DikeAssessmentSectionEntity> modifiedList = new List<DikeAssessmentSectionEntity>();

        private readonly DikesPipingFailureMechanismEntityPersistor dikePipingFailureMechanismEntityPersistor;
        private readonly HydraulicLocationEntityPersistor hydraulicLocationEntityPersistor;

        /// <summary>
        /// New instance of <see cref="DikeAssessmentSectionEntityPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public DikeAssessmentSectionEntityPersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            dbContext = ringtoetsContext;

            converter = new DikeAssessmentSectionEntityConverter();

            dikePipingFailureMechanismEntityPersistor = new DikesPipingFailureMechanismEntityPersistor(dbContext);
            hydraulicLocationEntityPersistor = new HydraulicLocationEntityPersistor(dbContext);
        }

        /// <summary>
        /// Loads the <see cref="DikeAssessmentSectionEntity"/> as <see cref="DikeAssessmentSection"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DikeAssessmentSectionEntity"/> to load.</param>
        /// <param name="model">The <see cref="Func{TResult}"/> to obtain the model.</param>
        /// <returns>A new instance of <see cref="DikeAssessmentSection"/>, based on the properties of <paramref name="entity"/>.</returns>
        public DikeAssessmentSection LoadModel(DikeAssessmentSectionEntity entity, Func<DikeAssessmentSection> model)
        {
            var dikeAssessmentSection = converter.ConvertEntityToModel(entity, model);

            foreach (var hydraulicLocationEntity in entity.HydraulicLocationEntities)
            {
                dikeAssessmentSection.HydraulicBoundaryDatabase.Locations.Add(hydraulicLocationEntityPersistor.LoadModel(hydraulicLocationEntity, () => new HydraulicBoundaryLocation()));
            }

            foreach (var failureMechanismEntity in entity.FailureMechanismEntities)
            {
                if (failureMechanismEntity.FailureMechanismType == (int) FailureMechanismType.DikesPipingFailureMechanism)
                {
                    dikePipingFailureMechanismEntityPersistor.LoadModel(failureMechanismEntity, () => dikeAssessmentSection.PipingFailureMechanism);
                }
            }

            return dikeAssessmentSection;
        }

        /// <summary>
        /// Ensures that the model is added as <see cref="DikeAssessmentSectionEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="DikeAssessmentSectionEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="DikeAssessmentSection"/> to be saved in the storage.</param>
        /// <param name="order">Value used for sorting.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        public void InsertModel(ICollection<DikeAssessmentSectionEntity> parentNavigationProperty, DikeAssessmentSection model, int order)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            var entity = PerformInsertModel(model, parentNavigationProperty, order);

            InsertChildren(model, entity);
        }

        /// <summary>
        /// Ensures that the <paramref name="model"/> is set as <see cref="DikeAssessmentSectionEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="DikeAssessmentSectionEntity"/> objects can be searched and added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="DikeAssessmentSection"/> to be saved in the storage.</param>
        /// <param name="order">Value used for sorting.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        /// <exception cref="EntityNotFoundException">Thrown when the storageId of <paramref name="model"/> &gt; 0 and: <list type="bullet">
        /// <item>More than one element found in <paramref name="parentNavigationProperty"/> that should have been unique.</item>
        /// <item>No such element exists in <paramref name="parentNavigationProperty"/>.</item>
        /// </list></exception>
        public void UpdateModel(ICollection<DikeAssessmentSectionEntity> parentNavigationProperty, DikeAssessmentSection model, int order)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            var entity = model.StorageId > 0 ? PerformUpdateModel(model, parentNavigationProperty, order) : PerformInsertModel(model, parentNavigationProperty, order);

            UpdateChildren(model, entity);
        }

        /// <summary>
        /// All unmodified <see cref="DikeAssessmentSectionEntity"/> in <paramref name="parentNavigationProperty"/> will be removed.
        /// </summary>
        /// <param name="parentNavigationProperty">List where <see cref="DikeAssessmentSectionEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        public void RemoveUnModifiedEntries(ICollection<DikeAssessmentSectionEntity> parentNavigationProperty)
        {
            var originalList = parentNavigationProperty.ToList();
            foreach (var u in modifiedList)
            {
                originalList.Remove(u);
            }

            foreach (var toDelete in originalList)
            {
                // If id = 0, the entity is marked as inserted
                if (toDelete.DikeAssessmentSectionEntityId > 0)
                {
                    dbContext.DikeAssessmentSectionEntities.Remove(toDelete);
                }
            }

            modifiedList.Clear();
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            UpdateStorageIdsInModel();

            dikePipingFailureMechanismEntityPersistor.PerformPostSaveActions();
        }

        /// <summary>
        /// Updates the children of <paramref name="model"/>, in reference to <paramref name="entity"/>, in the storage.
        /// </summary>
        /// <param name="model">The <see cref="DikeAssessmentSection"/> of which children need to be updated.</param>
        /// <param name="entity">Referenced <see cref="DikeAssessmentSectionEntity"/>.</param>
        public void UpdateChildren(DikeAssessmentSection model, DikeAssessmentSectionEntity entity)
        {
            dikePipingFailureMechanismEntityPersistor.UpdateModel(entity.FailureMechanismEntities, model.PipingFailureMechanism, 0);
            dikePipingFailureMechanismEntityPersistor.RemoveUnModifiedEntries(entity.FailureMechanismEntities);
        }

        /// <summary>
        /// Inserts the children of <paramref name="model"/>, in reference to <paramref name="model"/>, in the storage.
        /// </summary>
        /// <param name="model">The <see cref="DikeAssessmentSection"/> of which children need to be inserted.</param>
        /// <param name="entity">Referenced <see cref="DikeAssessmentSectionEntity"/>.</param>
        public void InsertChildren(DikeAssessmentSection model, DikeAssessmentSectionEntity entity)
        {
            dikePipingFailureMechanismEntityPersistor.InsertModel(entity.FailureMechanismEntities, model.PipingFailureMechanism, 0);
            dikePipingFailureMechanismEntityPersistor.RemoveUnModifiedEntries(entity.FailureMechanismEntities);

            if (model.HydraulicBoundaryDatabase != null)
            {
                foreach (var hydraulicBoundaryLocation in model.HydraulicBoundaryDatabase.Locations)
                {
                    hydraulicLocationEntityPersistor.InsertModel(entity.HydraulicLocationEntities, hydraulicBoundaryLocation, 0);
                }
            }
            hydraulicLocationEntityPersistor.RemoveUnModifiedEntries(entity.HydraulicLocationEntities);
        }

        /// <summary>
        /// Performs the update of <paramref name="model"/> to <see cref="DikeAssessmentSectionEntity"/>.
        /// </summary>
        /// <param name="model"><see cref="DikeAssessmentSection"/> to update.</param>
        /// <param name="parentNavigationProperty">Collection where the <see cref="DikeAssessmentSectionEntity"/> can be found.</param>
        /// <param name="order">Value used for sorting.</param>
        /// <returns>The <paramref name="model"/> to <see cref="DikeAssessmentSectionEntity"/>.</returns>
        private DikeAssessmentSectionEntity PerformUpdateModel(DikeAssessmentSection model, ICollection<DikeAssessmentSectionEntity> parentNavigationProperty, int order)
        {
            DikeAssessmentSectionEntity entity;
            try
            {
                entity = parentNavigationProperty.SingleOrDefault(db => db.DikeAssessmentSectionEntityId == model.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "DikeAssessmentSectionEntity", model.StorageId), exception);
            }
            if (entity == null)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "DikeAssessmentSectionEntity", model.StorageId));
            }

            modifiedList.Add(entity);

            converter.ConvertModelToEntity(model, entity);
            entity.Order = order;

            return entity;
        }

        /// <summary>
        /// Inserts the <paramref name="model"/> as <see cref="DikeAssessmentSectionEntity"/> in <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="model"><see cref="DikeAssessmentSection"/> to be added.</param>
        /// <param name="parentNavigationProperty">Collection where to add the <paramref name="model"/> as <see cref="DikeAssessmentSectionEntity"/>.</param>
        /// <param name="order">Value used for sorting.</param>
        /// <returns>The added <paramref name="model"/> as <see cref="DikeAssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parentNavigationProperty"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        private DikeAssessmentSectionEntity PerformInsertModel(DikeAssessmentSection model, ICollection<DikeAssessmentSectionEntity> parentNavigationProperty, int order)
        {
            var entity = new DikeAssessmentSectionEntity();
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, model);

            converter.ConvertModelToEntity(model, entity);
            entity.Order = order;

            if (model.StorageId > 0)
            {
                modifiedList.Add(entity);
            }

            return entity;
        }

        /// <summary>
        /// Updates the StorageId of each inserted <see cref="DikeAssessmentSection"/> to the DikeAssessmentSectionEntityId of the corresponding <see cref="DikeAssessmentSectionEntity"/>.
        /// </summary>
        /// <remarks><see cref="IRingtoetsEntities.SaveChanges"/> must have been called to update the ids.</remarks>
        private void UpdateStorageIdsInModel()
        {
            foreach (var entry in insertedList)
            {
                Debug.Assert(entry.Key.DikeAssessmentSectionEntityId > 0, "DikeAssessmentSectionEntityId is not set. Have you called IRingtoetsEntities.SaveChanges?");
                entry.Value.StorageId = entry.Key.DikeAssessmentSectionEntityId;
            }
            insertedList.Clear();
        }
    }
}