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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class PipingFailureMechanismPersistor : FailureMechanismEntityPersistorBase
    {
        /// <summary>
        /// New instance of <see cref="PipingFailureMechanismPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public PipingFailureMechanismPersistor(IRingtoetsEntities ringtoetsContext) : base(ringtoetsContext) {}

        /// <summary>
        /// Loads the <see cref="FailureMechanismEntity"/> as <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="entity"><see cref="FailureMechanismEntity"/> to load from.</param>
        /// <param name="model"><see cref="PipingFailureMechanism"/> to save to.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="entity"/> is not of type <see cref="FailureMechanismType.PipingFailureMechanism"/>.</exception>
        public void LoadModel(FailureMechanismEntity entity, PipingFailureMechanism model)
        {
            ConvertEntityToModel(entity, model);
            if (entity.FailureMechanismType != (int) FailureMechanismType.PipingFailureMechanism)
            {
                throw new ArgumentException("Incorrect modelType", "entity");
            }
        }

        /// <summary>
        /// Ensures that the model is added as <see cref="FailureMechanismEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="FailureMechanismEntity"/> objects can be added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="PipingFailureMechanism"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        public void InsertModel(ICollection<FailureMechanismEntity> parentNavigationProperty, PipingFailureMechanism model)
        {
            base.InsertModel(parentNavigationProperty, model);
        }

        /// <summary>
        /// Ensures that the <paramref name="model"/> is set as <see cref="FailureMechanismEntity"/> in the <paramref name="parentNavigationProperty"/>.
        /// </summary>
        /// <param name="parentNavigationProperty">Collection where <see cref="FailureMechanismEntity"/> objects can be searched and added. Usually, this collection is a navigation property of a <see cref="IDbSet{TEntity}"/>.</param>
        /// <param name="model"><see cref="PipingFailureMechanism"/> to be saved in the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="parentNavigationProperty"/> is <c>null</c>.</item>
        /// <item><paramref name="model"/> is <c>null</c>.</item>
        /// </list></exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="parentNavigationProperty"/> is read-only.</exception>
        /// <exception cref="EntityNotFoundException">Thrown when the storageId of <paramref name="model"/> &gt; 0 and: <list type="bullet">
        /// <item>More than one element found in <paramref name="parentNavigationProperty"/> that should have been unique.</item>
        /// <item>No such element exists in <paramref name="parentNavigationProperty"/>.</item>
        /// </list></exception>
        public void UpdateModel(ICollection<FailureMechanismEntity> parentNavigationProperty, PipingFailureMechanism model)
        {
            base.UpdateModel(parentNavigationProperty, model);
        }
    }
}