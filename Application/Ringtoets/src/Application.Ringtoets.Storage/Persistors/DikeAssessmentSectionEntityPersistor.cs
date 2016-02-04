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
    /// Persistor for <see cref="DikeAssessmentSectionEntity"/>.
    /// </summary>
    public class DikeAssessmentSectionEntityPersistor
    {
        private readonly Dictionary<DikeAssessmentSectionEntity, DikeAssessmentSection> insertedList = new Dictionary<DikeAssessmentSectionEntity, DikeAssessmentSection>();
        private readonly DikeAssessmentSectionEntityConverter converter;
        private readonly IRingtoetsEntities dbContext;
        private readonly ICollection<DikeAssessmentSectionEntity> modifiedList = new List<DikeAssessmentSectionEntity>();

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
        }

        /// <summary>
        /// Ensures that the <paramref name="listOfDikeAssessmentSections"/> is set as <see cref="DikeAssessmentSectionEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// All other <see cref="DikeAssessmentSectionEntity"/> in <paramref name="parentNavigationProperty"/> will be removed.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="DikeAssessmentSectionEntity"/> objects can be searched and added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="listOfDikeAssessmentSections">List of <see cref="DikeAssessmentSection"/> objects to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="listOfDikeAssessmentSections"/> is <c>null</c>.</item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">Thrown when the storageId of an element in <paramref name="listOfDikeAssessmentSections"/> &gt; 0 and: <list type="bullet">
        /// <item>More than one element found in <paramref name="parentNavigationProperty"/> that should have been unique.</item>
        /// <item>No such element exists in <paramref name="parentNavigationProperty"/>.</item>
        /// </list></exception>
        public void UpdateModels(ICollection<DikeAssessmentSectionEntity> parentNavigationProperty, IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }
            if (listOfDikeAssessmentSections == null)
            {
                throw new ArgumentNullException("listOfDikeAssessmentSections");
            }
            var beforeCount = parentNavigationProperty.Count;
            foreach (var model in listOfDikeAssessmentSections)
            {
                DikeAssessmentSectionEntity entity = null;
                if (model.StorageId > 0)
                {
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
                }
                if (entity == null)
                {
                    entity = InsertModelInParentNavigationProperty(model, parentNavigationProperty);
                }
                converter.ConvertModelToEntity(model, entity);
            }

            var modified = modifiedList.Count;
            var removed = RemoveRedundant(parentNavigationProperty.ToList());

            Debug.Assert((modified + removed) == beforeCount, "Storage corrupted");

            modifiedList.Clear();
        }

        /// <summary>
        /// Ensures that the <paramref name="listOfDikeAssessmentSections"/> is added as <see cref="DikeAssessmentSectionEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// All other <see cref="DikeAssessmentSectionEntity"/> in <paramref name="parentNavigationProperty"/> will be removed.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="DikeAssessmentSectionEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="listOfDikeAssessmentSections">List of <see cref="DikeAssessmentSection"/> objects to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="listOfDikeAssessmentSections"/> is <c>null</c>.</item>
        /// </list></exception>
        public void InsertModels(ICollection<DikeAssessmentSectionEntity> parentNavigationProperty, IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }
            if (listOfDikeAssessmentSections == null)
            {
                throw new ArgumentNullException("listOfDikeAssessmentSections");
            }
            RemoveRedundant(parentNavigationProperty.ToList());
            foreach (var model in listOfDikeAssessmentSections)
            {
                InsertModelInParentNavigationProperty(model, parentNavigationProperty);
            }
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            UpdateStorageIdsInModel();
        }

        /// <summary>
        /// Loads the <see cref="DikeAssessmentSectionEntity"/> as <see cref="DikeAssessmentSection"/> from <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="DikeAssessmentSectionEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <returns>List of <see cref="DikeAssessmentSection"/>.</returns>
        public IEnumerable<DikeAssessmentSection> LoadModels(ICollection<DikeAssessmentSectionEntity> parentNavigationProperty)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }
            var list = new List<DikeAssessmentSection>();
            var entities = parentNavigationProperty.ToList();
            foreach (var entity in entities)
            {
                list.Add(converter.ConvertEntityToModel(entity));
            }
            return list;
        }

        /// <summary>
        /// Inserts the <paramref name="model"/> as <see cref="DikeAssessmentSectionEntity"/> in <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="model"><see cref="DikeAssessmentSection"/> to be added.</param>
        /// <param name="parentNavigationProperty"></param>
        /// <returns>The added <see cref="DikeAssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parentNavigationProperty"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        private DikeAssessmentSectionEntity InsertModelInParentNavigationProperty(DikeAssessmentSection model, ICollection<DikeAssessmentSectionEntity> parentNavigationProperty)
        {
            // Insert in parent
            var entity = new DikeAssessmentSectionEntity();
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, model);

            converter.ConvertModelToEntity(model, entity);
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

        /// <summary>
        /// Removes all entities from <see cref="IRingtoetsEntities.DikeAssessmentSectionEntities"/> that are not marked as 'updated'.
        /// </summary>
        /// <param name="listOfParentNavigationProperty">List where <see cref="DikeAssessmentSectionEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <returns>Number of entities removed.</returns>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="listOfParentNavigationProperty"/> is read-only.</exception>
        private int RemoveRedundant(ICollection<DikeAssessmentSectionEntity> listOfParentNavigationProperty)
        {
            foreach (var u in modifiedList)
            {
                listOfParentNavigationProperty.Remove(u);
            }

            var removedCount = 0;
            foreach (var toDelete in listOfParentNavigationProperty)
            {
                // If id = 0, the entity is marked as inserted
                if (toDelete.DikeAssessmentSectionEntityId > 0)
                {
                    dbContext.DikeAssessmentSectionEntities.Remove(toDelete);
                    removedCount++;
                }
            }

            return removedCount;
        }
    }
}