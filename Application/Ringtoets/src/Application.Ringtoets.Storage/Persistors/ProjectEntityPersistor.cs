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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
using Core.Common.Base.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="ProjectEntity"/>.
    /// </summary>
    public class ProjectEntityPersistor
    {
        private readonly Dictionary<ProjectEntity, Project> insertedList = new Dictionary<ProjectEntity, Project>();
        private readonly ICollection<ProjectEntity> modifiedList = new List<ProjectEntity>();

        private readonly ProjectEntityConverter converter;
        private readonly IDbSet<ProjectEntity> dbSet;
        private readonly IRingtoetsEntities dbContext;

        private readonly DikeAssessmentSectionEntityPersistor dikeAssessmentSectionEntityPersistor;

        /// <summary>
        /// Instanciate a new ProjectEntityPersistor.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is null.</exception>
        public ProjectEntityPersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            dbContext = ringtoetsContext;
            dbSet = dbContext.ProjectEntities;

            converter = new ProjectEntityConverter();

            dikeAssessmentSectionEntityPersistor = new DikeAssessmentSectionEntityPersistor(dbContext);
        }

        /// <summary>
        /// Gets the only <see cref="ProjectEntity"/> as <see cref="Project"/> from the sequence.
        /// </summary>
        /// <returns>A new <see cref="Project"/>, loaded from the sequence, or <c>null</c> when not found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there are more than one elements in the sequence.</exception>
        public Project GetEntityAsModel()
        {
            var entry = dbSet.SingleOrDefault();
            if (entry == null)
            {
                return null;
            }
            var project = converter.ConvertEntityToModel(entry);
            if (entry.DikeAssessmentSectionEntities.Count > 0)
            {
                var list = dikeAssessmentSectionEntityPersistor.LoadModels(entry.DikeAssessmentSectionEntities);
                foreach (var section in list)
                {
                    project.Items.Add(section);
                }
            }

            return project;
        }

        /// <summary>
        /// Updates the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="DbContext"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="project"><see cref="Project"/> to be saved in the storage.</param>
        /// <returns>The <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="EntityNotFoundException">Thrown when <paramref name="project"/> is not found.</exception>
        public ProjectEntity UpdateEntity(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project", "Cannot update databaseSet when no project is set.");
            }
            var entry = dbSet.SingleOrDefault(db => db.ProjectEntityId == project.StorageId);
            if (entry == null)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "ProjectEntity", project.StorageId));
            }
            modifiedList.Add(entry);
            converter.ConvertModelToEntity(project, entry);

            UpdateChildren(project, entry);
            return entry;
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            UpdateStorageIdsInModel();
            dikeAssessmentSectionEntityPersistor.PerformPostSaveActions();
        }

        /// <summary>
        /// Insert the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="DbContext"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="project"><see cref="Project"/> to be inserted in the sequence.</param>
        /// <returns>New instance of <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        public ProjectEntity AddEntity(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }
            RemoveRedundant(dbContext.ProjectEntities.ToList());
            var entity = new ProjectEntity();
            dbSet.Add(entity);
            insertedList.Add(entity, project);

            converter.ConvertModelToEntity(project, entity);
            InsertChildren(project, entity);
            return entity;
        }

        /// <summary>
        /// Updates the StorageId of each inserted <see cref="Project"/> to the DikeAssessmentSectionEntityId of the corresponding <see cref="ProjectEntity"/>.
        /// </summary>
        /// <remarks><see cref="IRingtoetsEntities.SaveChanges"/> must have been called to update the ids.</remarks>
        private void UpdateStorageIdsInModel()
        {
            foreach (var entry in insertedList)
            {
                Debug.Assert(entry.Key.ProjectEntityId > 0, "ProjectEntityId is not set. Have you called IRingtoetsEntities.SaveChanges?");
                entry.Value.StorageId = entry.Key.ProjectEntityId;
            }
            insertedList.Clear();
        }

        /// <summary>
        /// Updates the children of <paramref name="project"/>, in reference to <paramref name="entity"/>, in the storage.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> of which children need to be updated.</param>
        /// <param name="entity">Referenced <see cref="ProjectEntity"/>.</param>
        private void UpdateChildren(Project project, ProjectEntity entity)
        {
            var listOfDikeAssessmentSections = project.Items.OfType<DikeAssessmentSection>();
            dikeAssessmentSectionEntityPersistor.UpdateModels(entity.DikeAssessmentSectionEntities, listOfDikeAssessmentSections);
        }

        /// <summary>
        /// Inserts the children of <paramref name="project"/>, in reference to <paramref name="entity"/>, in the storage.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> of which children need to be inserted.</param>
        /// <param name="entity">Referenced <see cref="ProjectEntity"/>.</param>
        private void InsertChildren(Project project, ProjectEntity entity)
        {
            var listOfDikeAssessmentSections = project.Items.OfType<DikeAssessmentSection>();
            dikeAssessmentSectionEntityPersistor.InsertModels(entity.DikeAssessmentSectionEntities, listOfDikeAssessmentSections);
        }

        /// <summary>
        /// Removes all entities from <see cref="IRingtoetsEntities.ProjectEntities"/> that are not marked as 'updated'.
        /// </summary>
        /// <param name="listOfParentNavigationProperty">List where <see cref="ProjectEntity"/> objects can be searched. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <returns>Number of entities removed.</returns>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="listOfParentNavigationProperty"/> is read-only.</exception>
        private int RemoveRedundant(ICollection<ProjectEntity> listOfParentNavigationProperty)
        {
            foreach (var u in modifiedList)
            {
                listOfParentNavigationProperty.Remove(u);
            }

            var removedCount = 0;
            foreach (var toDelete in listOfParentNavigationProperty)
            {
                // If id = 0, the entity is marked as inserted
                if (toDelete.ProjectEntityId > 0)
                {
                    dbContext.ProjectEntities.Remove(toDelete);
                    removedCount++;
                }
            }

            return removedCount;
        }
    }
}