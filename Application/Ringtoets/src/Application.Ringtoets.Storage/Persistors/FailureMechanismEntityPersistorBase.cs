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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    public abstract class FailureMechanismEntityPersistorBase
    {
        protected readonly IRingtoetsEntities dbContext;
        private readonly Dictionary<FailureMechanismEntity, IFailureMechanism> insertedList = new Dictionary<FailureMechanismEntity, IFailureMechanism>();
        private readonly ICollection<FailureMechanismEntity> modifiedList = new List<FailureMechanismEntity>();

        protected FailureMechanismEntityPersistorBase(IRingtoetsEntities ringtoetsContext)
        {
            if (ringtoetsContext == null)
            {
                throw new ArgumentNullException("ringtoetsContext");
            }
            dbContext = ringtoetsContext;
        }

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

        public void PerformPostSaveActions()
        {
            foreach (var entry in insertedList)
            {
                entry.Value.StorageId = entry.Key.FailureMechanismEntityId;
            }
            insertedList.Clear();
        }

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

        protected static void LoadModel(IFailureMechanism model, FailureMechanismType modelType, FailureMechanismEntity entity)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (entity.FailureMechanismType != (int) modelType)
            {
                throw new ArgumentException("Incorrect modelType", "entity");
            }
            model.StorageId = entity.FailureMechanismEntityId;
        }

        private static void ConvertModelToEntity(IFailureMechanism modelObject, FailureMechanismEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            entity.FailureMechanismEntityId = modelObject.StorageId;
            if (modelObject is PipingFailureMechanism)
            {
                entity.FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism;
                return;
            }
            throw new InvalidOperationException("modelObject is not a known implementation of IFailureMechanism");
        }
    }
}