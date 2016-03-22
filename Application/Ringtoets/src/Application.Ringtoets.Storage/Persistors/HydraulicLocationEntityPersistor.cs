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

        public void UpdateModel(ICollection<HydraulicLocationEntity> parentNavigationProperty, HydraulicBoundaryLocation model, int order)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (model.StorageId < 1)
            {
                InsertModel(parentNavigationProperty, model, 0);
                return;
            }

            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            HydraulicLocationEntity entity;
            try
            {
                entity = parentNavigationProperty.SingleOrDefault(db => db.HydraulicLocationEntityId == model.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "HydraulicLocationEntity", model.StorageId), exception);
            }

            if (entity == null)
            {
                throw new EntityNotFoundException(String.Format(Resources.Error_Entity_Not_Found_0_1, "HydraulicLocationEntity", model.StorageId));
            }

            modifiedList.Add(entity);

            converter.ConvertModelToEntity(model, entity);
        }

        public void InsertModel(ICollection<HydraulicLocationEntity> parentNavigationProperty, HydraulicBoundaryLocation model, int order)
        {
            if (parentNavigationProperty == null)
            {
                throw new ArgumentNullException("parentNavigationProperty");
            }

            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var entity = new HydraulicLocationEntity();
            parentNavigationProperty.Add(entity);
            insertedList.Add(entity, model);

            converter.ConvertModelToEntity(model, entity);

            if (model.StorageId > 0)
            {
                modifiedList.Add(entity);
            }
        }

        public void RemoveUnModifiedEntries(ICollection<HydraulicLocationEntity> parentNavigationProperty)
        {
            var originalList = parentNavigationProperty.ToList();

            foreach (var hydraulicLocationEntity in modifiedList)
            {
                originalList.Remove(hydraulicLocationEntity);
            }

            foreach (var toDelete in originalList)
            {
                if (toDelete.HydraulicLocationEntityId > 0)
                {
                    ringtoetsContext.HydraulicLocationEntities.Remove(toDelete);
                }
            }

            modifiedList.Clear();
        }

        public void PerformPostSaveActions()
        {
            foreach (var entry in insertedList)
            {
                entry.Value.StorageId = entry.Key.HydraulicLocationEntityId;
            }
            insertedList.Clear();
        }

        public HydraulicBoundaryLocation LoadModel(HydraulicLocationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return converter.ConvertEntityToModel(entity);
        }
    }
}