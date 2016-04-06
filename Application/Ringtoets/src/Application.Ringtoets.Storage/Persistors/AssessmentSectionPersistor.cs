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
using System.Diagnostics;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="AssessmentSection"/>.
    /// </summary>
    public class AssessmentSectionPersistor
    {
        private readonly DbSet<AssessmentSectionEntity> assessmentSectionSet;
        private readonly AssessmentSectionConverter converter;
        private readonly Dictionary<AssessmentSectionEntity, AssessmentSection> insertedList = new Dictionary<AssessmentSectionEntity, AssessmentSection>();
        private readonly ICollection<AssessmentSectionEntity> modifiedList = new List<AssessmentSectionEntity>();

        private readonly PipingFailureMechanismPersistor pipingFailureMechanismEntityPersistor;
        private readonly HydraulicBoundaryLocationPersistor hydraulicLocationEntityPersistor;
        private readonly ReferenceLinePersistor referenceLinePersistor;

        /// <summary>
        /// New instance of <see cref="AssessmentSectionPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public AssessmentSectionPersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            assessmentSectionSet = ringtoetsContext.AssessmentSectionEntities;

            converter = new AssessmentSectionConverter();

            pipingFailureMechanismEntityPersistor = new PipingFailureMechanismPersistor(ringtoetsContext);
            hydraulicLocationEntityPersistor = new HydraulicBoundaryLocationPersistor(ringtoetsContext);
            referenceLinePersistor = new ReferenceLinePersistor(ringtoetsContext);
        }

        /// <summary>
        /// Loads the <see cref="AssessmentSectionEntity"/> as <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="entity">The <see cref="AssessmentSectionEntity"/> to load.</param>
        /// <returns>A new instance of <see cref="AssessmentSection"/>, based on the properties of <paramref name="entity"/>.</returns>
        public AssessmentSection LoadModel(AssessmentSectionEntity entity)
        {
            var assessmentSection = converter.ConvertEntityToModel(entity);

            if (assessmentSection.HydraulicBoundaryDatabase != null)
            {
                assessmentSection.HydraulicBoundaryDatabase.Locations.AddRange(hydraulicLocationEntityPersistor.LoadModel(entity.HydraulicLocationEntities));
            }

            foreach (var failureMechanismEntity in entity.FailureMechanismEntities)
            {
                if (failureMechanismEntity.FailureMechanismType == (int) FailureMechanismType.PipingFailureMechanism)
                {
                    pipingFailureMechanismEntityPersistor.LoadModel(failureMechanismEntity, assessmentSection.Piping);
                }
            }

            assessmentSection.ReferenceLine = referenceLinePersistor.LoadModel(entity.ReferenceLinePointEntities);

            return assessmentSection;
        }

        /// <summary>
        /// Ensures that the model is added as <see cref="AssessmentSectionEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="AssessmentSectionEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="AssessmentSection"/> to be saved in the storage.</param>
        /// <param name="order">Value used for sorting.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        public void InsertModel(ICollection<AssessmentSectionEntity> parentNavigationProperty, AssessmentSection model, int order)
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
        /// Ensures that the <paramref name="model"/> is set as <see cref="AssessmentSectionEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="AssessmentSectionEntity"/> objects can be searched and added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="AssessmentSection"/> to be saved in the storage.</param>
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
        public void UpdateModel(ICollection<AssessmentSectionEntity> parentNavigationProperty, AssessmentSection model, int order)
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
        /// All unmodified <see cref="AssessmentSectionEntity"/> in <paramref name="parentNavigationProperty"/> will be removed.
        /// </summary>
        /// <param name="parentNavigationProperty">List where <see cref="AssessmentSectionEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        public void RemoveUnModifiedEntries(ICollection<AssessmentSectionEntity> parentNavigationProperty)
        {
            var untouchedModifiedList = parentNavigationProperty.Where(e => e.AssessmentSectionEntityId > 0 && !modifiedList.Contains(e));
            assessmentSectionSet.RemoveRange(untouchedModifiedList);

            modifiedList.Clear();
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            UpdateStorageIdsInModel();

            pipingFailureMechanismEntityPersistor.PerformPostSaveActions();
            hydraulicLocationEntityPersistor.PerformPostSaveActions();
        }

        /// <summary>
        /// Updates the children of <paramref name="model"/>, in reference to <paramref name="entity"/>, in the storage.
        /// </summary>
        /// <param name="model">The <see cref="AssessmentSection"/> of which children need to be updated.</param>
        /// <param name="entity">Referenced <see cref="AssessmentSectionEntity"/>.</param>
        private void UpdateChildren(AssessmentSection model, AssessmentSectionEntity entity)
        {
            pipingFailureMechanismEntityPersistor.UpdateModel(entity.FailureMechanismEntities, model.Piping);
            pipingFailureMechanismEntityPersistor.RemoveUnModifiedEntries(entity.FailureMechanismEntities);

            hydraulicLocationEntityPersistor.UpdateModel(entity.HydraulicLocationEntities, model.HydraulicBoundaryDatabase);
            referenceLinePersistor.InsertModel(entity.ReferenceLinePointEntities, model.ReferenceLine);
        }

        /// <summary>
        /// Inserts the children of <paramref name="model"/>, in reference to <paramref name="model"/>, in the storage.
        /// </summary>
        /// <param name="model">The <see cref="AssessmentSection"/> of which children need to be inserted.</param>
        /// <param name="entity">Referenced <see cref="AssessmentSectionEntity"/>.</param>
        private void InsertChildren(AssessmentSection model, AssessmentSectionEntity entity)
        {
            pipingFailureMechanismEntityPersistor.InsertModel(entity.FailureMechanismEntities, model.Piping);
            pipingFailureMechanismEntityPersistor.RemoveUnModifiedEntries(entity.FailureMechanismEntities);

            hydraulicLocationEntityPersistor.InsertModel(entity.HydraulicLocationEntities, model.HydraulicBoundaryDatabase);
            referenceLinePersistor.InsertModel(entity.ReferenceLinePointEntities, model.ReferenceLine);
        }

        /// <summary>
        /// Performs the update of <paramref name="model"/> to <see cref="AssessmentSectionEntity"/>.
        /// </summary>
        /// <param name="model"><see cref="AssessmentSection"/> to update.</param>
        /// <param name="parentNavigationProperty">Collection where the <see cref="AssessmentSectionEntity"/> can be found.</param>
        /// <param name="order">Value used for sorting.</param>
        /// <returns>The <paramref name="model"/> to <see cref="AssessmentSectionEntity"/>.</returns>
        private AssessmentSectionEntity PerformUpdateModel(AssessmentSection model, ICollection<AssessmentSectionEntity> parentNavigationProperty, int order)
        {
            AssessmentSectionEntity entity;
            try
            {
                entity = parentNavigationProperty.SingleOrDefault(db => db.AssessmentSectionEntityId == model.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "AssessmentSectionEntity", model.StorageId), exception);
            }
            if (entity == null)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "AssessmentSectionEntity", model.StorageId));
            }

            modifiedList.Add(entity);

            converter.ConvertModelToEntity(model, entity);
            entity.Order = order;

            return entity;
        }

        /// <summary>
        /// Inserts the <paramref name="model"/> as <see cref="AssessmentSectionEntity"/> in <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="model"><see cref="AssessmentSection"/> to be added.</param>
        /// <param name="parentNavigationProperty">Collection where to add the <paramref name="model"/> as <see cref="AssessmentSectionEntity"/>.</param>
        /// <param name="order">Value used for sorting.</param>
        /// <returns>The added <paramref name="model"/> as <see cref="AssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parentNavigationProperty"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        private AssessmentSectionEntity PerformInsertModel(AssessmentSection model, ICollection<AssessmentSectionEntity> parentNavigationProperty, int order)
        {
            var entity = new AssessmentSectionEntity();
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
        /// Updates the StorageId of each inserted <see cref="AssessmentSection"/> to the
        /// <see cref="AssessmentSectionEntity.AssessmentSectionEntityId"/> of the corresponding 
        /// <see cref="AssessmentSectionEntity"/>.
        /// </summary>
        /// <remarks><see cref="IRingtoetsEntities.SaveChanges"/> must have been called to 
        /// update the ids.</remarks>
        private void UpdateStorageIdsInModel()
        {
            foreach (var entry in insertedList)
            {
                Debug.Assert(entry.Key.AssessmentSectionEntityId > 0, 
                    "AssessmentSectionEntityId is not set. Have you called IRingtoetsEntities.SaveChanges?");
                entry.Value.StorageId = entry.Key.AssessmentSectionEntityId;
            }
            insertedList.Clear();
        }
    }
}