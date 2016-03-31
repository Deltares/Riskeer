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
using Core.Common.Base.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="Project"/>.
    /// </summary>
    public class ProjectPersistor
    {
        private readonly DbSet<ProjectEntity> projectEntitySet;
        private readonly ProjectConverter converter;
        private readonly Dictionary<ProjectEntity, Project> insertedList = new Dictionary<ProjectEntity, Project>();
        private readonly ICollection<ProjectEntity> modifiedList = new List<ProjectEntity>();

        private readonly AssessmentSectionPersistor assessmentSectionEntityPersistor;

        /// <summary>
        /// Instantiate a new <see cref="ProjectPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public ProjectPersistor(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            projectEntitySet = ringtoetsContext.ProjectEntities;

            converter = new ProjectConverter();

            assessmentSectionEntityPersistor = new AssessmentSectionPersistor(ringtoetsContext);
        }

        /// <summary>
        /// Gets the only <see cref="ProjectEntity"/> as <see cref="Project"/> from the sequence.
        /// </summary>
        /// <returns>A new <see cref="Project"/>, loaded from the sequence, or <c>null</c> when not found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there are more than one elements in the sequence.</exception>
        public Project GetEntityAsModel()
        {
            var entry = projectEntitySet.SingleOrDefault();
            if (entry == null)
            {
                return null;
            }
            var project = converter.ConvertEntityToModel(entry);

            var nrOfItems = entry.AssessmentSectionEntities.Count;
            var assessmentSections = new object[nrOfItems];

            foreach (var sectionEntity in entry.AssessmentSectionEntities)
            {
                assessmentSections[sectionEntity.Order] = assessmentSectionEntityPersistor.LoadModel(sectionEntity);
            }

            // Add to items sorted 
            foreach (var assessmentSection in assessmentSections)
            {
                project.Items.Add(assessmentSection);
            }

            return project;
        }

        /// <summary>
        /// Insert the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="IRingtoetsEntities.SaveChanges"/> afterwards to update the storage.</remarks>
        /// <param name="project"><see cref="Project"/> to be inserted in the sequence.</param>
        /// <returns>New instance of <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The parentNavigationProperty is read-only.</exception>
        public void InsertModel(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project", @"Cannot update databaseSet when no project is set.");
            }

            var entity = new ProjectEntity();
            projectEntitySet.Add(entity);
            insertedList.Add(entity, project);

            converter.ConvertModelToEntity(project, entity);

            if (project.StorageId > 0)
            {
                modifiedList.Add(entity);
            }

            InsertChildren(project, entity);
        }

        /// <summary>
        /// Updates the <see cref="ProjectEntity"/>, based upon the <paramref name="model"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="IRingtoetsEntities.SaveChanges"/> afterwards to update the storage.</remarks>
        /// <param name="model">The <see cref="ProjectEntity"/> to be saved in the storage.</param>
        /// <returns>The <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="EntityNotFoundException">Thrown when: <list type="bullet">
        /// <item><paramref name="model"/> is not found.</item>
        /// <item>More than one <paramref name="model"/> satisfies the condition in predicate.</item>
        /// </list> </exception>
        /// <exception cref="NotSupportedException">The parentNavigationProperty is read-only.</exception>
        public void UpdateModel(Project model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model", @"Cannot update databaseSet when no project is set.");
            }
            ProjectEntity entity;
            try
            {
                entity = projectEntitySet.SingleOrDefault(db => db.ProjectEntityId == model.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "ProjectEntity", model.StorageId), exception);
            }
            if (entity == null)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "ProjectEntity", model.StorageId));
            }
            modifiedList.Add(entity);
            converter.ConvertModelToEntity(model, entity);

            UpdateChildren(model, entity);
        }

        /// <summary>
        /// Removes all entities from <see cref="IRingtoetsEntities.ProjectEntities"/> that are not marked as 'updated'.
        /// </summary>
        public void RemoveUnModifiedEntries()
        {
            var untouchedList = projectEntitySet.ToList().Where(e => !modifiedList.Contains(e));
            projectEntitySet.RemoveRange(untouchedList);

            modifiedList.Clear();
        }

        /// <summary>
        /// Perform actions that can only be executed after <see cref="IRingtoetsEntities.SaveChanges"/> has been called.
        /// </summary>
        public void PerformPostSaveActions()
        {
            UpdateStorageIdsInModel();
            assessmentSectionEntityPersistor.PerformPostSaveActions();
        }

        /// <summary>
        /// Updates the children of <paramref name="project"/>, in reference to <paramref name="entity"/>, in the storage.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> of which children need to be updated.</param>
        /// <param name="entity">Referenced <see cref="ProjectEntity"/>.</param>
        private void UpdateChildren(Project project, ProjectEntity entity)
        {
            var order = 0;
            foreach (var item in project.Items.Where(i => i is AssessmentSection).Cast<AssessmentSection>())
            {
                assessmentSectionEntityPersistor.UpdateModel(entity.AssessmentSectionEntities, item, order);
                order++;
            }
            assessmentSectionEntityPersistor.RemoveUnModifiedEntries(entity.AssessmentSectionEntities);
        }

        /// <summary>
        /// Inserts the children of <paramref name="project"/>, in reference to <paramref name="entity"/>, in the storage.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> of which children need to be inserted.</param>
        /// <param name="entity">Referenced <see cref="ProjectEntity"/>.</param>
        private void InsertChildren(Project project, ProjectEntity entity)
        {
            var order = 0;
            foreach (var item in project.Items.Where(i => i is AssessmentSection).Cast<AssessmentSection>())
            {
                assessmentSectionEntityPersistor.InsertModel(entity.AssessmentSectionEntities, item, order);
                order++;
            }
            assessmentSectionEntityPersistor.RemoveUnModifiedEntries(entity.AssessmentSectionEntities);
        }

        /// <summary>
        /// Updates the StorageId of each inserted <see cref="Project"/> to the 
        /// <see cref="ProjectEntity.ProjectEntityId"/> of the corresponding <see cref="ProjectEntity"/>.
        /// </summary>
        /// <remarks><see cref="IRingtoetsEntities.SaveChanges"/> must have been called to update the ids.</remarks>
        private void UpdateStorageIdsInModel()
        {
            foreach (var entry in insertedList)
            {
                Debug.Assert(entry.Key.ProjectEntityId > 0, 
                    "ProjectEntityId is not set. Have you called IRingtoetsEntities.SaveChanges?");
                entry.Value.StorageId = entry.Key.ProjectEntityId;
            }
            insertedList.Clear();
        }
    }
}