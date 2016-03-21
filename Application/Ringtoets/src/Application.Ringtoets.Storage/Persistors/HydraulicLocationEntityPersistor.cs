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
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    public class HydraulicLocationEntityPersistor : IPersistor<HydraulicLocationEntity, HydraulicBoundaryLocation>
    {
        private readonly IRingtoetsEntities ringtoetsContext;
        private readonly HydraulicLocationConverter converter;

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
        }

        public void InsertModel(ICollection<HydraulicLocationEntity> parentNavigationProperty, HydraulicBoundaryLocation model, int order)
        {
        }

        public void RemoveUnModifiedEntries(ICollection<HydraulicLocationEntity> parentNavigationProperty)
        {
        }

        public void PerformPostSaveActions()
        {
        }

        public HydraulicBoundaryLocation LoadModel(HydraulicLocationEntity entity, Func<HydraulicBoundaryLocation> model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                if (model() == null)
                {
                    throw new ArgumentNullException("model");
                }
            }
            catch (NullReferenceException)
            {
                throw new ArgumentNullException("model");
            }

            return converter.ConvertEntityToModel(entity, model);
        }

        public void UpdateChildren(HydraulicBoundaryLocation model, HydraulicLocationEntity entity)
        {
        }

        public void InsertChildren(HydraulicBoundaryLocation model, HydraulicLocationEntity entity)
        {            
        }
    }
}
